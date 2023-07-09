using smash.plugin;

namespace smash.plugins.sysProxy
{
    public partial class SysProxyForm : Form, ITabForm
    {
        private readonly SysProxyConfig sysProxyConfig;
        public SysProxyForm(SysProxyConfig sysProxyConfig)
        {
            this.sysProxyConfig = sysProxyConfig;
            InitializeComponent();
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
        private void cbUseSysProxy_CheckedChanged(object sender, EventArgs e)
        {
            sysProxyConfig.UseSysProxy = cbUseSysProxy.Checked;
            BindInfo();
        }
        private void cbIsEnv_CheckedChanged(object sender, EventArgs e)
        {
            if (sysProxyConfig.SysProxy != null)
            {
                sysProxyConfig.SysProxy.IsEnv = cbIsEnv.Checked;
                BindInfo();
            }
        }
        private void cbIsPac_CheckedChanged(object sender, EventArgs e)
        {
            if (sysProxyConfig.SysProxy != null)
            {
                sysProxyConfig.SysProxy.IsPac = cbIsPac.Checked;
                BindInfo();
            }
        }
        #endregion
    }
}
