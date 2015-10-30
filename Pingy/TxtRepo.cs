using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pingy
{
    public interface IRepo
    {
        Task<IReadOnlyCollection<string>> LoadAsync();
    }

    public class TxtRepo : IRepo
    {
        private string filePath = string.Empty;

        public TxtRepo(string filePath)
        {
            this.filePath = filePath;
        }

        public async Task<IReadOnlyCollection<string>> LoadAsync()
        {
            List<string> addresses = new List<string>();

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
                            addresses.Add(line);
                        }
                    }
                }
            }
            catch (FileNotFoundException e)
            {
                addresses = null;

                Utils.LogException(e, "file not found");
            }

            return addresses != null
                ? addresses.AsReadOnly()
                : (IReadOnlyCollection<string>)Enumerable.Empty<string>();
        }
    }
}
