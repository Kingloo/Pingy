﻿using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Pingy.Common;

namespace Pingy
{
    public partial class App : Application
    {
        private static string defaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string defaultFileName = "Pingy.txt";

        private static string defaultFilePath = Path.Combine(defaultDirectory, defaultFileName);

        public App()
            : this(defaultFilePath)
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

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is Exception ex)
            {
                Log.Exception(ex, true);
            }
            else
            {
                Log.Message("an empty unhandled exception occurred");
            }
        }
    }
}
