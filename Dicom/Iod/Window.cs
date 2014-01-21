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
using System.Linq;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Dicom.Iod
{
	/// <summary>
	/// Represents a window centre/width pair, with optional accompanying descriptive explanation.
	/// </summary>
	public sealed class Window : IEquatable<Window>
	{
		#region Private Members

		private readonly double _width;
		private readonly double _center;
		private readonly string _explanation;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of <see cref="Window"/> with no descriptive explanation.
		/// </summary>
		/// <param name="width">The window width.</param>
		/// <param name="center">The window centre.</param>
		public Window(double width, double center)
			: this(width, center, null) {}

		/// <summary>
		/// Initializes a new instance of <see cref="Window"/>.
		/// </summary>
		/// <param name="width">The window width.</param>
		/// <param name="center">The window centre.</param>
		/// <param name="explanation">An optional descriptive explanation for the window.</param>
		public Window(double width, double center, string explanation)
		{
			_width = width;
			_center = center;
			_explanation = explanation ?? string.Empty;
		}

		/// <summary>
		/// Initializes a new instance of <see cref="Window"/>.
		/// </summary>
		[Obsolete("Cloning a Window has no utility, as the properties of the Window are immutable.")]
		public Window(Window window)
			: this(window.Width, window.Center) {}

		#endregion

		#region Static Helpers

		/// <summary>
		/// Gets a list of window width and center pairs, and their respective decriptive explanations, from the specified DICOM data set.
		/// </summary>
		/// <param name="provider">The source DICOM data set.</param>
		/// <returns>A list of windows.</returns>
		public static List<Window> GetWindowCenterAndWidth(IDicomAttributeProvider provider)
		{
			Platform.CheckForNullReference(provider, "provider");

			DicomAttribute windowCenterAttribute;
			DicomAttribute windowWidthAttribute;
			DicomAttribute windowExplanationAttribute;

			if (!provider.TryGetAttribute(DicomTags.WindowCenter, out windowCenterAttribute) || windowCenterAttribute.IsNull || windowCenterAttribute.IsEmpty)
				return new List<Window>();

			if (!provider.TryGetAttribute(DicomTags.WindowWidth, out windowWidthAttribute) || windowWidthAttribute.IsNull || windowWidthAttribute.IsEmpty)
				throw new DicomDataException("Window Center exists without Window Width.");

			if (windowWidthAttribute.Count != windowCenterAttribute.Count)
				throw new DicomDataException("Number of Window Center and Width entries differ.");

			if (provider.TryGetAttribute(DicomTags.WindowCenterWidthExplanation, out windowExplanationAttribute))
			{
				return Enumerable.Range(0, (int) windowCenterAttribute.Count).Select(i => new Window(windowWidthAttribute.GetFloat64(i, 0)
				                                                                                     , windowCenterAttribute.GetFloat64(i, 0)
				                                                                                     , windowExplanationAttribute.GetString(i, string.Empty))).ToList();
			}
			else
			{
				return Enumerable.Range(0, (int) windowCenterAttribute.Count).Select(i => new Window(windowWidthAttribute.GetFloat64(i, 0)
				                                                                                     , windowCenterAttribute.GetFloat64(i, 0))).ToList();
			}
		}

		/// <summary>
		/// Sets the window width, center and explanation attributes in the specified DICOM data set using the values from the list of windows.
		/// </summary>
		/// <param name="provider">The destination DICOM data set.</param>
		/// <param name="windows">A list of windows.</param>
		public static void SetWindowCenterAndWidth(IDicomAttributeProvider provider, IEnumerable<Window> windows)
		{
			Platform.CheckForNullReference(provider, "provider");
			Platform.CheckForNullReference(windows, "windows");

			var voiWindows = windows.ToArray();
			if (voiWindows.Length > 0)
			{
				var widthValues = new StringBuilder();
				var centerValues = new StringBuilder();
				var explanations = new StringBuilder();
				var hasExplanations = false;

				foreach (var value in voiWindows)
				{
					widthValues.Append(value.Width.ToString("G12"));
					widthValues.Append('\\');

					centerValues.Append(value.Center.ToString("G12"));
					centerValues.Append('\\');

					if (!string.IsNullOrEmpty(value.Explanation)) hasExplanations = true;
					explanations.Append(value.Explanation);
					explanations.Append('\\');
				}

				provider[DicomTags.WindowCenter].SetStringValue(centerValues.ToString(0, centerValues.Length - 1));
				provider[DicomTags.WindowWidth].SetStringValue(widthValues.ToString(0, widthValues.Length - 1));

				// only set the explanations attribute if any were provided - if there are none, remove that attribute entirely
				if (hasExplanations)
					provider[DicomTags.WindowCenterWidthExplanation].SetStringValue(explanations.ToString(0, explanations.Length - 1));
				else
					provider[DicomTags.WindowCenterWidthExplanation].SetEmptyValue();
			}
			else
			{
				// Remove the values from the dataset entirely
				provider[DicomTags.WindowCenter].SetEmptyValue();
				provider[DicomTags.WindowWidth].SetEmptyValue();
				provider[DicomTags.WindowCenterWidthExplanation].SetEmptyValue();
			}
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the window width.
		/// </summary>
		public double Width
		{
			get { return _width; }
		}

		/// <summary>
		/// Gets the window center.
		/// </summary>
		public double Center
		{
			get { return _center; }
		}

		/// <summary>
		/// Gets a descriptive explanation for the window.
		/// </summary>
		public string Explanation
		{
			get { return _explanation; }
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Gets a string representing the window width/center pair.
		/// </summary>
		public override string ToString()
		{
			// not meant to be a DICOM attribute value (since width and center pairs don't go in the same attribute anyway)
			return String.Format(@"{0:F2}/{1:F2}", _width, _center);
		}

		#region IEquatable<Window> Members

		public bool Equals(Window other)
		{
// ReSharper disable CompareOfFloatsByEqualityOperator
			return other != null && _width == other._width && _center == other._center;
// ReSharper restore CompareOfFloatsByEqualityOperator
		}

		#endregion

		public override bool Equals(object obj)
		{
			return Equals(obj as Window);
		}

		/// <summary>
		/// Serves as a hash function for a particular type. <see cref="M:System.Object.GetHashCode"></see> is suitable for use in hashing algorithms and data structures like a hash table.
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"></see>.
		/// </returns>
		public override int GetHashCode()
		{
			return _width.GetHashCode() ^ _center.GetHashCode() ^ -0x4B056F4B;
		}

		#endregion
	}
}