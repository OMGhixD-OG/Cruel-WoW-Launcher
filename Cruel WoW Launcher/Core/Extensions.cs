using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Cruel_WoW_Launcher.Core
{
    class Extensions
    {
        public static int GetRemoteGameVersion() => XMLTools.GetNodeAttributeInteger("Launcher", "client_version");

        public static int GetClientGameVersion() => Properties.Settings.Default.ClientVersion;

        public static bool IsNewRemoteClientVersion()
        {
            try
            {
                if (GetRemoteGameVersion() != GetClientGameVersion())
                {
                    LogHandler.WriteToLog($"New remote client version available {GetRemoteGameVersion()}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogHandler.WriteToLog($"Remote update version error:\r\n{ex.Message}");
            }
            return false;
        }

        public static void UpdateClientVersion()
        {
            Properties.Settings.Default.ClientVersion = GetRemoteGameVersion();
            Properties.Settings.Default.Save();
        }

        public static void ResetClientVersion()
        {
            Properties.Settings.Default.ClientVersion = 0;
            Properties.Settings.Default.Save();
        }

        public static void RemoveHDGraphics()
        {
            foreach (KeyValuePair<string, string> item in Downloader.Data.HDFilesList)
            {
                var filePath = item.Key;
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
        }

        public static string HexDecodeString(string hex)
        {
            try
            {
                int NumberChars = hex.Length;

                byte[] bytes = new byte[NumberChars / 2];

                for (int i = 0; i < NumberChars; i += 2)
                    bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);

                return Encoding.UTF8.GetString(bytes);
            }
            catch { }

            return string.Empty;
        }

        public static string ByteArrayToHex(byte[] bytes, bool upperCase)
        {
            StringBuilder result = new StringBuilder(bytes.Length * 2);

            for (int i = 0; i < bytes.Length; i++)
                result.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));

            return result.ToString();
        }

        public static async Task<string> GetOutputFromURL(string url)
        {
            string result = string.Empty;

            await Task.Run(() =>
            {
                using (WebClient client = new WebClient())
                {
                    result = client.DownloadString(url);
                }
            });
            return result;
        }

        public static bool IsValidURLData(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            if (string.IsNullOrWhiteSpace(str))
                return false;

            if (!str.StartsWith("data="))
                return false;

            return true;
        }

        public static bool IsHDGraphicsEnable() => Properties.Settings.Default.HDGraphics;

        public static string GetLauncherRemotePathURL()
        {
            string result = string.Empty;
            try
            {
                result = XMLTools.GetNodeAttributeString("Launcher", "remote_path");
            }
            catch (Exception ex)
            {
                LogHandler.WriteToLog($"Launcher Remote folder url error:\r\n{ex.Message}");
            }
            return result;
        }

        public static bool IsStringMD5(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                LogHandler.WriteToLog($"The input { input } is NOT a valid MD5 hash.");
                return false;
            }

            LogHandler.WriteToLog($"The input { input } is a valid MD5 hash.");
            return Regex.IsMatch(input, "^[0-9a-fA-F]{32}$", RegexOptions.Compiled);
        }

        public static async Task<bool> IsMD5Different(string fileURL, string localPath)
        {
            bool different = false;

            try
            {
                await Task.Run(() =>
                {
                    if (File.Exists(localPath))
                    {
                        string remoteMD5;
                        string localMD5;

                        // get remote md5
                        LogHandler.WriteToLog("---------------------------------------------");
                        LogHandler.WriteToLog("Checking MD5 hash:");
                        LogHandler.WriteToLog(fileURL);
                        LogHandler.WriteToLog(localPath);

                        using (WebClient client = new WebClient())
                        {
                            // correct the root path
                            fileURL = $"{GetLauncherRemotePathURL()}?md5={fileURL.Replace($"{GetLauncherRemotePathURL()}/", string.Empty)}";

                            remoteMD5 = client.DownloadString(fileURL);
                            if (!IsStringMD5(remoteMD5))
                            {
                                LogHandler.WriteToLog($"The result is not a valid md5 hash string: {remoteMD5}");
                                different = true;
                            }

                            client.Dispose();
                        }

                        // get local md5
                        LogHandler.WriteToLog($"Checking MD5 hash for local file: {localPath}");
                        using (var md5 = MD5.Create())
                        {
                            using (var stream = File.OpenRead(localPath))
                            {
                                localMD5 = ByteArrayToHex(md5.ComputeHash(stream), false);
                                LogHandler.WriteToLog($"Local file MD5 hash: {localMD5}");
                            }
                        }

                        // check if hashes match
                        if (remoteMD5 != localMD5)
                        {
                            LogHandler.WriteToLog($"File url {fileURL} is different md5 hash to local file {localPath}");
                            different = true;
                        }
                        else
                            LogHandler.WriteToLog($"File url {fileURL} is NOT different md5 hash to local file {localPath}");

                        LogHandler.WriteToLog("---------------------------------------------");
                    }
                    else
                    {
                        LogHandler.WriteToLog($"Local file path doesn't exist (file marked as different): {localPath}");
                        different = true;
                    }
                });
            }
            catch (Exception ex)
            {
                LogHandler.WriteToLog($"Cought exception error while checking md5 hashes:\r\n{ex.Message}");
            }

            return different;
        }

        public static async Task<long> GetRemoteFileSizeAsync(string url)
        {
            long result = 0;

            await Task.Run(() =>
            {
                try
                {
                    System.Net.WebRequest req = WebRequest.Create(url);
                    req.Method = "HEAD";
                    using (WebResponse resp = req.GetResponse())
                    {
                        if (long.TryParse(resp.Headers.Get("Content-Length"), out long ContentLength))
                            result = ContentLength;
                    }
                }
                catch (Exception ex)
                {
                    LogHandler.WriteToLog($"Error while getting remote file size:\r\n{ex.Message}");
                }
            });
            return result;
        }
    }
}
