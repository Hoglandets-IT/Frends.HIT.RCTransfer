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

    public class EnvironmentResponse
    {
        public string EnvironmentInput { get; set; }
        public string EnvironmentGeneral { get; set; }
    }

    public static EnvironmentResponse GetEnvironment(object testenv)
    {
        return new EnvironmentResponse()
        {
            EnvironmentInput = JsonConvert.SerializeObject(testenv),
            EnvironmentGeneral = JsonConvert.SerializeObject(Environment.GetEnvironmentVariables())
        };
    }

    public static async Task<Responses.SyncResponse> SyncFolders(
        [PropertyTab] RemoteFSInterface.RemoteFsTransferInput config, [PropertyTab] RemoteFSInterface.SyncFolderInput input,
        [PropertyTab] Client.ConnectionSettings connectionSettings)
    {
        var rcli = new Client.RcloneClient(connectionSettings);

        var source = config.GetSource();
        var destination = config.GetDestination();
        
        await Helpers.UpsertRemoteFSRemote(rcli, source);
        await Helpers.UpsertRemoteFSRemote(rcli, destination);
        
        var fmtSource = source.GetFormattedPath(input.SourcePath);
        var fmtDestination = destination.GetFormattedPath(input.DestinationPath);

        Actions.SyncSync syncReq = new Actions.SyncSync()
        {
            Async = true,
            SourceRemoteString = Helpers.JoinPath("/", fmtSource.Remote, fmtSource.Path),
            DestinationRemoteString = Helpers.JoinPath("/", fmtDestination.Remote, fmtDestination.Path),
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
    

    public static async Task<Responses.Response> RunCommand([PropertyTab] Actions.ActionParams parameters,
        [PropertyTab] Client.ConnectionSettings connectionSettings
    )
    {
        var rcli = new Client.RcloneClient(connectionSettings);
        Responses.Response response = await parameters.ActionConfig.MakeRequest(rcli);
        
        return response;
    }
}