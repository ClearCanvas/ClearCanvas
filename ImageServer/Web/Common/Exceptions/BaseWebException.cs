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

namespace ClearCanvas.ImageServer.Web.Common.Exceptions
{
    public class BaseWebException : Exception
    {
        private string _logMessage;
        private string _errorMessage;
        private string _errorDescription;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }

        public string LogMessage
        {
            get { return _logMessage; }
            set { _logMessage = value; }
        }

        public string ErrorDescription
        {
            get { return _errorDescription; }
            set { _errorDescription = value; }
        }

        public BaseWebException(string errorMessage, string logMessage, string errorDescription)
        {
            _errorMessage = errorMessage;
            _errorDescription = errorDescription;
            _logMessage = logMessage;
        }

        public BaseWebException() {}

    }
}
