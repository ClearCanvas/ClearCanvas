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

namespace ClearCanvas.Common
{
    /// <summary>
    /// Implements an extension filter that performs matching on attributes.
    /// </summary>
    /// <remarks>
    /// For each attribute that is supplied to the constructor of this filter, the filter
    /// will check if the extension is marked with at least one matching attribute.  A matching attribute is an
    /// attribute for which the <see cref="Attribute.Match"/> method returns true.  This allows
    /// for quite sophisticated matching capabilities, as the attribute itself decides what constitutes
    /// a match.
    /// </remarks>
    public class AttributeExtensionFilter : ExtensionFilter
    {
        private Attribute[] _attributes;

        /// <summary>
        /// Creates a filter to match on multiple attributes.
        /// </summary>
        /// <remarks>
		/// The extension must test true on each attribute.
		/// </remarks>
        /// <param name="attributes">The attributes to be used as test criteria.</param>
        public AttributeExtensionFilter(Attribute[] attributes)
        {
            _attributes = attributes;
        }

        /// <summary>
        /// Creates a filter to match on a single attribute.
        /// </summary>
        /// <param name="attribute">The attribute to be used as test criteria.</param>
        public AttributeExtensionFilter(Attribute attribute)
            :this(new Attribute[] { attribute })
        {
        }

        /// <summary>
        /// Checks whether the specified extension is marked with attributes that 
        /// match every test attribute supplied as criteria to this filter.
        /// </summary>
        /// <param name="extension">The information about the extension to test.</param>
        /// <returns>true if the test succeeds.</returns>
        public override bool Test(ExtensionInfo extension)
        {
            foreach (Attribute a in _attributes)
            {
                object[] candidates = extension.ExtensionClass.Resolve().GetCustomAttributes(a.GetType(), true);
                if (!AnyMatch(a, candidates))
                {
                    return false;
                }
            }
            return true;
        }

        private bool AnyMatch(Attribute a, object[] candidates)
        {
            foreach (Attribute c in candidates)
            {
                if (c.Match(a))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
