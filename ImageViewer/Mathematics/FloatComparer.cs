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
using System.Drawing;
using System.Runtime.InteropServices;

namespace ClearCanvas.ImageViewer.Mathematics
{
	/// <summary>
	/// A utility class to facilitate error-tolerant comparison of floating point values.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Floating point variables store decimal numbers as a significand (the significant
	/// figures part of the number) and an exponent. As a result, floating point arithmetic
	/// has the potential to introduce errors in computation due to the inability to
	/// represent certain values as a floating point variable.
	/// </para>
	/// <para>
	/// In order to compare floating point values in a robust manner, it is necessary to
	/// compare such values while allowing for a limited amount of error. The actual amount
	/// of acceptible tolernace that should be used will depend on the context of the computation.
	/// </para>
	/// <para>
	/// Throughout this class, tolerance may be expressed in terms of the number of ULPs
	/// ("Unit in Last Place"), a unit based on the minimum representable value of the significand.
	/// Alternatively, tolerance may also be expressed as an &quot;absolute&quot; value,
	/// in which case the equality comparision is carried out &#177; absolute tolerance.
	/// </para>
	/// <para>
	/// The comparison algorithms used in this class are based on the code presented
	/// at http://www.windojitsu.com/code/floatcomparer.html.
	/// </para>
	/// </remarks>
	public static class FloatComparer
	{
		#region Single-Precision Overloads

		/// <summary>
		/// Compares two floating point values within a specified tolerance in ULPs.
		/// </summary>
		/// <param name="x">One floating point value to be compared.</param>
		/// <param name="y">Other floating point value to be compared.</param>
		/// <param name="tolerance">The tolerance for equality in terms of ULPs.</param>
		/// <returns>0 if <paramref name="x"/> equals <paramref name="y"/> &#177; <paramref name="tolerance"/> ULPs; +1 if x &gt; y + tolerance ULPs; -1 if x &lt; y - tolerance ULPs.</returns>
		/// <remarks>
		/// <list>
		/// <item> 0 if x = y &#177; tolerance ULPs</item>
		/// <item>+1 if x &gt; y + tolerance ULPs</item>
		/// <item>-1 if x &lt; y - tolerance ULPs</item>
		/// </list>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="tolerance"/> is negative or in excess of 2<sup>22</sup> (the limit of the significand in a single-precision floating point value).</exception>
		public static int Compare(float x, float y, int tolerance)
		{
			int dummy;
			return Compare(x, y, tolerance, out dummy);
		}

		/// <summary>
		/// Compares two floating point values within a specified tolerance in ULPs.
		/// </summary>
		/// <param name="x">One floating point value to be compared.</param>
		/// <param name="y">Other floating point value to be compared.</param>
		/// <param name="tolerance">The tolerance for equality in terms of ULPs.</param>
		/// <param name="difference">The difference between the values in terms of ULPs.</param>
		/// <returns>0 if <paramref name="x"/> = <paramref name="y"/> &#177; <paramref name="tolerance"/> ULPs; +1 if x &gt; y + tolerance ULPs; -1 if x &lt; y - tolerance ULPs.</returns>
		/// <remarks>
		/// <list>
		/// <item> 0 if x = y &#177; tolerance ULPs</item>
		/// <item>+1 if x &gt; y + tolerance ULPs</item>
		/// <item>-1 if x &lt; y - tolerance ULPs</item>
		/// </list>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="tolerance"/> is negative or in excess of 2<sup>22</sup> (the limit of the significand in a single-precision floating point value).</exception>
		public static int Compare(float x, float y, int tolerance, out int difference)
		{
			// Make sure tolerance is non-negative and small enough that the
			// default NAN won't compare as equal to anything.
			const int maxSignificand = 4*1024*1024; // single-precision values has a 22-bit significand
			if (tolerance < 0 || tolerance >= maxSignificand)
				throw new ArgumentOutOfRangeException("tolerance", "Tolerance must be in the range [0,2\u00B2\u00B2)");

			// Reinterpret float bits as sign-magnitude integers.
			int xi = BitReinterpreter.Convert(x);
			int yi = BitReinterpreter.Convert(y);

			// Convert from sign-magnitude to two's complement form, 
			// by subtracting from 0x80000000.
			if (xi < 0)
				xi = int.MinValue - xi;
			if (yi < 0)
				yi = int.MinValue - yi;

			// How many epsilons apart?
			difference = xi - yi;

			// Is the difference outside our tolerance?
			if (xi > yi + tolerance)
				return +1;
			if (xi < yi - tolerance)
				return -1;
			else
				return 0;
		}

