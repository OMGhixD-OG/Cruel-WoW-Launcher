using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace Cruel_WoW_Launcher.Core
{
    class ClientHandler
    {
        public static void DeleteCache()
        {
            try
            {
                if (Directory.Exists($"{Properties.Settings.Default.WoWPath}\\Cache") && Properties.Settings.Default.ClearCache)
                {
                    LogHandler.WriteToLog("Deleting cache folder..");

                    var dir = new DirectoryInfo("Cache");

                    dir.Delete(true); // true => recursive delete

                    LogHandler.WriteToLog("Cache cleared.");
                }
            }
            catch (Exception ex)
            {
                LogHandler.WriteToLog($"Caught exception error while attempting to delete cache folder:\r\n{ex.Message}");
            }
        }

        private static void SetRealmlist()
        {
            if (File.Exists($"{Properties.Settings.Default.WoWPath}\\WTF\\Config.wtf"))
            {
                var configWTFPath = $"{ Properties.Settings.Default.WoWPath }\\WTF\\Config.wtf";

                var oldLines = File.ReadAllLines(configWTFPath);

                // reads all lines except the lines that contains SET portal
                var newLines = oldLines.Where(line => !line.Contains("SET realmList"));

                File.WriteAllLines(configWTFPath, newLines);

                string realmlist = XMLTools.GetNodeInnerString("Launcher/Realm/Realmlist");

                using (var outputFile = new StreamWriter(configWTFPath, true))
                    outputFile.WriteLine($"SET realmList \"{ realmlist }\"");
            }
        }

        public static void StartWoWClient()
        {
            if (File.Exists($@"{Properties.Settings.Default.WoWExe}"))
            {
                try
                {
                    if (!Extensions.IsHDGraphicsEnable())
                        Extensions.RemoveHDGraphics();

                    DeleteCache();

                    SetRealmlist();

                    LogHandler.WriteToLog("Attempting to start WoW Client..");

                    #region START SAFELY WOW.EXE FROM CMD
                    Process cmd = new Process();
                    cmd.StartInfo.Verb = "runas";
                    cmd.StartInfo.FileName = "cmd.exe";
                    cmd.StartInfo.RedirectStandardInput = true;
                    cmd.StartInfo.RedirectStandardOutput = true;
                    cmd.StartInfo.CreateNoWindow = true;
                    cmd.StartInfo.UseShellExecute = false;
                    cmd.Start();

                    cmd.StandardInput.WriteLine("cd /D " + Properties.Settings.Default.WoWPath);
                    cmd.StandardInput.WriteLine(Properties.Settings.Default.WoWExe);
                    cmd.StandardInput.Flush();
                    cmd.StandardInput.Close();
                    cmd.WaitForExit();
                    #endregion

                    LogHandler.WriteToLog($@"WoW Client started from {Properties.Settings.Default.WoWExe}");
                }
                catch (Exception ex)
                {
                    LogHandler.WriteToLog($"Caught exception error while attempting to start WoW Client:\r\n {ex.Message}");
                }
            }
            else
                MessageBox.Show($"{Properties.Settings.Default.WoWExe} not found!");
        }
    }
}
