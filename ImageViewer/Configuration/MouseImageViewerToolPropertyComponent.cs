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
using ClearCanvas.Desktop.Configuration.ActionModel;

namespace ClearCanvas.ImageViewer.Configuration
{
	[ExtensionPoint]
	public sealed class MouseImageViewerToolPropertyComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (MouseImageViewerToolPropertyComponentViewExtensionPoint))]
	public class MouseImageViewerToolPropertyComponent : NodePropertiesComponent
	{
		private XMouseButtons _activeMouseButtons = XMouseButtons.Left;
		private XMouseButtonCombo _globalMouseButtonCombo = XMouseButtonCombo.None;
		private XMouseButtons _globalMouseButtons = XMouseButtons.None;
		private ModifierFlags _globalModifiers = ModifierFlags.None;
		private bool _initiallyActive = false;

		public MouseImageViewerToolPropertyComponent(AbstractActionModelTreeNode selectedNode, XMouseButtons activeMouseButtons, XMouseButtons globalMouseButtons, ModifierFlags globalModifiers, bool initiallyActive)
			: base(selectedNode)
		{
			_activeMouseButtons = activeMouseButtons;
			_globalMouseButtonCombo = new XMouseButtonCombo(globalMouseButtons, globalModifiers);
			_globalMouseButtons = globalMouseButtons;
			_globalModifiers = globalModifiers;
			_initiallyActive = initiallyActive;
		}

		public bool InitiallyActive
		{
			get { return _initiallyActive; }
			set
			{
				if (!this.RequestPropertyValidation("InitiallyActive", value))
					return;

				if (_initiallyActive != value)
				{
					_initiallyActive = value;
					this.NotifyPropertyValidated("InitiallyActive", value);
					this.OnInitiallyActiveChanged();
				}
			}
		}

		public XMouseButtons ActiveMouseButtons
		{
			get { return _activeMouseButtons; }
			set
			{
				if (!this.RequestPropertyValidation("ActiveMouseButtons", value))
					return;

				if (_activeMouseButtons != value)
				{
					_activeMouseButtons = value;
					this.NotifyPropertyValidated("ActiveMouseButtons", value);
					this.OnActiveMouseButtonsChanged();
				}
			}
		}

		public XMouseButtonCombo GlobalMouseButtonCombo
		{
			get { return _globalMouseButtonCombo; }
			set
			{
				if (!this.RequestPropertyValidation("GlobalMouseButtonCombo", value))
					return;

				if (_globalMouseButtonCombo != value)
				{
					_globalMouseButtonCombo = value;
					this.NotifyPropertyValidated("GlobalMouseButtonCombo", value);
					this.OnGlobalMouseButtonComboShortcutChanged();
				}
			}
		}

		public XMouseButtons GlobalMouseButtons
		{
			get { return _globalMouseButtons; }
			set
			{
				if (!this.RequestPropertyValidation("GlobalMouseButtons", value))
					return;

				if (_globalMouseButtons != value)
				{
					_globalMouseButtons = value;
					this.NotifyPropertyValidated("GlobalMouseButtons", value);
					this.OnGlobalMouseButtonsChanged();
				}
			}
		}

		public ModifierFlags GlobalModifiers
		{
			get { return _globalModifiers; }
			set
			{
				if (!this.RequestPropertyValidation("GlobalModifiers", value))
					return;

				if (_globalModifiers != value)
				{
					_globalModifiers = value;
					this.NotifyPropertyValidated("GlobalModifiers", value);
					this.OnGlobalModifiersChanged();
				}
			}
		}

		protected virtual void OnInitiallyActiveChanged()
		{
			this.NotifyPropertyChanged("InitiallyActive");
		}

		protected virtual void OnActiveMouseButtonsChanged()
		{
			this.NotifyPropertyChanged("ActiveMouseButtons");
			this.InitiallyActive = false;
		}

		protected virtual void OnGlobalMouseButtonComboShortcutChanged()
		{
			this.NotifyPropertyChanged("GlobalMouseButtonCombo");
			this.GlobalMouseButtons = _globalMouseButtonCombo.MouseButtons;
			this.GlobalModifiers = _globalMouseButtonCombo.Modifiers;
		}

		protected virtual void OnGlobalMouseButtonsChanged()
		{
			this.NotifyPropertyChanged("GlobalMouseButtons");
			this.GlobalMouseButtonCombo = new XMouseButtonCombo(_globalMouseButtons, _globalModifiers);
		}

		protected virtual void OnGlobalModifiersChanged()
		{
			this.NotifyPropertyChanged("GlobalModifiers");
			this.GlobalMouseButtonCombo = new XMouseButtonCombo(_globalMouseButtons, _globalModifiers);
		}
	}
}