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
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
    /// <summary>
    /// Extension point for views onto <see cref="ConfirmReconciliationComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ConfirmReconciliationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ConfirmReconciliationComponent class
    /// </summary>
    [AssociateView(typeof(ConfirmReconciliationComponentViewExtensionPoint))]
    public class ReconciliationConfirmComponent : ApplicationComponent
    {
        private PatientProfileTable _sourceProfiles;
        private PatientProfileTable _targetProfiles;


        /// <summary>
        /// Constructor
        /// </summary>
        public ReconciliationConfirmComponent(IList<PatientProfileSummary> targets, IList<PatientProfileSummary> sources)
        {
            _sourceProfiles = new PatientProfileTable();
            _sourceProfiles.Items.AddRange(sources);

            _targetProfiles = new PatientProfileTable();
            _targetProfiles.Items.AddRange(targets);
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public ITable SourcePatientData
        {
            get { return _sourceProfiles; }
        }

        public ITable TargetPatientData
        {
            get { return _targetProfiles; }
        }

        public void Continue()
        {
            this.ExitCode = ApplicationComponentExitCode.Accepted;
            this.Host.Exit();
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.None;
            this.Host.Exit();
        }

    }
}
