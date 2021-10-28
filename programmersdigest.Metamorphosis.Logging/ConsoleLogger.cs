using programmersdigest.Metamorphosis.Attributes;
using Newtonsoft.Json;
using System;
using System.Text;

namespace programmersdigest.Metamorphosis.Logging
{
    [Component]
    public abstract class ConsoleLogger
    {
        [Trigger]
        public void Log(object item, LogLevel logLevel = LogLevel.Info)
        {
            switch (item)
            {
                case string message:
                    LogMessage(message, logLevel);
                    break;
                case Exception ex:
                    LogException(ex, logLevel);
                    break;
                default:
                    LogObject(item, logLevel);
                    break;
            }
        }

        private static void LogMessage(string message, LogLevel logLevel)
        {
            Console.Error.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {logLevel}: {message}");
        }

        private static void LogObject(object obj, LogLevel logLevel)
        {
            var message = JsonConvert.SerializeObject(obj);
            LogMessage(message, logLevel);
        }

        private static void LogException(Exception ex, LogLevel logLevel)
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

                var stackTrace = ex.StackTrace?.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                if (stackTrace != null)
                {
                    foreach (var line in stackTrace)
                    {
                        messageBuilder.Append(indent)
                                      .AppendLine(line);
                    }
                }

                ex = ex.InnerException!;
                indentCount += 4;
            } while (ex != null);

            LogMessage(messageBuilder.ToString(), logLevel);
        }
    }
}
