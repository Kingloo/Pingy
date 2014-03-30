﻿using System;
using System.Threading.Tasks;

namespace Pingy
{
    class Ping : ViewModelBase
    {
        public enum PingStatus { None, Updating, Success, Failure };

        #region Properties
        private string _address = string.Empty;
        public string Address
        {
            get { return this._address; }
            set
            {
                this._address = value;
                OnPropertyChanged("Address");
            }
        }

        private PingStatus _status = PingStatus.None;
        public PingStatus Status
        {
            get { return this._status; }
            set
            {
                this._status = value;
                OnPropertyChanged("Status");
            }
        }

        private long _roundtripTime = 0;
        public long RoundtripTime
        {
            get { return this._roundtripTime; }
            set
            {
                this._roundtripTime = value;
                OnPropertyChanged("RoundtripTime");
            }
        }

        private string _tooltip = string.Empty;
        public string Tooltip
        {
            get { return this._tooltip; }
            set
            {
                this._tooltip = value;
                OnPropertyChanged("Tooltip");
            }
        }
        #endregion

        public Ping(string address)
        {
            this.Address = address;
        }

        public async Task PingAsync()
        {
            Status = PingStatus.Updating;
            Tooltip = string.Format("Updating {0} ...", this.Address);

            System.Net.NetworkInformation.PingReply reply = await new System.Net.NetworkInformation.Ping().SendPingAsync(this.Address, 1000);

            if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
            {
                Status = PingStatus.Success;
                RoundtripTime = reply.RoundtripTime;
            }
            else
            {
                Status = PingStatus.Failure;
            }

            Tooltip = string.Format("{0} in {1} ms", reply.Status.ToString(), reply.RoundtripTime.ToString());
        }
    }
}
