using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageServer.Common.WorkQueue
{
    public static class WorkQueueDataTypeProvider
    {
        private static readonly List<Type> KnownTypes = (from p in Platform.PluginManager.Plugins
                                                         from t in p.Assembly.Resolve().GetTypes()
                                                         let a = AttributeUtils.GetAttribute<WorkQueueDataTypeAttribute>(t)
                                                         where (a != null)
                                                         select t).ToList();

        public static IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider ignored)
        {
            return KnownTypes;
        }

        public static Type[] GetTypeArray()
        {
            return KnownTypes.ToArray();
        }
    }
}
