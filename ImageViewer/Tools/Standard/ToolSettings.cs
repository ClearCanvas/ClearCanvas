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
using System.Configuration;
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Tools.Standard.Configuration;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[SettingsGroupDescription("Stores settings for standard tools.")]
	[SettingsProvider(typeof (StandardSettingsProvider))]
	internal sealed partial class ToolSettings
	{
		private ToolModalityBehaviorCollection _cachedToolModalityBehavior;

		public ToolSettings()
		{
        }

        //TODO (Phoenix5): #10730 - remove this when it's fixed.
        #region WebStation Settings Hack
        [ThreadStatic]
        private static ToolSettings _webDefault;

        public static ToolSettings DefaultInstance
        {
            get
            {
                if (Application.GuiToolkitID == GuiToolkitID.Web)
                    return _webDefault ?? (_webDefault = new ToolSettings());

                return Default;
            }
        }
        #endregion

		protected override void OnSettingsLoaded(object sender, SettingsLoadedEventArgs e)
		{
			_cachedToolModalityBehavior = null;

			base.OnSettingsLoaded(sender, e);
		}

		protected override void OnSettingChanging(object sender, SettingChangingEventArgs e)
		{
			if (e.SettingName == "ToolModalityBehavior")
				_cachedToolModalityBehavior = null;

			base.OnSettingChanging(sender, e);
		}

		public ToolModalityBehaviorCollection CachedToolModalityBehavior
		{
			get
			{
				if (_cachedToolModalityBehavior == null)
				{
					ToolModalityBehaviorCollection result;
					try
					{
						result = ToolModalityBehavior;
					}
					catch (Exception)
					{
						result = null;
					}
					_cachedToolModalityBehavior = result ?? new ToolModalityBehaviorCollection();
				}
				return _cachedToolModalityBehavior;
			}
		}
	}
}