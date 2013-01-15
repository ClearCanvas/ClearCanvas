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

namespace ClearCanvas.Desktop.Configuration.ActionModel
{
	public class AbstractActionModelTreeLeafClickAction : AbstractActionModelTreeLeafAction
	{
		private XKeys _keyStroke = XKeys.None;

		public AbstractActionModelTreeLeafClickAction(IClickAction clickAction) : base(clickAction)
		{
			_keyStroke = clickAction.KeyStroke;
		}

		protected new IClickAction Action
		{
			get { return (IClickAction) base.Action; }
		}

		public XKeys KeyStroke
		{
			get { return _keyStroke; }
			set
			{
				if (!this.RequestValidation("KeyStroke", value))
					return;

				if (_keyStroke != value)
				{
					_keyStroke = value;
					this.NotifyValidated("KeyStroke", value);
					this.OnKeyStrokeChanged();
				}
			}
		}

		public bool IsValidKeyStroke(XKeys keyStroke)
		{
			return this.RequestValidation("KeyStrokePreview", keyStroke);
		}

		protected virtual void OnKeyStrokeChanged()
		{
			this.NotifyItemChanged();

			this.Action.KeyStroke = _keyStroke;
		}
	}
}