
namespace FolderSync
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
                * We need to receive the following arguments:
                * args[0] - source folder path
                * args[1] - replica folder path
                * args[2] - log file path
                * args[3] - Sync interval in seconds
            */

            // test: dotnet run C:\Users\nicol\Documents\trabajo_dotnet_qa\folder-synchronizer\FolderSync\source_folder C:\Users\nicol\Documents\trabajo_dotnet_qa\folder-synchronizer\FolderSync\replica_folder C:\Users\nicol\Documents\trabajo_dotnet_qa\folder-synchronizer\FolderSync\logs\log.log 60
            // Parse and validate arguments
            AppConfig config = ArgParser.ParseArgs(args);
            // Start the synchronization process
            SyncApp.Run(config);

        }
    }
}