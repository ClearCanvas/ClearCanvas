namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class ProcedureEditorComponentControl
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
			this._okButton = new System.Windows.Forms.Button();
			this._cancelButton = new System.Windows.Forms.Button();
			this._scheduledTime = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._scheduledDate = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._performingFacility = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._laterality = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._portable = new System.Windows.Forms.CheckBox();
			this._checkedIn = new System.Windows.Forms.CheckBox();
			this._performingDepartment = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._schedulingCode = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._modality = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._duration = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this._procedureType = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
			((System.ComponentModel.ISupportInitialize)(this._duration)).BeginInit();
			this.SuspendLayout();
			// 
			// _okButton
			// 
			this._okButton.Location = new System.Drawing.Point(350, 425);
			this._okButton.Margin = new System.Windows.Forms.Padding(2);
			this._okButton.Name = "_okButton";
			this._okButton.Size = new System.Drawing.Size(75, 23);
			this._okButton.TabIndex = 12;
			this._okButton.Text = "OK";
			this._okButton.UseVisualStyleBackColor = true;
			this._okButton.Click += new System.EventHandler(this._okButton_Click);
			// 
			// _cancelButton
			// 
			this._cancelButton.Location = new System.Drawing.Point(425, 425);
			this._cancelButton.Margin = new System.Windows.Forms.Padding(2);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 13;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _scheduledTime
			// 
			this._scheduledTime.LabelText = "Scheduled Time";
			this._scheduledTime.Location = new System.Drawing.Point(277, 84);
			this._scheduledTime.Margin = new System.Windows.Forms.Padding(2);
			this._scheduledTime.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._scheduledTime.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._scheduledTime.Name = "_scheduledTime";
			this._scheduledTime.Nullable = true;
			this._scheduledTime.ShowDate = false;
			this._scheduledTime.ShowTime = true;
			this._scheduledTime.Size = new System.Drawing.Size(223, 41);
			this._scheduledTime.TabIndex = 2;
			this._scheduledTime.Value = null;
			// 
			// _scheduledDate
			// 
			this._scheduledDate.LabelText = "Scheduled Date";
			this._scheduledDate.Location = new System.Drawing.Point(15, 84);
			this._scheduledDate.Margin = new System.Windows.Forms.Padding(2);
			this._scheduledDate.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._scheduledDate.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._scheduledDate.Name = "_scheduledDate";
			this._scheduledDate.Nullable = true;
			this._scheduledDate.Size = new System.Drawing.Size(223, 41);
			this._scheduledDate.TabIndex = 1;
			this._scheduledDate.Value = null;
			// 
			// _performingFacility
			// 
			this._performingFacility.DataSource = null;
			this._performingFacility.DisplayMember = "";
			this._performingFacility.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._performingFacility.LabelText = "Performing Facility";
			this._performingFacility.Location = new System.Drawing.Point(15, 204);
			this._performingFacility.Margin = new System.Windows.Forms.Padding(2);
			this._performingFacility.Name = "_performingFacility";
			this._performingFacility.Size = new System.Drawing.Size(485, 41);
			this._performingFacility.TabIndex = 6;
			this._performingFacility.Value = null;
			// 
			// _laterality
			// 
			this._laterality.DataSource = null;
			this._laterality.DisplayMember = "";
			this._laterality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._laterality.LabelText = "Laterality";
			this._laterality.Location = new System.Drawing.Point(277, 309);
			this._laterality.Margin = new System.Windows.Forms.Padding(2);
			this._laterality.Name = "_laterality";
			this._laterality.Size = new System.Drawing.Size(223, 41);
			this._laterality.TabIndex = 10;
			this._laterality.Value = null;
			// 
			// _portable
			// 
			this._portable.AutoSize = true;
			this._portable.Location = new System.Drawing.Point(22, 321);
			this._portable.Margin = new System.Windows.Forms.Padding(2);
			this._portable.Name = "_portable";
			this._portable.Size = new System.Drawing.Size(65, 17);
			this._portable.TabIndex = 8;
			this._portable.Text = "Portable";
			this._portable.UseVisualStyleBackColor = true;
			// 
			// _checkedIn
			// 
			this._checkedIn.AutoSize = true;
			this._checkedIn.Location = new System.Drawing.Point(22, 367);
			this._checkedIn.Margin = new System.Windows.Forms.Padding(2);
			this._checkedIn.Name = "_checkedIn";
			this._checkedIn.Size = new System.Drawing.Size(125, 17);
			this._checkedIn.TabIndex = 9;
			this._checkedIn.Text = "Patient is checked-in";
			this._checkedIn.UseVisualStyleBackColor = true;
			// 
			// _performingDepartment
			// 
			this._performingDepartment.DataSource = null;
			this._performingDepartment.DisplayMember = "";
			this._performingDepartment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._performingDepartment.LabelText = "Performing Department";
			this._performingDepartment.Location = new System.Drawing.Point(15, 259);
			this._performingDepartment.Margin = new System.Windows.Forms.Padding(2);
			this._performingDepartment.Name = "_performingDepartment";
			this._performingDepartment.Size = new System.Drawing.Size(485, 41);
			this._performingDepartment.TabIndex = 7;
			this._performingDepartment.Value = null;
			// 
			// _schedulingCode
			// 
			this._schedulingCode.DataSource = null;
			this._schedulingCode.DisplayMember = "";
			this._schedulingCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._schedulingCode.LabelText = "Scheduling Code";
			this._schedulingCode.Location = new System.Drawing.Point(277, 355);
			this._schedulingCode.Margin = new System.Windows.Forms.Padding(2);
			this._schedulingCode.Name = "_schedulingCode";
			this._schedulingCode.Size = new System.Drawing.Size(223, 41);
			this._schedulingCode.TabIndex = 11;
			this._schedulingCode.Value = null;
			// 
			// _modality
			// 
			this._modality.DataSource = null;
			this._modality.DisplayMember = "";
			this._modality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._modality.LabelText = "Modality";
			this._modality.Location = new System.Drawing.Point(277, 150);
			this._modality.Margin = new System.Windows.Forms.Padding(2);
			this._modality.Name = "_modality";
			this._modality.Size = new System.Drawing.Size(223, 41);
			this._modality.TabIndex = 5;
			this._modality.Value = null;
			// 
			// _duration
			// 
			this._duration.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this._duration.Location = new System.Drawing.Point(21, 165);
			this._duration.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this._duration.Name = "_duration";
			this._duration.Size = new System.Drawing.Size(55, 20);
			this._duration.TabIndex = 4;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(16, 148);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(126, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Scheduled Duration (min)";
			// 
			// _procedureType
			// 
			this._procedureType.AutoSize = true;
			this._procedureType.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._procedureType.LabelText = "Procedure Type";
			this._procedureType.Location = new System.Drawing.Point(15, 19);
			this._procedureType.Margin = new System.Windows.Forms.Padding(2);
			this._procedureType.Name = "_procedureType";
			this._procedureType.Size = new System.Drawing.Size(485, 43);
			this._procedureType.TabIndex = 0;
			this._procedureType.Value = null;
			// 
			// ProcedureEditorComponentControl
			// 
			this.AcceptButton = this._okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this._procedureType);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._duration);
			this.Controls.Add(this._modality);
			this.Controls.Add(this._schedulingCode);
			this.Controls.Add(this._performingDepartment);
			this.Controls.Add(this._checkedIn);
			this.Controls.Add(this._portable);
			this.Controls.Add(this._laterality);
			this.Controls.Add(this._performingFacility);
			this.Controls.Add(this._scheduledTime);
			this.Controls.Add(this._scheduledDate);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._okButton);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "ProcedureEditorComponentControl";
			this.Size = new System.Drawing.Size(517, 467);
			((System.ComponentModel.ISupportInitialize)(this._duration)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Button _cancelButton;
        private ClearCanvas.Desktop.View.WinForms.DateTimeField _scheduledTime;
		private ClearCanvas.Desktop.View.WinForms.DateTimeField _scheduledDate;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _performingFacility;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _laterality;
        private System.Windows.Forms.CheckBox _portable;
		private System.Windows.Forms.CheckBox _checkedIn;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _performingDepartment;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _schedulingCode;
		private Desktop.View.WinForms.ComboBoxField _modality;
		private System.Windows.Forms.NumericUpDown _duration;
		private System.Windows.Forms.Label label1;
		private LookupField _procedureType;
    }
}
