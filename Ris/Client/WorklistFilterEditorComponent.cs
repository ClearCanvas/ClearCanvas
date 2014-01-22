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
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="WorklistFilterEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class WorklistFilterEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// WorklistFilterEditorComponent class
    /// </summary>
    [AssociateView(typeof(WorklistFilterEditorComponentViewExtensionPoint))]
    public class WorklistFilterEditorComponent : ApplicationComponent
    {
        class DummyItem
        {
            private readonly string _displayString;

            public DummyItem(string displayString)
            {
                _displayString = displayString;
            }

            public override string ToString()
            {
                return _displayString;
            }
        }

        private static readonly object _nullFilterItem = new DummyItem(SR.DummyItemNone);
        private static readonly object _workingFacilityItem = new DummyItem(SR.DummyItemWorkingFacility);
        private static readonly object _portableItem = new DummyItem(SR.DummyItemPortable);
        private static readonly object _nonPortableItem = new DummyItem(SR.DummyItemNonPortable);

        private static readonly object[] _portableChoices = new object[] { _portableItem, _nonPortableItem };

        private readonly ArrayList _facilityChoices;
        private ArrayList _selectedFacilities;

        private readonly List<EnumValueInfo> _priorityChoices;
        private readonly List<EnumValueInfo> _patientClassChoices;
        private ArrayList _selectedPortabilities;

    	private ExternalPractitionerLookupHandler _orderingPractitionerLookupHandler;
    	private ExternalPractitionerSummary _selectedOrderingPractitioner;

        private readonly WorklistAdminDetail _worklistDetail;

        /// <summary>
        /// Constructor
        /// </summary>
        public WorklistFilterEditorComponent(WorklistAdminDetail detail,
            List<FacilitySummary> facilityChoices,
            List<EnumValueInfo> priorityChoices,
            List<EnumValueInfo> patientClassChoices)
        {
            _worklistDetail = detail;

            _facilityChoices = new ArrayList();
            _facilityChoices.Add(_workingFacilityItem);
            _facilityChoices.AddRange(facilityChoices);
            _selectedFacilities = new ArrayList();
            _selectedPortabilities = new ArrayList();

            _priorityChoices = priorityChoices;
            _patientClassChoices = patientClassChoices;

        }

        public override void Start()
        {

            if (_worklistDetail.FilterByWorkingFacility)
                _selectedFacilities.Add(_workingFacilityItem);
            _selectedFacilities.AddRange(_worklistDetail.Facilities);

            if (_worklistDetail.Portabilities.Contains(true))
                _selectedPortabilities.Add(_portableItem);
            if (_worklistDetail.Portabilities.Contains(false))
                _selectedPortabilities.Add(_nonPortableItem);

			_orderingPractitionerLookupHandler = new ExternalPractitionerLookupHandler(this.Host.DesktopWindow);
			if(_worklistDetail.OrderingPractitioners != null)
			{
				// GUI only allows 1 ordering practitioner - could change this in future if needed
				_selectedOrderingPractitioner = CollectionUtils.FirstElement(_worklistDetail.OrderingPractitioners);
			}

            base.Start();
        }

        #region Presentation Model

        public string ProcedureTypeGroupClassName
        {
            get { return _worklistDetail.WorklistClass.ProcedureTypeGroupClassDisplayName; }
        }

        public object NullFilterItem
        {
            get { return _nullFilterItem; }
        }

        public IList FacilityChoices
        {
            get { return _facilityChoices; }
        }

        public string FormatFacility(object item)
        {
            if (item is FacilitySummary)
            {
                FacilitySummary facility = (FacilitySummary)item;
                return facility.Name;
            }
            else
                return item.ToString(); // place-holder items
        }

        public IList SelectedFacilities
        {
            get { return _selectedFacilities; }
            set
            {
                if (!CollectionUtils.Equal(value, _selectedFacilities, false))
                {
                    _selectedFacilities = new ArrayList(value);
                    this.Modified = true;
                }
            }
        }

        public IList PriorityChoices
        {
            get { return _priorityChoices; }
        }

        public IList SelectedPriorities
        {
            get { return _worklistDetail.OrderPriorities; }
            set
            {
                IList<EnumValueInfo> list = new TypeSafeListWrapper<EnumValueInfo>(value);
                if (!CollectionUtils.Equal(list, _worklistDetail.OrderPriorities, false))
                {
                    _worklistDetail.OrderPriorities = new List<EnumValueInfo>(list);
                    this.Modified = true;
                }
            }
        }

    	public bool PatientClassVisible
    	{
			get { return new WorkflowConfigurationReader().EnableVisitWorkflow; }
    	}

        public IList PatientClassChoices
        {
            get { return _patientClassChoices; }
        }

        public IList SelectedPatientClasses
        {
            get { return _worklistDetail.PatientClasses; }
            set
            {
                IList<EnumValueInfo> list = new TypeSafeListWrapper<EnumValueInfo>(value);
                if (!CollectionUtils.Equal(list, _worklistDetail.PatientClasses, false))
                {
                    _worklistDetail.PatientClasses = new List<EnumValueInfo>(list);
                    this.Modified = true;
                }
            }
        }

        public IList PortableChoices
        {
            get { return _portableChoices; }
        }

        public IList SelectedPortabilities
        {
            get { return _selectedPortabilities; }
            set
            {
                if (!CollectionUtils.Equal(value, _selectedPortabilities, false))
                {
                    _selectedPortabilities = new ArrayList(value);
                    this.Modified = true;
                }
            }
        }

    	public ILookupHandler OrderingPractitionerLookupHandler
    	{
			get { return _orderingPractitionerLookupHandler; }
    	}

    	public ExternalPractitionerSummary SelectedOrderingPractitioner
    	{
			get { return _selectedOrderingPractitioner; }
			set
			{
				if(!Equals(value, _selectedOrderingPractitioner))
				{
					_selectedOrderingPractitioner = value;
					this.Modified = true;
					NotifyPropertyChanged("SelectedOrderingPractitioner");
				}
			}
    	}

        public void ItemsAddedOrRemoved()
        {
            this.Modified = true;
        }

        #endregion


        internal void SaveData()
        {
            _worklistDetail.Facilities = new List<FacilitySummary>();
            _worklistDetail.Facilities.AddRange(
                new TypeSafeListWrapper<FacilitySummary>(
                    CollectionUtils.Select(_selectedFacilities, delegate(object f) { return f is FacilitySummary; })));
            _worklistDetail.FilterByWorkingFacility =
                CollectionUtils.Contains(_selectedFacilities, delegate(object f) { return f == _workingFacilityItem; });

            _worklistDetail.Portabilities = CollectionUtils.Map<object, bool>(_selectedPortabilities,
                delegate(object item) { return item == _portableItem ? true : false; });

			// GUI only allows 1 ordering practitioner - could change this in future if needed
			_worklistDetail.OrderingPractitioners = new List<ExternalPractitionerSummary>();
			if(_selectedOrderingPractitioner != null)
			{
				_worklistDetail.OrderingPractitioners.Add(_selectedOrderingPractitioner);
			}
        }
    }
}
