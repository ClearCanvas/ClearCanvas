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
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
    public static class WorkItemRequestHelper
    {
        private static readonly Type[] _requestRuntimeTypes = InternalGetRequestRuntimeTypes();
        private static readonly string[] _activityTypes = InternalGetActivityTypes();
        private static readonly IDictionary<WorkItemConcurrency, string[]> _workItemTypesByConcurrency = InternalGetWorkItemTypesByConcurrency();

        public static List<Type> GetWorkItemRequestRuntimeTypes()
        {
            return new List<Type>(_requestRuntimeTypes);
        }
        
        public static List<string> GetWorkItemTypes(this WorkItemConcurrency concurrency)
        {
            return new List<string>(_workItemTypesByConcurrency[concurrency]);
        }

        public static List<string> GetActivityTypes()
        {
            return new List<string>(_activityTypes);
        }

        public static Dictionary<WorkItemConcurrency, List<string>> GetWorkItemTypesByConcurrency()
        {
            return _workItemTypesByConcurrency.ToDictionary(k => k.Key, v => v.Value.ToList());
        }

        private static Type[] InternalGetRequestRuntimeTypes()
        {
            var types = (from p in Platform.PluginManager.Plugins
                         from t in p.Assembly.Resolve().GetTypes()
                         let a = AttributeUtils.GetAttribute<WorkItemRequestAttribute>(t)
                         where (a != null)
                         select t);

            return types.ToArray();
        }

        private static string[] InternalGetActivityTypes()
        {
            // build the contract map by finding all types having a T attribute
            var types = InternalGetRequestRuntimeTypes();
            var activityTypes = types.Select(t => Activator.CreateInstance(t, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, null, null))
                .OfType<WorkItemRequest>()
                .Select(request => request.ActivityTypeString).ToList();

            return activityTypes.Distinct().ToArray();
        }

        private static IDictionary<WorkItemConcurrency, string[]> InternalGetWorkItemTypesByConcurrency()
        {
            var types = InternalGetRequestRuntimeTypes();
            var requestsByConcurrency = types.Select(t => Activator.CreateInstance(t, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, null, null))
                .OfType<WorkItemRequest>()
                .GroupBy(w => w.ConcurrencyType);

            return requestsByConcurrency.ToDictionary(k => k.Key, v => v.Select(w => w.WorkItemType).ToArray());
        }
    }
}