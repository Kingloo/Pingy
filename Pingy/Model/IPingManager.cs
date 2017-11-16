using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Pingy.Model
{
    public interface IPingManager
    {
        FileInfo File { get; }

        IReadOnlyCollection<IPingable> Addresses { get; }

        void OpenAddressesFile();
        void SetFile(FileInfo file);

        Task LoadAddressesAsync();
        Task PingAllAsync();
    }
}
