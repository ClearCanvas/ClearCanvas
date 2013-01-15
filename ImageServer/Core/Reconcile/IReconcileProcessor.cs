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

namespace ClearCanvas.ImageServer.Core.Reconcile
{
	/// <summary>
	/// Defines the interface of processors that processes different types of 'ReconcileStudy' entries
	/// </summary>
	public interface IReconcileProcessor : IDisposable
	{
		/// <summary>
		/// Gets the name of the processor
		/// </summary>
		String Name { get; }

		/// <summary>
		/// Gets the reason why <see cref="Execute"/> failed.
		/// </summary>
		String FailureReason { get;}

		/// <summary>
		/// Initializes the processor with the specified context
		/// </summary>
		/// <param name="context"></param>
		/// <param name="complete"></param>
		void Initialize(ReconcileStudyProcessorContext context, bool complete);

		/// <summary>
		/// Executes the processor
		/// </summary>
		/// <returns></returns>
		bool Execute();
	}
}
