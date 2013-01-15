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
			this._toolStrip.Location = new System.Drawing.Point(0, 0);
			this._toolStrip.Name = "_toolStrip";
			this._toolStrip.Size = new System.Drawing.Size(252, 31);
			this._toolStrip.TabIndex = 0;
			// 
			// _searchTextBox
			// 
			this._searchTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._searchTextBox.Name = "_searchTextBox";
			this._searchTextBox.Size = new System.Drawing.Size(100, 31);
			this._searchTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this._searchTextBox_KeyDown);
			// 
			// _searchButton
			// 
			this._searchButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._searchButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._advancedSearch});
			this._searchButton.Image = ((System.Drawing.Image)(resources.GetObject("_searchButton.Image")));
			this._searchButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._searchButton.Name = "_searchButton";
			this._searchButton.Size = new System.Drawing.Size(40, 28);
			this._searchButton.Text = "Search";
			this._searchButton.ButtonClick += new System.EventHandler(this._searchButton_ButtonClick);
			// 
			// _advancedSearch
			// 
			this._advancedSearch.Image = ((System.Drawing.Image)(resources.GetObject("_advancedSearch.Image")));
			this._advancedSearch.Name = "_advancedSearch";
			this._advancedSearch.Size = new System.Drawing.Size(177, 22);
			this._advancedSearch.Text = "Advanced Search ...";
			this._advancedSearch.Click += new System.EventHandler(this._advancedSearch_Click);
			// 
			// _groupPanel
			// 
			this._groupPanel.ContextMenuStrip = this._contextMenu;
			this._groupPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._groupPanel.Location = new System.Drawing.Point(0, 31);
			this._groupPanel.Name = "_groupPanel";
			this._groupPanel.Size = new System.Drawing.Size(252, 281);
			this._groupPanel.TabIndex = 1;
			// 
			// _contextMenu
			// 
			this._contextMenu.Name = "_contextMenu";
			this._contextMenu.Size = new System.Drawing.Size(61, 4);
			// 
			// FolderExplorerGroupComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._groupPanel);
			this.Controls.Add(this._toolStrip);
			this.Name = "FolderExplorerGroupComponentControl";
			this.Size = new System.Drawing.Size(252, 312);
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
