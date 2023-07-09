namespace smash.plugins.proxy
{
    partial class ProxyForm
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
            groupBox2 = new GroupBox();
            labelProxy = new Label();
            cmbProxy = new ComboBox();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(labelProxy);
            groupBox2.Controls.Add(cmbProxy);
            groupBox2.Location = new Point(238, 124);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(298, 146);
            groupBox2.TabIndex = 5;
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
            // 
            // ProxyForm
            // 
            AutoScaleDimensions = new SizeF(14F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(groupBox2);
            Name = "ProxyForm";
            Text = "代理配置";
            groupBox2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox2;
        private Label labelProxy;
        private ComboBox cmbProxy;
    }
}