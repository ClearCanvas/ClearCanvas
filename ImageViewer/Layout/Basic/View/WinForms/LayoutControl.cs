#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
	/// <summary>
    /// Provides the user-interface for <see cref="LayoutComponentView"/>
	/// </summary>
	public class LayoutControl : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Label _tileColumnsLabel;
		private System.Windows.Forms.Label _tileRowsLabel;
		private System.Windows.Forms.Label _imageBoxColumnsLabel;
		private System.Windows.Forms.Label _imageBoxRowsLabel;
		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _imageBoxRows;
		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _imageBoxColumns;
		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _tileColumns;
		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _tileRows;
		private Button _applyTiles;
		private Button _applyImageBoxes;
		private Panel imageBoxPanel;
		private Panel tilePanel;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;


        private LayoutComponent _layoutComponent;
		private GroupBox groupBox1;
		private GroupBox groupBox2;
		private Button _buttonConfigure;
        private BindingSource _bindingSource;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="component">The component to look at</param>
		public LayoutControl(LayoutComponent component)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

            _layoutComponent = component;

            // rather than binding directly to the component, create a binding source
            // this is the only way that we can pull data from the component on demand
            _bindingSource = new BindingSource();
            _bindingSource.DataSource = _layoutComponent;

			//these values are just constants, so we won't databind them, it's unnecessary.
			_imageBoxRows.Minimum = 1;
			_imageBoxColumns.Minimum = 1;
			_tileRows.Minimum = 1;
			_tileColumns.Minimum = 1;

			_imageBoxRows.Maximum = _layoutComponent.MaximumImageBoxRows;
			_imageBoxColumns.Maximum = _layoutComponent.MaximumImageBoxColumns;
			_tileRows.Maximum = _layoutComponent.MaximumTileRows;
			_tileColumns.Maximum = _layoutComponent.MaximumTileColumns;
			
			// bind control values
            _tileColumns.DataBindings.Add("Value", _bindingSource, "TileColumns", true, DataSourceUpdateMode.OnPropertyChanged);
            _tileRows.DataBindings.Add("Value", _bindingSource, "TileRows", true, DataSourceUpdateMode.OnPropertyChanged);
            _imageBoxColumns.DataBindings.Add("Value", _bindingSource, "ImageBoxColumns", true, DataSourceUpdateMode.OnPropertyChanged);
            _imageBoxRows.DataBindings.Add("Value", _bindingSource, "ImageBoxRows", true, DataSourceUpdateMode.OnPropertyChanged);

            // bind control enablement
			_imageBoxColumns.DataBindings.Add("Enabled", _bindingSource, "ImageBoxSectionEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_imageBoxRows.DataBindings.Add("Enabled", _bindingSource, "ImageBoxSectionEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_applyImageBoxes.DataBindings.Add("Enabled", _bindingSource, "ImageBoxSectionEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_tileColumns.DataBindings.Add("Enabled", _bindingSource, "TileSectionEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_tileRows.DataBindings.Add("Enabled", _bindingSource, "TileSectionEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_applyTiles.DataBindings.Add("Enabled", _bindingSource, "TileSectionEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        /// <summary>
        /// Event handler for the image boxes Apply button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _applyImageBoxes_Click(object sender, EventArgs e)
        {
            _layoutComponent.ApplyImageBoxLayout();
        }

        /// <summary>
        /// Event handler for the tiles Apply button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _applyTiles_Click(object sender, EventArgs e)
        {
            _layoutComponent.ApplyTileLayout();
        }

		private void OnButtonConfigureClick(object sender, EventArgs e)
		{
			_layoutComponent.Configure();
		}

        /// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LayoutControl));
			this._tileColumnsLabel = new System.Windows.Forms.Label();
			this._tileRowsLabel = new System.Windows.Forms.Label();
			this._imageBoxColumnsLabel = new System.Windows.Forms.Label();
			this._imageBoxRowsLabel = new System.Windows.Forms.Label();
			this._applyTiles = new System.Windows.Forms.Button();
			this._applyImageBoxes = new System.Windows.Forms.Button();
			this.imageBoxPanel = new System.Windows.Forms.Panel();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this._imageBoxColumns = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
			this._imageBoxRows = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
			this.tilePanel = new System.Windows.Forms.Panel();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this._tileRows = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
			this._tileColumns = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
			this._buttonConfigure = new System.Windows.Forms.Button();
			this.imageBoxPanel.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._imageBoxColumns)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._imageBoxRows)).BeginInit();
			this.tilePanel.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._tileRows)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._tileColumns)).BeginInit();
			this.SuspendLayout();
			// 
			// _tileColumnsLabel
			// 
			resources.ApplyResources(this._tileColumnsLabel, "_tileColumnsLabel");
			this._tileColumnsLabel.Name = "_tileColumnsLabel";
			// 
			// _tileRowsLabel
			// 
			resources.ApplyResources(this._tileRowsLabel, "_tileRowsLabel");
			this._tileRowsLabel.Name = "_tileRowsLabel";
			// 
			// _imageBoxColumnsLabel
			// 
			resources.ApplyResources(this._imageBoxColumnsLabel, "_imageBoxColumnsLabel");
			this._imageBoxColumnsLabel.Name = "_imageBoxColumnsLabel";
			// 
			// _imageBoxRowsLabel
			// 
			resources.ApplyResources(this._imageBoxRowsLabel, "_imageBoxRowsLabel");
			this._imageBoxRowsLabel.Name = "_imageBoxRowsLabel";
			// 
			// _applyTiles
			// 
			resources.ApplyResources(this._applyTiles, "_applyTiles");
			this._applyTiles.Name = "_applyTiles";
			this._applyTiles.Click += new System.EventHandler(this._applyTiles_Click);
			// 
			// _applyImageBoxes
			// 
			resources.ApplyResources(this._applyImageBoxes, "_applyImageBoxes");
			this._applyImageBoxes.Name = "_applyImageBoxes";
			this._applyImageBoxes.Click += new System.EventHandler(this._applyImageBoxes_Click);
			// 
			// imageBoxPanel
			// 
			this.imageBoxPanel.Controls.Add(this.groupBox1);
			resources.ApplyResources(this.imageBoxPanel, "imageBoxPanel");
			this.imageBoxPanel.Name = "imageBoxPanel";
			// 
			// groupBox1
			// 
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Controls.Add(this._imageBoxColumnsLabel);
			this.groupBox1.Controls.Add(this._imageBoxColumns);
			this.groupBox1.Controls.Add(this._imageBoxRows);
			this.groupBox1.Controls.Add(this._imageBoxRowsLabel);
			this.groupBox1.Controls.Add(this._applyImageBoxes);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// _imageBoxColumns
			// 
			resources.ApplyResources(this._imageBoxColumns, "_imageBoxColumns");
			this._imageBoxColumns.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this._imageBoxColumns.Name = "_imageBoxColumns";
			this._imageBoxColumns.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// _imageBoxRows
			// 
			resources.ApplyResources(this._imageBoxRows, "_imageBoxRows");
			this._imageBoxRows.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this._imageBoxRows.Name = "_imageBoxRows";
			this._imageBoxRows.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// tilePanel
			// 
			this.tilePanel.Controls.Add(this.groupBox2);
			resources.ApplyResources(this.tilePanel, "tilePanel");
			this.tilePanel.Name = "tilePanel";
			// 
			// groupBox2
			// 
			resources.ApplyResources(this.groupBox2, "groupBox2");
			this.groupBox2.Controls.Add(this._applyTiles);
			this.groupBox2.Controls.Add(this._tileRowsLabel);
			this.groupBox2.Controls.Add(this._tileColumnsLabel);
			this.groupBox2.Controls.Add(this._tileRows);
			this.groupBox2.Controls.Add(this._tileColumns);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.TabStop = false;
			// 
			// _tileRows
			// 
			resources.ApplyResources(this._tileRows, "_tileRows");
			this._tileRows.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this._tileRows.Name = "_tileRows";
			this._tileRows.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// _tileColumns
			// 
			resources.ApplyResources(this._tileColumns, "_tileColumns");
			this._tileColumns.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this._tileColumns.Name = "_tileColumns";
			this._tileColumns.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// _buttonConfigure
			// 
			resources.ApplyResources(this._buttonConfigure, "_buttonConfigure");
			this._buttonConfigure.Name = "_buttonConfigure";
			this._buttonConfigure.UseVisualStyleBackColor = true;
			this._buttonConfigure.Click += new System.EventHandler(this.OnButtonConfigureClick);
			// 
			// LayoutControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._buttonConfigure);
			this.Controls.Add(this.tilePanel);
			this.Controls.Add(this.imageBoxPanel);
			this.Name = "LayoutControl";
			this.imageBoxPanel.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this._imageBoxColumns)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._imageBoxRows)).EndInit();
			this.tilePanel.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this._tileRows)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._tileColumns)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion
	}
}
