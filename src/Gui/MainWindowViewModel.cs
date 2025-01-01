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

		private async void Timer_Tick(object? sender, EventArgs e)
		{
			await PingAllAsync().ConfigureAwait(true);
		}

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

			string[] lines = await FileSystem.LoadLinesFromFileAsync(addressesFilePath).ConfigureAwait(true);

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

#pragma warning disable CA1822 // no instance data therefore can be made static
		public Task PingAsync(PingBase ping)
		{
			ArgumentNullException.ThrowIfNull(ping);

			return ping.PingAsync();
		}
#pragma warning restore CA1822

#pragma warning disable CA1822 // no instance data therefore can be made static
		public Task PingAsync(PingBase ping, TimeSpan delay)
		{
			ArgumentNullException.ThrowIfNull(ping);

			return ping.PingAsync(delay);
		}
#pragma warning restore CA1822

		public Task PingAllAsync()
		{
			if (Pings.Count == 0)
			{
				return Task.CompletedTask;
			}

			List<Task> tasks = new List<Task>(capacity: Pings.Count);

			foreach (PingBase each in Pings)
			{
				TimeSpan randomDelay = GetRandomDelay();

				tasks.Add(each.PingAsync(randomDelay));
			}

			return Task.WhenAll(tasks);
		}

		private static TimeSpan GetRandomDelay()
		{
			const int maxDelayMs = 30;

			int rand = System.Security.Cryptography.RandomNumberGenerator.GetInt32(fromInclusive: 10, toExclusive: maxDelayMs);

			return TimeSpan.FromMilliseconds(Math.Min(maxDelayMs, rand));
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
