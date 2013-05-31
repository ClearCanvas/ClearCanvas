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
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.Rendering;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[ExtensionPoint]
	public sealed class MagnificationViewExtensionPoint : ExtensionPoint<IMagnificationView>
	{
	}

    public delegate void RenderMagnifiedImage(DrawArgs args);

	public interface IMagnificationView : IView
	{
        void Open(IPresentationImage image, Point locationTile, RenderMagnifiedImage render);
        void UpdateMouseLocation(Point locationTile);
        void Close();
	}

    [MenuAction("activate", "imageviewer-contextmenu/MenuMagnification", "Select", Flags = ClickActionFlags.CheckAction, InitiallyAvailable = false)]
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuMagnification", "Select", Flags = ClickActionFlags.CheckAction)]
	[DropDownButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarMagnification", "Select", "MagnificationMenuModel", Flags = ClickActionFlags.CheckAction)]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[MouseToolButton(XMouseButtons.Left, false)]
	[MouseButtonIconSet("activate", "Icons.MagnificationToolSmall.png", "Icons.MagnificationToolMedium.png", "Icons.MagnificationToolLarge.png")]
    [CheckedStateObserver("activate", "Active", "ActivationChanged")]
    [VisibleStateObserver("activate", "Visible", "VisibleChanged")]
	[GroupHint("activate", "Tools.Image.Magnify")]

	[MenuAction("1.5x", "magnification-dropdown/MenuMagnification1AndOneHalf", "Set1And1HalfMagnification")]
	[CheckedStateObserver("1.5x", "Magnification1And1HalfChecked", "CheckedChanged")]

	[MenuAction("2x", "magnification-dropdown/MenuMagnification2x", "Set2xMagnification")]
	[CheckedStateObserver("2x", "Magnification2xChecked", "CheckedChanged")]

	[MenuAction("4x", "magnification-dropdown/MenuMagnification4x", "Set4xMagnification")]
	[CheckedStateObserver("4x", "Magnification4xChecked", "CheckedChanged")]

	[MenuAction("6x", "magnification-dropdown/MenuMagnification6x", "Set6xMagnification")]
	[CheckedStateObserver("6x", "Magnification6xChecked", "CheckedChanged")]

	[MenuAction("8x", "magnification-dropdown/MenuMagnification8x", "Set8xMagnification")]
	[CheckedStateObserver("8x", "Magnification8xChecked", "CheckedChanged")]
	
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public partial class MagnificationTool : MouseImageViewerTool
	{
		private IMagnificationView _view = null;
		private readonly CursorToken _cursorToken = new CursorToken("Icons.BlankCursor.png", typeof(MagnificationTool).Assembly);

        static MagnificationTool()
        {
            IsSupported = CreateView() != null;
        }

        private static IMagnificationView CreateView()
        {
            try
            {
                return (IMagnificationView)ViewFactory.CreateView(new MagnificationViewExtensionPoint());
            }
            catch
            {
                return null;
            }
        }

        private static bool IsSupported { get; set; }

        public MagnificationTool()
			: base(SR.TooltipMagnification)
		{
			base.Behaviour |= MouseButtonHandlerBehaviour.SuppressContextMenu;
		}

		public ActionModelNode MagnificationMenuModel
		{
			get
			{
				return ActionModelRoot.CreateModel(typeof (MagnificationTool).FullName, "magnification-dropdown", base.Actions);
			}	
		}

		public bool Magnification1And1HalfChecked
		{
			get
			{
				return FloatComparer.AreEqual(ToolSettings.DefaultInstance.MagnificationFactor, 1.5F);
			}	
		}

		public bool Magnification2xChecked
		{
			get
			{
                return FloatComparer.AreEqual(ToolSettings.DefaultInstance.MagnificationFactor, 2.0F);
			}
		}

		public bool Magnification4xChecked
		{
			get
			{
                return FloatComparer.AreEqual(ToolSettings.DefaultInstance.MagnificationFactor, 4.0F);
			}
		}

		public bool Magnification6xChecked
		{
			get
			{
                return FloatComparer.AreEqual(ToolSettings.DefaultInstance.MagnificationFactor, 6.0F);
			}
		}

		public bool Magnification8xChecked
		{
			get
			{
                return FloatComparer.AreEqual(ToolSettings.DefaultInstance.MagnificationFactor, 8.0F);
			}
		}
		
        public bool Visible
        { 
            get { return IsSupported; }
        }

        //Never fires.
#pragma warning disable 67
        public event EventHandler VisibleChanged;
#pragma warning restore 67
        
        public event EventHandler CheckedChanged;

		public override void Initialize()
		{
			base.Initialize();

            ToolSettings.DefaultInstance.PropertyChanged += OnMagnificationSettingChanged;
			UpdateEnabled();
		}

		protected override void Dispose(bool disposing)
		{
            ToolSettings.DefaultInstance.PropertyChanged -= OnMagnificationSettingChanged;
		    var view = _view as IDisposable;
            if (view != null)
            {
                view.Dispose();
                _view = null;
            }

		    base.Dispose(disposing);
		}

		public void Set1And1HalfMagnification()
		{
            ToolSettings.DefaultInstance.MagnificationFactor = 1.5F;
            ToolSettings.DefaultInstance.Save();
		}

		public void Set2xMagnification()
		{
            ToolSettings.DefaultInstance.MagnificationFactor = 2F;
            ToolSettings.DefaultInstance.Save();
		}

		public void Set4xMagnification()
		{
            ToolSettings.DefaultInstance.MagnificationFactor = 4F;
            ToolSettings.DefaultInstance.Save();
		}

		public void Set6xMagnification()
		{
            ToolSettings.DefaultInstance.MagnificationFactor = 6F;
            ToolSettings.DefaultInstance.Save();
		}

		public void Set8xMagnification()
		{
            ToolSettings.DefaultInstance.MagnificationFactor = 8F;
            ToolSettings.DefaultInstance.Save();
		}

		private void UpdateEnabled()
		{
			if (base.SelectedSpatialTransformProvider != null && base.SelectedPresentationImage is PresentationImage)
			{
				if (base.SelectedSpatialTransformProvider.SpatialTransform is IImageSpatialTransform)
				{
					Enabled = true;
					return;
				}
			}

			Enabled = false;
		}

		private void OnMagnificationSettingChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			EventsHelper.Fire(CheckedChanged, this, EventArgs.Empty);
		}

		protected override void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			UpdateEnabled();
		}

		protected override void OnTileSelected(object sender, TileSelectedEventArgs e)
		{
			UpdateEnabled();
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			if (!Enabled || !Visible)
				return false;

			base.Start(mouseInformation);

			try
			{
				if (_view != null)
					throw new InvalidOperationException("A magnification component is already active.");

			    var view = CreateView();
                _tileLocation = mouseInformation.Location;
			    InitializeMagnificationImage();
                view.Open(SelectedPresentationImage, mouseInformation.Location, RenderImage);

				_view = view;
				return true;
			}
			catch(Exception e)
			{
				ExceptionHandler.Report(e, Context.DesktopWindow);
				return false;
			}
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			if (_view != null)
			{
			    _tileLocation = mouseInformation.Location;
				_view.UpdateMouseLocation(ConstrainPointToTile(mouseInformation));
				return true;
			}

			return false;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			Cancel();
			return false;
		}

		public override void Cancel()
		{
			if (_view != null)
			{
				_view.Close();
				_view = null;
			}

            DisposeMagnificationImage();
        }

		public override CursorToken GetCursorToken(Point point)
		{
			if (_view != null)
				return _cursorToken;

			return null;
		}

		private Point ConstrainPointToTile(IMouseInformation mouseInformation)
		{
			Rectangle rectangle = mouseInformation.Tile.ClientRectangle;
			int x = mouseInformation.Location.X;
			int y = mouseInformation.Location.Y;
			if (x < rectangle.Left)
				x = rectangle.Left;
			if (x > rectangle.Right)
				x = rectangle.Right;

			if (y < rectangle.Top)
				y = rectangle.Top;
			if (y > rectangle.Bottom)
				y = rectangle.Bottom;

			return new Point(x, y);
		}
	}
}
