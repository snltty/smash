namespace smash.plugins.hijack
{
    partial class HijackForm
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
            groupBox1 = new GroupBox();
            cbUseHijack = new CheckBox();
            listProcess = new ListBox();
            cmbGroup = new ComboBox();
            menuStrip1 = new MenuStrip();
            配置ToolStripMenuItem = new ToolStripMenuItem();
            groupBox1.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(cbUseHijack);
            groupBox1.Controls.Add(listProcess);
            groupBox1.Controls.Add(cmbGroup);
            groupBox1.Location = new Point(235, 63);
            groupBox1.Margin = new Padding(6, 5, 6, 5);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(6, 5, 6, 5);
            groupBox1.Size = new Size(298, 289);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "进程劫持";
            // 
            // cbUseHijack
            // 
            cbUseHijack.AutoSize = true;
            cbUseHijack.Location = new Point(195, 45);
            cbUseHijack.Name = "cbUseHijack";
            cbUseHijack.Size = new Size(94, 35);
            cbUseHijack.TabIndex = 3;
            cbUseHijack.Text = "使用";
            cbUseHijack.UseVisualStyleBackColor = true;
            // 
            // listProcess
            // 
            listProcess.FormattingEnabled = true;
            listProcess.ItemHeight = 31;
            listProcess.Location = new Point(10, 87);
            listProcess.Name = "listProcess";
            listProcess.Size = new Size(276, 190);
            listProcess.TabIndex = 2;
            // 
            // cmbGroup
            // 
            cmbGroup.FormattingEnabled = true;
            cmbGroup.Location = new Point(10, 42);
            cmbGroup.Name = "cmbGroup";
            cmbGroup.Size = new Size(176, 39);
            cmbGroup.TabIndex = 1;
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(32, 32);
            menuStrip1.Items.AddRange(new ToolStripItem[] { 配置ToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 42);
            menuStrip1.TabIndex = 3;
            menuStrip1.Text = "menuStrip1";
            // 
            // 配置ToolStripMenuItem
            // 
            配置ToolStripMenuItem.Name = "配置ToolStripMenuItem";
            配置ToolStripMenuItem.Size = new Size(82, 38);
            配置ToolStripMenuItem.Text = "配置";
            // 
            // HijackForm
            // 
            AutoScaleDimensions = new SizeF(14F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(groupBox1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "HijackForm";
            Text = "进程劫持";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox groupBox1;
        private CheckBox cbUseHijack;
        private ListBox listProcess;
        private ComboBox cmbGroup;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem 配置ToolStripMenuItem;
    }
}