using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Pingy
{
    class PingManager : ViewModelBase
    {
        #region Commands
        private DelegateCommand _pingAllAsyncCommand = null;
        private DelegateCommand _pingAsyncCommand = null;

        public DelegateCommand PingAllAsyncCommand { get { return this._pingAllAsyncCommand; } }
        public DelegateCommand PingAsyncCommand { get { return this._pingAsyncCommand; } }
        #endregion

        #region Properties
        private readonly string addressesFilePath = string.Format("C:\\Users\\{0}\\Documents\\PingyAddresses.txt", Environment.UserName);
        DispatcherTimer _updateTimer = new DispatcherTimer(DispatcherPriority.SystemIdle);
        private int _updateTimerHours = 12;
        private int _updateTimerMinutes = 0;
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
            _pingAllAsyncCommand = new DelegateCommand(PingAllAsync, canExecutePinging);
            _pingAsyncCommand = new DelegateCommand(PingAsync, canExecutePinging);

            _updateTimer.Interval = new TimeSpan(_updateTimerHours, _updateTimerMinutes, _updateTimerSeconds);
            _updateTimer.Tick += (sender, e) =>
                {
                    PingAllAsync(null);
                };
            _updateTimer.IsEnabled = true;
        }

        public async Task<bool> LoadAddressesFromFileAsync()
        {
            bool addressesLoaded = false;

            StreamReader sr = CreateStreamReaderAgainstFile(addressesFilePath);

            if (sr == null)
            {
                Application.Current.Shutdown();
            }
            else
            {
                string line = string.Empty;

                while ((line = await sr.ReadLineAsync()) != null)
                {
                    Pings.Add(new Ping(line));
                }

                sr.Close();

                addressesLoaded = true;
            }

            return addressesLoaded;
        }

        private StreamReader CreateStreamReaderAgainstFile(string filePath)
        {
            StreamReader sr = null;

            try
            {
                sr = File.OpenText(filePath);
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("The file was not found.", "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Stop);

                if (sr != null)
                {
                    sr.Close();
                    sr = null;
                }
            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show("The directory was not found.", "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Stop);

                if (sr != null)
                {
                    sr.Close();
                    sr = null;
                }
            }

            return sr;
        }

        public void PingAllAsync(object parameter)
        {
            if (this.Pings.Count > 0)
            {
                foreach (Ping eachPing in Pings)
                {
                    eachPing.PingAsync();
                }
            }
        }

        public void PingAsync(object parameter)
        {
            ((Ping)parameter).PingAsync();
        }

        private bool canExecutePinging(object parameter)
        {
            return true; // ICommand requires a canExecute, yet no reason ever to deny
        }
    }
}
