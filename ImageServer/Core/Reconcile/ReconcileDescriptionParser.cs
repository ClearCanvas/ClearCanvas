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
using System.Xml;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Data;

namespace ClearCanvas.ImageServer.Core.Reconcile
{
	public class StudyReconcileDescriptorParser
	{
		public StudyReconcileDescriptor Parse(XmlDocument doc)
		{
			if (doc.DocumentElement != null)
			{
				if (doc.DocumentElement.Name == "Reconcile")
				{
					return XmlUtils.Deserialize<StudyReconcileDescriptor>(doc.DocumentElement);
				}
				// Note, the prior software versions had "MergeStudy", "CreateStudy"
				// and "Discard" Document Elements.  With 1.5, they were all changed
				// to "Reconcile".
				if (doc.DocumentElement.Name == "MergeStudy"
				    ||doc.DocumentElement.Name == "CreateStudy"
				    ||doc.DocumentElement.Name == "Discard")
					throw new NotSupportedException(String.Format("ReconcileStudy Command from prior version no longer supported: {0}", doc.DocumentElement.Name));
                
				throw new NotSupportedException(String.Format("ReconcileStudy Command: {0}", doc.DocumentElement.Name));
			}

			return null;
		}
	}
}