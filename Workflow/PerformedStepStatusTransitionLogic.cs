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

namespace ClearCanvas.Workflow
{
    /// <summary>
    /// Defines the basic FSM for allowable <see cref="ActivityPerformedStepStatus"/> transitions.  Subclass this class
    /// and override the <see cref="IsAllowed"/> method to customize the FSM for a particular scenario.
    /// </summary>
    public class PerformedStepStatusTransitionLogic : IFsmTransitionLogic<PerformedStepStatus>
    {
        private static readonly bool[,] _transitions = new bool[,] {
            // to:   IP,    CM,   DC       // from:
                { false, true, true },     // IP
                { false, false, false},   // CM
                { false, false, false},   // DC
            };


        #region IFsmTransitionLogic<ActivityPerformedStepStatus> Members

        /// <summary>
        /// Returns a boolean value indicating whether the specified transition is allowed.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public virtual bool IsAllowed(PerformedStepStatus from, PerformedStepStatus to)
        {
            return _transitions[(int)from, (int)to];
        }

        public bool IsTerminal(PerformedStepStatus state)
        {
            return state == PerformedStepStatus.CM || state == PerformedStepStatus.DC;
        }

        public bool IsInitial(PerformedStepStatus state)
        {
            return state == PerformedStepStatus.IP;
        }

        #endregion
    }
}
