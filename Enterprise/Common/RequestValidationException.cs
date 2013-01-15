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
using System.Runtime.Serialization;
using System.ServiceModel;

using ClearCanvas.Common;
using ClearCanvas.Common.Specifications;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using System.Threading;

namespace ClearCanvas.Enterprise.Common
{
    /// <summary>
    /// Used by a service to indicate to the client that a request was rejected because it was invalid.  The client will likely
    /// display the contained message to the end user.  Therefore, the message should be appropriate for the end-user.
    /// </summary>
    [Serializable]
    public class RequestValidationException : Exception
    {
        public RequestValidationException(string message)
            :base(message)
        {
        }

        public RequestValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

}
