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

using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Configuration.ActionModel
{
	[ExtensionOf(typeof (NodePropertiesComponentProviderExtensionPoint))]
	public sealed class ClickActionKeystrokePropertyProvider : INodePropertiesComponentProvider
	{
		public IEnumerable<NodePropertiesComponent> CreateComponents(AbstractActionModelTreeNode selectedNode)
		{
			if (selectedNode is AbstractActionModelTreeLeafClickAction)
				yield return new ClickActionKeystrokePropertyComponent((AbstractActionModelTreeLeafClickAction) selectedNode);
		}
	}

	[ExtensionPoint]
	public sealed class ClickActionKeystrokePropertyComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (ClickActionKeystrokePropertyComponentViewExtensionPoint))]
	public class ClickActionKeystrokePropertyComponent : NodePropertiesComponent
	{
		public ClickActionKeystrokePropertyComponent(AbstractActionModelTreeLeafClickAction selectedClickActionNode)
			: base(selectedClickActionNode) {}

		protected new AbstractActionModelTreeLeafClickAction SelectedNode
		{
			get { return (AbstractActionModelTreeLeafClickAction) base.SelectedNode; }
		}

		public XKeys KeyStroke
		{
			get { return this.SelectedNode.KeyStroke; }
			set
			{
				if (this.SelectedNode.KeyStroke != value)
				{
					this.SelectedNode.KeyStroke = value;
					this.NotifyPropertyChanged("KeyStroke");
				}
			}
		}

		public bool IsValidKeyStroke(XKeys keyStroke)
		{
			return this.SelectedNode.IsValidKeyStroke(keyStroke);
		}
	}
}