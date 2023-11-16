using Newtonsoft.Json;

namespace Frends.HIT.RCTransfer;

public static class Responses {
    public class Response {}

    public class CallStats
    {
        [JsonProperty("bytes", NullValueHandling = NullValueHandling.Ignore)]
        public int Bytes { get; set; }
        
        [JsonProperty("checks", NullValueHandling = NullValueHandling.Ignore)]
        public int Checks { get; set; }
        
        [JsonProperty("deletes", NullValueHandling = NullValueHandling.Ignore)]
        public int Deletes { get; set; }
        
        [JsonProperty("elapsedTime", NullValueHandling = NullValueHandling.Ignore)]
        public float ElapsedTime { get; set; }
        
        [JsonProperty("errors", NullValueHandling = NullValueHandling.Ignore)]
        public int Errors { get; set; }
        
        [JsonProperty("eta", NullValueHandling = NullValueHandling.Ignore)]
        public int Eta { get; set; }
        
        [JsonProperty("fatalError", NullValueHandling = NullValueHandling.Ignore)]
        public bool FatalError { get; set; }
        
        [JsonProperty("lastError", NullValueHandling = NullValueHandling.Ignore)]
        public string LastError { get; set; }
        
        [JsonProperty("renames", NullValueHandling = NullValueHandling.Ignore)]
        public int Renames { get; set; }
        
        [JsonProperty("retryError", NullValueHandling = NullValueHandling.Ignore)]
        public bool RetryError { get; set; }
        
        [JsonProperty("serverSideCopies", NullValueHandling = NullValueHandling.Ignore)]
        public int ServerSideCopies { get; set; }
        
        [JsonProperty("serverSideCopyBytes", NullValueHandling = NullValueHandling.Ignore)]
        public int ServerSideCopyBytes { get; set; }
        
        [JsonProperty("serverSideMoves", NullValueHandling = NullValueHandling.Ignore)]
        public int ServerSideMoves { get; set; }
        
        [JsonProperty("serverSideMoveBytes", NullValueHandling = NullValueHandling.Ignore)]
        public int ServerSideMoveBytes { get; set; }
        
        [JsonProperty("speed", NullValueHandling = NullValueHandling.Ignore)]
        public int Speed  { get; set; }
        
        [JsonProperty("totalBytes", NullValueHandling = NullValueHandling.Ignore)]
        public int TotalBytes { get; set; }
        
        [JsonProperty("totalChecks", NullValueHandling = NullValueHandling.Ignore)]
        public int TotalChecks  { get; set; }
        
        [JsonProperty("totalTransfers", NullValueHandling = NullValueHandling.Ignore)]
        public int TotalTransfers { get; set; }
        
        [JsonProperty("transferTime", NullValueHandling = NullValueHandling.Ignore)]
        public float TransferTime  { get; set; }
        
        [JsonProperty("transfers", NullValueHandling = NullValueHandling.Ignore)]
        public int Transfers { get; set; }
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
    public class ListResponse : Response {}
    public class CreateDirResponse : Response {}

    public class SyncResponse : Response
    {
        public JobStatusResponse JobStatus { get; set; }
        public CallStats Statistics { get; set; }
    }
    
    public class MoveResponse : Response {}
}