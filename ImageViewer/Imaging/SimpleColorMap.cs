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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Imaging
{
    /// TODO (CR Nov 2011): IColorMap does not inherit from IDataLut, so should there
    /// just be a clean separation here, or should it inherit?
    
	/// <summary>
	/// Basic implementation of an <see cref="IColorMap"/> whose size and data do not change.
	/// </summary>
	[Cloneable]
	public class SimpleColorMap : SimpleDataLut, IColorMap
	{
		private const string _exceptionMinOutputValue = "A minimum output value does not exist for a color map.";
		private const string _exceptionMaxOutputValue = "A maximum output value does not exist for a color map.";

		/// <summary>
		/// Initializes a new instance of <see cref="SimpleColorMap"/>.
		/// </summary>
		/// <param name="firstMappedPixelValue">The value of the first mapped pixel in the lookup table.</param>
		/// <param name="lutData">The lookup table mapping input pixel values to 32-bit ARGB colors.</param>
		/// <param name="key">A key suitable for identifying the color map.</param>
		/// <param name="description">A description of the color map.</param>
		public SimpleColorMap(int firstMappedPixelValue, int[] lutData, string key, string description)
			: base(firstMappedPixelValue, lutData, 0, 0, key, description) {}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected SimpleColorMap(SimpleColorMap source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		/// <summary>
		/// Creates a deep-copy of the <see cref="IColorMap"/>.
		/// </summary>
		/// <remarks>
		/// <see cref="IColorMap"/> implementations may return NULL from this method when appropriate.	
		/// </remarks>
		public new IColorMap Clone()
		{
			return base.Clone() as IColorMap;
		}

		/// <summary>
		/// This property is not applicable for this class.
		/// </summary>
		/// <exception cref="InvalidOperationException">Always thrown.</exception>
		public override sealed int MinOutputValue
		{
			get { throw new InvalidOperationException(_exceptionMinOutputValue); }
			protected set { throw new InvalidOperationException(_exceptionMinOutputValue); }
		}

		/// <summary>
		/// This property is not applicable for this class.
		/// </summary>
		/// <exception cref="InvalidOperationException">Always thrown.</exception>
		public override sealed int MaxOutputValue
		{
			get { throw new InvalidOperationException(_exceptionMaxOutputValue); }
			protected set { throw new InvalidOperationException(_exceptionMaxOutputValue); }
		}
	}
}