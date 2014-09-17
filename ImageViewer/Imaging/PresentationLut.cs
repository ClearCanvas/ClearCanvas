#region License

// Copyright (c) 2014, ClearCanvas Inc.
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
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Imaging
{
	[Cloneable(true)]
	public abstract class PresentationLut : ComposableLutBase, IPresentationLut
	{
		private double _minInputValue = double.MinValue;
		private double _maxInputValue = double.MaxValue;
		private int _minOutputValue = int.MinValue;
		private int _maxOutputValue = int.MaxValue;

		protected PresentationLut() {}

		protected PresentationLut(int minOutputValue, int maxOutputValue)
		{
			_minOutputValue = minOutputValue;
			_maxOutputValue = maxOutputValue;
		}

		#region Overrides of ComposableLutBase

		internal override sealed double MinInputValueCore
		{
			get { return _minInputValue; }
			set
			{
				if (FloatComparer.AreEqual(_minInputValue, value))
					return;

				_minInputValue = value;
				OnLutChanged();
			}
		}

		internal override sealed double MaxInputValueCore
		{
			get { return _maxInputValue; }
			set
			{
				if (FloatComparer.AreEqual(_maxInputValue, value))
					return;

				_maxInputValue = value;
				OnLutChanged();
			}
		}

		internal override sealed double MinOutputValueCore
		{
			get { return _minOutputValue; }
		}

		internal override sealed double MaxOutputValueCore
		{
			get { return _maxOutputValue; }
		}

		internal override sealed double LookupCore(double input)
		{
			return this[input];
		}

		internal override sealed void LookupCore(double[] input, double[] output, int count)
		{
			LookupValues(input, output, count);
		}

		#endregion

		#region IPresentationLut Members

		public double MinInputValue
		{
			get { return MinInputValueCore; }
			set { MinInputValueCore = value; }
		}

		public double MaxInputValue
		{
			get { return MaxInputValueCore; }
			set { MaxInputValueCore = value; }
		}

		public int MaxOutputValue
		{
			get { return _maxOutputValue; }
			set
			{
				if (FloatComparer.AreEqual(_maxOutputValue, value))
					return;

				_maxOutputValue = value;
				OnLutChanged();
			}
		}

		public int MinOutputValue
		{
			get { return _minOutputValue; }
			set
			{
				if (FloatComparer.AreEqual(_minOutputValue, value))
					return;

				_minOutputValue = value;
				OnLutChanged();
			}
		}

		public abstract int this[double input] { get; }

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

		public new IPresentationLut Clone()
		{
			return base.Clone() as IPresentationLut;
		}

		#endregion
	}
}