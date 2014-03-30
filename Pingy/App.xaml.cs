using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Pingy
{
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string logFilePath = string.Format("C:\\Users\\{0}\\Documents\\logfile.txt", Environment.UserName);

            using (StreamWriter sw = new StreamWriter(logFilePath))
            {
                sw.WriteLine(string.Format("\n{0}: {1}: {2}: {3}\n", DateTime.Now, Application.Current.ToString(), e.Exception.ToString(), e.Exception.Message.ToString()));
            }

            e.Handled = true;
        }
    }
}
