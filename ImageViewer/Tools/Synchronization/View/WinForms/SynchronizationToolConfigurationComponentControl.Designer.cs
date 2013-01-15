namespace ClearCanvas.ImageViewer.Tools.Synchronization.View.WinForms {
	partial class SynchronizationToolConfigurationComponentControl {
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
			System.Windows.Forms.FlowLayoutPanel _flowSynchroTools;
			System.Windows.Forms.Label _lblToleranceAngle;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SynchronizationToolConfigurationComponentControl));
			System.Windows.Forms.Label _lblToleranceUnits;
			this._pnlToleranceAngleControl = new System.Windows.Forms.Panel();
			this._txtToleranceAngle = new System.Windows.Forms.TextBox();
			this._errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			_flowSynchroTools = new System.Windows.Forms.FlowLayoutPanel();
			_lblToleranceAngle = new System.Windows.Forms.Label();
			_lblToleranceUnits = new System.Windows.Forms.Label();
			_flowSynchroTools.SuspendLayout();
			this._pnlToleranceAngleControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
			this.SuspendLayout();
			// 
			// _flowSynchroTools
			// 
			_flowSynchroTools.Controls.Add(_lblToleranceAngle);
			_flowSynchroTools.Controls.Add(this._pnlToleranceAngleControl);
			resources.ApplyResources(_flowSynchroTools, "_flowSynchroTools");
			_flowSynchroTools.Name = "_flowSynchroTools";
			// 
			// _lblToleranceAngle
			// 
			resources.ApplyResources(_lblToleranceAngle, "_lblToleranceAngle");
			_lblToleranceAngle.Name = "_lblToleranceAngle";
			// 
			// _pnlToleranceAngleControl
			// 
			this._pnlToleranceAngleControl.Controls.Add(this._txtToleranceAngle);
			this._pnlToleranceAngleControl.Controls.Add(_lblToleranceUnits);
			resources.ApplyResources(this._pnlToleranceAngleControl, "_pnlToleranceAngleControl");
			this._pnlToleranceAngleControl.Name = "_pnlToleranceAngleControl";
			// 
			// _txtToleranceAngle
			// 
			resources.ApplyResources(this._txtToleranceAngle, "_txtToleranceAngle");
			this._txtToleranceAngle.Name = "_txtToleranceAngle";
			// 
			// _lblToleranceUnits
			// 
			resources.ApplyResources(_lblToleranceUnits, "_lblToleranceUnits");
			_lblToleranceUnits.Name = "_lblToleranceUnits";
			// 
			// _errorProvider
			// 
			this._errorProvider.ContainerControl = this;
			// 
			// SynchronizationToolConfigurationComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(_flowSynchroTools);
			this.Name = "SynchronizationToolConfigurationComponentControl";
			_flowSynchroTools.ResumeLayout(false);
			_flowSynchroTools.PerformLayout();
			this._pnlToleranceAngleControl.ResumeLayout(false);
			this._pnlToleranceAngleControl.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TextBox _txtToleranceAngle;
		private System.Windows.Forms.ErrorProvider _errorProvider;
		private System.Windows.Forms.Panel _pnlToleranceAngleControl;
	}
}
