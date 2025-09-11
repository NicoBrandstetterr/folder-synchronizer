
namespace FolderSync
{
    public interface IReadOnlyFolder
    {
        /*
            * Design interface focused on source folder.
            * Stablish RootPath property read-only.
            * Declares EnumerateFilesAndDirectories method that return a ScanResult object.
                * This method uses the DirectoryScanner class.
        */
        string RootPath { get; }
        ScanResult EnumerateFilesAndDirectories();
    }

    public interface IWritableFolder : IReadOnlyFolder
    {
        /*
            * Design interface focused on replica folder.
            * Inherits from IreadOnlyFolder.
            * Declares CopyFile method that copy a file on a relative path.
            * Declares CreateDirectory method that create a Directory.
            * Declares DeleteFile method that delete a specific file.
            * Declares a DeleteDirectory method that delete a specific directory.
        */
        void CopyFile(string relativePath, string originalFilePath);
        void CreateDirectory(string relativePath);
        void DeleteFile(string relativePath);
        void DeleteDirectory(string relativeDirPath);
    }

    public class ReadOnlyFolder(string rootPath) : IReadOnlyFolder
    {
        public string RootPath { get; } = rootPath;

        public ScanResult EnumerateFilesAndDirectories()
        {
            return DirectoryScanner.Scan(RootPath);
        }
    }

    public class WritableFolder(string rootPath) : ReadOnlyFolder(rootPath), IWritableFolder
    {
        public void CopyFile(string relativePath, string originalFilePath)
        {
            var full = Path.Combine(RootPath, relativePath);

            var parentDir = Path.GetDirectoryName(full);

            if (parentDir == null || !Directory.Exists(parentDir))
            {
                throw new DirectoryNotFoundException($"Parent directory does not exist: {parentDir}");
            }

            File.Copy(originalFilePath, full, overwrite: true);
        }

        public void CreateDirectory(string relativePath)
        {
            var full = Path.Combine(RootPath, relativePath);

            if (Directory.Exists(full))
            {
                throw new Exception($"The folder {full} already exist! can´t be created again.");
            }

            Directory.CreateDirectory(full);
        }

        public void DeleteFile(string relativePath)
        {
            var full = Path.Combine(RootPath, relativePath);
            if (!File.Exists(full))
            {
                throw new FileNotFoundException($"File does not exist: {full}");
            }
            File.Delete(full);
        }

        public void DeleteDirectory(string relativeDirPath)
        {
            var full = Path.Combine(RootPath, relativeDirPath);
            if (!Directory.Exists(full))
            {
                throw new Exception($"The folder {full} doesn´t exist! can´t be deleted.");
            }
            Directory.Delete(full, recursive: true);
        }

    }

    
}