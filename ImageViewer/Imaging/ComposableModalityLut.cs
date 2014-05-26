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
	/// <summary>
	/// Abstract base implementation for a lookup table in the standard grayscale image display pipeline that transforms stored pixel values to manufacturer-independent values.
	/// </summary>
	/// <seealso cref="LutComposer"/>
	/// <seealso cref="IModalityLut"/>
	[Cloneable(true)]
	public abstract class ComposableModalityLut : ComposableLutBase, IModalityLut
	{
		public abstract int MinInputValue { get; set; }
		public abstract int MaxInputValue { get; set; }
		public abstract double MinOutputValue { get; protected set; }
		public abstract double MaxOutputValue { get; protected set; }
		public abstract double this[int input] { get; }

		/// <summary>
		/// Performs bulk lookup for a set of input values.
		/// </summary>
		/// <param name="input">Source array of input values to be transformed.</param>
		/// <param name="output">Destination array where transformed output values will be written (may be same array as <paramref name="input"/>).</param>
		/// <param name="count">Number of values in the arrays to transform (contiguous entries from start of array).</param>
		protected virtual void LookupValues(double[] input, double[] output, int count)
		{
			LutFunctions.LookupLut(input, output, count, this);
		}

		internal override sealed double MinInputValueCore
		{
			get { return MinInputValue; }
			set { MinInputValue = (int) Math.Round(value); }
		}

		internal override sealed double MaxInputValueCore
		{
			get { return MaxInputValue; }
			set { MaxInputValue = (int) Math.Round(value); }
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
			return this[(int) Math.Round(input)];
		}

		internal override sealed void LookupCore(double[] input, double[] output, int count)
		{
			LookupValues(input, output, count);
		}

		public new IModalityLut Clone()
		{
			return (IModalityLut) base.Clone();
		}
	}
}