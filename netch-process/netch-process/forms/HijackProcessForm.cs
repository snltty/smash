using netch_process.libs;

namespace netch_process
{
    public partial class HijackProcessForm : Form
    {
        Config config;
        public HijackProcessForm(Config config)
        {
            this.config = config;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            InitializeComponent();

            cmbGroup.SelectedIndexChanged += CmbGroup_SelectedIndexChanged;
            BindGroup();
        }

        ProcessInfo processInfo;
        private void CmbGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbGroup.SelectedItem != null)
            {
                processInfo = config.Processs.FirstOrDefault(c => c.Name == cmbGroup.SelectedItem.ToString());
                editInfo = processInfo;
            }
            BindFileNames();
            BindEdit();
        }
        private void BindGroup()
        {
            cmbGroup.DataSource = null;
            cmbGroup.DataSource = config.Processs.Select(c => c.Name).ToList();
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
                config.Processs.Add(editInfo);
                selectIndex = config.Processs.Count - 1;
            }
            BindGroup();

            if (selectIndex > 0)
            {
                cmbGroup.SelectedIndex = selectIndex;
            }
            config.Save();
        }
        private void btnClearForm_Click(object sender, EventArgs e)
        {
            editInfo = null;
            BindEdit();
        }

        private void btnDelGroup_Click(object sender, EventArgs e)
        {
            if (processInfo == null) return;
            if (config.Processs.Count == 1) return;

            int selectIndex = cmbGroup.SelectedIndex;
            config.Processs.Remove(processInfo);
            BindGroup();
            if (selectIndex > config.Processs.Count - 1)
            {
                selectIndex = config.Processs.Count - 1;
            }
            if (selectIndex > 0)
            {
                cmbGroup.SelectedIndex = selectIndex;
            }
            config.Save();
        }

        private void btnDelProcess_Click(object sender, EventArgs e)
        {
            if (processInfo != null && listProcess.SelectedItem != null)
            {
                processInfo.FileNames.Remove(listProcess.SelectedItem.ToString());
            }
            BindFileNames();
            config.Save();
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
                    config.Save();
                }
            }
        }
    }
}
