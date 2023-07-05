using smash.libs;
using System.Net;

namespace smash.forms
{
    public partial class SysProxySettingForm : Form
    {
        Config config;
        public SysProxySettingForm(Config config)
        {
            this.config = config;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            InitializeComponent();


            sysProxysView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            sysProxysView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            sysProxysView.ContextMenuStrip = contextMenu;
            sysProxysView.CellMouseUp += SysProxysView_CellMouseUp; ;
            sysProxysView.SelectionChanged += SysProxysView_SelectionChanged; ;
            BindData();
        }
        private void BindData()
        {
            sysProxysView.DataSource = null;
            sysProxysView.DataSource = config.SysProxys;
            sysProxysView.Columns["Name"].HeaderText = "名称";
            sysProxysView.Columns["IsEnv"].HeaderText = "设置环境变量";
            sysProxysView.Columns["IsPac"].HeaderText = "设置PAC";
            sysProxysView.Columns["Pac"].HeaderText = "Pac地址";
            config.Save();
        }

        SysProxyInfo proxy;
        private void SysProxysView_SelectionChanged(object sender, EventArgs e)
        {
            if (sysProxysView.SelectedRows.Count > 0)
            {
                proxy = sysProxysView.SelectedRows[0].DataBoundItem as SysProxyInfo;
                inputName.Text = proxy.Name;
                cbPac.Checked = proxy.IsPac;
                inputPac.Text = proxy.Pac;
                cbEnv.Checked = proxy.IsEnv;
            }
            else
            {
                proxy = null;
                inputName.Text = string.Empty;
                cbPac.Checked = false;
                inputPac.Text = string.Empty;
                cbEnv.Checked = false;
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
        private void btnClear_Click(object sender, EventArgs e)
        {
            sysProxysView.ClearSelection();
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            config.SysProxys.Remove(proxy);
            btnClear.PerformClick();
            BindData();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(inputName.Text))
            {
                MessageBox.Show("名称必填");
                return;
            }
            if (cbPac.Checked && string.IsNullOrWhiteSpace(inputPac.Text))
            {
                MessageBox.Show("pac地址必填");
                return;
            }
            if (proxy != null)
            {
                proxy.Name = inputName.Text;
                proxy.IsPac = cbPac.Checked;
                proxy.Pac = inputPac.Text;
                proxy.IsEnv = cbEnv.Checked;
            }
            else
            {
                config.SysProxys.Add(new SysProxyInfo
                {
                    Name = inputName.Text,
                    IsPac = cbPac.Checked,
                    Pac = inputPac.Text,
                    IsEnv = cbEnv.Checked
                });
               
            }
            BindData();
            btnClear.PerformClick();
            sysProxysView.Rows[sysProxysView.Rows.Count - 1].Selected = true;
        }
    }
}
