using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Cruel_WoW_Launcher.Core
{
    class Tools
    {
        public static void RestartLauncher()
        {
            Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        public static void ShutdownLauncher()
        {
            Application.Current.Shutdown();
        }
    }
}
