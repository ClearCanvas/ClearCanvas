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

using System.Runtime.Serialization;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.Dicom.ServiceModel.Query
{
	//NOTE: internal for now because we don't actually implement IPatientRootQuery anywhere.

	[DataContract(Namespace = QueryNamespace.Value)]
	internal class PatientRootStudyIdentifier : StudyIdentifier
	{
		public PatientRootStudyIdentifier()
		{
		}

		public PatientRootStudyIdentifier(IStudyIdentifier other)
			: base(other)
		{
		}

		public PatientRootStudyIdentifier(IStudyData other)
			: base(other)
		{
		}

		public PatientRootStudyIdentifier(IStudyData other, IIdentifier identifier)
			: base(other, identifier)
		{
		}

		public PatientRootStudyIdentifier(DicomAttributeCollection attributes)
			: base(attributes)
		{
		}
	}
}
