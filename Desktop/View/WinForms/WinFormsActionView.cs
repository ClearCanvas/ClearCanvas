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

using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.View.WinForms
{
	public abstract class WinFormsActionView : WinFormsView, IActionView
	{
		private IActionViewContext _context;

		protected WinFormsActionView()
		{
		}

		protected IActionViewContext Context
		{
			get { return _context; }	
		}

		protected virtual void OnSetContext()
		{ }

		#region IActionView Members

		IActionViewContext IActionView.Context
		{
			get { return Context; }
			set
			{
				_context = value;
				OnSetContext();
			}
		}

		#endregion
	}

	internal class StandardWinFormsActionView : WinFormsActionView
	{
		private delegate object CreateGuiElementDelegate(IActionViewContext context);

		private object _guiElement;
		private readonly CreateGuiElementDelegate _createGuiElement;

		private StandardWinFormsActionView(CreateGuiElementDelegate createGuiElement)
		{
			_createGuiElement = createGuiElement;
		}

		public override object GuiElement
		{
			get
			{
				if (_guiElement == null)
					_guiElement = _createGuiElement(base.Context);

				return _guiElement;
			}
		}

		public static IActionView CreateDropDownButtonActionView()
		{
			return new StandardWinFormsActionView(
				delegate(IActionViewContext context)
				{
					DropDownButtonToolbarItem item = new DropDownButtonToolbarItem((IClickAction)context.Action, context.IconSize);
					context.IconSizeChanged += delegate { item.IconSize = context.IconSize; };
					return item;
				});
		}

		public static IActionView CreateDropDownActionView()
		{
			return new StandardWinFormsActionView(
				delegate(IActionViewContext context)
				{
					DropDownToolbarItem item = new DropDownToolbarItem((IDropDownAction) context.Action, context.IconSize);
					context.IconSizeChanged += delegate { item.IconSize = context.IconSize; };
					return item;
				});
		}

		public static IActionView CreateButtonActionView()
		{
			return new StandardWinFormsActionView(
				delegate(IActionViewContext context)
				{
					ActiveToolbarButton item = new ActiveToolbarButton((IClickAction) context.Action, context.IconSize);
					context.IconSizeChanged += delegate { item.IconSize = context.IconSize; };
					return item;
				});
		}

		public  static IActionView CreateMenuActionView()
		{
			return new StandardWinFormsActionView(
				delegate(IActionViewContext context)
				{
					ActiveMenuItem item = new ActiveMenuItem((IClickAction)context.Action, context.IconSize);
					context.IconSizeChanged += delegate { item.IconSize = context.IconSize; };
					return item;
				});
		}

		public static IActionView CreateTextBoxActionView()
		{
			return new StandardWinFormsActionView(
				delegate(IActionViewContext context)
				{
					var item = new TextBoxToolbarItem((ITextBoxAction)context.Action, context.IconSize);
					context.IconSizeChanged += delegate { item.IconSize = context.IconSize; };
					return item;
				});
		}
	}
}