using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Pingy.Model
{
    public class PingManager : IPingManager, IDisposable
    {
        #region Fields
        private readonly DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.ApplicationIdle)
        {
            Interval = TimeSpan.FromSeconds(20)
        };

        private CancellationTokenSource cts = new CancellationTokenSource();
        #endregion

        #region Properties
        public bool IsUpdating => Addresses
            .Where(x => x.Status == PingStatus.Updating)
            .Any();

        private FileInfo _file = null;
        public FileInfo File => _file;

        private readonly ObservableCollection<IPingable> _addresses
            = new ObservableCollection<IPingable>();
        public IReadOnlyCollection<IPingable> Addresses => _addresses;
        #endregion

        public PingManager()
        {
            timer.Tick += Timer_Tick;

            timer.Start();
        }

        private async void Timer_Tick(object sender, EventArgs e)
            => await PingAllAsync();

        public void OpenAddressesFile() => Process.Start(_file.FullName);

        public void SetFile(FileInfo file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (!file.Exists)
            {
                throw new FileNotFoundException("SetFile failed", file.FullName);
            }

            _file = file;
        }

        public async Task LoadAddressesAsync()
        {
            _addresses.Clear();

            string[] lines = Array.Empty<string>();

            try
            {
                lines = await FileSystem.GetLinesAsync(File);
            }
            catch (FileNotFoundException ex)
            {
                await Log.LogExceptionAsync(ex).ConfigureAwait(false);

                return;
            }
            
            foreach (string each in lines)
            {
                if (each.StartsWith("#"))
                {
                    continue;
                }

                if (PingBase.TryCreate(each, out PingBase ping))
                {
                    _addresses.Add(ping);
                }
            }
        }

        public async Task PingAllAsync()
        {
            if (IsUpdating)
            {
                return;
            }
            
            var tasks = Addresses
                .Select(x => x.PingAsync(cts.Token))
                .ToList();

            await Task.WhenAll(tasks);
        }
        
        public void Cancel()
        {
            if (IsUpdating)
            {
                cts.Cancel();

                cts = new CancellationTokenSource();
            }
        }
        

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    cts.Dispose();
                }
                
                disposedValue = true;
            }
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
