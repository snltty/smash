using common.libs;
using smash.plugins.proxy;
using System.Net;
using System.Net.NetworkInformation;

namespace smash.plugins
{
    public partial class ProxySettingForm : Form
    {
        private readonly ProxyConfig proxyConfig;
        public ProxySettingForm(ProxyConfig proxyConfig)
        {
            this.proxyConfig = proxyConfig;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            InitializeComponent();

            BindData();
            proxysView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            proxysView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            proxysView.ContextMenuStrip = contextMenu;
            proxysView.CellMouseUp += ProxysView_CellMouseUp;
            proxysView.SelectionChanged += ProxysView_SelectionChanged;
            proxysView.CellFormatting += ProxysView_CellFormatting;

            ShowUsed();
            Ping();
        }

      
        private void BindData()
        {
            proxysView.DataSource = null;
            proxysView.DataSource = proxyConfig.Proxys;
            proxysView.Columns["Name"].HeaderText = "名称";
            proxysView.Columns["Host"].HeaderText = "主机";
            proxysView.Columns["Port"].HeaderText = "端口";
            proxysView.Columns["UserName"].HeaderText = "账号";
            proxysView.Columns["Password"].HeaderText = "密码";
            proxysView.Columns["Delay"].HeaderText = "延迟";
            proxyConfig.Save();
        }

        ProxyInfo proxy;
        private void ProxysView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 4)
            {
                if (e.Value != null && e.Value.ToString().Length > 0)
                {
                    e.Value = new string('*', e.Value.ToString().Length);
                }
            }
        }
        private void ProxysView_SelectionChanged(object sender, EventArgs e)
        {
            if (proxysView.SelectedRows.Count > 0)
            {
                proxy = proxysView.SelectedRows[0].DataBoundItem as ProxyInfo;
                inputName.Text = proxy.Name;
                inputHost.Text = proxy.Host;
                inputPort.Text = proxy.Port.ToString();
                inputUserName.Text = proxy.UserName;
                inputPassword.Text = proxy.Password;
            }
            else
            {
                proxy = null;
                inputName.Text = string.Empty;
                inputHost.Text = string.Empty;
                inputPort.Text = string.Empty;
                inputUserName.Text = string.Empty;
                inputPassword.Text = string.Empty;
            }
        }
        private void ProxysView_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.RowIndex >= 0)
                {
                    proxysView.ClearSelection();
                    proxysView.Rows[e.RowIndex].Selected = true;
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            proxysView.ClearSelection();
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(inputHost.Text) || string.IsNullOrWhiteSpace(inputName.Text) || string.IsNullOrWhiteSpace(inputPort.Text))
            {
                MessageBox.Show("名称，主机，必填");
                return;
            }

            IPAddress ip = NetworkHelper.GetDomainIp(inputHost.Text);
            if (ip == null)
            {
                MessageBox.Show("主机无效");
                return;
            }
            if (int.TryParse(inputPort.Text, out int port) == false)
            {
                MessageBox.Show("端口无效");
                return;
            }

            if (proxy != null)
            {
                proxy.Name = inputName.Text;
                proxy.Host = inputHost.Text;
                proxy.Port = port;
                proxy.UserName = inputUserName.Text;
                proxy.Password = inputPassword.Text;
            }
            else
            {
                proxyConfig.Proxys.Add(new ProxyInfo
                {
                    Name = inputName.Text,
                    Host = inputHost.Text,
                    Port = port,
                    UserName = inputUserName.Text,
                    Password = inputPassword.Text
                });

            }
            BindData();
            btnClear.PerformClick();
            proxysView.Rows[proxysView.Rows.Count - 1].Selected = true;
        }

        private void ShowUsed()
        {
            if (proxyConfig.Proxy == null && proxyConfig.Proxys.Count > 0)
            {
                Use(proxyConfig.Proxys.FirstOrDefault());
            }
            useName.Text = proxyConfig.Proxy.Name;
            useHost.Text = $"{proxyConfig.Proxy.Host}:{proxyConfig.Proxy.Port}";
        }
        private void Use(ProxyInfo proxy)
        {
            if (proxy == null) return;
            proxyConfig.Proxy = proxy;
        }

        private bool isPing = false;
        CancellationTokenSource cts = new CancellationTokenSource();
        private void CancelPing()
        {
            cts.Cancel();
            isPing = false;
        }
        private void Ping()
        {
            isPing = true;
            Task.Run(async () =>
            {
                while (isPing)
                {
                    foreach (ProxyInfo item in proxyConfig.Proxys)
                    {
                        IPAddress ip = NetworkHelper.GetDomainIp(item.Host);
                        if (ip == null) continue;

                        using Ping ping = new Ping();
                        _ = ping.SendPingAsync(ip).ContinueWith((res, obj) =>
                        {
                            ProxyInfo proxy = obj as ProxyInfo;
                            PingReply delay = res.Result;
                            if (delay.Status == IPStatus.Success)
                            {
                                proxy.Delay = delay.RoundtripTime;
                            }
                            else
                            {
                                proxy.Delay = -1;
                            }

                        }, item);


                    }
                    await Task.Delay(1000);
                }
            }, cts.Token);
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            CancelPing();
        }

        private void showPassword_CheckedChanged(object sender, EventArgs e)
        {
            inputPassword.PasswordChar = showPassword.Checked ? '\0' : '*';
        }

        private void MainMenuDelProxy_Click(object sender, EventArgs e)
        {
            if (proxyConfig.Proxys.Count <= 1) return;
            proxyConfig.Proxys.Remove(proxy);
            BindData();
            btnClear.PerformClick();
        }
        private void MainManuUseProxy_Click(object sender, EventArgs e)
        {
            Use(proxy);
            ShowUsed();
        }
    }
}
