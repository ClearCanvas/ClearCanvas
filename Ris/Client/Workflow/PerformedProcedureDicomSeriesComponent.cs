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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Desktop;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	/// <summary>
	/// Extension point for views onto <see cref="PerformedProcedureDicomSeriesComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class PerformedProcedureDicomSeriesComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

    public class DicomSeriesTable : Table<DicomSeriesDetail>
    {
        public DicomSeriesTable()
        {
			this.Columns.Add(new TableColumn<DicomSeriesDetail, string>(SR.ColumnSeriesNumber,
                delegate(DicomSeriesDetail detail) { return detail.SeriesNumber; }, 
                0.5f));

            this.Columns.Add(new TableColumn<DicomSeriesDetail, string>(SR.ColumnDescription,
                delegate(DicomSeriesDetail detail) { return detail.SeriesDescription; }, 
                1.0f));

            this.Columns.Add(new TableColumn<DicomSeriesDetail, int>(SR.ColumnNumberOfImages,
                delegate(DicomSeriesDetail detail) { return detail.NumberOfSeriesRelatedInstances; }, 
                0.5f));

			this.Columns.Add(new TableColumn<DicomSeriesDetail, string>(SR.ColumnStudyInstanceUid,
				delegate(DicomSeriesDetail detail) { return detail.StudyInstanceUID; },
				1.0f));

			this.Columns.Add(new TableColumn<DicomSeriesDetail, string>(SR.ColumnSeriesInstanceUid,
				delegate(DicomSeriesDetail detail) { return detail.SeriesInstanceUID; },
				1.0f));
		}
    }

	/// <summary>
	/// PerformedProcedureDicomSeriesComponent class.
	/// </summary>
	public class PerformedProcedureDicomSeriesComponent : SummaryComponentBase<DicomSeriesDetail, DicomSeriesTable>, IPerformedStepEditorPage
	{
		private readonly IPerformedStepEditorContext _context;

		public PerformedProcedureDicomSeriesComponent(IPerformedStepEditorContext context)
		{
			_context = context;
			_context.SelectedPerformedStepChanged += OnSelectedPerformedStepChanged;
		}

		private void OnSelectedPerformedStepChanged(object sender, System.EventArgs e)
		{
			this.Table.Items.Clear();
			this.Table.Items.AddRange(_context.SelectedPerformedStep.DicomSeries);
		}

		#region SummaryComponentBase Overrides

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
		protected override IList<DicomSeriesDetail> ListItems(int firstItem, int maxItems)
		{
			// return empty list, the OnSelectedPerformedStepChanged takes care of populating table
			return new List<DicomSeriesDetail>();
		}

		/// <summary>
		/// Called to handle the "add" action.
		/// </summary>
		/// <param name="addedItems"></param>
		/// <returns>True if items were added, false otherwise.</returns>
		protected override bool AddItems(out IList<DicomSeriesDetail> addedItems)
		{
			addedItems = new List<DicomSeriesDetail>();

			DicomSeriesDetail detail = new DicomSeriesDetail();

			// Keep looping until user enters an unique Dicom Series Number, or cancel the add operation
			DicomSeriesEditorComponent editor = new DicomSeriesEditorComponent(detail, true);
			ApplicationComponentExitCode exitCode = LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleAddDicomSeries);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				addedItems.Add(detail);
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
		protected override bool EditItems(IList<DicomSeriesDetail> items, out IList<DicomSeriesDetail> editedItems)
		{
			editedItems = new List<DicomSeriesDetail>();
			DicomSeriesDetail detail = CollectionUtils.FirstElement(items);
			DicomSeriesDetail clone = (DicomSeriesDetail) detail.Clone();

			// Keep looping until user enters an unique Dicom Series Number, or cancel the edit operation
			DicomSeriesEditorComponent editor = new DicomSeriesEditorComponent(clone, false);
			ApplicationComponentExitCode exitCode = LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleUpdateDicomSeries);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				editedItems.Add(clone);
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
		protected override bool DeleteItems(IList<DicomSeriesDetail> items, out IList<DicomSeriesDetail> deletedItems, out string failureMessage)
		{
			failureMessage = null;
			deletedItems = new List<DicomSeriesDetail>();

			foreach (DicomSeriesDetail item in items)
			{
				deletedItems.Add(item);
			}

			return deletedItems.Count > 0;
		}

		/// <summary>
		/// Compares two items to see if they represent the same item.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		protected override bool IsSameItem(DicomSeriesDetail x, DicomSeriesDetail y)
		{
			if (ReferenceEquals(x, y))
				return true;

			if (x.DicomSeriesRef != null && y.DicomSeriesRef != null)
				return x.DicomSeriesRef.Equals(y.DicomSeriesRef, true);

			//TODO: this won't work very well, because nothing prevents user from modifying series number
			//should fix in future when we have a better understanding of how this component will work.
			if (x.SeriesNumber == y.SeriesNumber)
				return true;

			return false;
		}

		#endregion

		#region IPerformedStepEditorPage Members

		public void Save()
		{
			if(_context.SelectedPerformedStep != null)
			{
				_context.SelectedPerformedStep.DicomSeries = new List<DicomSeriesDetail>(this.Table.Items);
			}
		}

		public Path Path
		{
			get { return new Path("TitleDicomSeries", new ResourceResolver(this.GetType().Assembly)); }
		}

		public IApplicationComponent GetComponent()
		{
			return this;
		}

		#endregion
	}
}
