using System;

namespace Pingy
{
    public class Program
    {
        private static readonly string addressesFilePath = string.Format(@"C:\Users\{0}\Documents\PingyAddresses.txt", Environment.UserName);

        [STAThread]
        public static int Main()
        {
            TxtRepo repo = new TxtRepo(addressesFilePath);

            App app = new App(repo);
            app.InitializeComponent();

            int exitCode = app.Run();

            if (exitCode != 0)
            {
                string errorMessage = string.Format("exited with code: {0}", exitCode);

                Utils.LogMessage(errorMessage);
            }

            return exitCode;
        }
    }
}
