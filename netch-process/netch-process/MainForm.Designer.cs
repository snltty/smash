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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.files = new System.Windows.Forms.ListView();
            this.startBtn = new System.Windows.Forms.Button();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.代理设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.选项设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.关于ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            this.mainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.files);
            this.groupBox1.Location = new System.Drawing.Point(12, 184);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(333, 205);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "选择进程";
            // 
            // files
            // 
            this.files.Location = new System.Drawing.Point(4, 22);
            this.files.Name = "files";
            this.files.Size = new System.Drawing.Size(185, 205);
            this.files.TabIndex = 0;
            this.files.UseCompatibleStateImageBehavior = false;
            // 
            // startBtn
            // 
            this.startBtn.Location = new System.Drawing.Point(142, 395);
            this.startBtn.Name = "startBtn";
            this.startBtn.Size = new System.Drawing.Size(94, 43);
            this.startBtn.TabIndex = 2;
            this.startBtn.Text = "启动驱动";
            this.startBtn.UseVisualStyleBackColor = true;
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.代理设置ToolStripMenuItem,
            this.选项设置ToolStripMenuItem,
            this.关于ToolStripMenuItem});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(354, 25);
            this.mainMenu.TabIndex = 3;
            this.mainMenu.Text = "主菜单";
            // 
            // 代理设置ToolStripMenuItem
            // 
            this.代理设置ToolStripMenuItem.Name = "代理设置ToolStripMenuItem";
            this.代理设置ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.代理设置ToolStripMenuItem.Text = "代理设置";
            this.代理设置ToolStripMenuItem.Click += new System.EventHandler(this.OnProxySettingClick);
            // 
            // 选项设置ToolStripMenuItem
            // 
            this.选项设置ToolStripMenuItem.Name = "选项设置ToolStripMenuItem";
            this.选项设置ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.选项设置ToolStripMenuItem.Text = "选项设置";
            this.选项设置ToolStripMenuItem.Click += new System.EventHandler(this.OnOptionsSettingClick);
            // 
            // 关于ToolStripMenuItem
            // 
            this.关于ToolStripMenuItem.Name = "关于ToolStripMenuItem";
            this.关于ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.关于ToolStripMenuItem.Text = "关于";
            this.关于ToolStripMenuItem.Click += new System.EventHandler(this.OnAboutClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(354, 450);
            this.Controls.Add(this.startBtn);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.mainMenu);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mainMenu;
            this.Name = "MainForm";
            this.Text = "netch-process";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Closing);
            this.groupBox1.ResumeLayout(false);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private GroupBox groupBox1;
        private Button startBtn;
        private ListView files;
        private MenuStrip mainMenu;
        private ToolStripMenuItem 代理设置ToolStripMenuItem;
        private ToolStripMenuItem 选项设置ToolStripMenuItem;
        private ToolStripMenuItem 关于ToolStripMenuItem;
    }
}