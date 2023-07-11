using common.libs;
using smash.plugin;

namespace smash.plugins.about
{
    public partial class AboutForm : Form, ITabForm
    {
        public int Order => int.MaxValue;
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
            CommandHelper.Windows(string.Empty, new string[] { $"start https://github.com/snltty/smash" });
        }
    }
}
