using smash.plugin;
using smash.plugins.sysProxy;
using System.Diagnostics;

namespace smash.plugins
{
    public partial class SysProxySettingForm : Form, ITabForm
    {
        public int Order => 2;

        private readonly SysProxyConfig sysProxyConfig;
        public SysProxySettingForm(SysProxyConfig sysProxyConfig)
        {
            this.sysProxyConfig = sysProxyConfig;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            InitializeComponent();

            sysProxysView.RowHeadersVisible = false;
            sysProxysView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            sysProxysView.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
            sysProxysView.ContextMenuStrip = contextMenu;
            sysProxysView.CellMouseUp += SysProxysView_CellMouseUp; ;
            sysProxysView.SelectionChanged += SysProxysView_SelectionChanged;
            sysProxysView.CellEndEdit += SysProxysView_CellEndEdit;
            sysProxysView.CellContentClick += SysProxysView_CellContentClick;
            BindData();
        }

        private void SysProxysView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                int count = sysProxysView.Rows.Count;
                for (int i = 0; i < count; i++)
                {
                    if (i != e.RowIndex)
                        sysProxysView.Rows[i].Cells[0].Value = false;
                }
                sysProxyConfig.SysProxys[e.RowIndex].Use = true;
                sysProxyConfig.Save();
            }
        }

        private void SysProxysView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            sysProxyConfig.Save();
        }

        private void BindData()
        {
            sysProxysView.DataSource = null;
            sysProxysView.DataSource = sysProxyConfig.SysProxys;

            sysProxysView.Columns["Use"].HeaderText = "使用";
            sysProxysView.Columns["Name"].HeaderText = "名称";
            sysProxysView.Columns["IsEnv"].HeaderText = "环境变量";
            sysProxysView.Columns["IsPac"].HeaderText = "设置PAC";
            sysProxysView.Columns["Pac"].HeaderText = "Pac文件";
            sysProxysView.Columns["Pac"].Visible = false;

            DataGridViewComboBoxColumn c = new DataGridViewComboBoxColumn();
            c.DataSource = GetPacFiles();
            c.DataPropertyName = "Pac";
            c.HeaderText = "pac文件";
            //c.DefaultCellStyle.NullValue = "default.pac";
            sysProxysView.Columns.Add(c);

            sysProxysView.Columns["Use"].FillWeight = 50;
            sysProxysView.Columns["Name"].FillWeight = 50;
            sysProxysView.Columns["IsEnv"].FillWeight = 50;
            sysProxysView.Columns["IsPac"].FillWeight = 50;

            sysProxyConfig.Save();
        }

        SysProxyInfo proxy;
        private void SysProxysView_SelectionChanged(object sender, EventArgs e)
        {
            if (sysProxysView.SelectedRows.Count > 0)
            {
                proxy = sysProxysView.SelectedRows[0].DataBoundItem as SysProxyInfo;
            }
            else
            {
                proxy = null;
            }
        }
        private void SysProxysView_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.RowIndex >= 0)
                {
                    sysProxysView.ClearSelection();
                    sysProxysView.Rows[e.RowIndex].Selected = true;
                }
            }
        }

        private void MainMenuDelProxy_Click(object sender, EventArgs e)
        {
            if (sysProxyConfig.SysProxys.Count <= 1) return;
            sysProxyConfig.SysProxys.Remove(proxy);
            BindData();
        }
        private void OnMainMenuAddProxyClick(object sender, EventArgs e)
        {
            if (sysProxyConfig.SysProxys.FirstOrDefault(c => c.Name == "新项") != null)
            {
                MessageBox.Show("已存在一个新项");
                return;
            }
            sysProxyConfig.SysProxys.Add(new SysProxyInfo
            {
                IsEnv = false,
                IsPac = false,
                Name = "新项",
                Pac = string.Empty
            });
            BindData();
            sysProxysView.ClearSelection();
            sysProxysView.Rows[sysProxyConfig.SysProxys.Count - 1].Selected = true;
        }

        private void OnBtnPacPathClick(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", $"{Path.GetFullPath(sysProxyConfig.PacRoot)}");
        }

        private List<string> GetPacFiles()
        {
            return Directory.GetFiles(sysProxyConfig.PacRoot).Select(c => Path.GetFileName(c)).Where(c => c.StartsWith("--") == false).ToList();
        }
    }
}
