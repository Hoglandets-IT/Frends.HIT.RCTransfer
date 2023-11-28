using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic;
using Newtonsoft.Json;

namespace Frends.HIT.RCTransfer;

// ReSharper disable IdentifierTypo

public static class Actions
{
    public enum ActionTypes
    {
        /// <summary>
        /// List available remotes in the rclone configuration
        /// </summary>
        [Display(Name = "Remote - List")]
        CONFIGLISTREMOTES,
        
        /// <summary>
        /// Get the configuration for a specific remote
        /// </summary>
        [Display(Name = "Remote - Get")]
        CONFIGGET,

        /// <summary>
        /// Create a new remote in the rclone configuration
        /// </summary>
        [Display(Name = "Remote - Create")]
        CONFIGCREATE,

        /// <summary>
        /// Update an existing remote in the rclone configuration
        /// </summary>
        [Display(Name = "Remote - Update")]
        CONFIGUPDATE,

        /// <summary>
        /// Delete an existing remote from the rclone configuration
        /// </summary>
        [Display(Name = "Remote - Delete")]
        CONFIGDELETE,

        /// <summary>
        /// Copy a file between remotes
        /// </summary>
        [Display(Name = "Operations - Copy File")]
        OPERATIONSCOPYFILE,

        /// <summary>
        /// Download an URL to a remote filesystem
        /// </summary>
        [Display(Name = "Operations - Copy URL")]
        OPERATIONSCOPYURL,

        /// <summary>
        /// Move a file between filesystems (copy + delete)
        /// </summary>
        [Display(Name = "Operations - Move File")]
        OPERATIONSMOVEFILE,

        /// <summary>
        /// Delete a file on a remote filesystem
        /// </summary>
        [Display(Name = "Operations - Delete File")]
        OPERATIONSDELETEFILE,

        /// <summary>
        /// Recursively delete files and folders on a remote filesystem
        /// </summary>
        [Display(Name = "Operations - Recursive Delete")]
        OPERATIONSRMDIRS,

        /// <summary>
        /// List files and folders on a remote filesystem
        /// </summary>
        [Display(Name = "Operations - List Files/Folders")]
        OPERATIONSLIST,

        /// <summary>
        /// Create a directory on a remote filesystem
        /// </summary>
        [Display(Name = "Operations - Create Directory")]
        OPERATIONSMKDIR,

        /// <summary>
        /// Sync files and folders between two remotes (one-way)
        /// </summary>
        [Display(Name = "Transfer - Sync")]
        SYNCSYNC,

        /// <summary>
        /// Move files and folders between two remotes
        /// </summary>
        [Display(Name = "Transfer - Move")]
        SYNCMOVE,
    }

    // public abstract class MakeRequestBase
    // {
    //     public abstract Task<Responses.Response> MakeRequest(Client.RcloneClient client);
    // }

    /// <summary>
    /// The base call object
    /// </summary>
    public class CallBase
    {
        /// <summary>
        /// The bare JSON data object
        /// </summary>
        [JsonIgnore]
        public string JsonData { get; set; } = "";

        /// <summary>
        /// Get the path for the API client to call based on the name of the Call
        /// </summary>
        /// <returns>The name of the API endpoint</returns>
        protected string GetPath()
        {
            return "/" + System.Text.RegularExpressions.Regex.Replace(this.GetType().Name, "(?<=.)([A-Z])", "/$0",
                System.Text.RegularExpressions.RegexOptions.Compiled).ToLowerInvariant();
        }

        /// <summary>
        /// Serialize a call to json
        /// </summary>
        /// <returns></returns>
        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public async Task<Responses.Response> MakeRequest(Client.RcloneClient client)
        {
            var response = await client.Call(this.GetPath(), GetJson());
            return JsonConvert.DeserializeObject<Responses.Response>(response);
        }
    }

    public class SingleRemoteBase : CallBase
    {
        /// <summary>
        /// The remote to use for the call
        /// </summary>
        [JsonProperty("fs", NullValueHandling = NullValueHandling.Ignore)]
        public string Remote { get; set; }

        /// <summary>
        /// The path on the remote filesystem
        /// </summary>
        [JsonProperty("remote")]
        public string Path { get; set; }

        /// <summary>
        /// The combined Remote String (remote name:path
        /// </summary>
        /// <exception cref="Exception">Didn't include a colon at the end of the remote name</exception>
        [JsonIgnore]
        public string RemoteString
        {
            get
            {
                return Strings.Join(new string[] { Remote, Path }, "");
            }
            set
            {
                if (!value.Contains(":"))
                {
                    throw new Exception("RemoteString parameter must include a : separating the path from the remote.");
                }

                var split = value.Split(":");
                Remote = split[0] + ":";
                Path = Strings.Join(split.Skip(1).ToArray(), "");
            }
        }
        
