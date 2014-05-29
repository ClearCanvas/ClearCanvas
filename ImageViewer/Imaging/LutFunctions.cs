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

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Methods for batch computing various LUTs.
	/// </summary>
	public static unsafe class LutFunctions
	{
		private static int CheckArraySize(Array input, Array output, int count)
		{
			var countValues = input.Length;
			if (countValues != output.Length)
			{
				const string msg = "Size of output array must be equal to the size of input array.";
				throw new ArgumentException(msg, "output");
			}
			if (count > countValues)
			{
				const string msg = "Count must be less than or equal to size of arrays.";
				throw new ArgumentOutOfRangeException("count", msg);
			}
			return count;
		}

		/// <summary>
		/// Performs batch LUT value lookup by applying a slope-intercept linear function.
		/// </summary>
		/// <remarks>
		/// <para>The values are transformed according to the following function:</para>
		/// <code>
		/// output = input*slope + intercept
		/// </code>
		/// </remarks>
		/// <param name="input">Source array of input values to be transformed.</param>
		/// <param name="output">Destination array where transformed output values will be written (may be same array as <paramref name="input"/>).</param>
		/// <param name="count">Number of values in the arrays to transform (contiguous entries from start of array).</param>
		/// <param name="slope">The slope of the linear function.</param>
		/// <param name="intercept">The intercept of the linear function.</param>
		public static void LookupLinear(double[] input, double[] output, int count, double slope, double intercept)
		{
			var countValues = CheckArraySize(input, output, count);

			fixed (double* pInput = input)
			fixed (double* pOutput = output)
			{
				var pIn = pInput;
				var pOut = pOutput;

				for (var n = 0; n < countValues; ++n)
					*pOut++ = *pIn++*slope + intercept;
			}
		}

		/// <summary>
		/// Performs batch LUT value lookup by applying the inverse of a slope-intercept linear function.
		/// </summary>
		/// <remarks>
		/// <para>The values are transformed according to the following function:</para>
		/// <code>
		/// output = (input - intercept)/slope
		/// </code>
		/// </remarks>
		/// <param name="input">Source array of input values to be transformed.</param>
		/// <param name="output">Destination array where transformed output values will be written (may be same array as <paramref name="input"/>).</param>
		/// <param name="count">Number of values in the arrays to transform (contiguous entries from start of array).</param>
		/// <param name="slope">The slope of the linear function.</param>
		/// <param name="intercept">The intercept of the linear function.</param>
		public static void LookupLinearInverse(double[] input, double[] output, int count, double slope, double intercept)
		{
			var countValues = CheckArraySize(input, output, count);

			fixed (double* pInput = input)
			fixed (double* pOutput = output)
			{
				var pIn = pInput;
				var pOut = pOutput;

				for (var n = 0; n < countValues; ++n)
					*pOut++ = (*pIn++ - intercept)/slope;
			}
		}

		/// <summary>
		/// Performs batch LUT value lookup by multiplying the input by a scalar.
		/// </summary>
		/// <remarks>
		/// <para>The values are transformed according to the following function:</para>
		/// <code>
		/// output = input*scalar
		/// </code>
		/// </remarks>
		/// <param name="input">Source array of input values to be transformed.</param>
		/// <param name="output">Destination array where transformed output values will be written (may be same array as <paramref name="input"/>).</param>
		/// <param name="count">Number of values in the arrays to transform (contiguous entries from start of array).</param>
		/// <param name="scalar">The scalar applied to each input value.</param>
		public static void LookupScaleValue(double[] input, double[] output, int count, double scalar)
		{
			var countValues = CheckArraySize(input, output, count);

			fixed (double* pInput = input)
			fixed (double* pOutput = output)
			{
				var pIn = pInput;
				var pOut = pOutput;

				for (var n = 0; n < countValues; ++n)
					*pOut++ = *pIn++*scalar;
			}
		}

		/// <summary>
		/// Performs batch LUT value lookup against a black box <see cref="IComposableLut"/> implementation.
		/// </summary>
		/// <remarks>
		/// This method just calls the <see cref="IComposableLut.this"/> indexer in a tight loop, which will
		/// generally not provide optimal performance. It is recommended that a custom implementation be used
		/// instead of calling this method in order to take advantage of the knowledge of the lookup's
		/// internal details.
		/// </remarks>
		/// <param name="input">Source array of input values to be transformed.</param>
		/// <param name="output">Destination array where transformed output values will be written (may be same array as <paramref name="input"/>).</param>
		/// <param name="count">Number of values in the arrays to transform (contiguous entries from start of array).</param>
		/// <param name="lut">The <see cref="IComposableLut"/> with which to transform the input values.</param>
		public static void LookupLut(double[] input, double[] output, int count, IComposableLut lut)
		{
			var countValues = CheckArraySize(input, output, count);

			fixed (double* pInput = input)
			fixed (double* pOutput = output)
			{
				var pIn = pInput;
				var pOut = pOutput;

				for (var n = 0; n < countValues; ++n)
					*pOut++ = lut[*pIn++];
			}
		}

		/// <summary>
		/// Performs batch LUT value lookup against the specified lookup table data.
		/// </summary>
		/// <remarks>
		/// <para>The values are transformed according to the following algorithm:</para>
		/// <code>
		/// if input &lt;= firstMappedInputValue then<br/>
		///		output = lutData[0]<br/>
		/// else if input &gt;= lastMappedInputValue then<br/>
		///		output = lutData[lutData.Length - 1]<br/>
		/// else<br/>
		///		output = lutData[input - firstMappedInputValue]
		/// </code>
		/// </remarks>
		/// <param name="input">Source array of input values to be transformed (values are rounded to nearest integer).</param>
		/// <param name="output">Destination array where transformed output values will be written (may be same array as <paramref name="input"/>).</param>
		/// <param name="count">Number of values in the arrays to transform (contiguous entries from start of array).</param>
		///<param name="lutData">LUT table mapping input values to output values.</param>
		///<param name="firstMappedInputValue">The input value corresponding to the first element of <paramref name="lutData"/>.</param>
		///<param name="lastMappedInputValue">The input value corresponding to the last element of <paramref name="lutData"/>.</param>
		public static void LookupLut(double[] input, double[] output, int count, int[] lutData, int firstMappedInputValue, int lastMappedInputValue)
		{
			var countValues = CheckArraySize(input, output, count);

			var mappedValues = lutData.Length;
			if (mappedValues != lastMappedInputValue - firstMappedInputValue + 1)
				throw new ArgumentException("Size of LUT array must be equal to the specified range of mapped input values.");

			fixed (double* pInput = input)
			fixed (double* pOutput = output)
			fixed (int* pLut = lutData)
			{
				var pIn = pInput;
				var pOut = pOutput;

				for (var n = 0; n < countValues; ++n)
				{
					var value = (int) Math.Round(*pIn++);
					if (value <= firstMappedInputValue)
						*pOut++ = pLut[0];
					else if (value >= lastMappedInputValue)
						*pOut++ = pLut[mappedValues - 1];
					else
						*pOut++ = pLut[value - firstMappedInputValue];
				}
			}
		}

		/// <summary>
		/// Performs batch LUT value lookup against the specified lookup table data.
		/// </summary>
		/// <remarks>
		/// <para>The values are transformed according to the following algorithm:</para>
		/// <code>
		/// if input &lt;= firstMappedInputValue then<br/>
		///		output = lutData[0]<br/>
		/// else if input &gt;= lastMappedInputValue then<br/>
		///		output = lutData[lutData.Length - 1]<br/>
		/// else<br/>
		///		output = lutData[input - firstMappedInputValue]
		/// </code>
		/// </remarks>
		/// <param name="input">Source array of input values to be transformed (values are rounded to nearest integer).</param>
		/// <param name="output">Destination array where transformed output values will be written (may be same array as <paramref name="input"/>).</param>
		/// <param name="count">Number of values in the arrays to transform (contiguous entries from start of array).</param>
		///<param name="lutData">LUT table mapping input values to output values.</param>
		///<param name="firstMappedInputValue">The input value corresponding to the first element of <paramref name="lutData"/>.</param>
		///<param name="lastMappedInputValue">The input value corresponding to the last element of <paramref name="lutData"/>.</param>
		public static void LookupLut(double[] input, double[] output, int count, double[] lutData, int firstMappedInputValue, int lastMappedInputValue)
		{
			var countValues = CheckArraySize(input, output, count);

			var mappedValues = lutData.Length;
			if (mappedValues != lastMappedInputValue - firstMappedInputValue + 1)
				throw new ArgumentException("Size of LUT array must be equal to the specified range of mapped input values.");

			fixed (double* pInput = input)
			fixed (double* pOutput = output)
			fixed (double* pLut = lutData)
			{
				var pIn = pInput;
				var pOut = pOutput;

				for (var n = 0; n < countValues; ++n)
				{
					var value = (int) Math.Round(*pIn++);
					if (value <= firstMappedInputValue)
						*pOut++ = pLut[0];
					else if (value >= lastMappedInputValue)
						*pOut++ = pLut[mappedValues - 1];
					else
						*pOut++ = pLut[value - firstMappedInputValue];
				}
			}
		}

		/// <summary>
		/// Performs batch LUT value lookup by applying a linear rescale of the input range to the output range.
		/// </summary>
		/// <remarks>
		/// <para>The values are transformed according to the following algorithm:</para>
		/// <code>
		/// if input &lt;= minInputValue then<br/>
		///		output = minOutputValue<br/>
		/// else if input &gt;= maxInputValue then<br/>
		///		output = maxOutputValue<br/>
		/// else<br/>
		///		output = Round((input - minInputValue)*(maxOutputValue - minOutputValue)/(maxInputValue - minInputValue) + minOutputValue)
		/// </code>
		/// </remarks>
		/// <param name="input">Source array of input values to be transformed.</param>
		/// <param name="output">Destination array where transformed output values will be written (may be same array as <paramref name="input"/>).</param>
		/// <param name="count">Number of values in the arrays to transform (contiguous entries from start of array).</param>
		/// <param name="minInputValue">Specifies the lower bound of the range of input values.</param>
		/// <param name="maxInputValue">Specifies the upper bound of the range of input values.</param>
		/// <param name="minOutputValue">Specifies the lower bound of the range of output values.</param>
		/// <param name="maxOutputValue">Specifies the upper bound of the range of output values.</param>
		public static void LookupLinearRescale(double[] input, double[] output, int count, double minInputValue, double maxInputValue, int minOutputValue, int maxOutputValue)
		{
			var countValues = CheckArraySize(input, output, count);

			var inputRange = maxInputValue - minInputValue;
			double outputRange = maxOutputValue - minOutputValue;

			fixed (double* pInput = input)
			fixed (double* pOutput = output)
			{
				var pIn = pInput;
				var pOut = pOutput;

				for (var n = 0; n < countValues; ++n)
				{
					var value = *pIn++;
					if (value <= minInputValue)
						*pOut++ = minOutputValue;
					else if (value >= maxInputValue)
						*pOut++ = maxOutputValue;
					else
					{
						var scale = (value - minInputValue)/inputRange;
						*pOut++ = Math.Min(maxOutputValue, Math.Max(minOutputValue, (int) Math.Round(scale*outputRange + minOutputValue)));
					}
				}
			}
		}

		/// <summary>
		/// Performs batch LUT value lookup by applying a linear VOI window rescaling.
		/// </summary>
		/// <remarks>
		/// <para>The values are transformed according to the VOI window algorithm defined in DICOM 2011 PS 3.3 C.11.2.1.2:</para>
		/// <code>
		/// if input &lt;= center - 0.5 - (width - 1)/2 then<br/>
		///		output = minOutputValue<br/>
		/// else if input &gt; center - 0.5 + (width - 1)/2 then<br/>
		///		output = maxOutputValue<br/>
		/// else<br/>
		///		output = ((input - (center - 0.5))/(width - 1) + 0.5)*(maxOutputValue - minOutputValue) + minOutputValue
		/// </code>
		/// </remarks>
		/// <param name="input">Source array of input values to be transformed.</param>
		/// <param name="output">Destination array where transformed output values will be written (may be same array as <paramref name="input"/>).</param>
		/// <param name="count">Number of values in the arrays to transform (contiguous entries from start of array).</param>
		/// <param name="windowCenter">The centre value of the VOI window.</param>
		/// <param name="windowWidth">The width of the VOI window.</param>
		/// <param name="minOutputValue">Specifies the lower bound of the range of output values.</param>
		/// <param name="maxOutputValue">Specifies the upper bound of the range of output values.</param>
		public static void LookupVoiWindowLinear(double[] input, double[] output, int count, double windowCenter, double windowWidth, double minOutputValue, double maxOutputValue)
		{
			var countValues = CheckArraySize(input, output, count);

			var halfWindow = (windowWidth - 1)/2;
			var windowStart = windowCenter - 0.5 - halfWindow;
			var windowEnd = windowCenter - 0.5 + halfWindow;
			var outputRange = maxOutputValue - minOutputValue;

			fixed (double* pInput = input)
			fixed (double* pOutput = output)
			{
				var pIn = pInput;
				var pOut = pOutput;

				for (var n = 0; n < countValues; ++n)
				{
					var value = *pIn++;
					if (value <= windowStart)
						*pOut++ = minOutputValue;
					else if (value > windowEnd)
						*pOut++ = maxOutputValue;
					else
					{
						var scale = ((value - (windowCenter - 0.5))/(windowWidth - 1)) + 0.5;
						*pOut++ = Math.Min(maxOutputValue, Math.Max(minOutputValue, scale*outputRange + minOutputValue));
					}
				}
			}
		}

		/// <summary>
		/// Performs batch LUT value lookup by clamping the output range, but otherwise passing values through without transformation.
		/// </summary>
		/// <remarks>
		/// <para>The values are transformed according to the following algorithm:</para>
		/// <code>
		/// if input &lt; minValue then
		///		output = minValue
		/// else if input &gt; maxValue then
		///		output = maxValue
		/// else
		///		output = input
		/// </code>
		/// </remarks>
		/// <param name="input">Source array of input values to be transformed.</param>
		/// <param name="output">Destination array where transformed output values will be written (may be same array as <paramref name="input"/>).</param>
		/// <param name="count">Number of values in the arrays to transform (contiguous entries from start of array).</param>
		/// <param name="minValue">Specifies the lower bound of the range of values.</param>
		/// <param name="maxValue">Specifies the upper bound of the range of values.</param>
		public static void LookupClampValue(double[] input, double[] output, int count, double minValue, double maxValue)
		{
			var countValues = CheckArraySize(input, output, count);

			fixed (double* pInput = input)
			fixed (double* pOutput = output)
			{
				var pIn = pInput;
				var pOut = pOutput;

				for (var n = 0; n < countValues; ++n)
				{
					var value = *pIn++;
					if (value < minValue)
						*pOut++ = minValue;
					else if (value > maxValue)
						*pOut++ = maxValue;
					else
						*pOut++ = value;
				}
			}
		}
	}
}