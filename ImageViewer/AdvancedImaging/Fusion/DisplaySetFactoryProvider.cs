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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Layout.Basic;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	//TODO (CR Sept 2010): for as often as we make these providers return more than one object,
	//we should just make the objects themselves the extensions.
	[ExtensionOf(typeof (DisplaySetFactoryProviderExtensionPoint))]
	public class DisplaySetFactoryProvider : IDisplaySetFactoryProvider
	{
		public DisplaySetFactoryProvider()
		{
			if (!PermissionsHelper.IsInRole(AuthorityTokens.ViewerClinical))
				throw new NotSupportedException();
		}

		#region IDisplaySetFactoryProvider Members

		public IEnumerable<IDisplaySetFactory> CreateDisplaySetFactories(IPresentationImageFactory presentationImageFactory)
		{
			yield return new PETFusionDisplaySetFactory(PETFusionType.CT);
		}

		#endregion
	}
}