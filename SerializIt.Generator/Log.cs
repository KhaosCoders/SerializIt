using System;
using System.IO;
using System.Text;

#if LOGS
#pragma warning disable RS1035 // Do not use APIs banned for analyzers

namespace SerializIt.Generator;

internal static class Log
{
    private static readonly string? logFile = Environment.GetEnvironmentVariable("SERIALIZIT_LOG");

    public static void Debug(string message)
    {
        if (logFile == null) return;
        File.AppendAllText(logFile, $"{DateTime.Now:dd.MM.yyyy hh:mm:ss} [DBG] {message}{Environment.NewLine}");
    }

    public static void Warn(string message)
    {
        if (logFile == null) return;
        File.AppendAllText(logFile, $"{DateTime.Now:dd.MM.yyyy hh:mm:ss} [WRN] {message}{Environment.NewLine}");
    }

    public static void Fatal(Exception? ex, string message)
    {
        if (logFile == null) return;
        if (ex == null)
        {
            File.AppendAllText(logFile, $"{DateTime.Now:dd.MM.yyyy hh:mm:ss} [FTL] {message}{Environment.NewLine}");
            return;
        }
        StringBuilder sb = new();
        BuildExceptionString(sb, ex);

        File.AppendAllText(logFile, $"{DateTime.Now:dd.MM.yyyy hh:mm:ss} [FTL] {message}{Environment.NewLine}{sb}{Environment.NewLine}");
    }

    private static void BuildExceptionString(StringBuilder sb, Exception ex)
    {
        sb.Append(ex.GetType().Name)
            .Append(": ")
            .Append(ex.Message)
            .Append(' ')
            .AppendLine(ex.StackTrace);
        if (ex.InnerException != null)
        {
            sb.AppendLine("--- InnerException: ");
            BuildExceptionString(sb, ex.InnerException);
        }
    }
}

#pragma warning restore RS1035 // Do not use APIs banned for analyzers
#endif
