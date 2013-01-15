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

namespace ClearCanvas.Ris.Application.Common
{
	/// <summary>
	/// Provides options that control the behaviour of worklist item text queries.
	/// </summary>
	[Flags]
	[Serializable]
	public enum WorklistItemTextQueryOptions
	{
		/// <summary>
		/// Specifies that the search query string should be applied against properties
		/// of the patient/order associated with the worklist item.
		/// </summary>
		PatientOrder = 0x001,

		/// <summary>
		/// Specifies that the search query string should be applied against properties
		/// of the scheduled or actual performing staff of the procedure step (assuming the worklist item is based on a procedure step).
		/// </summary>
		ProcedureStepStaff = 0x002,

		/// <summary>
		/// Specifies that identifiers, such as MRN, A#, Healthcard#, found in the query, will use partial matching, instead
		/// of exact matching.
		/// </summary>
		EnablePartialMatchingOnIdentifiers = 0x100,

		/// <summary>
		/// Find items associated with procedure in 'Downtime Recovery Mode', as opposed to live worklist items.
		/// </summary>
		DowntimeRecovery = 0x800,
	}
}
