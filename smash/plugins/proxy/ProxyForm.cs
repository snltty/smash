using smash.plugin;
using smash.plugins.hijack;
using System.Data;

namespace smash.plugins.proxy
{
    public partial class ProxyForm : Form, ITabForm
    {
        public int Order => 99;

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

        private void BindProxy()
        {
            cmbProxy.DataSource = null;
            if (proxyConfig.Proxy != null)
            {
                int index = 0;
                for (int i = 0; i< proxyConfig.Proxys.Count; i++)
                {
                    if (proxyConfig.Proxys[i].Name == proxyConfig.Proxy.Name)
                    {
                        index = i;
                        break;
                    }
                }
                cmbProxy.DataSource = proxyConfig.Proxys.Select(c => c.Name).ToList();
                cmbProxy.SelectedIndex = index;
            }
            else if(proxyConfig.Proxys.Count > 0)
            {
                cmbProxy.DataSource = proxyConfig.Proxys.Select(c => c.Name).ToList();
                cmbProxy.SelectedIndex = 0;
            }
        }
        private void cmbProxy_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbProxy.SelectedItem != null)
            {
                proxyConfig.Proxy = proxyConfig.Proxys.FirstOrDefault(c => c.Name == cmbProxy.SelectedItem.ToString());
                labelProxy.Text = $"{proxyConfig.Proxy.Host}:{proxyConfig.Proxy.Port}";
                proxyConfig.Save();
            }
        }

        ProxySettingForm proxySettingForm;
        private void OnMainMenuOptionsClick(object sender, EventArgs e)
        {
            proxySettingForm = new ProxySettingForm(proxyConfig);
            proxySettingForm.FormClosed += (a, b) =>
            {
                proxySettingForm = null;
                BindInfo();
            };
            proxySettingForm.ShowDialog();
        }
    }
}
