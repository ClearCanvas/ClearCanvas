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
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Clipboard;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
	[ExtensionPoint]
	public sealed class KeyImageClipboardComponentToolExtensionPoint : ExtensionPoint<ITool> {}

	[ExtensionPoint]
	public sealed class KeyImageClipboardComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (KeyImageClipboardComponentViewExtensionPoint))]
	public class KeyImageClipboardComponent : ClipboardComponent
	{
		public const string MenuSite = "keyimageclipboard-contextmenu";
		public const string ToolbarSite = "keyimageclipboard-toolbar";

		// not static so that, if items are somehow added to this context, they won't stay around forever
		private readonly KeyImageInformation _emptyContext = new KeyImageInformation();

		private readonly BindingList<KeyImageInformation> _availableContexts;

		private event EventHandler _currentContextChanged;
		private KeyImageClipboard _clipboard;

		private IToolSet _toolSet;
		private IActionSet _actionSet;

		public KeyImageClipboardComponent(KeyImageClipboard clipboard)
			: base(ToolbarSite, MenuSite, clipboard != null ? clipboard.CurrentContext : null, false)
		{
			_availableContexts = new BindingList<KeyImageInformation>();

			Clipboard = clipboard;
		}

		public new KeyImageClipboard Clipboard
		{
			get { return _clipboard; }
			internal set
			{
				if (_clipboard != value)
				{
					if (_clipboard != null) _clipboard.CurrentContextChanged -= OnClipboardCurrentContextChanged;

					_clipboard = value;

					if (_clipboard != null) _clipboard.CurrentContextChanged += OnClipboardCurrentContextChanged;

					AvailableContexts.RaiseListChangedEvents = false;
					try
					{
						AvailableContexts.Clear();
						if (_clipboard != null)
							foreach (var x in _clipboard.AvailableContexts)
								AvailableContexts.Add(x);
					}
					finally
					{
						AvailableContexts.RaiseListChangedEvents = true;
						AvailableContexts.ResetBindings();
					}
					if (IsStarted) OnClipboardCurrentContextChanged(null, null);
				}
			}
		}

		public BindingList<KeyImageInformation> AvailableContexts
		{
			get { return _availableContexts; }
		}

		public KeyImageInformation CurrentContext
		{
			get { return _clipboard != null ? _clipboard.CurrentContext : _emptyContext; }
			set { if (_clipboard != null) _clipboard.CurrentContext = value; }
		}

		public event EventHandler CurrentContextChanged
		{
			add { _currentContextChanged += value; }
			remove { _currentContextChanged -= value; }
		}

		protected override IActionSet CreateToolActions()
		{
			if (_toolSet == null)
				_toolSet = new ToolSet(new KeyImageClipboardComponentToolExtensionPoint(), new ClipboardToolContext(this));
			return _actionSet ?? (_actionSet = new ActionSet(_toolSet.Actions));
		}

		public override void Start()
		{
			base.Start();

			// ensure the clipboard context properties are loaded and ready just before component is shown for the first time
			OnClipboardCurrentContextChanged(null, null);
		}

		public override void Stop()
		{
			// upon component stop, ensure we clear the clipboard field to unsub any event handlers
			Clipboard = null;

			base.Stop();
		}

		private void OnClipboardCurrentContextChanged(object sender, EventArgs e)
		{
			var currentContext = _clipboard != null ? _clipboard.CurrentContext : _emptyContext;
			base.Clipboard = currentContext;
			NotifyPropertyChanged("CurrentContext");
			EventsHelper.Fire(_currentContextChanged, this, new EventArgs());
		}

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