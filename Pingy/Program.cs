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
            string fullPath = Path.Combine(directory, filename);
            FileInfo file = new FileInfo(fullPath);

            if (!file.Exists)
            {
                string errorMessage = string.Format(CultureInfo.CurrentCulture, "file doesn't exist: ({0})", directory, filename);

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
    }
}
