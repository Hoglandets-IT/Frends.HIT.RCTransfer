using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Frends.HIT.RCTransfer;

public static class Remotes
{

    public enum RemoteTypes
    {
        /// <summary>
        /// Transfer to/from FTP Server
        /// </summary>
        [Display(Name = "FTP Server")]
        [EnumMember(Value = "ftp")]
        FTP,

        /// <summary>
        /// Transfer to/from SMB/Windows Share
        /// </summary>
        [Display(Name = "SMB/SAMBA")]
        [EnumMember(Value = "smb")]
        SMB,

        /// <summary>
        /// Transfer or post files to http url
        /// </summary>
        [Display(Name = "HTTP URL")]
        [EnumMember(Value = "http")]
        HTTP,

        /// <summary>
        /// Transfer to/from SFTP Server
        /// </summary>
        [Display(Name = "SFTP Server")]
        [EnumMember(Value = "sftp")]
        SFTP
    }

    public class RemoteParams { }


    /// <summary>
    /// Parameters for FTP Remote
    /// </summary>
    public class FtpParams : RemoteParams
    {
        [JsonProperty("host", NullValueHandling = NullValueHandling.Ignore)]
        public string Host { get; set; }
        [JsonProperty("port", NullValueHandling = NullValueHandling.Ignore)]
        public string Port { get; set; }
        [JsonProperty("username", NullValueHandling = NullValueHandling.Ignore)]
        public string Username { get; set; }
        [JsonProperty("password", NullValueHandling = NullValueHandling.Ignore)]
        public string Password { get; set; }
    }

    /// <summary>
    /// Parameters for SFTP Remote
    /// </summary>
    public class SftpParams : RemoteParams
    {
        [JsonProperty("host", NullValueHandling = NullValueHandling.Ignore)]
        public string Host { get; set; }
        [JsonProperty("port", NullValueHandling = NullValueHandling.Ignore)]
        public string Port { get; set; }
        [JsonProperty("user", NullValueHandling = NullValueHandling.Ignore)]
        public string User { get; set; }
        [JsonProperty("pass", NullValueHandling = NullValueHandling.Ignore)]
        public string Pass { get; set; }
        [JsonProperty("key-pem", NullValueHandling = NullValueHandling.Ignore)]
        public string KeyPem { get; set; }
    }

    /// <summary>
    /// Parameters for SMB/Windows Share
    /// </summary>
    public class SmbParams : RemoteParams
    {
        [JsonProperty("domain", NullValueHandling = NullValueHandling.Ignore)]
        public string Domain { get; set; }
        [JsonProperty("user", NullValueHandling = NullValueHandling.Ignore)]
        public string Username { get; set; }
        [JsonProperty("pass", NullValueHandling = NullValueHandling.Ignore)]
        public string Password { get; set; }
        [JsonProperty("host", NullValueHandling = NullValueHandling.Ignore)]
        public string Address { get; set; }
    }

    /// <summary>
    /// Parameters for HTTP/ss
    /// </summary>
    public class HttpParams : RemoteParams
    {
        [JsonProperty("configjson", NullValueHandling = NullValueHandling.Ignore)]
        public string Configjson { get; set; }

        [JsonProperty("http-url", NullValueHandling = NullValueHandling.Ignore)]
        public string HttpUrl { get; set; }
    }

    /// <summary>
    /// Configuration for a given remote
    /// </summary>
    public class RemoteInput
    {
        /// <summary>
        /// The name of the remote, used to uniquely identify it in other operations
        /// </summary>
        /// <value></value>
        [Display(Name = "Remote Name (server_user)")]
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The type of the remote
        /// </summary>
        [Display(Name = "Remote Type")]
        [JsonProperty("type")]
        [DefaultValue(null)]
        public RemoteTypes? RemoteType { get; set; }

        /// <summary>
        /// The configuration for the given remote
        /// </summary>
        /// <value></value>
        [Display(Name = "Remote Configuration")]
        [UIHint(nameof(Name), "", "manualconfiguration")]
        [JsonProperty("parameters")]
        public RemoteParams Config { get; set; }

        /// <summary>
        /// Additional Options
        /// </summary>
        /// <value></value>    
        [Display(Name = "Additional Options")]
        [JsonProperty("opt", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, bool> Options { get; set; }

         /// <summary>
        /// Configuration for the FTP Remote
        /// </summary>
        /// <value></value>
        [Display(Name = "Remote Configuration")]
        [UIHint(nameof(RemoteTypes), "", RemoteTypes.FTP)]
        [JsonIgnore]
        public FtpParams? Ftp
        {
            set
            {
                if (value != null && RemoteType == RemoteTypes.FTP)
                {
                    Config = value;
                }
            }
        }

        /// <summary>
        /// Configuration for the Sftp Remote
        /// </summary>
        /// <value></value>
        [Display(Name = "Remote Configuration")]
        [UIHint(nameof(RemoteType), "", RemoteTypes.SFTP)]
        [JsonIgnore]
        public SftpParams? Sftp
        {
            set
            {
                if (value != null && RemoteType == RemoteTypes.SFTP)
                {
                    Config = value;
                }
            }
        }

        /// <summary>
        /// Configuration for the HTTP Remote
        /// </summary>
        /// <value></value>
        [Display(Name = "Remote Configuration")]
        [UIHint(nameof(RemoteType), "", RemoteTypes.HTTP)]
        [JsonIgnore]
        public HttpParams? Http
        {
            set
            {
                if (value != null && RemoteType == RemoteTypes.HTTP)
                {
                    Config = value;
                }
            }
        }

        /// <summary>
        /// Configuration for the SMB Remote
        /// </summary>
        /// <value></value>
        [Display(Name = "Remote Configuration")]
        [UIHint(nameof(RemoteType), "", RemoteTypes.SMB)]
        [JsonIgnore]
        public SmbParams? Smb
        {
            set
            {
                if (value != null && RemoteType == RemoteTypes.SMB)
                {
                    Config = value;
                }
            }
        }
    }
    public class RemoteInputList
    {
        /// <summary>
        /// The remotes you want to add/update on the Rclone Server
        /// </summary>
        /// <value></value>
        [Display(Name = "Remotes")]
        public RemoteInput[] Remotes { get; set; }

        /// <summary>
        /// Create a new remote if one with the same name doesn't already exist
        /// </summary>
        /// <value></value>
        [Display(Name = "Create if not exists")]
        [DefaultValue(true)]
        public bool CreateIfNotExists { get; set; }

        /// <summary>
        /// Update a remote automatically if it exists with the same name
        /// </summary>
        /// <value></value>
        [Display(Name = "Update if exists")]
        [DefaultValue(true)]
        public bool UpdateIfExists { get; set; }
    }
}
