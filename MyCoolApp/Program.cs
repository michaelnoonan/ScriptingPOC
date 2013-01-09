using System;
using System.Windows.Forms;
using MyCoolApp.Development;

namespace MyCoolApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.ApplicationExit += ShutDown;

            EventListener.Instance.StartListening();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Shell());
        }

        private static void ShutDown(object sender, EventArgs e)
        {
            RemoteControlManager.Instance.Dispose();
            EventListener.Instance.Dispose();
        }
    }
}
