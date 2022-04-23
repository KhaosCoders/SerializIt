using System;
using System.IO;

#if LOGS

namespace SerializIt.Generator;

internal static class Log
{
    private static readonly string LogFile = @"D:\serializit.log";

    public static void Debug(string message, params object[] @params)
    {
        File.AppendAllText(LogFile, $"{DateTime.Now:dd.MM.yyyy hh:mm:ss} [DBG] {string.Format(message, @params)}{Environment.NewLine}");
    }
}

#endif
