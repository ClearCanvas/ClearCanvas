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

namespace ClearCanvas.Desktop.Actions
{
	#region Subclasses

	/// <summary>
	/// Node that represents an action.
	/// </summary>
	public class ActionNode : ActionModelNode
	{
		private readonly IAction _action; // null if this is not a leaf node

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="pathSegment"></param>
		/// <param name="action"></param>
		protected internal ActionNode(PathSegment pathSegment, IAction action)
			: base(pathSegment)
		{
			_action = action;
		}

		/// <summary>
		/// Gets the action associated with this node, or null if this node is not a leaf node.
		/// </summary>
		public IAction Action
		{
			get { return _action; }
		}

		/// <summary>
		/// Used by the <see cref="ActionModelNode.CloneTree"/> method.
		/// </summary>
		/// <remarks>
		/// Derived classes must override this method to return a clone node.  This clone should
		/// not copy the sub-tree.
		/// </remarks>
		/// <param name="pathSegment">The path segment which this node represents.</param>
		/// <returns>A new node of this type.</returns>
		protected override ActionModelNode CloneNode(PathSegment pathSegment)
		{
			return new ActionNode(pathSegment, _action);
		}
	}

	/// <summary>
	/// Node that represents a branch.
	/// </summary>
	internal class BranchNode : ActionModelNode
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="pathSegment"></param>
		protected internal BranchNode(PathSegment pathSegment)
			: base(pathSegment)
		{
		}

		protected override ActionModelNode CloneNode(PathSegment pathSegment)
		{
			return new BranchNode(pathSegment);
		}
	}

	/// <summary>
	/// Node that represents a separator.
	/// </summary>
	public class SeparatorNode : ActionModelNode
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="pathSegment"></param>
		protected internal SeparatorNode(PathSegment pathSegment)
			: base(pathSegment)
		{
		}

		/// <summary>
		/// Used by the <see cref="ActionModelNode.CloneTree"/> method.
		/// </summary>
		/// <remarks>
		/// Derived classes must override this method to return a clone node.  This clone should
		/// not copy the sub-tree.
		/// </remarks>
		/// <param name="pathSegment">The path segment which this node represents.</param>
		/// <returns>A new node of this type.</returns>
		protected override ActionModelNode CloneNode(PathSegment pathSegment)
		{
			return new SeparatorNode(pathSegment);
		}
	}

	#endregion


	/// <summary>
    /// Represents a node in an action model.
    /// </summary>
    public abstract class ActionModelNode
    {
        private readonly PathSegment _pathSegment;
        private readonly ActionModelNodeList _childNodes;

        /// <summary>
        /// Protected constructor.
        /// </summary>
        /// <param name="pathSegment">The segment of the action path to which this node corresponds.</param>
        protected ActionModelNode(PathSegment pathSegment)
        {
            _pathSegment = pathSegment;
            _childNodes = new ActionModelNodeList();
        }

		/// <summary>
		/// Used by the <see cref="CloneTree"/> method.
		/// </summary>
		/// <remarks>
		/// Derived classes must override this method to return a clone node.  This clone should
		/// not copy the sub-tree.
		/// </remarks>
		/// <param name="pathSegment">The path segment which this node represents.</param>
		/// <returns>A new node of this type.</returns>
		protected abstract ActionModelNode CloneNode(PathSegment pathSegment);

        /// <summary>
        /// Gets the action path segment represented by this node.
        /// </summary>
        public PathSegment PathSegment
        {
            get { return _pathSegment; }
        }

        /// <summary>
        /// Gets the list of child nodes of this node.
        /// </summary>
        public ActionModelNodeList ChildNodes
        {
            get { return _childNodes; }
        }

        /// <summary>
        /// Merges the specified model into this model.
        /// </summary>
        public void Merge(ActionModelNode other)
        {
            foreach (ActionModelNode otherChild in other._childNodes)
            {
                ActionModelNode thisChild = FindChild(otherChild.PathSegment);
                if (thisChild != null)
                {
                    thisChild.Merge(otherChild);
                }
                else
                {
                    _childNodes.Add(otherChild.CloneTree());
                }
            }
        }

        /// <summary>
        /// Performs an in-order traversal of this model and returns the set of actions as an array.
        /// </summary>
        /// <returns>An array of <see cref="IAction"/> objects.</returns>
        public IAction[] GetActionsInOrder()
        {
            List<IAction> actions = new List<IAction>();
            GetActionsInOrder(actions);
            return actions.ToArray();
        }

		/// <summary>
		/// Performs an in-order traversal of this model and returns the leaf nodes as an array.
		/// </summary>
		/// <returns>An array of leaf <see cref="ActionModelNode"/>s.</returns>
		internal ActionModelNode[] GetLeafNodesInOrder()
		{
			List<ActionModelNode> leafNodes = new List<ActionModelNode>();
			GetLeafNodesInOrder(leafNodes);
			return leafNodes.ToArray();
		}

		/// <summary>
		/// Traverses the specified path, inserting <see cref="BranchNode"/>s as necessary, until the end of the path
		/// is reached, at which point the <paramref name="leafNodeProvider"/> is called to provide a leaf node to insert.
		/// </summary>
		protected void Insert(Path path, int pathDepth, Converter<PathSegment, ActionModelNode> leafNodeProvider)
		{
			int segmentCount = path.Segments.Count;
			if (segmentCount < 2)
				throw new ArgumentException(SR.ExceptionInvalidActionPath);

			PathSegment segment = path.Segments[pathDepth];
			if (pathDepth + 1 == segmentCount)
			{
				// this is the last path segment -> leaf node
				_childNodes.Add(leafNodeProvider(segment));
			}
			else
			{
				ActionModelNode child = FindChild(segment);
				if (child == null)
				{
					child = new BranchNode(segment);
					_childNodes.Add(child);
				}
				child.Insert(path, pathDepth + 1, leafNodeProvider);
			}
		}

		/// <summary>
		/// Finds a child of this node, based on the specified <see cref="PathSegment"/>.
		/// </summary>
        protected ActionModelNode FindChild(PathSegment segment)
        {
            return _childNodes[segment.LocalizedText];
        }

        /// <summary>
        /// Creates a copy of the subtree beginning at this node.
        /// </summary>
        protected ActionModelNode CloneTree()
        {
            ActionModelNode clone = CloneNode(this.PathSegment);
            foreach (ActionModelNode child in _childNodes)
            {
                clone._childNodes.Add(child.CloneTree());
            }
            return clone;
        }

        private void GetActionsInOrder(List<IAction> actions)
        {
            if(this is ActionNode)
            {
                actions.Add(((ActionNode) this).Action);
            }
            else 
            {
                foreach (ActionModelNode child in _childNodes)
                {
                    child.GetActionsInOrder(actions);
                }
            }
        }

		private void GetLeafNodesInOrder(List<ActionModelNode> leafNodes)
		{
			if (this is ActionNode || this is SeparatorNode)
			{
				leafNodes.Add(this);
			}
			else
			{
				foreach (ActionModelNode child in _childNodes)
				{
					child.GetLeafNodesInOrder(leafNodes);
				}
			}
		}
    }
}
