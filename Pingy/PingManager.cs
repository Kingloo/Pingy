using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

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
                    this._pingAllAsyncCommand = new DelegateCommandAsync(new Func<Task>(PingAllAsync), canExecutePinging);
                }

                return this._pingAllAsyncCommand;
            }
        }

        public async Task PingAllAsync()
        {
            if (this.Pings.Count > 0)
            {
                active = true;

                await Task.WhenAll(from each in Pings select each.PingAsync());

                active = false;
            }
        }

        private DelegateCommandAsync<Ping> _pingAsyncCommand = null;
        public DelegateCommandAsync<Ping> PingAsyncCommand
        {
            get
            {
                if (this._pingAsyncCommand == null)
                {
                    this._pingAsyncCommand = new DelegateCommandAsync<Ping>(new Func<Ping, Task>(PingAsync), canExecutePinging);
                }

                return this._pingAsyncCommand;
            }
        }

        public async Task PingAsync(Ping ping)
        {
            active = true;

            await ping.PingAsync();

            active = false;
        }

        private DelegateCommand _exitCommand = null;
        public DelegateCommand ExitCommand
        {
            get
            {
                if (this._exitCommand == null)
                {
                    this._exitCommand = new DelegateCommand(Exit, canExecute);
                }

                return this._exitCommand;
            }
        }

        private void Exit()
        {
            Application.Current.Shutdown();
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

        private bool canExecute(object _)
        {
            return true;
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
                    await PingAllAsync();
                };

            _updateTimer.IsEnabled = true;
        }

        private async void mainWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            bool didLoadAddresses = await LoadAddressesFromFileAsync();

            if (didLoadAddresses)
            {
                await PingAllAsync();
            }
        }

        private void mainWindow_ContentRendered(object sender, EventArgs e)
        {
            Misc.SetWindowToMiddleOfScreen(this.mainWindow);
        }

        public async Task<bool> LoadAddressesFromFileAsync()
        {
            FileStream fsAsync = null;

            try
            {
                fsAsync = new FileStream(addressesFilePath, FileMode.Open, FileAccess.Read, FileShare.None, 4096, true);
            }
            catch (FileNotFoundException)
            {
                return false;
            }

            using (fsAsync)
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
    }
}
