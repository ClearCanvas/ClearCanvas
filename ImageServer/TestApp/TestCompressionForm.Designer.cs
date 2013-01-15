namespace ClearCanvas.ImageServer.TestApp
{
    partial class TestCompressionForm
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
            this.buttonDecompress = new System.Windows.Forms.Button();
            this.buttonCompress = new System.Windows.Forms.Button();
            this.comboBoxCompressionType = new System.Windows.Forms.ComboBox();
            this.textBoxSourceFile = new System.Windows.Forms.TextBox();
            this.textBoxDestinationFile = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonBrowseSourceFile = new System.Windows.Forms.Button();
            this.buttonBrowseDestinationFile = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // buttonDecompress
            // 
            this.buttonDecompress.Location = new System.Drawing.Point(12, 12);
            this.buttonDecompress.Name = "buttonDecompress";
            this.buttonDecompress.Size = new System.Drawing.Size(142, 23);
            this.buttonDecompress.TabIndex = 0;
            this.buttonDecompress.Text = "Decompress Image";
            this.buttonDecompress.UseVisualStyleBackColor = true;
            this.buttonDecompress.Click += new System.EventHandler(this.buttonDecompress_Click);
            // 
            // buttonCompress
            // 
            this.buttonCompress.Location = new System.Drawing.Point(12, 57);
            this.buttonCompress.Name = "buttonCompress";
            this.buttonCompress.Size = new System.Drawing.Size(142, 23);
            this.buttonCompress.TabIndex = 1;
            this.buttonCompress.Text = "Compress Image";
            this.buttonCompress.UseVisualStyleBackColor = true;
            this.buttonCompress.Click += new System.EventHandler(this.buttonCompress_Click);
            // 
            // comboBoxCompressionType
            // 
            this.comboBoxCompressionType.FormattingEnabled = true;
            this.comboBoxCompressionType.Location = new System.Drawing.Point(178, 59);
            this.comboBoxCompressionType.Name = "comboBoxCompressionType";
            this.comboBoxCompressionType.Size = new System.Drawing.Size(254, 21);
            this.comboBoxCompressionType.TabIndex = 2;
            // 
            // textBoxSourceFile
            // 
            this.textBoxSourceFile.Location = new System.Drawing.Point(12, 151);
            this.textBoxSourceFile.Name = "textBoxSourceFile";
            this.textBoxSourceFile.Size = new System.Drawing.Size(339, 20);
            this.textBoxSourceFile.TabIndex = 3;
            // 
            // textBoxDestinationFile
            // 
            this.textBoxDestinationFile.Location = new System.Drawing.Point(12, 203);
            this.textBoxDestinationFile.Name = "textBoxDestinationFile";
            this.textBoxDestinationFile.Size = new System.Drawing.Size(339, 20);
            this.textBoxDestinationFile.TabIndex = 4;
            this.textBoxDestinationFile.Text = "f:\\compress.dcm";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 135);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Source";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 187);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Destination";
            // 
            // buttonBrowseSourceFile
            // 
            this.buttonBrowseSourceFile.Location = new System.Drawing.Point(357, 148);
            this.buttonBrowseSourceFile.Name = "buttonBrowseSourceFile";
            this.buttonBrowseSourceFile.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowseSourceFile.TabIndex = 7;
            this.buttonBrowseSourceFile.Text = "Browse...";
            this.buttonBrowseSourceFile.UseVisualStyleBackColor = true;
            this.buttonBrowseSourceFile.Click += new System.EventHandler(this.buttonBrowseSourceFile_Click);
            // 
            // buttonBrowseDestinationFile
            // 
            this.buttonBrowseDestinationFile.Location = new System.Drawing.Point(357, 200);
            this.buttonBrowseDestinationFile.Name = "buttonBrowseDestinationFile";
            this.buttonBrowseDestinationFile.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowseDestinationFile.TabIndex = 8;
            this.buttonBrowseDestinationFile.Text = "Browse...";
            this.buttonBrowseDestinationFile.UseVisualStyleBackColor = true;
            this.buttonBrowseDestinationFile.Click += new System.EventHandler(this.buttonBrowseDestinationFile_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "dcm";
            this.openFileDialog.FileName = "Input";
            this.openFileDialog.Filter = "DICOM Files|*.dcm|All files|*.*";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "DICOM Files|*.dcm|All files|*.*";
            // 
            // TestCompressionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 271);
            this.Controls.Add(this.buttonBrowseDestinationFile);
            this.Controls.Add(this.buttonBrowseSourceFile);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxDestinationFile);
            this.Controls.Add(this.textBoxSourceFile);
            this.Controls.Add(this.comboBoxCompressionType);
            this.Controls.Add(this.buttonCompress);
            this.Controls.Add(this.buttonDecompress);
            this.Name = "TestCompressionForm";
            this.Text = "Compression Test";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonDecompress;
        private System.Windows.Forms.Button buttonCompress;
        private System.Windows.Forms.ComboBox comboBoxCompressionType;
        private System.Windows.Forms.TextBox textBoxSourceFile;
        private System.Windows.Forms.TextBox textBoxDestinationFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonBrowseSourceFile;
        private System.Windows.Forms.Button buttonBrowseDestinationFile;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
    }
}