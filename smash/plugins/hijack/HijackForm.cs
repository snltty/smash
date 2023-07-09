using smash.plugin;
using System.Data;

namespace smash.plugins.hijack
{
    public partial class HijackForm : Form, ITabForm
    {
        private readonly HijackConfig hijackConfig;
        public HijackForm(HijackConfig hijackConfig)
        {
            this.hijackConfig = hijackConfig;
            InitializeComponent();
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
            BindFileNames();
        }
        private void BindGroup()
        {
            int index = cmbGroup.SelectedIndex;
            cmbGroup.DataSource = null;
            cmbGroup.DataSource = hijackConfig.Processs.Select(c => c.Name).ToList();
            cbUseHijack.Checked = hijackConfig.UseHijack;
            if (index < hijackConfig.Processs.Count && hijackConfig.Processs.Count > 0)
            {
                cmbGroup.SelectedIndex = index < 0 ? 0 : index;
            }
        }
        private void BindFileNames()
        {
            listProcess.DataSource = null;
            if (hijackConfig.Process == null) return;
            listProcess.DataSource = hijackConfig.Process.FileNames;
            listProcess.ClearSelected();
        }
        private void cbUseHijack_CheckedChanged(object sender, EventArgs e)
        {
            hijackConfig.UseHijack = cbUseHijack.Checked;
            BindInfo();
        }
        #endregion
    }
}
