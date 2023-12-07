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
            this.HCRLabel = new System.Windows.Forms.Label();
            this.ContrastScaleLabel = new System.Windows.Forms.Label();
            this.ContrastScaleTextBox = new System.Windows.Forms.Label();
            this.WaterValueLabel = new System.Windows.Forms.Label();
            this.HighContrastTextBox = new System.Windows.Forms.Label();
            this.WaterValueTextBox = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 64.21725F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35.78275F));
            this.tableLayoutPanel1.Controls.Add(this.WaterValueTextBox, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.HighContrastTextBox, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.WaterValueLabel, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.ContrastScaleLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.TitleLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.HCRLabel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.ContrastScaleTextBox, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
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
            this.TitleLabel.Size = new System.Drawing.Size(194, 44);
            this.TitleLabel.TabIndex = 0;
            this.TitleLabel.Text = "Weekly CT QC";
            this.TitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // HCRLabel
            // 
            this.HCRLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.HCRLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HCRLabel.Location = new System.Drawing.Point(3, 81);
            this.HCRLabel.Margin = new System.Windows.Forms.Padding(3);
            this.HCRLabel.Name = "HCRLabel";
            this.HCRLabel.Size = new System.Drawing.Size(194, 22);
            this.HCRLabel.TabIndex = 1;
            this.HCRLabel.Text = "High Contrast Resolution";
            this.HCRLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ContrastScaleLabel
            // 
            this.ContrastScaleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ContrastScaleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ContrastScaleLabel.Location = new System.Drawing.Point(3, 53);
            this.ContrastScaleLabel.Margin = new System.Windows.Forms.Padding(3);
            this.ContrastScaleLabel.Name = "ContrastScaleLabel";
            this.ContrastScaleLabel.Size = new System.Drawing.Size(194, 22);
            this.ContrastScaleLabel.TabIndex = 3;
            this.ContrastScaleLabel.Text = "Contrast Scale";
            this.ContrastScaleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ContrastScaleTextBox
            // 
            this.ContrastScaleTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ContrastScaleTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ContrastScaleTextBox.Location = new System.Drawing.Point(203, 53);
            this.ContrastScaleTextBox.Margin = new System.Windows.Forms.Padding(3);
            this.ContrastScaleTextBox.Name = "ContrastScaleTextBox";
            this.ContrastScaleTextBox.Size = new System.Drawing.Size(107, 22);
            this.ContrastScaleTextBox.TabIndex = 4;
            this.ContrastScaleTextBox.Text = "contrast_scale";
            this.ContrastScaleTextBox.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // WaterValueLabel
            // 
            this.WaterValueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.WaterValueLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WaterValueLabel.Location = new System.Drawing.Point(3, 109);
            this.WaterValueLabel.Margin = new System.Windows.Forms.Padding(3);
            this.WaterValueLabel.Name = "WaterValueLabel";
            this.WaterValueLabel.Size = new System.Drawing.Size(194, 22);
            this.WaterValueLabel.TabIndex = 5;
            this.WaterValueLabel.Text = "Water Value";
            this.WaterValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // HighContrastTextBox
            // 
            this.HighContrastTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.HighContrastTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HighContrastTextBox.Location = new System.Drawing.Point(203, 81);
            this.HighContrastTextBox.Margin = new System.Windows.Forms.Padding(3);
            this.HighContrastTextBox.Name = "HighContrastTextBox";
            this.HighContrastTextBox.Size = new System.Drawing.Size(107, 22);
            this.HighContrastTextBox.TabIndex = 6;
            this.HighContrastTextBox.Text = "high_contrast";
            this.HighContrastTextBox.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // WaterValueTextBox
            // 
            this.WaterValueTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.WaterValueTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WaterValueTextBox.Location = new System.Drawing.Point(203, 109);
            this.WaterValueTextBox.Margin = new System.Windows.Forms.Padding(3);
            this.WaterValueTextBox.Name = "WaterValueTextBox";
            this.WaterValueTextBox.Size = new System.Drawing.Size(107, 22);
            this.WaterValueTextBox.TabIndex = 7;
            this.WaterValueTextBox.Text = "water_value";
            this.WaterValueTextBox.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // WeeklyCTComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "WeeklyCTComponentControl";
            this.Size = new System.Drawing.Size(313, 478);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label TitleLabel;
        private System.Windows.Forms.Label HCRLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label ContrastScaleLabel;
        private System.Windows.Forms.Label ContrastScaleTextBox;
        private System.Windows.Forms.Label WaterValueLabel;
        private System.Windows.Forms.Label WaterValueTextBox;
        private System.Windows.Forms.Label HighContrastTextBox;
    }
}
