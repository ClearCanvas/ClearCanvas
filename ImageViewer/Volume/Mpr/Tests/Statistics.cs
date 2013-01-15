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
using System.Collections.Generic;
using ClearCanvas.ImageViewer.Mathematics;

#if	UNIT_TESTS
#pragma warning disable 1591,0419,1574,1587

namespace ClearCanvas.ImageViewer.Volume.Mpr.Tests
{
	internal class Statistics
	{
		public readonly double Mean;
		public readonly double StandardDeviation;
		public readonly double MaxValue;
		public readonly double MinValue;

		public Statistics(IEnumerable<double> data)
		{
			this.Mean = CalculateMean(data);
			this.StandardDeviation = CalculateStandardDeviation(this.Mean, data);
			this.MaxValue = double.MaxValue;
			this.MinValue = double.MinValue;
		}

		public Statistics(IEnumerable<int> data)
		{
			this.Mean = CalculateMean(Enumerate(data));
			this.StandardDeviation = CalculateStandardDeviation(this.Mean, Enumerate(data));
			this.MaxValue = int.MaxValue;
			this.MinValue = int.MinValue;
		}

		public bool IsEqualTo(double value)
		{
			if (FloatComparer.AreEqual((float) this.Mean, (float) value))
			{
				if (FloatComparer.AreEqual((float) this.StandardDeviation, 0))
					return true;
			}
			return false;
		}

		public bool IsEqualTo(double value, double tolerance)
		{
			if (FloatComparer.AreEqual((float) this.Mean, (float) value, (float) tolerance))
			{
				if (FloatComparer.AreEqual((float) this.StandardDeviation, 0, (float) tolerance))
					return true;
			}
			return false;
		}

		public override string ToString()
		{
			return string.Format("\u03BC={0:f3}  \u03C3={1:f3}", this.Mean, this.StandardDeviation);
		}

		private static IEnumerable<double> Enumerate(IEnumerable<int> data)
		{
			foreach (int value in data)
				yield return value;
		}

		private static double CalculateMean(IEnumerable<double> data)
		{
			double sum = 0;
			int count = 0;

			foreach (double value in data)
			{
				count++;
				sum += value;
			}

			if (count == 0)
				return 0;

			return sum/count;
		}

		private static double CalculateStandardDeviation(double mean, IEnumerable<double> data)
		{
			double sum = 0;
			int count = 0;

			foreach (double value in data)
			{
				count++;
				double deviation = value - mean;
				sum += deviation*deviation;
			}

			if (count == 0)
				return 0;

			return Math.Sqrt(sum/count);
		}
	}
}

#endif