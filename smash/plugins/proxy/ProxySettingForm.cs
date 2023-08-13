using common.libs;
using smash.plugin;
using smash.plugins.proxy;
using smash.plugins.sysProxy;
using System.Net;
using System.Net.NetworkInformation;

namespace smash.plugins
{
    public partial class ProxySettingForm : Form, ITabForm
    {
        public int Order => 3;

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

            proxysView.RowHeadersVisible = false;
            proxysView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            proxysView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            proxysView.ContextMenuStrip = contextMenu;
            proxysView.CellMouseUp += ProxysView_CellMouseUp;
            proxysView.SelectionChanged += ProxysView_SelectionChanged;
            proxysView.CellFormatting += ProxysView_CellFormatting;
            proxysView.CellEndEdit += ProxysView_CellEndEdit;
            proxysView.CellContentClick += ProxysView_CellContentClick;
            Ping();
        }

        private void ProxysView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                int count = proxysView.Rows.Count;
                for (int i = 0; i < count; i++)
                {
                    if (i != e.RowIndex)
                        proxysView.Rows[i].Cells[0].Value = false;
                }
                proxyConfig.Proxys[e.RowIndex].Use = true;
                proxyConfig.Save();
            }
        }

        private void ProxysView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            proxyConfig.Save();
        }

        private void BindData()
        {
            proxysView.DataSource = null;
            proxysView.DataSource = proxyConfig.Proxys;

            proxysView.Columns["Use"].HeaderText = "使用";
            proxysView.Columns["Name"].HeaderText = "名称";
            proxysView.Columns["Host"].HeaderText = "主机";
            proxysView.Columns["Port"].HeaderText = "端口";
            proxysView.Columns["UserName"].HeaderText = "账号";
            proxysView.Columns["Password"].HeaderText = "密码";
            proxysView.Columns["Delay"].HeaderText = "延迟";
            proxysView.Columns["Delay"].ReadOnly = true;

            proxyConfig.Save();
        }

        ProxyInfo proxy;
        private void ProxysView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 5)
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
            }
            else
            {
                proxy = null;
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

        private void MainMenuDelProxy_Click(object sender, EventArgs e)
        {
            if (proxyConfig.Proxys.Count <= 1) return;
            proxyConfig.Proxys.Remove(proxy);
            BindData();
        }
        private void MainManuUseProxy_Click(object sender, EventArgs e)
        {
            if (proxyConfig.Proxys.FirstOrDefault(c => c.Name == "新项") != null)
            {
                MessageBox.Show("已存在一个新项");
                return;
            }
            proxyConfig.Proxys.Add(new ProxyInfo
            {
                Delay = 0,
                Host = "127.0.0.1",
                Name = "新项",
                Password = string.Empty,
                Port = 5413,
                UserName = string.Empty
            });
            BindData();
            proxysView.ClearSelection();
            proxysView.Rows[proxyConfig.Proxys.Count - 1].Selected = true;
        }
    }
}
