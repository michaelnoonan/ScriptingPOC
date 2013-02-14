using System;
using System.Threading;
using System.Windows.Forms;
using Caliburn.Micro;
using MyCoolApp.Development;
using MyCoolApp.Diagnostics;

namespace MyCoolApp
{
    static class Program
    {
        public static EventAggregator GlobalEventAggregator = new EventAggregator();

        [STAThread]
        static void Main()
        {
            Application.ApplicationExit += ShutDown;
            Application.ThreadException += ThreadException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;

            HostApplicationServiceHost.Instance.StartListening();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Shell());
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Instance.Error((Exception)e.ExceptionObject, "Unhandled Exception: ");
        }

        private static void ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Logger.Instance.Error(e.Exception, "Thread Exception: ");
        }

        private static void ShutDown(object sender, EventArgs e)
        {
            SharpDevelopAdapter.Instance.Dispose();
            HostApplicationServiceHost.Instance.Dispose();
        }
    }
}
