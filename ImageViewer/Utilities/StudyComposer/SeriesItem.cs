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
using System.ComponentModel;
using System.Drawing;
using System.Text;
using ClearCanvas.Dicom.Utilities.StudyBuilder;

namespace ClearCanvas.ImageViewer.Utilities.StudyComposer
{
	/// <summary>
	/// A <see cref="StudyComposerItemBase{T}"/> that wraps a <see cref="SeriesNode"/> in a <see cref="StudyBuilder"/> tree.
	/// </summary>
	public class SeriesItem : StudyComposerItemBase<SeriesNode>
	{
		private readonly ImageItemCollection _images;

		public SeriesItem(SeriesNode series)
		{
			base.Node = series;
			_images = new ImageItemCollection(series.Images);
			_images.ListChanged += new ListChangedEventHandler(OnChildListChanged);
		}

		private SeriesItem(SeriesItem source) : this(source.Node.Copy(false))
		{
			this.Icon = (Image) source.Icon.Clone();
			foreach (ImageItem image in source.Images)
			{
				this.Images.Add(image.Copy());
			}
		}

		public ImageItemCollection Images
		{
			get { return _images; }
		}

		public void InsertItems(ImageItem[] images)
		{
			foreach (ImageItem item in images)
			{
				this.Images.Add(item);
			}
		}

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public SeriesItem Copy()
		{
			return new SeriesItem(this);
		}

		private void OnChildListChanged(object sender, ListChangedEventArgs e)
		{
			switch(e.ListChangedType)
			{
				case ListChangedType.Reset:
				case ListChangedType.ItemAdded:
				case ListChangedType.ItemDeleted:
				case ListChangedType.ItemMoved:
				case ListChangedType.ItemChanged:
					UpdateIcon();
					break;
			}
		}

		// The key image is the middle image of the series
		private Image GetKeyImage()
		{
			if (_images != null && _images.Count > 0)
				return _images[(_images.Count - 1)/2].Icon;
			return null;
		}

		#region Overrides

		/// <summary>
		/// Gets or sets the name label of this item.
		/// </summary>
		public override string Name
		{
			get { return base.Node.Description; }
			set { base.Node.Description = value; }
		}

		/// <summary>
		/// Gets a short, multi-line description of the item that contains ancillary information.
		/// </summary>
		public override string Description
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				if (base.Node.DateTime.HasValue)
					sb.AppendLine(base.Node.DateTime.Value.ToString());
				sb.AppendLine(base.Node.InstanceUid);
				return sb.ToString();
			}
		}

		protected override void OnNodePropertyChanged(string propertyName)
		{
			base.OnNodePropertyChanged(propertyName);

			if (propertyName == "Description")
				base.FirePropertyChanged("Name");
			else if (propertyName == "DateTime" || propertyName == "InstanceUid")
				base.FirePropertyChanged("Description");
		}

		/// <summary>
		/// Regenerates the icon for a specific icon size.
		/// </summary>
		/// <param name="iconSize">The <see cref="Size"/> of the icon to generate.</param>
		public override void UpdateIcon(Size iconSize)
		{
			Image keyImage = GetKeyImage();
			_helper.IconSize = iconSize;
			base.Icon = _helper.CreateStackIcon(keyImage);
		}

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public override StudyComposerItemBase<SeriesNode> Clone()
		{
			return this.Copy();
		}

		#endregion

		#region Statics

		private static readonly IconHelper _helper = new IconHelper();

		static SeriesItem()
		{
			_helper.IconSize = new Size(64, 64);
			_helper.StackSize = 3;
			_helper.StackOffset = new Size(5, -5);
		}

		#endregion
	}
}