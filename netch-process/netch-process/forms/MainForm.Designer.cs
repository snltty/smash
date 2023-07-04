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
            cbUseHijack = new CheckBox();
            listProcess = new ListBox();
            cmbGroup = new ComboBox();
            startBtn = new Button();
            mainMenu = new MenuStrip();
            mainMenuProxy = new ToolStripMenuItem();
            mainMenuProcessHijack = new ToolStripMenuItem();
            mainMenuHijackOptions = new ToolStripMenuItem();
            mainMenuProcess = new ToolStripMenuItem();
            mainMenuSystemProxy = new ToolStripMenuItem();
            mainManuAbout = new ToolStripMenuItem();
            groupBox2 = new GroupBox();
            labelProxy = new Label();
            cmbProxy = new ComboBox();
            groupBox3 = new GroupBox();
            lbPac = new Label();
            cbIsEnv = new CheckBox();
            cbIsPac = new CheckBox();
            cbUseSysProxy = new CheckBox();
            cmbSysProxy = new ComboBox();
            groupBox1.SuspendLayout();
            mainMenu.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(cbUseHijack);
            groupBox1.Controls.Add(listProcess);
            groupBox1.Controls.Add(cmbGroup);
            groupBox1.Location = new Point(24, 58);
            groupBox1.Margin = new Padding(6, 5, 6, 5);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(6, 5, 6, 5);
            groupBox1.Size = new Size(298, 289);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "进程劫持";
            // 
            // cbUseHijack
            // 
            cbUseHijack.AutoSize = true;
            cbUseHijack.Location = new Point(192, 43);
            cbUseHijack.Name = "cbUseHijack";
            cbUseHijack.Size = new Size(94, 35);
            cbUseHijack.TabIndex = 3;
            cbUseHijack.Text = "使用";
            cbUseHijack.UseVisualStyleBackColor = true;
            cbUseHijack.CheckedChanged += cbUseHijack_CheckedChanged;
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
            cmbGroup.SelectedIndexChanged += cmbGroup_SelectedIndexChanged;
            // 
            // startBtn
            // 
            startBtn.Location = new Point(240, 519);
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
            mainMenu.Items.AddRange(new ToolStripItem[] { mainMenuProxy, mainMenuProcessHijack, mainMenuSystemProxy, mainManuAbout });
            mainMenu.Location = new Point(0, 0);
            mainMenu.Name = "mainMenu";
            mainMenu.Padding = new Padding(12, 4, 0, 4);
            mainMenu.Size = new Size(652, 43);
            mainMenu.TabIndex = 3;
            mainMenu.Text = "主菜单";
            // 
            // mainMenuProxy
            // 
            mainMenuProxy.Name = "mainMenuProxy";
            mainMenuProxy.Size = new Size(130, 35);
            mainMenuProxy.Text = "代理设置";
            mainMenuProxy.Click += OnMainMenuProxyClick;
            // 
            // mainMenuProcessHijack
            // 
            mainMenuProcessHijack.DropDownItems.AddRange(new ToolStripItem[] { mainMenuHijackOptions, mainMenuProcess });
            mainMenuProcessHijack.Name = "mainMenuProcessHijack";
            mainMenuProcessHijack.Size = new Size(130, 35);
            mainMenuProcessHijack.Text = "进程劫持";
            // 
            // mainMenuHijackOptions
            // 
            mainMenuHijackOptions.Name = "mainMenuHijackOptions";
            mainMenuHijackOptions.Size = new Size(243, 44);
            mainMenuHijackOptions.Text = "选项设置";
            mainMenuHijackOptions.Click += OnMainMenuHijackOptionsClick;
            // 
            // mainMenuProcess
            // 
            mainMenuProcess.Name = "mainMenuProcess";
            mainMenuProcess.Size = new Size(243, 44);
            mainMenuProcess.Text = "进程设置";
            mainMenuProcess.Click += OnMainMenuHijackProcessClick;
            // 
            // mainMenuSystemProxy
            // 
            mainMenuSystemProxy.Name = "mainMenuSystemProxy";
            mainMenuSystemProxy.Size = new Size(130, 35);
            mainMenuSystemProxy.Text = "系统代理";
            mainMenuSystemProxy.Click += OnMainMenuSysProxyClick;
            // 
            // mainManuAbout
            // 
            mainManuAbout.Name = "mainManuAbout";
            mainManuAbout.Size = new Size(82, 35);
            mainManuAbout.Text = "关于";
            mainManuAbout.Click += OnMainMenuAboutClick;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(labelProxy);
            groupBox2.Controls.Add(cmbProxy);
            groupBox2.Location = new Point(182, 359);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(298, 146);
            groupBox2.TabIndex = 4;
            groupBox2.TabStop = false;
            groupBox2.Text = "选择代理";
            // 
            // labelProxy
            // 
            labelProxy.Location = new Point(6, 90);
            labelProxy.Name = "labelProxy";
            labelProxy.Size = new Size(286, 47);
            labelProxy.TabIndex = 1;
            labelProxy.Text = "label1";
            labelProxy.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // cmbProxy
            // 
            cmbProxy.FormattingEnabled = true;
            cmbProxy.Location = new Point(34, 44);
            cmbProxy.Name = "cmbProxy";
            cmbProxy.Size = new Size(234, 39);
            cmbProxy.TabIndex = 0;
            cmbProxy.SelectedIndexChanged += cmbProxy_SelectedIndexChanged;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(lbPac);
            groupBox3.Controls.Add(cbIsEnv);
            groupBox3.Controls.Add(cbIsPac);
            groupBox3.Controls.Add(cbUseSysProxy);
            groupBox3.Controls.Add(cmbSysProxy);
            groupBox3.Location = new Point(331, 58);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(298, 289);
            groupBox3.TabIndex = 5;
            groupBox3.TabStop = false;
            groupBox3.Text = "系统代理";
            // 
            // lbPac
            // 
            lbPac.Location = new Point(6, 172);
            lbPac.Name = "lbPac";
            lbPac.Size = new Size(282, 105);
            lbPac.TabIndex = 8;
            lbPac.Text = "label1";
            lbPac.TextAlign = ContentAlignment.TopCenter;
            // 
            // cbIsEnv
            // 
            cbIsEnv.AutoSize = true;
            cbIsEnv.Location = new Point(12, 94);
            cbIsEnv.Name = "cbIsEnv";
            cbIsEnv.Size = new Size(190, 35);
            cbIsEnv.TabIndex = 7;
            cbIsEnv.Text = "设置环境变量";
            cbIsEnv.UseVisualStyleBackColor = true;
            cbIsEnv.CheckedChanged += cbIsEnv_CheckedChanged;
            // 
            // cbIsPac
            // 
            cbIsPac.AutoSize = true;
            cbIsPac.Location = new Point(12, 134);
            cbIsPac.Name = "cbIsPac";
            cbIsPac.Size = new Size(142, 35);
            cbIsPac.TabIndex = 6;
            cbIsPac.Text = "设置PAC";
            cbIsPac.UseVisualStyleBackColor = true;
            cbIsPac.CheckedChanged += cbIsPac_CheckedChanged;
            // 
            // cbUseSysProxy
            // 
            cbUseSysProxy.AutoSize = true;
            cbUseSysProxy.Location = new Point(194, 45);
            cbUseSysProxy.Name = "cbUseSysProxy";
            cbUseSysProxy.Size = new Size(94, 35);
            cbUseSysProxy.TabIndex = 5;
            cbUseSysProxy.Text = "使用";
            cbUseSysProxy.UseVisualStyleBackColor = true;
            cbUseSysProxy.CheckedChanged += cbUseSysProxy_CheckedChanged;
            // 
            // cmbSysProxy
            // 
            cmbSysProxy.FormattingEnabled = true;
            cmbSysProxy.Location = new Point(12, 43);
            cmbSysProxy.Name = "cmbSysProxy";
            cmbSysProxy.Size = new Size(176, 39);
            cmbSysProxy.TabIndex = 4;
            cmbSysProxy.SelectedIndexChanged += cmbSysProxy_SelectedIndexChanged;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(14F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(652, 614);
            Controls.Add(groupBox3);
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
            groupBox1.PerformLayout();
            mainMenu.ResumeLayout(false);
            mainMenu.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private GroupBox groupBox1;
        private Button startBtn;
        private MenuStrip mainMenu;
        private ToolStripMenuItem mainMenuProxy;
        private ToolStripMenuItem mainManuAbout;
        private ComboBox cmbGroup;
        private GroupBox groupBox2;
        private ComboBox cmbProxy;
        private Label labelProxy;
        private ListBox listProcess;
        private ToolStripMenuItem mainMenuProcessHijack;
        private ToolStripMenuItem mainMenuHijackOptions;
        private ToolStripMenuItem mainMenuProcess;
        private ToolStripMenuItem mainMenuSystemProxy;
        private CheckBox cbUseHijack;
        private GroupBox groupBox3;
        private CheckBox cbUseSysProxy;
        private ComboBox cmbSysProxy;
        private CheckBox cbIsPac;
        private CheckBox cbIsEnv;
        private Label lbPac;
    }
}