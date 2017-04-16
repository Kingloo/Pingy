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
    public class PingManager : ViewModelBase
    {
        #region Fields
        private bool active = false;

        private readonly DispatcherTimer updateTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(18d)
        };
        #endregion

        #region Commands
        private DelegateCommandAsync _pingAllAsyncCommand = null;
        public DelegateCommandAsync PingAllAsyncCommand
        {
            get
            {
                if (_pingAllAsyncCommand == null)
                {
                    _pingAllAsyncCommand = new DelegateCommandAsync(PingAllAsync, CanExecuteAsync);
                }

                return _pingAllAsyncCommand;
            }
        }

        public async Task PingAllAsync()
        {
            if (Pings.Count > 0)
            {
                active = true;
                
                var pingTasks = Pings.Select(x => x.PingAsync());
                
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
                    _pingAsyncCommand = new DelegateCommandAsync<Ping>(PingAsync, CanExecuteAsync);
                }

                return _pingAsyncCommand;
            }
        }

        public async Task PingAsync(Ping ping) => await ping.PingAsync();

        private DelegateCommand _openAddressesFileCommand = null;
        public DelegateCommand OpenAddressesFileCommand
        {
            get
            {
                if (_openAddressesFileCommand == null)
                {
                    _openAddressesFileCommand = new DelegateCommand(OpenAddressesFile, CanExecute);
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
            catch (FileNotFoundException ex)
            {
                Utils.LogException(ex, "notepad.exe not found");

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
                    _loadAddressesAsyncCommand = new DelegateCommandAsync(LoadAddressesAsync, CanExecuteAsync);
                }

                return _loadAddressesAsyncCommand;
            }
        }

        public async Task LoadAddressesAsync()
        {
            _pings.Clear();

            IEnumerable<Ping> loaded = await (App.Current as App).Repo.LoadAsync();
            
            _pings.AddRange(loaded);

            await PingAllAsync();
        }

        private DelegateCommand _exitCommand = null;
        public DelegateCommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new DelegateCommand(Exit, CanExecute);
                }

                return _exitCommand;
            }
        }

        private void Exit() => Application.Current.MainWindow.Close();

        private bool CanExecute(object _) => true;

        private bool CanExecuteAsync(object _) => !active;
        #endregion
        
        #region Properties
        private readonly ObservableCollection<Ping> _pings
            = new ObservableCollection<Ping>();
        public IReadOnlyCollection<Ping> Pings => _pings;
        #endregion

        public PingManager()
        {
            updateTimer.Tick += UpdateTimer_Tick;
            updateTimer.Start();
        }
        
        private async void UpdateTimer_Tick(object sender, EventArgs e)
            => await PingAllAsync();
    }
}
