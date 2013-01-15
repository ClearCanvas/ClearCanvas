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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Trees;
using CheckState=ClearCanvas.Desktop.Trees.CheckState;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Tree-view that binds to an instance of an <see cref="ITree"/>, which acts as data-source.
    /// Also has built-in drag & drop support, delegating drop decisions to the underlying <see cref="ITree"/>.
    /// </summary>
    public partial class BindingTreeView : UserControl
    {
		public class ItemDroppedEventArgs : EventArgs
		{
			private readonly object _item;
			private readonly DragDropKind _kind;

			public ItemDroppedEventArgs(object item, DragDropKind kind)
			{
				_item = item;
				_kind = kind;
			}

			public object Item
			{
				get { return _item; }
			}

			public DragDropKind Kind
			{
				get { return _kind; }
			}
		}

        private ITree _root;
        private BindingTreeLevelManager _rootLevelManager;
        private event EventHandler _selectionChanged;
        private event EventHandler _nodeMouseDoubleClicked;
		private event EventHandler _nodeMouseClicked;
    	private event EventHandler _searchTextChanged;
		private event EventHandler<ItemDragEventArgs> _itemDrag;
		private event EventHandler<ItemDroppedEventArgs> _itemDropped;

        private BindingTreeNode _dropTargetNode;
        private DragDropEffects _dropEffect;
    	private DragDropPosition _dropPosition;

        private ActionModelNode _toolbarModel;
        private ActionModelNode _menuModel;

    	private IconSize _iconResourceSize = ClearCanvas.Desktop.IconSize.Medium;
    	private bool _checkBoxes = false;
        private bool _selectionDisabled = false;
    	private bool _allowDropToIndex = false;

        private bool _isLoaded = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public BindingTreeView()
        {
            InitializeComponent();

			// if some but not all nodes have icons, then the treeview control pulls the first icon from the list
			// to fix this problem, we'll explicitly add a dummy transparent icon for use by nodes without icons
			Bitmap blankIcon = new Bitmap(1, 1);
			blankIcon.SetPixel(0, 0, Color.Transparent);
			_imageList.Images.Add(string.Empty, blankIcon);

			this.RebuildStateImageList();
        }

        #region Public members

        /// <summary>
        /// Gets or sets the model that this view looks at
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ITree Tree
        {
            get { return _root; }
            set
            {
				if(value != _root)
				{
					if (_rootLevelManager != null)
					{
						// be sure to dispose of _rootLevelManager, in order to unsubscribe events, etc.
						_rootLevelManager.Dispose();
						_rootLevelManager = null;
					}

					_root = value;

					if (_root != null)
					{
						_rootLevelManager = new BindingTreeLevelManager(_root, _treeCtrl.Nodes, this);
					}
				}
            }
        }

        /// <summary>
        /// Gets or sets the current selection
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ISelection Selection
        {
            get
            {
                return GetSelectionHelper();
            }
            set
            {
                // if someone tries to assign null, just convert it to an empty selection - this makes everything easier
                ISelection newSelection = value ?? new Selection();

                // get the existing selection
                ISelection existingSelection = GetSelectionHelper();

                if (!existingSelection.Equals(newSelection))
                {
                    if (newSelection.Item == null)
                    {
                        _treeCtrl.SelectedNode = null;
						EventsHelper.Fire(_selectionChanged, this, EventArgs.Empty);
                    }
                    else
                    {
                        _treeCtrl.SelectedNode = FindNodeRecursive(_treeCtrl.Nodes, delegate(BindingTreeNode node) { return node.DataBoundItem == newSelection.Item; });
                    }

                    // we don't need to fire SelectionChanged here, because setting _treeCtrl.SelectedNode will do that for us indirectly
					// except when _treeCtrl.SelectedNode is set to null
                }
            }
        }

		[Obsolete("Toolstrip item display style is controlled by ToolStripBuilder.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RightToLeft ToolStripRightToLeft
        {
            get { return RightToLeft.No; }
            set { }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ActionModelNode ToolbarModel
        {
            get { return _toolbarModel; }
            set
            {
                _toolbarModel = value;
                if (_isLoaded) InitializeToolStrip();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ActionModelNode MenuModel
        {
            get { return _menuModel; }
            set
            {
                _menuModel = value;
				if (_isLoaded) InitializeMenu();
			}
        }

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string SearchText
		{
			get { return _searchTextBox.Text; }
			set
			{
				if (Equals(_searchTextBox.Text, value))
					return;

				_searchTextBox.Text = value;
				EventsHelper.Fire(_searchTextChanged, this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Notifies that the search textbox text has changed
		/// </summary>
		public event EventHandler SearchTextChanged
		{
			add { _searchTextChanged += value; }
			remove { _searchTextChanged -= value; }
		}

		/// <summary>
        /// Notifies that the selection has changed
        /// </summary>
        public event EventHandler SelectionChanged
        {
            add { _selectionChanged += value; }
            remove { _selectionChanged -= value; }
        }
        
        /// <summary>
        /// Notifies that the selection is double clicked
        /// </summary>
        public event EventHandler NodeMouseDoubleClicked
        {
            add { _nodeMouseDoubleClicked += value; }
            remove { _nodeMouseDoubleClicked -= value; }
        }

		/// <summary>
		/// Notifies that the selection is clicked
		/// </summary>
		public event EventHandler NodeMouseClicked
		{
			add { _nodeMouseClicked += value; }
			remove { _nodeMouseClicked -= value; }
		}

		/// <summary>
		/// Start editing the selected node.
		/// </summary>
		public void EditSelectedNode()
		{
			if (!_treeCtrl.LabelEdit || _treeCtrl.SelectedNode == null)
				return;
			
			_treeCtrl.SelectedNode.BeginEdit();
		}

		/// <summary>
        /// Expands the entire tree
        /// </summary>
        public void ExpandAll()
        {
            _treeCtrl.ExpandAll();
        }

        #endregion

		#region Protected Members

		protected internal TreeView TreeView
		{
			get { return _treeCtrl; }
		}

		#endregion

        #region Design time properties

		[Browsable(false)]
		[Obsolete("Use CheckBoxes property instead. The ITree model determines whether or not tristate check boxes are used.")]
		public CheckBoxStyle CheckBoxStyle
		{
			get { return CheckBoxes ? CheckBoxStyle.TriState : CheckBoxStyle.None; }
			set { CheckBoxes = value != CheckBoxStyle.None; }
		}

		private bool ShouldSerializeCheckBoxStyle()
		{
			return false;
		}

    	[DefaultValue(false)]
    	public bool AllowDropToIndex
    	{
			get { return this.AllowDrop && _allowDropToIndex; }
			set { _allowDropToIndex = value; }
    	}

        [DefaultValue(true)]
        public bool ShowToolbar
        {
            get { return _toolStrip.Visible; }
            set { _toolStrip.Visible = value; }
        }

        [DefaultValue(false)]
        public bool FullRowSelect
        {
            get { return _treeCtrl.FullRowSelect; }
            set { _treeCtrl.FullRowSelect = value; }
        }

        [DefaultValue(true)]
        public bool ShowLines
        {
            get { return _treeCtrl.ShowLines; }
            set { _treeCtrl.ShowLines = value; }
        }

        [DefaultValue(true)]
        public bool ShowPlusMinus
        {
            get { return _treeCtrl.ShowPlusMinus; }
            set { _treeCtrl.ShowPlusMinus = value; }
        }

        [DefaultValue(true)]
        public bool ShowRootLines
        {
            get { return _treeCtrl.ShowRootLines; }
            set { _treeCtrl.ShowRootLines = value;}
        }

    	[DefaultValue(false)]
    	public bool CheckBoxes
    	{
    		get { return _checkBoxes; }
    		set
    		{
    			if (_checkBoxes != value)
    			{
    				_checkBoxes = value;
    				OnCheckBoxesChanged();
    			}
    		}
    	}

        [DefaultValue(false)]
        public bool SelectionDisabled
        {
            get { return _selectionDisabled; }
            set
            {
                _selectionDisabled = value;
            }
        }

    	[DefaultValue(KnownColor.Window)]
		public Color TreeBackColor
    	{
			get { return _treeCtrl.BackColor; }	
			set { _treeCtrl.BackColor = value; }	
    	}

		[DefaultValue(KnownColor.WindowText)]
		public Color TreeForeColor
		{
			get { return _treeCtrl.ForeColor; }
			set { _treeCtrl.ForeColor = value; }
		}

		[DefaultValue(KnownColor.Black)]
		public Color TreeLineColor
		{
			get { return _treeCtrl.LineColor; }
			set { _treeCtrl.LineColor = value; }
		}

		[DefaultValue(false)]
		public bool SearchTextBoxVisible
		{
			get { return _searchTextBox.Visible; }
			set
			{
				_searchTextBox.Visible = value;
				_clearSearchButton.Visible = value;
			}
		}

		[DefaultValue(100)]
		[Localizable(true)]
		public int SearchTextBoxWidth
		{
			get { return _searchTextBox.Width; }
			set { _searchTextBox.Size = new Size(value, _searchTextBox.Height); }
		}

		[Obsolete("Toolstrip item display style is controlled by ToolStripBuilder.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ToolStripItemDisplayStyle ToolStripItemDisplayStyle
        {
			get { return ToolStripItemDisplayStyle.Image; }
			set {  }
        }

    	public Size IconSize
    	{
    		get { return _imageList.ImageSize; }
    		set
    		{
    			if (_imageList.ImageSize != value)
    			{
    				_imageList.ImageSize = value;
    				_stateImageList.ImageSize = value; // use same size, because otherwise the line spacing is off
    				this.OnIconSizeChanged();
    			}
    		}
    	}

    	public IconSize IconResourceSize
    	{
			get { return _iconResourceSize; }
			set { _iconResourceSize = value; }
    	}

		[DefaultValue(ColorDepth.Depth32Bit)]
        public ColorDepth IconColorDepth
        {
            get { return _imageList.ColorDepth; }
            set { _imageList.ColorDepth = value; }
        }

        public event EventHandler<ItemDragEventArgs> ItemDrag
        {
            add { _itemDrag += value; }
            remove { _itemDrag -= value; }
        }

		public event EventHandler<ItemDroppedEventArgs> ItemDropped
		{
			add { _itemDropped += value; }
			remove { _itemDropped -= value; }
		}

        #endregion

        #region Helper methods

        /// <summary>
        /// Obtains the current selection
        /// </summary>
        /// <returns></returns>
        private ISelection GetSelectionHelper()
        {
            BindingTreeNode selNode = (BindingTreeNode)_treeCtrl.SelectedNode;
            return selNode == null ? new Selection() : new Selection(selNode.DataBoundItem);
        }

        /// <summary>
        /// Searches the tree depth-first for a node matching the specified criteria
        /// </summary>
        /// <param name="nodeCollection"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        private BindingTreeNode FindNodeRecursive(TreeNodeCollection nodeCollection, Predicate<BindingTreeNode> criteria)
        {
            foreach (TreeNode node in nodeCollection)
            {
                //  Bug #871
                //  See BindingTreeNode.UpdateDisplay():  
                //  the Nodes property may contain a "dummy" TreeNode, so ensure each iterated TreeNode is actually a BindingTreeNode
                BindingTreeNode bindingTreeNode = node as BindingTreeNode;

                if (bindingTreeNode != null && criteria(bindingTreeNode))
                {
                    return bindingTreeNode;
                }
                else
                {
                    BindingTreeNode nodeFound = FindNodeRecursive(node.Nodes, criteria);
                    if (nodeFound != null)
                        return nodeFound;
                }
            }
            return null;
        }

		[Obsolete("Calls OnCheckBoxesChanged() instead.")]
		protected virtual void OnCheckBoxStyleChanged()
		{
			OnCheckBoxesChanged();
		}

		protected virtual void OnCheckBoxesChanged()
		{
			_treeCtrl.BeginUpdate();
			_treeCtrl.CheckBoxes = false;
			_treeCtrl.StateImageList = CheckBoxes ? _stateImageList : null;
			_treeCtrl.EndUpdate();
		}

		protected virtual void OnIconSizeChanged()
		{
			this.RebuildStateImageList();
		}

        /// <summary>
        /// When the user is about to expand a node, need to build the level beneath it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _treeCtrl_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            BindingTreeNode expandingNode = (BindingTreeNode)e.Node;
            if (!expandingNode.IsSubTreeBuilt)
            {
                expandingNode.BuildSubTree();
            }
        }

        /// <summary>
        /// Notify that the <see cref="Selection"/> property has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _treeCtrl_AfterSelect(object sender, TreeViewEventArgs e)
        {
			BindingTreeNode selNode = (BindingTreeNode)_treeCtrl.SelectedNode;
			_treeCtrl.LabelEdit = selNode != null && selNode.CanSetNodeText();
			
			EventsHelper.Fire(_selectionChanged, this, EventArgs.Empty);
        }

        private void InitializeToolStrip()
        {
            ToolStripBuilder.Clear(_toolStrip.Items);
            if (_toolbarModel != null)
            {
                ToolStripBuilder.BuildToolbar(_toolStrip.Items, _toolbarModel.ChildNodes);
            }
        }
		private void InitializeMenu()
		{
			ToolStripBuilder.Clear(_contextMenu.Items);
			if (_menuModel != null)
			{
				ToolStripBuilder.BuildMenu(_contextMenu.Items, _menuModel.ChildNodes);
			}
		}

        private void _contextMenu_Opening(object sender, CancelEventArgs e)
        {
            // Find the node we're on
            Point pt = _treeCtrl.PointToClient(TreeView.MousePosition);
            BindingTreeNode node = (BindingTreeNode)_treeCtrl.GetNodeAt(pt.X, pt.Y);
            _treeCtrl.SelectedNode = node;
			if (node == null)
				EventsHelper.Fire(_selectionChanged, this, EventArgs.Empty);
        }

        private void _contextMenu_Opened(object sender, EventArgs e)
        {

        }

        private void _contextMenu_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {

        }

        private void _contextMenu_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {

        }

        private void BindingTreeView_Load(object sender, EventArgs e)
        {
			InitializeMenu();
			InitializeToolStrip();
            _isLoaded = true;
        }

        private void _treeCtrl_AfterCheck(object sender, TreeViewEventArgs e)
        {
        	// the built-in checkbox functionality of the TreeView control is not used because of a
			// double clicking bug where the node's internal state is not in sync with what is being painted
        	// all checkbox support is now handled through the custom state image list
        	// ref: http://connect.microsoft.com/VisualStudio/feedback/details/374516/treeview-control-does-not-fire-events-reliably-when-double-clicking-on-checkbox
			// ref: CC ticket #9233
        }

        private void _treeCtrl_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (_selectionDisabled)
            {
                e.Cancel = true;
            }
        }

        private void _treeCtrl_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            EventsHelper.Fire(_nodeMouseDoubleClicked, this, e);
        }

		private void _treeCtrl_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			if (CheckBoxes && _treeCtrl.HitTest(e.Location).Location == TreeViewHitTestLocations.StateImage)
			{
				BindingTreeNode node = e.Node as BindingTreeNode;
				if (node != null)
				{
					node.OnChecked();
				}
			}

			EventsHelper.Fire(_nodeMouseClicked, this, e);
		}

    	private void _treeCtrl_ForeColorChanged(object sender, EventArgs e)
    	{
    		Color color = _treeCtrl.ForeColor;
    		SendMessage(_treeCtrl.Handle, (int) WindowsMessages.TVM_SETINSERTMARKCOLOR, IntPtr.Zero, new IntPtr(((((color.B << 8) + color.G) << 8) + color.R)));
    	}

		#endregion

        #region Drag Drop support

        /// <summary>
        /// Called when an object is first dragged into this control
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDragEnter(DragEventArgs e)
        {
            // clear any record of a previous drop target node
            _dropTargetNode = null;
            _dropEffect = DragDropEffects.None;
			_dropPosition = DragDropPosition.Default;

            base.OnDragEnter(e);
        }

        /// <summary>
        /// Called repeatedly as the object is dragged within this control
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDragOver(DragEventArgs e)
        {
            // determine the node under the cursor
			Point cursor = _treeCtrl.PointToClient(new Point(e.X, e.Y));
            BindingTreeNode node = (BindingTreeNode)_treeCtrl.GetNodeAt(cursor);
 
            // determine what effect the user is trying to accomplish
            DragDropEffects desiredEffect = GetDragDropDesiredEffect(e);

			// determine if the user is trying to drag to the top, middle or bottom areas of the node
        	DragDropPosition position = DragDropPosition.Default;
        	if (this.AllowDropToIndex && node != null)
        	{
        		float result = 1f*(cursor.Y - node.Bounds.Y)/node.Bounds.Height;
        		if (result <= 0.2f)
        			position = DragDropPosition.Before;
        		else if (result >= 0.8f)
        			position = DragDropPosition.After;
        	}

            // optimization: only care if different than the last known drop-target node, or different desired effect, or different drop position
        	if (node != _dropTargetNode || desiredEffect != _dropEffect || position != _dropPosition)
            {
                _treeCtrl.BeginUpdate();    // suspend drawing

                // un-highlight the last known drop-target node
				HighlightNode(_dropTargetNode, false);
				SetInsertMark(_dropTargetNode, DragDropPosition.Default);

                // set the drop target node to this node
                _dropTargetNode = node;
                _dropEffect = desiredEffect;
				_dropPosition = position;

                // check if drop target node exists and what kind of operation it will accept
                DragDropKind acceptableKind = (_dropTargetNode == null) ?
                    DragDropKind.None : _dropTargetNode.CanAcceptDrop(GetDragDropData(e), GetDragDropKind(desiredEffect), _dropPosition);

                // display the appropriate effect cue based on the result
                e.Effect = GetDragDropEffect(acceptableKind);

                // if the drop target is valid and willing to accept data, highlight it
                if (acceptableKind != DragDropKind.None)
                {
					HighlightNode(_dropTargetNode, _dropPosition == DragDropPosition.Default);
					SetInsertMark(_dropTargetNode, _dropPosition);
                }

                _treeCtrl.EndUpdate(); // resume drawing
            }

			// perform drag scrolling
			if (_treeCtrl.ClientRectangle.Contains(cursor))
			{
				if (cursor.Y > _treeCtrl.Height - _treeCtrl.ItemHeight/2)
					SendMessage(_treeCtrl.Handle, (int) WindowsMessages.WM_VSCROLL, new IntPtr(1), IntPtr.Zero);
				else if (cursor.Y < _treeCtrl.ItemHeight/2)
					SendMessage(_treeCtrl.Handle, (int) WindowsMessages.WM_VSCROLL, IntPtr.Zero, IntPtr.Zero);
			}
            
            base.OnDragOver(e);
        }

        /// <summary>
        /// Called when an object is dropped onto this control
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDragDrop(DragEventArgs e)
        {
            // is there a current drop-target node?
            if (_dropTargetNode != null)
            {
                try
                {
					object dragDropData = GetDragDropData(e);
					
					// ask the node to accept the drop
					DragDropKind result = _dropTargetNode.AcceptDrop(dragDropData, GetDragDropKind(e.Effect), _dropPosition);

                    // be sure to set the resulting effect in the event args, so that it gets communicated
                    // back to the initiator of the drag drop operation
                    e.Effect = GetDragDropEffect(result);

					// Fire the item dropped event
					if (e.Effect != DragDropEffects.None)
					{
						ItemDroppedEventArgs args = new ItemDroppedEventArgs(dragDropData, result);
						EventsHelper.Fire(_itemDropped, this, args);
					}
				}
                catch (Exception ex)
                {
                    Platform.Log(LogLevel.Error, ex);
                }

                // remove highlighting from drop target node
                HighlightNode(_dropTargetNode, false);
            	SetInsertMark(_dropTargetNode, DragDropPosition.Default);
            }

            // clear the drop target node
            _dropTargetNode = null;
            _dropEffect = DragDropEffects.None;
        	_dropPosition = DragDropPosition.Default;

            base.OnDragDrop(e);
        }

        protected override void OnDragLeave(EventArgs e)
        {
            // is there a current drop-target node?
            if (_dropTargetNode != null)
            {
                // remove highlighting from drop target node
				HighlightNode(_dropTargetNode, false);
				SetInsertMark(_dropTargetNode, DragDropPosition.Default);
            }

            // clear the drop target node
            _dropTargetNode = null;
            _dropEffect = DragDropEffects.None;
        	_dropPosition = DragDropPosition.Default;

            base.OnDragLeave(e);
        }

    	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
    	private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

		/// <summary>
		/// Sets or unsets an insertion mark before or after the specified node. <see cref="DragDropPosition.Default"/> clears the insertion mark.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="position"></param>
		private void SetInsertMark(TreeNode node, DragDropPosition position)
		{
			IntPtr drawAfter = new IntPtr(position == DragDropPosition.After ? 1 : 0);
			IntPtr nodeHandle = IntPtr.Zero;
			if (node != null && position != DragDropPosition.Default)
				nodeHandle = node.Handle;

			SendMessage(_treeCtrl.Handle, (int) WindowsMessages.TVM_SETINSERTMARK, drawAfter, nodeHandle);
		}

        /// <summary>
        /// Highlights or un-highlights the specified node, without altering the current selection
        /// </summary>
        /// <param name="node"></param>
        /// <param name="highlight"></param>
        private void HighlightNode(TreeNode node, bool highlight)
        {
            if (node != null)
            {
                node.BackColor = highlight ? SystemColors.Highlight : _treeCtrl.BackColor;
                node.ForeColor = highlight ? SystemColors.HighlightText : _treeCtrl.ForeColor;
            }
        }

        /// <summary>
        /// Returns the desired effect, chosen from the allowed effects in the DragEventArgs.
        /// The user indicates the desired effect (move/copy/link) by using modifier keys.
        /// Copied this logic from MSDN - seems to be standard windows convention.  
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private DragDropEffects GetDragDropDesiredEffect(DragEventArgs e)
        {
            // Set the effect based upon the KeyState.
            if ((e.KeyState & (8 + 32)) == (8 + 32) &&
                (e.AllowedEffect & DragDropEffects.Link) == DragDropEffects.Link)
            {
                // KeyState 8 + 32 = CTL + ALT
                // Link drag-and-drop effect.
                return DragDropEffects.Link;
            }
            else if ((e.KeyState & 32) == 32 &&
                (e.AllowedEffect & DragDropEffects.Link) == DragDropEffects.Link)
            {
                // ALT KeyState for link.
                return DragDropEffects.Link;
            }
            else if ((e.KeyState & 4) == 4 &&
              (e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move)
            {
                // SHIFT KeyState for move.
                return DragDropEffects.Move;
            }
            else if ((e.KeyState & 8) == 8 &&
              (e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
            {
                // CTL KeyState for copy.
                return DragDropEffects.Copy;
            }
            else if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move)
            {
                // By default, the drop action should be move, if allowed.
                return DragDropEffects.Move;
            }

            return DragDropEffects.None;
        }

        /// <summary>
        /// Converts a <see cref="DragDropEffects"/>, which is WinForms specific, to a <see cref="DragDropKind"/>
        /// </summary>
        /// <param name="effect"></param>
        /// <returns></returns>
        private DragDropKind GetDragDropKind(DragDropEffects effect)
        {
            if ((effect & DragDropEffects.Copy) == DragDropEffects.Copy)
                return DragDropKind.Copy;
            if ((effect & DragDropEffects.Move) == DragDropEffects.Move)
                return DragDropKind.Move;

            // other effects are not currently supported by this control, so just return Move
            return DragDropKind.Move;
        }

        /// <summary>
        /// Converts a <see cref="DragDropKind"/> to the WinForms <see cref="DragDropEffects"/>
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        private DragDropEffects GetDragDropEffect(DragDropKind kind)
        {
            switch (kind)
            {
                case DragDropKind.Move:
                    return DragDropEffects.Move;
                case DragDropKind.Copy:
                    return DragDropEffects.Copy;
                default:
                    return DragDropEffects.None;
            }
        }

        /// <summary>
        /// Extracts the drag-drop data from the event args, assuming this is an in-process drop.
        /// Out-of-process drops are not currently supported by this control.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private object GetDragDropData(DragEventArgs e)
        {
            IDataObject dao = e.Data;
            string[] formats = dao.GetFormats();

            // use any available format, since we are assuming the data is in-process
            return formats.Length > 0 ? dao.GetData(formats[0]) : null;
        }

        #endregion

        private void _treeCtrl_ItemDrag(object sender, ItemDragEventArgs e)
        {
			// the item being dragged should be selected as well.
			BindingTreeNode node = (BindingTreeNode)e.Item;
			_treeCtrl.SelectedNode = node;

            ItemDragEventArgs args = new ItemDragEventArgs(e.Button, this.GetSelectionHelper());
            EventsHelper.Fire(_itemDrag, this, args);
        }

		private void _treeCtrl_AfterExpand(object sender, TreeViewEventArgs e)
		{
			BindingTreeNode node = e.Node as BindingTreeNode;
			if (node != null)
			{
				node.OnExpandCollapse();
			}
		}

		private void _treeCtrl_AfterCollapse(object sender, TreeViewEventArgs e)
		{
			BindingTreeNode node = e.Node as BindingTreeNode;
			if (node != null)
			{
				node.OnExpandCollapse();
			}
		}

		private void _treeCtrl_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
		{
			if (string.IsNullOrEmpty(e.Label))
			{
				// user cancel the edit, stop editing
				e.CancelEdit = true;
			}
			else
			{
				BindingTreeNode node = e.Node as BindingTreeNode;
				if (node != null)
				{
					node.AfterLabelEdit(e.Label);
				}
			}
		}

		private void _clearSearchButton_Click(object sender, EventArgs e)
		{
			this.SearchText = "";
		}

		private void _searchTextBox_TextChanged(object sender, EventArgs e)
		{
			if (String.IsNullOrEmpty(_searchTextBox.Text))
			{
				_searchTextBox.ToolTipText = SR.MessageEmptySearchTree;
				_clearSearchButton.Enabled = false;
			}
			else
			{
				_searchTextBox.ToolTipText = String.Format(SR.MessageSearchBy, _searchTextBox.Text);
				_clearSearchButton.Enabled = true;
			}

			EventsHelper.Fire(_searchTextChanged, this, EventArgs.Empty);
		}

    	private static CheckBoxState ConvertCheckState(CheckState checkState)
    	{
    		switch (checkState)
    		{
    			case CheckState.Checked:
    				return CheckBoxState.CheckedNormal;
    			case CheckState.Partial:
    				return CheckBoxState.MixedNormal;
    			default:
    			case CheckState.Unchecked:
    				return CheckBoxState.UncheckedNormal;
    		}
    	}

		private static void DisposeAll(IEnumerable enumerable)
		{
			foreach (object o in enumerable)
				if (o is IDisposable)
					((IDisposable)o).Dispose();
		}

    	private void RebuildStateImageList()
    	{
    		_treeCtrl.BeginUpdate();
    		try
    		{
				DisposeAll(_stateImageList.Images);
				_stateImageList.Images.Clear();

    			Size stateImageSize = _stateImageList.ImageSize;
    			foreach (CheckState checkState in Enum.GetValues(typeof (CheckState)))
    			{
    				Bitmap bitmap = new Bitmap(stateImageSize.Width, stateImageSize.Height);
    				using (Graphics g = Graphics.FromImage(bitmap))
    				{
    					g.FillRectangle(Brushes.Transparent, 0, 0, stateImageSize.Width, stateImageSize.Height);
    					CheckBoxState checkBoxState = ConvertCheckState(checkState);
    					Size glyphSize = CheckBoxRenderer.GetGlyphSize(g, checkBoxState);
    					CheckBoxRenderer.DrawCheckBox(g,
    					                              new Point((stateImageSize.Width - glyphSize.Width)/2, (stateImageSize.Height - glyphSize.Height)/2),
    					                              checkBoxState);
    				}
    				_stateImageList.Images.Add(checkState.ToString(), bitmap);
    			}
    		}
    		finally
    		{
    			_treeCtrl.EndUpdate();
    		}
    	}

		#region Fixed TreeView Control

		/// <summary>
		/// <see cref="TreeView"/> control that fixes the 16x16 state image size restriction.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Basically, the WinForms wrapper of a TreeView uses a copy of the state image list we provide.
		/// This copy is modified by duplicating the first image as a dummy "no-state" image - but the
		/// code was unfortunately written in a way that ensured a constant 16x16 image size.
		/// </para>
		/// </remarks>
		/// <seealso cref="http://www.codeproject.com/KB/tree/customstatetreeview.aspx?msg=1519062"/>
		private class XTreeView : TreeView
		{
			private ImageList _fixedInternalStateImageList = null;

			private const int TV_FIRST = 0x1100;
			private const int TVM_SETEXTENDEDSTYLE = TV_FIRST + 44;
			private const int TVS_EX_DOUBLEBUFFER = 0x0004;

			public XTreeView()
			{
				// Enable default double buffering processing (DoubleBuffered returns true)
				SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
				// Disable default CommCtrl painting on non-Vista systems
				if (Environment.OSVersion.Version.Major < 6)
					SetStyle(ControlStyles.UserPaint, true);
			}

			private void UpdateExtendedStyles()
			{
				int Style = 0;

				if (DoubleBuffered)
					Style |= TVS_EX_DOUBLEBUFFER;

				if (Style != 0)
					SendMessage(Handle, TVM_SETEXTENDEDSTYLE, (IntPtr)TVS_EX_DOUBLEBUFFER, (IntPtr)Style);
			}

			protected override void OnHandleCreated(EventArgs e)
			{
				base.OnHandleCreated(e);
				UpdateExtendedStyles();
			}
			protected override void OnPaint(PaintEventArgs e)
			{
				if (GetStyle(ControlStyles.UserPaint))
				{
					Message m = new Message();
					m.HWnd = Handle;
					m.Msg = 0x0318; // WM_PRINTCLIENT;
					m.WParam = e.Graphics.GetHdc();
					m.LParam = (IntPtr)4; // PRF_CLIENT;
					DefWndProc(ref m);
					e.Graphics.ReleaseHdc(m.WParam);
				}
				base.OnPaint(e);
			}

			protected override void Dispose(bool disposing)
			{
				if (disposing)
				{
					if (_fixedInternalStateImageList != null)
					{
						DisposeAll(_fixedInternalStateImageList.Images);
						_fixedInternalStateImageList.Dispose();
						_fixedInternalStateImageList = null;
					}
				}
				base.Dispose(disposing);
			}

			protected override void WndProc(ref Message m)
			{
				// intercept the message that sets the state image list, and pass on a fixed image list
				// instead of the modified 16x16 version. the control will take care of the internal
				// modified image list it created - we just need to handle disposal for our own image list
				const int TVSIL_STATE = 2;
				if (m.Msg == (int) WindowsMessages.TVM_SETIMAGELIST)
				{
					if (_fixedInternalStateImageList != null)
					{
						DisposeAll(_fixedInternalStateImageList.Images);
						_fixedInternalStateImageList.Dispose();
						_fixedInternalStateImageList = null;
					}

					if (m.WParam.ToInt32() == TVSIL_STATE && m.LParam != IntPtr.Zero && this.StateImageList != null)
					{
						// setup a new image list as a rough clone of the real one
						_fixedInternalStateImageList = new ImageList();
						_fixedInternalStateImageList.ColorDepth = this.StateImageList.ColorDepth;
						_fixedInternalStateImageList.ImageSize = this.StateImageList.ImageSize;
						_fixedInternalStateImageList.TransparentColor = this.StateImageList.TransparentColor;

						// add the dummy image (we still need to do this because otherwise the image indexes are shifted by 1)
						_fixedInternalStateImageList.Images.Add(string.Empty, new Bitmap(1, 1));

						// copy over the original images
						for (int n = 0; n < this.StateImageList.Images.Count; n++)
							_fixedInternalStateImageList.Images.Add((Image) this.StateImageList.Images[n].Clone());

						m.LParam = _fixedInternalStateImageList.Handle;
					}
				}
				base.WndProc(ref m);
			}
		}

		#endregion
	}
}
