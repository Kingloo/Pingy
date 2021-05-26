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
		private static readonly TimeSpan defaultUpdateFrequency = TimeSpan.FromSeconds(15d);

		private readonly string addressesFilePath = string.Empty;

		private DispatcherTimer? timer = null;

		private readonly ObservableCollection<PingBase> _pings = new ObservableCollection<PingBase>();
		public IReadOnlyCollection<PingBase> Pings { get => _pings; }

		public MainWindowViewModel(string path)
		{
			if (String.IsNullOrWhiteSpace(path))
			{
				throw new ArgumentNullException(nameof(path));
			}

			addressesFilePath = path;
		}

		private async void Timer_Tick(object? sender, EventArgs e) => await PingAllAsync();

		public void StartTimer() => StartTimer(defaultUpdateFrequency);

		public void StartTimer(TimeSpan updateFrequency)
		{
			if (timer != null)
			{
				StopTimer();
			}

			timer = new DispatcherTimer(DispatcherPriority.ApplicationIdle)
			{
				Interval = updateFrequency
			};

			timer.Tick += Timer_Tick;
			timer.Start();
		}

		public void StopTimer()
		{
			if (timer != null)
			{
				timer.Stop();
				timer.Tick -= Timer_Tick;

				timer = null;
			}
		}

		public async Task LoadAsync()
		{
			_pings.Clear();
			
			string[] lines = await FileSystem.LoadLinesFromFileAsync(addressesFilePath, "#");

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
					yield return ping;
				}
			}
		}

		public Task PingAsync(PingBase ping) => ping.PingAsync();

		public Task PingAsync(PingBase ping, TimeSpan delay) => ping.PingAsync(delay);

		public async Task PingAllAsync()
		{
			if (Pings.Count == 0)
			{
				return;
			}

			List<Task> tasks = new List<Task>(Pings.Count);

			foreach (PingBase each in Pings)
			{
				TimeSpan delay = TimeSpan.FromMilliseconds(GetDelayMs());

				tasks.Add(each.PingAsync(delay));
			}

			await Task.WhenAll(tasks);
		}

		private static int GetDelayMs()
		{
			const int maxDelayMs = 30;

			int randomInt = Int32.Parse(Environment.TickCount.ToString()[^2..].ToString());

			return Math.Min(maxDelayMs, randomInt);
		}

		public void OpenFile()
		{
			if (!SystemLaunch.Path(addressesFilePath))
			{
				LogStatic.Message($"could not open path: {addressesFilePath}");
			}
		}
	}
}