        // TODO (CR Feb 2013 - Med): This is the only method here that doesn't use ULPs - it's easy to assume it does.

		/// <summary>
		/// Compares two floating point values for equality within a specified absolute tolerance.
		/// </summary>
		/// <param name="x">One floating point value to be compared.</param>
		/// <param name="y">Other floating point value to be compared.</param>
		/// <param name="tolerance">The absolute tolerance.</param>
		/// <returns>True if <paramref name="x"/> = <paramref name="y"/> &#177; <paramref name="tolerance"/>; False otherwise.</returns>
		public static bool AreEqual(float x, float y, float tolerance)
		{
			return Math.Abs(x - y) < tolerance;
		}

		/// <summary>
		/// Compares two floating point values for equality within 100 ULPs.
		/// </summary>
		/// <param name="x">One floating point value to be compared.</param>
		/// <param name="y">Other floating point value to be compared.</param>
		/// <returns>True if <paramref name="x"/> = <paramref name="y"/> &#177; 100 ULPs; False otherwise.</returns>
		/// <remarks>A tolerance of 100 ULPs is assumed.</remarks>
		public static bool AreEqual(float x, float y)
		{
			int dummy;
			const int tolerance = 100;

			int result = Compare(x, y, tolerance, out dummy);
			return result == 0;
		}

		/// <summary>
		/// Compares two floating point values to see if <paramref name="x"/> is greater than <paramref name="y"/> within 100 ULPs.
		/// </summary>
		/// <param name="x">One floating point value to be compared.</param>
		/// <param name="y">Other floating point value to be compared.</param>
		/// <returns>True if <paramref name="x"/> &gt; <paramref name="y"/> + 100 ULPs; False otherwise.</returns>
		/// <remarks>A tolerance of 100 ULPs is assumed.</remarks>
		public static bool IsGreaterThan(float x, float y)
		{
			int dummy;
			const int tolerance = 100;

			int result = Compare(x, y, tolerance, out dummy);
			return result == 1;
		}

		/// <summary>
		/// Compares two floating point values to see if <paramref name="x"/> is less than <paramref name="y"/> within 100 ULPs.
		/// </summary>
		/// <param name="x">One floating point value to be compared.</param>
		/// <param name="y">Other floating point value to be compared.</param>
		/// <returns>True if <paramref name="x"/> &lt; <paramref name="y"/> - 100 ULPs; False otherwise.</returns>
		/// <remarks>A tolerance of 100 ULPs is assumed.</remarks>
		public static bool IsLessThan(float x, float y)
		{
			int dummy;
			const int tolerance = 100;

			int result = Compare(x, y, tolerance, out dummy);
			return result == -1;
		}

		#endregion

		#region Double-Precision Overloads

		/// <summary>
		/// Compares two floating point values within a specified tolerance in ULPs.
		/// </summary>
		/// <param name="x">One floating point value to be compared.</param>
		/// <param name="y">Other floating point value to be compared.</param>
		/// <param name="tolerance">The tolerance for equality in terms of ULPs.</param>
		/// <returns>0 if <paramref name="x"/> equals <paramref name="y"/> &#177; <paramref name="tolerance"/> ULPs; +1 if x &gt; y + tolerance ULPs; -1 if x &lt; y - tolerance ULPs.</returns>
		/// <remarks>
		/// <list>
		/// <item> 0 if x = y &#177; tolerance ULPs</item>
		/// <item>+1 if x &gt; y + tolerance ULPs</item>
		/// <item>-1 if x &lt; y - tolerance ULPs</item>
		/// </list>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="tolerance"/> is negative or in excess of 2<sup>51</sup> (the limit of the significand in a double-precision floating point value).</exception>
		public static int Compare(double x, double y, long tolerance)
		{
			long dummy;
			return Compare(x, y, tolerance, out dummy);
		}

