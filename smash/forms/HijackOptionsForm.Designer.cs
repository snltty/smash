namespace smash.forms
{
    partial class HijackOptionsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HijackOptionsForm));
            groupBox1 = new GroupBox();
            panel1 = new Panel();
            dnsOnly = new RadioButton();
            dnsProxy = new RadioButton();
            btnSave = new Button();
            filterSubProcess = new CheckBox();
            label2 = new Label();
            filterOnlyDNS = new CheckBox();
            dnsServer = new TextBox();
            filterDNS = new CheckBox();
            filterUDP = new CheckBox();
            filterTcp = new CheckBox();
            groupBox1.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(panel1);
            groupBox1.Controls.Add(btnSave);
            groupBox1.Controls.Add(filterSubProcess);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(filterOnlyDNS);
            groupBox1.Controls.Add(dnsServer);
            groupBox1.Controls.Add(filterDNS);
            groupBox1.Controls.Add(filterUDP);
            groupBox1.Controls.Add(filterTcp);
            groupBox1.Location = new Point(24, 11);
            groupBox1.Margin = new Padding(6, 5, 6, 5);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(6, 5, 6, 5);
            groupBox1.Size = new Size(666, 257);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "选项设置";
            // 
            // panel1
            // 
            panel1.Controls.Add(dnsOnly);
            panel1.Controls.Add(dnsProxy);
            panel1.Location = new Point(10, 120);
            panel1.Margin = new Padding(6, 5, 6, 5);
            panel1.Name = "panel1";
            panel1.Size = new Size(436, 46);
            panel1.TabIndex = 2;
            // 
            // dnsOnly
            // 
            dnsOnly.AutoSize = true;
            dnsOnly.Location = new Point(214, 4);
            dnsOnly.Margin = new Padding(6, 5, 6, 5);
            dnsOnly.Name = "dnsOnly";
            dnsOnly.Size = new Size(217, 35);
            dnsOnly.TabIndex = 2;
            dnsOnly.TabStop = true;
            dnsOnly.Text = "服务器处理DNS";
            dnsOnly.UseVisualStyleBackColor = true;
            // 
            // dnsProxy
            // 
            dnsProxy.AutoSize = true;
            dnsProxy.Location = new Point(2, 4);
            dnsProxy.Margin = new Padding(6, 5, 6, 5);
            dnsProxy.Name = "dnsProxy";
            dnsProxy.Size = new Size(193, 35);
            dnsProxy.TabIndex = 1;
            dnsProxy.TabStop = true;
            dnsProxy.Text = "代理处理DNS";
            dnsProxy.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(270, 204);
            btnSave.Margin = new Padding(6, 5, 6, 5);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(140, 42);
            btnSave.TabIndex = 9;
            btnSave.Text = "保存更改";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += saveBtn_Click;
            // 
            // filterSubProcess
            // 
            filterSubProcess.AutoSize = true;
            filterSubProcess.Location = new Point(462, 40);
            filterSubProcess.Margin = new Padding(6, 5, 6, 5);
            filterSubProcess.Name = "filterSubProcess";
            filterSubProcess.Size = new Size(166, 35);
            filterSubProcess.TabIndex = 8;
            filterSubProcess.Text = "处理子进程";
            filterSubProcess.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = SystemColors.ControlDarkDark;
            label2.Location = new Point(6, 168);
            label2.Margin = new Padding(6, 0, 6, 0);
            label2.Name = "label2";
            label2.Size = new Size(646, 31);
            label2.TabIndex = 7;
            label2.Text = "通过代理处理DNS请求，或者将DNS请求指向指定的服务器";
            // 
            // filterOnlyDNS
            // 
            filterOnlyDNS.AutoSize = true;
            filterOnlyDNS.Location = new Point(224, 78);
            filterOnlyDNS.Margin = new Padding(6, 5, 6, 5);
            filterOnlyDNS.Name = "filterOnlyDNS";
            filterOnlyDNS.Size = new Size(194, 35);
            filterOnlyDNS.TabIndex = 5;
            filterOnlyDNS.Text = "处理进程DNS";
            filterOnlyDNS.UseVisualStyleBackColor = true;
            // 
            // dnsServer
            // 
            dnsServer.Location = new Point(460, 122);
            dnsServer.Margin = new Padding(6, 5, 6, 5);
            dnsServer.Name = "dnsServer";
            dnsServer.Size = new Size(186, 38);
            dnsServer.TabIndex = 4;
            // 
            // filterDNS
            // 
            filterDNS.AutoSize = true;
            filterDNS.Location = new Point(12, 82);
            filterDNS.Margin = new Padding(6, 5, 6, 5);
            filterDNS.Name = "filterDNS";
            filterDNS.Size = new Size(146, 35);
            filterDNS.TabIndex = 2;
            filterDNS.Text = "处理DNS";
            filterDNS.UseVisualStyleBackColor = true;
            // 
            // filterUDP
            // 
            filterUDP.AutoSize = true;
            filterUDP.Location = new Point(224, 40);
            filterUDP.Margin = new Padding(6, 5, 6, 5);
            filterUDP.Name = "filterUDP";
            filterUDP.Size = new Size(145, 35);
            filterUDP.TabIndex = 1;
            filterUDP.Text = "处理UDP";
            filterUDP.UseVisualStyleBackColor = true;
            // 
            // filterTcp
            // 
            filterTcp.AutoSize = true;
            filterTcp.Location = new Point(12, 40);
            filterTcp.Margin = new Padding(6, 5, 6, 5);
            filterTcp.Name = "filterTcp";
            filterTcp.Size = new Size(139, 35);
            filterTcp.TabIndex = 0;
            filterTcp.Text = "处理TCP";
            filterTcp.UseVisualStyleBackColor = true;
            // 
            // HijackOptionsForm
            // 
            AutoScaleDimensions = new SizeF(14F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(712, 283);
            Controls.Add(groupBox1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(6, 5, 6, 5);
            Name = "HijackOptionsForm";
            Text = "进程劫持选项设置";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private Panel panel1;
        private RadioButton dnsProxy;
        private Button btnSave;
        private CheckBox filterSubProcess;
        private Label label2;
        private CheckBox filterOnlyDNS;
        private TextBox dnsServer;
        private CheckBox filterDNS;
        private CheckBox filterUDP;
        private CheckBox filterTcp;
        private RadioButton dnsOnly;
    }
}