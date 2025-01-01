using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace Pingy.Model
{
	public abstract class PingBase : INotifyPropertyChanged
	{
		private const int timeout = 2500;

		public event PropertyChangedEventHandler? PropertyChanged;

		protected bool SetProperty<T>(ref T storage, T value, string propertyName)
		{
			if (Equals(storage, value))
			{
				return false;
			}

			storage = value;

			OnPropertyChanged(propertyName);

			return true;
		}

		protected void OnPropertyChanged(string propertyName)
			=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		private string _displayName = string.Empty;
		public string DisplayName
		{
			get => _displayName;
			protected set => SetProperty(ref _displayName, value, nameof(DisplayName));
		}

		public string DisplayText => Status.ToString();

		private PingStatus _status = PingStatus.None;
		public PingStatus Status
		{
			get => _status;
			set
			{
				SetProperty(ref _status, value, nameof(Status));

				OnPropertyChanged(nameof(DisplayText));
			}
		}

		private IPAddress _address = IPAddress.None;
		public IPAddress Address
		{
			get => _address;
			protected set => SetProperty(ref _address, value, nameof(Address));
		}

		private Int64 _roundtripTime = 0;
		public Int64 RoundtripTime
		{
			get => _roundtripTime;
			protected set => SetProperty(ref _roundtripTime, value, nameof(RoundtripTime));
		}

		public static bool TryCreate(string line, [NotNullWhen(true)] out PingBase? pingBase)
		{
			if (IPAddress.TryParse(line, out IPAddress? address))
			{
				pingBase = new IP(address);
			}
			else if (Uri.TryCreate(string.Format(CultureInfo.CurrentCulture, "http://{0}", line), UriKind.Absolute, out Uri? uri))
			{
				IPHostEntry hostEntry = new IPHostEntry
				{
					HostName = uri.DnsSafeHost
				};

				pingBase = new Domain(hostEntry);
			}
			else
			{
				pingBase = null;
			}

			return pingBase is not null;
		}

		public Task PingAsync() => PingAsync(TimeSpan.Zero, CancellationToken.None);

		public Task PingAsync(TimeSpan delay) => PingAsync(delay, CancellationToken.None);

		public Task PingAsync(CancellationToken token) => PingAsync(TimeSpan.Zero, token);

		public virtual async Task PingAsync(TimeSpan delay, CancellationToken token)
		{
			Status = PingStatus.Updating;

			await Task.Yield();

			if (delay > TimeSpan.Zero)
			{
				await Task.Delay(delay, token).ConfigureAwait(false);
			}

			PingReply? reply = null;

			try
			{
				Task<PingReply?> task = Task.Run(() => PingIPAddressAsync(Address), token);

				reply = await task.ConfigureAwait(false);
			}
			catch (TaskCanceledException)
			{
				Status = PingStatus.Cancelled;
			}

			ParsePingReply(reply);

			RoundtripTime = (reply is not null) ? reply.RoundtripTime : -1;
		}

		protected static async Task<PingReply?> PingIPAddressAsync(IPAddress ip)
		{
			PingReply? reply = null;

			try
			{
				using (Ping ping = new Ping())
				{
					reply = await ping.SendPingAsync(ip, timeout).ConfigureAwait(false);
				}
			}
			catch (PingException) { }

			return reply;
		}

		protected virtual void ParsePingReply(PingReply? reply)
		{
			Status = reply switch
			{
				PingReply pingReply => pingReply.Status switch
				{
					IPStatus.Success => PingStatus.Success,
					_ => PingStatus.Failure
				},
				null => PingStatus.Failure
			};
		}
	}
}
