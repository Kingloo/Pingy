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
            FileInfo file = null;

            try
            {
                file = GetAddressesFile();
            }
            catch (DirectoryNotFoundException ex)
            {
                string errorMessage = string.Format(CultureInfo.CurrentCulture, "Pingy exited with code: {0}", -1);

                Log.LogException(ex, errorMessage);

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

        private static FileInfo GetAddressesFile()
        {
            if (!Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "directory doesn't exist: {0}",
                        directory));
            }
            
            string fullPath = Path.Combine(directory, filename);
            
            return File.Exists(fullPath)
                ? new FileInfo(fullPath)
                : CreateFile(fullPath, "yahoo.com");
        }

        private static FileInfo CreateFile(string fullPath, string defaultDomainName)
        {
            using (StreamWriter sw = File.CreateText(fullPath))
            {
                sw.WriteLine(defaultDomainName);
            }

            return new FileInfo(fullPath);
        }
    }
}
