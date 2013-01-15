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

namespace ClearCanvas.Common
{
	/// <summary>
	/// An extension filter that checks for equality with the extension class name.
	/// </summary>
    public class ClassNameExtensionFilter : ExtensionFilter
    {
        private string _name;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">The extension class name that will be a match for this filter.</param>
        public ClassNameExtensionFilter(string name)
        {
            _name = name;
        }

		/// <summary>
		/// Tests whether or not the input <see cref="ExtensionInfo.ExtensionClass"/>' full name matches 
		/// the name supplied to the filter constructor.
		/// </summary>
        public override bool Test(ExtensionInfo extension)
        {
            return extension.ExtensionClass.FullName.Equals(_name);
        }
    }
}
