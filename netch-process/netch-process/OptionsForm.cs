using netch_process.libs;
using System.Net;

namespace netch_process
{
    public partial class OptionsForm : Form
    {
        Config config;
        public OptionsForm(Config config)
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
            filterSubProcess.Checked = config.FilterParent;
            filterDNS.Checked = config.FilterDNS;
            filterOnlyDNS.Checked = config.HandleOnlyDNS;
            dnsProxy.Checked = config.DNSProxy;
            dnsOnly.Checked = config.DNSProxy == false;
            dnsServer.Text = $"{config.DNSHost}:{config.DNSPort}";
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(dnsServer.Text))
            {
                dnsServer.Text = "8.8.8.8:53";
            }
            if (IPEndPoint.TryParse(dnsServer.Text, out IPEndPoint ep) == false)
            {
                MessageBox.Show($"dns服务器格式有误!");
                return;
            }
            config.DNSHost = ep.Address.ToString();
            config.DNSPort = ep.Port;

            config.FilterTCP = filterTcp.Checked;
            config.FilterUDP = filterUDP.Checked;
            config.FilterParent = filterSubProcess.Checked;
            config.FilterDNS = filterDNS.Checked;
            config.HandleOnlyDNS = filterOnlyDNS.Checked;
            config.DNSProxy = dnsProxy.Checked;
            config.Save();
            MessageBox.Show($"已保存");
        }
    }
}
