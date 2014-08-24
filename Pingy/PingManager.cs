using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Linq;

namespace Pingy
{
    class PingManager : ViewModelBase
    {
        #region Commands
        private DelegateCommandAsync _pingAllAsyncCommand = null;
        public DelegateCommandAsync PingAllAsyncCommand
        {
            get
            {
                if (this._pingAllAsyncCommand == null)
                {
                    this._pingAllAsyncCommand = new DelegateCommandAsync(new Func<object, Task>(PingAllAsync), canExecutePinging);
                }

                return this._pingAllAsyncCommand;
            }
        }

        private DelegateCommandAsync _pingAsyncCommand = null;
        public DelegateCommandAsync PingAsyncCommand
        {
            get
            {
                if (this._pingAsyncCommand == null)
                {
                    this._pingAsyncCommand = new DelegateCommandAsync(new Func<object, Task>(PingAsync), canExecutePinging);
                }

                return this._pingAsyncCommand;
            }
        }
        #endregion

        #region Fields
        private readonly MainWindow mainWindow = null;
        private bool active = false;
        private readonly string addressesFilePath = string.Format(@"C:\Users\{0}\Documents\PingyAddresses.txt", Environment.UserName);
        private DispatcherTimer _updateTimer = new DispatcherTimer(DispatcherPriority.SystemIdle);
        private int _updateTimerHours = 0;
        private int _updateTimerMinutes = 5;
        private int _updateTimerSeconds = 0;
        #endregion

        #region Properties
        private ObservableCollection<Ping> _pings = new ObservableCollection<Ping>();
        public ObservableCollection<Ping> Pings { get { return this._pings; } }
        #endregion

        public PingManager(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            this.mainWindow.Loaded += mainWindow_Loaded;
            this.mainWindow.ContentRendered += mainWindow_ContentRendered;

            _updateTimer.Interval = new TimeSpan(_updateTimerHours, _updateTimerMinutes, _updateTimerSeconds);
            _updateTimer.Tick += async (sender, e) =>
                {
                    await PingAllAsync(null);
                };

            _updateTimer.IsEnabled = true;
        }

        private async void mainWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            bool didLoadAddresses = await LoadAddressesFromFileAsync();

            if (didLoadAddresses)
            {
                await PingAllAsync(null);
            }
        }

        private void mainWindow_ContentRendered(object sender, EventArgs e)
        {
            Misc.SetWindowToMiddleOfScreen(this.mainWindow);
        }

        public async Task<bool> LoadAddressesFromFileAsync()
        {
            using (FileStream fsAsync = new FileStream(addressesFilePath, FileMode.Open, FileAccess.Read, FileShare.None, 4096, true))
            {
                using (StreamReader sr = new StreamReader(fsAsync))
                {
                    string line = string.Empty;

                    while ((line = await sr.ReadLineAsync()) != null)
                    {
                        this.Pings.Add(new Ping(line));
                    }
                }
            }

            if (this.Pings.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task PingAllAsync(object parameter)
        {
            if (this.Pings.Count > 0)
            {
                active = true;

                await Task.WhenAll(from each in Pings select each.PingAsync());

                active = false;
            }
        }

        public async Task PingAsync(object parameter)
        {
            active = true;
            
            Ping ping = (Ping)parameter;

            await ping.PingAsync();

            active = false;
        }

        private bool canExecutePinging(object parameter)
        {
            if (active)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
