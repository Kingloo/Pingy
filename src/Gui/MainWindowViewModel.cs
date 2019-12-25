using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Threading;
using Pingy.Common;
using Pingy.Model;

namespace Pingy.Gui
{
    public class MainWindowViewModel
    {
        private static readonly TimeSpan defaultTimerFrequency = TimeSpan.FromSeconds(12d);

        private readonly string path = string.Empty;
        private readonly DispatcherTimer timer;

        private readonly ObservableCollection<PingBase> _pings = new ObservableCollection<PingBase>();
        public IReadOnlyCollection<PingBase> Pings => _pings;

        public MainWindowViewModel(string path)
            : this(path, defaultTimerFrequency)
        { }

        public MainWindowViewModel(string path, TimeSpan updateFrequency)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }
            
            this.path = path;

            timer = new DispatcherTimer(DispatcherPriority.ApplicationIdle)
            {
                Interval = updateFrequency
            };

            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private async void Timer_Tick(object? sender, EventArgs e) => await PingAllAsync();

        public async Task LoadAsync()
        {
            _pings.Clear();

            string[] lines = await FileSystem.LoadLinesFromFileAsync(path, "#");

            foreach (PingBase each in CreatePings(lines))
            {
                _pings.Add(each);
            }
        }

        private static IEnumerable<PingBase> CreatePings(string[] lines)
        {
            foreach (string line in lines)
            {
                if (PingBase.TryCreate(line, out PingBase? ping))
                {
                    #nullable disable
                    yield return ping;
                    #nullable enable
                }
            }
        }

        public Task PingAsync(PingBase ping) => ping.PingAsync();

        public Task PingAllAsync()
        {
            List<Task> tasks = new List<Task>();

            foreach (PingBase each in Pings)
            {
                tasks.Add(PingAsync(each));
            }

            return (tasks.Count > 0) ? Task.WhenAll(tasks) : Task.CompletedTask;
        }

        public void OpenFile()
        {
            if (!SystemLaunch.Path(path))
            {
                Log.Message($"could not open path: {path}");
            }
        }
    }
}