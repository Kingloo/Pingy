using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Pingy
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChangedEventHandler pceh = this.PropertyChanged;

            if (pceh != null)
            {
                PropertyChangedEventArgs args = new PropertyChangedEventArgs(propertyName);

                pceh(this, args);
            }
        }
    }
}
