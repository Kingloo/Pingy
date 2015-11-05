using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Pingy.Extensions;

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
            if (Pings.Count > 0)
            {
                active = true;

                IEnumerable<Task> pingTasks = from each in Pings
                                              select each.PingAsync();
                
                await Task.WhenAll(pingTasks);

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
            await ping.PingAsync();
        }

        private DelegateCommand _openAddressesFileCommand = null;
        public DelegateCommand OpenAddressesFileCommand
        {
            get
            {
                if (_openAddressesFileCommand == null)
                {
                    _openAddressesFileCommand = new DelegateCommand(OpenAddressesFile, canExecute);
                }

                return _openAddressesFileCommand;
            }
        }

        private void OpenAddressesFile()
        {
            try
            {
                Process.Start("notepad.exe", (App.Current as App).Repo.FilePath);
            }
            catch (FileNotFoundException e)
            {
                Utils.LogException(e, "notepad.exe not found");

                Process.Start((App.Current as App).Repo.FilePath);
            }
        }

        private DelegateCommandAsync _loadAddressesAsyncCommand = null;
        public DelegateCommandAsync LoadAddressesAsyncCommand
        {
            get
            {
                if (_loadAddressesAsyncCommand == null)
                {
                    _loadAddressesAsyncCommand = new DelegateCommandAsync(LoadAddressesAsync, canExecuteAsync);
                }

                return _loadAddressesAsyncCommand;
            }
        }

        private async Task LoadAddressesAsync()
        {
            Pings.Clear();

            IEnumerable<Ping> loaded = await (App.Current as App).Repo.LoadAsync();

            Pings.AddList<Ping>(loaded);

            await PingAllAsync();
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

        //private readonly DispatcherTimer updateTimer = new DispatcherTimer
        //{
        //    Interval = new TimeSpan(0, 0, 18)
        //};
        private readonly DispatcherTimer updateTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(18d)
        };
        #endregion

        #region Properties
        private ObservableCollection<Ping> _pings = new ObservableCollection<Ping>();
        public ObservableCollection<Ping> Pings { get { return _pings; } }
        #endregion

        public PingManager(MainWindow mainWindow)
        {
            mainWindow.Loaded += mainWindow_Loaded;

            updateTimer.Tick += updateTimer_Tick;
            updateTimer.Start();
        }

        private async void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadAddressesAsync();
        }

        private async void updateTimer_Tick(object sender, EventArgs e)
        {
            await PingAllAsync();
        }
    }
}
