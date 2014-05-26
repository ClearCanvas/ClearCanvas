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
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion.Utilities
{
	[Cloneable]
	internal sealed class InvertedLinearLut : ComposableLut
	{
		[CloneIgnore]
		private readonly double _rescaleSlope;

		[CloneIgnore]
		private readonly double _rescaleIntercept;

		/// <summary>
		/// Initializes a new instance of <see cref="NormalizationLutLinear"/>.
		/// </summary>
		/// <param name="rescaleSlope">Original frame rescale slope to be inverted.</param>
		/// <param name="rescaleIntercept">Original frame rescale intercept to be inverted.</param>
		public InvertedLinearLut(double rescaleSlope, double rescaleIntercept)
		{
			_rescaleSlope = rescaleSlope;
			_rescaleIntercept = rescaleIntercept;
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		private InvertedLinearLut(InvertedLinearLut source, ICloningContext context)
		{
			_rescaleSlope = source._rescaleSlope;
			_rescaleIntercept = source._rescaleIntercept;
		}

		public override double MinInputValue { get; set; }

		public override double MaxInputValue { get; set; }

		public override double MinOutputValue
		{
			get { return this[MinInputValue]; }
			protected set { throw new NotSupportedException(); }
		}

		public override double MaxOutputValue
		{
			get { return this[MaxInputValue]; }
			protected set { throw new NotSupportedException(); }
		}

		public override double this[double input]
		{
			get { return (input - _rescaleIntercept)/_rescaleSlope; }
		}

		public override void LookupValues(double[] input, double[] output, int count)
		{
			LutFunctions.LookupLinearInverse(input, output, count, _rescaleSlope, _rescaleIntercept);
		}

		public override string GetKey()
		{
			return string.Format(@"NormInverse_{0}_{1}", _rescaleSlope, _rescaleIntercept);
		}

		public override string GetDescription()
		{
			return string.Format(@"Normalization Function Inverse of m={0:0.#} b={1:0.#}", _rescaleSlope, _rescaleIntercept);
		}
	}
}