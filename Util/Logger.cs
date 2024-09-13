using System;
using System.IO;
using System.Diagnostics;

namespace MountandShardblade.Util
{
    public enum LogSeverity
    {
        Info,
        Warning,
        Error,
        Critical
    }

    public class Logger
    {
        private static Logger? _instance;
        private static string _logFilePath = "log.txt";  // Default log file
        private static readonly object _lockObj = new object();

        // Private constructor to prevent instantiation
        private Logger()
        {
            // Ensure directory exists for the default log file
            string logDirectory = Path.GetDirectoryName(_logFilePath);
            if (!string.IsNullOrWhiteSpace(logDirectory) && !Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
        }

        // Singleton pattern to ensure a single Logger instance
        public static Logger Instance()
        {
            if (_instance == null)
            {
                _instance = new Logger();
                _instance.Log("Logger initialized.", LogSeverity.Info);
            }
            return _instance;
        }

        // Method to set the log file path if needed
        public static void SetLogFilePath(string filePath)
        {
            lock (_lockObj)
            {
                _logFilePath = filePath;
                Instance().Log($"Log file path set to: {_logFilePath}", LogSeverity.Info);
            }
        }

        // Log a message with optional severity
        public void Log(string message, LogSeverity severity = LogSeverity.Info)
        {
            string logMessage = FormatLogMessage(message, severity);

            lock (_lockObj)
            {
                try
                {
                    // Ensure the file and directory are writable
                    File.AppendAllText(_logFilePath, logMessage + Environment.NewLine);
                }
                catch (UnauthorizedAccessException uex)
                {
                    Console.WriteLine($"[ERROR] Unauthorized access while logging: {uex.Message}");
                }
                catch (DirectoryNotFoundException dex)
                {
                    Console.WriteLine($"[ERROR] Directory not found while logging: {dex.Message}");
                }
                catch (IOException ioex)
                {
                    Console.WriteLine($"[ERROR] IO exception during logging: {ioex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Logging failed: {ex.Message}");
                }
            }
        }

        // Log method entry with method name
        public void LogMethodEntry([System.Runtime.CompilerServices.CallerMemberName] string methodName = "")
        {
            Log($"Entering method: {methodName}", LogSeverity.Info);
        }

        // Log method exit with method name
        public void LogMethodExit([System.Runtime.CompilerServices.CallerMemberName] string methodName = "")
        {
            Log($"Exiting method: {methodName}", LogSeverity.Info);
        }

        // Log exceptions with a specific method name
        public void LogException(Exception ex, [System.Runtime.CompilerServices.CallerMemberName] string methodName = "")
        {
            string exceptionMessage = $"Exception in method: {methodName}. Message: {ex.Message}. StackTrace: {ex.StackTrace}";
            Log(exceptionMessage, LogSeverity.Error);
        }

        // Helper to format log messages with timestamp and severity
        private string FormatLogMessage(string message, LogSeverity severity)
        {
            return $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{severity}] {message}";
        }

        // Method to test logging functionality
        public void TestLogging()
        {
            Log("Logger Test: Logging system is operational.", LogSeverity.Info);
            Log($"Current log file path: {_logFilePath}", LogSeverity.Info);
        }
    }
}
