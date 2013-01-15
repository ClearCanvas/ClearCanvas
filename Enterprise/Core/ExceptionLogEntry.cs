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
using ClearCanvas.Common;
using System.Threading;
using System.Reflection;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Records information about an exception.
    /// </summary>
    public class ExceptionLogEntry : LogEntry
    {
        private string _exceptionClass;
        private string _message;
    	private string _assemblyName;
    	private string _assemblyLocation;


        /// <summary>
        /// Private no-args constructor to support NHibernate
        /// </summary>
        protected ExceptionLogEntry()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="e"></param>
        /// <param name="details"></param>
        public ExceptionLogEntry(string operation, Exception e, string details)
            :base(operation, details)
        {
            _exceptionClass = e.GetType().FullName;
            _message = e.Message;
			if(e.TargetSite != null)
			{
				Assembly assembly = e.TargetSite.DeclaringType.Assembly;
				_assemblyName = assembly.FullName;
				_assemblyLocation = assembly.Location;
			}
        }

        /// <summary>
        /// Gets or sets the name of the exception class.
        /// </summary>
        public string ExceptionClass
        {
            get { return _exceptionClass; }
            set { _exceptionClass = value; }
        }

        /// <summary>
        /// Gets or sets the top-level message exposed by the exception.
        /// </summary>
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

		/// <summary>
		/// Gets or sets the name of the assembly that threw the exception.
		/// </summary>
    	public string AssemblyName
    	{
			get { return _assemblyName; }
			set { _assemblyName = value; }
    	}

		/// <summary>
		/// Gets or sets the disk location of the assembly that threw the exception.
		/// </summary>
		public string AssemblyLocation
    	{
			get { return _assemblyLocation; }
			set { _assemblyLocation = value; }
    	}
    }
}
