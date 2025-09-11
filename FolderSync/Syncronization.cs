
namespace FolderSync
{
    public class Comparator
    {
        public static ComparisonResult Compare(ScanResult source, ScanResult replica)
        {
            /* Return:
                * list of files that are in source but not in replica folder
                * list of files that are in replica but not in source folder
                * list of folders that are in source but not in replica folder
                * list of folders that are in replica but not in source folder
            */
            return new ComparisonResult
            {
                FilesOnlyInSource = [.. source.Files.Except(replica.Files)],
                FilesOnlyInReplica = [.. replica.Files.Except(source.Files)],
                DirsOnlyInSource = [.. source.Directories.Except(replica.Directories)],
                DirsOnlyInReplica = [.. replica.Directories.Except(source.Directories)]
            };
        }
    }

    public class ComparisonResult
    {
        public List<HashedFile> FilesOnlyInSource { get; set; } = [];
        public List<HashedFile> FilesOnlyInReplica { get; set; } = [];
        public List<string> DirsOnlyInSource { get; set; } = [];
        public List<string> DirsOnlyInReplica { get; set; } = [];
    }


    public class Syncronizator
    {
        public static void Sync(ComparisonResult comparisonResult,
            ReadOnlyFolder sourceFolder,
            WritableFolder replicaFolder,
            Logger logger)
        {
            /*
                * FilesOnlyInSource have to be created on replicaFolder
                * FilesOnlyInReplica have to be deleted on replicaFolder
                * DirsOnlyInSource have to be created on replicaFolder
                * DirsOnlyInReplica have to be deleted on replicaFolder
            */
            SyncDirsOnlyInSource(comparisonResult, replicaFolder, logger);
            SyncFilesOnlyInSource(comparisonResult, sourceFolder, replicaFolder, logger);
            SyncFilesOnlyInReplica(comparisonResult, replicaFolder, logger);
            SyncDirsOnlyInReplica(comparisonResult, replicaFolder, logger);

        }

        // Copy files that exist only in the source
        private static void SyncFilesOnlyInSource(
            ComparisonResult comparisonResult,
            ReadOnlyFolder sourceFolder,
            WritableFolder replicaFolder,
            Logger logger)
        {
            foreach (var file in comparisonResult.FilesOnlyInSource)
            {
                try
                {
                    var sourceFile = Path.Combine(sourceFolder.RootPath, file.RelativePath);
                    replicaFolder.CopyFile(file.RelativePath, sourceFile);
                    logger.Log($"Copied file: {file.RelativePath}");
                }
                catch (Exception ex)
                {
                    logger.Log($"[ERROR] Copying file {file.RelativePath}: {ex.Message}");
                }
            }
        }

        // Delete files that exist only in the replica
        private static void SyncFilesOnlyInReplica(
            ComparisonResult comparisonResult,
            WritableFolder replicaFolder,
            Logger logger)
        {
            foreach (var file in comparisonResult.FilesOnlyInReplica)
            {
                try
                {
                    replicaFolder.DeleteFile(file.RelativePath);
                    logger.Log($"Deleted file: {file.RelativePath}");
                }
                catch (Exception ex)
                {
                    logger.Log($"[ERROR] Deleting file {file.RelativePath}: {ex.Message}");
                }
            }
        }

        // Create directories that exist only in the source
        private static void SyncDirsOnlyInSource(
            ComparisonResult comparisonResult,
            WritableFolder replicaFolder,
            Logger logger)
        {
            foreach (var dir in comparisonResult.DirsOnlyInSource)
            {
                try
                {
                    replicaFolder.CreateDirectory(dir);
                    logger.Log($"Created directory: {dir}");
                }
                catch (Exception ex)
                {
                    logger.Log($"[ERROR] Creating directory {dir}: {ex.Message}");
                }
            }
        }

        // Delete directories that exist only in the replica
        private static void SyncDirsOnlyInReplica(
            ComparisonResult comparisonResult,
            WritableFolder replicaFolder,
            Logger logger)
        {
            var dirsOnlyInReplica = RemoveRedundantSubdirectories(comparisonResult.DirsOnlyInReplica);
            foreach (var dir in dirsOnlyInReplica)
            {
                try
                {
                    replicaFolder.DeleteDirectory(dir);
                    logger.Log($"Deleted directory: {dir}");
                }
                catch (Exception ex)
                {
                    logger.Log($"[ERROR] Deleting directory {dir}: {ex.Message}");
                }
            }
        }

        private static List<string> RemoveRedundantSubdirectories(List<string> dirs)
        {
            // Normalize and sort to ensure parent directories come before children
            var ordered = dirs
                .Select(d => d.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar))
                .OrderBy(d => d, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var result = new List<string>();

            foreach (var dir in ordered)
            {
                // Check if dir is already under some parent in result
                if (!result.Any(parent =>
                    dir.Length > parent.Length &&
                    dir.StartsWith(parent + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)))
                {
                    result.Add(dir);
                }
            }

            return result;
        }   
    }

}