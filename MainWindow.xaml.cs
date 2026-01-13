using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Web.WebView2.Core;

namespace Odyssey
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BrowserConfig _config;

        public MainWindow()
        {
            InitializeComponent();
            
            // Load configuration
            _config = ConfigLoader.LoadConfig();

            // Initialize WebView2
            InitializeAsync();

            // Handle window loaded event for fullscreen setup
            Loaded += MainWindow_Loaded;
            SizeChanged += MainWindow_SizeChanged;
        }

        private async void InitializeAsync()
        {
            try
            {
                // Show a message while initializing
                Browser.Visibility = Visibility.Collapsed;
                
                // Initialize WebView2 with explicit environment
                var env = await CoreWebView2Environment.CreateAsync(null, null, new CoreWebView2EnvironmentOptions());
                await Browser.EnsureCoreWebView2Async(env);

                // Now show the browser
                Browser.Visibility = Visibility.Visible;

                // Set startup URL
                string startupUrl = _config.StartupUrl;
                if (!startupUrl.StartsWith("http://") && !startupUrl.StartsWith("https://"))
                {
                    startupUrl = "http://" + startupUrl;
                }
                
                // Navigate to URL
                Browser.Source = new Uri(startupUrl);

                // Handle cookie settings
                if (_config.IgnoreCookies && Browser.CoreWebView2 != null)
                {
                    // Use InPrivate mode for session-only cookies
                    Browser.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = true;
                }

                // Update URL bar when navigation occurs
                if (Browser.CoreWebView2 != null)
                {
                    Browser.CoreWebView2.SourceChanged += (s, e) =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            UrlBar.Text = Browser.Source?.ToString() ?? "";
                        });
                    };

                    // Handle navigation errors
                    Browser.CoreWebView2.NavigationCompleted += (s, e) =>
                    {
                        if (!e.IsSuccess)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                MessageBox.Show($"Navigation failed: {e.WebErrorStatus}\nURL: {Browser.Source}", 
                                    "Navigation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                            });
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing browser: {ex.Message}\n\nPlease ensure Microsoft Edge WebView2 Runtime is installed.", 
                    "Initialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
                
                // Show browser anyway in case it partially initialized
                Browser.Visibility = Visibility.Visible;
            }
        }

        private OverlayWindow _overlayWindow;

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Handle fullscreen mode
            if (_config.Fullscreen)
            {
                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Maximized;
                ResizeMode = ResizeMode.NoResize;
                NavBar.Visibility = Visibility.Collapsed;

                // Create and show overlay window
                ShowOverlay();
            }
        }

        private void ShowOverlay()
        {
            try
            {
                string imagePath = ResourceHelper.GetResourcePath("farmo.png");
                
                // Parse dimensions
                double width = 120, height = 125;
                if (!string.IsNullOrEmpty(_config.ButtonSize))
                {
                    string[] parts = _config.ButtonSize.Split(',');
                    if (parts.Length == 2)
                    {
                        double.TryParse(parts[0].Trim(), out width);
                        double.TryParse(parts[1].Trim(), out height);
                    }
                }

                // Create overlay
                _overlayWindow = new OverlayWindow(imagePath, width, height, () => 
                {
                    if (_config.ForceClose)
                    {
                        Close();
                    }
                    else
                    {
                        WindowState = WindowState.Minimized;
                    }
                });
                _overlayWindow.Owner = this;
                
                // Position overlay
                UpdateOverlayPosition();

                _overlayWindow.Show();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating overlay: {ex.Message}");
            }
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_overlayWindow != null && _overlayWindow.IsVisible)
            {
                UpdateOverlayPosition();
            }
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            if (_overlayWindow != null && _overlayWindow.IsVisible)
            {
                UpdateOverlayPosition();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _overlayWindow?.Close();
        }

        private void UpdateOverlayPosition()
        {
            if (_overlayWindow == null) return;

            double x = 0, y = 0;
            bool positionSet = false;

            // Get offset from config
            if (!string.IsNullOrEmpty(_config.ButtonOffset))
            {
                try
                {
                    string[] parts = _config.ButtonOffset.Split(',');
                    if (parts.Length == 2)
                    {
                        double.TryParse(parts[0].Trim(), out x);
                        double.TryParse(parts[1].Trim(), out y);
                        positionSet = true;
                    }
                }
                catch { }
            }

            if (!positionSet)
            {
                // Default: top right
                x = ActualWidth - _overlayWindow.CloseButton.Width - 50;
                y = 50;
            }

            // Convert local coordinates to screen coordinates
            // Since we are in fullscreen/maximized, local 0,0 is usually screen 0,0 but let's be safe
            // However, for a maximized window, PointToScreen(0,0) might give negative values if borders are involved
            // But WindowStyle=None + Maximized usually means client area covers screen.
            
            // Simple approach: absolute positioning relative to window left/top
            _overlayWindow.Left = Left + x;
            _overlayWindow.Top = Top + y;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Browser.CanGoBack)
            {
                Browser.GoBack();
            }
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (Browser.CanGoForward)
            {
                Browser.GoForward();
            }
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            Browser.Reload();
        }

        private void UrlBar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                NavigateToUrl();
            }
        }

        private void NavigateToUrl()
        {
            string url = UrlBar.Text;
            if (!string.IsNullOrEmpty(url))
            {
                if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                {
                    url = "http://" + url;
                }

                try
                {
                    Browser.Source = new Uri(url);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Invalid URL: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
