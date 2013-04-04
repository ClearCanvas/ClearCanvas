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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Externals.CoreTools
{
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class PresentationImageExternalTool : ExternalToolBase
	{
		private readonly IResourceResolver _resourceResolver = new ApplicationThemeResourceResolver(typeof(PresentationImageExternalTool).Assembly);

		private IActionSet _actions = null;
		private IPresentationImage _selectedPresentationImage = null;

		public override IActionSet Actions
		{
			get
			{
				if (_actions == null)
				{
					List<IAction> actions = new List<IAction>();
					foreach (IExternal external in ExternalCollection.SavedExternals)
					{
						if (!external.IsValid || string.IsNullOrEmpty(external.Label))
							continue;

						IPresentationImageExternal consumer = external as IPresentationImageExternal;
						if (consumer != null && consumer.CanLaunch(base.SelectedPresentationImage))
						{
							string id = Guid.NewGuid().ToString();
							ActionPath actionPath = new ActionPath(string.Format("imageviewer-contextmenu/MenuExternals/{0}", id), _resourceResolver);
							MenuAction action = new MenuAction(id, actionPath, ClickActionFlags.None, _resourceResolver);
							action.Label = string.Format(SR.FormatOpenImageWith, consumer.Label);
							action.SetPermissibility(AuthorityTokens.Externals);
							action.SetClickHandler(delegate
							                       	{
							                       		try
							                       		{
							                       			consumer.Launch(base.SelectedPresentationImage);
							                       		}
							                       		catch (Exception ex)
							                       		{
							                       			ExceptionHandler.Report(ex, base.Context.DesktopWindow);
							                       		}
							                       	});
							actions.Add(action);
						}
					}
					_actions = new ActionSet(actions);
				}
				return _actions;
			}
		}

		protected override void Dispose(bool disposing)
		{
			_selectedPresentationImage = null;
			base.Dispose(disposing);
		}

		protected override void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			base.OnPresentationImageSelected(sender, e);
			if (_selectedPresentationImage != this.SelectedPresentationImage)
			{
				_selectedPresentationImage = this.SelectedPresentationImage;
				_actions = null;
			}
		}

		protected override void OnExternalsChanged(EventArgs e)
		{
			base.OnExternalsChanged(e);
			_actions = null;
		}
	}
}