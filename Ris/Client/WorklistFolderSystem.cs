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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    public interface IWorklistFolderSystem : IFolderSystem
    {
        /// <summary>
        /// Gets a list of class names of worklist classes supported by this folder system.
        /// </summary>
        IList<string> SupportedWorklistClasses { get; }


        /// <summary>
        /// Attempts to add the specified worklist to this folder system.
        /// </summary>
        /// <remarks>
        /// Returns the new folder if successful, or null if this folder system does not support the
        /// specified worklist class.
        /// </remarks>
        IWorklistFolder AddWorklistFolder(WorklistSummary worklist);

		/// <summary>
		/// Updates the specified worklist folder to reflect the specified worklist.
		/// </summary>
		/// <param name="folder"></param>
		/// <param name="worklist"></param>
    	void UpdateWorklistFolder(IWorklistFolder folder, WorklistSummary worklist);
    }


    /// <summary>
    /// Abstract base class for folder systems that consist of <see cref="WorklistFolder{TItem,TWorklistService}"/>s.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <typeparam name="TFolderExtensionPoint"></typeparam>
    /// <typeparam name="TFolderToolExtensionPoint"></typeparam>
    /// <typeparam name="TItemToolExtensionPoint"></typeparam>
    /// <typeparam name="TWorklistService"></typeparam>
    public abstract class WorklistFolderSystem<TItem, TFolderExtensionPoint, TFolderToolExtensionPoint, TItemToolExtensionPoint, TWorklistService>
        : WorkflowFolderSystem<TItem, TFolderToolExtensionPoint, TItemToolExtensionPoint, WorklistSearchParams>, IWorklistFolderSystem
        where TItem : DataContractBase
        where TFolderExtensionPoint : ExtensionPoint<IWorklistFolder>, new()
        where TFolderToolExtensionPoint : ExtensionPoint<ITool>, new()
        where TItemToolExtensionPoint : ExtensionPoint<ITool>, new()
        where TWorklistService : IWorklistService<TItem>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="title"></param>
        protected WorklistFolderSystem(string title)
            : base(title)
        {
        }

        #region IWorklistFolderSystem Members

        /// <summary>
        /// Gets a list of class names of worklist classes supported by this folder system.
        /// </summary>
        public IList<string> SupportedWorklistClasses
        {
            get
            {
                return CollectionUtils.Map(
                    new TFolderExtensionPoint().ListExtensions(),
                    (ExtensionInfo info) => GetWorklistClassForFolderClass(info.ExtensionClass.Resolve()));
            }
        }

        /// <summary>
        /// Attempts to add the specified worklist to this folder system.
        /// </summary>
        /// <remarks>
        /// Returns the new folder if successful, or null if this folder system does not support the
        /// specified worklist class.
        /// </remarks>
        public IWorklistFolder AddWorklistFolder(WorklistSummary worklist)
        {
            return AddWorklistFolderHelper(worklist);
        }

    	/// <summary>
    	/// Updates the specified worklist folder to reflect the specified worklist.
    	/// </summary>
    	/// <param name="folder"></param>
    	/// <param name="worklist"></param>
    	public void UpdateWorklistFolder(IWorklistFolder folder, WorklistSummary worklist)
    	{
			if(!this.Folders.Contains(folder))
				return;

    		InitializeFolderProperties(folder, worklist);

			// notify that the folder properties have changed
			NotifyFolderPropertiesChanged(folder);
    	}

    	public override SearchParams CreateSearchParams(string searchText)
        {
            return new WorklistSearchParams(searchText);
        }

        public override void LaunchSearchComponent()
        {
            SearchComponent.Launch(this.DesktopWindow);
        }

        public override Type SearchComponentType
        {
            get { return typeof(SearchComponent); }
        }

        #endregion

        #region Overrides

        ///<summary>
        ///Initializes or re-initializes the folder system.
        ///</summary>
        ///<remarks>
        /// This method will be called after <see cref="M:ClearCanvas.Ris.Client.IFolderSystem.SetContext(ClearCanvas.Ris.Client.IFolderSystemContext)" /> has been called. 
        ///</remarks>
        public override void Initialize()
        {
            base.Initialize();

            this.Folders.Clear();

            InitializeFolders(new TFolderExtensionPoint());
        }


        /// <summary>
        /// Called to obtain the set of worklists for the current user.  May be overridden, but typically not necessary.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual ListWorklistsForUserResponse QueryWorklistSet(ListWorklistsForUserRequest request)
        {
            ListWorklistsForUserResponse response = null;
            Platform.GetService<TWorklistService>(
				service => response = service.ListWorklistsForUser(request));

            return response;
        }

        #endregion

        #region Protected API

        /// <summary>
        /// This method should be overridden by any folder systems that require folders not added through the extension mechanism.
        /// </summary>
        protected virtual void AddDefaultFolders()
        {
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Creates the folder system based on the specified extension point.
        /// </summary>
        /// <param name="folderExtensionPoint"></param>
        private void InitializeFolders(ExtensionPoint<IWorklistFolder> folderExtensionPoint)
        {
            AddDefaultFolders();

            var worklistClassNames = new List<string>();

            // collect worklist class names, and add unfiltered folders if authorized
            foreach (IWorklistFolder folder in folderExtensionPoint.CreateExtensions())
            {
                if (!string.IsNullOrEmpty(folder.WorklistClassName))
                    worklistClassNames.Add(folder.WorklistClassName);

#if DEBUG	// only available in debug builds (only used for testing)
                // if unfiltered folders are visible, add the root folder
                if (Thread.CurrentPrincipal.IsInRole(Application.Common.AuthorityTokens.Development.ViewUnfilteredWorkflowFolders))
                {
                    this.Folders.Add(folder);
                }
#endif
            }

            // query the set of worklists to which the current user is subscribed, add a folder for each
            var response = QueryWorklistSet(new ListWorklistsForUserRequest(new List<string>(worklistClassNames)));
            foreach (var summary in response.Worklists)
            {
                var added = AddWorklistFolderHelper(summary);

                if (added == null)
                    Platform.Log(LogLevel.Error, string.Format("Worklist class {0} not added to folder system, most likely because it is not mapped to a folder class.", summary.ClassName));
            }
        }

        /// <summary>
        /// Creates and adds a worklist folder for the specified worklist.
        /// </summary>
        /// <param name="worklist"></param>
        /// <returns></returns>
        private IWorklistFolder AddWorklistFolderHelper(WorklistSummary worklist)
        {
            // create an instance of the folder corresponding to the specified worklist class
            var folder = (IWorklistFolder)new TFolderExtensionPoint()
                .CreateExtension(info => worklist.ClassName == GetWorklistClassForFolderClass(info.ExtensionClass.Resolve()));

            if (folder == null || !(folder is IInitializeWorklistFolder))
                return null;

            InitializeFolderProperties(folder, worklist);

        	// add to folders list
            this.Folders.Add(folder);

            return folder;
        }

		/// <summary>
		/// Initializes the specified folder's properties from the specified worklist.
		/// </summary>
		/// <param name="worklist"></param>
		/// <param name="folder"></param>
    	private static void InitializeFolderProperties(IWorklistFolder folder, WorklistSummary worklist)
    	{
			var initFolder = (IInitializeWorklistFolder)folder;

    		// augment default base path with worklist name
    		var path = folder.FolderPath;
    		if (!string.IsNullOrEmpty(worklist.DisplayName))
    		{
    			path = path.Append(new PathSegment(worklist.DisplayName, folder.ResourceResolver));
    		}

    		// init folder
    		initFolder.Initialize(
    			path,
    			worklist.WorklistRef,
    			worklist.Description,
    			GetWorklistOwnership(worklist),
    			GetWorklistOwnerName(worklist)
    			);
    	}

    	private static string GetWorklistClassForFolderClass(Type folderClass)
        {
            return WorklistFolder<TItem, TWorklistService>.GetWorklistClassName(folderClass);
        }

        private static WorklistOwnership GetWorklistOwnership(WorklistSummary worklist)
        {
            return worklist.IsUserWorklist ?
                (worklist.IsStaffOwned ? WorklistOwnership.Staff : WorklistOwnership.Group)
                : WorklistOwnership.Admin;
        }

        private static string GetWorklistOwnerName(WorklistSummary worklist)
        {
            if (worklist.IsStaffOwned)
                return Formatting.StaffNameAndRoleFormat.Format(worklist.OwnerStaff);
            if (worklist.IsGroupOwned)
                return worklist.OwnerGroup.Name;
            return null;
        }

        #endregion

    }
}
