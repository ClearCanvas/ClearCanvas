namespace ClearCanvas.ImageViewer.Tools.MedicalPhysics.View.WinForms
{
    partial class NMDailyQCControl
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
            this.D2Sensitivity = new ClearCanvas.Desktop.View.WinForms.TextAreaField();
            this.D2BackgroundField = new ClearCanvas.Desktop.View.WinForms.TextAreaField();
            this.D2Label = new System.Windows.Forms.Label();
            this.D1Label = new System.Windows.Forms.Label();
            this.D1BackgroundField = new ClearCanvas.Desktop.View.WinForms.TextAreaField();
            this.D1Sensitivity = new ClearCanvas.Desktop.View.WinForms.TextAreaField();
            this.FloodSourceGroupBox = new System.Windows.Forms.GroupBox();
            this.SourceCurrentField = new ClearCanvas.Desktop.View.WinForms.TextAreaField();
            this.AcquisitionDateField = new ClearCanvas.Desktop.View.WinForms.TextAreaField();
            this.SourceCalibrationActivityField = new ClearCanvas.Desktop.View.WinForms.TextAreaField();
            this.SourceCalibrationDateField = new ClearCanvas.Desktop.View.WinForms.TextAreaField();
            this.tableLayoutPanel1.SuspendLayout();
            this.FloodSourceGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.D2Sensitivity, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.D2BackgroundField, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.D2Label, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.D1Label, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.D1BackgroundField, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.D1Sensitivity, 0, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(5, 124);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(357, 156);
            this.tableLayoutPanel1.TabIndex = 0;
            this.tableLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
            // 
            // D2Sensitivity
            // 
            this.D2Sensitivity.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.D2Sensitivity.LabelText = "Sensitivity";
            this.D2Sensitivity.Location = new System.Drawing.Point(180, 107);
            this.D2Sensitivity.Margin = new System.Windows.Forms.Padding(2);
            this.D2Sensitivity.Name = "D2Sensitivity";
            this.D2Sensitivity.Size = new System.Drawing.Size(161, 41);
            this.D2Sensitivity.TabIndex = 5;
            this.D2Sensitivity.Value = null;
            // 
            // D2BackgroundField
            // 
            this.D2BackgroundField.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.D2BackgroundField.LabelText = "Background Count Rate";
            this.D2BackgroundField.Location = new System.Drawing.Point(180, 47);
            this.D2BackgroundField.Margin = new System.Windows.Forms.Padding(2);
            this.D2BackgroundField.Name = "D2BackgroundField";
            this.D2BackgroundField.Size = new System.Drawing.Size(161, 41);
            this.D2BackgroundField.TabIndex = 3;
            this.D2BackgroundField.Value = null;
            // 
            // D2Label
            // 
            this.D2Label.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.D2Label.AutoSize = true;
            this.D2Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.D2Label.Location = new System.Drawing.Point(178, 0);
            this.D2Label.Margin = new System.Windows.Forms.Padding(0);
            this.D2Label.Name = "D2Label";
            this.D2Label.Size = new System.Drawing.Size(179, 45);
            this.D2Label.TabIndex = 1;
            this.D2Label.Text = "Detector 2";
            this.D2Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // D1Label
            // 
            this.D1Label.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.D1Label.AutoSize = true;
            this.D1Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.D1Label.Location = new System.Drawing.Point(0, 0);
            this.D1Label.Margin = new System.Windows.Forms.Padding(0);
            this.D1Label.Name = "D1Label";
            this.D1Label.Size = new System.Drawing.Size(178, 45);
            this.D1Label.TabIndex = 0;
            this.D1Label.Text = "Detector 1";
            this.D1Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // D1BackgroundField
            // 
            this.D1BackgroundField.BackColor = System.Drawing.SystemColors.Control;
            this.D1BackgroundField.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.D1BackgroundField.LabelText = "Background Count Rate";
            this.D1BackgroundField.Location = new System.Drawing.Point(2, 47);
            this.D1BackgroundField.Margin = new System.Windows.Forms.Padding(2);
            this.D1BackgroundField.Name = "D1BackgroundField";
            this.D1BackgroundField.Size = new System.Drawing.Size(174, 41);
            this.D1BackgroundField.TabIndex = 2;
            this.D1BackgroundField.Value = null;
            // 
            // D1Sensitivity
            // 
            this.D1Sensitivity.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.D1Sensitivity.LabelText = "Sensitivity";
            this.D1Sensitivity.Location = new System.Drawing.Point(2, 107);
            this.D1Sensitivity.Margin = new System.Windows.Forms.Padding(2);
            this.D1Sensitivity.Name = "D1Sensitivity";
            this.D1Sensitivity.Size = new System.Drawing.Size(174, 41);
            this.D1Sensitivity.TabIndex = 4;
            this.D1Sensitivity.Value = null;
            // 
            // FloodSourceGroupBox
            // 
            this.FloodSourceGroupBox.Controls.Add(this.SourceCurrentField);
            this.FloodSourceGroupBox.Controls.Add(this.AcquisitionDateField);
            this.FloodSourceGroupBox.Controls.Add(this.SourceCalibrationActivityField);
            this.FloodSourceGroupBox.Controls.Add(this.SourceCalibrationDateField);
            this.FloodSourceGroupBox.Location = new System.Drawing.Point(6, 4);
            this.FloodSourceGroupBox.Name = "FloodSourceGroupBox";
            this.FloodSourceGroupBox.Size = new System.Drawing.Size(354, 114);
            this.FloodSourceGroupBox.TabIndex = 1;
            this.FloodSourceGroupBox.TabStop = false;
            this.FloodSourceGroupBox.Text = "Flood Source";
            this.FloodSourceGroupBox.Enter += new System.EventHandler(this.groupBox1_Enter_1);
            // 
            // SourceCurrentField
            // 
            this.SourceCurrentField.LabelText = "Activity at Acquisition";
            this.SourceCurrentField.Location = new System.Drawing.Point(179, 60);
            this.SourceCurrentField.Margin = new System.Windows.Forms.Padding(2);
            this.SourceCurrentField.Name = "SourceCurrentField";
            this.SourceCurrentField.ReadOnly = true;
            this.SourceCurrentField.Size = new System.Drawing.Size(170, 38);
            this.SourceCurrentField.TabIndex = 3;
            this.SourceCurrentField.Value = null;
            // 
            // AcquisitionDateField
            // 
            this.AcquisitionDateField.LabelText = "Acquisition Date";
            this.AcquisitionDateField.Location = new System.Drawing.Point(5, 60);
            this.AcquisitionDateField.Margin = new System.Windows.Forms.Padding(2);
            this.AcquisitionDateField.Name = "AcquisitionDateField";
            this.AcquisitionDateField.ReadOnly = true;
            this.AcquisitionDateField.Size = new System.Drawing.Size(170, 38);
            this.AcquisitionDateField.TabIndex = 2;
            this.AcquisitionDateField.Value = null;
            // 
            // SourceCalibrationActivityField
            // 
            this.SourceCalibrationActivityField.LabelText = "Calibration Activity";
            this.SourceCalibrationActivityField.Location = new System.Drawing.Point(178, 18);
            this.SourceCalibrationActivityField.Margin = new System.Windows.Forms.Padding(2);
            this.SourceCalibrationActivityField.Name = "SourceCalibrationActivityField";
            this.SourceCalibrationActivityField.ReadOnly = true;
            this.SourceCalibrationActivityField.Size = new System.Drawing.Size(170, 38);
            this.SourceCalibrationActivityField.TabIndex = 1;
            this.SourceCalibrationActivityField.Value = null;
            // 
            // SourceCalibrationDateField
            // 
            this.SourceCalibrationDateField.LabelText = "Calibration Date";
            this.SourceCalibrationDateField.Location = new System.Drawing.Point(5, 18);
            this.SourceCalibrationDateField.Margin = new System.Windows.Forms.Padding(2);
            this.SourceCalibrationDateField.Name = "SourceCalibrationDateField";
            this.SourceCalibrationDateField.ReadOnly = true;
            this.SourceCalibrationDateField.Size = new System.Drawing.Size(170, 38);
            this.SourceCalibrationDateField.TabIndex = 0;
            this.SourceCalibrationDateField.Value = null;
            // 
            // NMDailyQCControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.FloodSourceGroupBox);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "NMDailyQCControl";
            this.Size = new System.Drawing.Size(365, 313);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.FloodSourceGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label D1Label;
        private System.Windows.Forms.Label D2Label;
        private Desktop.View.WinForms.TextAreaField D2BackgroundField;
        private Desktop.View.WinForms.TextAreaField D1BackgroundField;
        private Desktop.View.WinForms.TextAreaField D2Sensitivity;
        private Desktop.View.WinForms.TextAreaField D1Sensitivity;
        private System.Windows.Forms.GroupBox FloodSourceGroupBox;
        private Desktop.View.WinForms.TextAreaField SourceCurrentField;
        private Desktop.View.WinForms.TextAreaField AcquisitionDateField;
        private Desktop.View.WinForms.TextAreaField SourceCalibrationActivityField;
        private Desktop.View.WinForms.TextAreaField SourceCalibrationDateField;
    }
}
