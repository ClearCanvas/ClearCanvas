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
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin.PatientAdmin;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="PatientProfileAdditionalInfoEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class PatientProfileAdditionalInfoEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// PatientProfileAdditionalInfoEditorComponent class
    /// </summary>
    [AssociateView(typeof(PatientProfileAdditionalInfoEditorComponentViewExtensionPoint))]
    public class PatientProfileAdditionalInfoEditorComponent : ApplicationComponent
    {
        private PatientProfileDetail _profile;

        private readonly List<EnumValueInfo> _religionChoices;
        private readonly List<EnumValueInfo> _languageChoices;

        /// <summary>
        /// Constructor
        /// </summary>
        public PatientProfileAdditionalInfoEditorComponent(List<EnumValueInfo> religionChoices, List<EnumValueInfo> languageChoices)
        {
            _languageChoices = languageChoices;
            _religionChoices = religionChoices;
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public PatientProfileDetail Subject
        {
            get { return _profile; }
            set { _profile = value; }
        }

        #region Presentation Model

        public EnumValueInfo Religion
        {
            get { return _profile.Religion; }
            set
            {
                _profile.Religion = value;
                this.Modified = true;
            }
        }

        public IList ReligionChoices
        {
            get { return _religionChoices; }
        }

        public EnumValueInfo Language
        {
            get { return _profile.PrimaryLanguage; }
            set
            {
                _profile.PrimaryLanguage = value;
                this.Modified = true;
            }
        }

        public IList LanguageChoices
        {
            get { return _languageChoices; }
        }

        #endregion
    }
}