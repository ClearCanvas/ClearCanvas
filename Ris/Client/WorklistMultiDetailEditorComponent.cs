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
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;
using ClearCanvas.Common.Utilities;
using System.Collections;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="WorklistMultiDetailEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class WorklistMultiDetailEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// WorklistMultiDetailEditorComponent class
    /// </summary>
    [AssociateView(typeof(WorklistMultiDetailEditorComponentViewExtensionPoint))]
    public class WorklistMultiDetailEditorComponent : WorklistDetailEditorComponentBase
	{

		#region Worklist TableEntry class

		public class WorklistTableEntry
        {
            private bool _checked;
            private readonly WorklistClassSummary _worklistClass;
            private string _name;
            private string _description;

            public WorklistTableEntry(WorklistClassSummary worklistClass, string name)
            {
                _worklistClass = worklistClass;
                _name = name;
                _checked = true;
                _description = MakeDefaultDescription(worklistClass, name);
            }

            public bool Checked
            {
                get { return _checked; }
                set { _checked = value; }
            }

            public WorklistClassSummary Class
            {
                get { return _worklistClass; }
            }

            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }

            public string Description
            {
                get { return _description; }
                set { _description = value; }
            }
		}

		#endregion

		private Table<WorklistTableEntry> _worklistTable;
        private WorklistTableEntry _selectedWorklist;
        private string _defaultWorklistName;
    	private List<StaffGroupSummary> _ownerGroupChoices;

    	private CrudActionModel _worklistActionModel;

        /// <summary>
        /// Constructor
        /// </summary>
        public WorklistMultiDetailEditorComponent(List<WorklistClassSummary> worklistClasses, List<StaffGroupSummary> ownerGroupChoices)
			: base(worklistClasses, GetDefaultWorklistClass(worklistClasses))
        {
        	_ownerGroupChoices = ownerGroupChoices;
        }

        public override void Start()
        {
            _worklistTable = new Table<WorklistTableEntry>();
            _worklistTable.Columns.Add(new TableColumn<WorklistTableEntry, bool>(SR.ColumnCreate,
                delegate(WorklistTableEntry item) { return item.Checked; },
                delegate(WorklistTableEntry item, bool value) { item.Checked = value; }, 0.5f));
            _worklistTable.Columns.Add(new TableColumn<WorklistTableEntry, string>(SR.ColumnClass,
                delegate(WorklistTableEntry item) { return string.Format("{0} - {1}", item.Class.DisplayName, item.Class.Description); }, 1.0f));
            _worklistTable.Columns.Add(new TableColumn<WorklistTableEntry, string>(SR.ColumnWorklistName,
                delegate(WorklistTableEntry item) { return item.Name; },
                delegate(WorklistTableEntry item, string value) { item.Name = value; }, 1.0f));
            _worklistTable.Columns.Add(new TableColumn<WorklistTableEntry, string>(SR.ColumnWorklistDescription,
                delegate(WorklistTableEntry item) { return item.Description; },
                delegate(WorklistTableEntry item, string value) { item.Description = value; }, 1.0f));

            _worklistActionModel = new CrudActionModel(false, true, false);
            _worklistActionModel.Edit.SetClickHandler(EditSelectedWorklist);

            UpdateWorklistActionModel();

            // add validation rule to ensure all worklists that will be created have a name
            this.Validation.Add(new ValidationRule("SelectedWorklist",
                delegate 
                {
                    bool allWorklistsHaveNames = CollectionUtils.TrueForAll(_worklistTable.Items,
                        delegate(WorklistTableEntry item) { return !item.Checked || !string.IsNullOrEmpty(item.Name); });

                    return new ValidationResult(allWorklistsHaveNames, SR.MessageWorklistMustHaveName);
                }));

            base.Start();
        }

    	public List<WorklistTableEntry> WorklistsToCreate
        {
            get
            {
                return CollectionUtils.Select(_worklistTable.Items,
                    delegate(WorklistTableEntry item) { return item.Checked; });
            }
        }

        #region Presentation Model

    	public string DefaultWorklistName
        {
            get { return _defaultWorklistName; }
            set
            {
                if(value != _defaultWorklistName)
                {
                    UpdateWorklistNamesAndDescriptions(_defaultWorklistName, value);
                    _defaultWorklistName = value;
                    this.Modified = true;
                    NotifyPropertyChanged("DefaultWorklistName");
                }
            }
        }

        public ActionModelNode WorklistActionModel
        {
            get { return _worklistActionModel; }
        }

        public ITable WorklistTable
        {
            get { return _worklistTable; }
        }

        public ISelection SelectedWorklist
        {
            get { return new Selection(_selectedWorklist); }
            set
            {
                WorklistTableEntry item = (WorklistTableEntry) value.Item;
                if(!Equals(item, _selectedWorklist))
                {
                    _selectedWorklist = item;
                    UpdateWorklistActionModel();
                    NotifyPropertyChanged("SelectedWorklist");
                }
            }
        }

        public void EditSelectedWorklist()
        {
            if(_selectedWorklist != null)
            {
                WorklistAdminDetail detail = new WorklistAdminDetail();
                detail.Name = _selectedWorklist.Name;
                detail.Description = _selectedWorklist.Description;
                detail.WorklistClass = _selectedWorklist.Class;

            	WorklistDetailEditorComponent editor =
            		new WorklistDetailEditorComponent(
						detail,
						this.WorklistClasses,
						_ownerGroupChoices,
                        WorklistEditorMode.Edit,
                        true,
                        true);

                if(ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow,
					editor,
                    SR.TitleEditWorklist) == ApplicationComponentExitCode.Accepted)
                {
                    _selectedWorklist.Name = detail.Name;
                    _selectedWorklist.Description = detail.Description;
                    _worklistTable.Items.NotifyItemUpdated(_selectedWorklist);
                }
            }
        }

        #endregion

		protected override void UpdateWorklistClassChoices()
		{
			_worklistTable.Items.Clear();
			_worklistTable.Items.AddRange(
				CollectionUtils.Map<WorklistClassSummary, WorklistTableEntry>(this.WorklistClassChoices,
					delegate(WorklistClassSummary wc) { return new WorklistTableEntry(wc, _defaultWorklistName); }));

			base.UpdateWorklistClassChoices();
		}

        private void UpdateWorklistNamesAndDescriptions(string oldName, string newName)
        {
            // only update the names and descriptions that the user has not explicitly modified
            foreach (WorklistTableEntry item in _worklistTable.Items)
            {
                if(item.Name == oldName)
                {
                    item.Name = newName;
                }
                if(item.Description == MakeDefaultDescription(item.Class, oldName))
                {
                    item.Description = MakeDefaultDescription(item.Class, newName);
                }
                _worklistTable.Items.NotifyItemUpdated(item);
            }
        }

        private static string MakeDefaultDescription(WorklistClassSummary worklistClass, string name)
        {
            return string.Format("{0} {1}", name, worklistClass.DisplayName);
        }

        private void UpdateWorklistActionModel()
        {
            _worklistActionModel.Edit.Enabled = _selectedWorklist != null;
        }

		private static WorklistClassSummary GetDefaultWorklistClass(List<WorklistClassSummary> worklistClasses)
		{
			// return the first class that matches the saved default category
			return CollectionUtils.SelectFirst(worklistClasses,
				delegate(WorklistClassSummary w) { return w.CategoryName == WorklistEditorComponentSettings.Default.DefaultWorklistCategory; });
		}
    }
}
