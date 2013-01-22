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

#region Additional permission to link with DotNetMagic

// Additional permission under GNU GPL version 3 section 7
// 
// If you modify this Program, or any covered work, by linking or combining it
// with DotNetMagic (or a modified version of that library), containing parts
// covered by the terms of the Crownwood Software DotNetMagic license, the
// licensors of this Program grant you additional permission to convey the
// resulting work.

#endregion

using System;
using System.Drawing;
using System.Windows.Forms;
using Crownwood.DotNetMagic.Controls;

namespace ClearCanvas.Desktop.View.WinForms
{
	/// <summary>
	/// Summary description for Example.
	/// </summary>
	public class StackTab : UserControl
	{        
        // Private field
        private Control _applicationComponentControl;
        private EventHandler _buttonClicked;
		private EventHandler _titleClicked;
		private EventHandler _titleDoubleClicked;

		// Designer generated
        private Crownwood.DotNetMagic.Controls.TitleBar _titleBar;
        private Panel _panel;
        private TableLayoutPanel tableLayoutPanel1;

		private readonly StackTabPage _page;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public StackTab()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
		}

		// New constructor
		public StackTab(StackTabPage page, DockStyle docStyle)
		{
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

        	_page = page;
			_titleBar.Dock = docStyle;

            // Set initial values
			_titleBar.PreText = String.Empty;
			_titleBar.Text = _page.Title;
			_titleBar.PostText = String.Empty;
			if (_page.IconSet != null)
				_titleBar.Image = _page.IconSet.CreateIcon(IconSize.Small, _page.ResourceResolver);

			_page.TitleChanged += OnPageTitleChanged;
			_page.IconSetChanged += OnPageIconChanged;
		}

		#region Event Handlers

		private void OnPageTitleChanged(object sender, EventArgs e)
		{
			_titleBar.PreText = String.Empty;
			_titleBar.Text = _page.Title;
			_titleBar.PostText = String.Empty;
		}

		private void OnPageIconChanged(object sender, EventArgs e)
		{
			if (_page.IconSet != null)
				_titleBar.Image = _page.IconSet.CreateIcon(IconSize.Small, _page.ResourceResolver);
			else
				_titleBar.Image = null;
		}

		private void OnButtonClick(object sender, EventArgs e)
		{
			if (_buttonClicked != null)
				_buttonClicked(sender, e);
		}

		private void OnTitleClicked(object sender, EventArgs e)
		{
			if (_titleClicked != null)
				_titleClicked(sender, e);
		}

		private void OnTitleDoubleClick(object sender, EventArgs e)
		{
			if (_titleDoubleClicked != null)
				_titleDoubleClicked(sender, e);
		}

		#endregion

		#region UserControl overrides

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

		#endregion

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StackTab));
			this._titleBar = new Crownwood.DotNetMagic.Controls.TitleBar();
			this._panel = new System.Windows.Forms.Panel();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _titleBar
			// 
			this._titleBar.ArrowButton = Crownwood.DotNetMagic.Controls.ArrowButton.DownArrow;
			resources.ApplyResources(this._titleBar, "_titleBar");
			this._titleBar.GradientColoring = Crownwood.DotNetMagic.Controls.GradientColoring.LightBackToDarkBack;
			this._titleBar.ImageAlignment = Crownwood.DotNetMagic.Controls.ImageAlignment.Far;
			this._titleBar.MouseOverColor = System.Drawing.Color.Empty;
			this._titleBar.Name = "_titleBar";
			this._titleBar.Style = Crownwood.DotNetMagic.Common.VisualStyle.Office2007Black;
			this._titleBar.Click += new System.EventHandler(this.OnTitleClicked);
			this._titleBar.ButtonClick += new System.EventHandler(this.OnButtonClick);
			this._titleBar.DoubleClick += new System.EventHandler(this.OnTitleDoubleClick);
			// 
			// _panel
			// 
			resources.ApplyResources(this._panel, "_panel");
			this._panel.Name = "_panel";
			// 
			// tableLayoutPanel1
			// 
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this._titleBar, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this._panel, 0, 1);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			// 
			// StackTab
			// 
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "StackTab";
			resources.ApplyResources(this, "$this");
			this.tableLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		// Allow direct access to the titlebar
		public Crownwood.DotNetMagic.Controls.TitleBar TitleBar
		{
			get { return _titleBar; }
		}

		public event EventHandler ButtonClicked
		{
			add { _buttonClicked += value; }
			remove { _buttonClicked -= value; }
		}

		public event EventHandler TitleClicked
		{
			add { _titleClicked += value; }
			remove { _titleClicked -= value; }
		}

		public event EventHandler TitleDoubleClicked
		{
			add { _titleDoubleClicked += value; }
			remove { _titleDoubleClicked -= value; }
		}

		public Control ApplicationComponentControl
        {
            get { return _applicationComponentControl; }
            set
            {
                _applicationComponentControl = value;
                this._panel.Controls.Clear();
                this._panel.Controls.Add(_applicationComponentControl);
            }
        }

		// Caller can discover minimum requested size
		public Size MinimumRequestedSize
		{
			get { return new Size(23, 23); }
		}
	}
}
