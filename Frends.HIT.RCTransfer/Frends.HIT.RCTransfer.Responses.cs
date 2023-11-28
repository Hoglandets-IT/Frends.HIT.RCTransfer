using Newtonsoft.Json;

namespace Frends.HIT.RCTransfer;

public static class Responses {
    public class Response {}

    public class CallStats
    {
        [JsonProperty("bytes", NullValueHandling = NullValueHandling.Ignore)]
        public UInt64 Bytes { get; set; }
        
        [JsonProperty("checks", NullValueHandling = NullValueHandling.Ignore)]
        public UInt64 Checks { get; set; }
        
        [JsonProperty("deletes", NullValueHandling = NullValueHandling.Ignore)]
        public UInt64 Deletes { get; set; }
        
        [JsonProperty("elapsedTime", NullValueHandling = NullValueHandling.Ignore)]
        public float ElapsedTime { get; set; }
        
        [JsonProperty("errors", NullValueHandling = NullValueHandling.Ignore)]
        public UInt64 Errors { get; set; }
        
        [JsonProperty("eta", NullValueHandling = NullValueHandling.Ignore)]
        public UInt64 Eta { get; set; }
        
        [JsonProperty("fatalError", NullValueHandling = NullValueHandling.Ignore)]
        public bool FatalError { get; set; }
        
        [JsonProperty("lastError", NullValueHandling = NullValueHandling.Ignore)]
        public string LastError { get; set; }
        
        [JsonProperty("renames", NullValueHandling = NullValueHandling.Ignore)]
        public UInt64 Renames { get; set; }
        
        [JsonProperty("retryError", NullValueHandling = NullValueHandling.Ignore)]
        public bool RetryError { get; set; }
        
        [JsonProperty("serverSideCopies", NullValueHandling = NullValueHandling.Ignore)]
        public UInt64 ServerSideCopies { get; set; }
        
        [JsonProperty("serverSideCopyBytes", NullValueHandling = NullValueHandling.Ignore)]
        public UInt64 ServerSideCopyBytes { get; set; }
        
        [JsonProperty("serverSideMoves", NullValueHandling = NullValueHandling.Ignore)]
        public UInt64 ServerSideMoves { get; set; }
        
        [JsonProperty("serverSideMoveBytes", NullValueHandling = NullValueHandling.Ignore)]
        public UInt64 ServerSideMoveBytes { get; set; }
        
        [JsonProperty("speed", NullValueHandling = NullValueHandling.Ignore)]
        public float Speed  { get; set; }
        
        [JsonProperty("totalBytes", NullValueHandling = NullValueHandling.Ignore)]
        public UInt64 TotalBytes { get; set; }
        
        [JsonProperty("totalChecks", NullValueHandling = NullValueHandling.Ignore)]
        public UInt64 TotalChecks  { get; set; }
        
        [JsonProperty("totalTransfers", NullValueHandling = NullValueHandling.Ignore)]
        public UInt64 TotalTransfers { get; set; }
        
        [JsonProperty("transferTime", NullValueHandling = NullValueHandling.Ignore)]
        public float TransferTime  { get; set; }
        
        [JsonProperty("transfers", NullValueHandling = NullValueHandling.Ignore)]
        public UInt64 Transfers { get; set; }
    }
    
    public class AsyncResponse : Response
    {
        [JsonProperty("jobid")]
        public int JobId { get; set; }
    }

    public class JobStatusResponse : Response
    {
        [JsonProperty("duration")]
        public float Duration { get; set; }
        
        [JsonProperty("endTime")]
        public string EndTime { get; set; }
        
        [JsonProperty("error")]
        public string Error { get; set; }
        
        [JsonProperty("finished")]
        public bool Finished { get; set; }
        
        [JsonProperty("id")]
        public bool JobId { get; set; }
        
        [JsonProperty("startTime")]
        public string StartTime { get; set; }
        
        [JsonProperty("success")]
        public bool Success { get; set; }
        
        [JsonProperty("output")]
        public object? Output { get; set; }
    }
    
    public class ListRemoteResponse : Response {
        [JsonProperty("remotes")]
        public List<string> Remotes {get;set;} 
    }
    public class GetRemoteResponse : Response {}
    public class CreateRemoteResponse : Response {}
    public class UpdateRemoteResponse : Response {}
    public class DeleteRemoteResponse : Response {}

    public class CopyFileResponse : Response
    {
        
    }
    public class CopyUrlResponse : Response {}
    public class MoveFileResponse : Response {}
    public class DeleteFileResponse : Response {}
    public class RemoveDirsResponse : Response {}

    public class ListItem
    {
        [JsonProperty("Hashes")]
        public Dictionary<string,string> Hashes { get; set; }
        
        [JsonProperty("ID")]
        public string Id { get; set; }
        
        [JsonProperty("OrigID")]
        public string OrigId { get; set; }
        
        [JsonProperty("IsBucket")]
        public bool IsBucket { get; set; }
        
        [JsonProperty("IsDeleted")]
        public bool IsDir { get; set; }
        
        [JsonProperty("MimeType")]
        public string MimeType { get; set; }
        
        [JsonProperty("ModTime")]
        public string ModTime { get; set; }
        
        [JsonProperty("Name")]
        public string Name { get; set; }
        
        [JsonProperty("Encrypted")]
        public string Encrypted { get; set; }
        
        [JsonProperty("EncryptedPath")]
        public string EncryptedPath { get; set; }
        
        [JsonProperty("Path")]
        public string Path { get; set; }
        
        [JsonProperty("Size")]
        public int Size { get; set; }
        
        [JsonProperty("Tier")]
        public string Tier { get; set; }
    }
    
    public class ListResponse : Response
    {
        [JsonProperty("list")]
        public ListItem[] Items { get; set; }
    }
    public class CreateDirResponse : Response {}

    public class SyncResponse : Response
    {
        public JobStatusResponse JobStatus { get; set; }
        public CallStats Statistics { get; set; }
    }
    
    public class MoveResponse : Response {}
}