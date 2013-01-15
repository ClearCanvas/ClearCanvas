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

namespace ClearCanvas.Enterprise.Tests
{
    public class ServiceLayerTestHelper
    {
        /// <summary>
        /// This method exposes the internal ServicLayer.CurrentContext setter.  This method may be used when a ServiceLayer-derived object is manually instantiated 
        /// outside of the normal framework mechanism (eg when testing via NUnit) to manually set the PersistenceContext.  Allows for a mock PersistenceContext to be 
        /// used, which in turn allows mock EntityBroker objects to be used to return test data.
        /// </summary>
        /// <example>
        /// <code>
        ///    _mocks = new Mockery();
        ///    _mockPersistanceContext = _mocks.NewMock&lt;IPersistenceContext&gt;();
        ///    _service= new ServiceLayer();
        ///    ServiceLayerTestHelper.SetServiceLayerPersistenceContext(
        ///        _service, _mockPersistanceContext);
        /// </code>
        /// </example>
        /// <see>ClearCanvas.Ris.Services.Tests.AdtServiceTest</see>
        /// <param name="serviceLayer"></param>
        /// <param name="context"></param>
        public static void SetServiceLayerPersistenceContext(ServiceLayer serviceLayer, IPersistenceContext context)
        {
            serviceLayer.CurrentContext = context;
        }
    }
}
