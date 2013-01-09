using System;
using System.Windows.Forms;
using Caliburn.Micro;
using MyCoolApp.Development;

namespace MyCoolApp
{
    static class Program
    {
        public static EventAggregator GlobalEventAggregator = new EventAggregator();

        [STAThread]
        static void Main()
        {
            Application.ApplicationExit += ShutDown;

            EventListenerHost.Instance.StartListening();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Shell());
        }

        private static void ShutDown(object sender, EventArgs e)
        {
            SharpDevelopAdapter.Instance.Dispose();
            EventListenerHost.Instance.Dispose();
        }
    }
}
