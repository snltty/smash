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

        private void cmbGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbGroup.SelectedItem != null)
            {
                hijackConfig.Process = hijackConfig.Processs.FirstOrDefault(c => c.Name == cmbGroup.SelectedItem.ToString());
                SaveCheckbox();
                hijackConfig.Save();
            }
        }
        private void BindGroup()
        {
            cmbGroup.DataSource = null;
            UpdateCheckbox();

            if (hijackConfig.Process != null)
            {
                int index = 0;
                for (int i = 0; i < hijackConfig.Processs.Count; i++)
                {
                    if (hijackConfig.Processs[i].Name == hijackConfig.Process.Name)
                    {
                        index = i;
                        break;
                    }
                }
                cmbGroup.DataSource = hijackConfig.Processs.Select(c => c.Name).ToList();
                cmbGroup.SelectedIndex = index;
            }
            else if (hijackConfig.Processs.Count > 0)
            {
                cmbGroup.SelectedIndex = 0;
            }
        }
        private void UpdateCheckbox()
        {
            cbUseHijack.Checked = hijackConfig.UseHijack;
            cbFilterTcp.Checked = hijackConfig.FilterTCP;
            cbFilterUdp.Checked = hijackConfig.FilterUDP;
            cbFilterDns.Checked = hijackConfig.FilterDNS;
        }
        private void SaveCheckbox()
        {
            hijackConfig.UseHijack = cbUseHijack.Checked;
            hijackConfig.FilterTCP = cbFilterTcp.Checked;
            hijackConfig.FilterUDP = cbFilterUdp.Checked;
            hijackConfig.FilterDNS = cbFilterDns.Checked;
        }
        private void UpdateConfig()
        {
            SaveCheckbox();
            hijackConfig.Save();
            BindInfo();
        }
        private void CheckedChanged(object sender, EventArgs e)
        {
            UpdateConfig();
        }


        HijackProcessForm hijackProcessForm;
        private void OnMainMenuOptionsClick(object sender, EventArgs e)
        {
            hijackProcessForm = new HijackProcessForm(hijackConfig);
            hijackProcessForm.FormClosed += (a, b) =>
            {
                hijackProcessForm = null;
                BindInfo();
            };
            hijackProcessForm.ShowDialog();
        }
    }
}
