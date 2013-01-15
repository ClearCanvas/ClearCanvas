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

namespace ClearCanvas.Dicom.Iod
{
	public class ModalityDataLut : DataLut
	{
		#region Private Fields

		private readonly string _modalityLutType;
		
		#endregion

		#region Constructors

		public ModalityDataLut(int firstMappedPixelValue, int bitsPerEntry, int[] data, string modalityLutType)
			: this(firstMappedPixelValue, bitsPerEntry, data, modalityLutType, null)
		{
		}

		public ModalityDataLut(int firstMappedPixelValue, int bitsPerEntry, int[] data, string modalityLutType, string explanation)
			: base(firstMappedPixelValue, bitsPerEntry, data, explanation)
		{
			_modalityLutType = modalityLutType;
		}

		public ModalityDataLut(ModalityDataLut item)
			: base(item)
		{
			_modalityLutType = item.ModalityLutType;
		}

		protected ModalityDataLut(DataLut dataLut, string modalityLutType)
			: base(dataLut.FirstMappedPixelValue, dataLut.BitsPerEntry, dataLut.Data,
					dataLut.Explanation, dataLut.MinOutputValue, dataLut.MaxOutputValue)
		{
			_modalityLutType = modalityLutType;
		}

		#endregion

		#region Public Properties

		public string ModalityLutType
		{
			get { return _modalityLutType; }
		}

		#endregion

		#region Internal/Public Static Factory Methods
		
		internal static ModalityDataLut Create(DicomAttributeSQ modalityLutSequence, int pixelRepresentation)
		{
			List<DataLut> data = DataLut.Create(modalityLutSequence, pixelRepresentation != 0, false);
			if (data.Count == 0)
				return null;

			string modalityLutType = ((DicomSequenceItem[]) modalityLutSequence.Values)[0][DicomTags.ModalityLutType].ToString();
			return new ModalityDataLut(data[0], modalityLutType);
		}

		public static ModalityDataLut Create(IDicomAttributeProvider dicomAttributeProvider)
		{
			DicomAttributeSQ modalityLutSequence = (DicomAttributeSQ)dicomAttributeProvider[DicomTags.ModalityLutSequence];
			int pixelRepresentation = GetPixelRepresentation(dicomAttributeProvider);

			return Create(modalityLutSequence, pixelRepresentation);
		}

		#endregion
	}
}