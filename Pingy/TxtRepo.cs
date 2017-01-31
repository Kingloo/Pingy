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
        private string filePath = string.Empty;
        public string FilePath { get { return filePath; } }

        public TxtRepo(string filePath)
        {
            this.filePath = filePath;
        }

        public async Task<IReadOnlyList<Ping>> LoadAsync()
        {
            List<Ping> addresses = new List<Ping>();

            FileStream fsAsync = null;

            try
            {
                fsAsync = new FileStream(filePath,
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

                        if (line.Length > 0)
                        {
                            Ping ping = new Ping(line);

                            addresses.Add(ping);
                        }
                    }
                }
            }
            catch (FileNotFoundException e)
            {
                Utils.LogException(e, "addresses file not found");
            }
            finally
            {
                fsAsync?.Dispose();
            }

            return addresses;
        }
    }
}
