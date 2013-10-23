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
using System.ComponentModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Clipboard;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
	[ExtensionPoint]
	public sealed class KeyImageClipboardComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (KeyImageClipboardComponentViewExtensionPoint))]
	public class KeyImageClipboardComponent : ClipboardComponent, IKeyImageClipboardComponent
	{
		public const string MenuSite = "keyimageclipboard-contextmenu";
		public const string ToolbarSite = "keyimageclipboard-toolbar";

		private event EventHandler _keyImageInformationChanged;
		private KeyImageInformation _keyImageInformation;

		public KeyImageClipboardComponent(KeyImageInformation keyImageInformation)
			: base(ToolbarSite, MenuSite, keyImageInformation.ClipboardItems, false)
		{
			_keyImageInformation = keyImageInformation;
		}

		public KeyImageInformation KeyImageInformation
		{
			get { return _keyImageInformation; }
			internal set
			{
				if (_keyImageInformation != value)
				{
					_keyImageInformation = value;
					DataSource = _keyImageInformation.ClipboardItems;

					EventsHelper.Fire(_keyImageInformationChanged, this, new EventArgs());
				}
			}
		}

		public event EventHandler KeyImageInformationChanged
		{
			add { _keyImageInformationChanged += value; }
			remove { _keyImageInformationChanged -= value; }
		}

		#region IKeyImageClipboard Implementation

		IKeyObjectSelectionDocumentInformation IKeyImageClipboardComponent.DocumentInformation
		{
			get { return KeyImageInformation; }
		}

		event EventHandler IKeyImageClipboardComponent.DocumentInformationChanged
		{
			add { KeyImageInformationChanged += value; }
			remove { KeyImageInformationChanged -= value; }
		}

		BindingList<IClipboardItem> IKeyImageClipboardComponent.Items
		{
			get { return DataSource; }
		}

		#endregion

		#region Static

		internal static readonly bool HasViewPlugin;

		static KeyImageClipboardComponent()
		{
			try
			{
				HasViewPlugin = ViewFactory.IsAssociatedViewAvailable<KeyImageClipboardComponent>();
			}
			catch (Exception)
			{
				HasViewPlugin = false;
			}
		}

		#endregion
	}
}