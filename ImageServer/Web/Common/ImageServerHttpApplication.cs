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
using System.Web;
using ClearCanvas.Common;
using System.Reflection;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageServer.Web.Common
{
    public class ImageServerHttpApplication:HttpApplication
    {
        public override void Init()
        {
            try
            {
                object[] extensions = new HttpApplicationExtensionPoint().CreateExtensions();

                foreach (IHttpApplicationExtension ext in extensions)
                {
                    if (AttributeUtils.HasAttribute<ImageServerModuleAttribute>(ext.GetType(), true))
                    {
                        string methodName = AttributeUtils.GetAttribute<ImageServerModuleAttribute>(ext.GetType()).RegisterMethod;
                        if (String.IsNullOrEmpty(methodName))
                        {
                            Platform.Log(LogLevel.Warn, "ImageServerModuleAttribute RegisterMethod is missing in {0}", ext.GetType().FullName);
                        }
                        else
                        {
                            System.Reflection.MethodInfo method = ext.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
                            if (method != null)
                            {
                                method.Invoke(null, new[] { this });
                            }
                        }
                    }
                    else
                    {
                        Platform.Log(LogLevel.Warn, "{0} must be decorated with ImageServerModuleAttribute", ext.GetType().FullName);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Platform.Log(LogLevel.Warn, ex);
            }

            
            base.Init();
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ImageServerModuleAttribute : Attribute
    {
        public string RegisterMethod { get; set; }
    }
}
