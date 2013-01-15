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

using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	public class ExternalPractitionerOverviewComponent : DHtmlComponent
	{
		// Internal data contract used for jscript deserialization
		[DataContract]
		public class ExternalPractitionerContext : DataContractBase
		{
			[DataMember]
			public ExternalPractitionerSummary PractitionerSummary;

			[DataMember]
			public ExternalPractitionerDetail PractitionerDetail;
		}

		private readonly ExternalPractitionerContext _context;

		public ExternalPractitionerOverviewComponent()
		{
			_context = new ExternalPractitionerContext();
		}

		public override void Start()
		{
			Refresh();
			base.Start();
		}

		public string PreviewUrl
		{
			get { return WebResourcesSettings.Default.ExternalPractitionerOverviewPageUrl; }
		}

		public void Refresh()
		{
			this.SetUrl(this.PreviewUrl);
		}

		public ExternalPractitionerSummary PractitionerSummary
		{
			set
			{
				_context.PractitionerSummary = value;
				_context.PractitionerDetail = null;
				Refresh();
			}
		}

		public ExternalPractitionerDetail PractitionerDetail
		{
			set
			{
				_context.PractitionerSummary = null;
				_context.PractitionerDetail = value;
				Refresh();
			}
		}

		protected override DataContractBase GetHealthcareContext()
		{
			return _context;
		}
	}
}
