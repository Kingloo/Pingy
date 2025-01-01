using System;
using System.ComponentModel;
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
			await vm.LoadAsync().ConfigureAwait(true);

			await vm.PingAllAsync().ConfigureAwait(true);

			vm.StartTimer();
		}

		private async void Window_KeyUp(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.F5:
					await vm.PingAllAsync().ConfigureAwait(true);
					break;
				case Key.F11:
					vm.OpenFile();
					break;
				case Key.F12:
					await vm.LoadAsync().ConfigureAwait(true);
					await vm.PingAllAsync().ConfigureAwait(true);
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
			Grid grid = (Grid)sender;
			PingBase ping = (PingBase)grid.DataContext;

			await vm.PingAsync(ping).ConfigureAwait(true);
		}

		private void Window_LocationChanged(object sender, EventArgs e)
		{
			IntPtr windowHandle = new WindowInteropHelper(this).EnsureHandle();

			var currentMonitor = System.Windows.Forms.Screen.FromHandle(windowHandle);

			double bottom = currentMonitor?.WorkingArea.Bottom ?? SystemParameters.WorkArea.Bottom;
			double leeway = 150d;

			ArgumentOutOfRangeException.ThrowIfNegative(bottom - leeway, "new max height");

			MaxHeight = bottom - leeway;
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			vm.StopTimer();
		}
	}
}
