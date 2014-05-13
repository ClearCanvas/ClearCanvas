namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
    partial class PerformingDocumentationOrderDetailsComponentControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PerformingDocumentationOrderDetailsComponentControl));
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this._protocolsPanel = new System.Windows.Forms.Panel();
			this._orderNotesGroupBox = new System.Windows.Forms.GroupBox();
			this._rightHandPanel = new System.Windows.Forms.Panel();
			this._borderPanel = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this._borderPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			resources.ApplyResources(this.splitContainer1, "splitContainer1");
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
			resources.ApplyResources(this.splitContainer1.Panel1, "splitContainer1.Panel1");
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this._rightHandPanel);
			// 
			// splitContainer2
			// 
			resources.ApplyResources(this.splitContainer2, "splitContainer2");
			this.splitContainer2.Name = "splitContainer2";
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this._protocolsPanel);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this._orderNotesGroupBox);
			// 
			// _protocolsPanel
			// 
			resources.ApplyResources(this._protocolsPanel, "_protocolsPanel");
			this._protocolsPanel.Name = "_protocolsPanel";
			// 
			// _orderNotesGroupBox
			// 
			resources.ApplyResources(this._orderNotesGroupBox, "_orderNotesGroupBox");
			this._orderNotesGroupBox.Name = "_orderNotesGroupBox";
			this._orderNotesGroupBox.TabStop = false;
			// 
			// _rightHandPanel
			// 
			resources.ApplyResources(this._rightHandPanel, "_rightHandPanel");
			this._rightHandPanel.Name = "_rightHandPanel";
			// 
			// _borderPanel
			// 
			this._borderPanel.Controls.Add(this.splitContainer1);
			resources.ApplyResources(this._borderPanel, "_borderPanel");
			this._borderPanel.Name = "_borderPanel";
			// 
			// PerformingDocumentationOrderDetailsComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._borderPanel);
			this.Name = "PerformingDocumentationOrderDetailsComponentControl";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this._borderPanel.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.Panel _protocolsPanel;
        private System.Windows.Forms.Panel _rightHandPanel;
		private System.Windows.Forms.GroupBox _orderNotesGroupBox;
		private System.Windows.Forms.Panel _borderPanel;

    }
}
