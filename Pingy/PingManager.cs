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
                if (_pingAllAsyncCommand == null)
                {
                    _pingAllAsyncCommand = new DelegateCommandAsync(PingAllAsync, canExecuteAsync);
                }

                return _pingAllAsyncCommand;
            }
        }

        public async Task PingAllAsync()
        {
            if (this.Pings.Count > 0)
            {
                active = true;

                IEnumerable<Task> pingTasks = from each in Pings
                                              select each.PingAsync();
                
                await Task.WhenAll(pingTasks).ConfigureAwait(false);

                active = false;
            }
        }

        private DelegateCommandAsync<Ping> _pingAsyncCommand = null;
        public DelegateCommandAsync<Ping> PingAsyncCommand
        {
            get
            {
                if (_pingAsyncCommand == null)
                {
                    _pingAsyncCommand = new DelegateCommandAsync<Ping>(PingAsync, canExecuteAsync);
                }

                return _pingAsyncCommand;
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
                if (_exitCommand == null)
                {
                    _exitCommand = new DelegateCommand(Exit, canExecute);
                }

                return _exitCommand;
            }
        }

        private void Exit()
        {
            Application.Current.MainWindow.Close();
        }

        private bool canExecute(object _)
        {
            return true;
        }

        private bool canExecuteAsync(object _)
        {
            return !active;
        }
        #endregion

        #region Fields
        private bool active = false;

        private readonly DispatcherTimer updateTimer = new DispatcherTimer
        {
            Interval = new TimeSpan(0, 0, 18)
        };
        #endregion

        #region Properties
        private ObservableCollection<Ping> _pings = new ObservableCollection<Ping>();
        public ObservableCollection<Ping> Pings { get { return _pings; } }
        #endregion

        public PingManager()
        {
            IEnumerable<Ping> newPings = from each in Program.Addresses
                                         select new Ping(each);

            Pings.AddList<Ping>(newPings);

            updateTimer.Tick += updateTimer_Tick;
            updateTimer.Start();
        }

        private async void updateTimer_Tick(object sender, EventArgs e)
        {
            await PingAllAsync().ConfigureAwait(false);
        }
    }
}
