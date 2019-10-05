using System;
using System.Windows;

namespace Pingy
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel vm = null;

        public MainWindow(MainWindowViewModel viewModel)
        {
            if (viewModel is null) { throw new ArgumentNullException(nameof(viewModel)); }

            InitializeComponent();

            vm = viewModel;
        }

        private async void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            await vm.LoadAsync();
        }
    }
}
