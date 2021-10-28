using programmersdigest.Metamorphosis.Attributes;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace programmersdigest.Metamorphosis.Logging
{
    [Component]
    public abstract class FileLogger : IDisposable
    {
        private const string ConfigKeyLogFilename = "Logfile";
        private const string DefaultLogFilename = "log.txt";

        [Signal]
        protected abstract T GetConfigValue<T>(string key, T defaultValue);

        private StreamWriter? _writer;

        public void Init()
        {
            var logFileName = GetConfigValue(ConfigKeyLogFilename, DefaultLogFilename);

            var stream = new FileStream(logFileName, FileMode.Append, FileAccess.Write, FileShare.Read);
            _writer = new StreamWriter(stream);
        }

        public void Dispose()
        {
            _writer?.Dispose();
            GC.SuppressFinalize(this);
        }

        [Trigger]
        public void Log(string message, LogLevel logLevel = LogLevel.Info)
        {
            _writer?.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {logLevel}: {message}");
        }

        [Trigger]
        public void Log(Exception ex, LogLevel logLevel = LogLevel.Error)
        {
            var messageBuilder = new StringBuilder();

            var indentCount = 0;
            do
            {
                var indent = new string(' ', indentCount);

                messageBuilder.Append(indent)
                              .Append(ex.GetType())
                              .Append(": ")
                              .Append(ex.Message)
                              .AppendLine();

                var stackTrace = ex.StackTrace?.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries) ?? Enumerable.Empty<string>();
                foreach (var line in stackTrace)
                {
                    messageBuilder.Append(indent)
                                  .AppendLine(line);
                }

                ex = ex.InnerException!;
                indentCount += 4;
            } while (ex != null);

            Log(messageBuilder.ToString(), logLevel);
        }
    }
}
