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
using ClearCanvas.Common;
using ClearCanvas.Dicom.Validation;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Implements the DICOM concept of a Modality LUT.
	/// </summary>
	/// <remarks>
	/// For efficiency's sake, <see cref="ModalityLutLinear"/> is implemented as a <see cref="GeneratedDataLut"/>
	/// although it could also be a purely calculated Lut.
	/// for more information.
	/// </remarks>
	internal sealed class ModalityLutLinear : GeneratedDataModalityLut
	{
		#region Private Fields

		private readonly int _bitsStored;
		private readonly bool _isSigned;
		private double _rescaleSlope;
		private double _rescaleIntercept;

		#endregion

		/// <summary>
		/// Initializes a new instance of <see cref="ModalityLutLinear"/> with the specified parameters.
		/// </summary>
		/// <param name="bitsStored">Indicates the number of bits stored of the associated pixel data.</param>
		/// <param name="isSigned">Indicates whether or not the associated pixel data is signed.</param>
		/// <param name="rescaleSlope">The rescale slope.</param>
		/// <param name="rescaleIntercept">The rescale intercept.</param>
		public ModalityLutLinear(
			int bitsStored,
			bool isSigned, 
			double rescaleSlope,
			double rescaleIntercept)
			: base()
		{
			DicomValidator.ValidateBitsStored(bitsStored);

			_bitsStored = bitsStored;
			_isSigned = isSigned;
			this.RescaleSlope = rescaleSlope;
			this.RescaleIntercept = rescaleIntercept;

			Initialize();
		}

		#region Private Members
		#region Properties

		private int BitsStored
		{
			get { return _bitsStored; }
		}

		private bool IsSigned
		{
			get { return _isSigned; }
		}

		private double RescaleSlope
		{
			get { return _rescaleSlope; }
			set
			{
				if (value <= double.Epsilon || double.IsNaN(value))
					_rescaleSlope = 1;
				else
					_rescaleSlope = value;
			}
		}

		private double RescaleIntercept
		{
			get { return _rescaleIntercept; }
			set
			{
				if (double.IsNaN(value))
					_rescaleIntercept = 0;
				else
					_rescaleIntercept = value;
			}
		}

		#endregion
		#endregion

		#region Overrides

		public override int MinInputValue
		{
			get { return base.MinInputValue; }
			set { }
		}

		public override int MaxInputValue
		{
			get { return base.MaxInputValue; }
			set { }
		}

		public override double MinOutputValue
		{
			get { return base.MinOutputValue; }
			protected set { }
		}

		public override double MaxOutputValue
		{
			get { return base.MaxOutputValue; }
			protected set { }
		}

		#region Methods

		protected override void Fill(double[] data, int firstMappedPixelValue, int lastMappedPixelValue)
		{
			unsafe
			{
				fixed (double* pData = data)
				{
					var length = data.Length;
					for (int i = 0; i < length; ++i)
						pData[i] = _rescaleSlope*(i + firstMappedPixelValue) + _rescaleIntercept;
				}
			}
		}

		public override string GetKey()
		{
			return String.Format("{0}_{1}_{2}_{3:F2}",
				this.BitsStored,
				this.IsSigned.ToString(),
				this.RescaleSlope,
				this.RescaleIntercept);
		}

		public override string GetDescription()
		{
			return String.Format(SR.FormatDescriptionModalityLutLinear, _rescaleSlope, _rescaleIntercept);
		}

		#endregion

		private void Initialize()
		{
			if (this.IsSigned)
			{
				base.MinInputValue = -(1 << (this.BitsStored - 1));
				base.MaxInputValue = (1 << (this.BitsStored - 1)) - 1;
			}
			else
			{
				base.MinInputValue = 0;
				base.MaxInputValue = (1 << this.BitsStored) - 1;
			}

			var minMax1 = (this.RescaleSlope * this.MinInputValue + this.RescaleIntercept);
			var minMax2 = (this.RescaleSlope * this.MaxInputValue + this.RescaleIntercept);

			base.MinOutputValue = Math.Min(minMax1, minMax2);
			base.MaxOutputValue = Math.Max(minMax1, minMax2);
		}

		#endregion
	}
}