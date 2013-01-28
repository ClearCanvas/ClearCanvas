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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.ImageViewer.Tools.Standard.Configuration
{
	[ExtensionPoint]
	public sealed class ToolConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (ToolConfigurationComponentViewExtensionPoint))]
	public class ToolConfigurationComponent : ConfigurationApplicationComponent
	{
		private ToolModalityBehaviorCollection _modalityBehavior;
		private bool _invertZoomDirection;

		public ToolModalityBehaviorCollection ModalityBehavior
		{
			get { return _modalityBehavior; }
			set
			{
				if (_modalityBehavior != value)
				{
					_modalityBehavior = value;
					NotifyPropertyChanged("ModalityBehavior");
					Modified = true;
				}
			}
		}

		public bool InvertZoomDirection
		{
			get { return _invertZoomDirection; }
			set
			{
				if (_invertZoomDirection != value)
				{
					_invertZoomDirection = value;
					NotifyPropertyChanged("InvertZoomDirection");
					Modified = true;
				}
			}
		}

		public override void Start()
		{
			base.Start();

            var settings = ToolSettings.DefaultInstance;
			_modalityBehavior = settings.CachedToolModalityBehavior;
			_invertZoomDirection = settings.InvertedZoomToolOperation;
		}

		public override void Save()
		{
            var settings = ToolSettings.DefaultInstance;
			settings.ToolModalityBehavior = _modalityBehavior;
			settings.InvertedZoomToolOperation = _invertZoomDirection;
			settings.Save();
		}
	}
}