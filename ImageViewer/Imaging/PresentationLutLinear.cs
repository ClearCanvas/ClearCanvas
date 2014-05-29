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

using System;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Imaging
{
	[Cloneable(true)]
	public class PresentationLutLinear : PresentationLut
	{
		public PresentationLutLinear() {}

		public PresentationLutLinear(int minOutputValue, int maxOutputValue)
			: base(minOutputValue, maxOutputValue) {}

		public override int this[double value]
		{
			get
			{
				//Rather than accessing the methods repeatedly.
				var minInput = MinInputValue;
				var maxInput = MaxInputValue;
				var minOutput = MinOutputValue;
				var maxOutput = MaxOutputValue;

				//Optimization.
				if (value <= minInput)
					return minOutput;
				if (value >= maxInput)
					return maxOutput;

				var inputRange = maxInput - minInput;
				double outputRange = maxOutput - minOutput;
				return Math.Min(maxOutput, Math.Max(minOutput, (int) Math.Round(outputRange*(value - minInput)/inputRange + minOutput)));
			}
		}

		protected override void LookupValues(double[] input, double[] output, int count)
		{
			LutFunctions.LookupLinearRescale(input, output, count, MinInputValue, MaxInputValue, MinOutputValue, MaxOutputValue);
		}

		public override string GetKey()
		{
			return String.Format("{0}_{1}_{2}_{3}_{4}", GetType(), MinInputValue, MaxInputValue, MinOutputValue, MaxOutputValue);
		}

		public override string GetDescription()
		{
			return SR.DescriptionPresentationLut;
		}
	}
}