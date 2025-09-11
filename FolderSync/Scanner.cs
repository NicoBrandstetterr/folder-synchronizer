using System.Security.Cryptography;

namespace FolderSync
{
    // Define immutable data-only type to save files paths and hashes.
    // record give "equality by value" so in Comparator class we will just compare them.
    public readonly record struct HashedFile(
        string RelativePath,
        string Hash
    );
    // Define immutable data-only type to save the IEnumerable scan results.
    public readonly record struct ScanResult(
        List<HashedFile> Files,
        List<string> Directories
    );
    public class DirectoryScanner
    {
        public static ScanResult Scan(string rootPath)
        {
            // Scanfiles getting a list of files, which each object has the path and the hash
            // ScanDirectories getting a list of directories
            // Return 2 objects, the list of files and the list of directories.
            return new ScanResult(
                Files: ScanFiles(rootPath),
                Directories: ScanDirectories(rootPath)
            );
        }
        public static List<HashedFile> ScanFiles(string rootPath)
        {
            var option = SearchOption.AllDirectories;
            using var md5 = MD5.Create(); // disposed automatically when scope ends
            var result = new List<HashedFile>();
            foreach (var full in Directory.EnumerateFiles(rootPath, "*", option))
            {
                var rel = Path.GetRelativePath(rootPath, full);
                using var s = File.OpenRead(full); // disposed automatically when scope ends
                var hashBytes = md5.ComputeHash(s);
                var hex = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                result.Add(new HashedFile(rel, hex));
            }
            return result;
        }
        public static List<string> ScanDirectories(string rootPath)
        {
            var result = new List<string>();

            foreach (var dir in Directory.EnumerateDirectories(rootPath, "*", SearchOption.AllDirectories))
            {
                result.Add(Path.GetRelativePath(rootPath, dir));
            }

            return result;
        }

    }
}