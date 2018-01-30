using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Pingy
{
    public static class FileSystem
    {
        public static async Task<string[]> GetLinesAsync(FileInfo file)
        {
            if (file == null) { throw new ArgumentNullException(nameof(file)); }
            if (!file.Exists) { throw new FileNotFoundException(nameof(file)); }
            
            var lines = new List<string>();

            FileStream fsAsync = new FileStream(
                file.FullName,
                FileMode.Open,
                FileAccess.Read,
                FileShare.None,
                4096,
                FileOptions.Asynchronous | FileOptions.SequentialScan);

            using (StreamReader sr = new StreamReader(fsAsync))
            {
                fsAsync = null;

                string line = string.Empty;

                while ((line = await sr.ReadLineAsync().ConfigureAwait(false)) != null)
                {
                    lines.Add(line);
                }
            }

            fsAsync?.Close();

            return lines.ToArray();
        }
    }
}
