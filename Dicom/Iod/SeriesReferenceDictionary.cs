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
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.Dicom.Iod.Macros.PresentationStateRelationship;

namespace ClearCanvas.Dicom.Iod
{
	public class SeriesReferenceDictionary
	{
		private readonly Dictionary<string, ImageSopInstanceReferenceDictionary> _dictionary = new Dictionary<string, ImageSopInstanceReferenceDictionary>();

		public SeriesReferenceDictionary(IEnumerable<IReferencedSeriesSequence> seriesReferences)
		{
			foreach (IReferencedSeriesSequence seriesReference in seriesReferences)
			{
				ImageSopInstanceReferenceDictionary imageSopDictionary = null;
				ImageSopInstanceReferenceMacro[] imageSopReferences = seriesReference.ReferencedImageSequence;

				if (imageSopReferences != null && imageSopReferences.Length > 0)
				{
					imageSopDictionary = new ImageSopInstanceReferenceDictionary(imageSopReferences);
				}

				_dictionary.Add(seriesReference.SeriesInstanceUid, imageSopDictionary);
			}
		}

		public bool ReferencesSeries(string seriesInstanceUid)
		{
			if (_dictionary.ContainsKey(seriesInstanceUid))
				return true;
			return false;
		}

		public bool ReferencesSop(string seriesInstanceUid, string sopInstanceUid)
		{
			if (_dictionary.ContainsKey(seriesInstanceUid))
			{
				ImageSopInstanceReferenceDictionary sopDictionary = _dictionary[seriesInstanceUid];
				if (sopDictionary == null || sopDictionary.ReferencesSop(sopInstanceUid))
					return true;
			}
			return false;
		}

		public bool ReferencesAllFrames(string seriesInstanceUid, string sopInstanceUid)
		{
			if (_dictionary.ContainsKey(seriesInstanceUid))
			{
				ImageSopInstanceReferenceDictionary sopDictionary = _dictionary[seriesInstanceUid];
				if (sopDictionary == null || sopDictionary.ReferencesAllFrames(sopInstanceUid))
					return true;
			}
			return false;
		}

		public bool ReferencesAllSegments(string seriesInstanceUid, string sopInstanceUid)
		{
			if (_dictionary.ContainsKey(seriesInstanceUid))
			{
				ImageSopInstanceReferenceDictionary sopDictionary = _dictionary[seriesInstanceUid];
				if (sopDictionary == null || sopDictionary.ReferencesAllSegments(sopInstanceUid))
					return true;
			}
			return false;
		}

		public bool ReferencesFrame(string seriesInstanceUid, string sopInstanceUid, int frameNumber)
		{
			if (_dictionary.ContainsKey(seriesInstanceUid))
			{
				ImageSopInstanceReferenceDictionary sopDictionary = _dictionary[seriesInstanceUid];
				if (sopDictionary == null || sopDictionary.ReferencesFrame(sopInstanceUid, frameNumber))
					return true;
			}
			return false;
		}

		public bool ReferencesSegment(string seriesInstanceUid, string sopInstanceUid, uint segmentNumber)
		{
			if (_dictionary.ContainsKey(seriesInstanceUid))
			{
				ImageSopInstanceReferenceDictionary sopDictionary = _dictionary[seriesInstanceUid];
				if (sopDictionary == null || sopDictionary.ReferencesSegment(sopInstanceUid, segmentNumber))
					return true;
			}
			return false;
		}
	}
}