        public new async Task<Responses.Response> MakeRequest(Client.RcloneClient client)
        {
            var response = await client.Call(this.GetPath(), GetJson());
            return JsonConvert.DeserializeObject<Responses.Response>(response);
        }
    }
    
    /// <summary>
    /// The basic class for a source/destination transfer
    /// </summary>
    public class SourceDestinationBase : CallBase
    {
        [JsonProperty("srcFs", NullValueHandling = NullValueHandling.Ignore)]
        public string SourceRemote { get; set; }

        [JsonProperty("srcRemote")]
        public string SourcePath { get; set; }

        [JsonProperty("dstFs", NullValueHandling = NullValueHandling.Ignore)]
        public string DestinationRemote { get; set; }

        [JsonProperty("dstRemote")]
        public string DestinationPath { get; set; }

        [JsonIgnore]
        public string SourceRemoteString
        {
            get
            {
                return Strings.Join(new string[] { SourceRemote, SourcePath }, "");
            }
            set
            {
                if (!value.Contains(":"))
                {
                    throw new Exception("SourceRemoteString parameter must include a : separating the path from the remote.");
                }

                var split = value.Split(":");
                SourceRemote = split[0] + ":";
                SourcePath = Strings.Join(split.Skip(1).ToArray(), "");
            }
        }

        [JsonIgnore]
        public string DestinationRemoteString
        {
            get
            {
                return Strings.Join(new string[] { DestinationRemote, DestinationPath }, "");
            }
            set
            {
                if (!value.Contains(":"))
                {
                    throw new Exception("DestinationRemoteString parameter must include a : separating the path from the remote.");
                }

                var split = value.Split(":");
                DestinationRemote = split[0] + ":";
                DestinationPath = Strings.Join(split.Skip(1).ToArray(), "");
            }
        }
        
