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
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// An implementation of a VOI LUT that represents the identity function for integer inputs between <see cref="MinInputValue"/> and <see cref="MaxInputValue"/>.
	/// </summary>
	[Cloneable(true)]
	public sealed class IdentityVoiLinearLut : ComposableVoiLut, IVoiLutLinear
	{
		/// <summary>
		/// Initializes a new instance of <see cref="IdentityVoiLinearLut"/>.
		/// </summary>
		public IdentityVoiLinearLut()
		{
			MinInputValue = double.MinValue;
			MaxInputValue = double.MaxValue;
		}

#if UNIT_TESTS

		/// <summary>
		/// Constructor for unit testing
		/// </summary>
		/// <remarks>
		/// This overload is just for convenience, as all it does is initialize <see cref="MaxInputValue"/> and <see cref="MinInputValue"/>.
		/// In the actual image display pipeline, the input range would be automatically set to the output range of the modality LUT.
		/// </remarks>
		internal IdentityVoiLinearLut(int bitsStored, bool signed)
		{
			MinInputValue = DicomPixelData.GetMinPixelValue(bitsStored, signed);
			MaxInputValue = DicomPixelData.GetMaxPixelValue(bitsStored, signed);
		}

#endif

		/// <summary>
		/// Gets an abbreviated description of the lookup table.
		/// </summary>
		public override string GetDescription()
		{
			return string.Format(SR.FormatDescriptionIdentityVoiLinearLut, WindowWidth, WindowCenter);
		}

		/// <summary>
		/// Gets or sets the minimum input value.
		/// </summary>
		/// <remarks>
		/// This value is set internally by the framework and should not be modified by client code.
		/// </remarks>
		public override double MinInputValue { get; set; }

		/// <summary>
		/// Gets or sets the maximum input value.
		/// </summary>
		/// <remarks>
		/// This value is set internally by the framework and should not be modified by client code.
		/// </remarks>
		public override double MaxInputValue { get; set; }

		/// <summary>
		/// Gets the minimum output value.
		/// </summary>
		/// <remarks>
		/// This will always return <see cref="MinInputValue"/> rounded to an integer.
		/// </remarks>
		public override double MinOutputValue
		{
			get { return MinInputValue; }
			protected set { throw new NotSupportedException(); }
		}

		/// <summary>
		/// Gets the maximum output value.
		/// </summary>
		/// <remarks>
		/// This will always return <see cref="MaxInputValue"/> rounded to an integer.
		/// </remarks>
		public override double MaxOutputValue
		{
			get { return MaxInputValue; }
			protected set { throw new NotSupportedException(); }
		}

		/// <summary>
		/// Gets the output value of the lookup table for a given input value.
		/// </summary>
		public override double this[double input]
		{
			get
			{
				if (input < MinInputValue)
					return MinOutputValue;
				if (input > MaxInputValue)
					return MaxOutputValue;
				return input;
			}
		}

		public override void LookupValues(double[] input, double[] output, int count)
		{
			LutFunctions.LookupClampValue(input, output, count, MinInputValue, MaxInputValue);
		}

		/// <summary>
		/// Gets the window width.
		/// </summary>
		/// <remarks>
		/// This value is always exactly <see cref="MaxInputValue"/>-<see cref="MinInputValue"/>+1.
		/// </remarks>
		public double WindowWidth
		{
			get { return MaxInputValue - MinInputValue + 1; }
		}

		/// <summary>
		/// Gets the window centre.
		/// </summary>
		/// <remarks>
		/// This value is always exactly (<see cref="MaxInputValue"/>-<see cref="MinInputValue"/>+1)/2.
		/// </remarks>
		public double WindowCenter
		{
			get { return WindowWidth/2; }
		}

		/// <summary>
		/// Gets a string key that identifies this particular lookup table's characteristics.
		/// </summary>
		/// <remarks>
		/// This method is not to be confused with <b>equality</b>, since some lookup tables can be
		/// dependent upon the actual image to which it belongs.
		/// </remarks>
		public override string GetKey()
		{
			return string.Format("IDENTITY_{0}_to_{1}", MinInputValue, MaxInputValue);
		}
	}
}