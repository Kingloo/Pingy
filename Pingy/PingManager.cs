using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Pingy
{
    class PingManager : ViewModelBase
    {
        #region Commands
        private DelegateCommandAsync _pingAllAsyncCommand = null;
        private DelegateCommandAsync _pingAsyncCommand = null;

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

        #region Properties
        private readonly string addressesFilePath = string.Format(@"C:\Users\{0}\Documents\PingyAddresses.txt", Environment.UserName);
        DispatcherTimer _updateTimer = new DispatcherTimer(DispatcherPriority.SystemIdle);
        private int _updateTimerHours = 0;
        private int _updateTimerMinutes = 5;
        private int _updateTimerSeconds = 0;
        private ObservableCollection<Ping> _pings = new ObservableCollection<Ping>();
        public ObservableCollection<Ping> Pings
        {
            get { return this._pings; }
            set { this._pings = value; }
        }
        #endregion

        public PingManager()
        {
            _updateTimer.Interval = new TimeSpan(_updateTimerHours, _updateTimerMinutes, _updateTimerSeconds);
            _updateTimer.Tick += async (sender, e) =>
                {
                    await PingAllAsync(null);
                };

            _updateTimer.IsEnabled = true;
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
                foreach (Ping each in Pings)
                {
                    await each.PingAsync();
                }
            }
        }

        public async Task PingAsync(object parameter)
        {
            await ((Ping)parameter).PingAsync();
        }

        private bool canExecutePinging(object parameter)
        {
            return true; // ICommand requires a canExecute, yet no reason ever to deny
        }
    }
}