		/// <summary>
		/// Compares two floating point values within a specified tolerance in ULPs.
		/// </summary>
		/// <param name="x">One floating point value to be compared.</param>
		/// <param name="y">Other floating point value to be compared.</param>
		/// <param name="tolerance">The tolerance for equality in terms of ULPs.</param>
		/// <param name="difference">The difference between the values in terms of ULPs.</param>
		/// <returns>0 if <paramref name="x"/> = <paramref name="y"/> &#177; <paramref name="tolerance"/> ULPs; +1 if x &gt; y + tolerance ULPs; -1 if x &lt; y - tolerance ULPs.</returns>
		/// <remarks>
		/// <list>
		/// <item> 0 if x = y &#177; tolerance ULPs</item>
		/// <item>+1 if x &gt; y + tolerance ULPs</item>
		/// <item>-1 if x &lt; y - tolerance ULPs</item>
		/// </list>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="tolerance"/> is negative or in excess of 2<sup>51</sup> (the limit of the significand in a double-precision floating point value).</exception>
		public static int Compare(double x, double y, long tolerance, out long difference)
		{
			// Make sure tolerance is non-negative and small enough that the
			// default NAN won't compare as equal to anything.
			const long maxSignificand = 2L*1024*1024*1024*1024*1024; // double-precision values has a 51-bit significand
			if (tolerance < 0 || tolerance >= maxSignificand)
				throw new ArgumentOutOfRangeException("tolerance", "Tolerance must be in the range [0,2\u2075\u00B9)");

			// Reinterpret double bits as sign-magnitude long integers.
			long xi = DoubleBitReinterpreter.Convert(x);
			long yi = DoubleBitReinterpreter.Convert(y);

			// Convert from sign-magnitude to two's complement form, 
			// by subtracting from 0x8000000000000000.
			if (xi < 0)
				xi = long.MinValue - xi;
			if (yi < 0)
				yi = long.MinValue - yi;

			// How many epsilons apart?
			difference = xi - yi;

			// Is the difference outside our tolerance?
			if (xi > yi + tolerance)
				return +1;
			if (xi < yi - tolerance)
				return -1;
			else
				return 0;
		}

	    // TODO (CR Feb 2013 - Med): This is the only method here that doesn't use ULPs - it's easy to assume it does.

		/// <summary>
		/// Compares two floating point values for equality within a specified absolute tolerance.
		/// </summary>
		/// <param name="x">One floating point value to be compared.</param>
		/// <param name="y">Other floating point value to be compared.</param>
		/// <param name="tolerance">The absolute tolerance.</param>
		/// <returns>True if <paramref name="x"/> = <paramref name="y"/> &#177; <paramref name="tolerance"/>; False otherwise.</returns>
		public static bool AreEqual(double x, double y, double tolerance)
		{
			return Math.Abs(x - y) < tolerance;
		}

		/// <summary>
		/// Compares two floating point values for equality within 100 ULPs.
		/// </summary>
		/// <param name="x">One floating point value to be compared.</param>
		/// <param name="y">Other floating point value to be compared.</param>
		/// <returns>True if <paramref name="x"/> = <paramref name="y"/> &#177; 100 ULPs; False otherwise.</returns>
		/// <remarks>A tolerance of 100 ULPs is assumed.</remarks>
		public static bool AreEqual(double x, double y)
		{
			long dummy;
			const int tolerance = 100;

			int result = Compare(x, y, tolerance, out dummy);
			return result == 0;
		}

		/// <summary>
		/// Compares two floating point values to see if <paramref name="x"/> is greater than <paramref name="y"/> within 100 ULPs.
		/// </summary>
		/// <param name="x">One floating point value to be compared.</param>
		/// <param name="y">Other floating point value to be compared.</param>
		/// <returns>True if <paramref name="x"/> &gt; <paramref name="y"/> + 100 ULPs; False otherwise.</returns>
		/// <remarks>A tolerance of 100 ULPs is assumed.</remarks>
		public static bool IsGreaterThan(double x, double y)
		{
			long dummy;
			const int tolerance = 100;

			int result = Compare(x, y, tolerance, out dummy);
			return result == 1;
		}

