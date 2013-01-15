namespace ImageStreaming
{
    partial class Form1
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
            this.BaseUri = new System.Windows.Forms.TextBox();
            this.StudyUid = new System.Windows.Forms.TextBox();
            this.SeriesUid = new System.Windows.Forms.TextBox();
            this.ObjectUid = new System.Windows.Forms.TextBox();
            this.Go = new System.Windows.Forms.Button();
            this.Browse = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.UseFrame = new System.Windows.Forms.CheckBox();
            this.Frame = new System.Windows.Forms.TextBox();
            this.ContentTypes = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.ServerAE = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // BaseUri
            // 
            this.BaseUri.Location = new System.Drawing.Point(161, 12);
            this.BaseUri.Name = "BaseUri";
            this.BaseUri.Size = new System.Drawing.Size(259, 20);
            this.BaseUri.TabIndex = 0;
            this.BaseUri.Text = "http://localhost:1000/wado";
            // 
            // StudyUid
            // 
            this.StudyUid.Location = new System.Drawing.Point(161, 94);
            this.StudyUid.Name = "StudyUid";
            this.StudyUid.Size = new System.Drawing.Size(259, 20);
            this.StudyUid.TabIndex = 1;
            // 
            // SeriesUid
            // 
            this.SeriesUid.Location = new System.Drawing.Point(161, 120);
            this.SeriesUid.Name = "SeriesUid";
            this.SeriesUid.Size = new System.Drawing.Size(259, 20);
            this.SeriesUid.TabIndex = 1;
            // 
            // ObjectUid
            // 
            this.ObjectUid.Location = new System.Drawing.Point(161, 146);
            this.ObjectUid.Name = "ObjectUid";
            this.ObjectUid.Size = new System.Drawing.Size(259, 20);
            this.ObjectUid.TabIndex = 1;
            // 
            // Go
            // 
            this.Go.Location = new System.Drawing.Point(426, 221);
            this.Go.Name = "Go";
            this.Go.Size = new System.Drawing.Size(75, 23);
            this.Go.TabIndex = 2;
            this.Go.Text = "Go";
            this.Go.UseVisualStyleBackColor = true;
            this.Go.Click += new System.EventHandler(this.Retrieve_Click);
            // 
            // Browse
            // 
            this.Browse.Location = new System.Drawing.Point(426, 144);
            this.Browse.Name = "Browse";
            this.Browse.Size = new System.Drawing.Size(75, 23);
            this.Browse.TabIndex = 3;
            this.Browse.Text = "Browse";
            this.Browse.UseVisualStyleBackColor = true;
            this.Browse.Click += new System.EventHandler(this.Browse_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // UseFrame
            // 
            this.UseFrame.AutoSize = true;
            this.UseFrame.Location = new System.Drawing.Point(161, 185);
            this.UseFrame.Name = "UseFrame";
            this.UseFrame.Size = new System.Drawing.Size(55, 17);
            this.UseFrame.TabIndex = 6;
            this.UseFrame.Text = "Frame";
            this.UseFrame.UseVisualStyleBackColor = true;
            this.UseFrame.CheckedChanged += new System.EventHandler(this.RetrieveFrame_CheckedChanged);
            // 
            // Frame
            // 
            this.Frame.Enabled = false;
            this.Frame.Location = new System.Drawing.Point(222, 182);
            this.Frame.Name = "Frame";
            this.Frame.Size = new System.Drawing.Size(47, 20);
            this.Frame.TabIndex = 7;
            this.Frame.Text = "0";
            // 
            // ContentTypes
            // 
            this.ContentTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ContentTypes.FormattingEnabled = true;
            this.ContentTypes.Location = new System.Drawing.Point(161, 223);
            this.ContentTypes.Name = "ContentTypes";
            this.ContentTypes.Size = new System.Drawing.Size(259, 21);
            this.ContentTypes.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(48, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Base Uri";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(48, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Study Instance Uid";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(48, 126);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Series Uid";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(48, 152);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Object Uid";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(48, 226);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Retrieve type";
            // 
            // ServerAE
            // 
            this.ServerAE.Location = new System.Drawing.Point(161, 48);
            this.ServerAE.Name = "ServerAE";
            this.ServerAE.Size = new System.Drawing.Size(142, 20);
            this.ServerAE.TabIndex = 15;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(51, 54);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Server AE";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(576, 307);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.ServerAE);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ContentTypes);
            this.Controls.Add(this.Frame);
            this.Controls.Add(this.UseFrame);
            this.Controls.Add(this.Browse);
            this.Controls.Add(this.Go);
            this.Controls.Add(this.ObjectUid);
            this.Controls.Add(this.SeriesUid);
            this.Controls.Add(this.StudyUid);
            this.Controls.Add(this.BaseUri);
            this.Name = "Form1";
            this.Text = "Image Streaming Test";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox BaseUri;
        private System.Windows.Forms.TextBox StudyUid;
        private System.Windows.Forms.TextBox SeriesUid;
        private System.Windows.Forms.TextBox ObjectUid;
        private System.Windows.Forms.Button Go;
        private System.Windows.Forms.Button Browse;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.CheckBox UseFrame;
        private System.Windows.Forms.TextBox Frame;
        private System.Windows.Forms.ComboBox ContentTypes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox ServerAE;
        private System.Windows.Forms.Label label6;
    }
}

