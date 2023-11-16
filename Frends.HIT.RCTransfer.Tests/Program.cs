using Newtonsoft.Json;

namespace Frends.HIT.RCTransfer;

public static class Tests
{
    public static void Main()
    {

        var rfsInterface = new RemoteFSInterface.RemoteFsTransferInput()
        {
            Source = TestSettings.HitSftpConnectionString,
            Destination = TestSettings.HphenixConnectionString
        };

        var xx = FrendsInterface.SyncFolders(
            rfsInterface,
            new RemoteFSInterface.SyncFolderInput()
            {
                SourcePath = "home/fp-test",
                DestinationPath = "IntegrationOutput$/fp-test"
            },
            TestSettings.Connection
        ).GetAwaiter().GetResult();
        
        Console.WriteLine(JsonConvert.SerializeObject(xx));
        
        Console.Write("Finished");

    }
}