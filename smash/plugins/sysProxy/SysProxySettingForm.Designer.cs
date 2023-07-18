namespace smash.plugins
{
    partial class SysProxySettingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SysProxySettingForm));
            sysProxysView = new DataGridView();
            groupBox1 = new GroupBox();
            label2 = new Label();
            btnClear = new Button();
            btnSave = new Button();
            cbEnv = new CheckBox();
            inputName = new TextBox();
            label1 = new Label();
            inputPac = new TextBox();
            cbPac = new CheckBox();
            contextMenu = new ContextMenuStrip(components);
            mainMenuDelProxy = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)sysProxysView).BeginInit();
            groupBox1.SuspendLayout();
            contextMenu.SuspendLayout();
            SuspendLayout();
            // 
            // sysProxysView
            // 
            sysProxysView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            sysProxysView.Location = new Point(12, 12);
            sysProxysView.Name = "sysProxysView";
            sysProxysView.RowHeadersWidth = 82;
            sysProxysView.RowTemplate.Height = 40;
            sysProxysView.Size = new Size(776, 277);
            sysProxysView.TabIndex = 0;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(btnClear);
            groupBox1.Controls.Add(btnSave);
            groupBox1.Controls.Add(cbEnv);
            groupBox1.Controls.Add(inputName);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(inputPac);
            groupBox1.Controls.Add(cbPac);
            groupBox1.Location = new Point(12, 295);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(776, 218);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "设置";
            // 
            // label2
            // 
            label2.ForeColor = SystemColors.ControlDarkDark;
            label2.Location = new Point(14, 171);
            label2.Name = "label2";
            label2.Size = new Size(748, 31);
            label2.TabIndex = 7;
            label2.Text = "某些程序会去读取系统环境变量的代理设置";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnClear
            // 
            btnClear.Location = new Point(223, 107);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(150, 46);
            btnClear.TabIndex = 6;
            btnClear.Text = "清空表单";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += btnClear_Click;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(388, 107);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(150, 46);
            btnSave.TabIndex = 5;
            btnSave.Text = "保存更改";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // cbEnv
            // 
            cbEnv.AutoSize = true;
            cbEnv.Location = new Point(286, 47);
            cbEnv.Name = "cbEnv";
            cbEnv.Size = new Size(142, 35);
            cbEnv.TabIndex = 4;
            cbEnv.Text = "环境变量";
            cbEnv.UseVisualStyleBackColor = true;
            // 
            // inputName
            // 
            inputName.Location = new Point(74, 44);
            inputName.Name = "inputName";
            inputName.Size = new Size(181, 38);
            inputName.TabIndex = 3;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 47);
            label1.Name = "label1";
            label1.Size = new Size(62, 31);
            label1.TabIndex = 2;
            label1.Text = "名称";
            // 
            // inputPac
            // 
            inputPac.Location = new Point(557, 44);
            inputPac.Name = "inputPac";
            inputPac.Size = new Size(205, 38);
            inputPac.TabIndex = 1;
            // 
            // cbPac
            // 
            cbPac.AutoSize = true;
            cbPac.Location = new Point(457, 47);
            cbPac.Name = "cbPac";
            cbPac.Size = new Size(94, 35);
            cbPac.TabIndex = 0;
            cbPac.Text = "PAC";
            cbPac.UseVisualStyleBackColor = true;
            // 
            // contextMenu
            // 
            contextMenu.ImageScalingSize = new Size(32, 32);
            contextMenu.Items.AddRange(new ToolStripItem[] { mainMenuDelProxy });
            contextMenu.Name = "contextMenu";
            contextMenu.Size = new Size(137, 42);
            // 
            // mainMenuDelProxy
            // 
            mainMenuDelProxy.Name = "mainMenuDelProxy";
            mainMenuDelProxy.Size = new Size(136, 38);
            mainMenuDelProxy.Text = "删除";
            mainMenuDelProxy.Click += MainMenuDelProxy_Click;
            // 
            // SysProxySettingForm
            // 
            AutoScaleDimensions = new SizeF(14F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 525);
            Controls.Add(groupBox1);
            Controls.Add(sysProxysView);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "SysProxySettingForm";
            Text = "系统代理设置";
            ((System.ComponentModel.ISupportInitialize)sysProxysView).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            contextMenu.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private DataGridView sysProxysView;
        private GroupBox groupBox1;
        private TextBox inputName;
        private Label label1;
        private TextBox inputPac;
        private CheckBox cbPac;
        private CheckBox cbEnv;
        private Button btnSave;
        private ContextMenuStrip contextMenu;
        private ToolStripMenuItem mainMenuDelProxy;
        private Button btnClear;
        private Label label2;
    }
}