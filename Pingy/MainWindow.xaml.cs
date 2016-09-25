using System;
using System.Windows;
using Pingy.Extensions;

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

        private static double CalculateMaxHeight()
        {
            double screenHeight = SystemParameters.WorkArea.Bottom;
            double maxHeight = screenHeight - 150;

            return maxHeight;
        }

        private void Window_LayoutUpdated(object sender, EventArgs e)
        {
            this.SetToMiddleOfScreen();
        }
    }
}
