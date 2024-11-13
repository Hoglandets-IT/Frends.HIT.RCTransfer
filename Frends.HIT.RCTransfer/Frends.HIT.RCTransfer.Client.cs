using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

using Flurl.Http;

namespace Frends.HIT.RCTransfer;

public static class Client
{

    /// <summary>
    /// Connection settings for the RClone API
    /// </summary>
    public class ConnectionSettings
    {
        /// <summary>
        /// Connection URL for the RClone API
        /// </summary>
        /// <value>https://rclone.example.com</value>
        [Display(Name = "Rclone REST URL")]
        [DisplayFormat(DataFormatString = "Expression")]
        [DefaultValue("#env.RCLONE.HOST")]
        public string RestUrl { get; set; }

        /// <summary>
        /// Username for the RClone API
        /// </summary>
        /// <value></value>
        [DisplayFormat(DataFormatString = "Expression")]
        [DefaultValue("#env.RCLONE.USER")]
        [Display(Name = "Username")]
        public string Username { get; set; }

        /// <summary>
        /// Password for the RClone API
        /// </summary>
        /// <value></value>
        [Display(Name = "Password")]
        [DisplayFormat(DataFormatString = "Expression")]
        [DefaultValue("#env.RCLONE.PASSWORD")]
        public string Password { get; set; }

        /// <summary>
        /// Connection Timeout for the Rclone API
        /// </summary>
        /// <value></value>
        [Display(Name = "Connection Timeout")]
        [DefaultValue(180)]
        public int ConnectionTimeout { get; set; } = 180;
    }

    /// <summary>
    /// The client for connections to the RClone API
    /// </summary>
    public class RcloneClient
    {
        private string Url { get; set; }
        private string Username { get; set; }
        private string Password { get; set; }
        private int Timeout { get; set; }

        private static readonly HttpClient RClient = new HttpClient();

        /// <summary>
        /// Create a new RClone Client
        /// </summary>
        /// <param name="setting">The connection settings for connection to RClone</param>
        public RcloneClient(ConnectionSettings setting)
        {
            Url = setting.RestUrl;
            Username = setting.Username;
            Password = setting.Password;
            Timeout = setting.ConnectionTimeout;
        }

        /// <summary>
        /// Call a function in the RClone API
        /// </summary>
        /// <param name="Endpoint">The API endpoint to call</param>
        /// <param name="Data">The data to send to the endpoint</param>
        /// <returns></returns>
        public async Task<string> Call(string Endpoint, string Data)
        {
            var fullUrl = Url + Endpoint;
            
            if (Data == null)
            {
                Data = "{}";
            }
            var responseString = "";

            responseString = await fullUrl
            .WithBasicAuth(Username, Password)
            .WithHeader("Content-Type", "application/json")
            .WithHeader("Accept", "application/json")
            .AllowAnyHttpStatus()
            .PostStringAsync(Data)
            .ReceiveString();
            
            // TODO: Catch 404 errors and prettify
            try
            {
                var x = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseString);
                if ((string)x["error"] != "")
                {
                    var errorData = "Error: " + x["error"] + "\n";
                    errorData += "Endpoint: " + Endpoint + "\n";

                    if (!Endpoint.StartsWith("/config"))
                    {
                        errorData += "Call Data: " + Data;    
                    }
                    
                    throw new InvalidProgramException(errorData);
                }
            }
            catch (InvalidProgramException EX)
            {
                throw EX;
            }
            catch { }

            return responseString;
        }

        public async Task<Responses.JobStatusResponse> GetJobStatus(Responses.AsyncResponse jobId)
        {
            var responseString = await Call("/job/status", JsonConvert.SerializeObject(jobId));
            return JsonConvert.DeserializeObject<Responses.JobStatusResponse>(responseString);
        }

        public async Task<Responses.CallStats> GetCallStats(int jobId)
        {
            var responseString = await Call("/core/stats", "{\"group\": \"job/" + jobId + "\"}");
            return JsonConvert.DeserializeObject<Responses.CallStats>(responseString);  
        }
        
        
    }
}
