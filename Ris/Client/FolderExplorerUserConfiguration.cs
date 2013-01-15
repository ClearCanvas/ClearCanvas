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
using System.IO;
using System.Text;
using System.Xml;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Interface for updating a users customizations to the folder explorer
	/// </summary>
	public interface IFolderExplorerUserConfigurationUpdater
	{
		/// <summary>
		/// Sets the order in which folder systems should appear in the folder explorer
		/// </summary>
		/// <param name="folderSystems"></param>
		void SaveUserFolderSystemsOrder(IEnumerable<IFolderSystem> folderSystems);

		/// <summary>
		/// Saves the customizations contained in the specified <see cref="IFolder"/> list for the specified <see cref="IFolderSystem"/>
		/// </summary>
		/// <param name="folderSystem"></param>
		/// <param name="customizedFolders"></param>
		void SaveUserFoldersCustomizations(IFolderSystem folderSystem, List<IFolder> customizedFolders);

		/// <summary>
		/// Removes all user customizations from the specified <see cref="IFolderSystem"/>
		/// </summary>
		/// <param name="folderSystem"></param>
		void RemoveUserFoldersCustomizations(IFolderSystem folderSystem);
	}

	/// <summary>
	/// Interface for applying, updating and transactionally persisting a per user customizations to the folder explorer
	/// </summary>
	public interface IFolderExplorerUserConfiguration : IFolderExplorerConfiguration, IFolderExplorerUserConfigurationUpdater
	{
		/// <summary>
		/// Begins a transaction.
		/// </summary>
		void BeginTransaction();

		/// <summary>
		/// Commits the transaction.
		/// </summary>
		void CommitTransaction();

		/// <summary>
		/// Rollsback the transaction.
		/// </summary>
		void RollbackTransaction();

		/// <summary>
		/// Indicates that the current user's folder/folder system customizations have been committed.
		/// </summary>
		event EventHandler ChangesCommitted;
	}

	/// <summary>
	/// Provides per user folder explorer customizations
	/// </summary>
	public class FolderExplorerUserConfiguration : FolderExplorerConfiguration, IFolderExplorerUserConfiguration
	{
		#region Private fields

		private event EventHandler _changesCommitted;
		private bool _transactionPending;
		private readonly Action<string> _updateSetting;

		#endregion

		#region Constructor

		public FolderExplorerUserConfiguration(SettingProvider settingXml, Action<string> updateSetting)
			: base(settingXml)
		{
			_updateSetting = updateSetting;
		}

		#endregion

		#region IFolderExplorerUserConfiguration members

		public void BeginTransaction()
		{
			_transactionPending = true;
		}

		public void RollbackTransaction()
		{
			if (!_transactionPending)
				throw new InvalidOperationException("Must call BeginTransaction() first.");

			this.XmlDocument = null;     // force xml doc to be reloaded from stored setting
			_transactionPending = false;
		}

		public void CommitTransaction()
		{
			if (!_transactionPending)
				throw new InvalidOperationException("Must call BeginTransaction() first.");

			// copy document to setting
			var sb = new StringBuilder();
			var writer = new XmlTextWriter(new StringWriter(sb)) { Formatting = System.Xml.Formatting.Indented };
			this.XmlDocument.Save(writer);

			_updateSetting(sb.ToString());

			this.XmlDocument = null;
			_transactionPending = false;

			// notify subscribers
			EventsHelper.Fire(_changesCommitted, this, EventArgs.Empty);
		}

		/// <summary>
		/// Saves the order of the specified folder sytems.
		/// </summary>
		/// <param name="folderSystems"></param>
		public void SaveUserFolderSystemsOrder(IEnumerable<IFolderSystem> folderSystems)
		{
			if (!_transactionPending)
				throw new InvalidOperationException("Must call BeginTransaction() first.");

			var replacementFolderSystems = this.XmlDocument.CreateElement("folder-systems");

			foreach (var folderSystem in folderSystems)
			{
				var existingFolderSystem = FindXmlFolderSystem(folderSystem.Id) ?? CreateXmlFolderSystem(folderSystem.Id);
				replacementFolderSystems.AppendChild(existingFolderSystem.CloneNode(true));
			}

			GetFolderSystemsNode().InnerXml = replacementFolderSystems.InnerXml;
		}

		/// <summary>
		/// For the specified <see cref="IFolderSystem"/>, saves customizations of paths and visibility of <see cref="IFolder"/> items in the list.
		/// </summary>
		/// <param name="folderSystem"></param>
		/// <param name="customizedFolders"></param>
		public void SaveUserFoldersCustomizations(IFolderSystem folderSystem, List<IFolder> customizedFolders)
		{
			if (!_transactionPending)
				throw new InvalidOperationException("Must call BeginTransaction() first.");

			var xmlFolderSystem = FindXmlFolderSystem(folderSystem.Id);
			var replacementFolderSystem = CreateXmlFolderSystem(folderSystem.Id);

			foreach (var folder in customizedFolders)
			{
				var newXmlFolder = CreateXmlFolder(folder);
				replacementFolderSystem.AppendChild(newXmlFolder);
			}

			var folderSystemsNode = GetFolderSystemsNode();
			if (xmlFolderSystem != null)
				xmlFolderSystem.InnerXml = replacementFolderSystem.InnerXml;
			else
				folderSystemsNode.AppendChild(replacementFolderSystem);
		}

		public void RemoveUserFoldersCustomizations(IFolderSystem folderSystem)
		{
			var xmlFolderSystem = FindXmlFolderSystem(folderSystem.Id);

			if (xmlFolderSystem != null)
				this.GetFolderSystemsNode().ReplaceChild(CreateXmlFolderSystem(folderSystem.Id), xmlFolderSystem);
		}

		public event EventHandler ChangesCommitted
		{
			add { _changesCommitted += value; }
			remove { _changesCommitted -= value; }
		}

		#endregion

		#region FolderExplorerConfiguration overrides

		protected override Predicate<IFolder> GetFolderFilterFromCustomizationSpec(XmlElement element)
		{
			// User customizations are made to a specific folder, not a folder class, so use the "id"
			var id = element.GetAttribute("id");
			return f => f.Id == id;
		}

		protected override void CustomizeItem(IFolder folder, XmlElement element)
		{
			// reuse visibility customization from FolderExplorerConfiguration
			base.CustomizeItem(folder, element);

			// and add path customization
			var pathSetting = element.GetAttribute("path");
			if (!string.IsNullOrEmpty(pathSetting))
			{
				folder.FolderPath = new Desktop.Path(pathSetting);
			}
		}

		protected override IList<XmlNode> GetFolderCustomizationSpecsForFolderSystem(string folderSystemId)
		{
			// for user customizations, need to create any missing nodes so that new customizations can be stored
			var xmlFolderSystem = FindXmlFolderSystem(folderSystemId) ?? CreateXmlFolderSystem(folderSystemId);
			// User customizations are made to a specific folder, not a folder class, so look for <folder> nodes
			return CollectionUtils.Select<XmlNode>(xmlFolderSystem.ChildNodes, n => n.Name == "folder");
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Creates the specified folder system, but *does not* immediately append it to the xmlDoc.
		/// </summary>
		/// <param name="id">the id of the "folder-system" to create</param>
		/// <returns>An "folder-system" element</returns>
		protected XmlElement CreateXmlFolderSystem(string id)
		{
			var xmlFolderSystem = this.XmlDocument.CreateElement("folder-system");
			xmlFolderSystem.SetAttribute("id", id);
			return xmlFolderSystem;
		}

		/// <summary>
		/// Creates a "folder" node for insertion into an "folder-system" node in the Xml store.
		/// </summary>
		/// <param name="folder">the folder whose relevant properties are to be used to create the node</param>
		/// <returns>a "folder" element</returns>
		protected XmlElement CreateXmlFolder(IFolder folder)
		{
			var xmlFolder = this.XmlDocument.CreateElement("folder");

			xmlFolder.SetAttribute("id", folder.Id);
			xmlFolder.SetAttribute("path", folder.FolderPath.LocalizedPath);
			xmlFolder.SetAttribute("visible", folder.Visible.ToString());

			return xmlFolder;
		}

		#endregion
	}
}