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
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageViewer.Common.ServerDirectory;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.Common.WorkItem;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage
{
	internal class Series : ISeries
    {
		#region Private Fields

		private readonly Study _parentStudy;
		private readonly SeriesXml _seriesXml;
		private IList<ISopInstance> _sopInstances;

		#endregion

		internal Series(Study parentStudy, SeriesXml seriesXml)
        {
			_parentStudy = parentStudy;
			_seriesXml = seriesXml;
		}

        internal Study ParentStudy { get { return _parentStudy; } }

	    #region Private Members

		private IList<ISopInstance> SopInstances
		{
			get
			{
				if (_sopInstances == null)
				    _sopInstances = _seriesXml.Select(instance => (ISopInstance)new SopInstance(this, instance)).ToList();

				return _sopInstances;
			}	
		}

		private InstanceXml GetFirstSopInstanceXml()
		{
            try
            {
                return _seriesXml.First();
            }
            catch (Exception e)
            {
                string message = String.Format("There are no instances in this series ({0}).", SeriesInstanceUid);
                throw new Exception(message, e);
            }
		}

		#endregion

		#region ISeries Members

		public IStudy GetParentStudy()
		{
			return _parentStudy;
		}

		public string SpecificCharacterSet
		{
			get { return GetFirstSopInstanceXml()[DicomTags.SpecificCharacterSet].ToString(); }
		}

		public string StudyInstanceUid
		{
			get { return _parentStudy.StudyInstanceUid; }
		}

		public string SeriesInstanceUid
		{
			get { return _seriesXml.SeriesInstanceUid; }
		}

		public string Modality
		{
			get { return GetFirstSopInstanceXml()[DicomTags.Modality].GetString(0, ""); }
		}

		public string SeriesDescription
		{
			get { return GetFirstSopInstanceXml()[DicomTags.SeriesDescription].GetString(0, ""); }
		}

		public int SeriesNumber
		{
			get { return GetFirstSopInstanceXml()[DicomTags.SeriesNumber].GetInt32(0, 0); }
		}

		public int NumberOfSeriesRelatedInstances
		{
			get { return SopInstances.Count; }
		}

		int? ISeriesData.NumberOfSeriesRelatedInstances
		{
			get { return NumberOfSeriesRelatedInstances; }
		}

    	public IEnumerable<ISopInstance> GetSopInstances()
        {
    		return SopInstances;
        }

        #endregion

        public string[] SourceAETitlesInSeries
        {
            get
            {
                return SopInstances.Cast<SopInstance>()
                    .Where(s => !String.IsNullOrEmpty(s.SourceAETitle))
                    .Select(s => s.SourceAETitle).Distinct().ToArray();
            }
        }

	    internal SeriesEntry ToStoreEntry()
        {
            var entry = new SeriesEntry
            {
                Series = new SeriesIdentifier(this)
                {
                    InstanceAvailability = "ONLINE",
                    RetrieveAE = ServerDirectory.GetLocalServer(),
                    SpecificCharacterSet = SpecificCharacterSet
                },
                Data = new SeriesEntryData
                {
                    ScheduledDeleteTime = GetScheduledDeleteTime(),
                    SourceAETitlesInSeries = SourceAETitlesInSeries
                }
            };
            return entry;
        }

        private DateTime? GetScheduledDeleteTime()
        {
            using (var context = new DataAccessContext())
            {
                var broker = context.GetWorkItemBroker();
                var items = broker.GetWorkItems(DeleteSeriesRequest.WorkItemTypeString, null, StudyInstanceUid);
                if (items == null)
                    return null;

                //Only consider those items that have not yet run, or are still happening. Something that failed,
                //is being deleted, was canceled, aren't valid. We could have actually received the same series
                //again after already deleting it, for example.
                var validItems = items.Where(item => item.Status == WorkItemStatusEnum.Pending || item.Status == WorkItemStatusEnum.InProgress);
                var deleteItems = validItems
                                    .Where(item => item.Request is DeleteSeriesRequest)
                                    .Where(item => ((DeleteSeriesRequest) item.Request).SeriesInstanceUids.Contains(SeriesInstanceUid)).ToList();

                if (!deleteItems.Any())
                    return null;

                return deleteItems.Min(item => item.DeleteTime);
            }
        }
    }
}
