using smash.plugin;
using System.Data;

namespace smash.plugins.proxy
{
    public partial class ProxyForm : Form, ITabForm
    {
        private readonly ProxyConfig proxyConfig;
        public ProxyForm(ProxyConfig proxyConfig)
        {
            this.proxyConfig = proxyConfig;
            InitializeComponent();
            BindInfo();
        }

        private void BindInfo()
        {
            BindProxy();
        }

        #region 代理
        private void BindProxy()
        {
            int index = cmbProxy.SelectedIndex;
            cmbProxy.DataSource = null;
            if (proxyConfig.Proxy != null)
            {
                cmbProxy.DataSource = proxyConfig.Proxys.Select(c => c.Name).ToList();
            }
            if (index < proxyConfig.Proxys.Count && proxyConfig.Proxys.Count > 0)
            {
                cmbProxy.SelectedIndex = index < 0 ? 0 : index;
            }
        }
        private void cmbProxy_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbProxy.SelectedItem != null)
            {
                proxyConfig.Proxy = proxyConfig.Proxys.FirstOrDefault(c => c.Name == cmbProxy.SelectedItem.ToString());
                labelProxy.Text = $"{proxyConfig.Proxy.Host}:{proxyConfig.Proxy.Port}";
            }
        }
        #endregion
    }
}
