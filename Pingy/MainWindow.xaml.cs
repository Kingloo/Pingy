using System;
using System.Windows;

namespace Pingy
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bool didLoadAddresses = await pingManager.LoadAddressesFromFileAsync();

            if (didLoadAddresses)
            {
                await pingManager.PingAllAsync(null);
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            Misc.SetWindowToMiddleOfScreen(this);
        }
    }
}
