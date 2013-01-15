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
using System.Runtime.Serialization;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Imex
{
    [ExtensionOf(typeof(XmlDataImexExtensionPoint))]
    [ImexDataClass("Enumeration")]
    public class XmlEnumValueImex : XmlDataImexBase
    {
        [DataContract]
        public class EnumerationData
        {
            [DataMember]
            public string EnumerationClass;

            [DataMember]
            public List<EnumerationMemberData> Members;
        }

        [DataContract]
        public class EnumerationMemberData
        {
            public EnumerationMemberData()
	        {
	        }

            public EnumerationMemberData(EnumValue value)
	        {
                this.Code = value.Code;
                this.Value = value.Value;
                this.Description = value.Description;
                this.DisplayOrder = value.DisplayOrder;
            	this.Deactivated = value.Deactivated;
	        }

            [DataMember]
            public string Code;

            [DataMember]
            public string Value;

            [DataMember]
            public string Description;

            [DataMember]
            public float DisplayOrder;

			[DataMember]
			public bool Deactivated;
		}

        #region ExportItem class

        class ExportItem : IExportItem
        {
            private readonly EnumerationData _data;

            public ExportItem(EnumerationData data)
            {
                _data = data;
            }

            public void Write(XmlWriter writer)
            {
                XmlDataImexBase.Write(writer, _data);
            }
        }

        #endregion

        protected override IEnumerable<IExportItem> ExportCore()
        {
            using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Read))
            {
                IReadContext context = (IReadContext) PersistenceScope.CurrentContext;
                IMetadataBroker metaBroker = context.GetBroker<IMetadataBroker>();
                IEnumBroker enumBroker = context.GetBroker<IEnumBroker>();
                List<Type> enumClasses = CollectionUtils.Sort(metaBroker.ListEnumValueClasses(),
                    delegate (Type x, Type y) { return x.FullName.CompareTo(y.FullName);});
                foreach (Type enumClass in enumClasses)
                {
                    EnumerationData data = new EnumerationData();
                    data.EnumerationClass = enumClass.FullName;
                    data.Members = CollectionUtils.Map<EnumValue, EnumerationMemberData>(enumBroker.Load(enumClass, true),
                        delegate(EnumValue v) { return new EnumerationMemberData(v); });

                    yield return new ExportItem(data);
                }

                scope.Complete();
            }
        }

        protected override void ImportCore(IEnumerable<IImportItem> items)
        {
            using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update))
            {
                IUpdateContext context = (IUpdateContext) PersistenceScope.CurrentContext;
                context.ChangeSetRecorder.OperationName = this.GetType().FullName;

                IMetadataBroker metaBroker = context.GetBroker<IMetadataBroker>();
                IEnumBroker enumBroker = context.GetBroker<IEnumBroker>();
                IList<Type> enumClasses = metaBroker.ListEnumValueClasses();

                foreach (IImportItem item in items)
                {
                    EnumerationData data = (EnumerationData) Read(item.Read(), typeof(EnumerationData));

                    // find the enum class
                    Type enumClass = CollectionUtils.SelectFirst(enumClasses,
                        delegate(Type ec)
                        {
                            return ec.FullName == data.EnumerationClass;
                        });

					if(enumClass == null)
					{
						Platform.Log(LogLevel.Error, string.Format("{0} is not a valid enumeration class name.", data.EnumerationClass));
						continue;
					}

					IList<EnumValue> existingValues = enumBroker.Load(enumClass, true);
					foreach (EnumerationMemberData md in data.Members)
					{
						// check if a conflicting value exists
						// (this can happen if there is existing data in the db with the same value but different code)
						EnumValue conflict = CollectionUtils.SelectFirst(existingValues,
							delegate(EnumValue v) { return v.Code != md.Code && v.Value == md.Value; });

						if(conflict != null)
						{
							Platform.Log(LogLevel.Error, string.Format("{0} value {1} conflicts with existing value {2} and will not be imported.",
								data.EnumerationClass, md.Code, conflict.Code));
							continue;
						}

						// check if the value already exists
						EnumValue value = CollectionUtils.SelectFirst(existingValues,
							delegate(EnumValue v) { return v.Code == md.Code; });

						if (value == null)
						{
							// value does not exist - add it
							value = enumBroker.AddValue(enumClass, md.Code, md.Value, md.Description, md.DisplayOrder, md.Deactivated);
							existingValues.Add(value);
						}
						else
						{
							// value exists - update it
							enumBroker.UpdateValue(enumClass, md.Code, md.Value, md.Description, md.DisplayOrder, md.Deactivated);
						}
					}

					context.SynchState();
				}

                scope.Complete();
            }
        }
    }
}
