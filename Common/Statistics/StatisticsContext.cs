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

namespace ClearCanvas.Common.Statistics
{
    /// <summary>
    /// <see cref="IStatisticsContext"/> implemenation class 
    /// </summary>
    public class StatisticsContext : IStatisticsContext
    {
        #region Private Memebers

        private string _id;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="StatisticsContext"/> with a specified ID.
        /// </summary>
        /// <param name="id"></param>
        public StatisticsContext(string id)
        {
            _id = id;
        }

        #endregion

        #region IStatisticsContext Members

        /// <summary>
        /// Gets or sets the ID of the context
        /// </summary>
        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }

        #endregion
    }
}