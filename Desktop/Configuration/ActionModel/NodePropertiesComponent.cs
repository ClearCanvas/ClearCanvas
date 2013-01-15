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
	[ExtensionPoint]
	public sealed class NodePropertiesComponentProviderExtensionPoint : ExtensionPoint<INodePropertiesComponentProvider> {}

	public interface INodePropertiesComponentProvider
	{
		IEnumerable<NodePropertiesComponent> CreateComponents(AbstractActionModelTreeNode selectedNode);
	}

	public abstract class NodePropertiesComponent : ApplicationComponent
	{
		private readonly AbstractActionModelTreeNode _selectedNode;

		protected NodePropertiesComponent(AbstractActionModelTreeNode selectedNode)
		{
			Platform.CheckForNullReference(selectedNode, "selectedNode");
			_selectedNode = selectedNode;
		}

		protected AbstractActionModelTreeNode SelectedNode
		{
			get { return _selectedNode; }
		}

		protected bool RequestPropertyValidation(string propertyName, object value)
		{
			return this.SelectedNode.RequestValidation(propertyName, value);
		}

		protected void NotifyPropertyValidated(string propertyName, object value)
		{
			this.SelectedNode.NotifyValidated(propertyName, value);
		}
	}
}