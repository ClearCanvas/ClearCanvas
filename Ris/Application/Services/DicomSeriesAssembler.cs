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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;

namespace ClearCanvas.Ris.Application.Services
{
	public class DicomSeriesAssembler
	{
		class DicomSeriesSynchronizeHelper : CollectionSynchronizeHelper<DicomSeries, DicomSeriesDetail>
        {
            private readonly DicomSeriesAssembler _assembler;
            private readonly ModalityPerformedProcedureStep _mpps;
            private readonly IPersistenceContext _context;

            public DicomSeriesSynchronizeHelper(DicomSeriesAssembler assembler, ModalityPerformedProcedureStep mpps, IPersistenceContext context)
                : base(true, true)
            {
            	_assembler = assembler;
                _mpps = mpps;
                _context = context;
            }

            protected override bool CompareItems(DicomSeries domainItem, DicomSeriesDetail sourceItem)
            {
                return Equals(domainItem.GetRef(), sourceItem.DicomSeriesRef);
            }

            protected override void AddItem(DicomSeriesDetail sourceItem, ICollection<DicomSeries> domainList)
            {
                DicomSeries item = _assembler.CreateDicomSeries(sourceItem, _mpps);
                _context.Lock(item, DirtyState.New);
                _context.SynchState();
                sourceItem.DicomSeriesRef = item.GetRef();
                domainList.Add(item);
            }

            protected override void UpdateItem(DicomSeries domainItem, DicomSeriesDetail sourceItem, ICollection<DicomSeries> domainList)
            {
                _assembler.UpdateDicomSeries(domainItem, sourceItem);
            }

            protected override void RemoveItem(DicomSeries domainItem, ICollection<DicomSeries> domainList)
            {
				domainList.Remove(domainItem);
            }
        }

        public void SynchronizeDicomSeries(ModalityPerformedProcedureStep mpps, IList<DicomSeriesDetail> sourceList, IPersistenceContext context)
        {
			DicomSeriesSynchronizeHelper synchronizer = new DicomSeriesSynchronizeHelper(this, mpps, context);
			synchronizer.Synchronize(mpps.DicomSeries, sourceList);
        }

		public List<DicomSeriesDetail> GetDicomSeriesDetails(IEnumerable<DicomSeries> sourceList)
		{
			List<DicomSeriesDetail> dicomSeries = CollectionUtils.Map<DicomSeries, DicomSeriesDetail>(
				sourceList,
				delegate(DicomSeries series) { return CreateDicomSeriesDetail(series); });

			return dicomSeries;
		}

		#region Private Helpers

		private DicomSeries CreateDicomSeries(DicomSeriesDetail detail, ModalityPerformedProcedureStep mpps)
        {
			DicomSeries newSeries = new DicomSeries();
			newSeries.ModalityPerformedProcedureStep = mpps;
			UpdateDicomSeries(newSeries, detail);
			return newSeries;
        }

		private void UpdateDicomSeries(DicomSeries domainItem, DicomSeriesDetail sourceItem)
        {
            domainItem.StudyInstanceUID = sourceItem.StudyInstanceUID;
        	domainItem.SeriesInstanceUID = sourceItem.SeriesInstanceUID;
            domainItem.SeriesDescription = sourceItem.SeriesDescription;
            domainItem.SeriesNumber = sourceItem.SeriesNumber;
            domainItem.NumberOfSeriesRelatedInstances = sourceItem.NumberOfSeriesRelatedInstances;
        }

		private DicomSeriesDetail CreateDicomSeriesDetail(DicomSeries dicomSeries)
		{
			DicomSeriesDetail detail = new DicomSeriesDetail();

			detail.ModalityPerformedProcedureStepRef = dicomSeries.ModalityPerformedProcedureStep.GetRef();
			detail.DicomSeriesRef = dicomSeries.GetRef();
			detail.StudyInstanceUID = dicomSeries.StudyInstanceUID;
			detail.SeriesInstanceUID = dicomSeries.SeriesInstanceUID;
			detail.SeriesDescription = dicomSeries.SeriesDescription;
			detail.SeriesNumber = dicomSeries.SeriesNumber;
			detail.NumberOfSeriesRelatedInstances = dicomSeries.NumberOfSeriesRelatedInstances;

			return detail;
		}

		#endregion
	}
}
