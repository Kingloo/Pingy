using System;
using System.Diagnostics;
using System.IO;

namespace Pingy.Extensions
{
    public static class FileInfoExtensions
    {
        public static void Launch(this FileSystemInfo file)
        {
            if (file is null) { throw new ArgumentNullException(nameof(file)); }

            if (file.Exists)
            {
                ProcessStartInfo pInfo = new ProcessStartInfo(file.FullName)
                {
                    UseShellExecute = true
                };

                Process.Start(pInfo);
            }
        }
    }
}
