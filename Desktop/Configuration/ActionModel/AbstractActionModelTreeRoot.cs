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
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Trees;

namespace ClearCanvas.Desktop.Configuration.ActionModel
{
	public class AbstractActionModelTreeRoot : AbstractActionModelTreeBranch
	{
		internal event EventHandler<NodeValidationRequestedEventArgs> NodeValidationRequested;
		internal event EventHandler<NodeValidatedEventArgs> NodeValidated;

		private readonly string _site;

		public AbstractActionModelTreeRoot(string site) : base(site)
		{
			_site = site;
		}

		public string Site
		{
			get { return _site; }
		}

		public ITree Tree
		{
			get { return base.Subtree; }
		}

		public ActionModelRoot GetAbstractActionModel()
		{
			ActionModelRoot actionModelRoot = new ActionModelRoot(_site);
			this.CreateActionModelRoot(actionModelRoot);
			return actionModelRoot;
		}

		public new IEnumerable<AbstractActionModelTreeNode> EnumerateDescendants()
		{
			return base.EnumerateDescendants();
		}

		internal override bool RequestValidation(AbstractActionModelTreeNode node, string propertyName, object value)
		{
			NodeValidationRequestedEventArgs e = new NodeValidationRequestedEventArgs(node, propertyName, value);
			EventsHelper.Fire(this.NodeValidationRequested, this, e);
			return e.IsValid;
		}

		internal override void NotifyValidated(AbstractActionModelTreeNode node, string propertyName, object value)
		{
			NodeValidatedEventArgs e = new NodeValidatedEventArgs(node, propertyName, value);
			EventsHelper.Fire(this.NodeValidated, this, e);
		}
	}

	internal class NodeValidationRequestedEventArgs : EventArgs
	{
		public readonly AbstractActionModelTreeNode Node;
		public readonly string PropertyName;
		public readonly object Value;
		public bool IsValid { get; set; }

		public NodeValidationRequestedEventArgs(AbstractActionModelTreeNode node, string propertyName, object value)
		{
			this.Node = node;
			this.PropertyName = propertyName;
			this.Value = value;
			this.IsValid = true;
		}
	}

	internal class NodeValidatedEventArgs : EventArgs
	{
		public readonly AbstractActionModelTreeNode Node;
		public readonly string PropertyName;
		public readonly object Value;

		public NodeValidatedEventArgs(AbstractActionModelTreeNode node, string propertyName, object value)
		{
			this.Node = node;
			this.PropertyName = propertyName;
			this.Value = value;
		}
	}
}