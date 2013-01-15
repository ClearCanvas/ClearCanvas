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
using System.ComponentModel;

namespace ClearCanvas.ImageViewer.Layout
{
	/// <summary>
	/// Provides a default implementation of <see cref="IHpProperty"/>.
	/// </summary>
	/// <remarks>
	/// This class can be used as-is or subclassed for advanced functionality.
	/// </remarks>
	/// <typeparam name="TProperty"></typeparam>
    public class HpProperty<TProperty> : IHpProperty
	{
        public delegate TProperty ValueGetter();
        public delegate void ValueSetter(TProperty value);

		private readonly ValueGetter _getter;
		private readonly ValueSetter _setter;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="description"></param>
		/// <param name="getter"></param>
		/// <param name="setter"></param>
        public HpProperty(string name, string description, ValueGetter getter, ValueSetter setter)
		{
		    Type = typeof (TProperty);
            DisplayName = name;
            Description = description;
            _getter = getter;
            _setter = setter;
        }

		///<summary>
		/// Constructor
		///</summary>
		///<param name="name"></param>
		///<param name="description"></param>
		///<param name="category"></param>
		///<param name="getter"></param>
		///<param name="setter"></param>
		public HpProperty(string name, string description, string category, ValueGetter getter, ValueSetter setter)
		{
			Type = typeof(TProperty);
			DisplayName = name;
			Description = description;
			Category = category;
			_getter = getter;
			_setter = setter;
		}

	    #region Implementation of IHpProperty

        public Type Type { get; protected set; }

		/// <summary>
		/// Gets the display name of this property for display in the user-interface.
		/// </summary>
		public string DisplayName { get; private set; }

		/// <summary>
		/// Gets the description of this property for display in the user-interface.
		/// </summary>
		public string Description { get; private set; }

		/// <summary>
		/// Gets the category of this property for display in the user-interface.
		/// </summary>
		public string Category { get; private set; }

		/// <summary>
		/// Gets the type converter for this property.
		/// </summary>
		public TypeConverter Converter { get; set; }

		/// <summary>
		/// Gets a value indicating whether this property can be edited by a custom dialog box.
		/// </summary>
		public virtual bool HasEditor
		{
			get { return false; }
		}

		/// <summary>
		/// Called to invoke custom editing of this property, if <see cref="IHpProperty.HasEditor"/> returns true. 
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public virtual bool EditProperty(IHpPropertyEditContext context)
		{
			return false;
		}

        public virtual bool CanSetValue
        {
            get { return true; }
        }

        object IHpProperty.Value
        {
            get { return Value; }
            set
            {
                if (!CanSetValue)
                    throw new InvalidOperationException("The value property cannot be set.");

                Value = (TProperty)value;
            }
        }
        
        #endregion

	    /// <summary>
		/// Gets or sets the value associated with this property.
		/// </summary>
		protected TProperty Value
		{
			get { return _getter(); }
			set { _setter(value); }
		}
	}
}
