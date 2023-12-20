namespace ClearCanvas.ImageViewer.Tools.MedicalPhysics.View.WinForms
{
    partial class WeeklyCTComponentControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.TitleLabel = new System.Windows.Forms.Label();
            this.ContrastScaleTextField = new ClearCanvas.Desktop.View.WinForms.TextField();
            this.HighContrastTextField = new ClearCanvas.Desktop.View.WinForms.TextField();
            this.WaterValueTextField = new ClearCanvas.Desktop.View.WinForms.TextField();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.tableLayoutPanel1.Controls.Add(this.TitleLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ContrastScaleTextField, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.HighContrastTextField, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.WaterValueTextField, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(313, 478);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // TitleLabel
            // 
            this.TitleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TitleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TitleLabel.Location = new System.Drawing.Point(3, 3);
            this.TitleLabel.Margin = new System.Windows.Forms.Padding(3);
            this.TitleLabel.Name = "TitleLabel";
            this.TitleLabel.Size = new System.Drawing.Size(307, 54);
            this.TitleLabel.TabIndex = 0;
            this.TitleLabel.Text = "Weekly CT QC";
            this.TitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ContrastScaleTextField
            // 
            this.ContrastScaleTextField.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ContrastScaleTextField.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ContrastScaleTextField.LabelText = "Contrast Scale";
            this.ContrastScaleTextField.Location = new System.Drawing.Point(3, 62);
            this.ContrastScaleTextField.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ContrastScaleTextField.Mask = "";
            this.ContrastScaleTextField.Name = "ContrastScaleTextField";
            this.ContrastScaleTextField.PasswordChar = '\0';
            this.ContrastScaleTextField.Size = new System.Drawing.Size(307, 46);
            this.ContrastScaleTextField.TabIndex = 9;
            this.ContrastScaleTextField.ToolTip = null;
            this.ContrastScaleTextField.Value = null;
            // 
            // HighContrastTextField
            // 
            this.HighContrastTextField.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HighContrastTextField.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HighContrastTextField.LabelText = "High Contrast Resolution";
            this.HighContrastTextField.Location = new System.Drawing.Point(3, 112);
            this.HighContrastTextField.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.HighContrastTextField.Mask = "";
            this.HighContrastTextField.Name = "HighContrastTextField";
            this.HighContrastTextField.PasswordChar = '\0';
            this.HighContrastTextField.Size = new System.Drawing.Size(307, 46);
            this.HighContrastTextField.TabIndex = 10;
            this.HighContrastTextField.ToolTip = null;
            this.HighContrastTextField.Value = null;
            // 
            // WaterValueTextField
            // 
            this.WaterValueTextField.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WaterValueTextField.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WaterValueTextField.LabelText = "WaterValue";
            this.WaterValueTextField.Location = new System.Drawing.Point(3, 162);
            this.WaterValueTextField.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.WaterValueTextField.Mask = "";
            this.WaterValueTextField.Name = "WaterValueTextField";
            this.WaterValueTextField.PasswordChar = '\0';
            this.WaterValueTextField.Size = new System.Drawing.Size(307, 46);
            this.WaterValueTextField.TabIndex = 11;
            this.WaterValueTextField.ToolTip = null;
            this.WaterValueTextField.Value = null;
            // 
            // WeeklyCTComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "WeeklyCTComponentControl";
            this.Size = new System.Drawing.Size(313, 478);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label TitleLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Desktop.View.WinForms.TextField ContrastScaleTextField;
        private Desktop.View.WinForms.TextField HighContrastTextField;
        private Desktop.View.WinForms.TextField WaterValueTextField;
    }
}
