using System;
using System.Windows.Forms;

namespace ServerSuperIO.Device
{
    public partial class MonitorChannelForm : Form
    {
        public MonitorChannelForm(string devname)
        {
            InitializeComponent();
            this.Text = this.Text + "-" + devname;
        }
        private  Action<string> _std = null;

        public void Update(string content)
        {
             if (!this.IsDisposed)
            {
                if (this.listBox1.InvokeRequired)
                {
                    this.BeginInvoke(_std, new object[] { content });
                    return;
                }
                else
                {
                    if (this.listBox1.Items.Count >= 1000)
                        this.listBox1.Items.Clear();
                    this.listBox1.Items.Insert(0, content);
                }
            }
        }

        private void PastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string text = "";
                for (int i = 0; i < this.listBox1.Items.Count; i++)
                {
                    text += this.listBox1.Items[i].ToString() + "\r\n";
                }
                Clipboard.Clear();
                Clipboard.SetText(text, TextDataFormat.Text);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MonitorIOForm_Load(object sender, EventArgs e)
        {
            _std = new Action<string>(this.Update);
        }
    }
}