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

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;
using System;

namespace ClearCanvas.ImageViewer.Thumbnails.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="ThumbnailComponent"/>.
    /// </summary>
    public partial class ThumbnailComponentControl : ApplicationComponentUserControl
    {
        private ThumbnailComponent _component;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ThumbnailComponentControl(ThumbnailComponent component)
            :base(component)
        {
			_component = component;
            InitializeComponent();

			_galleryView.DataSource = _component.Thumbnails;

        	_imageSetTree.SelectionChanged += 
				delegate
            	{
            		_component.TreeSelection = _imageSetTree.Selection;
            	};

        	_imageSetTree.TreeBackColor = Color.FromKnownColor(KnownColor.Black);
			_imageSetTree.TreeForeColor = Color.FromKnownColor(KnownColor.ControlLight);
			_imageSetTree.TreeLineColor = Color.FromKnownColor(KnownColor.ControlLight);

			_component.PropertyChanged += OnPropertyChanged;

			_imageSetTree.Tree = _component.Tree;
        	_imageSetTree.VisibleChanged += OnTreeVisibleChanged;
		}

		private void OnTreeVisibleChanged(object sender, EventArgs e)
		{
			_imageSetTree.VisibleChanged -= OnTreeVisibleChanged;
			//the control isn't really visible until it's been drawn, so the selection can't be set until then.
			_imageSetTree.Selection = _component.TreeSelection;
		}

    	private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Tree")
			{
				_imageSetTree.Tree = _component.Tree;
			}
			else if (e.PropertyName == "TreeSelection")
			{
				_imageSetTree.Selection = _component.TreeSelection;
			}
		}
    }
}
