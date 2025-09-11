namespace FolderSync
{
    public class Logger(string LogPath)
    {
        public string LogPath { get; } = LogPath;

        public void Log(string message)
        {
            LogToConsole(message);
            LogToFile(message);
        }

        public static void LogToConsole(string message)
        {
            Console.WriteLine(message);
        }

        public void LogToFile(string message)
        {
            using var writer = new StreamWriter(LogPath, append: true);
            writer.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}");
        }

    }
}