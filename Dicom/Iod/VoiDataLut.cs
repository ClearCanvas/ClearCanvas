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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Dicom.Iod
{
	public class VoiDataLut : DataLut
	{
		private bool _alreadyCorrected;

		#region Constructors

		public VoiDataLut(int firstMappedPixelValue, int bitsPerEntry, int[] data)
			: this(firstMappedPixelValue, bitsPerEntry, data, null) {}

		public VoiDataLut(int firstMappedPixelValue, int bitsPerEntry, int[] data, string explanation)
			: base(firstMappedPixelValue, bitsPerEntry, data, explanation) {}

		public VoiDataLut(VoiDataLut item)
			: base(item) {}

		protected VoiDataLut(DataLut dataLut)
			: base(dataLut.FirstMappedPixelValue, dataLut.BitsPerEntry, dataLut.Data, dataLut.Explanation, dataLut.MinOutputValue, dataLut.MaxOutputValue) {}

		#endregion

		#region Public Methods

		/// <summary>
		/// Corrects the <see cref="VoiDataLut.MinOutputValue"/> and <see cref="VoiDataLut.MaxOutputValue"/> when <see cref="VoiDataLut.BitsPerEntry"/> is 16.
		/// </summary>
		/// <remarks>
		/// Some vendors set the <see cref="VoiDataLut.BitsPerEntry"/> to 16, but then only encode the lut values to the same bit depth
		/// as the actual pixel values (e.g. 12 bits).  This can make the images look really bad.  Apparently, this is the
		/// workaround David Clunie suggests.
		/// </remarks>
		/// <seealso cref="http://groups.google.com/group/comp.protocols.dicom/browse_thread/thread/6a033444802a35fc/0f0a9a1e35c1468e?lnk=gst"/>
		public void CorrectMinMaxOutput()
		{
			if (!_alreadyCorrected && BitsPerEntry == 16)
			{
				_alreadyCorrected = true;
				CorrectMinMaxOutputInternal();
			}
		}

		#endregion

		#region Static Methods

		#region Private

		private static void GetMinMaxPixelValue(int bitsStored, bool isSigned, out int min, out int max)
		{
			if (isSigned)
			{
				min = -(1 << (bitsStored - 1));
				max = (1 << (bitsStored - 1)) - 1;
			}
			else
			{
				min = 0;
				max = (1 << bitsStored) - 1;
			}
		}

		private unsafe void CorrectMinMaxOutputInternal()
		{
			int minLutValue = int.MaxValue;
			int maxLutValue = int.MinValue;

			int numberOfEntries = Data.Length;
			fixed (int* data = Data)
			{
				int* pData = data;
				int i = 0;
				while (i < numberOfEntries)
				{
					if (*pData < minLutValue)
						minLutValue = *pData;
					if (*pData > maxLutValue)
						maxLutValue = *pData;

					++pData;
					++i;
				}
			}

			bool isSigned = FirstMappedPixelValue < 0;
			int trueDepth = 16; //start at 16 and go downwards.

			while (trueDepth > 0)
			{
				int minAllowedValue, maxAllowedValue;
				GetMinMaxPixelValue(trueDepth, isSigned, out minAllowedValue, out maxAllowedValue);
				if (minLutValue < minAllowedValue || maxLutValue > maxAllowedValue)
				{
					//we've found 'true depth minus 1' (we started at 16).
					++trueDepth;
					GetMinMaxPixelValue(trueDepth, isSigned, out minAllowedValue, out maxAllowedValue);

					BitsPerEntry = trueDepth;
					MinOutputValue = minAllowedValue;
					MaxOutputValue = maxAllowedValue;

					break;
				}
				else
				{
					//go down one value.
					--trueDepth;
				}
			}
		}

		private static List<VoiDataLut> Convert(IEnumerable<DataLut> dataLuts)
		{
			return CollectionUtils.Map<DataLut, VoiDataLut>(dataLuts, dataLut => new VoiDataLut(dataLut));
		}

		#endregion

		#region Private Factory

		private static List<VoiDataLut> Create(DicomAttributeSQ voiLutSequence, int pixelRepresentation)
		{
			bool isFirstMappedPixelSigned = pixelRepresentation != 0;

			List<DataLut> dataLuts = Create(voiLutSequence, isFirstMappedPixelSigned, false);
			return Convert(dataLuts);
		}

		private static List<VoiDataLut> Create(DicomAttributeSQ voiLutSequence, int bitsStored, int pixelRepresentation, double rescaleSlope, double rescaleIntercept)
		{
			int minPixelValue;
			int maxPixelValue;
			bool isSigned = pixelRepresentation != 0;

			GetMinMaxPixelValue(bitsStored, isSigned, out minPixelValue, out maxPixelValue);

			double minModalityLutValue = minPixelValue*rescaleSlope + rescaleIntercept;
			double maxModalityLutValue = maxPixelValue*rescaleSlope + rescaleIntercept;

			bool isFirstMappedPixelValueSigned = minModalityLutValue < 0 || maxModalityLutValue < 0;

			List<DataLut> dataLuts = Create(voiLutSequence, isFirstMappedPixelValueSigned, false);
			return Convert(dataLuts);
		}

		private static List<VoiDataLut> Create(DicomAttributeSQ voiLutSequence, DicomAttributeSQ modalityLutSequence, int pixelRepresentation)
		{
			ModalityDataLut modalityLut = ModalityDataLut.Create(modalityLutSequence, pixelRepresentation);
			if (modalityLut == null)
				throw new DicomDataException("Input Modality Lut Sequence is not valid.");

			//Hounsfield units are always signed.
			bool isFirstMappedPixelValueSigned = pixelRepresentation != 0 || modalityLut.ModalityLutType == "HU";

			List<DataLut> dataLuts = Create(voiLutSequence, isFirstMappedPixelValueSigned, false);
			return Convert(dataLuts);
		}

		#endregion

		#region Public Factory

		public static List<VoiDataLut> Create(IDicomAttributeProvider attributeProvider)
		{
			Platform.CheckForNullReference(attributeProvider, "attributeProvider");

			DicomAttributeSQ voiLutSequence;
			if (!TryGetAttributeSQ(attributeProvider, DicomTags.VoiLutSequence, out voiLutSequence))
				return new List<VoiDataLut>();

			int pixelRepresentation = GetPixelRepresentation(attributeProvider);

			DicomAttributeSQ modalityLutSequence;
			TryGetAttributeSQ(attributeProvider, DicomTags.ModalityLutSequence, out modalityLutSequence);
			if (IsValidAttribute(modalityLutSequence))
				return Create(voiLutSequence, modalityLutSequence, pixelRepresentation);

			DicomAttribute rescaleInterceptAttribute = attributeProvider[DicomTags.RescaleIntercept];
			if (IsValidAttribute(rescaleInterceptAttribute))
			{
				double rescaleSlope = GetRescaleSlope(attributeProvider);
				double rescaleIntercept = rescaleInterceptAttribute.GetFloat64(0, 0);
				int bitsStored = GetBitsStored(attributeProvider);

				return Create(voiLutSequence, bitsStored, pixelRepresentation, rescaleSlope, rescaleIntercept);
			}

			return Create(voiLutSequence, pixelRepresentation);
		}

		private static double GetRescaleSlope(IDicomAttributeProvider attributeProvider)
		{
			DicomAttribute rescaleSlopeAttribute = attributeProvider[DicomTags.RescaleSlope];
			if (rescaleSlopeAttribute == null)
				return 1.0;
			else
				return rescaleSlopeAttribute.GetFloat64(0, 1);
		}

		private static bool TryGetAttributeSQ(IDicomAttributeProvider provider, uint tag, out DicomAttributeSQ dicomAttributeSQ)
		{
			DicomAttribute dicomAttribute;
			if (provider.TryGetAttribute(tag, out dicomAttribute))
			{
				dicomAttributeSQ = dicomAttribute as DicomAttributeSQ;
				return dicomAttributeSQ != null;
			}
			dicomAttributeSQ = null;
			return false;
		}

		#endregion

		#endregion
	}
}