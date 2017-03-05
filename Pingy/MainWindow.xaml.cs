using System;
using System.Windows;
using System.Windows.Forms;
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
            ContentRendered += (s, e) => this.SetToMiddleOfScreen(Screen.FromHandle(windowHandle));
            Loaded += async (s, e) => await vm.LoadAddressesAsync();
            LocationChanged += (s, e) => CalculateMaxHeight();
        }
        
        private void CalculateMaxHeight()
        {
            Screen currentMonitor = Screen.FromHandle(windowHandle);
            
            MaxHeight = currentMonitor == null
                ? SystemParameters.WorkArea.Bottom - 100
                : currentMonitor.WorkingArea.Bottom - 100;
        }
    }
}