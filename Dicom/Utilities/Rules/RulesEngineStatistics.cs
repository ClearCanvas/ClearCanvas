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

using ClearCanvas.Common.Statistics;

namespace ClearCanvas.Dicom.Utilities.Rules
{
    /// <summary>
    /// Stores the engine statistics of a rule engine.
    /// </summary>
    public class RulesEngineStatistics : StatisticsSet
    {    
        #region Constructors

        public RulesEngineStatistics()
        {
        }

        public RulesEngineStatistics(string name, string description)
            : base(name, description)
        {
            Context = new StatisticsContext(name);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the execution time of the rule engine in miliseconds.
        /// </summary>
        public TimeSpanStatistics ExecutionTime
        {
            get
            {
                if (this["ExecutionTime"] == null)
                {
                    this["ExecutionTime"] = new TimeSpanStatistics("ExecutionTime");
                }

                return (this["ExecutionTime"] as TimeSpanStatistics);
            }
        }

        /// <summary>
        /// Gets or sets the load time of the rule engine in miliseconds.
        /// </summary>
        public TimeSpanStatistics LoadTime
        {
            get
            {
                if (this["LoadTime"] == null)
                    this["LoadTime"] = new TimeSpanStatistics("LoadTime");
                return (this["LoadTime"] as TimeSpanStatistics);
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Reset the timer.
        /// </summary>
        public void Reset()
        {
            LoadTime.Reset();
            ExecutionTime.Reset();
        }

        #endregion
    }
}
