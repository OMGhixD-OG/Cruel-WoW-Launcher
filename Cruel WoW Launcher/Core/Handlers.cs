using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Xml;

namespace Cruel_WoW_Launcher.Core
{
    class Handlers
    {
        public MainWindow WindowParent;
        public Handlers(MainWindow _window) => WindowParent = _window;

        public void LoadNavbarLinks()
        {
            // Clear navbar children before
            WindowParent.Navbar.Children.Clear();

            try
            {
                double BtnLinkMarginLeft = 50;

                foreach (XmlNode nodeLink in XMLTools.WebConfig.SelectNodes("Launcher/Navbar/Link"))
                {
                    Style buttonStyleLink = (Style)Application.Current.Resources["ButtonStyleLink"];
                    Button button = new Button
                    {
                        Content = nodeLink.Attributes["text"].Value,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                        Width = 12 * nodeLink.Attributes["text"].Value.Length,
                        FontFamily = new FontFamily("Arial"),
                        FontWeight = FontWeights.Bold,
                        FontSize = 16,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF8A8383")),
                        Background = null,
                        BorderBrush = null,
                        Margin = new Thickness(BtnLinkMarginLeft, 0, 0, 0),
                        Style = buttonStyleLink,
                        Tag = nodeLink.InnerText,
                    };

                    BtnLinkMarginLeft = button.Margin.Left + button.Width + 50;

                    button.Click += (s, e) =>
                    {
                        if (button.Tag == null)
                        {
                            MessageBox.Show("Link address not yet resolved.");
                            return;
                        }

                        if (!string.IsNullOrEmpty(button.Tag.ToString()) && !string.IsNullOrWhiteSpace(button.Tag.ToString()))
                            Process.Start(button.Tag.ToString());
                        else
                            MessageBox.Show("Link address is null or empty.");
                    };

                    LogHandler.WriteToLog("Navbar link loaded: " + nodeLink.Attributes["text"].Value);

                    WindowParent.Navbar.Children.Add(button);
                }
            }
            catch (Exception ex)
            {
                LogHandler.WriteToLog(ex.Message);
            }
        }

        private bool IsNewsEnabled() => XMLTools.GetNodeAttributeBolean("Launcher/News", "enable");
        private bool IsChangelogsEnabled() => XMLTools.GetNodeAttributeBolean("Launcher/Changelogs", "enable");

        private async Task LoadNewsHeaderImageAsync(Grid _grid, Image _image)
        {
            try
            {
                MotionEffects motionEffects = new MotionEffects(WindowParent);

                BitmapImage bitmapImage = new BitmapImage();

                bitmapImage.BeginInit();

                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;

                await Task.Delay(100);

                bitmapImage.UriSource = new Uri(XMLTools.GetNodeInnerString("Launcher/News/Image"), UriKind.Absolute);

                bitmapImage.EndInit();

                _image.Source = bitmapImage;

                _image.Stretch = Stretch.Fill;

                _grid.Visibility = Visibility.Visible;

                _image.Visibility = Visibility.Hidden;

                motionEffects.FadeInControl(_image);

                LogHandler.WriteToLog("News header image loaded.");
            }
            catch (Exception ex)
            {
                LogHandler.WriteToLog($"Error loading news header image:\r\n{ex.Message}");
            }
        }

        private async Task<byte[]> GetRemoteData(string url)
        {
            return await Task.Run(() =>
            {
                byte[] remoteData = { 0x00 };
                try
                {
                    using (WebClient webClient = new WebClient())
                        remoteData = webClient.DownloadData(url);
                }
                catch (Exception ex)
                {
                    LogHandler.WriteToLog($"Error loading remote byte[] data:\r\n{ex.Message}");
                }
                return remoteData;
            });
        }

        private async Task LoadArticle(RichTextBox _richTextBox)
        {
            try
            {
                _richTextBox.Document.Blocks.Clear();
                _richTextBox.Visibility = Visibility.Hidden;

                FlowDocument document = new FlowDocument();

                string url = XMLTools.GetNodeInnerString("Launcher/News/Article");

                using (MemoryStream stream = new MemoryStream(await GetRemoteData(url)))
                {
                    TextRange txtRange = new TextRange(_richTextBox.Document.ContentEnd, _richTextBox.Document.ContentEnd);
                    txtRange.Load(stream, DataFormats.Rtf);
                }

                MotionEffects motionEffects = new MotionEffects(WindowParent);
                motionEffects.FadeInControl(_richTextBox);
            }
            catch(Exception ex)
            {
                LogHandler.WriteToLog($"Error loading article remote data:\r\n{ex.Message}");
            }
        }

