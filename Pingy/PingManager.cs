using System;
using System.Collections.Generic;
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

                IEnumerable<Task> pingTasks = from each in Pings select each.PingAsync();
                
                await Task.WhenAll(pingTasks).ConfigureAwait(false);

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
            await ping.PingAsync().ConfigureAwait(false);
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
            Application.Current.MainWindow.Close();
        }

        private bool canExecutePinging(object parameter)
        {
            return !active;
        }

        private bool canExecute(object _)
        {
            return true;
        }
        #endregion

        #region Fields
        private bool active = false;
        private DispatcherTimer updateTimer = null;
        private readonly TimeSpan updateTimeSpan = new TimeSpan(0, 5, 0); // hours, minutes, seconds
        #endregion

        #region Properties
        private ObservableCollection<Ping> _pings = new ObservableCollection<Ping>();
        public ObservableCollection<Ping> Pings { get { return this._pings; } }
        #endregion

        public PingManager()
        {
            IEnumerable<Ping> newPings = from each in Program.Addresses
                                         select new Ping(each);

            this.Pings.AddList<Ping>(newPings);

            updateTimer = new DispatcherTimer
            {
                Interval = updateTimeSpan,
                IsEnabled = false
            };

            updateTimer.Tick += updateTimer_Tick;
            updateTimer.IsEnabled = true;
        }

        private async void updateTimer_Tick(object sender, EventArgs e)
        {
            await PingAllAsync().ConfigureAwait(false);
        }
    }
}
