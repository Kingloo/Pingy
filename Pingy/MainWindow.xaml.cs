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

            PingManager pingManager = new PingManager(this);
            this.DataContext = pingManager;
        }
    }
}
