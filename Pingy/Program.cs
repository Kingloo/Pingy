using System;
using System.Globalization;
using System.IO;

namespace Pingy
{
    public static class Program
    {
#if DEBUG
        private static string filename = "PingyAddresses-test.txt";
#else
        private static string filename = "PingyAddresses.txt";
#endif
        private static string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        [STAThread]
        public static int Main()
        {
            FileInfo file = GetAddressesFile();
            
            App app = new App(file);

            int exitCode = app.Run();

            if (exitCode != 0)
            {
                string message = string.Format(CultureInfo.CurrentCulture, "", exitCode);

                Log.LogMessage(message);
            }
            
            return exitCode;
        }

        private static FileInfo GetAddressesFile()
        {
            if (!Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException(string.Format(CultureInfo.CurrentCulture, "directory doesn't exist: {0}", directory));
            }

            string fullPath = Path.Combine(directory, filename);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException("file doesn't exist", fullPath);
            }

            return new FileInfo(fullPath);
        }
    }
}
