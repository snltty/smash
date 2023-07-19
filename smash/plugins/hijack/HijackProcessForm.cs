using smash.plugins.hijack;
using System.Diagnostics;

namespace smash.plugins
{
    public partial class HijackProcessForm : Form
    {
        private readonly HijackConfig hijackConfig;
        public HijackProcessForm(HijackConfig hijackConfig)
        {
            this.hijackConfig = hijackConfig;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            InitializeComponent();

            listProcess.ContextMenuStrip = contextMenu;
            listProcess.MouseUp += ListProcess_MouseClick;
            cmbGroup.SelectedIndexChanged += CmbGroup_SelectedIndexChanged;
            BindGroup();
        }

        private void ListProcess_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int currentIndex = e.Y / listProcess.ItemHeight;
                if (currentIndex < listProcess.Items.Count)
                {
                    listProcess.SelectedIndex = currentIndex;
                }
            }
        }



        ProcessInfo processInfo;
        private void CmbGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbGroup.SelectedItem != null)
            {
                processInfo = hijackConfig.Processs.FirstOrDefault(c => c.Name == cmbGroup.SelectedItem.ToString());
                editInfo = processInfo;
            }
            BindFileNames();
            BindEdit();
        }
        private void BindGroup()
        {
            cmbGroup.DataSource = null;
            cmbGroup.DataSource = hijackConfig.Processs.Select(c => c.Name).ToList();
        }
        private void BindFileNames()
        {
            listProcess.DataSource = null;
            if (processInfo == null) return;
            listProcess.DataSource = processInfo.FileNames;
        }


        ProcessInfo editInfo;
        private void BindEdit()
        {
            if (editInfo == null)
            {
                inputName.Text = string.Empty;
            }
            else
            {
                inputName.Text = editInfo.Name;
            }
        }
        private void btnSaveGroup_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(inputName.Text)) return;

            int selectIndex = cmbGroup.SelectedIndex;

            if (editInfo != null)
            {
                editInfo.Name = inputName.Text;
            }
            else
            {
                editInfo = new ProcessInfo { Name = inputName.Text, FileNames = new List<string>() };
                hijackConfig.Processs.Add(editInfo);
                selectIndex = hijackConfig.Processs.Count - 1;
            }
            BindGroup();

            if (selectIndex > 0)
            {
                cmbGroup.SelectedIndex = selectIndex;
            }
            hijackConfig.Save();
        }
        private void btnClearForm_Click(object sender, EventArgs e)
        {
            editInfo = null;
            BindEdit();
        }

        private void btnDelGroup_Click(object sender, EventArgs e)
        {
            if (processInfo == null) return;
            if (hijackConfig.Processs.Count <= 1) return;

            int selectIndex = cmbGroup.SelectedIndex;
            hijackConfig.Processs.Remove(processInfo);
            BindGroup();
            if (selectIndex > hijackConfig.Processs.Count - 1)
            {
                selectIndex = hijackConfig.Processs.Count - 1;
            }
            if (selectIndex > 0)
            {
                cmbGroup.SelectedIndex = selectIndex;
            }
            hijackConfig.Save();
        }

        private void btnDelProcess_Click(object sender, EventArgs e)
        {
            if (processInfo != null && listProcess.SelectedItem != null)
            {
                processInfo.FileNames.Remove(listProcess.SelectedItem.ToString());
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
    }
}
