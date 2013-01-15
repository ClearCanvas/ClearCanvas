#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

namespace ClearCanvas.Desktop.View.WinForms
{
	partial class TableView
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer _components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (_components != null))
			{
				UnsubscribeFromOldTable();
				_components.Dispose();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TableView));
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			this._contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this._bindingSource = new System.Windows.Forms.BindingSource(this.components);
			this._toolStrip = new System.Windows.Forms.ToolStrip();
			this._sortButton = new System.Windows.Forms.ToolStripDropDownButton();
			this._sortAscendingButton = new System.Windows.Forms.ToolStripMenuItem();
			this._sortDescendingButton = new System.Windows.Forms.ToolStripMenuItem();
			this._sortSeparator = new System.Windows.Forms.ToolStripSeparator();
			this._filterLabel = new System.Windows.Forms.ToolStripLabel();
			this._filterTextBox = new System.Windows.Forms.ToolStripTextBox();
			this._clearFilterButton = new System.Windows.Forms.ToolStripButton();
			this._statusStrip = new System.Windows.Forms.StatusStrip();
			this._statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this._dataGridView = new ClearCanvas.Desktop.View.WinForms.DataGridViewWithDragSupport();
			((System.ComponentModel.ISupportInitialize)(this._bindingSource)).BeginInit();
			this._toolStrip.SuspendLayout();
			this._statusStrip.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._dataGridView)).BeginInit();
			this.SuspendLayout();
			// 
			// _contextMenu
			// 
			this._contextMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
			this._contextMenu.Name = "_contextMenu";
			resources.ApplyResources(this._contextMenu, "_contextMenu");
			this._contextMenu.Opening += new System.ComponentModel.CancelEventHandler(this._contextMenu_Opening);
			// 
			// _toolStrip
			// 
			this._toolStrip.GripMargin = new System.Windows.Forms.Padding(0);
			this._toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this._toolStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
			this._toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._sortButton,
            this._filterLabel,
            this._filterTextBox,
            this._clearFilterButton});
			resources.ApplyResources(this._toolStrip, "_toolStrip");
			this._toolStrip.Name = "_toolStrip";
			// 
			// _sortButton
			// 
			this._sortButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this._sortButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._sortAscendingButton,
            this._sortDescendingButton,
            this._sortSeparator});
			resources.ApplyResources(this._sortButton, "_sortButton");
			this._sortButton.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
			this._sortButton.Name = "_sortButton";
			this._sortButton.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
			// 
			// _sortAscendingButton
			// 
			this._sortAscendingButton.Name = "_sortAscendingButton";
			resources.ApplyResources(this._sortAscendingButton, "_sortAscendingButton");
			this._sortAscendingButton.Click += new System.EventHandler(this.sortAscendingButton_Click);
			// 
			// _sortDescendingButton
			// 
			this._sortDescendingButton.Name = "_sortDescendingButton";
			resources.ApplyResources(this._sortDescendingButton, "_sortDescendingButton");
			this._sortDescendingButton.Click += new System.EventHandler(this.sortDescendingButton_Click);
			// 
			// _sortSeparator
			// 
			this._sortSeparator.Name = "_sortSeparator";
			resources.ApplyResources(this._sortSeparator, "_sortSeparator");
			// 
			// _filterLabel
			// 
			this._filterLabel.Name = "_filterLabel";
			resources.ApplyResources(this._filterLabel, "_filterLabel");
			// 
			// _filterTextBox
			// 
			this._filterTextBox.Margin = new System.Windows.Forms.Padding(1, 1, 0, 1);
			this._filterTextBox.Name = "_filterTextBox";
			this._filterTextBox.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
			resources.ApplyResources(this._filterTextBox, "_filterTextBox");
			this._filterTextBox.TextChanged += new System.EventHandler(this._filterText_TextChanged);
			// 
			// _clearFilterButton
			// 
			this._clearFilterButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this._clearFilterButton, "_clearFilterButton");
			this._clearFilterButton.Image = global::ClearCanvas.Desktop.View.WinForms.SR.ClearFilterMini;
			this._clearFilterButton.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
			this._clearFilterButton.Name = "_clearFilterButton";
			this._clearFilterButton.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
			this._clearFilterButton.Click += new System.EventHandler(this._clearFilterButton_Click);
			// 
			// _statusStrip
			// 
			this._statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._statusLabel});
			resources.ApplyResources(this._statusStrip, "_statusStrip");
			this._statusStrip.Name = "_statusStrip";
			// 
			// _statusLabel
			// 
			this._statusLabel.Name = "_statusLabel";
			resources.ApplyResources(this._statusLabel, "_statusLabel");
			this._statusLabel.Spring = true;
			// 
			// _dataGridView
			// 
			this._dataGridView.AllowUserToAddRows = false;
			this._dataGridView.AllowUserToDeleteRows = false;
			this._dataGridView.AllowUserToOrderColumns = true;
			this._dataGridView.AllowUserToResizeRows = false;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
			this._dataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
			this._dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this._dataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
			this._dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this._dataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
			this._dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this._dataGridView.ContextMenuStrip = this._contextMenu;
			resources.ApplyResources(this._dataGridView, "_dataGridView");
			this._dataGridView.Name = "_dataGridView";
			this._dataGridView.RowHeadersVisible = false;
			this._dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this._dataGridView.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this._dataGridView_CellBeginEdit);
			this._dataGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this._dataGridView_CellDoubleClick);
			this._dataGridView.CellParsing += new System.Windows.Forms.DataGridViewCellParsingEventHandler(this._dataGridView_CellParsing);
			this._dataGridView.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this._dataGridView_CellFormatting);
			this._dataGridView.CellToolTipTextNeeded += new System.Windows.Forms.DataGridViewCellToolTipTextNeededEventHandler(this._dataGridView_CellToolTipTextNeeded);
			this._dataGridView.CurrentCellDirtyStateChanged += new System.EventHandler(this._dataGridView_CurrentCellDirtyStateChanged);
			this._dataGridView.ItemDrag += new System.EventHandler<System.Windows.Forms.ItemDragEventArgs>(this._dataGridView_ItemDrag);
			this._dataGridView.ColumnWidthChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.DataGridView_ColumnWidthChanged);
			this._dataGridView.SelectionChanged += new System.EventHandler(this._dataGridView_SelectionChanged);
			this._dataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this._dataGridView_CellContentClick);
			// 
			// TableView
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._dataGridView);
			this.Controls.Add(this._statusStrip);
			this.Controls.Add(this._toolStrip);
			this.Name = "TableView";
			this.Load += new System.EventHandler(this.TableView_Load);
			((System.ComponentModel.ISupportInitialize)(this._bindingSource)).EndInit();
			this._toolStrip.ResumeLayout(false);
			this._toolStrip.PerformLayout();
			this._statusStrip.ResumeLayout(false);
			this._statusStrip.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._dataGridView)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.BindingSource _bindingSource;
        private System.Windows.Forms.ContextMenuStrip _contextMenu;
		private System.ComponentModel.IContainer components;
		private ClearCanvas.Desktop.View.WinForms.DataGridViewWithDragSupport _dataGridView;
        private System.Windows.Forms.ToolStrip _toolStrip;
        private System.Windows.Forms.ToolStripDropDownButton _sortButton;
        private System.Windows.Forms.ToolStripMenuItem _sortAscendingButton;
        private System.Windows.Forms.ToolStripMenuItem _sortDescendingButton;
        private System.Windows.Forms.ToolStripSeparator _sortSeparator;
        private System.Windows.Forms.ToolStripTextBox _filterTextBox;
        private System.Windows.Forms.ToolStripButton _clearFilterButton;
        private System.Windows.Forms.StatusStrip _statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel _statusLabel;
		private System.Windows.Forms.ToolStripLabel _filterLabel;
	}
}
