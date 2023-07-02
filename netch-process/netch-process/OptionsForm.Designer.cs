namespace netch_process
{
    partial class OptionsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.dnsProxy = new System.Windows.Forms.RadioButton();
            this.saveBtn = new System.Windows.Forms.Button();
            this.filterSubProcess = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.filterOnlyDNS = new System.Windows.Forms.CheckBox();
            this.dnsServer = new System.Windows.Forms.TextBox();
            this.filterDNS = new System.Windows.Forms.CheckBox();
            this.filterUDP = new System.Windows.Forms.CheckBox();
            this.filterTcp = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Controls.Add(this.saveBtn);
            this.groupBox1.Controls.Add(this.filterSubProcess);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.filterOnlyDNS);
            this.groupBox1.Controls.Add(this.dnsServer);
            this.groupBox1.Controls.Add(this.filterDNS);
            this.groupBox1.Controls.Add(this.filterUDP);
            this.groupBox1.Controls.Add(this.filterTcp);
            this.groupBox1.Location = new System.Drawing.Point(12, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(333, 141);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "选项设置";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.radioButton1);
            this.panel1.Controls.Add(this.dnsProxy);
            this.panel1.Location = new System.Drawing.Point(5, 66);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(218, 25);
            this.panel1.TabIndex = 2;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(107, 2);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(112, 21);
            this.radioButton1.TabIndex = 2;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "服务器处理DNS";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // dnsProxy
            // 
            this.dnsProxy.AutoSize = true;
            this.dnsProxy.Location = new System.Drawing.Point(1, 2);
            this.dnsProxy.Name = "dnsProxy";
            this.dnsProxy.Size = new System.Drawing.Size(100, 21);
            this.dnsProxy.TabIndex = 1;
            this.dnsProxy.TabStop = true;
            this.dnsProxy.Text = "代理处理DNS";
            this.dnsProxy.UseVisualStyleBackColor = true;
            // 
            // saveBtn
            // 
            this.saveBtn.Location = new System.Drawing.Point(135, 112);
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(70, 23);
            this.saveBtn.TabIndex = 9;
            this.saveBtn.Text = "保存更改";
            this.saveBtn.UseVisualStyleBackColor = true;
            // 
            // filterSubProcess
            // 
            this.filterSubProcess.AutoSize = true;
            this.filterSubProcess.Location = new System.Drawing.Point(231, 22);
            this.filterSubProcess.Name = "filterSubProcess";
            this.filterSubProcess.Size = new System.Drawing.Size(87, 21);
            this.filterSubProcess.TabIndex = 8;
            this.filterSubProcess.Text = "处理子进程";
            this.filterSubProcess.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label2.Location = new System.Drawing.Point(3, 92);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(324, 17);
            this.label2.TabIndex = 7;
            this.label2.Text = "通过代理处理DNS请求，或者将DNS请求指向指定的服务器";
            // 
            // filterOnlyDNS
            // 
            this.filterOnlyDNS.AutoSize = true;
            this.filterOnlyDNS.Location = new System.Drawing.Point(112, 43);
            this.filterOnlyDNS.Name = "filterOnlyDNS";
            this.filterOnlyDNS.Size = new System.Drawing.Size(101, 21);
            this.filterOnlyDNS.TabIndex = 5;
            this.filterOnlyDNS.Text = "处理进程DNS";
            this.filterOnlyDNS.UseVisualStyleBackColor = true;
            // 
            // dnsServer
            // 
            this.dnsServer.Location = new System.Drawing.Point(230, 67);
            this.dnsServer.Name = "dnsServer";
            this.dnsServer.Size = new System.Drawing.Size(95, 23);
            this.dnsServer.TabIndex = 4;
            // 
            // filterDNS
            // 
            this.filterDNS.AutoSize = true;
            this.filterDNS.Location = new System.Drawing.Point(6, 45);
            this.filterDNS.Name = "filterDNS";
            this.filterDNS.Size = new System.Drawing.Size(77, 21);
            this.filterDNS.TabIndex = 2;
            this.filterDNS.Text = "处理DNS";
            this.filterDNS.UseVisualStyleBackColor = true;
            // 
            // filterUDP
            // 
            this.filterUDP.AutoSize = true;
            this.filterUDP.Location = new System.Drawing.Point(112, 22);
            this.filterUDP.Name = "filterUDP";
            this.filterUDP.Size = new System.Drawing.Size(76, 21);
            this.filterUDP.TabIndex = 1;
            this.filterUDP.Text = "处理UDP";
            this.filterUDP.UseVisualStyleBackColor = true;
            // 
            // filterTcp
            // 
            this.filterTcp.AutoSize = true;
            this.filterTcp.Location = new System.Drawing.Point(6, 22);
            this.filterTcp.Name = "filterTcp";
            this.filterTcp.Size = new System.Drawing.Size(73, 21);
            this.filterTcp.TabIndex = 0;
            this.filterTcp.Text = "处理TCP";
            this.filterTcp.UseVisualStyleBackColor = true;
            // 
            // Options
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(356, 155);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Options";
            this.Text = "选项设置";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private GroupBox groupBox1;
        private Panel panel1;
        private RadioButton radioButton1;
        private RadioButton dnsProxy;
        private Button saveBtn;
        private CheckBox filterSubProcess;
        private Label label2;
        private CheckBox filterOnlyDNS;
        private TextBox dnsServer;
        private CheckBox filterDNS;
        private CheckBox filterUDP;
        private CheckBox filterTcp;
    }
}