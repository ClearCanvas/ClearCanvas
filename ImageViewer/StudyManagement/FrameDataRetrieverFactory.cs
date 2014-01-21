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

using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	//TODO (later): get rid of this class.

	/// <summary>
	/// A delegate used to retrieve data from an <see cref="Frame"/>.
	/// </summary>
	public delegate T FrameDataRetrieverDelegate<T>(Frame frame);

	/// <summary>
	/// A helper factory for constructing a delegate to return a specific <see cref="DicomTag"/>'s
	/// value from an <see cref="ImageSop"/>.
	/// </summary>
	/// <remarks>
	/// Note that the <see cref="FrameDataRetrieverDelegate{T}"/>s returned by this factory
	/// simply return the default value for the return type when the tag has no value or
	/// does not exist.  Therefore, you should only use <see cref="FrameDataRetrieverDelegate{T}"/>s
	/// for cases when the existence of the tag is unimportant.
	/// </remarks>
	public static class FrameDataRetrieverFactory
	{
		/// <summary>
		/// Returns a delegate that will get the value of <paramref name="dicomTag"/> (at position 0),
		/// from an <see cref="ImageSop"/> as a <see cref="string"/>.
		/// </summary>
		public static FrameDataRetrieverDelegate<string> GetStringRetriever(uint dicomTag)
		{
			return GetStringRetriever(dicomTag, 0);
		}

		/// <summary>
		/// Returns a delegate that will get the value of <paramref name="dicomTag"/> (at <paramref name="position"/>),
		/// from an <see cref="ImageSop"/> as a <see cref="string"/>.
		/// </summary>
		public static FrameDataRetrieverDelegate<string> GetStringRetriever(uint dicomTag, uint position)
		{
			return delegate(Frame frame)
				{
					string value;
					value = frame[dicomTag].GetString((int)position, null);
					return value ?? "";
				};
		}

		/// <summary>
		/// Returns a delegate that will get the value of <paramref name="dicomTag"/> (all positions),
		/// from an <see cref="ImageSop"/> as an array of <see cref="string"/>s.
		/// </summary>
		public static FrameDataRetrieverDelegate<string[]> GetStringArrayRetriever(uint dicomTag)
		{
			return delegate(Frame frame)
				{
					string value;
					value = frame[dicomTag].ToString();
					return DicomStringHelper.GetStringArray(value ?? "");
				};
		}

		/// <summary>
		/// Returns a delegate that will get the value of <paramref name="dicomTag"/> (at position 0),
		/// from an <see cref="ImageSop"/> as an <see cref="int"/>.
		/// </summary>
		public static FrameDataRetrieverDelegate<int> GetIntRetriever(uint dicomTag)
		{
			return GetIntRetriever(dicomTag, 0);
		}

		/// <summary>
		/// Returns a delegate that will get the value of <paramref name="dicomTag"/> (at <paramref name="position"/>),
		/// from an <see cref="ImageSop"/> as an <see cref="int"/>.
		/// </summary>
		public static FrameDataRetrieverDelegate<int> GetIntRetriever(uint dicomTag, uint position)
		{
			return delegate(Frame frame)
				{
					int value;
					value = frame[dicomTag].GetInt32((int)position, 0);
					return value;
				};
		}

		/// <summary>
		/// Returns a delegate that will get the value of <paramref name="dicomTag"/> (all positions),
		/// from an <see cref="ImageSop"/> as an array of <see cref="int"/>s.
		/// </summary>
		public static FrameDataRetrieverDelegate<int[]> GetIntArrayRetriever(uint dicomTag)
		{
			return delegate(Frame frame)
				{
					string value;
					value = frame[dicomTag].ToString();
					
					int[] values;
					if (!DicomStringHelper.TryGetIntArray(value ?? "", out values))
						values = new int[]{};

					return values;
				};
		}

		/// <summary>
		/// Returns a delegate that will get the value of <paramref name="dicomTag"/> (at position 0),
		/// from an <see cref="ImageSop"/> as a <see cref="double"/>.
		/// </summary>
		public static FrameDataRetrieverDelegate<double> GetDoubleRetriever(uint dicomTag)
		{
			return GetDoubleRetriever(dicomTag, 0);
		}

		/// <summary>
		/// Returns a delegate that will get the value of <paramref name="dicomTag"/> (at <paramref name="position"/>),
		/// from an <see cref="ImageSop"/> as a <see cref="double"/>.
		/// </summary>
		public static FrameDataRetrieverDelegate<double> GetDoubleRetriever(uint dicomTag, uint position)
		{
			return delegate(Frame frame)
				{
					double value;
					value = frame[dicomTag].GetFloat64((int)position, 0);
					return value;
				};
		}

		/// <summary>
		/// Returns a delegate that will get the value of <paramref name="dicomTag"/> (all positions),
		/// from an <see cref="ImageSop"/> as an array of <see cref="double"/>s.
		/// </summary>
		public static FrameDataRetrieverDelegate<double[]> GetDoubleArrayRetriever(uint dicomTag)
		{
			return delegate(Frame frame)
				{
					string value;
					value = frame[dicomTag].ToString();
					double[] values;
					if (!DicomStringHelper.TryGetDoubleArray(value ?? "", out values))
						values = new double[] { };

					return values;

				};
		}
	}
}