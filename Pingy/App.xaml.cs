using System.Windows;
using System.Windows.Threading;

namespace Pingy
{
    public partial class App : Application
    {
        public IRepo Repo { get; set; }

        public App(IRepo repo)
        {
            Repo = repo;
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Log.LogException(e.Exception);
        }
    }
}
