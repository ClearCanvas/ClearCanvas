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

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Abstract base class for <see cref="IDataLut"/>s whose size and data
	/// do not change.
	/// </summary>
	[Cloneable]
	public class SimpleDataLut : DataLut
	{
		[CloneIgnore]
		private readonly int[] _data;
		private readonly string _key;
		private readonly string _description;
		private readonly int _firstMappedPixelValue;

		/// <summary>
		/// Constructor.
		/// </summary>
		public SimpleDataLut(int firstMappedPixelValue, int[] data, int minOutputValue, int maxOutputValue, string key, string description)
		{
			Platform.CheckForNullReference(data, "data");
			Platform.CheckForEmptyString(key, "key");
			Platform.CheckForEmptyString(description, "description");

			_data = data;

			_key = key;
			_description = description;

			_firstMappedPixelValue = firstMappedPixelValue;

			base.MinInputValue = _firstMappedPixelValue;
			base.MaxInputValue = LastMappedPixelValue;
			base.MinOutputValue = minOutputValue;
			base.MaxOutputValue = maxOutputValue;
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected SimpleDataLut(SimpleDataLut source, ICloningContext context)
		{
			context.CloneFields(source, this);

			//clone the actual buffer
			_data = (int[])source._data.Clone();
		}

		/// <summary>
		/// Gets the minimum output value.
		/// </summary>
		/// <remarks>
		/// This value is constant and cannot be changed.
		/// </remarks>
		public override int MinOutputValue
		{
			get { return base.MinOutputValue; }
			protected set
			{
			}
		}

		/// <summary>
		/// Gets the maximum output value.
		/// </summary>
		/// <remarks>
		/// This value is constant and cannot be changed.
		/// </remarks>
		public override int MaxOutputValue
		{
			get { return base.MaxOutputValue; }
			protected set
			{
			}
		}

		/// <summary>
		/// Gets a string key that identifies this particular Lut's characteristics, so that 
		/// an image's <see cref="IComposedLut"/> can be more efficiently determined.
		/// </summary>
		/// <remarks>
		/// This method is not to be confused with <b>equality</b>, since some Luts can be
		/// dependent upon the actual image to which it belongs.
		/// </remarks>
		public override string GetKey()
		{
			return _key;
		}

		/// <summary>
		/// Gets an abbreviated description of the Lut.
		/// </summary>
		public override string GetDescription()
		{
			return _description;
		}

		#region IDataLut Members

		/// <summary>
		/// Gets the first mapped pixel value.
		/// </summary>
		public sealed override int FirstMappedPixelValue
		{
			get { return _firstMappedPixelValue; }
		}

		/// <summary>
		/// Gets the last mapped pixel value.
		/// </summary>
		public sealed override int LastMappedPixelValue
		{
			get { return _firstMappedPixelValue + _data.Length - 1; }
		}

		/// <summary>
		/// Gets the lut data.
		/// </summary>
		public override int[] Data
		{
			get { return _data; }
		}

		#endregion
	}
}
