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
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.StudyBuilder;

namespace ClearCanvas.ImageViewer.Utilities.StudyComposer
{
	/// <summary>
	/// A <see cref="StudyComposerItemBase{T}"/> that wraps a <see cref="SopInstanceNode"/> in a <see cref="StudyBuilder"/> tree.
	/// </summary>
	public class ImageItem : StudyComposerItemBase<SopInstanceNode>
	{
		private readonly string _igKey;
		private string _name = SR.FormatStudyComposerGenericImageLabelCaption;

		public ImageItem(SopInstanceNode sopInstance, IPresentationImage pImage)
		{
			base.Node = sopInstance;

			int num = sopInstance.DicomData[DicomTags.InstanceNumber].GetInt32(0, -1);
			if (num >= 0)
				_name = string.Format(SR.FormatStudyComposerImageLabelCaption, num);

			string seriesDesc = sopInstance.DicomData[DicomTags.SeriesDescription].GetString(0, "");
			if (seriesDesc.Length > 0)
				_name = string.Format("{0}: {1}", seriesDesc, _name);

			_igKey = sopInstance.InstanceUid;

			if (pImage != null)
			{
				//_cache.LoadIcon(_igKey, delegate() { return _helper.CreateImageIcon(pImage); }, this.RefreshIcon);
				base.Icon = (Image)_helper.CreateImageIcon(pImage);// _cache[_igKey];
			}
		}

		private ImageItem(ImageItem source) : this(source.Node.Copy(), null)
		{
			this._name = source._name;
			this._igKey = source._igKey;
			base.Icon = (Image) source.Icon.Clone();// _cache[_igKey];
		}

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public ImageItem Copy()
		{
			return new ImageItem(this);
		}

		private void RefreshIcon() {
			//base.Icon = _cache[_igKey];
		}

		#region Overrides

		/// <summary>
		/// Gets or sets the name label of this item.
		/// </summary>
		public override string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>
		/// Gets a short, multi-line description of the item that contains ancillary information.
		/// </summary>
		public override string Description
		{
			get { return base.Node.InstanceUid; }
		}

		/// <summary>
		/// Regenerates the icon for a specific icon size.
		/// </summary>
		/// <param name="iconSize">The <see cref="Size"/> of the icon to generate.</param>
		public override void UpdateIcon(Size iconSize) {
			Image _baseIcon = this.Icon;// _cache[_igKey];
			if (_baseIcon.Size != iconSize)
			{
				// if requesting a new icon size
				_helper.IconSize = iconSize;
				base.Icon = _helper.CreateImageIcon(_baseIcon);
			}
			else
			{
				base.Icon = _baseIcon;
			}
		}

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public override StudyComposerItemBase<SopInstanceNode> Clone()
		{
			return this.Copy();
		}

		protected override void OnNodePropertyChanged(string propertyName)
		{
			base.OnNodePropertyChanged(propertyName);
			if (propertyName == "InstanceUid")
				FirePropertyChanged("Description");
		}

		#endregion

		#region Statics

		private static readonly IconHelper _helper = new IconHelper(64, 64);
		//private static readonly IconCache _cache = new IconCache();

		static ImageItem()
		{
			_helper.IconSize = new Size(64, 64);
		}

		internal static void ClearIconCache()
		{
			//_cache.Clear();
		}

		#endregion
	}
}