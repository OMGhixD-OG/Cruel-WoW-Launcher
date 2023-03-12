using Cruel_WoW_Launcher.Core;
using System.Windows;
using System.Windows.Input;

namespace Cruel_WoW_Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

        private void ExitButton_Click(object sender, RoutedEventArgs e) => Close();

        private void MinimizeButton_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        private void ChkHDGraphics_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.HDGraphics = (bool)ChkHDGraphics.IsChecked;
            Properties.Settings.Default.Save();

            if ((bool)ChkHDGraphics.IsChecked)
            {
                if (MessageBox.Show("If you want to enable HD Graphics then launcher has to restart and recheck for updates!\r\n \r\nWould you like to restart now?", "ENABLE HD GRAPHICS CONTENT",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Extensions.ResetClientVersion();
                    Tools.RestartLauncher();
                }
                else
                    ChkHDGraphics.IsChecked = false;
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LogHandler.OnStartUp();

            // Load remote config document
            await XMLTools.LoadXMLRemoteConfigAsync();

            ChkHDGraphics.IsChecked = Extensions.IsHDGraphicsEnable();

            // Load navbar links
            // Load news async
            Handlers handlers = new Handlers(this);
            handlers.LoadNavbarLinks();
            await handlers.LoadNewsAsync();

            if (Extensions.IsNewRemoteClientVersion())
            {
                // Get Normal & HD Graphics files list from remote server
                await Downloader.UpdateNormalFilestList();
                await Downloader.UpdateHDFilestList();

                // update available
                PlayButton.Visibility = Visibility.Hidden;
                UpdateButton.Visibility = Visibility.Visible;
            }
            else
            {
                // ready to play
                UpdateButton.Visibility = Visibility.Hidden;
                PlayButton.Visibility = Visibility.Visible;
            }

            // Hide placeholder
            UpdatingPlaceholder.Visibility = Visibility.Hidden;
        }


        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            ClientHandler.StartWoWClient();

            WindowState = WindowState.Minimized;
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            Downloader downloader = new Downloader(this);
            // Update download list based on if hd graphics checkbox is checked
            await downloader.UpdateDownloadListAsync();
            downloader.StartUpdating();
        }
    }
}
