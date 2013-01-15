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

namespace ClearCanvas.ImageViewer.Layout
{
	/// <summary>
	/// Represents the result of a HP applicability test.
	/// </summary>
	public struct HpApplicabilityResult : IEquatable<HpApplicabilityResult>
	{
		/// <summary>
		/// The <see cref="HpApplicabilityResult"/> value representing "not applicable".
		/// </summary>
		public static HpApplicabilityResult Negative = new HpApplicabilityResult(false, 0);

		/// <summary>
		/// The <see cref="HpApplicabilityResult"/> value representing neutral applicability (contributes nothing to the quality score).
		/// </summary>
		public static HpApplicabilityResult Neutral = new HpApplicabilityResult(true, 0);

		/// <summary>
		/// The <see cref="HpApplicabilityResult"/> value representing positive applicability (contributes 1 point to the quality score).
		/// </summary>
		public static HpApplicabilityResult Positive = new HpApplicabilityResult(true, 1);

		/// <summary>
		/// Computes a result representing the sum of the specified sequence of results.
		/// </summary>
		/// <param name="results"></param>
		/// <returns></returns>
		public static HpApplicabilityResult Sum(IEnumerable<HpApplicabilityResult> results)
		{
			var sum = Neutral;
			foreach (var score in results)
			{
				sum = sum + score;
			}
			return sum;
		}


		private readonly int _quality;
		private readonly bool _applicable;


		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="applicable"></param>
		/// <param name="quality"></param>
		private HpApplicabilityResult(bool applicable, int quality)
		{
			_applicable = applicable;
			_quality = quality;
		}

		/// <summary>
		/// Gets a value indicating whether this result represents a match or not.
		/// </summary>
		public bool IsApplicable
		{
			get { return _applicable; }
		}

		/// <summary>
		/// Gets a value indicating the quality of applicability (assuming that <see cref="IsApplicable"/> returns true.
		/// </summary>
		/// <exception cref="InvalidOperationException">If this property is invoked when <see cref="IsApplicable"/> returns false.</exception>
		public int Quality
		{
			get
			{
				if(!_applicable)
					throw new InvalidOperationException("A non-match does not have a quality value.");
				return _quality;
			}
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.
		///                 </param>
		public bool Equals(HpApplicabilityResult other)
		{
			// all non-matches are equal, regardless of score
			return _applicable == false ? other._applicable == false : _quality == other._quality;
		}

		/// <summary>
		/// Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <returns>
		/// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
		/// </returns>
		/// <param name="obj">Another object to compare to. 
		///                 </param><filterpriority>2</filterpriority>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (obj.GetType() != typeof(HpApplicabilityResult)) return false;
			return Equals((HpApplicabilityResult)obj);
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>
		/// A 32-bit signed integer that is the hash code for this instance.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override int GetHashCode()
		{
			unchecked
			{
				return _applicable == false ? _applicable.GetHashCode() : (_quality * 397) ^ _applicable.GetHashCode();
			}
		}

		/// <summary>
		/// Determines if two <see cref="HpApplicabilityResult"/> have the same value.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static bool operator ==(HpApplicabilityResult left, HpApplicabilityResult right)
		{
			return left.Equals(right);
		}

		/// <summary>
		/// Determines if two <see cref="HpApplicabilityResult"/> have a different value.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static bool operator !=(HpApplicabilityResult left, HpApplicabilityResult right)
		{
			return !left.Equals(right);
		}

		/// <summary>
		/// Computes an <see cref="HpApplicabilityResult"/> representing the sum of two <see cref="HpApplicabilityResult"/>.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static HpApplicabilityResult operator +(HpApplicabilityResult x, HpApplicabilityResult y)
		{
			return x.IsApplicable && y.IsApplicable ? new HpApplicabilityResult(true, x.Quality + y.Quality) : Negative;
		}
	}
}
