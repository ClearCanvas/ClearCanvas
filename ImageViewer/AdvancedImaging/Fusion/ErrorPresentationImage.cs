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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	[Cloneable]
	internal sealed class ErrorPresentationImage : BasicPresentationImage
	{
		private readonly string _errorMessage;

		public ErrorPresentationImage(string errorMessage)
			: base(new GrayscaleImageGraphic(8, 8))
		{
			CompositeImageGraphic.Graphics.Add(new ErrorMessageGraphic {Text = _errorMessage = errorMessage, Color = Color.WhiteSmoke});
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		private ErrorPresentationImage(ErrorPresentationImage source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		protected override IAnnotationLayout CreateAnnotationLayout()
		{
			return new AnnotationLayout();
		}

		public override IPresentationImage CreateFreshCopy()
		{
			return new ErrorPresentationImage(_errorMessage);
		}

		[Cloneable(true)]
		private class ErrorMessageGraphic : InvariantTextPrimitive
		{
			protected override SpatialTransform CreateSpatialTransform()
			{
				return new InvariantSpatialTransform(this);
			}

			public override void OnDrawing()
			{
				if (base.ParentPresentationImage != null)
				{
					CoordinateSystem = CoordinateSystem.Destination;
					try
					{
						var clientRectangle = ParentPresentationImage.ClientRectangle;
						Location = new PointF(clientRectangle.Width/2f, clientRectangle.Height/2f);
					}
					finally
					{
						ResetCoordinateSystem();
					}
				}
				base.OnDrawing();
			}
		}
	}
}