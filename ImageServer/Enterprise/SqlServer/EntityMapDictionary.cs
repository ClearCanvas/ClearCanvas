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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageServer.Enterprise.SqlServer
{
    public static class EntityMapDictionary
    {
        private static readonly object SyncLock = new object();
        private static readonly Dictionary<Type, Dictionary<string, PropertyInfo>> Maps = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

        public static Dictionary<string, PropertyInfo> GetEntityMap(Type entityType)
        {
            lock (SyncLock)
            {
                if (Maps.ContainsKey(entityType))
                    return Maps[entityType];

                Dictionary<string, PropertyInfo> propMap = LoadMap(entityType);
                Maps.Add(entityType, propMap);
                return propMap;
            }
        }

        private static Dictionary<string, PropertyInfo> LoadMap(Type entityType)
        {
            ObjectWalker walker = new ObjectWalker();
            Dictionary<string, PropertyInfo> propMap = new Dictionary<string, PropertyInfo>();

            foreach (IObjectMemberContext member in walker.Walk(entityType))
            {
                EntityFieldDatabaseMappingAttribute map =
                    AttributeUtils.GetAttribute<EntityFieldDatabaseMappingAttribute>(member.Member);
                if (map != null)
                {
                    propMap.Add(map.ColumnName, member.Member as PropertyInfo);
                }
            }

            return propMap;
        }
    }
}