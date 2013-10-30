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

using System.Drawing;
using System.Linq;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.KeyObjects
{
	[Cloneable]
	public class KeyObjectPlaceholderImage : GrayscalePresentationImage
	{
		[CloneCopyReference]
		private readonly KeyImageReference _keyImageReference;

		[CloneCopyReference]
		private readonly PresentationStateReference _presentationStateReference;

		private readonly string _reason;

		public KeyObjectPlaceholderImage(KeyImageReference keyImageReference, PresentationStateReference presentationStateReference, string reason)
			: base(5, 5)
		{
			_keyImageReference = keyImageReference;
			_presentationStateReference = presentationStateReference;

			InvariantTextPrimitive textGraphic = new InvariantTextPrimitive(_reason = reason);
			textGraphic.Color = Color.WhiteSmoke;
			base.ApplicationGraphics.Add(textGraphic);
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected KeyObjectPlaceholderImage(KeyObjectPlaceholderImage source, ICloningContext context) : base(source, context)
		{
			context.CloneFields(source, this);
		}

		public KeyImageReference KeyImageReference
		{
			get { return _keyImageReference; }
		}

		public PresentationStateReference PresentationStateReference
		{
			get { return _presentationStateReference; }
		}

		protected override void OnDrawing()
		{
			// upon drawing, re-centre the text
			RectangleF bounds = base.ClientRectangle;
			PointF anchor = new PointF(bounds.Left + bounds.Width/2, bounds.Top + bounds.Height/2);
			InvariantTextPrimitive textGraphic = (InvariantTextPrimitive) base.ApplicationGraphics.FirstOrDefault(IsType<InvariantTextPrimitive>);
			textGraphic.CoordinateSystem = CoordinateSystem.Destination;
			textGraphic.Location = anchor;
			textGraphic.ResetCoordinateSystem();
			base.OnDrawing();
		}

		public override IPresentationImage CreateFreshCopy()
		{
			return new KeyObjectPlaceholderImage(_keyImageReference, _presentationStateReference, _reason);
		}

		public override Size SceneSize
		{
			get { return new Size(100, 100); }
		}

		private static bool IsType<T>(object test)
		{
			return test is T;
		}
	}
}