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

namespace ClearCanvas.Common.Utilities
{
	//TODO (CR Sept 2010): move to Tests namespace?
	//TODO (CR Sept 2010): #if UNIT_TESTS?
    /// <summary>
    /// An implementation of <see cref="IExtensionFactory"/> that returns no extensions.
    /// </summary>
    /// <remarks>
    /// This implementation simply returns zero extensions for any extension point.  This is useful
    /// for unit-testing scenarios to prevent any extensions from being inadvertantly created.  This class
    /// may also be used as a base class for a more specialized extension factory that may respond to requests
    /// for certain extension points but not for others.
    /// </remarks>
    public class NullExtensionFactory : IExtensionFactory
    {
        #region IExtensionFactory Members

        /// <summary>
        /// Return an empty array.
        /// </summary>
        public virtual object[] CreateExtensions(ExtensionPoint extensionPoint, ExtensionFilter filter, bool justOne)
        {
            return new object[] { };
        }

        /// <summary>
        /// Returns an empty array.
        /// </summary>
        public virtual ExtensionInfo[] ListExtensions(ExtensionPoint extensionPoint, ExtensionFilter filter)
        {
            return new ExtensionInfo[] { };
        }

        #endregion
    }
}
