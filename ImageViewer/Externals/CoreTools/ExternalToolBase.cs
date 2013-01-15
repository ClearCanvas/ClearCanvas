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
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Externals.Config;

namespace ClearCanvas.ImageViewer.Externals.CoreTools
{
	public abstract class ExternalToolBase : ImageViewerTool
	{
		private ExternalsConfigurationSettings _settings;

		public override void Initialize()
		{
			base.Initialize();

			_settings = ExternalsConfigurationSettings.Default;
			_settings.ExternalsChanged += Settings_ExternalsChanged;
		}

		protected override void Dispose(bool disposing)
		{
			_settings.ExternalsChanged -= Settings_ExternalsChanged;
			_settings = null;

			base.Dispose(disposing);
		}

		protected virtual void OnExternalsChanged(EventArgs e) {}

		private void Settings_ExternalsChanged(object sender, EventArgs e)
		{
			this.OnExternalsChanged(e);
		}
	}
}