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
using System.Threading;
using ClearCanvas.Common.UsageTracking;
using ClearCanvas.Common.Shreds;

namespace ClearCanvas.Ris.Shreds.UsageTracking
{
    [ExtensionOf(typeof(ShredExtensionPoint))]
    class UsageTrackingShred: Shred
    {
        private readonly string _className;
        private UsageTrackingService _service;

        #region Constructors

        public UsageTrackingShred()
        {
            _className = GetType().ToString();
        }

        #endregion

        public override void Start()
        {
            Platform.Log(LogLevel.Info, string.Format(ClearCanvas.Common.SR.ShredStarting, this.GetDisplayName()));

            try
            {
                _service = new UsageTrackingService();
                _service.Start();
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Fatal, e, "Unexpected exception starting {0}", GetDisplayName());
            }
        }

        public override void Stop()
        {
            Platform.Log(LogLevel.Info, string.Format(ClearCanvas.Common.SR.ShredStopping, this.GetDisplayName()));
            if (_service != null)
                _service.Stop();
        }

        public override string GetDisplayName()
        {
            return SR.UsageTrackingShred;
        }

        public override string GetDescription()
        {
            return SR.UsageTrackingShred;
        }
    }

   
}