        public new async Task<Responses.Response> MakeRequest(Client.RcloneClient client)
        {
            var response = await client.Call(this.GetPath(), GetJson());
            return JsonConvert.DeserializeObject<Responses.Response>(response);
        }
    }
    public class ConfigListremotes : CallBase
    {
        public new async Task<Responses.ListRemoteResponse> MakeRequest(Client.RcloneClient client)
        {
            var response = await client.Call(this.GetPath(), JsonConvert.SerializeObject(this));
            return JsonConvert.DeserializeObject<Responses.ListRemoteResponse>(response);
        }
    }
    public class ConfigCreate : CallBase
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("parameters", NullValueHandling = NullValueHandling.Ignore)]
        public Remotes.RemoteParams Parameters { get; set; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("opt", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, bool> Options { get; set; }
        public new async Task<Responses.CreateRemoteResponse> MakeRequest(Client.RcloneClient client)
        {
            var response = await client.Call(this.GetPath(), JsonConvert.SerializeObject(this));
            return JsonConvert.DeserializeObject<Responses.CreateRemoteResponse>(response);
        }
    }
    public class ConfigDelete : CallBase
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        public new async Task<Responses.DeleteRemoteResponse> MakeRequest(Client.RcloneClient client)
        {
            var response = await client.Call(this.GetPath(), JsonConvert.SerializeObject(this));
            return JsonConvert.DeserializeObject<Responses.DeleteRemoteResponse>(response);
        }
    }
    public class ConfigGet : CallBase
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        public new async Task<Responses.GetRemoteResponse> MakeRequest(Client.RcloneClient client)
        {
            var response = await client.Call(this.GetPath(), JsonConvert.SerializeObject(this));
            return JsonConvert.DeserializeObject<Responses.GetRemoteResponse>(response);
        }
    }
    public class ConfigUpdate : CallBase
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("parameters", NullValueHandling = NullValueHandling.Ignore)]
        public Remotes.RemoteParams Parameters { get; set; }

        [JsonProperty("opt", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, bool> Options { get; set; }
    }
    public class OperationsCopyfile : SourceDestinationBase
    {
        public async static Task<Responses.CopyFileResponse> DeserializeResponse(string jsonData) { return JsonConvert.DeserializeObject<Responses.CopyFileResponse>(jsonData); }

        public new async Task<Responses.CopyFileResponse> MakeRequest(Client.RcloneClient client)
        {
            var response = await client.Call(this.GetPath(), JsonConvert.SerializeObject(this));
            return JsonConvert.DeserializeObject<Responses.CopyFileResponse>(response);
        }

    }
    public class OperationsMovefile : SourceDestinationBase
    {
        public async Task<Responses.MoveFileResponse> MakeRequest(Client.RcloneClient client)
        {
            var response = await client.Call(this.GetPath(), GetJson());
            return JsonConvert.DeserializeObject<Responses.MoveFileResponse>(response);
        }
    }
    public class OperationsCopyurl : SingleRemoteBase
    {
        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; set; }

        [JsonProperty("autoFilename", NullValueHandling = NullValueHandling.Ignore)]
        public bool AutomaticFilename { get; set; }
        public new async Task<Responses.CopyUrlResponse> MakeRequest(Client.RcloneClient client)
        {
            var response = await client.Call(this.GetPath(), GetJson());
            return JsonConvert.DeserializeObject<Responses.CopyUrlResponse>(response);
        }    }
    public class OperationsDeletefile : SingleRemoteBase
    {
        public async Task<Responses.DeleteFileResponse> MakeRequest(Client.RcloneClient client)
        {
            var response = await client.Call(this.GetPath(), GetJson());
            return JsonConvert.DeserializeObject<Responses.DeleteFileResponse>(response);
        }    }
    public class OperationsRmdirs : SingleRemoteBase
    {
        [JsonProperty("leaveRoot")]
        public bool LeaveRoot { get; set; }

        public new async Task<Responses.Response> MakeRequest(Client.RcloneClient client)
        {
            var response = await client.Call(this.GetPath(), GetJson());
            return JsonConvert.DeserializeObject<Responses.Response>(response);
        }    }
    public class OperationsList : SingleRemoteBase
    {
        [JsonProperty("opt", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> ListOptions { get; set; }

        public new async Task<Responses.ListResponse> MakeRequest(Client.RcloneClient client)
        {
            var response = await client.Call(this.GetPath(), GetJson());
            return JsonConvert.DeserializeObject<Responses.ListResponse>(response);

        }
    }
    public class OperationsMkdir : SingleRemoteBase
    {
        public new async Task<Responses.CreateDirResponse> MakeRequest(Client.RcloneClient client)
        {
            var response = await client.Call(this.GetPath(), GetJson());
            return JsonConvert.DeserializeObject<Responses.CreateDirResponse>(response);
        }    
    }
    public class SyncSync : CallBase
    {
        [JsonProperty("_async")] 
        public bool Async { get; set; } = false;
        
        [JsonProperty("srcFs")]
        public string SourceRemoteString { get; set; }

        [JsonProperty("dstFs")]
        public string DestinationRemoteString { get; set; }

        [JsonProperty("createEmptySrcDirs")]
        public bool SyncEmptyDirectories { get; set; }
        
        public new async Task<Responses.AsyncResponse> MakeAsyncRequest(Client.RcloneClient client)
        {
            var response = await client.Call(this.GetPath(), JsonConvert.SerializeObject(this));
            return JsonConvert.DeserializeObject<Responses.AsyncResponse>(response);
        }
        
        public new async Task<Responses.SyncResponse> MakeRequest(Client.RcloneClient client)
        {
            var response = await client.Call(this.GetPath(), JsonConvert.SerializeObject(this));
            return JsonConvert.DeserializeObject<Responses.SyncResponse>(response);
        }
        
    }
    
    public class SyncMove : SyncSync
    {
        [JsonProperty("deleteEmptySrcDirs")]
        public bool DeleteEmptySourceDirectories { get; set; }
        public new async Task<Responses.MoveResponse> MakeRequest(Client.RcloneClient client)
        {
            var response = await client.Call(this.GetPath(), GetJson());
            return JsonConvert.DeserializeObject<Responses.MoveResponse>(response);
        }
        
    }

    public class ActionParams
    {
        [Display(Name = "Action Type")]
        public ActionTypes ActionType { get; set; }

        [Display(Name = "Action Configuration")]
        [UIHint(nameof(ActionType), "", "manualconfiguration")]
        public CallBase ActionConfig { get; set; }

        [Display(Name = "Action Configuration")]
        [UIHint(nameof(ActionType), "", ActionTypes.CONFIGLISTREMOTES)]
        public ConfigListremotes ConfigListremotes
        {
            set
            {
                if (value != null && ActionType == ActionTypes.CONFIGLISTREMOTES)
                {
                    ActionConfig = value;
                }
            }
        }


        [Display(Name = "Action Configuration")]
        [UIHint(nameof(ActionType), "", ActionTypes.CONFIGGET)]
        public ConfigGet ConfigGet
        {
            set
            {
                if (value != null && ActionType == ActionTypes.CONFIGGET)
                {
                    ActionConfig = value;
                }
            }
        }

        [Display(Name = "Action Configuration")]
        [UIHint(nameof(ActionType), "", ActionTypes.CONFIGCREATE)]
        public ConfigCreate ConfigCreate
        {
            set
            {
                if (value != null && ActionType == ActionTypes.CONFIGCREATE)
                {
                    ActionConfig = value;
                }
            }
        }

        [Display(Name = "Action Configuration")]
        [UIHint(nameof(ActionType), "", ActionTypes.CONFIGUPDATE)]
        public ConfigUpdate ConfigUpdate
        {
            set
            {
                if (value != null && ActionType == ActionTypes.CONFIGUPDATE)
                {
                    ActionConfig = value;
                }
            }
        }

        [Display(Name = "Action Configuration")]
        [UIHint(nameof(ActionType), "", ActionTypes.CONFIGDELETE)]
        public ConfigDelete ConfigDelete
        {
            set
            {
                if (value != null && ActionType == ActionTypes.CONFIGDELETE)
                {
                    ActionConfig = value;
                }
            }
        }

        [Display(Name = "Action Configuration")]
        [UIHint(nameof(ActionType), "", ActionTypes.OPERATIONSCOPYFILE)]
        public OperationsCopyfile OperationsCopyfile
        {
            set
            {
                if (value != null && ActionType == ActionTypes.OPERATIONSCOPYFILE)
                {
                    ActionConfig = value;
                }
            }
        }

        [Display(Name = "Action Configuration")]
        [UIHint(nameof(ActionType), "", ActionTypes.OPERATIONSCOPYURL)]
        public OperationsCopyurl OperationsCopyurl
        {
            set
            {
                if (value != null && ActionType == ActionTypes.OPERATIONSCOPYURL)
                {
                    ActionConfig = value;
                }
            }
        }

        [Display(Name = "Action Configuration")]
        [UIHint(nameof(ActionType), "", ActionTypes.OPERATIONSMOVEFILE)]
        public OperationsMovefile OperationsMovefile
        {
            set
            {
                if (value != null && ActionType == ActionTypes.OPERATIONSMOVEFILE)
                {
                    ActionConfig = value;
                }
            }
        }

        [Display(Name = "Action Configuration")]
        [UIHint(nameof(ActionType), "", ActionTypes.OPERATIONSDELETEFILE)]
        public OperationsDeletefile OperationsDeletefile
        {
            set
            {
                if (value != null && ActionType == ActionTypes.OPERATIONSDELETEFILE)
                {
                    ActionConfig = value;
                }
            }
        }

        [Display(Name = "Action Configuration")]
        [UIHint(nameof(ActionType), "", ActionTypes.OPERATIONSRMDIRS)]
        public OperationsRmdirs OperationsRmdirs
        {
            set
            {
                if (value != null && ActionType == ActionTypes.OPERATIONSRMDIRS)
                {
                    ActionConfig = value;
                }
            }
        }

        [Display(Name = "Action Configuration")]
        [UIHint(nameof(ActionType), "", ActionTypes.OPERATIONSLIST)]
        public OperationsList OperationsList
        {
            set
            {
                if (value != null && ActionType == ActionTypes.OPERATIONSLIST)
                {
                    ActionConfig = value;
                }
            }
        }

        [Display(Name = "Action Configuration")]
        [UIHint(nameof(ActionType), "", ActionTypes.OPERATIONSMKDIR)]
        public OperationsMkdir OperationsMkdir
        {
            set
            {
                if (value != null && ActionType == ActionTypes.OPERATIONSMKDIR)
                {
                    ActionConfig = value;
                }
            }
        }

        [Display(Name = "Action Configuration")]
        [UIHint(nameof(ActionType), "", ActionTypes.SYNCSYNC)]
        public SyncSync SyncSync
        {
            set
            {
                if (value != null && ActionType == ActionTypes.SYNCSYNC)
                {
                    ActionConfig = value;
                }
            }
        }

        [Display(Name = "Action Configuration")]
        [UIHint(nameof(ActionType), "", ActionTypes.SYNCMOVE)]
        public SyncMove SyncMove
        {
            set
            {
                if (value != null && ActionType == ActionTypes.SYNCMOVE)
                {
                    ActionConfig = value;
                }
            }
        }
    }
}
