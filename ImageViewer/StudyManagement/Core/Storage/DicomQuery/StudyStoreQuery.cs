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
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage.DicomQuery
{
    public class StudyStoreQuery
    {
        private readonly DicomStoreDataContext _context;

        internal StudyStoreQuery(DicomStoreDataContext context)
        {
            _context = context;
        }

        public int GetStudyCount()
        {
            return GetStudyCount(null);
        }

        public int GetStudyCount(StudyEntry criteria)
        {
            int count = GetStudies(criteria).Count();
            return count;
        }

        public IList<StudyEntry> GetStudyEntries(StudyEntry criteria)
        {
            var studies = GetStudies(criteria);
            return studies.Select(s => s.ToStoreEntry()).ToList();
        }

        public IList<SeriesEntry> GetSeriesEntries(SeriesEntry criteria)
        {
            var series = GetSeries(criteria);
            return series.Select(s => s.ToStoreEntry()).ToList();
        }

        public IList<ImageEntry> GetImageEntries(ImageEntry criteria)
        {
            var sops = GetSopInstances(criteria);
            return sops.Select(s => s.ToStoreEntry()).ToList();
        }

        public IList<StudyRootStudyIdentifier> StudyQuery(StudyRootStudyIdentifier criteria)
        {
            var entries = GetStudies(criteria);
            return entries.Select(e => e.ToStoreEntry().Study).ToList();
        }

        public IList<SeriesIdentifier> SeriesQuery(SeriesIdentifier criteria)
        {
            var entries = GetSeries(criteria);
            return entries.Select(e => e.ToStoreEntry().Series).ToList();
        }

        public IList<ImageIdentifier> ImageQuery(ImageIdentifier criteria)
        {
            var entries = GetSopInstances(criteria);
            return entries.Select(e => e.ToStoreEntry().Image).ToList();
        }

        public IList<DicomAttributeCollection> Query(DicomAttributeCollection queryCriteria)
        {
            Platform.CheckForNullReference(queryCriteria, "queryCriteria");

            string level = queryCriteria[DicomTags.QueryRetrieveLevel].ToString();
            switch (level)
            {
                case "STUDY":
                    return StudyQuery(queryCriteria);
                case "SERIES":
                    return SeriesQuery(queryCriteria);
                case "IMAGE":
                    return ImageQuery(queryCriteria);
                default:
                    throw new ArgumentException(String.Format("Invalid query level: {0}", level));
            }
        }

        private IEnumerable<Study> GetStudies(StudyRootStudyIdentifier criteria)
        {
            return GetStudies(new StudyEntry { Study = criteria });
        }

        private IEnumerable<Series> GetSeries(SeriesIdentifier criteria)
        {
            return GetSeries(new SeriesEntry { Series = criteria });
        }

        private IEnumerable<SopInstance> GetSopInstances(ImageIdentifier criteria)
        {
            return GetSopInstances(new ImageEntry { Image = criteria });
        }

        private IEnumerable<Study> GetStudies(StudyEntry criteria)
        {
            try
            {
                //TODO (Marmot): make extended data queryable, too?
                DicomAttributeCollection dicomCriteria;
                if (criteria == null || criteria.Study == null)
                    dicomCriteria = new DicomAttributeCollection();
                else
                    dicomCriteria = criteria.Study.ToDicomAttributeCollection();

                var filters = new StudyPropertyFilters(dicomCriteria);
                var results = filters.Query(_context.Studies);
                return results;
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while performing the study query.", e);
            }
        }

        private IEnumerable<Series> GetSeries(SeriesEntry criteria)
        {
            try
            {
                string studyInstanceUid = null;
                if (criteria != null && criteria.Series != null)
                    studyInstanceUid = criteria.Series.StudyInstanceUid;

                //This will throw when Uid parameter is empty.
                IStudy study = GetStudy(studyInstanceUid);
                if (study == null)
                    return new List<Series>();
                
                //TODO (Marmot): make extended data queryable, too.
                var dicomCriteria = criteria.Series.ToDicomAttributeCollection();
                var filters = new SeriesPropertyFilters(dicomCriteria);
                var results = filters.FilterResults(study.GetSeries().Cast<Series>());
                return results;
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while performing the series query.", e);
            }
        }

        private IEnumerable<SopInstance> GetSopInstances(ImageEntry criteria)
        {
            try
            {
                string studyInstanceUid = null, seriesInstanceUid = null;
                if (criteria != null && criteria.Image != null)
                {
                    studyInstanceUid = criteria.Image.StudyInstanceUid;
                    seriesInstanceUid = criteria.Image.SeriesInstanceUid;
                }

                //This will throw when either Uid parameter is empty.
                var series = GetSeries(studyInstanceUid, seriesInstanceUid);
                if (series == null)
                    return new List<SopInstance>();

                //TODO (Marmot): make extended data queryable, too.
                var dicomCriteria = criteria.Image.ToDicomAttributeCollection();
                var filters = new SopInstancePropertyFilters(dicomCriteria);
                var results = filters.FilterResults(series.GetSopInstances().Cast<SopInstance>());
                return results;
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while performing the image query.", e);
            }
        }

        
        private List<DicomAttributeCollection> StudyQuery(DicomAttributeCollection queryCriteria)
        {
            try
            {
                var filters = new StudyPropertyFilters(queryCriteria);
                var results = filters.Query(_context.Studies);
                return filters.ConvertResultsToDataSets(results);
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while performing the study root query.", e);
            }
        }

        private List<DicomAttributeCollection> SeriesQuery(DicomAttributeCollection queryCriteria)
        {
            var study = GetStudy(queryCriteria[DicomTags.StudyInstanceUid]);
            if (study == null)
                return new List<DicomAttributeCollection>();

            try
            {
                var filters = new SeriesPropertyFilters(queryCriteria);
                var results = filters.FilterResults(study.GetSeries().Cast<Series>());
                return filters.ConvertResultsToDataSets(results);
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while performing the series query.", e);
            }
        }

        private List<DicomAttributeCollection> ImageQuery(DicomAttributeCollection queryCriteria)
        {
            var series = GetSeries(queryCriteria[DicomTags.StudyInstanceUid], queryCriteria[DicomTags.SeriesInstanceUid]);
            if (series == null)
                return new List<DicomAttributeCollection>();

            try
            {
                var filters = new SopInstancePropertyFilters(queryCriteria);
                var results = filters.FilterResults(series.GetSopInstances().Cast<SopInstance>());
                return filters.ConvertResultsToDataSets(results);
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while performing the image query.", e);
            }
        }

        private IStudy GetStudy(string studyInstanceUid)
        {
            if (String.IsNullOrEmpty(studyInstanceUid))
                throw new ArgumentException("The study uid must be specified for a series level query.");

            IStudy study = GetStudies(new StudyRootStudyIdentifier {StudyInstanceUid = studyInstanceUid}).FirstOrDefault();
            if (study == null)
                Platform.Log(LogLevel.Debug, "No study exists with the given study uid ({0}).", studyInstanceUid);

            return study;
        }

        private ISeries GetSeries(string studyInstanceUid, string seriesInstanceUid)
        {
            if (String.IsNullOrEmpty(studyInstanceUid) || String.IsNullOrEmpty(seriesInstanceUid))
                throw new ArgumentException("The study and series uids must be specified for an image level query.");

            IStudy study = GetStudies(new StudyRootStudyIdentifier { StudyInstanceUid = studyInstanceUid }).FirstOrDefault();
            if (study == null)
            {
                Platform.Log(LogLevel.Debug, "No study exists with the given study uid ({0}).", studyInstanceUid);
                return null;
            }

            ISeries series = (from s in study.GetSeries() where s.SeriesInstanceUid == seriesInstanceUid select s).FirstOrDefault();
            if (series == null)
                Platform.Log(LogLevel.Debug, "No series exists with the given study and series uids ({0}, {1})", studyInstanceUid, seriesInstanceUid);

            return series;
        }
    }
}