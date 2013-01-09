#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS
//
// The ClearCanvas RIS/PACS is free software: you can redistribute it 
// and/or modify it under the terms of the GNU General Public License 
// as published by the Free Software Foundation, either version 3 of 
// the License, or (at your option) any later version.
//
// ClearCanvas RIS/PACS is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with ClearCanvas RIS/PACS.  If not, 
// see <http://www.gnu.org/licenses/>.

#endregion

using System.Collections.Generic;

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// CR Series Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.1.1 (Table C.8-1)</remarks>
	public class CrSeriesModuleIod : IodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CrSeriesModuleIod"/> class.
		/// </summary>	
		public CrSeriesModuleIod() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="CrSeriesModuleIod"/> class.
		/// </summary>
		/// <param name="dicomAttributeProvider">The DICOM attribute collection.</param>
		public CrSeriesModuleIod(IDicomAttributeProvider dicomAttributeProvider)
			: base(dicomAttributeProvider) {}

		/// <summary>
		/// Initializes the underlying collection to implement the module or sequence using default values.
		/// </summary>
		public void InitializeAttributes()
		{
			BodyPartExamined = string.Empty;
			ViewPosition = string.Empty;
			FilterType = null;
			CollimatorGridName = null;
			FocalSpots = null;
			PlateType = null;
			PhosphorType = null;
		}

		/// <summary>
		/// Checks if this module appears to be non-empty.
		/// </summary>
		/// <returns>True if the module appears to be non-empty; False otherwise.</returns>
		public bool HasValues()
		{
			if (string.IsNullOrEmpty(BodyPartExamined)
			    && string.IsNullOrEmpty(ViewPosition)
			    && string.IsNullOrEmpty(FilterType)
			    && string.IsNullOrEmpty(CollimatorGridName)
			    && IsNullOrEmpty(FocalSpots)
			    && string.IsNullOrEmpty(PlateType)
			    && string.IsNullOrEmpty(PhosphorType))
				return false;
			return true;
		}

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags
		{
			get
			{
				yield return DicomTags.BodyPartExamined;
				yield return DicomTags.ViewPosition;
				yield return DicomTags.FilterType;
				yield return DicomTags.CollimatorGridName;
				yield return DicomTags.FocalSpots;
				yield return DicomTags.PlateType;
				yield return DicomTags.PhosphorType;
			}
		}

		/// <summary>
		/// Gets or sets the value of BodyPartExamined in the underlying collection. Type 2.
		/// </summary>
		public string BodyPartExamined
		{
			get { return DicomAttributeProvider[DicomTags.BodyPartExamined].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.BodyPartExamined].SetNullValue();
					return;
				}
				DicomAttributeProvider[DicomTags.BodyPartExamined].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ViewPosition in the underlying collection. Type 2.
		/// </summary>
		public string ViewPosition
		{
			get { return DicomAttributeProvider[DicomTags.ViewPosition].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.ViewPosition].SetNullValue();
					return;
				}
				DicomAttributeProvider[DicomTags.ViewPosition].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of FilterType in the underlying collection. Type 3.
		/// </summary>
		public string FilterType
		{
			get { return DicomAttributeProvider[DicomTags.FilterType].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.FilterType] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.FilterType].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of CollimatorGridName in the underlying collection. Type 3.
		/// </summary>
		public string CollimatorGridName
		{
			get { return DicomAttributeProvider[DicomTags.CollimatorGridName].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.CollimatorGridName] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.CollimatorGridName].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of FocalSpots in the underlying collection. Type 3.
		/// </summary>
		public double[] FocalSpots
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.FocalSpots];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;

				var values = new double[dicomAttribute.Count];
				for (var n = 0; n < values.Length; n++)
					values[n] = dicomAttribute.GetFloat64(n, 0);
				return values;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					DicomAttributeProvider[DicomTags.FocalSpots] = null;
					return;
				}

				var dicomAttribute = DicomAttributeProvider[DicomTags.FocalSpots];
				for (var n = 0; n < value.Length; n++)
					dicomAttribute.SetFloat64(n, value[n]);
			}
		}

		/// <summary>
		/// Gets or sets the value of PlateType in the underlying collection. Type 3.
		/// </summary>
		public string PlateType
		{
			get { return DicomAttributeProvider[DicomTags.PlateType].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.PlateType] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.PlateType].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of PhosphorType in the underlying collection. Type 3.
		/// </summary>
		public string PhosphorType
		{
			get { return DicomAttributeProvider[DicomTags.PhosphorType].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.PhosphorType] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.PhosphorType].SetString(0, value);
			}
		}
	}
}