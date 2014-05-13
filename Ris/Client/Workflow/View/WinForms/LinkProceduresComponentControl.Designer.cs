namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
    partial class LinkProceduresComponentControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LinkProceduresComponentControl));
			this._worklistItemTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
			this._okButton = new System.Windows.Forms.Button();
			this._instructionsLabel = new System.Windows.Forms.Label();
			this._heading = new System.Windows.Forms.Label();
			this._sourceWorklistItem = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.SuspendLayout();
			// 
			// _worklistItemTableView
			// 
			resources.ApplyResources(this._worklistItemTableView, "_worklistItemTableView");
			this._worklistItemTableView.Name = "_worklistItemTableView";
			this._worklistItemTableView.ReadOnly = false;
			this._worklistItemTableView.ShowToolbar = false;
			this._worklistItemTableView.TabStop = false;
			// 
			// _okButton
			// 
			resources.ApplyResources(this._okButton, "_okButton");
			this._okButton.Name = "_okButton";
			this._okButton.UseVisualStyleBackColor = true;
			this._okButton.Click += new System.EventHandler(this._okButton_Click);
			// 
			// _instructionsLabel
			// 
			resources.ApplyResources(this._instructionsLabel, "_instructionsLabel");
			this._instructionsLabel.Name = "_instructionsLabel";
			// 
			// _heading
			// 
			resources.ApplyResources(this._heading, "_heading");
			this._heading.Name = "_heading";
			// 
			// _sourceWorklistItem
			// 
			resources.ApplyResources(this._sourceWorklistItem, "_sourceWorklistItem");
			this._sourceWorklistItem.Name = "_sourceWorklistItem";
			this._sourceWorklistItem.ReadOnly = false;
			this._sourceWorklistItem.ShowToolbar = false;
			this._sourceWorklistItem.TabStop = false;
			// 
			// LinkProceduresComponentControl
			// 
			this.AcceptButton = this._okButton;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._sourceWorklistItem);
			this.Controls.Add(this._heading);
			this.Controls.Add(this._instructionsLabel);
			this.Controls.Add(this._okButton);
			this.Controls.Add(this._worklistItemTableView);
			this.Name = "LinkProceduresComponentControl";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _worklistItemTableView;
		private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Label _instructionsLabel;
		private System.Windows.Forms.Label _heading;
		private ClearCanvas.Desktop.View.WinForms.TableView _sourceWorklistItem;
    }
}
