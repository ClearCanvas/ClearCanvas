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
using System.Xml;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Data;

namespace ClearCanvas.ImageServer.Core.Reconcile.CreateStudy
{
    public class SeriesMapUpdatedEventArgs:EventArgs
    {
        public Dictionary<string, SeriesMapping> SeriesMap { get; set; }
    }

    public class SopMapUpdatedEventArgs : EventArgs
    {
        public Dictionary<string, string> SopMap { get; set; }
    }

	public class UidMapper
	{
		#region Private Members

		private readonly Dictionary<string, string> _studyMap = new Dictionary<string, string>();
        private readonly Dictionary<string, SeriesMapping> _seriesMap = new Dictionary<string, SeriesMapping>();
		private readonly Dictionary<string, string> _sopMap = new Dictionary<string, string>();        
	    private object _sync = new object();

		#endregion

		#region Events

		public event EventHandler<SeriesMapUpdatedEventArgs> SeriesMapUpdated;
		public event EventHandler<SopMapUpdatedEventArgs> SopMapUpdated;
		
		#endregion

		#region Public Properties

		public bool Dirty { get; set; }

		#endregion

		#region Constructors

		public UidMapper(UidMapXml xml)
        {
            foreach(StudyUidMap studyMap in xml.StudyUidMaps)
            {
                _studyMap.Add(studyMap.Source, studyMap.Target);

                foreach (Map seriesMap in studyMap.Series)
                    _seriesMap.Add(seriesMap.Source,
                                   new SeriesMapping { OriginalSeriesUid = seriesMap.Source, NewSeriesUid = seriesMap.Target });

                foreach (Map sopMap in studyMap.Instances)
                    _sopMap.Add(sopMap.Source, sopMap.Target);
            
            }           
        }

		public UidMapper(IEnumerable<SeriesMapping> seriesList)
		{
			foreach (SeriesMapping map in seriesList)
				_seriesMap.Add(map.OriginalSeriesUid, map);
		}

		public UidMapper()
		{
		}

		#endregion

		#region Public Methods

		public bool ContainsSop(string originalSopUid)
		{
            lock(_sync)
            {
                return _sopMap.ContainsKey(originalSopUid);
            }
		    
		}

        public string FindNewSopUid(string originalSopUid)
        {
            lock (_sync)
            {
                string newSopUid;
                if (_sopMap.TryGetValue(originalSopUid, out newSopUid))
                    return newSopUid;
            	return null;
            }
        }

        public string FindNewSeriesUid(string originalSeriesUid)
        {
            lock (_sync)
            {
                SeriesMapping mapping;
                if (_seriesMap.TryGetValue(originalSeriesUid, out mapping))
                    return mapping.NewSeriesUid;
            	return null;
            }
        }

	    public bool ContainsSeries(string originalSeriesUid)
        {
            lock (_sync)
            {
                return _seriesMap.ContainsKey(originalSeriesUid);
            }
        }

        public void AddSop(string originalSopUid, string newSopUid)
        {
            lock (_sync)
            {
                _sopMap.Add(originalSopUid, newSopUid);
                Dirty = true;
            }

            EventsHelper.Fire(SeriesMapUpdated, this, new SeriesMapUpdatedEventArgs { SeriesMap = _seriesMap });
        }

        public void AddSeries(string originalStudyUid, string newStudyUid, string originalSeriesUid, string newSeriesUid)
        {
            lock (_sync)
            {
                if (!ContainsStudyMap(originalStudyUid, newStudyUid))
                {
                    _studyMap.Add(originalStudyUid, newStudyUid);
                }

                _seriesMap.Add(originalSeriesUid,
                               new SeriesMapping {OriginalSeriesUid = originalSeriesUid, NewSeriesUid = newSeriesUid});

                Dirty = true;
            }
            EventsHelper.Fire(SopMapUpdated, this, new SopMapUpdatedEventArgs { SopMap = _sopMap });
		}
		
		public IEnumerable<SeriesMapping> GetSeriesMappings()
	    {
	        return _seriesMap.Values;
	    }

	    public void Save(string path)
	    {
            lock (_sync)
            {
                UidMapXml xml = new UidMapXml();
                xml.StudyUidMaps = new List<StudyUidMap>();

                foreach (var entry in _studyMap)
                {
                    StudyUidMap studyMap = new StudyUidMap {Source = entry.Key, Target = entry.Value};
                    xml.StudyUidMaps.Add(studyMap);

                    studyMap.Series = new List<Map>();
                    foreach (SeriesMapping seriesMap in _seriesMap.Values)
                    {
                        studyMap.Series.Add(new Map {Source = seriesMap.OriginalSeriesUid, Target = seriesMap.NewSeriesUid});
                    }

                    studyMap.Instances = new List<Map>();
                    foreach (var sop in _sopMap)
                    {
                        studyMap.Instances.Add(new Map {Source = sop.Key, Target = sop.Value});
                    }
                }

                XmlDocument doc = XmlUtils.SerializeAsXmlDoc(xml);
                doc.Save(path);

                Dirty = false;
            }
	    }

		#endregion

		#region Private Methods

		private bool ContainsStudyMap(string originalStudyUid, string newStudyUid)
		{
			lock (_sync)
			{
				foreach (var entry in _studyMap)
				{
					if (entry.Key == originalStudyUid && entry.Value == newStudyUid)
					{
						return true;
					}
				}

				return false;
			}
		}

		#endregion
	}
}
