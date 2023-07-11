using smash.plugin;

namespace smash.plugins.sysProxy
{
    public partial class SysProxyForm : Form, ITabForm
    {
        public int Order => 2;

        private readonly SysProxyConfig sysProxyConfig;
        public SysProxyForm(SysProxyConfig sysProxyConfig)
        {
            this.sysProxyConfig = sysProxyConfig;
            InitializeComponent();
            BindInfo();
        }

        private void BindInfo()
        {
            BindSysProxy();
        }

        #region 系统代理
        private void BindSysProxy()
        {
            int index = cmbSysProxy.SelectedIndex;
            cmbSysProxy.DataSource = null;
            if (sysProxyConfig.SysProxys != null)
            {
                cmbSysProxy.DataSource = sysProxyConfig.SysProxys.Select(c => c.Name).ToList();
            }
            cbUseSysProxy.Checked = sysProxyConfig.UseSysProxy;
            if (index < sysProxyConfig.SysProxys.Count && sysProxyConfig.SysProxys.Count > 0)
            {
                cmbSysProxy.SelectedIndex = index < 0 ? 0 : index;
            }
        }
        private void cmbSysProxy_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSysProxy.SelectedItem != null)
            {
                sysProxyConfig.SysProxy = sysProxyConfig.SysProxys.FirstOrDefault(c => c.Name == cmbSysProxy.SelectedItem.ToString());
                lbPac.Text = $"{sysProxyConfig.SysProxy.Pac}";
                cbIsPac.Checked = sysProxyConfig.SysProxy.IsPac;
                cbIsEnv.Checked = sysProxyConfig.SysProxy.IsEnv;
            }
        }
        private void UpdateConfig()
        {
            sysProxyConfig.UseSysProxy = cbUseSysProxy.Checked;
            if (sysProxyConfig.SysProxy != null)
            {
                sysProxyConfig.SysProxy.IsEnv = cbIsEnv.Checked;
                sysProxyConfig.SysProxy.IsPac = cbIsPac.Checked;
            }
            BindInfo();
        }
        private void CheckedChanged(object sender, EventArgs e)
        {
            UpdateConfig();
        }
        #endregion
    }
}
