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
using ClearCanvas.Healthcare;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare
{
    /// <summary>
    /// Manages instantiation of noteboxs.
    /// </summary>
    public class NoteboxFactory
    {
        #region Static members

        private static readonly NoteboxFactory _theInstance = new NoteboxFactory();

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static NoteboxFactory Instance
        {
            get { return _theInstance; }
        }

        #endregion

        private readonly List<Type> _noteboxClasses;

        private NoteboxFactory()
        {
            _noteboxClasses = CollectionUtils.Map<ExtensionInfo, Type>(
				new NoteboxExtensionPoint().ListExtensions(), info => info.ExtensionClass.Resolve());
        }

        /// <summary>
        /// Creates an instance of the notebox class as specified by the class name, which may be fully or
        /// only partially qualified.
        /// </summary>
        /// <param name="noteboxClassName"></param>
        /// <returns></returns>
        public Notebox CreateNotebox(string noteboxClassName)
        {
            return CreateNotebox(ResolvePartialClassName(noteboxClassName));
        }

        /// <summary>
        /// Creates an instance of the specified notebox class.
        /// </summary>
        /// <param name="noteboxClass"></param>
        /// <returns></returns>
        public Notebox CreateNotebox(Type noteboxClass)
        {
            return (Notebox)Activator.CreateInstance(noteboxClass);
        }


        private Type ResolvePartialClassName(string noteboxClassName)
        {
            Type noteboxClass = CollectionUtils.SelectFirst(_noteboxClasses,
                delegate(Type t) { return t.FullName.Contains(noteboxClassName); });

            if(noteboxClass == null)
                throw new ArgumentException(string.Format("{0} is not a valid notebox class name.", noteboxClassName));

            return noteboxClass;
        }
    }
}