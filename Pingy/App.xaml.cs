using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Pingy.GUI;

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

            MainWindow = new MainWindow(file);

            MainWindow.Show();
        }

        private async void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            string errorMessage = string.Format(
                CultureInfo.CurrentCulture,
                "A fatal error occurred.\n{0}\n{1}",
                e.Exception.GetType().Name,
                e.Exception.Message);

            MessageBox.Show(errorMessage, "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);

            await Log.LogExceptionAsync(e.Exception, true).ConfigureAwait(false);
        }
    }
}
