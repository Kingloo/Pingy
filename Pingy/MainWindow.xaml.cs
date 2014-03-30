using System;
using System.Windows;
using System.Windows.Shapes;

namespace Pingy
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bool beginPinging = await pingManager.LoadAddressesFromFileAsync();

            if (beginPinging)
            {
                pingManager.PingAllAsync(null);
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            double screenHeight = SystemParameters.PrimaryScreenHeight;
            double windowHeight = this.Height;
            this.Top = (screenHeight / 2) - (windowHeight / 2);

            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double windowWidth = this.Width;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
        }
    }
}
