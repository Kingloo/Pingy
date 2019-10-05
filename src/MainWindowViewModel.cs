using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Threading;
using Pingy.Extensions;
using Pingy.Model;

namespace Pingy
{
    public class MainWindowViewModel
    {
        private static TimeSpan defaultTimerFrequency = TimeSpan.FromSeconds(12d);

        private string path = string.Empty;
        private readonly DispatcherTimer timer = null;

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

        private async void Timer_Tick(object sender, EventArgs e) => await PingAllAsync();

        public async Task LoadAsync()
        {
            _pings.Clear();

            string[] lines = await LoadLinesFromFileAsync(path, "#");

            foreach (PingBase each in CreatePings(lines))
            {
                _pings.Add(each);
            }
        }

        private static async Task<string[]> LoadLinesFromFileAsync(string path, string commentChar)
        {
            List<string> lines = new List<string>();

            FileStream fsAsync = null;

            try
            {
                fsAsync = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan);

                using (StreamReader sr = new StreamReader(fsAsync))
                {
                    fsAsync = null;

                    string line = string.Empty;

                    while ((line = await sr.ReadLineAsync().ConfigureAwait(false)) != null)
                    {
                        if (!line.StartsWith(commentChar))
                        {
                            lines.Add(line);
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                return Array.Empty<string>();
            }
            finally
            {
                fsAsync?.Dispose();
            }

            return lines.ToArray();
        }

        private static IEnumerable<PingBase> CreatePings(string[] lines)
        {
            foreach (string line in lines)
            {
                if (PingBase.TryCreate(line, out PingBase ping))
                {
                    yield return ping;
                }
            }
        }

        public Task PingAsync(PingBase ping)
        {
            if (ping is null) { throw new ArgumentNullException(nameof(ping)); }

            return ping.PingAsync();
        }

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
            FileInfo fInfo = new FileInfo(path);

            fInfo.Launch();
        }
    }
}