using System;
using System.IO;
using System.Text;

#if LOGS

namespace SerializIt.Generator;

internal static class Log
{
    private static readonly string LogFile = @"D:\serializit.log";

    public static void Debug(string message, params object[] @params)
    {
        File.AppendAllText(LogFile, $"{DateTime.Now:dd.MM.yyyy hh:mm:ss} [DBG] {string.Format(message, @params)}{Environment.NewLine}");
    }
    public static void Fatal(Exception? ex, string message, params object[] @params)
    {
        if (ex == null)
        {
            File.AppendAllText(LogFile, $"{DateTime.Now:dd.MM.yyyy hh:mm:ss} [FTL] {string.Format(message, @params)}{Environment.NewLine}");
            return;
        }
        StringBuilder sb = new();
        BuildExceptionString(sb, ex);

        File.AppendAllText(LogFile, $"{DateTime.Now:dd.MM.yyyy hh:mm:ss} [FTL] {string.Format(message, @params)}{Environment.NewLine}{sb}{Environment.NewLine}");
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

#endif
