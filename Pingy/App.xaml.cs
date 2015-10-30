using System.Windows;

namespace Pingy
{
    public partial class App : Application
    {
        public IRepo Repo { get; set; }

        public App(IRepo repo)
        {
            this.Repo = repo;
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Utils.LogException(e.Exception);
        }
    }
}
