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

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// A <see cref="TabGroup"/> to be hosted within a <see cref="TabGroupComponentContainer"/>.
	/// </summary>
    public class TabGroup
    {
        private float _weight;
        private TabComponentContainer _tabContainer;

        private ApplicationComponentHost _host;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="tabContainer">The owning container.</param>
        /// <param name="initialWeight">Initial weight of the tab group, relative to other tab groups.</param>
        public TabGroup(TabComponentContainer tabContainer, float initialWeight)
        {
            _tabContainer = tabContainer;
            _weight = initialWeight;
        }

        /// <summary>
        /// Gets the owner <see cref="TabComponentContainer"/>.
        /// </summary>
        public TabComponentContainer Component
        {
            get { return _tabContainer; }
        }

        /// <summary>
        /// Gets the weight assigned to this group, relative to the other groups.
        /// </summary>
        public float Weight
        {
            get { return _weight; }
        }

        /// <summary>
        /// Gets or sets the component host for this pane.
        /// </summary>
        /// <remarks>
		/// For internal framework use only.
		/// </remarks>
        public ApplicationComponentHost ComponentHost
        {
            get { return _host; }
            internal set { _host = value; }
        }
    }
}
