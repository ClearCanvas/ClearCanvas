namespace ClearCanvas.ImageServer.Model.SqlServer.CodeGenerator
{
    partial class MainForm
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
            this.textBoxModelNamespace = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonGenerateCode = new System.Windows.Forms.Button();
            this.textBoxModelFolder = new System.Windows.Forms.TextBox();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.textBoxEntityInterfaceNamespace = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxEntityInterfaceFolder = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxEntityImplementationNamespace = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxEntityImplementationFolder = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.comboBoxDatabase = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxModelNamespace
            // 
            this.textBoxModelNamespace.Location = new System.Drawing.Point(6, 32);
            this.textBoxModelNamespace.Name = "textBoxModelNamespace";
            this.textBoxModelNamespace.Size = new System.Drawing.Size(353, 20);
            this.textBoxModelNamespace.TabIndex = 0;
            this.textBoxModelNamespace.Text = "ClearCanvas.ImageServer.Model";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Namespace";
            // 
            // buttonGenerateCode
            // 
            this.buttonGenerateCode.Location = new System.Drawing.Point(12, 403);
            this.buttonGenerateCode.Name = "buttonGenerateCode";
            this.buttonGenerateCode.Size = new System.Drawing.Size(100, 23);
            this.buttonGenerateCode.TabIndex = 2;
            this.buttonGenerateCode.Text = "Generate Code";
            this.buttonGenerateCode.UseVisualStyleBackColor = true;
            this.buttonGenerateCode.Click += new System.EventHandler(this.buttonGenerateCode_Click);
            // 
            // textBoxModelFolder
            // 
            this.textBoxModelFolder.Location = new System.Drawing.Point(6, 76);
            this.textBoxModelFolder.Name = "textBoxModelFolder";
            this.textBoxModelFolder.Size = new System.Drawing.Size(353, 20);
            this.textBoxModelFolder.TabIndex = 3;
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Location = new System.Drawing.Point(365, 76);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowse.TabIndex = 4;
            this.buttonBrowse.Text = "Browse...";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(131, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "ImageServer Model Folder";
            // 
            // textBoxEntityInterfaceNamespace
            // 
            this.textBoxEntityInterfaceNamespace.Location = new System.Drawing.Point(6, 32);
            this.textBoxEntityInterfaceNamespace.Name = "textBoxEntityInterfaceNamespace";
            this.textBoxEntityInterfaceNamespace.Size = new System.Drawing.Size(433, 20);
            this.textBoxEntityInterfaceNamespace.TabIndex = 6;
            this.textBoxEntityInterfaceNamespace.Text = "ClearCanvas.ImageServer.Model.EntityBrokers";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Namespace";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 59);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(144, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Entity Broker Interface Folder";
            // 
            // textBoxEntityInterfaceFolder
            // 
            this.textBoxEntityInterfaceFolder.Location = new System.Drawing.Point(6, 75);
            this.textBoxEntityInterfaceFolder.Name = "textBoxEntityInterfaceFolder";
            this.textBoxEntityInterfaceFolder.Size = new System.Drawing.Size(433, 20);
            this.textBoxEntityInterfaceFolder.TabIndex = 9;
            this.textBoxEntityInterfaceFolder.Text = "c:\\Projects\\ClearCanvas\\ImageServer\\Model\\EntityBrokers";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Namespace";
            // 
            // textBoxEntityImplementationNamespace
            // 
            this.textBoxEntityImplementationNamespace.Location = new System.Drawing.Point(6, 32);
            this.textBoxEntityImplementationNamespace.Name = "textBoxEntityImplementationNamespace";
            this.textBoxEntityImplementationNamespace.Size = new System.Drawing.Size(433, 20);
            this.textBoxEntityImplementationNamespace.TabIndex = 11;
            this.textBoxEntityImplementationNamespace.Text = "ClearCanvas.ImageServer.Model.SqlServer.EntityBrokers";
            this.textBoxEntityImplementationNamespace.TextChanged += new System.EventHandler(this.textBoxEntityImplementationNamespace_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 59);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(184, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Entity Broker SQL Server 2005 Folder";
            // 
            // textBoxEntityImplementationFolder
            // 
            this.textBoxEntityImplementationFolder.Location = new System.Drawing.Point(6, 75);
            this.textBoxEntityImplementationFolder.Name = "textBoxEntityImplementationFolder";
            this.textBoxEntityImplementationFolder.Size = new System.Drawing.Size(433, 20);
            this.textBoxEntityImplementationFolder.TabIndex = 13;
            this.textBoxEntityImplementationFolder.Text = "c:\\Projects\\ClearCanvas\\ImageServer\\Model\\SqlServer\\EntityBrokers";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textBoxModelNamespace);
            this.groupBox1.Controls.Add(this.textBoxModelFolder);
            this.groupBox1.Controls.Add(this.buttonBrowse);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(12, 69);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(456, 106);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Root Model";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxEntityInterfaceNamespace);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.textBoxEntityInterfaceFolder);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(13, 181);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(455, 106);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Entity Broker Interfaces, Criteria, and Columns";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBoxEntityImplementationNamespace);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.textBoxEntityImplementationFolder);
            this.groupBox3.Location = new System.Drawing.Point(13, 293);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(455, 104);
            this.groupBox3.TabIndex = 16;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Entity Broker Implementation";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.comboBoxDatabase);
            this.groupBox4.Location = new System.Drawing.Point(12, 13);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(456, 50);
            this.groupBox4.TabIndex = 17;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Database";
            // 
            // comboBoxDatabase
            // 
            this.comboBoxDatabase.FormattingEnabled = true;
            this.comboBoxDatabase.Location = new System.Drawing.Point(7, 20);
            this.comboBoxDatabase.Name = "comboBoxDatabase";
            this.comboBoxDatabase.Size = new System.Drawing.Size(352, 21);
            this.comboBoxDatabase.TabIndex = 0;
            this.comboBoxDatabase.SelectedIndexChanged += new System.EventHandler(this.comboBoxDatabase_SelectedIndexChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(495, 437);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonGenerateCode);
            this.Name = "MainForm";
            this.Text = "ClearCanvas.ImageServer.Model.SqlServer.CodeGenerator";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxModelNamespace;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonGenerateCode;
        private System.Windows.Forms.TextBox textBoxModelFolder;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.TextBox textBoxEntityInterfaceNamespace;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxEntityInterfaceFolder;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxEntityImplementationNamespace;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxEntityImplementationFolder;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox comboBoxDatabase;
    }
}