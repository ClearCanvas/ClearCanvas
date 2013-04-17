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
	public class DisplaySetExternalTool : ExternalToolBase
	{
		private readonly IResourceResolver _resourceResolver = new ApplicationThemeResourceResolver(typeof(DisplaySetExternalTool).Assembly);

		private IActionSet _actions;
		private IDisplaySet _selectedDisplaySet;

		public override IActionSet Actions
		{
			get
			{
				if (_actions == null)
				{
					List<IAction> actions = new List<IAction>();
					if (this.SelectedDisplaySet != null && this.SelectedDisplaySet.PresentationImages.Count > 1)
					{
						foreach (IExternal external in ExternalCollection.SavedExternals)
						{
							if (!external.IsValid || string.IsNullOrEmpty(external.Label))
								continue;

							IDisplaySetExternal consumer = external as IDisplaySetExternal;
							if (consumer != null && consumer.CanLaunch(this.SelectedDisplaySet))
							{
								string id = Guid.NewGuid().ToString();
								ActionPath actionPath = new ActionPath(string.Format("imageviewer-contextmenu/MenuExternals/{0}", id), _resourceResolver);
								MenuAction action = new MenuAction(id, actionPath, ClickActionFlags.None, _resourceResolver);
								action.Label = string.Format(SR.FormatOpenDisplaySetWith, consumer.Label);
								action.SetPermissibility(AuthorityTokens.Externals);
								action.SetClickHandler(delegate
								                       	{
								                       		try
								                       		{
								                       			consumer.Launch(this.SelectedDisplaySet);
								                       		}
								                       		catch (Exception ex)
								                       		{
								                       			ExceptionHandler.Report(ex, base.Context.DesktopWindow);
								                       		}
								                       	});
								actions.Add(action);
							}
						}
					}
					_actions = new ActionSet(actions);
				}
				return _actions;
			}
		}

		protected IDisplaySet SelectedDisplaySet
		{
			get { return _selectedDisplaySet; }
			set
			{
				if (_selectedDisplaySet != value)
				{
					_selectedDisplaySet = value;
					_actions = null;
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			_selectedDisplaySet = null;
			base.Dispose(disposing);
		}

		protected override void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			base.OnPresentationImageSelected(sender, e);
			if (e.SelectedPresentationImage != null)
				this.SelectedDisplaySet = e.SelectedPresentationImage.ParentDisplaySet;
			else
				this.SelectedDisplaySet = null;
		}

		protected override void OnExternalsChanged(EventArgs e)
		{
			base.OnExternalsChanged(e);
			_actions = null;
		}
	}
}