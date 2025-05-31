using System;
using System.Reflection;
using Discord;

namespace CustomItems.Core;

internal class Log
{
    public static void Info(object message)
    {
        Send($"[{Assembly.GetCallingAssembly().GetName().Name}] {message}", LogLevel.Info, ConsoleColor.Cyan);
    }

    public static void Info(string message)
    {
        Send("[" + Assembly.GetCallingAssembly().GetName().Name + "] " + message, LogLevel.Info, ConsoleColor.Cyan);
    }

    public static void Debug(object message)
    {
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        if (CustomItemsPlugin.Instance.Config.Debug)
        {
            Send($"[{callingAssembly.GetName().Name}] {message}", LogLevel.Debug, ConsoleColor.Green);
        }
    }

    public static T DebugObject<T>(T @object)
    {
        Debug(@object);
        return @object;
    }

    public static void Debug(string message)
    {
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        if (CustomItemsPlugin.Instance.Config.Debug)
        {
            Send("[" + callingAssembly.GetName().Name + "] " + message, LogLevel.Debug, ConsoleColor.Green);
        }
    }

    public static void Warn(object message)
    {
        Send($"[{Assembly.GetCallingAssembly().GetName().Name}] {message}", LogLevel.Warn, ConsoleColor.Magenta);
    }

    public static void Warn(string message)
    {
        Send("[" + Assembly.GetCallingAssembly().GetName().Name + "] " + message, LogLevel.Warn, ConsoleColor.Magenta);
    }

    public static void Error(object message)
    {
        Send($"[{Assembly.GetCallingAssembly().GetName().Name}] {message}", LogLevel.Error, ConsoleColor.DarkRed);
    }

    public static void Error(string message)
    {
        Send("[" + Assembly.GetCallingAssembly().GetName().Name + "] " + message, LogLevel.Error, ConsoleColor.DarkRed);
    }

    public static void Send(object message, LogLevel level, ConsoleColor color = ConsoleColor.Gray)
    {
        SendRaw($"[{level.ToString().ToUpper()}] {message}", color);
    }

    public static void Send(string message, LogLevel level, ConsoleColor color = ConsoleColor.Gray)
    {
        SendRaw("[" + level.ToString().ToUpper() + "] " + message, color);
    }

    public static void SendRaw(object message, ConsoleColor color)
    {
        ServerConsole.AddLog(message.ToString(), color);
    }

    public static void SendRaw(string message, ConsoleColor color)
    {
        ServerConsole.AddLog(message, color);
    }

    public static void Assert(bool condition, object message)
    {
        if (condition)
        {
            return;
        }
        Error(message);
        throw new Exception(message.ToString());
    }
}