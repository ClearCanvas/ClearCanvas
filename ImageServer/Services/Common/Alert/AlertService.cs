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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.ServiceModel;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Services.Common.Alert
{
    /// <summary>
    /// Alert record service
    /// </summary>
    [ServiceImplementsContract(typeof(IAlertService))]
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class AlertService : IApplicationServiceLayer, IAlertService
    {
        #region Private Members
        private IAlertServiceExtension[] _extensions;
        #endregion

        #region Private Methods
        
        private IAlertServiceExtension[] GetExtensions()
        {
            if (_extensions == null)
            {
                _extensions =
                    CollectionUtils.ToArray<IAlertServiceExtension>(new AlertServiceExtensionPoint().CreateExtensions());
            }

            return _extensions;
        }

        #endregion

        #region IAlertService Members

        public void GenerateAlert(ImageServer.Common.Alert alert)
        {
            IAlertServiceExtension[] extensions = GetExtensions();
            foreach(IAlertServiceExtension ext in extensions)
            {
                try
                {
                    ext.OnAlert(alert);    
                }
                catch(Exception e)
                {
                    Platform.Log(LogLevel.Error, e, "Error occurred when calling {0} OnAlert()", ext.GetType());
                }
            }

        }
       

        #endregion
    }
}