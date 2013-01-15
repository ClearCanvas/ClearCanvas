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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Extended.Common.PatientReconciliation;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
    class ReconciliationCandidateTableEntry
    {
        private readonly ReconciliationCandidate _profileMatch;
        private bool _checked;
        private event EventHandler _checkedChanged;

        public ReconciliationCandidateTableEntry(ReconciliationCandidate match)
        {
            _profileMatch = match;
            _checked = false;
        }

        public ReconciliationCandidate ReconciliationCandidate
        {
            get { return _profileMatch; }
        }

        public bool Checked
        {
            get { return _checked; }
            set
            {
                if (_checked != value)
                {
                    _checked = value;
                    EventsHelper.Fire(_checkedChanged, this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler CheckedChanged
        {
            add { _checkedChanged += value; }
            remove { _checkedChanged -= value; }
        }
    }
}
