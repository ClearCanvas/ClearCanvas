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

namespace ClearCanvas.Common.Scripting
{
	/// <summary>
	/// Used to specify that a class (for example, an <see cref="IScriptEngine"/>) 
	/// supports a certain scripting language.
	/// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=false)]
    public class LanguageSupportAttribute : Attribute
    {
        private string _language;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="language">A string describing the language.</param>
		public LanguageSupportAttribute(string language)
        {
            _language = language;
        }

		/// <summary>
		/// Gets a string describing the language.
		/// </summary>
        public string Language
        {
            get { return _language; }
        }

		/// <summary>
		/// Determines whether or not this instance is the same as <paramref name="obj"/>, which is itself an <see cref="Attribute"/>.
		/// </summary>
		public override bool Match(object obj)
        {
            LanguageSupportAttribute that = obj as LanguageSupportAttribute;
            return that != null && that.Language.ToLower().Equals(this.Language.ToLower());
        }
    }
}
