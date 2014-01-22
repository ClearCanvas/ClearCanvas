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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Desktop;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	public class PhoneNumbersSummaryComponent : SummaryComponentBase<TelephoneDetail, TelephoneNumberTable>
    {
        private IList<TelephoneDetail> _phoneNumberList;
        private readonly List<EnumValueInfo> _phoneTypeChoices;

        public PhoneNumbersSummaryComponent(List<EnumValueInfo> phoneTypeChoices)
			: base(false)
        {
            _phoneTypeChoices = phoneTypeChoices;
        }

        public IList<TelephoneDetail> Subject
        {
            get { return _phoneNumberList; }
            set { _phoneNumberList = value; }
        }

        #region Overrides

		/// <summary>
		/// Gets a value indicating whether this component supports deletion.  The default is false.
		/// Override this method to support deletion.
		/// </summary>
		protected override bool SupportsDelete
		{
			get { return true; }
		}

		/// <summary>
		/// Gets a value indicating whether this component supports paging.  The default is true.
		/// Override this method to change support for paging.
		/// </summary>
		protected override bool SupportsPaging
		{
			get { return false; }
		}

		/// <summary>
		/// Gets the list of items to show in the table, according to the specifed first and max items.
		/// </summary>
		/// <param name="firstItem"></param>
		/// <param name="maxItems"></param>
		/// <returns></returns>
		protected override IList<TelephoneDetail> ListItems(int firstItem, int maxItems)
		{
			return _phoneNumberList;
		}

		/// <summary>
		/// Called to handle the "add" action.
		/// </summary>
		/// <param name="addedItems"></param>
		/// <returns>True if items were added, false otherwise.</returns>
		protected override bool AddItems(out IList<TelephoneDetail> addedItems)
		{
			addedItems = new List<TelephoneDetail>();

			TelephoneDetail phoneNumber = new TelephoneDetail();
			phoneNumber.Type = _phoneTypeChoices[0];

			PhoneNumberEditorComponent editor = new PhoneNumberEditorComponent(phoneNumber, _phoneTypeChoices);
			ApplicationComponentExitCode exitCode = LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleAddPhoneNumber);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				addedItems.Add(phoneNumber);
				_phoneNumberList.Add(phoneNumber);
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
		protected override bool EditItems(IList<TelephoneDetail> items, out IList<TelephoneDetail> editedItems)
		{
			editedItems = new List<TelephoneDetail>();
			TelephoneDetail oldItem = CollectionUtils.FirstElement(items);
			TelephoneDetail newItem = (TelephoneDetail)oldItem.Clone();

			PhoneNumberEditorComponent editor = new PhoneNumberEditorComponent(newItem, _phoneTypeChoices);
			ApplicationComponentExitCode exitCode = LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleUpdatePhoneNumber);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				editedItems.Add(newItem);

				// Since there is no way to use IsSameItem to identify the address before and after are the same
				// We must manually remove the old and add the new item
				this.Table.Items.Replace(
					delegate(TelephoneDetail x) { return IsSameItem(oldItem, x); },
					newItem);

				// Preserve the order of the items
				int index = _phoneNumberList.IndexOf(oldItem);
				_phoneNumberList.Insert(index, newItem);
				_phoneNumberList.Remove(oldItem);

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
		protected override bool DeleteItems(IList<TelephoneDetail> items, out IList<TelephoneDetail> deletedItems, out string failureMessage)
		{
			failureMessage = null;
			deletedItems = new List<TelephoneDetail>();

			foreach (TelephoneDetail item in items)
			{
				deletedItems.Add(item);
				_phoneNumberList.Remove(item);
			}

			return deletedItems.Count > 0;
		}

		/// <summary>
		/// Compares two items to see if they represent the same item.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		protected override bool IsSameItem(TelephoneDetail x, TelephoneDetail y)
		{
			return Equals(x, y);
		}

		#endregion
	}
}
