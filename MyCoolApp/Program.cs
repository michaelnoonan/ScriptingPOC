using System;
using System.Threading;
using System.Windows.Forms;
using MyCoolApp.Domain;
using MyCoolApp.Domain.Development;
using MyCoolApp.Domain.Diagnostics;
using MyCoolApp.Domain.Events;

namespace MyCoolApp
{
    static class Program
    {
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
            SharpDevelopIntegrationService.Instance.Dispose();
            HostApplicationServiceHost.Instance.Dispose();
        }
    }
}
