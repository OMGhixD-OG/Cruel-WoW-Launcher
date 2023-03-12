using System;
using System.IO;
using System.Reflection;

namespace Cruel_WoW_Launcher.Core
{
    class LogHandler
    {
        private static string LogFileName = $"{ Assembly.GetExecutingAssembly().GetName().Name }.log";

        public static void OnStartUp()
        {
            var LogFilePath = LogFileName;

            if (Directory.Exists(Properties.Settings.Default.WoWPath) 
                && !string.IsNullOrEmpty(Properties.Settings.Default.WoWPath)
                && !string.IsNullOrWhiteSpace(Properties.Settings.Default.WoWPath))
                LogFilePath = $"{Properties.Settings.Default.WoWPath}\\{LogFileName}";

            if (File.Exists(LogFilePath))
            {
                File.Delete(LogFilePath);
                WriteToLog("Started a new log file.");
            }
        }

        public static void WriteToLog(string text)
        {
            var LogFilePath = LogFileName;

            if (Directory.Exists(Properties.Settings.Default.WoWPath)
                && !string.IsNullOrEmpty(Properties.Settings.Default.WoWPath)
                && !string.IsNullOrWhiteSpace(Properties.Settings.Default.WoWPath))
                LogFilePath = $"{Properties.Settings.Default.WoWPath}\\{LogFileName}";

            var log = $"[{DateTime.Now}]: {text}";

            if (!File.Exists(LogFilePath))
            {
                using (StreamWriter sw = File.CreateText(LogFilePath))
                    sw.WriteLine(log);
            }
            else
                File.AppendAllLines(LogFilePath, new[] { log });
        }
    }
}