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

using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	internal class InteractiveTextAreaBuilder : InteractiveTextGraphicBuilder
	{
		public InteractiveTextAreaBuilder(ITextGraphic textGraphic) : base(textGraphic) {}

		internal new ITextGraphic Graphic
		{
			get { return (ITextGraphic) base.Graphic; }
		}

		protected override ITextGraphic FindTextGraphic()
		{
			IGraphic graphic = this.Graphic;
			while (graphic != null && !(graphic is TextEditControlGraphic))
				graphic = graphic.ParentGraphic;
			return graphic as TextEditControlGraphic;
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			this.Graphic.CoordinateSystem = CoordinateSystem.Destination;
			this.Graphic.Location = mouseInformation.Location;
			this.Graphic.ResetCoordinateSystem();
			this.NotifyGraphicComplete();
			return true;
		}
	}
}