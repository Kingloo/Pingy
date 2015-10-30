using System;
using System.Windows;

namespace Pingy
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new PingManager(this);

            MaxHeight = CalculateMaxHeight();

            WindowStartupLocation = WindowStartupLocation.CenterScreen;
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
    }
}
