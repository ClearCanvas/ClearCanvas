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
using System.Reflection;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Core
{
	public interface IServiceOperationRecorderContext
	{
		/// <summary>
		/// Gets the logical name of the operation.
		/// </summary>
		string OperationName { get; }

		/// <summary>
		/// Gets the class that provides the service implementation.
		/// </summary>
		Type ServiceClass { get; }

		/// <summary>
		/// Gets the <see cref="MethodInfo"/> object describing the operation.
		/// </summary>
		MethodInfo OperationMethodInfo { get; }

		/// <summary>
		/// Gets the request object passed to the operation.
		/// </summary>
		object Request { get; }

		/// <summary>
		/// Gets the response object returned from the operation, or null if an exception was thrown.
		/// </summary>
		object Response { get; }

		/// <summary>
		/// Gets the current change set.
		/// </summary>
		EntityChangeSet ChangeSet { get; }

		/// <summary>
		/// Writes the specified audit message, using the specified operation name.
		/// </summary>
		/// <param name="operation"></param>
		/// <param name="message"></param>
		void Write(string operation, string message);

		/// <summary>
		/// Writes the specified audit message, using the default operation name.
		/// </summary>
		/// <param name="message"></param>
		void Write(string message);
	}



	/// <summary>
	/// Defines an interface to an object that records information about the invocation of a service operation
	/// for auditing purposes.
	/// </summary>
	public interface IServiceOperationRecorder
	{
		/// <summary>
		/// Gets the audit log category under which to log the message.
		/// </summary>
		string Category { get; }

		/// <summary>
		/// Called after the body of the operation has executed, but prior to commit.
		/// </summary>
		/// <param name="recorderContext"></param>
		/// <param name="persistenceContent"></param>
		void PreCommit(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContent);

		/// <summary>
		/// Called after the transaction has been committed.
		/// </summary>
		/// <param name="recorderContext"></param>
		void PostCommit(IServiceOperationRecorderContext recorderContext);
	}
}
