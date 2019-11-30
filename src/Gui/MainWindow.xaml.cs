using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using Pingy.Model;

namespace Pingy.Gui
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel vm;

        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();

            vm = viewModel;
            
            DataContext = vm;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await vm.LoadAsync();

            await vm.PingAllAsync();
        }

        private async void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F5:
                    await vm.PingAllAsync();
                    break;
                case Key.F11:
                    vm.OpenFile();
                    break;
                case Key.F12:
                    await vm.LoadAsync();
                    await vm.PingAllAsync();
                    break;
                case Key.Escape:
                    Close();
                    break;
                default:
                    break;
            }
        }

        private async void Grid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if ((PingBase)((Grid)sender).DataContext is PingBase ping)
            {
                await vm.PingAsync(ping);
            }
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            IntPtr windowHandle = new WindowInteropHelper(this).EnsureHandle();

            var currentMonitor = System.Windows.Forms.Screen.FromHandle(windowHandle);

            double bottom = currentMonitor?.WorkingArea.Bottom ?? SystemParameters.WorkArea.Bottom;
            double leeway = 150d;

            if (bottom - leeway < 0)
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("bottom minus leeway was less than zero");
                sb.AppendLine($"bottom: {bottom}, leeway: {leeway}, bottom - leeway = {bottom - leeway}");
                sb.AppendLine(currentMonitor?.ToString() ?? "currentMonitor is null");

                throw new ArgumentOutOfRangeException(nameof(MaxHeight), sb.ToString());
            }

            MaxHeight = bottom - leeway;
        }
    }
}
