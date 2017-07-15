﻿using System;
using System.Globalization;
using System.IO;

namespace Pingy
{
    public static class Program
    {
        [STAThread]
        public static int Main()
        {
            string filePath = GetAddressesFilePath();

            var repo = new TxtRepo(filePath);

            var app = new App(repo);

            int exitCode = app.Run();

            if (exitCode != 0)
            {
                string errorMessage = string.Format(CultureInfo.CurrentCulture, "exited with code: {0}", exitCode);

                Log.LogMessage(errorMessage);
            }

            return exitCode;
        }

        private static string GetAddressesFilePath()
        {
            string dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filename = "PingyAddresses.txt";

            return Path.Combine(dir, filename);
        }
    }
}
