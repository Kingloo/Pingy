using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pingy
{
    public interface IRepo
    {
        string FilePath { get; }
        Task<IEnumerable<Ping>> LoadAsync();
    }

    public class TxtRepo : IRepo
    {
        private string filePath = string.Empty;
        public string FilePath { get { return filePath; } }

        public TxtRepo(string filePath)
        {
            this.filePath = filePath;
        }

        public async Task<IEnumerable<Ping>> LoadAsync()
        {
            List<Ping> addresses = new List<Ping>();

            try
            {
                using (FileStream fsAsync = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None, 1024, true))
                using (StreamReader sr = new StreamReader(fsAsync))
                {
                    string line = string.Empty;

                    while ((line = await sr.ReadLineAsync().ConfigureAwait(false)) != null)
                    {
                        if (line.StartsWith("#") == false && line.Length > 0)
                        {
                            Ping ping = new Ping(line);

                            addresses.Add(ping);
                        }
                    }
                }
            }
            catch (FileNotFoundException e)
            {
                addresses = null;

                Utils.LogException(e, "addresses file not found");
            }

            return addresses != null
                ? addresses
                : Enumerable.Empty<Ping>();
        }
    }
}
