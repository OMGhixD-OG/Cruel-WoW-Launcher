using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Cruel_WoW_Launcher.Core
{
    class Downloader
    {
        private MainWindow WindowParent;

        private static WebClient ClientDownloader = new WebClient();

        private Stopwatch SWSpeed = new Stopwatch();

        private DateTime WstartTime;

        public Downloader(MainWindow _window) => WindowParent = _window;

        public class Data
        {
            public static List<KeyValuePair<string, string>> DownloadList = new List<KeyValuePair<string, string>>();

            public static List<KeyValuePair<string, string>> FilesList = new List<KeyValuePair<string, string>>();

            public static List<KeyValuePair<string, string>> HDFilesList = new List<KeyValuePair<string, string>>();

            public static int CurrentFileIndex { get; set; }

            public static string CurrentFileName { get; set; }

            public static int DownloadListCount { get; set; }
        }

        private void Clear_Download_Session()
        {
            Data.DownloadListCount = 0;
            Data.CurrentFileIndex = 0;
            Data.CurrentFileName = "";
            Data.DownloadList.Clear();
        }

        // This will populate the non HD FilesList array with local file paths and download urls
        public static async Task UpdateNormalFilestList()
        {
            string url = string.Empty;

            try
            {
                Data.FilesList.Clear();

                // get patches hd output url example: http://localhost/cruel-launcher?patches
                url = $"{Extensions.GetLauncherRemotePathURL()}?patches";

                // get output from url
                string output = await Extensions.GetOutputFromURL(url);

                // get rid of "data=" string before the encrypted string
                output = output.Replace("data=", string.Empty);

                // decrypt hex string
                output = Extensions.HexDecodeString(output);

                using (StringReader reader = new StringReader(output))
                {
                    string fileURL;

                    string localPath;

                    while ((fileURL = reader.ReadLine()) != null)
                    {
                        // convert url to local path
                        localPath = fileURL.Replace($"patches/", string.Empty);

                        // replace "/" with "\" for windows path
                        localPath = localPath.Replace("/", @"\");

                        // add launcher path url prefix
                        fileURL = $"{Extensions.GetLauncherRemotePathURL()}/{fileURL}";

                        // replace "spaces" with "%20"
                        fileURL = fileURL.Replace(" ", "%20");

                        // add file url to HD files list
                        Data.FilesList.Add(new KeyValuePair<string, string>(localPath, fileURL));

                        //MessageBox.Show($"localpath: [{localPath}] and fileUrl: [{Extensions.GetLauncherRemotePathURL()}/{fileURL}]");
                        // write to log
                        LogHandler.WriteToLog($"-----------------------------------------------");
                        LogHandler.WriteToLog($"FilesList(Non HD) added:");
                        LogHandler.WriteToLog($"Local path: {localPath}");
                        LogHandler.WriteToLog($"Remote URL: {fileURL}");
                        LogHandler.WriteToLog($"-----------------------------------------------");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHandler.WriteToLog($"Error while directory listing ({url}):\r\n{ex.Message}");
            }
        }

        // This will populate the HDFilesList array with local file paths and download urls
        public static async Task UpdateHDFilestList()
        {
            string url = string.Empty;

            try
            {
                Data.HDFilesList.Clear();

                // get patches hd output url example: http://localhost/cruel-launcher?patches_hd
                url = $"{Extensions.GetLauncherRemotePathURL()}?patches_hd";

                // get output from url
                string output = await Extensions.GetOutputFromURL(url);

                // get rid of "data=" string before the encrypted string
                output = output.Replace("data=", string.Empty);

                // decrypt hex string
                output = Extensions.HexDecodeString(output);

                using (StringReader reader = new StringReader(output))
                {
                    string fileURL;

                    string localPath;

                    while ((fileURL = reader.ReadLine()) != null)
                    {
                        // convert url to local path
                        localPath = fileURL.Replace($"patches_hd/", string.Empty);

                        // replace "/" with "\" for windows path
                        localPath = localPath.Replace("/", @"\");

                        // add launcher path url prefix
                        fileURL = $"{Extensions.GetLauncherRemotePathURL()}/{fileURL}";

                        // replace "spaces" with "%20"
                        fileURL = fileURL.Replace(" ", "%20");

                        // add file url to HD files list
                        Data.HDFilesList.Add(new KeyValuePair<string, string>(localPath, fileURL));

                        //MessageBox.Show($"localpath: [{localPath}] and fileUrl: [{Extensions.GetLauncherRemotePathURL()}/{fileURL}]");
                        // write to log
                        LogHandler.WriteToLog($"-----------------------------------------------");
                        LogHandler.WriteToLog($"FilesList(HD Graphics) added:");
                        LogHandler.WriteToLog($"Local path: {localPath}");
                        LogHandler.WriteToLog($"Remote URL: {fileURL}");
                        LogHandler.WriteToLog($"-----------------------------------------------");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHandler.WriteToLog($"Error while directory listing ({url}):\r\n{ex.Message}");
            }
        }

        // This will populate the download list
        public async Task UpdateDownloadListAsync()
        {
            // Clear download session
            Clear_Download_Session();

            // Hide update and play buttons
            WindowParent.UpdateButton.Visibility = Visibility.Hidden;
            WindowParent.PlayButton.Visibility = Visibility.Hidden;

            // Display updating placeholder
            WindowParent.UpdatingPlaceholder.Visibility = Visibility.Visible;
            WindowParent.LabelPlaceholder.Content = "Verifying files";

            // Display progress grid
            WindowParent.ProgressGrid.Visibility = Visibility.Visible;

            // Hide size icon
            WindowParent.SizeIcon.Visibility = Visibility.Hidden;

            // Hide bot richtexbox
            WindowParent.RtbProgress.Visibility = Visibility.Hidden;

            // Update progress bar max value with patches + hd patches count
            WindowParent.pbar.Maximum = Data.FilesList.Count + (Extensions.IsHDGraphicsEnable() ? Data.HDFilesList.Count : 0);

            // Set "verifying text" next to the progress bar
            WindowParent.DownloadStatus.Content = "Verifying";

            // Disable hd graphics checkbox
            WindowParent.ChkHDGraphics.IsEnabled = false;

            // Loop normal FilesList
            foreach (var item in Data.FilesList)
            {
                string localPath = item.Key;
                string remoteUrl = item.Value;

                // Compare MD5 hashes and download only the different md5 hash files
                if (await Extensions.IsMD5Different(remoteUrl, localPath))
                {
                    LogHandler.WriteToLog("-------------------------------------------------------");
                    LogHandler.WriteToLog("Download list added:");
                    LogHandler.WriteToLog($"Local path: {localPath}");
                    LogHandler.WriteToLog($"Remote URL: {remoteUrl}");
                    LogHandler.WriteToLog("-------------------------------------------------------");
                    Data.DownloadList.Add(new KeyValuePair<string, string>(localPath, remoteUrl));
                    Data.DownloadListCount++;
                }

                // Update progressbar
                WindowParent.pbar.Value++;

                double percent = WindowParent.pbar.Value / WindowParent.pbar.Maximum * 100;

                WindowParent.DownloadStatus.Content = $"Verify {(int)percent}%";
            }

            // Loop HD FilesList
            if (Extensions.IsHDGraphicsEnable())
            {
                foreach (var item in Data.HDFilesList)
                {
                    string localPath = item.Key;
                    string remoteUrl = item.Value;

                    // Compare MD5 hashes and download only the different md5 hash files
                    if (await Extensions.IsMD5Different(remoteUrl, localPath))
                    {
                        Data.DownloadList.Add(new KeyValuePair<string, string>(localPath, remoteUrl));
                        Data.DownloadListCount++;
                    }

                    // Update progressbar
                    WindowParent.pbar.Value++;

                    double percent = WindowParent.pbar.Value / WindowParent.pbar.Maximum * 100;

                    WindowParent.DownloadStatus.Content = $"Verify {(int)percent}%";
                }
            }

            // Rest progress bar progress
            WindowParent.pbar.Value = 0;

            await Task.Delay(1000);
        }

        public void StartUpdating()
        {
            // hide update and play buttons
            WindowParent.UpdateButton.Visibility = Visibility.Hidden;
            WindowParent.PlayButton.Visibility = Visibility.Hidden;

            if (Data.DownloadListCount != 0)
            {
                try
                {
                    // Update label "Processing" next to the progress bar
                    WindowParent.DownloadStatus.Content = "Processing";

                    // Set progress bar maximum value to 0
                    WindowParent.pbar.Maximum = 100;

                    WindowParent.SizeIcon.Visibility = Visibility.Visible;
                    WindowParent.LabelPlaceholder.Content = "Downloading Updates";

                    // download first file from the list then break
                    foreach (var item in Data.DownloadList)
                    {
                        string localPath = item.Key;
                        string remoteUrl = item.Value;

                        Data.CurrentFileIndex++;
                        Data.CurrentFileName = item.Value.Substring(item.Value.LastIndexOf('/') + 1);

                        if (!string.IsNullOrEmpty(Path.GetDirectoryName(localPath)) && !string.IsNullOrWhiteSpace(Path.GetDirectoryName(localPath)))
                            if (!Directory.Exists(Path.GetDirectoryName(localPath)))
                                Directory.CreateDirectory(Path.GetDirectoryName(localPath));

                        LogHandler.WriteToLog($"Requested download from url {remoteUrl}");
                        DownloadFile(remoteUrl, localPath);
                        break; // stop after the first item
                    }
                }
                catch (Exception ex)
                {
                    LogHandler.WriteToLog($"Cought exception error while started/continue update:\r\n{ex.Message}");
                }
            }
            else
            {
                WindowParent.ProgressGrid.Visibility = Visibility.Hidden;
                WindowParent.PlayButton.Visibility = Visibility.Visible;
                WindowParent.UpdatingPlaceholder.Visibility = Visibility.Hidden;
                WindowParent.ChkHDGraphics.IsEnabled = true;
                Extensions.UpdateClientVersion();
            }
        }

        private void DownloadFile(string urlAddress, string destination)
        {
            WstartTime = DateTime.Now;

            using (ClientDownloader = new WebClient())
            {
                ClientDownloader.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed); // completed event

                ClientDownloader.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged); // progress change event

                Uri downloadURL = new Uri(urlAddress);

                SWSpeed.Start(); // Start the stopwatch which we will be using to calculate the download speed

                try
                {
                    // show bottom progress rich text box
                    WindowParent.RtbProgress.Visibility = Visibility.Visible;
                    ClientDownloader.DownloadFileAsync(downloadURL, destination);
                    LogHandler.WriteToLog("Download started:");
                    LogHandler.WriteToLog(downloadURL.ToString());
                    LogHandler.WriteToLog(destination);
                }
                catch (Exception ex)
                {
                    WindowParent.PlayButton.Visibility = Visibility.Visible;
                    LogHandler.WriteToLog($"Download for url {downloadURL} error:\r\n{ex.Message}");
                }
            }
        }

        private void UpdateTextProgress(string _speed, string _percent)
        {
            // update the richtextbox
            WindowParent._CurrentFileName.Text = Data.CurrentFileName;
            WindowParent._FirstIndex.Text = Data.CurrentFileIndex.ToString();
            WindowParent._SecondIndex.Text = Data.DownloadListCount.ToString();
            WindowParent._DownloadSpeed.Text = _speed;

            // update % status
            WindowParent.DownloadStatus.Content = _percent;
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            TimeSpan span = DateTime.Now - WstartTime;

            if (span.TotalMilliseconds >= 100)
            {
                WstartTime = DateTime.Now;

                UpdateTextProgress($"{e.BytesReceived / 1024d / 1024d / SWSpeed.Elapsed.TotalSeconds:0.00} MB/s", $"{e.ProgressPercentage}%");

                WindowParent.pbar.Value = e.ProgressPercentage;
            }
        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            SWSpeed.Reset(); // timer used for download speed

            if (e.Cancelled == true)
            {
                WindowParent.DownloadStatus.Content = "Cancelled";
                LogHandler.WriteToLog($"Download cancelled for file: {Data.CurrentFileName}");

                if (MessageBox.Show("Restart?", $"Download cancelled/failed for file {Data.CurrentFileName}. You can restart the launcher to retry updating from where its left",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    Tools.RestartLauncher();
                else
                    Tools.ShutdownLauncher();
            }
            else
            {
                if (Data.CurrentFileIndex >= Data.DownloadListCount) // all downloads completed
                {
                    WindowParent.ProgressGrid.Visibility = Visibility.Hidden;

                    WindowParent.PlayButton.Visibility = Visibility.Visible;

                    WindowParent.UpdatingPlaceholder.Visibility = Visibility.Hidden;

                    WindowParent.ChkHDGraphics.IsEnabled = true;

                    LogHandler.WriteToLog($"Downloaded file {Data.CurrentFileName} progress {Data.CurrentFileIndex} of {Data.DownloadListCount}");

                    LogHandler.WriteToLog("All downloads finished");

                    WindowParent.pbar.Value = 100; // update progress bar to 100% since the progress is only updating while downloading

                    WindowParent.DownloadStatus.Content = "100%";

                    Data.DownloadList.RemoveAt(0); // remove downloaded patch for the download list

                    Extensions.UpdateClientVersion();
                }
                else // continue download list left files
                {
                    LogHandler.WriteToLog($"Downloaded file {Data.CurrentFileName} progress {Data.CurrentFileIndex} of {Data.DownloadListCount}");

                    WindowParent.pbar.Value = 100; // update progress bar to 100% since the progress is only updating while downloading

                    //WindowParent.DownloadStatus.Content = "100%";

                    Data.DownloadList.RemoveAt(0); // remove downloaded patch for the download list

                    StartUpdating(); // continue with the next file from the download list
                }
            }
        }
    }
}
