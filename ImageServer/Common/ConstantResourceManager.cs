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

using System.Collections.Generic;
using System.Globalization;

namespace ClearCanvas.ImageServer.Common
{
    interface IConstantResourceManager
    {
        CultureInfo Culture { get; }
        object this[string key] { get; }
    }


    /// <summary>
    /// Provided access to resources whose localized values are hard-coded
    /// </summary>
    public static class ConstantResourceManager
    {
        private static readonly object _syncLock = new object();
        private static readonly List<IConstantResourceManager> _list = new List<IConstantResourceManager>();
        private static readonly Dictionary<CultureInfo, IConstantResourceManager> _map = new  Dictionary<CultureInfo, IConstantResourceManager> ();

        private static readonly IConstantResourceManager _default = new ConstantResourceManager_EN();

        static ConstantResourceManager()
        {

            _list.Add(new ConstantResourceManager_EN());
            _list.Add(new ConstantResourceManager_ES());
        }

        private static IConstantResourceManager ResourceManager
        {
            get
            {
                lock(_syncLock)
                {
                    IConstantResourceManager match;
                    if (_map.TryGetValue(CultureInfo.CurrentUICulture, out match))
                        return match;

                    // find exact match
                    foreach (var item in _list)
                    {
                        if (item.Culture.Equals(CultureInfo.CurrentUICulture))
                        {
                            match = item;
                            break;
                        }
                    }

                    if (match == null)
                    {
                        // find the ancestor of the current culture
                        List<CultureInfo> ancestors = new List<CultureInfo>();
                        var parent = CultureInfo.CurrentUICulture.Parent;

                        // Walk through the hierarchy. 
                        // The parent of a specific culture is a neutral culture, 
                        // the parent of a neutral culture is the InvariantCulture, 
                        // and the parent of the InvariantCulture is the invariant culture itsel
                        while (parent!=null && !parent.Equals(CultureInfo.InvariantCulture))
                        {
                            ancestors.Add(parent);
                            parent = parent.Parent;
                        }

                        foreach (var item in _list)
                        {
                            if (ancestors.Contains(item.Culture))
                            {
                                match = item;
                                break;
                            }
                        }
                    }

                    // fallback to default
                    if (match == null)
                        match = _default;

                    _map[CultureInfo.CurrentUICulture] = match;

                    return match;
                }
                
            }
        }

        public static string ModifiedInstallation
        {
            get
            {
                return ResourceManager["ModifiedInstallation"] as string;
            }
        }
    }

    class ConstantResourceManagerBase: IConstantResourceManager
    {
        private readonly Dictionary<string, object> _dict = new Dictionary<string, object>();
        
        public ConstantResourceManagerBase(CultureInfo culture)
        {
            Culture = culture;
        }

        public CultureInfo Culture { get; private set; }

        public object this[string key]
        {
            get
            {
                object value;
                if (_dict.TryGetValue(key, out value))
                    return value;

                return null;
            }
            set
            {
                _dict[key] = value;
            }
        }
    }

    class ConstantResourceManager_EN : ConstantResourceManagerBase
    {
        public ConstantResourceManager_EN():base(new CultureInfo("en"))
        {
            // English
            this["ModifiedInstallation"] = "Modified Installation";
        }

    }

    class ConstantResourceManager_ES : ConstantResourceManagerBase
    {
        public ConstantResourceManager_ES():base(new CultureInfo("es"))
        {
            // Spanish
            this["ModifiedInstallation"] = "Instalación Modificado";
        }
    }
}