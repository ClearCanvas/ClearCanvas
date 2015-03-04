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
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ProcedureTypeGroupAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
    /// <summary>
    /// Extension point for views onto <see cref="ProcedureTypeGroupSummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ProcedureTypeGroupSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ProcedureTypeGroupSummaryComponent class
    /// </summary>
	[AssociateView(typeof(ProcedureTypeGroupSummaryComponentViewExtensionPoint))]
	public class ProcedureTypeGroupSummaryComponent : SummaryComponentBase<ProcedureTypeGroupSummary, ProcedureTypeGroupSummaryTable, ListProcedureTypeGroupsRequest>
    {
    	private readonly EnumValueInfo _filterNone;
		private readonly List<EnumValueInfo> _categoryChoices;
		private EnumValueInfo _selectedCategory;
		
        /// <summary>
        /// Constructor
        /// </summary>
        public ProcedureTypeGroupSummaryComponent()
			: this(false)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dialogMode">Indicates whether the component will be shown in a dialog box or not</param>
        public ProcedureTypeGroupSummaryComponent(bool dialogMode)
            :base(dialogMode)
        {
			_filterNone = new EnumValueInfo(SR.DummyItemNone, SR.DummyItemNone);
			_categoryChoices = new List<EnumValueInfo>();
        }

		public override void Start()
		{
			_categoryChoices.Add(_filterNone);
			_selectedCategory = _filterNone;

			Platform.GetService<IProcedureTypeGroupAdminService>(
				delegate(IProcedureTypeGroupAdminService service)
					{
						GetProcedureTypeGroupSummaryFormDataResponse response =
							service.GetProcedureTypeGroupSummaryFormData(new GetProcedureTypeGroupSummaryFormDataRequest());
						_categoryChoices.AddRange(response.CategoryChoices);
						_categoryChoices.Sort(delegate(EnumValueInfo x, EnumValueInfo y) { return x.Value.CompareTo(y.Value); });
					});


			base.Start();
		}

		#region Presentation Model

		public IList CategoryChoices
		{
			get 
			{
				return _categoryChoices;
			}
		}

		public string FormatCategory(object item)
		{
			if (item is EnumValueInfo)
			{
				EnumValueInfo category = (EnumValueInfo)item;
				return category.Value;
			}
			else
				return item.ToString(); // place-holder items
		}

		public EnumValueInfo SelectedCategory
		{
			get { return _selectedCategory; }
			set
			{
				_selectedCategory = value;
			}
		}

		public EnumValueInfo NullFilter
		{
			get { return _filterNone; }
		}

		#endregion

		#region Overrides

		/// <summary>
		/// Override this method to perform custom initialization of the action model,
		/// such as adding permissions or adding custom actions.
		/// </summary>
		/// <param name="model"></param>
		protected override void InitializeActionModel(AdminActionModel model)
		{
			base.InitializeActionModel(model);

			model.Add.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.ProcedureTypeGroup);
			model.Edit.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.ProcedureTypeGroup);
			model.Delete.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.ProcedureTypeGroup);
		}

		protected override bool SupportsDelete
		{
			get { return true; }
		}
		
		/// <summary>
		/// Gets the list of items to show in the table, according to the specifed first and max items.
		/// </summary>
		/// <returns></returns>
		protected override IList<ProcedureTypeGroupSummary> ListItems(ListProcedureTypeGroupsRequest request)
		{
			ListProcedureTypeGroupsResponse listResponse = null;
			Platform.GetService<IProcedureTypeGroupAdminService>(
				delegate(IProcedureTypeGroupAdminService service)
				{
					request.CategoryFilter = (_selectedCategory == _filterNone) ? null : _selectedCategory;

					listResponse = service.ListProcedureTypeGroups(request);
				});

			return listResponse.Items;
		}

		/// <summary>
		/// Called to handle the "add" action.
		/// </summary>
		/// <param name="addedItems"></param>
		/// <returns>True if items were added, false otherwise.</returns>
		protected override bool AddItems(out IList<ProcedureTypeGroupSummary> addedItems)
		{
			addedItems = new List<ProcedureTypeGroupSummary>();
			ProcedureTypeGroupEditorComponent editor = new ProcedureTypeGroupEditorComponent();
			ApplicationComponentExitCode exitCode = LaunchAsDialog(
				this.Host.DesktopWindow, editor, SR.TitleAddProcedureTypeGroup);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				addedItems.Add(editor.ProcedureTypeGroupSummary);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Called to handle the "edit" action.
		/// </summary>
		/// <param name="items">A list of items to edit.</param>
		/// <param name="editedItems">The list of items that were edited.</param>
		/// <returns>True if items were edited, false otherwise.</returns>
		protected override bool EditItems(IList<ProcedureTypeGroupSummary> items, out IList<ProcedureTypeGroupSummary> editedItems)
		{
			editedItems = new List<ProcedureTypeGroupSummary>();
			ProcedureTypeGroupSummary item = CollectionUtils.FirstElement(items);

			ProcedureTypeGroupEditorComponent editor = new ProcedureTypeGroupEditorComponent(item.ProcedureTypeGroupRef);
			ApplicationComponentExitCode exitCode = LaunchAsDialog(
				this.Host.DesktopWindow, editor, string.Format(SR.FormatTitleSubtitleCategory, SR.TitleUpdateProcedureTypeGroup, item.Name, item.Category));
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				editedItems.Add(editor.ProcedureTypeGroupSummary);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Called to handle the "delete" action, if supported.
		/// </summary>
		/// <param name="items"></param>
		/// <param name="deletedItems">The list of items that were deleted.</param>
		/// <param name="failureMessage">The message if there any errors that occurs during deletion.</param>
		/// <returns>True if items were deleted, false otherwise.</returns>
		protected override bool DeleteItems(IList<ProcedureTypeGroupSummary> items, out IList<ProcedureTypeGroupSummary> deletedItems, out string failureMessage)
		{
			failureMessage = null;
			deletedItems = new List<ProcedureTypeGroupSummary>();

			foreach (ProcedureTypeGroupSummary item in items)
			{
				try
				{
					Platform.GetService<IProcedureTypeGroupAdminService>(
						delegate(IProcedureTypeGroupAdminService service)
						{
							service.DeleteProcedureTypeGroup(new DeleteProcedureTypeGroupRequest(item.ProcedureTypeGroupRef));
						});

					deletedItems.Add(item);
				}
				catch (Exception e)
				{
					failureMessage = e.Message;
				}
			}

			return deletedItems.Count > 0;
		}

		/// <summary>
		/// Compares two items to see if they represent the same item.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		protected override bool IsSameItem(ProcedureTypeGroupSummary x, ProcedureTypeGroupSummary y)
		{
			return x.ProcedureTypeGroupRef.Equals(y.ProcedureTypeGroupRef, true);
		}

		#endregion
	}
}
