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

using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Abstract base implementation for a lookup table in the standard grayscale image display pipeline that performs any additional transformation prior to selecting the VOI range.
	/// </summary>
	/// <seealso cref="LutComposer"/>
	/// <seealso cref="IComposableLut"/>
	[Cloneable(true)]
	public abstract class ComposableLut : ComposableLutBase
	{
		/// <summary>
		/// Gets or sets the minimum input value.
		/// </summary>
		/// <remarks>
		/// This value is set internally by the framework and should not be modified by client code.
		/// </remarks>
		public abstract double MinInputValue { get; set; }

		/// <summary>
		/// Gets or sets the maximum input value.
		/// </summary>
		/// <remarks>
		/// This value is set internally by the framework and should not be modified by client code.
		/// </remarks>
		public abstract double MaxInputValue { get; set; }

		/// <summary>
		/// Gets or sets the minimum output value.
		/// </summary>
		public abstract double MinOutputValue { get; protected set; }

		/// <summary>
		/// Gets or sets the maximum output value.
		/// </summary>
		public abstract double MaxOutputValue { get; protected set; }

		/// <summary>
		/// Gets the output value of the lookup table for a given input value.
		/// </summary>
		public abstract double this[double input] { get; }

		/// <summary>
		/// Performs bulk lookup for a set of input values.
		/// </summary>
		/// <param name="input">Source array of input values to be transformed.</param>
		/// <param name="output">Destination array where transformed output values will be written (may be same array as <paramref name="input"/>).</param>
		/// <param name="count">Number of values in the arrays to transform (contiguous entries from start of array).</param>
		public virtual void LookupValues(double[] input, double[] output, int count)
		{
			LutFunctions.LookupLut(input, output, count, this);
		}

		internal override sealed double MinInputValueCore
		{
			get { return MinInputValue; }
			set { MinInputValue = value; }
		}

		internal override sealed double MaxInputValueCore
		{
			get { return MaxInputValue; }
			set { MaxInputValue = value; }
		}

		internal override sealed double MinOutputValueCore
		{
			get { return MinOutputValue; }
		}

		internal override sealed double MaxOutputValueCore
		{
			get { return MaxOutputValue; }
		}

		internal override sealed double LookupCore(double input)
		{
			return this[input];
		}

		internal override sealed void LookupCore(double[] input, double[] output, int count)
		{
			LookupValues(input, output, count);
		}
	}
}