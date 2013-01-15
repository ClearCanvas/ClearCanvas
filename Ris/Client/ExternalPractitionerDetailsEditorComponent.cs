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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="ExternalPractitionerDetailsEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ExternalPractitionerDetailsEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ExternalPractitionerDetailsEditorComponent class
    /// </summary>
    [AssociateView(typeof(ExternalPractitionerDetailsEditorComponentViewExtensionPoint))]
    public class ExternalPractitionerDetailsEditorComponent : ApplicationComponent
    {
        private ExternalPractitionerDetail _practitionerDetail;
        private bool _isNew;
    	private bool _markVerified;

        /// <summary>
        /// Constructor
        /// </summary>
        public ExternalPractitionerDetailsEditorComponent(bool isNew)
        {
            _practitionerDetail = new ExternalPractitionerDetail();
            _isNew = isNew;
        }

        public ExternalPractitionerDetail ExternalPractitionerDetail
        {
            get { return _practitionerDetail; }
            set 
            { 
                _practitionerDetail = value;
            	_markVerified = _practitionerDetail.IsVerified;
            }
        }

        #region Presentation Model

        [ValidateNotNull]
        public string FamilyName
        {
            get { return _practitionerDetail.Name.FamilyName; }
            set 
            {
                _practitionerDetail.Name.FamilyName = value;
                this.Modified = true;
            }
        }

        [ValidateNotNull]
        public string GivenName
        {
            get { return _practitionerDetail.Name.GivenName; }
            set
            {
                _practitionerDetail.Name.GivenName = value;
                this.Modified = true;
            }
        }

        public string MiddleName
        {
            get { return _practitionerDetail.Name.MiddleName; }
            set
            {
                _practitionerDetail.Name.MiddleName = value;
                this.Modified = true;
            }
        }

        public string Prefix
        {
            get { return _practitionerDetail.Name.Prefix; }
            set
            {
                _practitionerDetail.Name.Prefix = value;
                this.Modified = true;
            }
        }

        public string Suffix
        {
            get { return _practitionerDetail.Name.Suffix; }
            set
            {
                _practitionerDetail.Name.Suffix = value;
                this.Modified = true;
            }
        }

        public string Degree
        {
            get { return _practitionerDetail.Name.Degree; }
            set
            {
                _practitionerDetail.Name.Degree = value;
                this.Modified = true;
            }
        }

        public string LicenseNumber
        {
            get { return _practitionerDetail.LicenseNumber; }
            set
            {
                _practitionerDetail.LicenseNumber = value;
                this.Modified = true;
            }
        }

        public string BillingNumber
        {
            get { return _practitionerDetail.BillingNumber; }
            set
            {
                _practitionerDetail.BillingNumber = value;
                this.Modified = true;
            }
        }

		public bool MarkVerified
		{
			get { return _markVerified; }
			set
			{
				_markVerified = value;
				this.Modified = true;
			}
		}

		public string LastVerified
		{
			get
			{
				var lastVerified = _practitionerDetail.LastVerifiedTime == null ? SR.MessageNever : Format.DateTime(_practitionerDetail.LastVerifiedTime);
				return string.Format(SR.FormatLastVerified, lastVerified);
			}
		}

		public bool CanVerify
		{
			get { return Thread.CurrentPrincipal.IsInRole(Application.Common.AuthorityTokens.Admin.Data.ExternalPractitionerVerification); }
		}

		public bool HasWarning
		{
			get { return _practitionerDetail.IsMerged; }
		}

		public string WarningMessage
		{
			get
			{
				var destination = PersonNameFormat.Format(_practitionerDetail.MergeDestination.Name);
				return string.Format(SR.WarnEditMergedPractitioner, destination);
			}
		}

		#endregion
	}
}
