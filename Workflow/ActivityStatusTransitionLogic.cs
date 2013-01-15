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
    /// Defines the basic FSM logic for allowable <see cref="ActivityStatus"/> transitions.  Subclass this class
    /// and override the <see cref="IsAllowed"/> method to customize the logic for a particular scenario.
    /// </summary>
    public class ActivityStatusTransitionLogic : IFsmTransitionLogic<ActivityStatus>
    {
        private static readonly bool[,] _transitions = new bool[,] {
            // to: SC,   IP,   SU,   CM,   DC           // from:
                { false, true, true, true, true },      // SC
                { false, false, true, true, true },     // IP
                { false, true, false, true, true },     // SU
                { false, false, false, false, false},   // CM
                { false, false, false, false, false},   // DC
            };

        /// <summary>
        /// Returns a boolean value indicating whether the specified transition is allowed.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public virtual bool IsAllowed(ActivityStatus from, ActivityStatus to)
        {
            return _transitions[(int)from, (int)to];
        }

        public virtual bool IsTerminal(ActivityStatus state)
        {
            return state == ActivityStatus.CM || state == ActivityStatus.DC;
        }

        public bool IsInitial(ActivityStatus state)
        {
            return state == ActivityStatus.SC;
        }
    }
}
