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
using ClearCanvas.Common;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Describes worklist ownership options.
    /// </summary>
    public enum WorklistOwnership
    {
        /// <summary>
        /// Worklist is owned by administrators.
        /// </summary>
        Admin,

        /// <summary>
        /// Worklist is owned by a staff person.
        /// </summary>
        Staff,

        /// <summary>
        /// Worklist is owned by a staff group.
        /// </summary>
        Group
    }

	/// <summary>
	/// Internal inteface used to initialize a <see cref="WorkflowFolder"/> once,
	/// without having to define a constructor.
	/// </summary>
	internal interface IInitializeWorklistFolder
	{
		/// <summary>
		/// Initializes this folder with the specified arguments.
		/// </summary>
		void Initialize(Path path, EntityRef worklistRef, string description, WorklistOwnership ownership, string ownerName);
	}

	public interface IWorklistFolder : IFolder
	{
		/// <summary>
		/// Gets the reference to the worklist that populates this folder, or null if the folder is not associated with a worklist instance.
		/// </summary>
		EntityRef WorklistRef { get;}

		/// <summary>
		/// Gets the name of the worklist class that this folder is associated with.
		/// </summary>
		string WorklistClassName { get;}

        /// <summary>
        /// Gets the ownership of the worklist associated with this folder.
        /// </summary>
        WorklistOwnership Ownership { get; }

        /// <summary>
        /// Gets the name of the worklist owner, or null if not applicable.
        /// </summary>
        string OwnerName { get; }
	}


	/// <summary>
	/// Abstract base class for folders that display the contents of worklists.
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	/// <typeparam name="TWorklistService"></typeparam>
	public abstract class WorklistFolder<TItem, TWorklistService> : WorkflowFolder<TItem>, IInitializeWorklistFolder, IWorklistFolder
		where TItem : DataContractBase
		where TWorklistService : IWorklistService<TItem>
	{
		private EntityRef _worklistRef;
        private WorklistOwnership _ownership;
        private string _ownerName;


        #region Personal and Group Iconsets

        private static readonly IconSet _closedUserWorklistIconSet = new IconSet("UserFolderClosedSmall.png");
        private static readonly IconSet _openUserWorklistIconSet = new IconSet("UserFolderOpenSmall.png");

        private static readonly IconSet _closedGroupWorklistIconSet = new IconSet("GroupFolderClosedSmall.png");
        private static readonly IconSet _openGroupWorklistIconSet = new IconSet("GroupFolderClosedSmall.png");

        #endregion

        /// <summary>
		/// Obtains the name of the worklist class associated with the specified folder class.
		/// </summary>
		/// <param name="folderClass"></param>
		/// <returns></returns>
		public static string GetWorklistClassName(Type folderClass)
		{
			var a = AttributeUtils.GetAttribute<FolderForWorklistClassAttribute>(folderClass);
			return a == null ? null : a.WorklistClassName;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="itemsTable"></param>
		protected WorklistFolder(Table<TItem> itemsTable)
			: base(itemsTable)
		{
        }

        #region Folder overrides

        protected override IconSet ClosedIconSet
        {
            get 
            {
                switch (_ownership)
                {
                    case WorklistOwnership.Admin:
                        return base.ClosedIconSet;
                    case WorklistOwnership.Group:
                        return _closedGroupWorklistIconSet;
                    case WorklistOwnership.Staff:
                        return _closedUserWorklistIconSet;
                    default:
                        return base.ClosedIconSet;
                }
            } 
        }

        protected override IconSet OpenIconSet
        {
            get
            {
                switch (_ownership)
                {
                    case WorklistOwnership.Admin:
                        return base.OpenIconSet;
                    case WorklistOwnership.Group:
                        return _openGroupWorklistIconSet;
                    case WorklistOwnership.Staff:
                        return _openUserWorklistIconSet;
                    default:
                        return base.OpenIconSet;
                }
            }
        }

        #endregion

        #region IInitializeWorklistFolder Members

        void IInitializeWorklistFolder.Initialize(Path path, EntityRef worklistRef, string description, WorklistOwnership ownership, string ownerName)
		{
			_worklistRef = worklistRef;
            _ownership = ownership;
            _ownerName = ownerName;

            this.FolderPath = path;
            this.Tooltip = description;
			this.IsStatic = false;  // folder is not static
		}

		#endregion

		public override string Id
		{
			get
			{
				return this.IsStatic ? base.Id : _worklistRef.ToString(false);
			}
		}

		#region IWorklistFolder Members

		/// <summary>
		/// Gets the reference to the worklist that populates this folder, or null if the folder is not associated with a worklist instance.
		/// </summary>
		public EntityRef WorklistRef
		{
			get { return _worklistRef; }
		}

		/// <summary>
		/// Gets the name of the worklist class that this folder is associated with,
		/// typically defined by the <see cref="FolderForWorklistClassAttribute"/>.
		/// </summary>
		public string WorklistClassName
		{
			get
			{
				return GetWorklistClassName(this.GetType());
			}
		}

        /// <summary>
        /// Gets the ownership of the worklist associated with this folder.
        /// </summary>
        public WorklistOwnership Ownership
        {
            get { return _ownership; }
        }

        /// <summary>
        /// Gets the name of the worklist owner, or null if not applicable.
        /// </summary>
        public string OwnerName
        {
            get { return _ownerName; }
        }

		#endregion

		#region Protected API

		/// <summary>
		/// Called to obtain the set of items in the folder.
		/// </summary>
		/// <returns></returns>
		protected override QueryItemsResult QueryItems(int firstRow, int maxRows)
		{
			QueryItemsResult result = null;

			Platform.GetService<TWorklistService>(
				service =>
				{
					var request = CreateQueryWorklistRequest(true, true);
					request.Page = new SearchResultPage(firstRow, maxRows);

					var response = service.QueryWorklist(request);
					result = new QueryItemsResult(response.WorklistItems, response.ItemCount);
				});

			return result;
		}


		/// <summary>
		/// Called to obtain a count of the logical total number of items in the folder (which may be more than the number in memory).
		/// </summary>
		/// <returns></returns>
		protected override int QueryCount()
		{
			var count = -1;

			Platform.GetService<TWorklistService>(
				service =>
				{
					var request = CreateQueryWorklistRequest(false, true);
					var response = service.QueryWorklist(request);
					count = response.ItemCount;
				});

			return count;
		}

		#endregion

		private QueryWorklistRequest CreateQueryWorklistRequest(bool queryItems, bool queryCount)
		{
			return this.WorklistRef == null
					? new QueryWorklistRequest(this.WorklistClassName, queryItems, queryCount, DowntimeRecovery.InDowntimeRecoveryMode, LoginSession.Current.WorkingFacility.FacilityRef)
					: new QueryWorklistRequest(this.WorklistRef, queryItems, queryCount, DowntimeRecovery.InDowntimeRecoveryMode, LoginSession.Current.WorkingFacility.FacilityRef);
		}

	}
}
