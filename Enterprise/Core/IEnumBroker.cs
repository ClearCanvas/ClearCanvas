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

namespace ClearCanvas.Enterprise.Core
{
    public interface IEnumBroker : IPersistenceBroker
    {
        /// <summary>
        /// Loads all enumeration values for the specified enumeration class, optionally including de-activated values.
        /// </summary>
        /// <param name="enumValueClass"></param>
        /// <param name="includeDeactivated"></param>
        /// <returns></returns>
        IList<EnumValue> Load(Type enumValueClass, bool includeDeactivated);

        /// <summary>
        /// Loads all enumeration values for the specified enumeration class, optionally including de-activated values.
        /// </summary>
        /// <typeparam name="TEnumValue"></typeparam>
        /// <returns></returns>
        IList<TEnumValue> Load<TEnumValue>(bool includeDeactivated) where TEnumValue : EnumValue;

        /// <summary>
        /// Finds the enumeration value for the specified enumeration class and enumeration code.
        /// </summary>
        /// <param name="enumValueClass"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        EnumValue Find(Type enumValueClass, string code);

        /// <summary>
        /// Finds the enumeration value for the specified enumeration class and enumeration code.
        /// </summary>
        /// <typeparam name="TEnumValue"></typeparam>
        /// <param name="code"></param>
        /// <returns></returns>
        TEnumValue Find<TEnumValue>(string code) where TEnumValue : EnumValue;

        /// <summary>
        /// Trys to find the enumeration value for the specified enumeration class and enumeration code.
        /// Throws a <see cref="EnumValueNotFoundException"/> if the class does not contain the specified code.
        /// </summary>
        /// <remarks>
        /// This method is less efficient than the corresponding <see cref="Find"/> method and should only be used 
        /// if the requested code is not known to be part of the enumeration.
        /// </remarks>
        /// <param name="enumValueClass"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        EnumValue TryFind(Type enumValueClass, string code);

        /// <summary>
        /// Trys to find the enumeration value for the specified enumeration class and enumeration code.
        /// Throws a <see cref="EnumValueNotFoundException"/> if the class does not contain the specified code.
        /// </summary>
        /// <remarks>
        /// This method is less efficient than the corresponding <see cref="Find"/> method and should only be used 
        /// if the requested code is not known to be part of the enumeration.
        /// </remarks>
        /// <typeparam name="TEnumValue"></typeparam>
        /// <param name="code"></param>
        /// <returns></returns>
        TEnumValue TryFind<TEnumValue>(string code) where TEnumValue : EnumValue;

        /// <summary>
        /// Adds a new value to the specified enumeration class.
        /// </summary>
        /// <param name="enumValueClass"></param>
        /// <param name="code"></param>
        /// <param name="value"></param>
        /// <param name="description"></param>
        /// <param name="displayOrder"></param>
        /// <returns></returns>
        EnumValue AddValue(Type enumValueClass, string code, string value, string description, float displayOrder, bool deactivated);

        /// <summary>
        /// Updates the value of the specified enumeration class and code, with the supplied arguments.
        /// </summary>
        /// <param name="enumValueClass"></param>
        /// <param name="code"></param>
        /// <param name="value"></param>
        /// <param name="description"></param>
        /// <param name="displayOrder"></param>
        /// <param name="deactivated"></param>
        /// <returns></returns>
        EnumValue UpdateValue(Type enumValueClass, string code, string value, string description, float displayOrder, bool deactivated);

        /// <summary>
        /// Removes the value with the specified code from the specified enumeration class.
        /// </summary>
        /// <param name="enumValueClass"></param>
        /// <param name="code"></param>
        void RemoveValue(Type enumValueClass, string code);
    }
}
