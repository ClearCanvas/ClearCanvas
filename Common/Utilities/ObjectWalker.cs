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
using System.Reflection;

namespace ClearCanvas.Common.Utilities
{
    /// <summary>
    /// Defines a callback interface used by the <see cref="ObjectWalker"/> class.
    /// </summary>
    public interface IObjectMemberContext
    {
        /// <summary>
        /// Gets the object instance being walked, or null if a type is being walked.
        /// </summary>
        object Object { get;}

        /// <summary>
        /// Gets the property or field that the walker is currently at.
        /// </summary>
        MemberInfo Member { get;}

        /// <summary>
        /// Gets the type of the property or field that the walker is currently at.
        /// </summary>
        Type MemberType { get; }

        /// <summary>
        /// Gets or sets the value of the property or field that the walker is currently at,
        /// assuming an object instance is being walked.
        /// </summary>
        object MemberValue { get; set; }

		/// <summary>
		/// Gets a value indicating whether this member is read-only.
		/// </summary>
		/// <remarks>
		/// Attempting to set the value of a read-only member (via <see cref="MemberValue"/>) will
		/// throw an exception.
		/// </remarks>
		bool IsReadOnly { get; }
    }

    /// <summary>
    /// Utility class for walking the properties and/or fields of an object.
    /// </summary>
    /// <remarks>
    /// By default, the public properties and fields of the object will be included in the walk.
    /// Set the properties of the <see cref="ObjectWalker"/> instance to optionally include
    /// private fields and/or properties, or to optionally exclude public fields/properties. 
    /// </remarks>
    public class ObjectWalker
    {
        #region IObjectWalkerContext implementations

        class PropertyContext : IObjectMemberContext
        {
            private readonly PropertyInfo _property;
            private readonly object _obj;

            public PropertyContext(object obj, PropertyInfo property)
            {
                _property = property;
                _obj = obj;
            }

            public object Object
            {
                get { return _obj; }
            }

            public Type MemberType
            {
                get { return _property.PropertyType; }
            }

            public MemberInfo Member
            {
                get { return _property;}
            }

            public object MemberValue
            {
                get { return _property.GetValue(_obj, BindingFlags.Public | BindingFlags.NonPublic, null, null, null);}
                set { _property.SetValue(_obj, value, BindingFlags.Public | BindingFlags.NonPublic, null, null, null); }
            }

        	public bool IsReadOnly
        	{
        		get { return _property.GetSetMethod() == null; }
        	}
        }

        class FieldContext : IObjectMemberContext
        {
            private readonly FieldInfo _field;
            private readonly object _obj;

            public FieldContext(object obj, FieldInfo field)
            {
                _field = field;
                _obj = obj;
            }

            public object Object
            {
                get { return _obj; }
            }

            public Type MemberType
            {
                get { return _field.FieldType; }
            }

            public MemberInfo Member
            {
                get { return _field;}
            }

            public object MemberValue
            {
                get { return _field.GetValue(_obj);}
                set { _field.SetValue(_obj, value);}
            }

        	public bool IsReadOnly
        	{
				get { return _field.IsInitOnly; }
        	}
        }

        #endregion

        #region Private fields

        private bool _includeNonPublicFields;
        private bool _includePublicFields;
        private bool _includeNonPublicProperties;
        private bool _includePublicProperties;

        private Predicate<MemberInfo> _memberFilter;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public ObjectWalker()
			:this(null)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
		/// <param name="memberFilter"></param>
        public ObjectWalker(Predicate<MemberInfo> memberFilter)
        {
			// includ public fields and properties by default
			_includePublicFields = true;
			_includePublicProperties = true;
			_memberFilter = memberFilter;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether to include non-public fields in the walk.
        /// </summary>
        public bool IncludeNonPublicFields
        {
            get { return _includeNonPublicFields; }
            set { _includeNonPublicFields = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to include public fields in the walk.
        /// </summary>
        public bool IncludePublicFields
        {
            get { return _includePublicFields; }
            set { _includePublicFields = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to include non-public properties in the walk.
        /// </summary>
        public bool IncludeNonPublicProperties
        {
            get { return _includeNonPublicProperties; }
            set { _includeNonPublicProperties = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to include public properties in the walk.
        /// </summary>
        public bool IncludePublicProperties
        {
            get { return _includePublicProperties; }
            set { _includePublicProperties = value; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Walks properties and/or fields of the specified object.
        /// </summary>
        /// <param name="obj"></param>
		public IEnumerable<IObjectMemberContext> Walk(object obj)
        {
            Platform.CheckForNullReference(obj, "obj");

            return WalkHelper(obj.GetType(), obj);
        }

        /// <summary>
        /// Walks properties and/or fields of the specified type.
        /// </summary>
        /// <param name="type"></param>
		public IEnumerable<IObjectMemberContext> Walk(Type type)
        {
            return WalkHelper(type, null);
        }

        #endregion

        private IEnumerable<IObjectMemberContext> WalkHelper(Type type, object instance)
        {
            // walk properties
            if (_includePublicProperties || _includeNonPublicProperties)
            {
                var bindingFlags = BindingFlags.Instance;
                if (_includePublicProperties)
                    bindingFlags |= BindingFlags.Public;
                if (_includeNonPublicProperties)
                    bindingFlags |= BindingFlags.NonPublic;
                foreach (var property in type.GetProperties(bindingFlags))
                {
                    if (_memberFilter == null || _memberFilter(property))
                    {
                    	yield return new PropertyContext(instance, property);
                    }
                }
            }

            // walk fields
            if (_includePublicFields || _includeNonPublicFields)
            {
                var bindingFlags = BindingFlags.Instance;
                if (_includePublicFields)
                    bindingFlags |= BindingFlags.Public;
                if (_includeNonPublicFields)
                    bindingFlags |= BindingFlags.NonPublic;
                foreach (var field in type.GetFields(bindingFlags))
                {
                    if (_memberFilter == null || _memberFilter(field))
                    {
						yield return new FieldContext(instance, field);
					}
                }
            }
        }
    }
}
