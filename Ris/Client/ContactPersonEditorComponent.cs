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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Enterprise;
using ClearCanvas.Desktop.Validation;
using System.Collections;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="ContactPersonEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ContactPersonEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ContactPersonEditorComponent class
    /// </summary>
    [AssociateView(typeof(ContactPersonEditorComponentViewExtensionPoint))]
    public class ContactPersonEditorComponent : ApplicationComponent
    {
        private ContactPersonDetail _contactPerson;
        private List<EnumValueInfo> _contactTypeChoices;
        private List<EnumValueInfo> _contactRelationshipChoices;

        /// <summary>
        /// Constructor
        /// </summary>
        public ContactPersonEditorComponent(ContactPersonDetail contactPerson, List<EnumValueInfo> contactTypeChoices, List<EnumValueInfo> contactRelationshipChoices)
        {
            _contactPerson = contactPerson;
            _contactTypeChoices = contactTypeChoices;
            _contactRelationshipChoices = contactRelationshipChoices;
        }

        public override void Start()
        {
            base.Start();

            if (_contactPerson.Relationship == null)
                _contactPerson.Relationship = _contactRelationshipChoices[0];
 
            if (_contactPerson.Type == null)
                _contactPerson.Type = _contactTypeChoices[0];
        }

        public override void Stop()
        {
            base.Stop();
        }

        #region Presentation Model

        [ValidateNotNull]
        public string Name
        {
            get { return _contactPerson.Name; }
            set
            {
                _contactPerson.Name = value;
                this.Modified = true;
            }
        }

        public string Address
        {
            get { return _contactPerson.Address; }
            set
            {
                _contactPerson.Address = value;
                this.Modified = true;
            }
        }

        public string HomePhoneNumber
        {
            get { return _contactPerson.HomePhoneNumber; }
            set
            {
                _contactPerson.HomePhoneNumber = value;
                this.Modified = true;
            }
        }

        public string BusinessPhoneNumber
        {
            get { return _contactPerson.BusinessPhoneNumber; }
            set
            {
                _contactPerson.BusinessPhoneNumber = value;
                this.Modified = true;
            }
        }

        public string PhoneNumberMask
        {
            get { return TextFieldMasks.TelephoneNumberFullMask; }
        }

        [ValidateNotNull]
        public EnumValueInfo Type
        {
            get { return _contactPerson.Type; }
            set
            {
                _contactPerson.Type = value;
                this.Modified = true;
            }
        }

        public IList TypeChoices
        {
            get { return _contactTypeChoices; }
        }

        [ValidateNotNull]
        public EnumValueInfo Relationship
        {
            get { return _contactPerson.Relationship; }
            set
            {
                _contactPerson.Relationship = value;
                this.Modified = true;
            }
        }

        public IList RelationshipChoices
        {
            get { return _contactRelationshipChoices; }
        }

        public void Accept()
        {
            if (this.HasValidationErrors)
            {
                this.ShowValidation(true);
            }
            else
            {
                this.ExitCode = ApplicationComponentExitCode.Accepted;
                Host.Exit();
            }
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.None;
            Host.Exit();
        }

        public bool AcceptEnabled
        {
            get { return this.Modified; }
        }

        public event EventHandler AcceptEnabledChanged
        {
            add { this.ModifiedChanged += value; }
            remove { this.ModifiedChanged -= value; }
        }

        #endregion
    }
}
