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

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// When applied to a service operation, specifies that the operation should be
    /// recorded in the audit log using the specified recorder class.
    /// </summary>
    /// <remarks>
    /// The auditor class must implement <see cref="IServiceOperationRecorder"/> and have
    /// a default constructor.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class AuditRecorderAttribute : Attribute
    {
    	/// <summary>
        /// Constructor
        /// </summary>
        /// <param name="recorderClass"></param>
        public AuditRecorderAttribute(Type recorderClass)
        {
            RecorderClass = recorderClass;
        }

    	/// <summary>
    	/// Gets the implementation of <see cref="IServiceOperationRecorder"/> that
    	/// will create the audit log entry.
    	/// </summary>
    	internal Type RecorderClass { get; private set; }
    }
}
