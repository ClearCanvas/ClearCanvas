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

namespace ClearCanvas.Dicom
{
	/// <summary>
	/// Represents errors in DICOM data.
	/// </summary>
	[Serializable]
	public class DicomDataException : DicomException
	{
		/// <summary>
		/// Initializes a new instance of <see cref="DicomDataException"/> with a default error message.
		/// </summary>
		public DicomDataException()
			: base("A DICOM data error has occured.") {}

		/// <summary>
		/// Initializes a new instance of <see cref="DicomDataException"/> with a specified error message.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="args">An object array that contains zero or more items to format in the error message.</param>
		public DicomDataException(string message, params object[] args)
			: base(message, args) {}

		/// <summary>
		/// Initializes a new instance of <see cref="DicomDataException"/> with a specified error message
		/// and a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or null if no inner exception is specified.</param>
		/// <param name="args">An object array that contains zero or more items to format in the error message.</param>
		public DicomDataException(string message, Exception innerException, params object[] args)
			: base(message, innerException, args) {}

		/// <summary>
		/// Initializes a new instance of <see cref="DicomDataException"/> with serialized data.
		/// </summary>
		/// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
		protected DicomDataException(SerializationInfo info, StreamingContext context)
			: base(info, context) {}
	}
}