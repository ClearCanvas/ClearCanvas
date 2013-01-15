namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms.ToolStripFilterItems {
	partial class CompareFilterMenuActionControl {
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
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.Label spacer;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CompareFilterMenuActionControl));
			System.Windows.Forms.Panel valigner;
			this._txtValue = new System.Windows.Forms.TextBox();
			this._modeToggle = new System.Windows.Forms.Button();
			this._errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this._tooltipProvider = new System.Windows.Forms.ToolTip(this.components);
			spacer = new System.Windows.Forms.Label();
			valigner = new System.Windows.Forms.Panel();
			valigner.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
			this.SuspendLayout();
			// 
			// spacer
			// 
			resources.ApplyResources(spacer, "spacer");
			spacer.Name = "spacer";
			// 
			// valigner
			// 
			valigner.Controls.Add(this._txtValue);
			resources.ApplyResources(valigner, "valigner");
			valigner.Name = "valigner";
			// 
			// _txtValue
			// 
			resources.ApplyResources(this._txtValue, "_txtValue");
			this._txtValue.Name = "_txtValue";
			this._txtValue.TextChanged += new System.EventHandler(this._txtValue_TextChanged);
			// 
			// _modeToggle
			// 
			resources.ApplyResources(this._modeToggle, "_modeToggle");
			this._modeToggle.Name = "_modeToggle";
			this._modeToggle.UseVisualStyleBackColor = true;
			this._modeToggle.Click += new System.EventHandler(this.modeToggle_Click);
			// 
			// _errorProvider
			// 
			this._errorProvider.ContainerControl = this;
			// 
			// CompareFilterMenuActionControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(valigner);
			this.Controls.Add(spacer);
			this.Controls.Add(this._modeToggle);
			this.Name = "CompareFilterMenuActionControl";
			valigner.ResumeLayout(false);
			valigner.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button _modeToggle;
		private System.Windows.Forms.TextBox _txtValue;
		private System.Windows.Forms.ErrorProvider _errorProvider;
		private System.Windows.Forms.ToolTip _tooltipProvider;
	}
}
