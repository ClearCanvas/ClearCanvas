namespace ClearCanvas.ImageViewer.Configuration.View.WinForms {
	partial class MouseImageViewerToolPropertyComponentControl {
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
			System.Windows.Forms.GroupBox groupBox1;
			System.Windows.Forms.GroupBox groupBox3;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MouseImageViewerToolPropertyComponentControl));
			System.Windows.Forms.GroupBox groupBox2;
			this._cboGlobalMouseButtons = new System.Windows.Forms.ComboBox();
			this._chkGlobalModifiers = new ClearCanvas.Desktop.View.WinForms.KeyModifiersPicker();
			this._chkInitiallySelected = new System.Windows.Forms.CheckBox();
			this._cboActiveMouseButtons = new System.Windows.Forms.ComboBox();
			this._errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			groupBox1 = new System.Windows.Forms.GroupBox();
			groupBox3 = new System.Windows.Forms.GroupBox();
			groupBox2 = new System.Windows.Forms.GroupBox();
			groupBox1.SuspendLayout();
			groupBox3.SuspendLayout();
			groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			groupBox1.Controls.Add(groupBox3);
			groupBox1.Controls.Add(groupBox2);
			resources.ApplyResources(groupBox1, "groupBox1");
			groupBox1.Name = "groupBox1";
			groupBox1.TabStop = false;
			// 
			// groupBox3
			// 
			resources.ApplyResources(groupBox3, "groupBox3");
			groupBox3.Controls.Add(this._cboGlobalMouseButtons);
			groupBox3.Controls.Add(this._chkGlobalModifiers);
			groupBox3.Name = "groupBox3";
			groupBox3.TabStop = false;
			// 
			// _cboGlobalMouseButtons
			// 
			resources.ApplyResources(this._cboGlobalMouseButtons, "_cboGlobalMouseButtons");
			this._cboGlobalMouseButtons.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._cboGlobalMouseButtons.FormattingEnabled = true;
			this._cboGlobalMouseButtons.Name = "_cboGlobalMouseButtons";
			this._cboGlobalMouseButtons.SelectedIndexChanged += new System.EventHandler(this.OnCboGlobalMouseButtonsSelectedIndexChanged);
			// 
			// _chkGlobalModifiers
			// 
			resources.ApplyResources(this._chkGlobalModifiers, "_chkGlobalModifiers");
			this._chkGlobalModifiers.Name = "_chkGlobalModifiers";
			// 
			// groupBox2
			// 
			resources.ApplyResources(groupBox2, "groupBox2");
			groupBox2.Controls.Add(this._chkInitiallySelected);
			groupBox2.Controls.Add(this._cboActiveMouseButtons);
			groupBox2.Name = "groupBox2";
			groupBox2.TabStop = false;
			// 
			// _chkInitiallySelected
			// 
			resources.ApplyResources(this._chkInitiallySelected, "_chkInitiallySelected");
			this._chkInitiallySelected.Name = "_chkInitiallySelected";
			this._chkInitiallySelected.UseVisualStyleBackColor = true;
			// 
			// _cboActiveMouseButtons
			// 
			resources.ApplyResources(this._cboActiveMouseButtons, "_cboActiveMouseButtons");
			this._cboActiveMouseButtons.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._cboActiveMouseButtons.FormattingEnabled = true;
			this._cboActiveMouseButtons.Name = "_cboActiveMouseButtons";
			// 
			// _errorProvider
			// 
			this._errorProvider.ContainerControl = this;
			// 
			// MouseImageViewerToolPropertyComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(groupBox1);
			this.Name = "MouseImageViewerToolPropertyComponentControl";
			groupBox1.ResumeLayout(false);
			groupBox3.ResumeLayout(false);
			groupBox2.ResumeLayout(false);
			groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ComboBox _cboActiveMouseButtons;
		private System.Windows.Forms.ComboBox _cboGlobalMouseButtons;
		private ClearCanvas.Desktop.View.WinForms.KeyModifiersPicker _chkGlobalModifiers;
		private System.Windows.Forms.ErrorProvider _errorProvider;
		private System.Windows.Forms.CheckBox _chkInitiallySelected;
	}
}
