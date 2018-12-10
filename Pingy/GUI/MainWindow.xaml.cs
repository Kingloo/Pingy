using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using Pingy.Model;

namespace Pingy.GUI
{
    public partial class MainWindow : Window
    {
        private PingManager pingManager = default;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            pingManager = (PingManager)e.NewValue;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            pingManager.StartTimer();

            await pingManager.LoadAddressesAsync();
            
            await pingManager.PingAllAsync();
        }

        private async void Window_KeyDown(object sender, KeyEventArgs e)
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
            var pingable = (IPingable)((Grid)sender).DataContext;

            await pingable.PingAsync().ConfigureAwait(false);
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            IntPtr windowHandle = new WindowInteropHelper(this).EnsureHandle();

            var currentMonitor = System.Windows.Forms.Screen.FromHandle(windowHandle);

            double bottom = currentMonitor?.WorkingArea.Bottom ?? SystemParameters.WorkArea.Bottom;
            double leeway = 150d;

            MaxHeight = bottom - leeway;
        }        
    }
}
