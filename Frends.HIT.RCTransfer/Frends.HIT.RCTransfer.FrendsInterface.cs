using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Frends.HIT.RCTransfer;

/// <summary>
/// The main class for RCTransfer
/// </summary>
public static class FrendsInterface
{
    /// <summary>
    /// Ensure that the given remotes exist in the rclone configuration
    /// </summary>
    /// <param name="remoteList">List of remotes to check on the rclone server</param>
    /// <param name="connectionSettings">The connection settings for the rclone server</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Remote doesn't exist and can't be updated because CreateIfNotExists is false</exception>
    public static async Task<bool> SetRemotes([PropertyTab] Remotes.RemoteInputList remoteList,
        [PropertyTab] Client.ConnectionSettings connectionSettings)
    {
        var rcli = new Client.RcloneClient(connectionSettings);
        var allRemotes = await Helpers.ListRemotes(rcli);

        foreach (var remote in remoteList.Remotes)
        {
            Actions.CallBase call = new Actions.CallBase();
            if (remoteList.UpdateIfExists && allRemotes.Remotes.Contains(remote.Name))
            {
                call = new Actions.ConfigUpdate()
                {
                    Name = remote.Name,
                    Parameters = remote.Config
                };
            }
            else if (remoteList.CreateIfNotExists)
            {
                call = new Actions.ConfigCreate()
                {
                    Name = remote.Name,
                    Type = remote.RemoteType.ToString().ToLower(),
                    Parameters = remote.Config
                };
            }
            else if (!allRemotes.Remotes.Contains(remote.Name))
            {
                throw new ArgumentException("Remote does not exist and CreateIfNotExists is false");
            }

            await call.MakeRequest(rcli);
        }

        return true;
    }

    public static async Task<Responses.Response> CopyFile([PropertyTab] RemoteFSInterface.RemoteFsTransferInput config,
        [PropertyTab] RemoteFSInterface.CopyFileInput input, [PropertyTab] Client.ConnectionSettings connectionSettings)
    {
        var rcli = new Client.RcloneClient(connectionSettings);
        
        var source = config.GetSource();
        var destination = config.GetDestination();
        
        await Helpers.UpsertRemoteFSRemote(rcli, source);
        await Helpers.UpsertRemoteFSRemote(rcli, destination);

        var fmtSource = source.GetFormattedPath(input.SourcePath);
        var fmtDestination = destination.GetFormattedPath(input.DestinationPath);
        
        Actions.OperationsCopyfile copyReq = new Actions.OperationsCopyfile()
        {
            SourceRemote = fmtSource.Remote,
            SourcePath = fmtSource.Path,
            DestinationRemote = fmtDestination.Remote,
            DestinationPath = fmtDestination.Path
        };

        return await copyReq.MakeRequest(rcli);
    }

    public class ErrorLogger
    {
        public RemoteFSInterface.RemoteFSServer SourceServer { get; set; }
        public RemoteFSInterface.RemoteFSServer DestinationServer { get; set; }
        public RemoteFSInterface.RemoteFSServer.FormattedRemotePath SourcePath { get; set; }
        public RemoteFSInterface.RemoteFSServer.FormattedRemotePath DestinationPath { get; set; }
        public string ErrorMessage { get; set; }
    }

    public static async Task<Responses.ListResponse> ListFolder(
        RemoteFSInterface.RemoteFsServerInput config,
        RemoteFSInterface.ListInput input, Client.ConnectionSettings connectionSettings
    )
    {
        var rcli = new Client.RcloneClient(connectionSettings);
        var errorLogger = new ErrorLogger();
    
        try
        {
            var server = config.GetServer();
            errorLogger.SourceServer = (RemoteFSInterface.RemoteFSServer)Helpers.StripPasswords(server);
    
            await Helpers.UpsertRemoteFSRemote(rcli, server);
    
            var fmtPaths = server.GetFormattedPath(input.Path);

            Actions.OperationsList req = new Actions.OperationsList()
            {
                Remote = fmtPaths.Remote,
                Path = fmtPaths.Path
            };
            
            var resp = await req.MakeRequest(rcli);

            return (Responses.ListResponse)resp;
        }
        catch (Exception er)
        {
            errorLogger.ErrorMessage = er.Message;
            throw new Exception(JsonConvert.SerializeObject(errorLogger));
        }
    }
    
    public static async Task<Responses.SyncResponse> SyncFolders( RemoteFSInterface.RemoteFsTransferInput config, 
       RemoteFSInterface.SyncFolderInput input, Client.ConnectionSettings connectionSettings)
    {
        var rcli = new Client.RcloneClient(connectionSettings);
        var errorLogger = new ErrorLogger();

        try
        {
            var source = config.GetSource();
            var destination = config.GetDestination();

            errorLogger.SourceServer = (RemoteFSInterface.RemoteFSServer)Helpers.StripPasswords(source);
            errorLogger.DestinationServer = (RemoteFSInterface.RemoteFSServer)Helpers.StripPasswords(destination);
            
            await Helpers.UpsertRemoteFSRemote(rcli, source);
            await Helpers.UpsertRemoteFSRemote(rcli, destination);
        
            var fmtSource = source.GetFormattedPath(input.SourcePath, true);
            var fmtDestination = destination.GetFormattedPath(input.DestinationPath, true);

            errorLogger.SourcePath = fmtSource;
            errorLogger.DestinationPath = fmtDestination;
            
            Actions.SyncSync syncReq = new Actions.SyncSync()
            {
                Async = true,
                SourceRemoteString = fmtSource.Remote,
                DestinationRemoteString = fmtDestination.Remote,
                SyncEmptyDirectories = input.CreateEmptyDirectories
            };

            var asyncReq = await syncReq.MakeAsyncRequest(rcli);
            await Task.Delay(2000);

            Responses.JobStatusResponse jobStatus = await rcli.GetJobStatus(asyncReq);
            while (jobStatus.Finished == false)
            {
                await Task.Delay(5000);
                jobStatus = await rcli.GetJobStatus(asyncReq);
            }

            Responses.SyncResponse resp = new Responses.SyncResponse()
            {
                JobStatus = jobStatus
            };

            resp.Statistics = await rcli.GetCallStats(asyncReq.JobId);
        
            return resp;
        }
        catch (Exception er)
        {
            errorLogger.ErrorMessage = er.Message;
            throw new Exception(JsonConvert.SerializeObject(errorLogger));
        }
        
        
    }
    

    public static async Task<Responses.Response> RunCommand([PropertyTab] Actions.ActionParams parameters,
        [PropertyTab] Client.ConnectionSettings connectionSettings
    )
    {
        var rcli = new Client.RcloneClient(connectionSettings);
        Responses.Response response = await parameters.ActionConfig.MakeRequest(rcli);
        
        return response;
    }
}