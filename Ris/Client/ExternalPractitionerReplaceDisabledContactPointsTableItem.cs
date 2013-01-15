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
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	public class ExternalPractitionerReplaceDisabledContactPointsTableItem
	{
		private ExternalPractitionerContactPointDetail _selectedNewContactPoint;
		private event EventHandler<EventArgs> _selectedNewContactPointModified;

		public string AffectedOrdersCount { get; private set; }
		public ExternalPractitionerContactPointDetail OldContactPoint { get; private set; }
		public string OldContactPointName { get; private set; }
		public string OldContactPointInfo { get; private set; }
		public List<ExternalPractitionerContactPointDetail> NewContactPointChoices { get; private set; }
		public string NewContactPointInfo { get; private set; }

		public ExternalPractitionerReplaceDisabledContactPointsTableItem(ExternalPractitionerContactPointDetail oldContactPoint, List<ExternalPractitionerContactPointDetail> newContactPointChoices)
		{
			this.OldContactPoint = oldContactPoint;
			this.OldContactPointName = oldContactPoint.Name;
			this.OldContactPointInfo = GetContactPointInfo(oldContactPoint);

			this.NewContactPointChoices = newContactPointChoices;

			this.AffectedOrdersCount = string.Format("{0} orders will be modified.", "???");
		}

		public void SetAffectedOrdersCount(int count)
		{
			this.AffectedOrdersCount = string.Format("{0} orders will be modified.", count);
		}

		public ExternalPractitionerContactPointDetail SelectedNewContactPoint
		{
			get { return _selectedNewContactPoint; }
			set
			{
				_selectedNewContactPoint = value;
				this.NewContactPointInfo = _selectedNewContactPoint == null ? null : GetContactPointInfo(_selectedNewContactPoint);
				EventsHelper.Fire(_selectedNewContactPointModified, this, EventArgs.Empty);
			}
		}

		public event EventHandler<EventArgs> SelectedNewContactPointModified
		{
			add { _selectedNewContactPointModified += value; }
			remove { _selectedNewContactPointModified += value; }
		}

		public string FormatNewContactPointChoice(object item)
		{
			var detail = (ExternalPractitionerContactPointDetail)item;

			return string.IsNullOrEmpty(detail.Description)
				? ExternalPractitionerContactPointFormat.Format(detail, "%N")
				: ExternalPractitionerContactPointFormat.Format(detail, "%N - %D");
		}

		private static string GetContactPointInfo(ExternalPractitionerContactPointDetail cp)
		{
			var builder = new StringBuilder();
			builder.AppendFormat("Description: {0}", cp.Description);
			builder.AppendLine();
			builder.AppendFormat(SR.FormatPhone, cp.CurrentPhoneNumber == null ? "" : TelephoneFormat.Format(cp.CurrentPhoneNumber));
			builder.AppendLine();
			builder.AppendFormat(SR.FormatFax, cp.CurrentFaxNumber == null ? "" : TelephoneFormat.Format(cp.CurrentFaxNumber));
			builder.AppendLine();
			builder.AppendFormat(SR.FormatAddress, cp.CurrentAddress == null ? "" : AddressFormat.Format(cp.CurrentAddress));
			builder.AppendLine();
			builder.AppendFormat(SR.FormatEmail, cp.CurrentEmailAddress == null ? "" : cp.CurrentEmailAddress.Address);
			return builder.ToString();
		}
	}
}
