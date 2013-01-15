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
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="ExternalPractitionerReplaceDisabledContactPointsComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class ExternalPractitionerReplaceDisabledContactPointsComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// ExternalPractitionerReplaceDisabledContactPointsComponent class.
	/// </summary>
	[AssociateView(typeof(ExternalPractitionerReplaceDisabledContactPointsComponentViewExtensionPoint))]
	public class ExternalPractitionerReplaceDisabledContactPointsComponent : ApplicationComponent
	{
		private List<ExternalPractitionerContactPointDetail> _deactivatedContactPoints;
		private List<ExternalPractitionerContactPointDetail> _activeContactPoints;
		private readonly Dictionary<EntityRef, EntityRef> _contactPointReplacements;
		private readonly List<ExternalPractitionerReplaceDisabledContactPointsTableItem> _tableItems;

		/// <summary>
		/// Constructor.
		/// </summary>
		public ExternalPractitionerReplaceDisabledContactPointsComponent()
		{
			_tableItems = new List<ExternalPractitionerReplaceDisabledContactPointsTableItem>();
			_contactPointReplacements = new Dictionary<EntityRef, EntityRef>();
		}

		// Dummy property for binding the validation icon to.
		public ExternalPractitionerReplaceDisabledContactPointsTableItem ValidationPlaceHolder { get; set; }

		public List<ExternalPractitionerContactPointDetail> DeactivatedContactPoints
		{
			get
			{
				return _deactivatedContactPoints;
			}
			set
			{
				if (CollectionUtils.Equal<ExternalPractitionerContactPointDetail>(_deactivatedContactPoints, value, false))
					return;

				_deactivatedContactPoints = value;
				UpdateTableItems();
			}
		}

		public List<ExternalPractitionerContactPointDetail> ActiveContactPoints
		{
			get { return _activeContactPoints; }
			set { _activeContactPoints = value; }
		}

		public Dictionary<EntityRef, EntityRef> ContactPointReplacements
		{
			get
			{
				UpdateContactPointReplacementMap();
				return _contactPointReplacements;
			}
		}

		public string Instruction
		{
			get { return SR.MessageInstructionReplacementContactPoints; }
		}

		public List<ExternalPractitionerReplaceDisabledContactPointsTableItem> ReplaceDisabledContactPointsTableItems
		{
			get { return _tableItems; }
		}

		public bool HasUnspecifiedContactPoints
		{
			get { return CollectionUtils.Contains(_tableItems, item => item.SelectedNewContactPoint == null); }
		}

		/// <summary>
		/// Called by the host to initialize the application component.
		/// </summary>
		public override void Start()
		{
			this.Validation.Add(new ValidationRule("ValidationPlaceHolder",
				component => new ValidationResult(!this.HasUnspecifiedContactPoints, SR.MessageValidationMustSpecifyActiveContactPoint)));

			base.Start();
		}

		public void Save()
		{
			UpdateContactPointReplacementMap();
		}

		private void UpdateContactPointReplacementMap()
		{
			_contactPointReplacements.Clear();
			foreach (var item in _tableItems)
			{
				if (item.SelectedNewContactPoint == null)
					continue;

				_contactPointReplacements.Add(
					item.OldContactPoint.ContactPointRef,
					item.SelectedNewContactPoint.ContactPointRef);
			}
		}

		private void UpdateTableItems()
		{
			_tableItems.Clear();
			foreach (var deactivatedContactPoint in this.DeactivatedContactPoints)
			{
				var tableItem = new ExternalPractitionerReplaceDisabledContactPointsTableItem(deactivatedContactPoint, this.ActiveContactPoints);
				tableItem.SelectedNewContactPointModified += delegate { this.ShowValidation(this.HasValidationErrors); };
				_tableItems.Add(tableItem);
			}

			NotifyAllPropertiesChanged();
		}
	}
}
