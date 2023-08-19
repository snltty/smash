using smash.plugin;
using smash.plugins.hijack;
using smash.plugins.proxy;
using System.Diagnostics;

namespace smash.plugins
{
    public partial class HijackProcessForm : Form, ITabForm
    {
        public int Order => 0;

        private readonly HijackConfig hijackConfig;
        public HijackProcessForm(HijackConfig hijackConfig)
        {
            this.hijackConfig = hijackConfig;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            InitializeComponent();

            BindData();

            processView.RowHeadersVisible = false;
            processView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            processView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            processView.ContextMenuStrip = contextMenu;
            processView.CellMouseUp += ProxysView_CellMouseUp;
            processView.SelectionChanged += ProxysView_SelectionChanged;
            processView.CellEndEdit += ProxysView_CellEndEdit;
            processView.CellContentClick += ProxysView_CellContentClick;

            processList.ContextMenuStrip = contextMenuList;
        }
        private void BindData()
        {
            processView.DataSource = null;
            processView.DataSource = hijackConfig.Processs;

            processView.Columns["Use"].HeaderText = "使用";
            processView.Columns["Name"].HeaderText = "名称";
            processView.Columns["TCP"].HeaderText = "TCP";
            processView.Columns["UDP"].HeaderText = "UDP";
            processView.Columns["DNS"].HeaderText = "DNS";

            hijackConfig.Save();
        }
        private void ProxysView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                hijackConfig.Processs[e.RowIndex].Use = !hijackConfig.Processs[e.RowIndex].Use;
                hijackConfig.Save();
            }
        }
        private void ProxysView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            hijackConfig.Save();
        }
        private void ProxysView_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.RowIndex >= 0)
                {
                    processView.ClearSelection();
                    processView.Rows[e.RowIndex].Selected = true;
                }
            }
        }

        ProcessInfo processInfo;
        private void ProxysView_SelectionChanged(object sender, EventArgs e)
        {
            if (processView.SelectedRows.Count > 0)
            {
                processInfo = processView.SelectedRows[0].DataBoundItem as ProcessInfo;
            }
            else
            {
                processInfo = null;
            }
            BindFileNames();
        }


        private void BindFileNames()
        {
            processList.DataSource = null;
            if (processInfo == null) return;
            processList.DataSource = processInfo.FileNames;
        }

        private void btnDelProcess_Click(object sender, EventArgs e)
        {

            if (processInfo != null && processList.SelectedItem != null)
            {
                processInfo.FileNames.Remove(processList.SelectedItem.ToString());
            }
            BindFileNames();

            hijackConfig.Save();
        }
        private void btnAddProcess_Click(object sender, EventArgs e)
        {
            if (processInfo == null) return;

            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Filter = "exe文件(*.exe)|*.exe";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                if (fileDialog.FileNames.Length > 0)
                {
                    foreach (var item in fileDialog.FileNames)
                    {
                        processInfo.FileNames.Add(Path.GetFileName(item));
                    }
                    BindFileNames();
                    hijackConfig.Save();
                }
            }
        }
        private void OnMainMenuDelGroup(object sender, EventArgs e)
        {
            if (hijackConfig.Processs.Count <= 1) return;
            hijackConfig.Processs.Remove(processInfo);

            BindData();
        }

        private void OnMainMenuAddGroupClick(object sender, EventArgs e)
        {
            if (hijackConfig.Processs.FirstOrDefault(c => c.Name == "新项") != null)
            {
                MessageBox.Show("已存在一个新项");
                return;
            }
            hijackConfig.Processs.Add(new ProcessInfo
            {
                Name = "新项",
            });
            BindData();
            processView.ClearSelection();
            processView.Rows[hijackConfig.Processs.Count - 1].Selected = true;
        }
    }
}
