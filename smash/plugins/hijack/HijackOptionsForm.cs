using smash.plugins.hijack;
using System.Net;

namespace smash.plugins
{
    public partial class HijackOptionsForm : Form
    {
        private readonly HijackConfig hijackConfig;
        public HijackOptionsForm(HijackConfig hijackConfig)
        {
            this.hijackConfig = hijackConfig;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            InitializeComponent();

            ShowOptions();
        }

        private void ShowOptions()
        {
            filterTcp.Checked = hijackConfig.FilterTCP;
            filterUDP.Checked = hijackConfig.FilterUDP;
            filterDNS.Checked = hijackConfig.FilterDNS;
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            hijackConfig.FilterTCP = filterTcp.Checked;
            hijackConfig.FilterUDP = filterUDP.Checked;
            hijackConfig.FilterDNS = filterDNS.Checked;
            hijackConfig.Save();
            MessageBox.Show($"已保存");
        }
    }
}
