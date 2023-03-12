using System;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace Cruel_WoW_Launcher.Core
{
    public class XMLTools
    {
        public static XmlDocument WebConfig;

        public static async Task LoadXMLRemoteConfigAsync()
        {
            try
            {
                await Task.Run(() =>
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(Properties.Settings.Default.ConfigUrl);
                    WebConfig = xmlDocument;
                });
                LogHandler.WriteToLog("XML remote document loaded successfully.");
            }
            catch (Exception ex)
            {
                LogHandler.WriteToLog(ex.Message);

                if (MessageBox.Show("Try again?", "Can't load launcher remote configuration", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    Tools.RestartLauncher();
                else
                    Tools.ShutdownLauncher();
            }
        }

        public static string GetNodeInnerString(string xpath)
        {
            string result = string.Empty;
            try
            {
                result = WebConfig.SelectSingleNode(xpath).InnerText;
            }
            catch (Exception ex)
            {
                LogHandler.WriteToLog($"Cought exception error while trying to get node inner string:\r\n{ex.Message}");
            }
            return result;
        }

        public static int GetNodeInnerInteger(string xpath)
        {
            int result = 0;
            try
            {
                result = int.Parse(WebConfig.SelectSingleNode(xpath).InnerText);
            }
            catch (Exception ex)
            {
                LogHandler.WriteToLog($"Cought exception error while trying to get node inner int:\r\n{ex.Message}");
            }
            return result;
        }

        public static string GetNodeAttributeString(string xpath, string attr)
        {
            string result = string.Empty;
            try
            {
                result = WebConfig.SelectSingleNode(xpath).Attributes[attr].Value;
            }
            catch (Exception ex)
            {
                LogHandler.WriteToLog($"Cought exception error while trying to get node attribute string:\r\n{ex.Message}");
            }
            return result;
        }

        public static int GetNodeAttributeInteger(string xpath, string attr)
        {
            int result = 0;
            try
            {
                result = int.Parse(WebConfig.SelectSingleNode(xpath).Attributes[attr].Value);
            }
            catch (Exception ex)
            {
                LogHandler.WriteToLog($"Cought exception error while trying to get node attribute string:\r\n{ex.Message}");
            }
            return result;
        }

        public static bool GetNodeAttributeBolean(string xpath, string attr)
        {
            bool result = false;
            try
            {
                result = bool.Parse(WebConfig.SelectSingleNode(xpath).Attributes[attr].Value);
            }
            catch (Exception ex)
            {
                LogHandler.WriteToLog($"Cought exception error while trying to get node attribute bool:\r\n{ex.Message}");
            }
            return result;
        }
    }
}