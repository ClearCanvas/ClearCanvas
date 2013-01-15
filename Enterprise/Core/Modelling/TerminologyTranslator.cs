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
using ClearCanvas.Common.Utilities;
using System.Reflection;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    /// <summary>
    /// Provides methods for translating domain object class and property names into user-friendly
    /// equivalents.
    /// </summary>
    public static class TerminologyTranslator
    {
        /// <summary>
        /// Translates the name of the specified property.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string Translate(PropertyInfo property)
        {
            return Translate(property.ReflectedType, property.Name);
        }

        /// <summary>
        /// Translates the name of the specified property on the specified domain class.
        /// </summary>
        /// <param name="domainClass"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static string Translate(Type domainClass, string propertyName)
        {
            IResourceResolver resolver = new ResourceResolver(domainClass.Assembly);

            string key = domainClass.Name + propertyName;
            string localized = resolver.LocalizeString(key);
            if (localized == key)
                localized = resolver.LocalizeString(propertyName);

            return localized;
        }

        /// <summary>
        /// Translates the name of the specified domain class.
        /// </summary>
        /// <param name="domainClass"></param>
        /// <returns></returns>
        public static string Translate(Type domainClass)
        {
            IResourceResolver resolver = new ResourceResolver(domainClass.Assembly);
            return resolver.LocalizeString(domainClass.Name);
        }
    }
}
