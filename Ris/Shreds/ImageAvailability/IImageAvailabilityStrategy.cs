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

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Shreds.ImageAvailability
{
	/// <summary>
	/// Defines an interface to a strategy for determining the Image Availability of a Procedure.
	/// </summary>
	public interface IImageAvailabilityStrategy
	{
		/// <summary>
		/// Computes the <see cref="Healthcare.ImageAvailability"/> for a given <see cref="Procedure"/>.
		/// </summary>
		/// <remarks>
		/// This method must compute and return the image availability for the specified procedure.  The persistence-context
		/// is provided in case it is necessary to query parts of the model that are not reachable from the procedure,
		/// however, the model should not be updated (this method should be free of side-effects).
		/// </remarks>
		/// <param name="procedure"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		Healthcare.ImageAvailability ComputeProcedureImageAvailability(Procedure procedure, IPersistenceContext context);
	}

	[ExtensionPoint]
	public class ImageAvailabilityStrategyExtensionPoint : ExtensionPoint<IImageAvailabilityStrategy>
	{
	}
}
