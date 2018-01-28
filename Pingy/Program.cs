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
            FileInfo file = GetAddressesFile(directory, filename);

            if (file == null)
            {
                string errorMessage = string.Format(CultureInfo.CurrentCulture, "exited: either directory ({0}) doesn't exist, or the file ({1}) couldn't be created", directory, filename);

                Log.LogMessage(errorMessage);

                return -1;
            }

            App app = new App(file);

            int exitCode = app.Run();
            
            if (exitCode != 0)
            {
                string errorMessage = string.Format(CultureInfo.CurrentCulture, "Pingy exited with code: {0}", exitCode);

                Log.LogMessage(errorMessage);
            }
            
            return exitCode;
        }

        private static FileInfo GetAddressesFile(string directory, string filename)
        {
            if (!Directory.Exists(directory))
            {
                return null;
            }

            string fullPath = Path.Combine(directory, filename);

            if (!File.Exists(fullPath))
            {
                try
                {
                    using (StreamWriter sw = File.CreateText(fullPath))
                    {
                        sw.WriteLine("yahoo.com"); // unimportant default
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    return null;
                }
            }
            
            return new FileInfo(fullPath);
        }
    }
}
