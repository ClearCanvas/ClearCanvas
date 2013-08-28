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

using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="MergeComponentBase"/>
	/// </summary>
	[ExtensionPoint]
	public class MergeComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// Abstract base class for merge components.
	/// </summary>
	[AssociateView(typeof(MergeComponentViewExtensionPoint))]
	public abstract class MergeComponentBase : ApplicationComponent
	{
		#region Presentation Model

		/// <summary>
		/// Gets a list of source items
		/// </summary>
		public abstract IList SourceItems { get; }

		/// <summary>
		/// Gets a list of target items
		/// </summary>
		public abstract IList TargetItems { get; }

		/// <summary>
		/// Gets and sets thecurrently selected duplicate item.
		/// </summary>
		public abstract object SelectedDuplicate { get; set;}

		/// <summary>
		/// Gets and sets the currently selected original item.
		/// </summary>
		public abstract object SelectedOriginal { get; set;}

		/// <summary>
		/// Handles the accept button.
		/// </summary>
		public abstract void Accept();

		/// <summary>
		/// Handles the cancel button.
		/// </summary>
		public abstract void Cancel();

		/// <summary>
		/// Switch duplicate and original.
		/// </summary>
		public abstract void Switch();

		#endregion

		/// <summary>
		/// Formats an item in either the <see mref="SourceItems"/> collection or <see mref="TargetItems"/> collection
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public abstract object FormatItem(object p);
	}

	public abstract class MergeComponentBase<TSummary> : MergeComponentBase
		where TSummary : DataContractBase
	{
		private TSummary _selectedDuplicate;
		private TSummary _selectedOriginal;
		private readonly IList<TSummary> _items;

		protected MergeComponentBase(IList<TSummary> items)
		{
			_items = items;

			this.Validation.Add(new ValidationRule("SelectedOriginal",
				delegate
				{
					var isIdentical = IsSameItem(_selectedDuplicate, _selectedOriginal);
					return new ValidationResult(!isIdentical, SR.MessageMergeIdenticalItems);
				}));
		}

		public override IList SourceItems
		{
			get { return new List<TSummary>(_items); }
		}

		public override IList TargetItems
		{
			get { return new List<TSummary>(_items); }
		}

		#region Abstract/overridable members

		/// <summary>
		/// Compares two items to see if they represent the same item.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		protected abstract bool IsSameItem(TSummary x, TSummary y);

		#endregion

		#region Presentation Model

		public TSummary SelectedDuplicateSummary
		{
			get { return _selectedDuplicate; }
		}

		[ValidateNotNull]
		public override object SelectedDuplicate
		{
			get { return _selectedDuplicate; }
			set
			{
				if (_selectedDuplicate != value)
				{
					_selectedDuplicate = (TSummary) value;
					NotifyPropertyChanged("SelectedDuplicate");
				}
			}
		}

		public TSummary SelectedOriginalSummary
		{
			get { return _selectedOriginal; }
		}

		[ValidateNotNull]
		public override object SelectedOriginal
		{
			get { return _selectedOriginal; }
			set
			{
				if (_selectedOriginal != value)
				{
					_selectedOriginal = (TSummary) value;
					NotifyPropertyChanged("SelectedOriginal");
				}
			}
		}

		public override void Accept()
		{
			this.ExitCode = ApplicationComponentExitCode.Accepted;
			this.Host.Exit();
		}

		public override void Cancel()
		{
			this.ExitCode = ApplicationComponentExitCode.None;
			this.Host.Exit();
		}

		public override void Switch()
		{
			var temp = this.SelectedOriginal;
			this.SelectedOriginal = this.SelectedDuplicate;
			this.SelectedDuplicate = temp;
		}

		#endregion
	}
}
