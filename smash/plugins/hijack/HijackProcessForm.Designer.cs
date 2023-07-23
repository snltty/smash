namespace smash.plugins
{
    partial class HijackProcessForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HijackProcessForm));
            groupBox1 = new GroupBox();
            btnSaveGroup = new Button();
            listProcess = new ListBox();
            cmbGroup = new ComboBox();
            contextMenu = new ContextMenuStrip(components);
            contextMenuAddProcess = new ToolStripMenuItem();
            contextMenuDelProcess = new ToolStripMenuItem();
            mainMenuDelGroup = new ToolStripMenuItem();
            mainMenuAddGroup = new ToolStripMenuItem();
            groupBox1.SuspendLayout();
            contextMenu.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btnSaveGroup);
            groupBox1.Controls.Add(listProcess);
            groupBox1.Controls.Add(cmbGroup);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(546, 612);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "进程组";
            // 
            // btnSaveGroup
            // 
            btnSaveGroup.Location = new Point(382, 37);
            btnSaveGroup.Name = "btnSaveGroup";
            btnSaveGroup.Size = new Size(150, 46);
            btnSaveGroup.TabIndex = 2;
            btnSaveGroup.Text = "保存修改";
            btnSaveGroup.UseVisualStyleBackColor = true;
            btnSaveGroup.Click += OnBtnSaveGroupClick;
            // 
            // listProcess
            // 
            listProcess.FormattingEnabled = true;
            listProcess.HorizontalScrollbar = true;
            listProcess.ItemHeight = 31;
            listProcess.Location = new Point(12, 91);
            listProcess.Name = "listProcess";
            listProcess.Size = new Size(520, 500);
            listProcess.TabIndex = 1;
            // 
            // cmbGroup
            // 
            cmbGroup.FormattingEnabled = true;
            cmbGroup.Location = new Point(12, 42);
            cmbGroup.Name = "cmbGroup";
            cmbGroup.Size = new Size(353, 39);
            cmbGroup.TabIndex = 0;
            // 
            // contextMenu
            // 
            contextMenu.ImageScalingSize = new Size(32, 32);
            contextMenu.Items.AddRange(new ToolStripItem[] { contextMenuAddProcess, contextMenuDelProcess, mainMenuAddGroup, mainMenuDelGroup });
            contextMenu.Name = "contextMenu";
            contextMenu.Size = new Size(301, 200);
            // 
            // contextMenuAddProcess
            // 
            contextMenuAddProcess.Name = "contextMenuAddProcess";
            contextMenuAddProcess.Size = new Size(300, 38);
            contextMenuAddProcess.Text = "添加进程";
            contextMenuAddProcess.Click += btnAddProcess_Click;
            // 
            // contextMenuDelProcess
            // 
            contextMenuDelProcess.Name = "contextMenuDelProcess";
            contextMenuDelProcess.Size = new Size(300, 38);
            contextMenuDelProcess.Text = "删除进程";
            contextMenuDelProcess.Click += btnDelProcess_Click;
            // 
            // mainMenuDelGroup
            // 
            mainMenuDelGroup.Name = "mainMenuDelGroup";
            mainMenuDelGroup.Size = new Size(300, 38);
            mainMenuDelGroup.Text = "删除分组";
            mainMenuDelGroup.Click += OnMainMenuDelGroup;
            // 
            // mainMenuAddGroup
            // 
            mainMenuAddGroup.Name = "mainMenuAddGroup";
            mainMenuAddGroup.Size = new Size(300, 38);
            mainMenuAddGroup.Text = "添加分组";
            mainMenuAddGroup.Click += OnMainMenuAddGroupClick;
            // 
            // HijackProcessForm
            // 
            AutoScaleDimensions = new SizeF(14F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(570, 636);
            Controls.Add(groupBox1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "HijackProcessForm";
            Text = "进程劫持进程设置";
            groupBox1.ResumeLayout(false);
            contextMenu.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private ListBox listProcess;
        private ComboBox cmbGroup;
        private ContextMenuStrip contextMenu;
        private ToolStripMenuItem contextMenuAddProcess;
        private ToolStripMenuItem contextMenuDelProcess;
        private Button btnSaveGroup;
        private ToolStripMenuItem mainMenuDelGroup;
        private ToolStripMenuItem mainMenuAddGroup;
    }
}