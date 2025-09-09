
using System.IO;
namespace FolderSync
{
    public class ArgParser
    {
        public static AppConfig ParseArgs(string[] args)
        {
            // Check if we have exactly 4 arguments
            if (args is null || args.Length != 4)
                throw new ArgumentException("Exactly 4 arguments are required: <source folder> <replica folder> <log file path> <sync interval in seconds>");
            // Validate paths using isValidPath method
            string sourcePath = args[0];
            string replicaPath = args[1];
            string logFilePath = args[2];
            string syncIntervalStr = args[3];
            IsValidPathFormat(sourcePath);
            DirectoryExists(sourcePath);

            IsValidPathFormat(replicaPath);
            // We dont check if the replica path exists, because it will be created if it does not exist

            IsValidPathFormat(logFilePath);
            FileExists(logFilePath);

            // Validate sync interval using isValidSyncInterval method
            int syncInterval = IsValidSyncInterval(syncIntervalStr);
            return new AppConfig(sourcePath, replicaPath, logFilePath, syncInterval);

        }

        // Method to validate path format of a string
        private static void IsValidPathFormat(string path)
        {
            // Check if the path is null or empty
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path is null or empty.");

            // Check if the path includes invalid characters
            if (path.IndexOfAny(Path.GetInvalidPathChars()) != -1)
                throw new ArgumentException($"path includes invalid characters: {path}");
            // check if the path is valid
            try
            {
                Path.GetFullPath(path);
            }
            catch
            {
                throw new ArgumentException($"path is not valid: {path}");
            }
        }
        private static void DirectoryExists(string path)
        {
            // Check if the directory exists
            if (!Directory.Exists(path))
                throw new ArgumentException($"Directory does not exist: {path}");
        }

        private static void FileExists(string path)
        {
            // Check if the file exists
            if (!File.Exists(path))
                throw new ArgumentException($"File does not exist: {path}");
        }
        // Method to validate that sync interval is a positive integer
        private static int IsValidSyncInterval(string intervalStr)
        {
            // Try to parse the string to an integer
            // If parsing fails or the integer is not positive, throw an exception
            // Otherwise, return the parsed integer
            if (!int.TryParse(intervalStr, out int interval) || interval <= 0)
                throw new ArgumentException("Sync interval must be a positive integer.");
            return interval;
        }
    }

}
