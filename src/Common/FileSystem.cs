using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Pingy.Common
{
    public static class FileSystem
    {
        public static async Task<string[]> LoadLinesFromFileAsync(string path, string commentChar)
        {
            List<string> lines = new List<string>();

            FileStream fsAsync = null;

            try
            {
                fsAsync = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan);

                using (StreamReader sr = new StreamReader(fsAsync))
                {
                    fsAsync = null;

                    string line = string.Empty;

                    while ((line = await sr.ReadLineAsync().ConfigureAwait(false)) != null)
                    {
                        if (!line.StartsWith(commentChar))
                        {
                            lines.Add(line);
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                return Array.Empty<string>();
            }
            finally
            {
                fsAsync?.Dispose();
            }

            return lines.ToArray();
        }
    }
}