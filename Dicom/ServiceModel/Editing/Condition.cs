#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
// 
// For information about the licensing and copyright of this software please
// contact ClearCanvas, Inc. at info@clearcanvas.ca

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Dicom.ServiceModel.Editing
{
    [KnownType("GetKnownTypes")]
    [DataContract(Namespace = DicomEditNamespace.Value)]
    public abstract class Condition : DataContractBase
    {
        private static readonly object _syncLock = new object();
        private static volatile IList<Type> _types;

        static Condition()
        {
        }

        /// <summary>
        /// Gets all known types derived from <see cref="Condition"/>.
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
                assemblies.Add(typeof(Edit).Assembly);

                return _types = (from assembly in assemblies
                                 from type in assembly.GetTypes()
                                 where typeof(Condition).IsAssignableFrom(type)
                                       && type.IsDefined(typeof (DataContractAttribute), false)
                                       && type.IsDefined(typeof (EditTypeAttribute), false)
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

        public abstract bool IsMatch(DicomAttributeCollection dataSet);
    }
}
