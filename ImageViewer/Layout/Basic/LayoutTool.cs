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

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using System;
using ClearCanvas.ImageViewer.Automation;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	[DropDownAction("show", "global-toolbars/ToolbarStandard/ToolbarChangeLayout", "LayoutDropDownMenuModel")]
	[IconSet("show", "Icons.LayoutToolSmall.png", "Icons.LayoutToolMedium.png", "Icons.LayoutToolLarge.png")]
	[Tooltip("show", "TooltipChangeLayout")]
	[GroupHint("show", "Tools.Layout")]
	[EnabledStateObserver("show", "Enabled", "EnabledChanged")]

	/// <summary>
	/// This tool runs an instance of <see cref="LayoutComponent"/> in a shelf and coordinates
	/// it so that it reflects the state of the active workspace, as well as provides a dropdown custom action
	/// that can directly change the layout in the active imageviewer.
	/// </summary>
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
    public partial class LayoutTool : Tool<IImageViewerToolContext>
	{
		private ActionModelRoot _actionModel;
		private bool _enabled;

		protected virtual int MaximumImageBoxRows { get { return LayoutSettings.MaximumImageBoxRows; } }
		protected virtual int MaximumImageBoxColumns { get { return LayoutSettings.MaximumImageBoxColumns; } }
		protected virtual int MaximumTileRows { get { return LayoutSettings.MaximumTileRows; } }
		protected virtual int MaximumTileColumns { get { return LayoutSettings.MaximumTileColumns; } }

		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				if (value == _enabled)
					return;

				_enabled = value;
				EventsHelper.Fire(EnabledChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler EnabledChanged;

		/// <summary>
		/// Gets the action model for the layout drop down menu.
		/// </summary>
		public ActionModelNode LayoutDropDownMenuModel
		{
			get
			{
				if (_actionModel == null)
				{
					ActionModelRoot root = new ActionModelRoot();
					ResourceResolver resolver = new ApplicationThemeResourceResolver(this.GetType().Assembly);

					ActionPath pathBoxes = new ActionPath("root/ToolbarLayoutBoxesChooser", resolver);
					LayoutChangerAction actionBoxes = new LayoutChangerAction("chooseBoxLayout",
					                                                          this.MaximumImageBoxRows,
					                                                          this.MaximumImageBoxColumns,
					                                                          this.SetImageBoxLayout, pathBoxes, resolver);
					root.InsertAction(actionBoxes);

					ActionPath pathTiles = new ActionPath("root/ToolbarLayoutTilesChooser", resolver);
					LayoutChangerAction actionTiles = new LayoutChangerAction("chooseTileLayout",
					                                                          this.MaximumTileRows,
					                                                          this.MaximumTileColumns,
					                                                          this.SetTileLayout, pathTiles, resolver);
					root.InsertAction(actionTiles);

					_actionModel = root;
				}

				return _actionModel;
			}
		}

		/// <summary>
		/// Sets the layout of the current imageviewer to the specified number of imageboxes.
		/// </summary>
		/// <param name="rows">The number of rows to show.</param>
		/// <param name="columns">The number of columns to show.</param>
		public void SetImageBoxLayout(int rows, int columns)
		{
			LayoutComponent.SetImageBoxLayout(base.Context.Viewer, rows, columns);
		}

		/// <summary>
		/// Sets the layout of the current imageviewer to the specified number of tiles.
		/// </summary>
		/// <param name="rows">The number of rows to show.</param>
		/// <param name="columns">The number of columns to show.</param>
		public void SetTileLayout(int rows, int columns)
		{
			LayoutComponent.SetTileLayout(base.Context.Viewer, rows, columns);
		}

		public override void Initialize()
		{
			base.Initialize();
			base.Context.Viewer.PhysicalWorkspace.LockedChanged += new System.EventHandler(OnLockedChanged);
			Enabled = !base.Context.Viewer.PhysicalWorkspace.Locked;
		}

		protected override void Dispose(bool disposing)
		{
			base.Context.Viewer.PhysicalWorkspace.LockedChanged -= new System.EventHandler(OnLockedChanged);
			base.Dispose(disposing);
		}
		
		private void OnLockedChanged(object sender, System.EventArgs e)
		{
			Enabled = !base.Context.Viewer.PhysicalWorkspace.Locked;
		}

    }

    #region Oto
    partial class LayoutTool : IWorkspaceLayout
    {
        RectangularGrid IWorkspaceLayout.GetLayout()
        {
            var workspace = Context.Viewer.PhysicalWorkspace;
            if (workspace.Rows <= 0 || workspace.Columns <= 0)
                throw new NotSupportedException("Non-rectangular layouts not supported.");

            return new RectangularGrid { Rows = workspace.Rows, Columns = workspace.Columns };
        }

        void IWorkspaceLayout.SetLayout(RectangularGrid layout)
        {
            LayoutComponent.SetImageBoxLayout(Context.Viewer, layout.Rows, layout.Columns);
        }

        RectangularGrid IWorkspaceLayout.GetImageBoxLayoutAt(RectangularGrid.Location imageBoxLocation)
        {
            var imageBox = Context.Viewer.PhysicalWorkspace[imageBoxLocation.Row, imageBoxLocation.Column];
            if (imageBox == null)
                throw new InvalidOperationException("No image box is selected.");

            if (imageBox.Rows <= 0 || imageBox.Columns <= 0)
                throw new NotSupportedException("Non-rectangular layouts not supported.");

            return new RectangularGrid { Rows = imageBox.Rows, Columns = imageBox.Columns };
        }

        void IWorkspaceLayout.SetSelectedImageBoxLayout(RectangularGrid layout)
        {
            LayoutComponent.SetTileLayout(Context.Viewer, layout.Rows, layout.Columns);
        }

        void IWorkspaceLayout.SelectImageBoxAt(RectangularGrid.Location imageBoxLocation)
        {
            var workspace = Context.Viewer.PhysicalWorkspace;
            if (workspace.Rows <= 0 || workspace.Columns <= 0)
                throw new NotSupportedException("Non-rectangular layouts not supported.");

            workspace[imageBoxLocation.Row, imageBoxLocation.Column].SelectDefaultTile();
        }

        void IWorkspaceLayout.SelectTileAt(RectangularGrid.Location tileLocation)
        {
            var imageBoxLocation = tileLocation.ParentGridLocation;
            Platform.CheckForNullReference(imageBoxLocation, "tileLocation.ParentGridLocation");
            var imageBox = Context.Viewer.PhysicalWorkspace[imageBoxLocation.Row, imageBoxLocation.Column];
            if (imageBox == null)
                throw new InvalidOperationException("No image box is selected.");

            if (imageBox.Rows <= 0 || imageBox.Columns <= 0)
                throw new NotSupportedException("Non-rectangular layouts not supported.");

            imageBox[tileLocation.Row, tileLocation.Column].Select();
        }

        IImageBox IWorkspaceLayout.GetSelectedImageBox(out RectangularGrid.Location imageBoxLocation)
        {
            var workspace = Context.Viewer.PhysicalWorkspace;
            if (workspace.Rows <= 0 || workspace.Columns <= 0)
                throw new NotSupportedException("Non-rectangular layouts not supported.");

            var selectedImageBox = workspace.SelectedImageBox;
            for (int row = 0; row < workspace.Rows; ++row)
            {
                for (int column = 0; column < workspace.Columns; ++column)
                {
                    if (workspace[row, column] == selectedImageBox)
                    {
                        imageBoxLocation = new RectangularGrid.Location {Row = row, Column = column};
                        return selectedImageBox;
                    }
                }
            }

            throw new NotSupportedException("There's no image box selected, or something really bad is happening.");
        }

        ITile IWorkspaceLayout.GetSelectedTile(out RectangularGrid.Location tileLocation)
        {
            RectangularGrid.Location imageBoxLocation;
            var selectedImageBox = ((IWorkspaceLayout) this).GetSelectedImageBox(out imageBoxLocation);
            var selectedTile = selectedImageBox.SelectedTile;
            for (int row = 0; row < selectedImageBox.Rows; ++row)
            {
                for (int column = 0; column < selectedImageBox.Columns; ++column)
                {
                    if (selectedImageBox[row, column] == selectedTile)
                    {
                        tileLocation = new RectangularGrid.Location
                                           {
                                               Row = row, Column = column, ParentGridLocation = imageBoxLocation
                                           };
                        return selectedTile;
                    }
                }
            }

            throw new NotSupportedException("There's no image box selected, or something really bad is happening.");
        }

        IImageBox IWorkspaceLayout.GetImageBoxAt(RectangularGrid.Location imageBoxLocation)
        {
            var workspace = Context.Viewer.PhysicalWorkspace;
            if (workspace.Rows <= 0 || workspace.Columns <= 0)
                throw new NotSupportedException("Non-rectangular layouts not supported.");

            return workspace[imageBoxLocation.Row, imageBoxLocation.Column];
        }

        ITile IWorkspaceLayout.GetTileAt(RectangularGrid.Location tileLocation)
        {
            var imageBox = ((IWorkspaceLayout)this).GetImageBoxAt(new RectangularGrid.Location
                                {
                                    Row = tileLocation.ParentGridLocation.Row,
                                    Column = tileLocation.ParentGridLocation.Column
                                });
        
            if (imageBox.Rows <= 0 || imageBox.Columns <= 0)
                throw new NotSupportedException("Non-rectangular layouts not supported.");

            return imageBox[tileLocation.Row, tileLocation.Column];
        }
	}
    #endregion
}