		/// <summary>
		/// Compares two floating point values to see if <paramref name="x"/> is less than <paramref name="y"/> within 100 ULPs.
		/// </summary>
		/// <param name="x">One floating point value to be compared.</param>
		/// <param name="y">Other floating point value to be compared.</param>
		/// <returns>True if <paramref name="x"/> &lt; <paramref name="y"/> - 100 ULPs; False otherwise.</returns>
		/// <remarks>A tolerance of 100 ULPs is assumed.</remarks>
		public static bool IsLessThan(double x, double y)
		{
			long dummy;
			const int tolerance = 100;

			int result = Compare(x, y, tolerance, out dummy);
			return result == -1;
		}

		#endregion

		#region PointF and SizeF Helpers

		/// <summary>
		/// Compares two <see cref="PointF"/> values for equality within 100 ULPs in each dimension.
		/// </summary>
		/// <param name="pt1">One <see cref="PointF"/> value to be compared.</param>
		/// <param name="pt2">Other <see cref="PointF"/> value to be compared.</param>
		/// <returns>True if <paramref name="pt1"/> = <paramref name="pt2"/> &#177; 100 ULPs for both <see cref="PointF.X"/> and <see cref="PointF.Y"/> dimensions separately; False otherwise.</returns>
		/// <remarks>A tolerance of 100 ULPs is assumed.</remarks>
		public static bool AreEqual(PointF pt1, PointF pt2)
		{
			int dummy;
			const int tolerance = 100;

			int xResult = Compare(pt1.X, pt2.X, tolerance, out dummy);
			int yResult = Compare(pt1.Y, pt2.Y, tolerance, out dummy);
			return xResult == 0 && yResult == 0;
		}

		/// <summary>
		/// Compares two <see cref="SizeF"/> values for equality within 100 ULPs in each dimension.
		/// </summary>
		/// <param name="size1">One <see cref="SizeF"/> value to be compared.</param>
		/// <param name="size2">Other <see cref="SizeF"/> value to be compared.</param>
		/// <returns>True if <paramref name="size1"/> = <paramref name="size2"/> &#177; 100 ULPs for both <see cref="SizeF.Width"/> and <see cref="SizeF.Height"/> dimensions separately; False otherwise.</returns>
		/// <remarks>A tolerance of 100 ULPs is assumed.</remarks>
		public static bool AreEqual(SizeF size1, SizeF size2)
		{
			int dummy;
			const int tolerance = 100;

			int xResult = Compare(size1.Width, size2.Width, tolerance, out dummy);
			int yResult = Compare(size1.Height, size2.Height, tolerance, out dummy);
			return xResult == 0 && yResult == 0;
		}

		#endregion

		#region Bit Reinterpreters

		[StructLayout(LayoutKind.Explicit)]
		private struct BitReinterpreter
		{
			public static int Convert(float f)
			{
				BitReinterpreter br = new BitReinterpreter(f);
				return br.i;
			}

			public static float Convert(int i)
			{
				BitReinterpreter br = new BitReinterpreter(i);
				return br.f;
			}

			[FieldOffset(0)]
			private float f;

			[FieldOffset(0)]
			private int i;

			private BitReinterpreter(float f)
			{
				this.i = 0;
				this.f = f;
			}

			private BitReinterpreter(int i)
			{
				this.f = 0;
				this.i = i;
			}
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct DoubleBitReinterpreter
		{
			[FieldOffset(0)]
			private readonly double _dValue;

			[FieldOffset(0)]
			private readonly long _lValue;

			private DoubleBitReinterpreter(double value)
			{
				_lValue = 0;
				_dValue = value;
			}

			private DoubleBitReinterpreter(long value)
			{
				_dValue = 0;
				_lValue = value;
			}

			public static long Convert(double value)
			{
				var dbr = new DoubleBitReinterpreter(value);
				return dbr._lValue;
			}

			public static double Convert(long value)
			{
				var dbr = new DoubleBitReinterpreter(value);
				return dbr._dValue;
			}
		}

#if UNIT_TESTS

		internal static int Convert(float value)
		{
			return BitReinterpreter.Convert(value);
		}

		internal static float Convert(int value)
		{
			return BitReinterpreter.Convert(value);
		}

		internal static long Convert(double value)
		{
			return DoubleBitReinterpreter.Convert(value);
		}

		internal static double Convert(long value)
		{
			return DoubleBitReinterpreter.Convert(value);
		}

#endif

		#endregion
	}
}