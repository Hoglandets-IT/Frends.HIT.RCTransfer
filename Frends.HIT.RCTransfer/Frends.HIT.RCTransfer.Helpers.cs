using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

namespace Frends.HIT.RCTransfer;

internal class Helpers
{
    public static async Task<Responses.ListRemoteResponse> ListRemotes(Client.RcloneClient client)
    {
        var allRemotesCall = new Actions.ConfigListremotes(){};
        var allRemotes = await allRemotesCall.MakeRequest(client);

        return allRemotes;
    }
    
    static string OSDirSeparator = Path.DirectorySeparatorChar.ToString();

    /// <summary>
    /// Checks if the given string is a valid, non-empty/null string
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    static bool IsValidString(string input)
    {
        return !string.IsNullOrEmpty(input) && !string.IsNullOrWhiteSpace(input) && input.Length > 0;
    }

    
    /// <summary>
    /// Joins a list of strings together with a separator (path)
    /// </summary>
    /// <param name="separator"></param>
    /// <param name="parts"></param>
    /// <returns></returns>
    public static string JoinPath(string separator, params string[] parts)
    {
        var cleanParts = new List<string>();
        foreach (var part in parts)
        {
            if (IsValidString(part))
            {
                cleanParts.Add(part);
            }
        }

        string retnString = string.Join(separator, cleanParts);
        
        while (retnString.Contains(separator+separator))
        {
            retnString = retnString.Replace(separator + separator, separator);
        }
        
        return retnString;
    }

    /// <summary>
    /// Join multiple paths together with a given separator
    /// </summary>
    /// <param name="separator">The separator to use</param>
    /// <param name="separateLastPart">Whether to separate the last part out or include it in the string</param>
    /// <param name="parts">The parts that are to be joined</param>
    /// <returns>A joined path</returns>
    public static string[] JoinPathArray(string separator, bool separateLastPart, params string[] parts)
    {
        string joinedPath = JoinPath(separator, parts);
        if (separateLastPart == true)
        {
            var split = joinedPath.Split(separator).ToList();
            var path = split.GetRange(0, split.Count - 1);
            var file = split.GetRange(split.Count - 1, 1);
            return new string[] {string.Join(separator, path), file[0]};
        }

        return new string[] { joinedPath };
    }

    public static async Task UpsertRemoteFSRemote(Client.RcloneClient client, RemoteFSInterface.RemoteFSServer server)
    {
        var allRemotes = await Helpers.ListRemotes(client);

        Actions.CallBase call;
        
        if (!allRemotes.Remotes.Contains(server.Name))
        {
            call = new Actions.ConfigCreate()
            {
                Name = server.Name,
                Type = server.ConnectionType.ToLower(),
                Parameters = server.GetRemoteParams()
            };
        }
        else
        {
            call = new Actions.ConfigUpdate()
            {
                Name = server.Name,
                Parameters = server.GetRemoteParams()
            };
        }

        await call.MakeRequest(client);
    }

    public static object DeepCopy(object input)
    {
        Type obType = input.GetType();
        var serialized = JsonConvert.SerializeObject(input);
        
        return JsonConvert.DeserializeObject(serialized, obType);
    }

    public static object StripPasswords(object input)
    {
        var dco = DeepCopy(input);
        Type obType = input.GetType();
            
        foreach (var prop in obType.GetProperties())
        {
            string prnl = prop.Name.ToLower();
            if (prnl.Contains("pass") || prnl.Contains("key"))
            {
                prop.SetValue(dco, "");
            }
        }

        return dco;
    }

   
}