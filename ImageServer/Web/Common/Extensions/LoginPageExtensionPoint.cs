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
using System.Text;
using ClearCanvas.Common;
using System.Web.UI;

namespace ClearCanvas.ImageServer.Web.Common.Extensions
{
    public sealed class ExtensibleAttribute : Attribute
    {
        public Type ExtensionPoint;
    }

    public interface ILoginPage
    {
        Control SplashScreenControl { get; }
    }

    public interface ILoginPageExtension
    {
        void OnLoginPageInit(Page page);
    }

    [ExtensionPoint]
    public class LoginPageExtensionPoint : ExtensionPoint<ILoginPageExtension>
    {

    }


    public interface IAboutPage
    {
        Control ExtensionContentParent { get; }
    }

    public interface IAboutPageExtension
    {
        void OnAboutPageInit(Page page);
    }

    [ExtensionPoint]
    public class AboutPageExtensionPoint : ExtensionPoint<IAboutPageExtension>
    {

    }
}
