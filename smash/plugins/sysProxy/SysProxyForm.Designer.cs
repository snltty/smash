namespace smash.plugins.sysProxy
{
    partial class SysProxyForm
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
            lbPac = new Label();
            cbIsEnv = new CheckBox();
            cbIsPac = new CheckBox();
            cbUseSysProxy = new CheckBox();
            cmbSysProxy = new ComboBox();
            groupBox1 = new GroupBox();
            groupBox2 = new GroupBox();
            button2 = new Button();
            button1 = new Button();
            textBox1 = new TextBox();
            mainMenu = new MenuStrip();
            mainMenuOptions = new ToolStripMenuItem();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            mainMenu.SuspendLayout();
            SuspendLayout();
            // 
            // lbPac
            // 
            lbPac.Location = new Point(18, 154);
            lbPac.Name = "lbPac";
            lbPac.Size = new Size(262, 31);
            lbPac.TabIndex = 8;
            lbPac.Text = "pac文件";
            lbPac.TextAlign = ContentAlignment.TopCenter;
            // 
            // cbIsEnv
            // 
            cbIsEnv.AutoSize = true;
            cbIsEnv.Location = new Point(18, 101);
            cbIsEnv.Name = "cbIsEnv";
            cbIsEnv.Size = new Size(142, 35);
            cbIsEnv.TabIndex = 7;
            cbIsEnv.Text = "环境变量";
            cbIsEnv.UseVisualStyleBackColor = true;
            cbIsEnv.CheckedChanged += CheckedChanged;
            // 
            // cbIsPac
            // 
            cbIsPac.AutoSize = true;
            cbIsPac.Location = new Point(186, 101);
            cbIsPac.Name = "cbIsPac";
            cbIsPac.Size = new Size(94, 35);
            cbIsPac.TabIndex = 6;
            cbIsPac.Text = "PAC";
            cbIsPac.UseVisualStyleBackColor = true;
            cbIsPac.CheckedChanged += CheckedChanged;
            // 
            // cbUseSysProxy
            // 
            cbUseSysProxy.AutoSize = true;
            cbUseSysProxy.Location = new Point(186, 55);
            cbUseSysProxy.Name = "cbUseSysProxy";
            cbUseSysProxy.Size = new Size(94, 35);
            cbUseSysProxy.TabIndex = 5;
            cbUseSysProxy.Text = "启用";
            cbUseSysProxy.UseVisualStyleBackColor = true;
            cbUseSysProxy.CheckedChanged += CheckedChanged;
            // 
            // cmbSysProxy
            // 
            cmbSysProxy.FormattingEnabled = true;
            cmbSysProxy.Location = new Point(18, 53);
            cmbSysProxy.Name = "cmbSysProxy";
            cmbSysProxy.Size = new Size(152, 39);
            cmbSysProxy.TabIndex = 4;
            cmbSysProxy.SelectedIndexChanged += cmbSysProxy_SelectedIndexChanged;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(cmbSysProxy);
            groupBox1.Controls.Add(lbPac);
            groupBox1.Controls.Add(cbUseSysProxy);
            groupBox1.Controls.Add(cbIsEnv);
            groupBox1.Controls.Add(cbIsPac);
            groupBox1.Location = new Point(12, 61);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(293, 227);
            groupBox1.TabIndex = 9;
            groupBox1.TabStop = false;
            groupBox1.Text = "系统代理配置";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(button2);
            groupBox2.Controls.Add(button1);
            groupBox2.Controls.Add(textBox1);
            groupBox2.Location = new Point(311, 61);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(391, 227);
            groupBox2.TabIndex = 10;
            groupBox2.TabStop = false;
            groupBox2.Text = "pac文件服务";
            // 
            // button2
            // 
            button2.Location = new Point(205, 117);
            button2.Name = "button2";
            button2.Size = new Size(150, 46);
            button2.TabIndex = 2;
            button2.Text = "打开目录";
            button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Location = new Point(40, 117);
            button1.Name = "button1";
            button1.Size = new Size(150, 46);
            button1.TabIndex = 1;
            button1.Text = "选择根目录";
            button1.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(18, 55);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(355, 38);
            textBox1.TabIndex = 0;
            // 
            // mainMenu
            // 
            mainMenu.ImageScalingSize = new Size(32, 32);
            mainMenu.Items.AddRange(new ToolStripItem[] { mainMenuOptions });
            mainMenu.Location = new Point(0, 0);
            mainMenu.Name = "mainMenu";
            mainMenu.Size = new Size(714, 42);
            mainMenu.TabIndex = 11;
            mainMenu.Text = "menuStrip1";
            // 
            // mainMenuOptions
            // 
            mainMenuOptions.Name = "mainMenuOptions";
            mainMenuOptions.Size = new Size(178, 38);
            mainMenuOptions.Text = "系统代理选项";
            mainMenuOptions.Click += OnMainMenuOptionsClick;
            // 
            // SysProxyForm
            // 
            AutoScaleDimensions = new SizeF(14F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(714, 305);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(mainMenu);
            MainMenuStrip = mainMenu;
            Name = "SysProxyForm";
            Text = "系统代理";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            mainMenu.ResumeLayout(false);
            mainMenu.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label lbPac;
        private CheckBox cbIsEnv;
        private CheckBox cbIsPac;
        private CheckBox cbUseSysProxy;
        private ComboBox cmbSysProxy;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Button button1;
        private TextBox textBox1;
        private Button button2;
        private MenuStrip mainMenu;
        private ToolStripMenuItem mainMenuOptions;
    }
}