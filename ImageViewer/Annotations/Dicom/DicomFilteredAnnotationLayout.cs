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

namespace ClearCanvas.ImageViewer.Annotations.Dicom
{
	internal sealed class DicomFilteredAnnotationLayout
	{
		private readonly string _identifier;
		private readonly string _matchingLayoutIdentifier;
		private readonly List<KeyValuePair<string, string>> _filters;

		public DicomFilteredAnnotationLayout(string identifier, string matchingLayoutIdentifier)
		{
			Platform.CheckForEmptyString(identifier, "identifier");
			Platform.CheckForEmptyString(matchingLayoutIdentifier, "matchingLayoutIdentifier");

			_identifier = identifier;
			_matchingLayoutIdentifier = matchingLayoutIdentifier;

			_filters = new List<KeyValuePair<string, string>>();
		}

		public string Identifier
		{
			get { return _identifier; }
		}

		public string MatchingLayoutIdentifier
		{
			get { return _matchingLayoutIdentifier; }
		}

		public IList<KeyValuePair<string, string>> Filters
		{
			get { return _filters; }
		}

		internal bool IsMatch(List<KeyValuePair<string, string>> filterCandidates)
		{
			foreach (KeyValuePair<string, string> filter in _filters)
			{
				foreach (KeyValuePair<string, string> candidate in filterCandidates)
				{
					if (candidate.Key == filter.Key)
					{
						if (filter.Value != candidate.Value)
							return false;
					}
				}
			}

			//all filters matched, or there were none, which is always a match.
			return true;
		}
	}
}
