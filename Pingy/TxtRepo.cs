using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Pingy
{
    public interface IRepo
    {
        string FilePath { get; }
        Task<IReadOnlyList<Ping>> LoadAsync();
    }

    public class TxtRepo : IRepo
    {
        private string _filePath = string.Empty;
        public string FilePath => _filePath;

        public TxtRepo(string filePath) => _filePath = filePath;

        public async Task<IReadOnlyList<Ping>> LoadAsync()
        {
            List<Ping> addresses = new List<Ping>();

            FileStream fsAsync = null;

            try
            {
                fsAsync = new FileStream(_filePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.None,
                    2048,
                    true);
                
                using (StreamReader sr = new StreamReader(fsAsync))
                {
                    fsAsync = null;

                    string line = string.Empty;

                    while ((line = await sr.ReadLineAsync().ConfigureAwait(false)) != null)
                    {
                        if (line.StartsWith("#")) { continue; } // allows for comment lines

                        if (Ping.TryCreate(line, out Ping ping))
                        {
                            addresses.Add(ping);
                        }
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                Utils.LogException(ex, "addresses file not found");
            }
            finally
            {
                fsAsync?.Dispose();
            }

            return addresses;
        }
    }
}
