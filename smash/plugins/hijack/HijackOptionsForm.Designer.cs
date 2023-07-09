namespace smash.plugins
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
            btnSave = new Button();
            filterDNS = new CheckBox();
            filterUDP = new CheckBox();
            filterTcp = new CheckBox();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btnSave);
            groupBox1.Controls.Add(filterDNS);
            groupBox1.Controls.Add(filterUDP);
            groupBox1.Controls.Add(filterTcp);
            groupBox1.Location = new Point(18, 11);
            groupBox1.Margin = new Padding(6, 5, 6, 5);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(6, 5, 6, 5);
            groupBox1.Size = new Size(197, 255);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "选项设置";
            // 
            // btnSave
            // 
            btnSave.Location = new Point(27, 191);
            btnSave.Margin = new Padding(6, 5, 6, 5);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(140, 42);
            btnSave.TabIndex = 9;
            btnSave.Text = "保存更改";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += saveBtn_Click;
            // 
            // filterDNS
            // 
            filterDNS.AutoSize = true;
            filterDNS.Location = new Point(27, 138);
            filterDNS.Margin = new Padding(6, 5, 6, 5);
            filterDNS.Name = "filterDNS";
            filterDNS.Size = new Size(146, 35);
            filterDNS.TabIndex = 2;
            filterDNS.Text = "代理DNS";
            filterDNS.UseVisualStyleBackColor = true;
            // 
            // filterUDP
            // 
            filterUDP.AutoSize = true;
            filterUDP.Location = new Point(27, 93);
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
            filterTcp.Location = new Point(27, 48);
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
            ClientSize = new Size(236, 286);
            Controls.Add(groupBox1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(6, 5, 6, 5);
            Name = "HijackOptionsForm";
            Text = "进程劫持选项设置";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private Button btnSave;
        private CheckBox filterDNS;
        private CheckBox filterUDP;
        private CheckBox filterTcp;
    }
}