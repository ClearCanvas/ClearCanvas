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
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow
{
	[DataContract]
	public class DicomSeriesDetail : DataContractBase, ICloneable 
	{
		[DataMember]
		public EntityRef ModalityPerformedProcedureStepRef;

		[DataMember]
		public EntityRef DicomSeriesRef;

		[DataMember]
		public string StudyInstanceUID;

		[DataMember]
		public string SeriesInstanceUID;

		[DataMember]
		public string SeriesDescription;

		[DataMember]
		public string SeriesNumber;

		[DataMember]
		public int NumberOfSeriesRelatedInstances;

		#region ICloneable Members

		public object Clone()
		{
			DicomSeriesDetail clone = new DicomSeriesDetail();
			clone.ModalityPerformedProcedureStepRef = this.ModalityPerformedProcedureStepRef;
			clone.DicomSeriesRef = this.DicomSeriesRef;
			clone.StudyInstanceUID = this.StudyInstanceUID;
			clone.SeriesInstanceUID = this.SeriesInstanceUID;
			clone.SeriesNumber = this.SeriesNumber;
			clone.SeriesDescription = this.SeriesDescription;
			clone.NumberOfSeriesRelatedInstances = this.NumberOfSeriesRelatedInstances;
			return clone;
		}

		#endregion
	}
}