        private void LoadNewsDate(Label _label)
        {
            try
            {
                MotionEffects motionEffects = new MotionEffects(WindowParent);

                _label.Content = XMLTools.GetNodeInnerString("Launcher/News/Date");

                motionEffects.FadeInControl(_label);

                LogHandler.WriteToLog("News date loaded.");
            }
            catch (Exception ex)
            {
                LogHandler.WriteToLog($"Error loading news post date:\r\n{ex.Message}");
            }
        }

        private void LoadReadMoreButton(Button _button)
        {
            try
            {
                MotionEffects motionEffects = new MotionEffects(WindowParent);

                _button.Tag = XMLTools.GetNodeInnerString("Launcher/News/ReadMore");

                _button.IsEnabled = true;

                _button.Click += (s, e) =>
                {
                    if (_button.Tag == null)
                    {
                        MessageBox.Show("Link address not yet resolved, please try again.");
                        return;
                    }

                    if (!string.IsNullOrEmpty(_button.Tag.ToString()) && !string.IsNullOrWhiteSpace(_button.Tag.ToString()))
                        Process.Start(_button.Tag.ToString());
                    else
                        MessageBox.Show("Link address is null or empty.");
                };

                motionEffects.FadeInControl(_button);

                LogHandler.WriteToLog("News read more button loaded.");
            }
            catch (Exception ex)
            {
                LogHandler.WriteToLog($"Error loading news read more button:\r\n{ex.Message}");
            }
        }

        private void LoadChangelogs()
        {
            try
            {
                double BtnChangelogMarginTop = 0;

                foreach (XmlNode cgNode in XMLTools.WebConfig.SelectNodes("Launcher/Changelogs/Changelog"))
                {
                    Style buttonStyleLink = (Style)Application.Current.Resources["ButtonStyleLink"];
                    Button button = new Button
                    {
                        Content = cgNode.Attributes["text"].Value,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Width = 272,
                        Height = 52,
                        FontFamily = new FontFamily("Arial"),
                        FontWeight = FontWeights.Bold,
                        FontSize = 14,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF83858a")),
                        BorderBrush = null,
                        Margin = new Thickness(0, BtnChangelogMarginTop, 0, 0),
                        Style = buttonStyleLink,
                        Tag = cgNode.InnerText,
                        Visibility = Visibility.Hidden
                    };

                    BtnChangelogMarginTop += button.Height + 8;

                    Uri resourceUri = new Uri("Resources/changelog_button_bg.png", UriKind.Relative);
                    StreamResourceInfo streamInfo = Application.GetResourceStream(resourceUri);
                    BitmapFrame temp = BitmapFrame.Create(streamInfo.Stream);
                    var brush = new ImageBrush();
                    brush.ImageSource = temp;
                    button.Background = brush;

                    button.Click += (s, e) =>
                    {
                        if (button.Tag == null)
                        {
                            MessageBox.Show("Link address not yet resolved.");
                            return;
                        }

                        if (!string.IsNullOrEmpty(button.Tag.ToString()) && !string.IsNullOrWhiteSpace(button.Tag.ToString()))
                            Process.Start(button.Tag.ToString());
                        else
                            MessageBox.Show("Link address is null or empty.");
                    };

                    MotionEffects motionEffects = new MotionEffects(WindowParent);
                    motionEffects.FadeInControl(button);

                    WindowParent.ChangelogsGrid.Children.Add(button);
                }
            }
            catch (Exception ex)
            {
                LogHandler.WriteToLog($"Error loading changelogs:\r\n{ex.Message}");
            }
        }

        public async Task LoadNewsAsync()
        {
            if (IsChangelogsEnabled())
            {
                LoadChangelogs();
            }

            if (IsNewsEnabled())
            {
                await LoadNewsHeaderImageAsync(WindowParent.NewsGrid, WindowParent.NewsImage);
                await LoadArticle(WindowParent.NewsArticle);
                LoadNewsDate(WindowParent.NewsDate);
                LoadReadMoreButton(WindowParent.BtnReadMore);
            }
        }
    }
}
