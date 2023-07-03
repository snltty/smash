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
            listProcess = new ListBox();
            cmbGroup = new ComboBox();
            startBtn = new Button();
            mainMenu = new MenuStrip();
            mainMenuProxy = new ToolStripMenuItem();
            mainMenuOptions = new ToolStripMenuItem();
            mainMenuProcess = new ToolStripMenuItem();
            mainManuAbout = new ToolStripMenuItem();
            groupBox2 = new GroupBox();
            labelProxy = new Label();
            cmbProxy = new ComboBox();
            groupBox1.SuspendLayout();
            mainMenu.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(listProcess);
            groupBox1.Controls.Add(cmbGroup);
            groupBox1.Location = new Point(24, 48);
            groupBox1.Margin = new Padding(6, 5, 6, 5);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(6, 5, 6, 5);
            groupBox1.Size = new Size(467, 289);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "选择进程";
            // 
            // listProcess
            // 
            listProcess.FormattingEnabled = true;
            listProcess.ItemHeight = 31;
            listProcess.Location = new Point(10, 87);
            listProcess.Name = "listProcess";
            listProcess.Size = new Size(446, 190);
            listProcess.TabIndex = 2;
            // 
            // cmbGroup
            // 
            cmbGroup.FormattingEnabled = true;
            cmbGroup.Location = new Point(106, 42);
            cmbGroup.Name = "cmbGroup";
            cmbGroup.Size = new Size(242, 39);
            cmbGroup.TabIndex = 1;
            cmbGroup.SelectedIndexChanged += cmbGroup_SelectedIndexChanged;
            // 
            // startBtn
            // 
            startBtn.Location = new Point(159, 499);
            startBtn.Margin = new Padding(6, 5, 6, 5);
            startBtn.Name = "startBtn";
            startBtn.Size = new Size(188, 78);
            startBtn.TabIndex = 2;
            startBtn.Text = "启动驱动";
            startBtn.UseVisualStyleBackColor = true;
            startBtn.Click += startBtn_Click;
            // 
            // mainMenu
            // 
            mainMenu.ImageScalingSize = new Size(32, 32);
            mainMenu.Items.AddRange(new ToolStripItem[] { mainMenuProxy, mainMenuOptions, mainMenuProcess, mainManuAbout });
            mainMenu.Location = new Point(0, 0);
            mainMenu.Name = "mainMenu";
            mainMenu.Padding = new Padding(12, 4, 0, 4);
            mainMenu.Size = new Size(506, 43);
            mainMenu.TabIndex = 3;
            mainMenu.Text = "主菜单";
            // 
            // mainMenuProxy
            // 
            mainMenuProxy.Name = "mainMenuProxy";
            mainMenuProxy.Size = new Size(130, 35);
            mainMenuProxy.Text = "代理设置";
            mainMenuProxy.Click += OnProxySettingClick;
            // 
            // mainMenuOptions
            // 
            mainMenuOptions.Name = "mainMenuOptions";
            mainMenuOptions.Size = new Size(130, 35);
            mainMenuOptions.Text = "选项设置";
            mainMenuOptions.Click += OnOptionsSettingClick;
            // 
            // mainMenuProcess
            // 
            mainMenuProcess.Name = "mainMenuProcess";
            mainMenuProcess.Size = new Size(130, 35);
            mainMenuProcess.Text = "进程设置";
            mainMenuProcess.Click += OnProcessClick;
            // 
            // mainManuAbout
            // 
            mainManuAbout.Name = "mainManuAbout";
            mainManuAbout.Size = new Size(82, 35);
            mainManuAbout.Text = "关于";
            mainManuAbout.Click += OnAboutClick;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(labelProxy);
            groupBox2.Controls.Add(cmbProxy);
            groupBox2.Location = new Point(24, 345);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(467, 146);
            groupBox2.TabIndex = 4;
            groupBox2.TabStop = false;
            groupBox2.Text = "选择代理";
            // 
            // labelProxy
            // 
            labelProxy.Location = new Point(10, 87);
            labelProxy.Name = "labelProxy";
            labelProxy.Size = new Size(446, 47);
            labelProxy.TabIndex = 1;
            labelProxy.Text = "label1";
            labelProxy.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // cmbProxy
            // 
            cmbProxy.FormattingEnabled = true;
            cmbProxy.Location = new Point(106, 40);
            cmbProxy.Name = "cmbProxy";
            cmbProxy.Size = new Size(242, 39);
            cmbProxy.TabIndex = 0;
            cmbProxy.SelectedIndexChanged += cmbProxy_SelectedIndexChanged;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(14F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(506, 591);
            Controls.Add(groupBox2);
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
            Load += OnWinLoad;
            groupBox1.ResumeLayout(false);
            mainMenu.ResumeLayout(false);
            mainMenu.PerformLayout();
            groupBox2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private GroupBox groupBox1;
        private Button startBtn;
        private MenuStrip mainMenu;
        private ToolStripMenuItem mainMenuProxy;
        private ToolStripMenuItem mainMenuOptions;
        private ToolStripMenuItem mainManuAbout;
        private ComboBox cmbGroup;
        private ToolStripMenuItem mainMenuProcess;
        private GroupBox groupBox2;
        private ComboBox cmbProxy;
        private Label labelProxy;
        private ListBox listProcess;
    }
}