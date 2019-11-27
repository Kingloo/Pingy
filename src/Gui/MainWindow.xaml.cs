using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
    }
}
