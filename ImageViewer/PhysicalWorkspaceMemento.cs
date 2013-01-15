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
using ClearCanvas.Desktop;
using System.Collections.Generic;

namespace ClearCanvas.ImageViewer
{
	internal class PhysicalWorkspaceMemento
	{
		private ImageBoxCollection _imageBoxes;
		private List<object> _imageBoxMementos;
		private int _rows;
		private int _columns;

		public PhysicalWorkspaceMemento(
			ImageBoxCollection imageBoxes,
			List<object> imageBoxMementos,
			int rows,
			int columns)
		{
			Platform.CheckForNullReference(imageBoxes, "imageBoxes");
			Platform.CheckForNullReference(imageBoxMementos, "imageBoxMementos");

			_imageBoxes = imageBoxes;
			_imageBoxMementos = imageBoxMementos;
			_rows = rows;
			_columns = columns;
		}

		public ImageBoxCollection ImageBoxes
		{
			get { return _imageBoxes; }
		}

		public List<object> ImageBoxMementos
		{
			get { return _imageBoxMementos; }
		}

		public int Rows
		{
			get { return _rows; }
		}

		public int Columns
		{
			get { return _columns; }
		}
	}
}
