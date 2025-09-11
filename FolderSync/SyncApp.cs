
namespace FolderSync
{
    public class SyncApp
    {
        public static void Run(AppConfig config)
        {
            // Get source and replica folder objects
            ReadOnlyFolder sourceFolder = new(config.SourceFolder);
            WritableFolder replicaFolder = new(config.ReplicaFolder);

            // get logger object
            Logger logger = new(config.LogFilePath);

            // time interval
            int interval = config.SyncInterval;

            // Wait for the specified sync interval before repeating
            while (true)
            {
                try
                {
                    // Scan the files in the source folder
                    ScanResult sourceScanResult = sourceFolder.EnumerateFilesAndDirectories();

                    // Scan the files in the replica folder
                    ScanResult replicaScanResult = replicaFolder.EnumerateFilesAndDirectories();

                    // Compare the files between source and replica
                    ComparisonResult comparisonResult = Comparator.Compare(sourceScanResult, replicaScanResult);
                    logger.Log("Starting syncronization");
                    // Update the replica folder to match the source folder
                    Syncronizator.Sync(comparisonResult, sourceFolder, replicaFolder, logger);
                    logger.Log("syncronization finished");
                    logger.Log($"waiting {interval} seconds for next syncronization...");
                }
                catch (Exception ex)
                {
                    logger.Log($"[ERROR] Synchronization failed: {ex.Message}");
                }
                // Wait for the specified sync interval before repeating
                Thread.Sleep(interval * 1000);  
            }
            
            
        }
    }
}