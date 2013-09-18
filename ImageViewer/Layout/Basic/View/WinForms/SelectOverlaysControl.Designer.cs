namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
    partial class SelectOverlaysControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectOverlaysControl));
			this._mainTable = new System.Windows.Forms.TableLayoutPanel();
			this.label1 = new System.Windows.Forms.Label();
			this._applyToAll = new System.Windows.Forms.Button();
			this._close = new System.Windows.Forms.Button();
			this._overlaysPanel = new System.Windows.Forms.FlowLayoutPanel();
			this._mainTable.SuspendLayout();
			this.SuspendLayout();
			// 
			// _mainTable
			// 
			resources.ApplyResources(this._mainTable, "_mainTable");
			this._mainTable.Controls.Add(this.label1, 0, 0);
			this._mainTable.Controls.Add(this._applyToAll, 1, 2);
			this._mainTable.Controls.Add(this._close, 0, 2);
			this._mainTable.Controls.Add(this._overlaysPanel, 0, 1);
			this._mainTable.Name = "_mainTable";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this._mainTable.SetColumnSpan(this.label1, 2);
			this.label1.Name = "label1";
			// 
			// _applyToAll
			// 
			resources.ApplyResources(this._applyToAll, "_applyToAll");
			this._applyToAll.Name = "_applyToAll";
			this._applyToAll.UseVisualStyleBackColor = true;
			// 
			// _close
			// 
			resources.ApplyResources(this._close, "_close");
			this._close.Name = "_close";
			this._close.UseVisualStyleBackColor = true;
			// 
			// _overlaysPanel
			// 
			resources.ApplyResources(this._overlaysPanel, "_overlaysPanel");
			this._overlaysPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._mainTable.SetColumnSpan(this._overlaysPanel, 2);
			this._overlaysPanel.MaximumSize = new System.Drawing.Size(142, 200);
			this._overlaysPanel.MinimumSize = new System.Drawing.Size(142, 20);
			this._overlaysPanel.Name = "_overlaysPanel";
			// 
			// SelectOverlaysControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._mainTable);
			this.Name = "SelectOverlaysControl";
			this._mainTable.ResumeLayout(false);
			this._mainTable.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel _mainTable;
        private System.Windows.Forms.Button _close;
        private System.Windows.Forms.Button _applyToAll;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel _overlaysPanel;
    }
}
