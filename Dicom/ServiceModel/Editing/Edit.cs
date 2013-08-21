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
using System.Linq;
using System.Runtime.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Dicom.ServiceModel.Editing
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class EditTypeAttribute : PolymorphicDataContractAttribute
	{
		public EditTypeAttribute(string dataContractGuid)
			: base(dataContractGuid)
		{
		}
	}

	public interface IEditContext
	{
		DicomAttributeCollection DataSet { get; }
		bool Excluded { get; }
		void Exclude();
	}

	[KnownType("GetKnownTypes")]
	[DataContract(Namespace = DicomEditNamespace.Value)]
	public abstract class Edit : DataContractBase
	{
		private static readonly object _syncLock = new object();
		private static volatile IList<Type> _types;

		static Edit()
		{
		}

		/// <summary>
		/// Gets all known types for <see cref="Edit"/> and <see cref="EditSet"/>,
		/// which includes classes derived from <see cref="Condition"/>.
		/// </summary>
		public static IEnumerable<Type> GetKnownTypes()
		{
			if (_types != null)
				return _types;

			lock (_syncLock)
			{
				if (_types != null)
					return _types;

				var assemblies = (from plugin in Platform.PluginManager.Plugins select plugin.Assembly.Resolve()).ToList();
				assemblies.Add(typeof (Edit).Assembly);

				return _types = (from assembly in assemblies
				                 from type in assembly.GetTypes()
				                 where typeof (DataContractBase).IsAssignableFrom(type)
				                       && type.IsDefined(typeof (EditTypeAttribute), false)
				                       && type.IsDefined(typeof (DataContractAttribute), false)
				                 select type).ToList().AsReadOnly();
			}
		}

#if UNIT_TESTS
		/// <summary>
		/// For unit testing only.
		/// </summary>
		public static void SetUnitTestKnownTypes(IEnumerable<Type> types)
		{
			_types = new List<Type>(types);
		}
#endif

		public abstract void Apply(IEditContext context);
	}
}