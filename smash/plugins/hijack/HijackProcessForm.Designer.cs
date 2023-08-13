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
            contextMenu = new ContextMenuStrip(components);
            mainMenuAddGroup = new ToolStripMenuItem();
            mainMenuDelGroup = new ToolStripMenuItem();
            processView = new DataGridView();
            processList = new ListBox();
            contextMenuList = new ContextMenuStrip(components);
            toolStripMenuItem1 = new ToolStripMenuItem();
            toolStripMenuItem2 = new ToolStripMenuItem();
            contextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)processView).BeginInit();
            contextMenuList.SuspendLayout();
            SuspendLayout();
            // 
            // contextMenu
            // 
            contextMenu.ImageScalingSize = new Size(32, 32);
            contextMenu.Items.AddRange(new ToolStripItem[] { mainMenuAddGroup, mainMenuDelGroup });
            contextMenu.Name = "contextMenu";
            contextMenu.Size = new Size(137, 80);
            // 
            // mainMenuAddGroup
            // 
            mainMenuAddGroup.Name = "mainMenuAddGroup";
            mainMenuAddGroup.Size = new Size(136, 38);
            mainMenuAddGroup.Text = "添加";
            mainMenuAddGroup.Click += OnMainMenuAddGroupClick;
            // 
            // mainMenuDelGroup
            // 
            mainMenuDelGroup.Name = "mainMenuDelGroup";
            mainMenuDelGroup.Size = new Size(136, 38);
            mainMenuDelGroup.Text = "删除";
            mainMenuDelGroup.Click += OnMainMenuDelGroup;
            // 
            // processView
            // 
            processView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            processView.Dock = DockStyle.Left;
            processView.Location = new Point(0, 0);
            processView.Name = "processView";
            processView.RowHeadersWidth = 82;
            processView.RowTemplate.Height = 40;
            processView.Size = new Size(580, 391);
            processView.TabIndex = 1;
            // 
            // processList
            // 
            processList.Dock = DockStyle.Right;
            processList.FormattingEnabled = true;
            processList.ItemHeight = 31;
            processList.Location = new Point(604, 0);
            processList.Name = "processList";
            processList.Size = new Size(240, 391);
            processList.TabIndex = 2;
            // 
            // contextMenuList
            // 
            contextMenuList.ImageScalingSize = new Size(32, 32);
            contextMenuList.Items.AddRange(new ToolStripItem[] { toolStripMenuItem1, toolStripMenuItem2 });
            contextMenuList.Name = "contextMenu";
            contextMenuList.Size = new Size(301, 124);
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(300, 38);
            toolStripMenuItem1.Text = "添加";
            toolStripMenuItem1.Click += btnAddProcess_Click;
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new Size(300, 38);
            toolStripMenuItem2.Text = "删除";
            toolStripMenuItem2.Click += btnDelProcess_Click;
            // 
            // HijackProcessForm
            // 
            AutoScaleDimensions = new SizeF(14F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(844, 391);
            Controls.Add(processList);
            Controls.Add(processView);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "HijackProcessForm";
            Text = "进程劫持";
            contextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)processView).EndInit();
            contextMenuList.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private ContextMenuStrip contextMenu;
        private ToolStripMenuItem mainMenuDelGroup;
        private ToolStripMenuItem mainMenuAddGroup;
        private DataGridView processView;
        private ListBox processList;
        private ContextMenuStrip contextMenuList;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItem2;
    }
}