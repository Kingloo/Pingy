using System;

namespace Pingy
{
    public class MainWindowViewModel
    {
        private ObservableCollection<Ping> _pings = new ObservableCollection<Ping>();
        public IReadOnlyCollection<Ping> Pings => _pings;

        public MainWindowViewModel() { }

        public async Task LoadAsync()
        {

        }

        public async Task PingAsync()
        {

        }

        public async Task PingAllAsync()
        {

        }
    }
}