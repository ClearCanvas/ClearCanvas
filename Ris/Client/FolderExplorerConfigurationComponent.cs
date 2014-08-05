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
using System.ComponentModel;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Desktop.Trees;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	[ExtensionOf(typeof(ConfigurationPageProviderExtensionPoint), FeatureToken = FeatureTokens.RIS.Core)]
	public class FolderExplorerConfigurationPageProvider : IConfigurationPageProvider
	{
		#region IConfigurationPageProvider Members

		/// <summary>
		/// Gets all the <see cref="IConfigurationPage"/>s for this provider.
		/// </summary>
		public IEnumerable<IConfigurationPage> GetPages()
		{
			var listPages = new List<IConfigurationPage>();

			if (Thread.CurrentPrincipal.IsInRole(Application.Common.AuthorityTokens.Workflow.HomePage.View)
				&& Thread.CurrentPrincipal.IsInRole(Application.Common.AuthorityTokens.Desktop.FolderOrganization)
				&& LoginSession.Current != null && LoginSession.Current.IsStaff
				&& Desktop.Application.SessionStatus == SessionStatus.Online)
			{
				listPages.Add(new ConfigurationPage<FolderExplorerConfigurationComponent>(FolderExplorerConfigurationComponent.ConfigurationPagePath));
			}

			return listPages.AsReadOnly();
		}

		#endregion
	}

	/// <summary>
	/// Extension point for views onto <see cref="FolderExplorerConfigurationComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class FolderExplorerConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// FolderExplorerConfigurationComponent class.
	/// </summary>
	[AssociateView(typeof(FolderExplorerConfigurationComponentViewExtensionPoint))]
	public class FolderExplorerConfigurationComponent : ConfigurationApplicationComponent
	{
		public const string ConfigurationPagePath = "TitleHomepage/TitleFolderOrganization";

		private BindingList<FolderSystemConfigurationNode> _folderSystems;
		private FolderSystemConfigurationNode _selectedFolderSystemNode;
		private SimpleActionModel _folderSystemsActionModel;
		private readonly Tree<FolderConfigurationNodeBase> _folderTree;
		private FolderConfigurationNodeBase _selectedFolderNode;
		private SimpleActionModel _foldersActionModel;
		private readonly IList<IFolderSystem> _folderSystemsToReset;

		private const string _moveFolderSystemUpKey = "MoveFolderSystemUp";
		private const string _moveFolderSystemDownKey = "MoveFolderSystemDown";
		private const string _resetFolderSystemKey = "ResetFolderSystem";
		private const string _addFolderKey = "AddFolder";
		private const string _editFolderKey = "EditFolder";
		private const string _deleteFolderKey = "DeleteFolder";
		private const string _moveFolderUpKey = "MoveFolderUp";
		private const string _moveFolderDownKey = "MoveFolderDown";

		private event EventHandler _onEditFolder;

		public FolderExplorerConfigurationComponent()
		{
			_folderTree = FolderConfigurationNodeBase.BuildTree();
			_folderSystemsToReset = new List<IFolderSystem>();
		}

		public override void Start()
		{
			base.Start();

			// establish default resource resolver on this assembly (not the assembly of the derived class)
			IResourceResolver resourceResolver = new ResourceResolver(typeof(FolderConfigurationNodeBase).Assembly);

			_folderSystemsActionModel = new SimpleActionModel(resourceResolver);
			_folderSystemsActionModel.AddAction(_moveFolderSystemUpKey, SR.TitleMoveUp, "Icons.UpToolSmall.png", SR.TitleMoveUp, MoveFolderSystemUp);
			_folderSystemsActionModel.AddAction(_moveFolderSystemDownKey, SR.TitleMoveDown, "Icons.DownToolSmall.png", SR.TitleMoveDown, MoveFolderSystemDown);
			_folderSystemsActionModel[_moveFolderSystemUpKey].Enabled = false;
			_folderSystemsActionModel[_moveFolderSystemDownKey].Enabled = false;

			_foldersActionModel = new SimpleActionModel(resourceResolver);
			_foldersActionModel.AddAction(_addFolderKey, SR.TitleAddContainerFolder, "Icons.AddToolSmall.png", SR.TitleAddContainerFolder, AddFolder);
			var editFolderAction = _foldersActionModel.AddAction(_editFolderKey, SR.TitleRenameFolder, "Icons.EditToolSmall.png", SR.TitleRenameFolder, EditFolder);
			_foldersActionModel.AddAction(_deleteFolderKey, SR.TitleDeleteContainerFolder, "Icons.DeleteToolSmall.png", SR.TitleDeleteContainerFolder, DeleteFolder);
			_foldersActionModel.AddAction(_moveFolderUpKey, SR.TitleMoveUp, "Icons.UpToolSmall.png", SR.TitleMoveUp, MoveFolderUp);
			_foldersActionModel.AddAction(_moveFolderDownKey, SR.TitleMoveDown, "Icons.DownToolSmall.png", SR.TitleMoveDown, MoveFolderDown);
			_foldersActionModel.AddAction(_resetFolderSystemKey, SR.TitleReset, "Icons.ResetToolSmall.png", SR.MessageResetAllFolders, ResetFolderSystem);
			_foldersActionModel[_addFolderKey].Enabled = false;
			_foldersActionModel[_editFolderKey].Enabled = false;
			_foldersActionModel[_deleteFolderKey].Enabled = false;
			_foldersActionModel[_moveFolderUpKey].Enabled = false;
			_foldersActionModel[_moveFolderDownKey].Enabled = false;
			_foldersActionModel[_resetFolderSystemKey].Enabled = false;

			editFolderAction.KeyStroke = XKeys.F2;

			LoadFolderSystems();
		}

		#region IConfigurationApplicationComponent Members

		public override void Save()
		{
			FolderExplorerComponentSettings.Default.UpdateUserConfiguration(userConfiguration =>
				{
					foreach(var folderSystem in _folderSystemsToReset)
					{
						userConfiguration.RemoveUserFoldersCustomizations(folderSystem);
					}

					// Save the ordering of the folder systems
					userConfiguration.SaveUserFolderSystemsOrder(
						CollectionUtils.Map<FolderSystemConfigurationNode, IFolderSystem>(
							_folderSystems,
							node => node.FolderSystem));

					// and then save each folder systems' folder customizations
					CollectionUtils.ForEach(_folderSystems, node =>
					{
						if (!node.Modified) return;

						node.UpdateFolderPath();
						userConfiguration.SaveUserFoldersCustomizations(
							node.FolderSystem, node.Folders);
					});
				});
		}

		#endregion

		#region Folder Systems Presentation Model

		public IBindingList FolderSystems
		{
			get { return _folderSystems; }
		}

		public string FormatFolderSystem(object item)
		{
			return ((FolderSystemConfigurationNode)item).Text;
		}

		public int SelectedFolderSystemIndex
		{
			get { return _selectedFolderSystemNode == null ? -1 : _folderSystems.IndexOf(_selectedFolderSystemNode); }
			set
			{
				var previousSelection = _selectedFolderSystemNode;
				_selectedFolderSystemNode = value < 0 ? null : _folderSystems[value];

				UpdateFolderSystemActionModel();
				UpdateFolderActionModel(this.FolderEditorEnabled);

				NotifyPropertyChanged("SelectedFolderSystemIndex");

				BuildFolderTreeIfNotExist(_selectedFolderSystemNode);

				if (previousSelection != _selectedFolderSystemNode)
				{
					_folderTree.Items.Clear();
					_folderTree.Items.Add(_selectedFolderSystemNode);
					this.SelectedFolderNode = new Selection(_selectedFolderSystemNode);
				}
			}
		}

		public bool FolderEditorEnabled
		{
			get { return _selectedFolderSystemNode != null ? !_selectedFolderSystemNode.Readonly : false; }
		}

		public bool CanMoveFolderSystemUp
		{
			get { return this.SelectedFolderSystemIndex > 0; }
		}

		public bool CanMoveFolderSystemDown
		{
			get { return this.SelectedFolderSystemIndex < _folderSystems.Count - 1; }
		}

		public void MoveSelectedFolderSystem(int index, int newIndex)
		{
			// invalid, should never happen.
			if (index < 0)
				return;

			// If a node is dragged to an empty space (not on a node), put it at the end of the list
			if (newIndex < 0)
				newIndex = _folderSystems.Count - 1;

			// Instead of remove/insert node at the source/target index, we opt to move folder system node up and down
			// one at a time.  So the selected node never leave the tree.  The folder tree won't flicker.
			if (newIndex > index)
			{
				while (newIndex > index)
				{
					MoveFolderSystemDown();
					index++;
				}
			}
			else
			{
				while (newIndex < index)
				{
					MoveFolderSystemUp();
					index--;
				}
			}
		}

		/// <summary>
		/// Gets the folder systems action model.
		/// </summary>
		public ActionModelNode FolderSystemsActionModel
		{
			get { return _folderSystemsActionModel; }
		}

		#endregion

		#region Folders Presentation Model

		public ITree FolderTree
		{
			get { return _folderTree; }
		}

		public string SelectedFolderNodeText
		{
			get { return _selectedFolderNode.Text; }
			set { _selectedFolderNode.Text = value; }
		}

		public ISelection SelectedFolderNode
		{
			get { return new Selection(_selectedFolderNode); }
			set
			{
				var node = (FolderConfigurationNodeBase)value.Item;

				_selectedFolderNode = node;
				UpdateFolderActionModel();
				NotifyPropertyChanged("SelectedFolderNode");
			}
		}

		/// <summary>
		/// Gets the folders action model.
		/// </summary>
		public ActionModelNode FoldersActionModel
		{
			get { return _foldersActionModel; }
		}

		public event EventHandler OnEditFolder
		{
			add { _onEditFolder += value; }
			remove { _onEditFolder -= value; }
		}

		public void OnItemDropped(object droppedItem, DragDropKind kind)
		{
			if (droppedItem is FolderConfigurationNodeBase && kind == DragDropKind.Move)
			{
				var droppedNode = (FolderConfigurationNodeBase)droppedItem;
				this.SelectedFolderNode = new Selection(droppedNode);
			}
		}

		#endregion

		#region Folder Systems Helper

		private void LoadFolderSystems()
		{
			var folderSystems = FolderExplorerComponentSettings.Default.ApplyFolderSystemsOrder(
				CollectionUtils.Cast<IFolderSystem>(new FolderSystemExtensionPoint().CreateExtensions()));

			var fsNodes = CollectionUtils.Map<IFolderSystem, FolderSystemConfigurationNode>(
				folderSystems,
				fs => new FolderSystemConfigurationNode(fs, FolderExplorerComponentSettings.Default.IsFolderSystemReadOnly(fs)));

			CollectionUtils.ForEach(
				fsNodes, 
				node => node.ModifiedChanged += ((sender, args) => this.Modified = true));

			_folderSystems = new BindingList<FolderSystemConfigurationNode>(fsNodes);

			// Set the initial selected folder system
			if (_folderSystems.Count > 0)
				this.SelectedFolderSystemIndex = 0;
		}

		private void MoveFolderSystemUp()
		{
			if (!this.CanMoveFolderSystemUp)
				return;

			NudgeSelectedFolderSystemUpOrDownOnePosition(false);
		}

		private void MoveFolderSystemDown()
		{
			if (!this.CanMoveFolderSystemDown)
				return;

			NudgeSelectedFolderSystemUpOrDownOnePosition(true);
		}

		private void ResetFolderSystem()
		{
			if (_selectedFolderSystemNode == null)
				return;

			_folderSystemsToReset.Add(_selectedFolderSystemNode.FolderSystem);
			RefreshSelectedFolderSystemFolders(false);
			this.Modified = true;
		}

		private void RefreshSelectedFolderSystemFolders(bool includeUserCustomizations)
		{
			if(_selectedFolderSystemNode == null)
				return;

			_selectedFolderSystemNode.ClearSubTree();
			BuildFolderTreeIfNotExist(_selectedFolderSystemNode, includeUserCustomizations);

			_folderTree.Items.Clear();
			_folderTree.Items.Add(_selectedFolderSystemNode);
			this.SelectedFolderNode = new Selection(_selectedFolderSystemNode);
		}

		private void NudgeSelectedFolderSystemUpOrDownOnePosition(bool up)
		{
			var offset = up ? +1 : -1;

			// We don't want to remove/insert the selected node, but rather the node before, so the folder tree does not flicker
			var index = this.SelectedFolderSystemIndex;
			var fs = _folderSystems[index + offset];
			_folderSystems.Remove(fs);
			_folderSystems.Insert(index, fs);

			this.SelectedFolderSystemIndex = index + offset;
			this.Modified = true;
		}

		private void UpdateFolderSystemActionModel()
		{
			_folderSystemsActionModel[_moveFolderSystemUpKey].Enabled = this.CanMoveFolderSystemUp;
			_folderSystemsActionModel[_moveFolderSystemDownKey].Enabled = this.CanMoveFolderSystemDown;
		}

		#endregion

		#region Folders Helper

		private void BuildFolderTreeIfNotExist(FolderSystemConfigurationNode folderSystemNode, bool includeUserCustomizations)
		{
			if (folderSystemNode.SubTree != null)
				return;

			// Initialize the list of Folders
			folderSystemNode.InitializeFolderSystemOnce();

			var folders = FolderExplorerComponentSettings.Default.ApplyFolderCustomizations(folderSystemNode.FolderSystem, includeUserCustomizations);

			// add each ordered folder to the tree
			folderSystemNode.ModifiedEnabled = false;
			folderSystemNode.ClearSubTree();
			CollectionUtils.ForEach(folders, folder => folderSystemNode.InsertNode(new FolderConfigurationNode(folder), folder.FolderPath));

			folderSystemNode.ModifiedEnabled = true;
		}

		private void BuildFolderTreeIfNotExist(FolderSystemConfigurationNode folderSystemNode)
		{
			BuildFolderTreeIfNotExist(folderSystemNode, true);
		}

		private void AddFolder()
		{
			var newFolderNode = new FolderConfigurationNodeBase.ContainerNode("New Folder");
			_selectedFolderNode.AddChildNode(newFolderNode);
			this.SelectedFolderNode = new Selection(newFolderNode);
			EventsHelper.Fire(_onEditFolder, this, EventArgs.Empty);
		}

		private void EditFolder()
		{
			EventsHelper.Fire(_onEditFolder, this, EventArgs.Empty);
		}

		private void DeleteFolder()
		{
			var parentNode = _selectedFolderNode.Parent;
			var nextSelectedNode = parentNode.RemoveChildNode(_selectedFolderNode);
			this.SelectedFolderNode = new Selection(nextSelectedNode);
		}

		private void MoveFolderUp()
		{
			_selectedFolderNode.MoveUp();

			// Must update action model because the node index may have changed after moving node.
			UpdateFolderActionModel();
		}

		private void MoveFolderDown()
		{
			_selectedFolderNode.MoveDown();

			// Must update action model because the node index may have changed after moving node.
			UpdateFolderActionModel();
		}

		private void UpdateFolderActionModel()
		{
			UpdateFolderActionModel(true);
		}

		private void UpdateFolderActionModel(bool canEditFolderSystem)
		{
			var editsEnabled = canEditFolderSystem && _selectedFolderNode != null;

			_foldersActionModel[_addFolderKey].Enabled = editsEnabled;
			_foldersActionModel[_editFolderKey].Enabled = editsEnabled && _selectedFolderNode.CanEdit;
			_foldersActionModel[_deleteFolderKey].Enabled = editsEnabled && _selectedFolderNode.CanDelete;
			_foldersActionModel[_moveFolderUpKey].Enabled = editsEnabled && _selectedFolderNode.PreviousSibling != null;
			_foldersActionModel[_moveFolderDownKey].Enabled = editsEnabled && _selectedFolderNode.NextSibling != null;
			_foldersActionModel[_resetFolderSystemKey].Enabled = editsEnabled;
		}

		#endregion
	}
}
