namespace ClearCanvas.ImageServer.TestApp
{
    partial class UsageTrackingForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxProduct = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxVersion = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxOS = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxRegion = new System.Windows.Forms.TextBox();
            this.buttonSend = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxLicense = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxEdition = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxMachineIdentifier = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxComponent = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxAppValue = new System.Windows.Forms.TextBox();
            this.textBoxAppKey = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Product";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxProduct
            // 
            this.textBoxProduct.Location = new System.Drawing.Point(77, 26);
            this.textBoxProduct.Name = "textBoxProduct";
            this.textBoxProduct.Size = new System.Drawing.Size(464, 20);
            this.textBoxProduct.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(29, 105);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Version";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxVersion
            // 
            this.textBoxVersion.Location = new System.Drawing.Point(76, 102);
            this.textBoxVersion.Name = "textBoxVersion";
            this.textBoxVersion.Size = new System.Drawing.Size(464, 20);
            this.textBoxVersion.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(48, 131);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(22, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "OS";
            // 
            // textBoxOS
            // 
            this.textBoxOS.Location = new System.Drawing.Point(76, 128);
            this.textBoxOS.Name = "textBoxOS";
            this.textBoxOS.Size = new System.Drawing.Size(464, 20);
            this.textBoxOS.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(31, 157);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Region";
            // 
            // textBoxRegion
            // 
            this.textBoxRegion.Location = new System.Drawing.Point(76, 154);
            this.textBoxRegion.Name = "textBoxRegion";
            this.textBoxRegion.Size = new System.Drawing.Size(464, 20);
            this.textBoxRegion.TabIndex = 5;
            // 
            // buttonSend
            // 
            this.buttonSend.Location = new System.Drawing.Point(12, 437);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(75, 23);
            this.buttonSend.TabIndex = 10;
            this.buttonSend.Text = "Send";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(28, 203);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "License";
            // 
            // textBoxLicense
            // 
            this.textBoxLicense.Location = new System.Drawing.Point(76, 211);
            this.textBoxLicense.Multiline = true;
            this.textBoxLicense.Name = "textBoxLicense";
            this.textBoxLicense.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxLicense.Size = new System.Drawing.Size(465, 91);
            this.textBoxLicense.TabIndex = 7;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxEdition);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.textBoxProduct);
            this.groupBox1.Controls.Add(this.textBoxMachineIdentifier);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.textBoxComponent);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.textBoxRegion);
            this.groupBox1.Controls.Add(this.textBoxLicense);
            this.groupBox1.Controls.Add(this.textBoxOS);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.textBoxVersion);
            this.groupBox1.Location = new System.Drawing.Point(12, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(547, 317);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General Product Information";
            // 
            // textBoxEdition
            // 
            this.textBoxEdition.Location = new System.Drawing.Point(77, 76);
            this.textBoxEdition.Name = "textBoxEdition";
            this.textBoxEdition.Size = new System.Drawing.Size(463, 20);
            this.textBoxEdition.TabIndex = 2;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(31, 79);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(39, 13);
            this.label10.TabIndex = 15;
            this.label10.Text = "Edition";
            // 
            // textBoxMachineIdentifier
            // 
            this.textBoxMachineIdentifier.Location = new System.Drawing.Point(76, 180);
            this.textBoxMachineIdentifier.Name = "textBoxMachineIdentifier";
            this.textBoxMachineIdentifier.Size = new System.Drawing.Size(464, 20);
            this.textBoxMachineIdentifier.TabIndex = 6;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(22, 183);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(48, 13);
            this.label9.TabIndex = 13;
            this.label9.Text = "Machine";
            // 
            // textBoxComponent
            // 
            this.textBoxComponent.Location = new System.Drawing.Point(77, 49);
            this.textBoxComponent.Name = "textBoxComponent";
            this.textBoxComponent.Size = new System.Drawing.Size(465, 20);
            this.textBoxComponent.TabIndex = 1;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(9, 52);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(61, 13);
            this.label8.TabIndex = 11;
            this.label8.Text = "Component";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxAppValue);
            this.groupBox2.Controls.Add(this.textBoxAppKey);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Location = new System.Drawing.Point(12, 332);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(547, 99);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Application Specific Data";
            // 
            // textBoxAppValue
            // 
            this.textBoxAppValue.Location = new System.Drawing.Point(76, 49);
            this.textBoxAppValue.Name = "textBoxAppValue";
            this.textBoxAppValue.Size = new System.Drawing.Size(465, 20);
            this.textBoxAppValue.TabIndex = 9;
            // 
            // textBoxAppKey
            // 
            this.textBoxAppKey.Location = new System.Drawing.Point(76, 17);
            this.textBoxAppKey.Name = "textBoxAppKey";
            this.textBoxAppKey.Size = new System.Drawing.Size(465, 20);
            this.textBoxAppKey.TabIndex = 8;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(36, 52);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(34, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Value";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(45, 20);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(25, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Key";
            // 
            // UsageTrackingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(571, 466);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.buttonSend);
            this.Controls.Add(this.groupBox1);
            this.Name = "UsageTrackingForm";
            this.Text = "Usage Tracking";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxProduct;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxVersion;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxOS;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxRegion;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxLicense;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBoxAppValue;
        private System.Windows.Forms.TextBox textBoxAppKey;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxComponent;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxMachineIdentifier;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBoxEdition;
        private System.Windows.Forms.Label label10;
    }
}