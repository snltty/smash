namespace netch_process
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            groupBox1 = new GroupBox();
            comboBox1 = new ComboBox();
            files = new ListView();
            startBtn = new Button();
            mainMenu = new MenuStrip();
            代理设置ToolStripMenuItem = new ToolStripMenuItem();
            选项设置ToolStripMenuItem = new ToolStripMenuItem();
            关于ToolStripMenuItem = new ToolStripMenuItem();
            groupBox1.SuspendLayout();
            mainMenu.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(comboBox1);
            groupBox1.Controls.Add(files);
            groupBox1.Location = new Point(24, 48);
            groupBox1.Margin = new Padding(6, 5, 6, 5);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(6, 5, 6, 5);
            groupBox1.Size = new Size(385, 468);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "选择进程";
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(9, 50);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(364, 39);
            comboBox1.TabIndex = 1;
            // 
            // files
            // 
            files.Location = new Point(9, 97);
            files.Margin = new Padding(6, 5, 6, 5);
            files.Name = "files";
            files.Size = new Size(364, 360);
            files.TabIndex = 0;
            files.UseCompatibleStateImageBehavior = false;
            // 
            // startBtn
            // 
            startBtn.Location = new Point(113, 526);
            startBtn.Margin = new Padding(6, 5, 6, 5);
            startBtn.Name = "startBtn";
            startBtn.Size = new Size(188, 78);
            startBtn.TabIndex = 2;
            startBtn.Text = "启动驱动";
            startBtn.UseVisualStyleBackColor = true;
            // 
            // mainMenu
            // 
            mainMenu.ImageScalingSize = new Size(32, 32);
            mainMenu.Items.AddRange(new ToolStripItem[] { 代理设置ToolStripMenuItem, 选项设置ToolStripMenuItem, 关于ToolStripMenuItem });
            mainMenu.Location = new Point(0, 0);
            mainMenu.Name = "mainMenu";
            mainMenu.Padding = new Padding(12, 4, 0, 4);
            mainMenu.Size = new Size(430, 43);
            mainMenu.TabIndex = 3;
            mainMenu.Text = "主菜单";
            // 
            // 代理设置ToolStripMenuItem
            // 
            代理设置ToolStripMenuItem.Name = "代理设置ToolStripMenuItem";
            代理设置ToolStripMenuItem.Size = new Size(130, 35);
            代理设置ToolStripMenuItem.Text = "代理设置";
            代理设置ToolStripMenuItem.Click += OnProxySettingClick;
            // 
            // 选项设置ToolStripMenuItem
            // 
            选项设置ToolStripMenuItem.Name = "选项设置ToolStripMenuItem";
            选项设置ToolStripMenuItem.Size = new Size(130, 35);
            选项设置ToolStripMenuItem.Text = "选项设置";
            选项设置ToolStripMenuItem.Click += OnOptionsSettingClick;
            // 
            // 关于ToolStripMenuItem
            // 
            关于ToolStripMenuItem.Name = "关于ToolStripMenuItem";
            关于ToolStripMenuItem.Size = new Size(82, 35);
            关于ToolStripMenuItem.Text = "关于";
            关于ToolStripMenuItem.Click += OnAboutClick;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(14F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(430, 628);
            Controls.Add(startBtn);
            Controls.Add(groupBox1);
            Controls.Add(mainMenu);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = mainMenu;
            Margin = new Padding(6, 5, 6, 5);
            Name = "MainForm";
            Text = "netch-process";
            FormClosing += Closing;
            groupBox1.ResumeLayout(false);
            mainMenu.ResumeLayout(false);
            mainMenu.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private GroupBox groupBox1;
        private Button startBtn;
        private ListView files;
        private MenuStrip mainMenu;
        private ToolStripMenuItem 代理设置ToolStripMenuItem;
        private ToolStripMenuItem 选项设置ToolStripMenuItem;
        private ToolStripMenuItem 关于ToolStripMenuItem;
        private ComboBox comboBox1;
    }
}