using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Pingy.Extensions;

namespace Pingy
{
    public partial class MainWindow : Window
    {
        private IntPtr windowHandle = IntPtr.Zero;
        
        public MainWindow()
        {
            InitializeComponent();

            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            
            SourceInitialized += (s, e) => windowHandle = new WindowInteropHelper(this).EnsureHandle();
            ContentRendered += (s, e) => this.SetToMiddleOfScreen(System.Windows.Forms.Screen.FromHandle(windowHandle));
            Loaded += async (s, e) => await vm.LoadAddressesAsync();
            LocationChanged += (s, e) => CalculateMaxHeight();
            KeyUp += MainWindow_KeyUp;
        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void CalculateMaxHeight()
        {
            var currentMonitor = System.Windows.Forms.Screen.FromHandle(windowHandle);
            
            MaxHeight = currentMonitor == null
                ? SystemParameters.WorkArea.Bottom - 100
                : currentMonitor.WorkingArea.Bottom - 100;
        }
    }
}