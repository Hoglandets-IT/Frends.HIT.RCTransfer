using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic;
using Newtonsoft.Json;

namespace Frends.HIT.RCTransfer;

public class RemoteFSInterface
{
    public class RemoteFSServer
    {
        [JsonIgnore]
        public string Name {
            get
            {
                return Strings.Join(new string[] { ConnectionType, Address, Username }, "-").Replace(".", "_").Replace(":", "").ToLower();
            }
            set {}
        }

        public string ConnectionType { get; set; }
        
        [JsonProperty("connectiontype")]
        public string ConnectionTypeJson
        {
            get
            {
                return ConnectionType.ToLower();
            }
            set
            {
                ConnectionType = value.ToLower();
            }
        }

        [JsonProperty("address")]
        public string Address { get; set; }
        
        [JsonProperty("username")]
        public string Username { get; set; }
        
        [JsonProperty("password")]
        public string Password { get; set; }
        
        [JsonProperty("domain")]
        public string Domain { get; set; }

        public Remotes.RemoteParams GetRemoteParams()
        {
            if (ConnectionType.ToLower() == "smb")
            {
                var parameters = new Remotes.SmbParams()
                {
                    Address = Address,
                    Username = Username,
                    Password = Password
                };
                if (!string.IsNullOrEmpty(Domain))
                {
                    parameters.Domain = Domain;
                }

                return parameters;
            }
            else if (ConnectionType.ToLower() == "sftp")
            {
                var parameters = new Remotes.SftpParams()
                {
                    Host = Address,
                    User = Username,
                    Pass = Password
                };
                if (parameters.Host.Contains(":"))
                {
                    var splitHost = parameters.Host.Split(":");
                    parameters.Host = splitHost[0];
                    parameters.Port = splitHost[1];
                }

                return parameters;
            }
            else if (ConnectionType.ToLower() == "ftp")
            {
                var parameters = new Remotes.FtpParams()
                {
                    Host = Address,
                    Username = Username,
                    Password = Password
                };
                if (parameters.Host.Contains(":"))
                {
                    var splitHost = parameters.Host.Split(":");
                    parameters.Host = splitHost[0];
                    parameters.Port = splitHost[1];
                }

                return parameters;
            }
            
            throw new ArgumentException("Invalid connection type for this integration (want: smb, sftp, ftp; got: " + ConnectionType + ")");
        }

        public string GetName()
        {
            return Strings.Join(new string[] { ConnectionType, Address, Username }, "-").Replace(".", "_").Replace(":", "").ToLower();

        }
        
        public class FormattedRemotePath
        {
            public string Remote { get; set; }
            public string Path { get; set; }
        }

        public FormattedRemotePath GetFormattedPath(string path, bool combined = false)
        {
            FormattedRemotePath returnVal = new FormattedRemotePath(); 
            
            if (ConnectionType.ToLower() == "smb" && path.Contains('$'))
            {
                var splitPath = path.Split("$");

                returnVal.Remote = GetName() + ":" + splitPath[0] + "$/";
                returnVal.Path = splitPath[1];
            }
            else
            {
                returnVal.Remote = GetName() + ":";
                returnVal.Path = path;
            }

            if (!combined)
            {
                return returnVal;
            }
            
            if (ConnectionType.ToLower() == "smb" && path.Contains('$'))
            {
                returnVal.Remote = GetName() + ":" + path;
            }
            else
            {
                returnVal.Remote = returnVal.Remote + returnVal.Path;    
            }

            return returnVal;

        }
        
        public Remotes.RemoteInput GetRemoteInput()
        {

             var input = new Remotes.RemoteInput()
             {
                 Name = GetName()
             };

             var cParams = GetRemoteParams();

             if (cParams.GetType() == typeof(Remotes.SmbParams))
             {
                 input.Smb = (Remotes.SmbParams)cParams;
                 input.RemoteType = Remotes.RemoteTypes.SMB;
             }
             else if (cParams.GetType() == typeof(Remotes.SftpParams))
             {
                 input.Sftp = (Remotes.SftpParams)cParams;
                 input.RemoteType = Remotes.RemoteTypes.SFTP;
             }
             else if (cParams.GetType() == typeof(Remotes.FtpParams))
             {
                 input.Ftp = (Remotes.FtpParams)cParams;
                 input.RemoteType = Remotes.RemoteTypes.FTP;
             }
             else
             {
                 throw new ArgumentException("Invalid connection type for this integration (want: smb, sftp, ftp; got: " + ConnectionType + ")");
             }

             return input;
         }
    }

    public class RemoteFsServerInput
    {
        [DisplayFormat(DataFormatString = "Expression")]
        public string Server { get; set; }
        
        public RemoteFSServer GetServer()
        {
            return JsonConvert.DeserializeObject<RemoteFSServer>(Server);
        }
    }
    
    public class RemoteFsTransferInput
    {
        /// <summary>
        /// The source server to fetch information from
        /// </summary>
        [DisplayFormat(DataFormatString = "Expression")]
        public string Source { get; set; }
        
        /// <summary>
        /// The destination server to push information to
        /// </summary>
        [DisplayFormat(DataFormatString = "Expression")]
        public string Destination { get; set; }

        public RemoteFSServer GetSource()
        {
            return JsonConvert.DeserializeObject<RemoteFSServer>(Source);
        }

        public RemoteFSServer GetDestination()
        {
            return JsonConvert.DeserializeObject<RemoteFSServer>(Destination);
        }
        
    }

    public class CopyFileInput
    {
        public string SourcePath { get; set; }
        public string DestinationPath { get; set; }
    }

    public class SyncFolderInput
    {
        public bool Async { get; set; }
        public string SourcePath { get; set; }
        public string DestinationPath { get; set; }
        public bool CreateEmptyDirectories { get; set; }
    }

    public class SyncMoveFolderInput
    {
        public bool Async { get; set; }
        public string SourcePath { get; set; }
        public string DestinationPath { get; set; }
        public bool CreateEmptyDirectories { get; set; }
        public bool DeleteEmptySourceDirectories { get; set; }
    }

    public class ListInput
    {
        public string Path { get; set; }
    }

}