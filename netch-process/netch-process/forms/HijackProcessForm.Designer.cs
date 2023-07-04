namespace netch_process
{
    partial class HijackProcessForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HijackProcessForm));
            groupBox1 = new GroupBox();
            listProcess = new ListBox();
            cmbGroup = new ComboBox();
            groupBox2 = new GroupBox();
            btnDelGroup = new Button();
            btnClearForm = new Button();
            btnSaveGroup = new Button();
            inputName = new TextBox();
            btnAddProcess = new Button();
            btnDelProcess = new Button();
            groupBox3 = new GroupBox();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(listProcess);
            groupBox1.Controls.Add(cmbGroup);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(378, 413);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "进程组";
            // 
            // listProcess
            // 
            listProcess.FormattingEnabled = true;
            listProcess.HorizontalScrollbar = true;
            listProcess.ItemHeight = 31;
            listProcess.Location = new Point(12, 87);
            listProcess.Name = "listProcess";
            listProcess.Size = new Size(353, 314);
            listProcess.TabIndex = 1;
            // 
            // cmbGroup
            // 
            cmbGroup.FormattingEnabled = true;
            cmbGroup.Location = new Point(12, 42);
            cmbGroup.Name = "cmbGroup";
            cmbGroup.Size = new Size(353, 39);
            cmbGroup.TabIndex = 0;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(btnDelGroup);
            groupBox2.Controls.Add(btnClearForm);
            groupBox2.Controls.Add(btnSaveGroup);
            groupBox2.Controls.Add(inputName);
            groupBox2.Location = new Point(396, 12);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(345, 210);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "添加修改组";
            // 
            // btnDelGroup
            // 
            btnDelGroup.Location = new Point(178, 146);
            btnDelGroup.Name = "btnDelGroup";
            btnDelGroup.Size = new Size(150, 46);
            btnDelGroup.TabIndex = 3;
            btnDelGroup.Text = "删除分组";
            btnDelGroup.UseVisualStyleBackColor = true;
            btnDelGroup.Click += btnDelGroup_Click;
            // 
            // btnClearForm
            // 
            btnClearForm.Location = new Point(13, 146);
            btnClearForm.Name = "btnClearForm";
            btnClearForm.Size = new Size(150, 46);
            btnClearForm.TabIndex = 2;
            btnClearForm.Text = "清空表单";
            btnClearForm.UseVisualStyleBackColor = true;
            btnClearForm.Click += btnClearForm_Click;
            // 
            // btnSaveGroup
            // 
            btnSaveGroup.Location = new Point(93, 87);
            btnSaveGroup.Name = "btnSaveGroup";
            btnSaveGroup.Size = new Size(150, 46);
            btnSaveGroup.TabIndex = 1;
            btnSaveGroup.Text = "保存分组";
            btnSaveGroup.UseVisualStyleBackColor = true;
            btnSaveGroup.Click += btnSaveGroup_Click;
            // 
            // inputName
            // 
            inputName.Location = new Point(13, 43);
            inputName.Name = "inputName";
            inputName.Size = new Size(315, 38);
            inputName.TabIndex = 0;
            // 
            // btnAddProcess
            // 
            btnAddProcess.Location = new Point(13, 48);
            btnAddProcess.Name = "btnAddProcess";
            btnAddProcess.Size = new Size(150, 46);
            btnAddProcess.TabIndex = 3;
            btnAddProcess.Text = "添加进程";
            btnAddProcess.UseVisualStyleBackColor = true;
            btnAddProcess.Click += btnAddProcess_Click;
            // 
            // btnDelProcess
            // 
            btnDelProcess.Location = new Point(178, 48);
            btnDelProcess.Name = "btnDelProcess";
            btnDelProcess.Size = new Size(150, 46);
            btnDelProcess.TabIndex = 4;
            btnDelProcess.Text = "删除进程";
            btnDelProcess.UseVisualStyleBackColor = true;
            btnDelProcess.Click += btnDelProcess_Click;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(btnDelProcess);
            groupBox3.Controls.Add(btnAddProcess);
            groupBox3.Location = new Point(396, 267);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(345, 158);
            groupBox3.TabIndex = 2;
            groupBox3.TabStop = false;
            groupBox3.Text = "添加删除进程";
            // 
            // HijackProcessForm
            // 
            AutoScaleDimensions = new SizeF(14F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(755, 434);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "HijackProcessForm";
            Text = "进程劫持进程设置";
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private ListBox listProcess;
        private ComboBox cmbGroup;
        private GroupBox groupBox2;
        private Button btnClearForm;
        private Button btnSaveGroup;
        private TextBox inputName;
        private Button btnDelGroup;
        private Button btnAddProcess;
        private Button btnDelProcess;
        private GroupBox groupBox3;
    }
}