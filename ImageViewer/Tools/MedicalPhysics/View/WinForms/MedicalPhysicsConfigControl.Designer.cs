namespace ClearCanvas.ImageViewer.Tools.MedicalPhysics.View.WinForms
{
    partial class MedicalPhysicsConfigControl
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
            this.FloodCalibrationDateField = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
            this.FloodSourceActivityField = new ClearCanvas.Desktop.View.WinForms.TextField();
            this.FloodSourceSerialNumberField = new ClearCanvas.Desktop.View.WinForms.TextAreaField();
            this.IsotopesField = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
            this.SuspendLayout();
            // 
            // FloodCalibrationDateField
            // 
            this.FloodCalibrationDateField.LabelText = "Flood Source Calibration Date";
            this.FloodCalibrationDateField.Location = new System.Drawing.Point(-1, 14);
            this.FloodCalibrationDateField.Margin = new System.Windows.Forms.Padding(2);
            this.FloodCalibrationDateField.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this.FloodCalibrationDateField.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this.FloodCalibrationDateField.Name = "FloodCalibrationDateField";
            this.FloodCalibrationDateField.Size = new System.Drawing.Size(169, 41);
            this.FloodCalibrationDateField.TabIndex = 0;
            this.FloodCalibrationDateField.Value = new System.DateTime(2023, 11, 27, 13, 38, 11, 835);
            // 
            // FloodSourceActivityField
            // 
            this.FloodSourceActivityField.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FloodSourceActivityField.LabelText = "Flood Source Calibration Activity";
            this.FloodSourceActivityField.Location = new System.Drawing.Point(-1, 66);
            this.FloodSourceActivityField.Margin = new System.Windows.Forms.Padding(2);
            this.FloodSourceActivityField.Mask = "";
            this.FloodSourceActivityField.Name = "FloodSourceActivityField";
            this.FloodSourceActivityField.PasswordChar = '\0';
            this.FloodSourceActivityField.Size = new System.Drawing.Size(169, 64);
            this.FloodSourceActivityField.TabIndex = 2;
            this.FloodSourceActivityField.ToolTip = null;
            this.FloodSourceActivityField.Value = null;
            // 
            // FloodSourceSerialNumberField
            // 
            this.FloodSourceSerialNumberField.LabelText = "Flood Source Serial Number";
            this.FloodSourceSerialNumberField.Location = new System.Drawing.Point(0, 134);
            this.FloodSourceSerialNumberField.Margin = new System.Windows.Forms.Padding(2);
            this.FloodSourceSerialNumberField.Name = "FloodSourceSerialNumberField";
            this.FloodSourceSerialNumberField.Size = new System.Drawing.Size(168, 41);
            this.FloodSourceSerialNumberField.TabIndex = 3;
            this.FloodSourceSerialNumberField.Value = null;
            // 
            // IsotopesField
            // 
            this.IsotopesField.DataSource = null;
            this.IsotopesField.DisplayMember = "FullName";
            this.IsotopesField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.IsotopesField.LabelText = "Isotope";
            this.IsotopesField.Location = new System.Drawing.Point(-1, 186);
            this.IsotopesField.Margin = new System.Windows.Forms.Padding(2);
            this.IsotopesField.Name = "IsotopesField";
            this.IsotopesField.Size = new System.Drawing.Size(169, 41);
            this.IsotopesField.TabIndex = 4;
            this.IsotopesField.Value = null;
            // 
            // MedicalPhysicsConfigControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.IsotopesField);
            this.Controls.Add(this.FloodSourceSerialNumberField);
            this.Controls.Add(this.FloodSourceActivityField);
            this.Controls.Add(this.FloodCalibrationDateField);
            this.Name = "MedicalPhysicsConfigControl";
            this.Size = new System.Drawing.Size(399, 258);
            this.ResumeLayout(false);

        }

        #endregion

        private Desktop.View.WinForms.DateTimeField FloodCalibrationDateField;
        private Desktop.View.WinForms.TextField FloodSourceActivityField;
        private Desktop.View.WinForms.TextAreaField FloodSourceSerialNumberField;
        private Desktop.View.WinForms.ComboBoxField IsotopesField;
    }
}
