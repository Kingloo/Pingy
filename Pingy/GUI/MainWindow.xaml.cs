using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Pingy.Model;

namespace Pingy.GUI
{
    public partial class MainWindow : Window
    {
        public MainWindow(FileInfo file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            InitializeComponent();

            pingManager.SetFile(file);
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            Top = 50d;
            Left = SystemParameters.WorkArea.Right - 150d - Width;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await pingManager.LoadAddressesAsync();
            
            await pingManager.PingAllAsync();
        }

        private async void Window_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F5:
                    await pingManager.PingAllAsync();
                    break;
                case Key.F11:
                    pingManager.OpenAddressesFile();
                    break;
                case Key.F12:
                    await pingManager.LoadAddressesAsync();
                    await pingManager.PingAllAsync();
                    break;
                case Key.C:
                    pingManager.Cancel();
                    break;
                case Key.Escape:
                    pingManager.Cancel();
                    Close();
                    break;
                default:
                    break;
            }
        }

        private async void Grid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            IPingable pingable = (IPingable)((Grid)sender).DataContext;

            await pingable.PingAsync().ConfigureAwait(false);
        }
    }
}
