using smash.plugin;
using System.Data;

namespace smash.plugins.hijack
{
    public partial class HijackForm : Form, ITabForm
    {
        public int Order => 1;
        private readonly HijackConfig hijackConfig;
        public HijackForm(HijackConfig hijackConfig)
        {
            this.hijackConfig = hijackConfig;
            InitializeComponent();
            BindInfo();
        }

        private void BindInfo()
        {
            BindGroup();
        }

        #region 进程劫持
        private void cmbGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbGroup.SelectedItem != null)
            {
                hijackConfig.Process = hijackConfig.Processs.FirstOrDefault(c => c.Name == cmbGroup.SelectedItem.ToString());
            }
        }
        private void BindGroup()
        {
            int index = cmbGroup.SelectedIndex;
            cmbGroup.DataSource = null;
            cmbGroup.DataSource = hijackConfig.Processs.Select(c => c.Name).ToList();
            cbUseHijack.Checked = hijackConfig.UseHijack;
            cbFilterTcp.Checked = hijackConfig.FilterTCP;
            cbFilterUdp.Checked = hijackConfig.FilterUDP;
            cbFilterDns.Checked = hijackConfig.FilterDNS;
            if (index < hijackConfig.Processs.Count && hijackConfig.Processs.Count > 0)
            {
                cmbGroup.SelectedIndex = index < 0 ? 0 : index;
            }
        }
        private void UpdateConfig()
        {
            hijackConfig.UseHijack = cbUseHijack.Checked;
            hijackConfig.FilterTCP = cbFilterTcp.Checked;
            hijackConfig.FilterUDP = cbFilterUdp.Checked;
            hijackConfig.FilterDNS = cbFilterDns.Checked;
            hijackConfig.Save();
            BindInfo();
        }
        private void CheckedChanged(object sender, EventArgs e)
        {
            UpdateConfig();
        }
        #endregion
    }
}
