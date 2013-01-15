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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="TileComponent"/>
    /// </summary>
	[ExtensionOf(typeof(ImageBoxViewExtensionPoint))]
    public class ImageBoxView : WinFormsView, IView
    {
		private ImageBox _imageBox;
		private ImageBoxControl _imageBoxControl;
		private Rectangle _parentRectangle;

		public ImageBox ImageBox
		{
			get { return _imageBox; }
			set { _imageBox = value; }
		}

		public Rectangle ParentRectangle
		{
			get { return _parentRectangle; }
			set { _parentRectangle = value; }
		}

        public override object GuiElement
        {
            get
            {
                if (_imageBoxControl == null)  
                {
					_imageBoxControl = new ImageBoxControl(this.ImageBox, this.ParentRectangle);

                    var decorators = new ImageBoxControlDecoratorExtensionPoint().CreateExtensions().Cast < IImageBoxControlDecorator>();
                    foreach(var decorator in decorators)
                    {
                        _imageBoxControl = decorator.Apply(_imageBoxControl, this.ImageBox);
                    }
                }
                return _imageBoxControl;
            }
        }

        
    }

    [ExtensionPoint]
    public class ImageBoxControlDecoratorExtensionPoint:ExtensionPoint<IImageBoxControlDecorator>{}

    public interface IImageBoxControlDecorator
    {
        ImageBoxControl Apply(ImageBoxControl control, ImageBox box);
    }

}
