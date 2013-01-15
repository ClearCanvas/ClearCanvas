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
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Alerts
{
	[ExtensionOf(typeof(PatientAlertExtensionPoint))]
	public class NoteAlert : PatientAlertBase
	{
		public override string Id
		{
			get { return "NoteAlert"; }
		}

		public override AlertNotification Test(Patient patient, IPersistenceContext context)
		{
			var reasons = new List<string>();
			foreach (var note in patient.Notes)
			{
				if (note.IsCurrent && note.Category.Severity == NoteSeverity.H)
				{
					if (!string.IsNullOrEmpty(note.Comment))
						reasons.Add(string.Format("{0}: {1}", note.Category.Name, note.Comment));
					else
						reasons.Add(note.Category.Name);
				}
			}

			return reasons.Count > 0 ? new AlertNotification(this.Id, reasons) : null;
		}
	}
}
