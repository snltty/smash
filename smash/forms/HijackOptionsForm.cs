using smash.libs;
using System.Net;

namespace smash.forms
{
    public partial class HijackOptionsForm : Form
    {
        Config config;
        public HijackOptionsForm(Config config)
        {
            this.config = config;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            InitializeComponent();

            ShowOptions();
        }

        private void ShowOptions()
        {
            filterTcp.Checked = config.FilterTCP;
            filterUDP.Checked = config.FilterUDP;
            filterDNS.Checked = config.FilterDNS;
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            config.FilterTCP = filterTcp.Checked;
            config.FilterUDP = filterUDP.Checked;
            config.FilterDNS = filterDNS.Checked;
            config.Save();
            MessageBox.Show($"已保存");
        }
    }
}
