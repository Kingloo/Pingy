using System;
using System.Windows;

namespace Pingy
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.MaxHeight = CalculateMaxHeight();

            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        private double CalculateMaxHeight()
        {
            double screenHeight = SystemParameters.WorkArea.Bottom;
            double maxHeight = screenHeight - 150;

            return maxHeight;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            Utils.SetWindowToMiddleOfScreen(this);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await vm.PingAllAsync().ConfigureAwait(false);
        }
    }
}
