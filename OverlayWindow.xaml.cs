using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Odyssey
{
    public partial class OverlayWindow : Window
    {
        private Action _closeAction;

        public OverlayWindow(string imagePath, double width, double height, Action closeAction)
        {
            InitializeComponent();
            _closeAction = closeAction;

            // Set size
            CloseButton.Width = width;
            CloseButton.Height = height;

            // Load image
            try
            {
                BitmapImage bitmap = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
                CloseButton.Tag = bitmap;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading overlay image: {ex.Message}");
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _closeAction?.Invoke();
        }
    }
}
