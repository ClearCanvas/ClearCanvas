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

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A Composed Lut is one that has been created by combining multiple Luts together.
	/// </summary>
	/// <remarks>
	/// The <see cref="Data"/> property should be considered readonly and is only provided
	/// for fast (unsafe) iteration overy the array.  However, it also enforces that <see cref="IComposedLut"/>s
	/// be data Luts, which is important because the overall efficiency of the Lut pipeline is improved 
	/// substantially.
	/// </remarks>
	public interface IComposedLut
	{
		/// <summary>
		/// Gets the minimum input value.
		/// </summary>
		int MinInputValue { get; }

		/// <summary>
		/// Gets the maximum input value.
		/// </summary>
		int MaxInputValue { get; }

		/// <summary>
		/// Gets the minimum output value.
		/// </summary>
		int MinOutputValue { get; }

		/// <summary>
		/// Gets the maximum output value.
		/// </summary>
		int MaxOutputValue { get; }

		/// <summary>
		/// Gets the output value of the lut at a given input index.
		/// </summary>
		int this[int index] { get; }

		/// <summary>
		/// Gets the lut's data.
		/// </summary>
		/// <remarks>
		/// This property should be considered readonly and is only 
		/// provided for fast (unsafe) iteration over the array.
		/// </remarks>
		int[] Data { get; }
	}
}
