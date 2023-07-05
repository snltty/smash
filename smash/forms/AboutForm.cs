using smash.libs;

namespace smash.forms
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Command.Windows(string.Empty,new string[] {$"start https://github.com/snltty/smash" });
        }
    }
}
