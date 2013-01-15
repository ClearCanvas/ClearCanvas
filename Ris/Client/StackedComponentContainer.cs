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
using System.Linq;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client
{

	/// <summary>
	/// Defines an extension point for views onto the <see cref="StackedComponentContainer"/>.
	/// </summary>
	public sealed class StackedComponentContainerViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(StackedComponentContainerViewExtensionPoint))]
	public class StackedComponentContainer : PagedComponentContainer<StackedComponentContainer.StackedComponentContainerPage>
	{
		private StackedComponentContainerPage _initialPage;

		public class StackedComponentContainerPage : ContainerPage
		{
			public StackedComponentContainerPage(IApplicationComponent component)
				: base(component)
			{
			}
		}

		public void Show(IApplicationComponent component)
		{
			var page = this.Pages.FirstOrDefault(p => p.Component == component);
			if(page == null)
			{
				page = new StackedComponentContainerPage(component);
				this.Pages.Add(page);
			}

			if (this.IsStarted)
			{
				this.CurrentPage = page;
			}
			else
			{
				// work around an issue with the PagedComponentContainer, where setting CurrentPage 
				// when the component is not started causes problems
				_initialPage = page;
			}
		}

		public override void Start()
		{
			base.Start();

			// set the correct initial page
			this.CurrentPage = _initialPage;
		}
	}
}
