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

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Shreds.Merge
{
	/// <summary>
	/// Defines an interface for handling an asynchronous merge operation in stages.
	/// </summary>
	public interface IMergeHandler
	{
		/// <summary>
		/// Gets a value indicating whether this handler supports merging of the specified target.
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		bool SupportsTarget(Entity target);

		/// <summary>
		/// Asks this handler to perform part of the merge operation, beginning at the specified stage.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="stage"></param>
		/// <param name="context"></param>
		/// <returns>The stage at which the merge operation should continue next.</returns>
		int Merge(Entity target, int stage, IPersistenceContext context);
	}
}
