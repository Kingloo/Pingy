using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Pingy.GUI;
using Pingy.Model;

namespace Pingy
{
    public partial class App : Application
    {
        public App(FileInfo file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            InitializeComponent();

            MainWindow = new MainWindow
            {
                DataContext = new PingManager(file)
            };

            MainWindow.Show();
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Log.LogException(e.Exception, true);
        }
    }
}
