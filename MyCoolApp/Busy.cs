using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace MyCoolApp
{
    public partial class Busy : Form
    {
        private readonly CancellationTokenSource _cancellationTokenSource;

        public Busy()
        {
            InitializeComponent();
        }

        public Busy(string message, CancellationTokenSource cancellationTokenSource)
            : this()
        {
            _cancellationTokenSource = cancellationTokenSource;
            SetMessage(message);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if (Owner != null && StartPosition == FormStartPosition.Manual)
            {
                Point p = new Point(Owner.Left + Owner.Width / 2 - Width / 2, Owner.Top + Owner.Height / 2 - Height / 2);
                this.Location = p;
            }
        }

        private void Busy_Load(object sender, EventArgs e)
        {
            progressBar1.Style = ProgressBarStyle.Marquee;
            cancelButton.Visible = _cancellationTokenSource != null;
        }

        private bool shouldClose = false;

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (shouldClose == false) e.Cancel = true;
            base.OnClosing(e);
        }

        public void SetMessage(string message)
        {
            label1.Text = message;
        }

        public void NotBusyAnymore()
        {
            shouldClose = true;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            _cancellationTokenSource.Cancel();
            NotBusyAnymore();
        }
    }
}
