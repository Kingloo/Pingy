using System;
using System.Windows;

namespace Pingy
{
    public partial class App : Application
    {
        private static string defaultPath = "";

        public App()
            : this(defaultPath)
        { }

        public App(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            InitializeComponent();

            MainWindowViewModel viewModel = new MainWindowViewModel(path);

            MainWindow = new MainWindow(viewModel);
            
            MainWindow.Show();
        }
    }
}
