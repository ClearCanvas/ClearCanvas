namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class FolderExplorerGroupComponentControl
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FolderExplorerGroupComponentControl));
			this._toolStrip = new System.Windows.Forms.ToolStrip();
			this._searchTextBox = new System.Windows.Forms.ToolStripTextBox();
			this._searchButton = new System.Windows.Forms.ToolStripSplitButton();
			this._advancedSearch = new System.Windows.Forms.ToolStripMenuItem();
			this._groupPanel = new System.Windows.Forms.Panel();
			this._contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this._toolStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// _toolStrip
			// 
			this._toolStrip.GripMargin = new System.Windows.Forms.Padding(0);
			this._toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this._toolStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
			this._toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._searchTextBox,
            this._searchButton});
			resources.ApplyResources(this._toolStrip, "_toolStrip");
			this._toolStrip.Name = "_toolStrip";
			// 
			// _searchTextBox
			// 
			this._searchTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._searchTextBox.Name = "_searchTextBox";
			resources.ApplyResources(this._searchTextBox, "_searchTextBox");
			this._searchTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this._searchTextBox_KeyDown);
			// 
			// _searchButton
			// 
			this._searchButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._searchButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._advancedSearch});
			resources.ApplyResources(this._searchButton, "_searchButton");
			this._searchButton.Name = "_searchButton";
			this._searchButton.ButtonClick += new System.EventHandler(this._searchButton_ButtonClick);
			// 
			// _advancedSearch
			// 
			resources.ApplyResources(this._advancedSearch, "_advancedSearch");
			this._advancedSearch.Name = "_advancedSearch";
			this._advancedSearch.Click += new System.EventHandler(this._advancedSearch_Click);
			// 
			// _groupPanel
			// 
			this._groupPanel.ContextMenuStrip = this._contextMenu;
			resources.ApplyResources(this._groupPanel, "_groupPanel");
			this._groupPanel.Name = "_groupPanel";
			// 
			// _contextMenu
			// 
			this._contextMenu.Name = "_contextMenu";
			resources.ApplyResources(this._contextMenu, "_contextMenu");
			// 
			// FolderExplorerGroupComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._groupPanel);
			this.Controls.Add(this._toolStrip);
			this.Name = "FolderExplorerGroupComponentControl";
			this.Load += new System.EventHandler(this.FolderExplorerGroupComponentControl_Load);
			this._toolStrip.ResumeLayout(false);
			this._toolStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.ToolStrip _toolStrip;
		private System.Windows.Forms.ToolStripTextBox _searchTextBox;
		private System.Windows.Forms.Panel _groupPanel;
		private System.Windows.Forms.ContextMenuStrip _contextMenu;
		private System.Windows.Forms.ToolStripSplitButton _searchButton;
		private System.Windows.Forms.ToolStripMenuItem _advancedSearch;
    }
}
