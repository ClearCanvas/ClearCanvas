namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms {
	partial class ExportComponentPanel {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportComponentPanel));
			this._studyDescription = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._patientId = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._accessionNumber = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._patientsName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._dateOfBirth = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._studyDate = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._cancelButton = new System.Windows.Forms.Button();
			this._okButton = new System.Windows.Forms.Button();
			this._studyId = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._browse = new System.Windows.Forms.Button();
			this._outputPath = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.label2 = new System.Windows.Forms.Label();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _studyDescription
			// 
			resources.ApplyResources(this._studyDescription, "_studyDescription");
			this._studyDescription.Name = "_studyDescription";
			this._studyDescription.Value = null;
			// 
			// _patientId
			// 
			resources.ApplyResources(this._patientId, "_patientId");
			this._patientId.Name = "_patientId";
			this._patientId.Value = null;
			// 
			// _accessionNumber
			// 
			resources.ApplyResources(this._accessionNumber, "_accessionNumber");
			this._accessionNumber.Name = "_accessionNumber";
			this._accessionNumber.Value = null;
			// 
			// _patientsName
			// 
			resources.ApplyResources(this._patientsName, "_patientsName");
			this._patientsName.Name = "_patientsName";
			this._patientsName.Value = null;
			// 
			// _dateOfBirth
			// 
			resources.ApplyResources(this._dateOfBirth, "_dateOfBirth");
			this._dateOfBirth.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._dateOfBirth.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._dateOfBirth.Name = "_dateOfBirth";
			this._dateOfBirth.Nullable = true;
			this._dateOfBirth.Value = new System.DateTime(2008, 4, 21, 9, 11, 13, 140);
			// 
			// _studyDate
			// 
			resources.ApplyResources(this._studyDate, "_studyDate");
			this._studyDate.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._studyDate.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._studyDate.Name = "_studyDate";
			this._studyDate.Nullable = true;
			this._studyDate.Value = new System.DateTime(2008, 4, 21, 9, 14, 8, 984);
			// 
			// _cancelButton
			// 
			resources.ApplyResources(this._cancelButton, "_cancelButton");
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _okButton
			// 
			resources.ApplyResources(this._okButton, "_okButton");
			this._okButton.Name = "_okButton";
			this._okButton.UseVisualStyleBackColor = true;
			this._okButton.Click += new System.EventHandler(this._okButton_Click);
			// 
			// _studyId
			// 
			resources.ApplyResources(this._studyId, "_studyId");
			this._studyId.Name = "_studyId";
			this._studyId.Value = null;
			// 
			// _browse
			// 
			resources.ApplyResources(this._browse, "_browse");
			this._browse.Name = "_browse";
			this._browse.UseVisualStyleBackColor = true;
			this._browse.Click += new System.EventHandler(this._browse_Click);
			// 
			// _outputPath
			// 
			resources.ApplyResources(this._outputPath, "_outputPath");
			this._outputPath.Name = "_outputPath";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.panel2);
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Name = "panel1";
			// 
			// panel2
			// 
			this.panel2.BackgroundImage = global::ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms.Properties.Resources.WarningHS;
			resources.ApplyResources(this.panel2, "panel2");
			this.panel2.Name = "panel2";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// ExportComponentPanel
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel1);
			this.Controls.Add(this._outputPath);
			this.Controls.Add(this._browse);
			this.Controls.Add(this._studyId);
			this.Controls.Add(this._studyDescription);
			this.Controls.Add(this._patientId);
			this.Controls.Add(this._accessionNumber);
			this.Controls.Add(this._patientsName);
			this.Controls.Add(this._dateOfBirth);
			this.Controls.Add(this._studyDate);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._okButton);
			this.Controls.Add(this.label1);
			this.Name = "ExportComponentPanel";
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private ClearCanvas.Desktop.View.WinForms.TextField _studyDescription;
		private ClearCanvas.Desktop.View.WinForms.TextField _patientId;
		private ClearCanvas.Desktop.View.WinForms.TextField _accessionNumber;
		private ClearCanvas.Desktop.View.WinForms.TextField _patientsName;
		private ClearCanvas.Desktop.View.WinForms.DateTimeField _dateOfBirth;
		private ClearCanvas.Desktop.View.WinForms.DateTimeField _studyDate;
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.Button _okButton;
		private ClearCanvas.Desktop.View.WinForms.TextField _studyId;
		private System.Windows.Forms.Button _browse;
		private System.Windows.Forms.TextBox _outputPath;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label label2;
	}
}
