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
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise
{
    /// <summary>
    /// Used to represent a specific parameter to a stored procedure.
    /// </summary>
    /// <typeparam name="T">The type associated with the parameter.</typeparam>
    public class ProcedureParameter<T> : SearchCriteria
    {
        private T _value;
		private bool _output = false;

		/// <summary>
		/// Constructor for input parameters.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
        public ProcedureParameter(String key, T value)
            : base(key)
        {
            _value = value;
        }

		/// <summary>
		/// Contructor for output parameters.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="output"></param>
		public ProcedureParameter(String key)
			: base(key)
		{
			_output = true;
		}

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected ProcedureParameter(ProcedureParameter<T> other)
            : base(other)
        {
            _value = other._value;
            _output = other._output;
        }

        public override object Clone()
        {
            return new ProcedureParameter<T>(this);
        }

		public bool Output
		{
			get { return _output; }
		}

		public T Value
		{
			get { return _value; }
			set { _value = value; }
		}
    }
}
