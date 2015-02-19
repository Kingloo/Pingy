using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Pingy
{
    public class Program
    {
        private static string addressesFilePath = string.Format(@"C:\Users\{0}\Documents\PingyAddresses.txt", Environment.UserName);

        private static List<string> _addresses = new List<string>();
        public static List<string> Addresses { get { return _addresses; } }

        [STAThread]
        public static int Main(string[] args)
        {
            LoadFromFileAsync().Wait();

            App app = new App();
            app.InitializeComponent();

            return app.Run();
        }

        public async static Task LoadFromFileAsync()
        {
            FileStream fsAsync = null;

            try
            {
                fsAsync = new FileStream(addressesFilePath, FileMode.Open, FileAccess.Read, FileShare.None, 1024, true);
            }
            catch (FileNotFoundException)
            {
                if (fsAsync != null)
                {
                    fsAsync.Close();
                }

                File.Create(addressesFilePath);

                return;
            }

            using (StreamReader sr = new StreamReader(fsAsync))
            {
                string line = string.Empty;

                while ((line = await sr.ReadLineAsync().ConfigureAwait(false)) != null)
                {
                    if (line.StartsWith("#") == false)
                    {
                        _addresses.Add(line);
                    }
                }
            }

            if (fsAsync != null)
            {
                fsAsync.Close();
            }
        }
    }
}
