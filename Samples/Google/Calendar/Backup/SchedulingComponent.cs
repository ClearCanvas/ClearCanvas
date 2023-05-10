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
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Samples.Google.Calendar
{
    /// <summary>
    /// Defines the tool context interface for tools that extend <see cref="SchedulingToolExtensionPoint"/>.
    /// </summary>
    public interface ISchedulingToolContext : IToolContext
    {
        /// <summary>
        /// Gets the desktop window in which the scheduling component is running.
        /// </summary>
        DesktopWindow DesktopWindow { get; }

        /// <summary>
        /// Gets the currently selected appointment in the appointment list, or null if none is selected.
        /// </summary>
        CalendarEvent SelectedAppointment { get; }

        /// <summary>
        /// Occurs when the selected appointment changes.
        /// </summary>
        event EventHandler SelectedAppointmentChanged;

        /// <summary>
        /// Gets a list of all appointments currently displayed in the appointment list.
        /// </summary>
        IList<CalendarEvent> AllAppointments { get; }
    }

    /// <summary>
    /// Extension point for tools that extend the functionality of the <see cref="SchedulingComponent"/>.
    /// </summary>
    [ExtensionPoint]
    public class SchedulingToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    /// <summary>
    /// Extension point for views onto <see cref="SchedulingComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class SchedulingComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// SchedulingComponent class
    /// </summary>
    [AssociateView(typeof(SchedulingComponentViewExtensionPoint))]
    public class SchedulingComponent : ApplicationComponent
    {

        #region Implementation of ISchedulingToolContext

        /// <summary>
        /// Inner class that provides the implementation of <see cref="ISchedulingToolContext"/>.
        /// </summary>
        public class SchedulingToolContext : ToolContext, ISchedulingToolContext
        {
            private SchedulingComponent _owner;

            internal SchedulingToolContext(SchedulingComponent owner)
            {
                _owner = owner;
            }

            #region ISchedulingToolContext Members

            public DesktopWindow DesktopWindow
            {
                get { return _owner.Host.DesktopWindow; }
            }

            public CalendarEvent SelectedAppointment
            {
                get { return _owner._selectedAppointment; }
            }

            public event EventHandler SelectedAppointmentChanged
            {
                add { _owner._selectedAppointmentChanged += value;  }
                remove { _owner._selectedAppointmentChanged -= value; }
            }

            public IList<CalendarEvent> AllAppointments
            {
                get { return _owner._appointments.Items; }
            }

            #endregion
        }

        #endregion

        private string _patientInfo;
        private string _comment;
        private DateTime _appointmentDate;
        private Table<CalendarEvent> _appointments;
        private CalendarEvent _selectedAppointment;
        private event EventHandler _selectedAppointmentChanged;


        private Calendar _calendar;

        private ToolSet _extensionTools;
        private ActionModelRoot _menuModel;
        private ActionModelRoot _toolbarModel;


        /// <summary>
        /// Constructor
        /// </summary>
        public SchedulingComponent()
        {
        }

        /// <summary>
        /// Initialize the component.
        /// </summary>
        public override void Start()
        {
            _calendar = new Calendar();

            _appointmentDate = Platform.Time;   // init to current time

            // define the structure of the appointments table
            _appointments = new Table<CalendarEvent>();
            _appointments.Columns.Add(new TableColumn<CalendarEvent, string>(SR.AppointmentTableDate,
                delegate(CalendarEvent e) { return Format.Date(e.StartTime); }));
            _appointments.Columns.Add(new TableColumn<CalendarEvent, string>(SR.AppointmentTableComment,
                delegate(CalendarEvent e) { return e.Description; }));

            // create extension tools and action models
            _extensionTools = new ToolSet(new SchedulingToolExtensionPoint(), new SchedulingToolContext(this));
            _menuModel = ActionModelRoot.CreateModel(this.GetType().FullName, "scheduling-appointments-contextmenu", _extensionTools.Actions);
            _toolbarModel = ActionModelRoot.CreateModel(this.GetType().FullName, "scheduling-appointments-toolbar", _extensionTools.Actions);

            // initialize patient info from active workspace
            UpdatePatientInfo(this.Host.DesktopWindow.ActiveWorkspace);

            // subscribe to desktop window for changes in active workspace
            this.Host.DesktopWindow.Workspaces.ItemActivationChanged += Workspaces_ItemActivationChanged;


            base.Start();
        }

        /// <summary>
        /// Clean up the component.
        /// </summary>
        public override void Stop()
        {
            // important to dispose of extension tools
            _extensionTools.Dispose();

            // important to unsubscribe from this event, or the DesktopWindow will continue to hold a reference to this component
            this.Host.DesktopWindow.Workspaces.ItemActivationChanged -= Workspaces_ItemActivationChanged;

            base.Stop();
        }

        /// <summary>
        /// Keep the component synchronized with the active workspace.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Workspaces_ItemActivationChanged(object sender, ItemEventArgs<Workspace> e)
        {
            Workspace workspace = e.Item;

            if (workspace.Active)
            {
                UpdatePatientInfo(workspace);
            }
            else
            {
                Reset();
            }
        }

        /// <summary>
        /// Helper method to update the component from the active workspace.
        /// </summary>
        /// <param name="workspace"></param>
        private void UpdatePatientInfo(Workspace workspace)
        {
            IImageViewer viewer = ImageViewerComponent.GetAsImageViewer(workspace);
            if (viewer != null)
            {
                Patient patient = CollectionUtils.FirstElement<Patient>(viewer.StudyTree.Patients);
                this.PatientInfo = string.Format("{0} {1}", patient.PatientId, patient.PatientsName);

                _appointments.Items.Clear();
                _appointments.Items.AddRange(_calendar.GetEvents(_patientInfo, null, null));
            }
            else
            {
                this.PatientInfo = null;
            }
        }

        /// <summary>
        /// Helper method to clear all data in the component.
        /// </summary>
        private void Reset()
        {
            _appointments.Items.Clear();

            this.PatientInfo = null;
            this.Comment = null;
            this.AppointmentDate = Platform.Time;
            this.ShowValidation(false);
        }


        #region Presentation Model

        /// <summary>
        /// Gets the model for the appointments table context-menu.
        /// </summary>
        public ActionModelNode MenuModel
        {
            get { return _menuModel; }
        }

        /// <summary>
        /// Gets the model for the appointments table toolbar.
        /// </summary>
        public ActionModelNode ToolbarModel
        {
            get { return _toolbarModel; }
        }

        /// <summary>
        /// Gets the appointments table.
        /// </summary>
        public ITable Appointments
        {
            get { return _appointments; }
        }

        /// <summary>
        /// Gets or sets the current selection in the appointments table.
        /// </summary>
        public ISelection SelectedAppointment
        {
            get { return new Selection(_selectedAppointment); }
            set
            {
                if (value.Item != _selectedAppointment)
                {
                    _selectedAppointment = (CalendarEvent)value.Item;
                    NotifyPropertyChanged("SelectedAppointment");

                    // also fire the private event, used by the tool context
                    EventsHelper.Fire(_selectedAppointmentChanged, this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets the patient information field.
        /// </summary>
        [ValidateNotNull]
        public string PatientInfo
        {
            get { return _patientInfo; }
            private set
            {
                if (value != _patientInfo)
                {
                    _patientInfo = value;
                    NotifyPropertyChanged("PatientInfo");
                }
            }
        }

        /// <summary>
        /// Gets or sets the comment for a follow-up appointment.
        /// </summary>
        [ValidateNotNull]
        public string Comment
        {
            get { return _comment; }
            set
            {
                if (value != _comment)
                {
                    _comment = value;
                    this.NotifyPropertyChanged("Comment");
                }
            }
        }

        /// <summary>
        /// Gets or sets the appointment date for a follow-up appointment.
        /// </summary>
        [ValidateGreaterThan("CurrentTime", Message="AppointmentMustBeInFuture")]
        public DateTime AppointmentDate
        {
            get { return _appointmentDate; }
            set
            {
                if (value != _appointmentDate)
                {
                    _appointmentDate = value;
                    this.NotifyPropertyChanged("AppointmentDate");
                }
            }
        }

        /// <summary>
        /// Gets the current time.  This is a reference property used to validate the <see cref="AppointmentDate"/> property.
        /// </summary>
        public DateTime CurrentTime
        {
            get { return DateTime.Now; }
        }

        /// <summary>
        /// Adds a follow-up appointment based on the d
        /// </summary>
        public void AddAppointment()
        {
            // check for validation errors
            if (this.HasValidationErrors)
            {
                // ensure validation errors are visible to the user, and bail
                this.ShowValidation(true);
                return;
            }

            // add the appointment to the calendar
            CalendarEvent e = _calendar.AddEvent(_patientInfo, _comment, _appointmentDate, _appointmentDate);

            // add the new appointment to the appointments list
            _appointments.Items.Add(e);
        }

        #endregion

    }
}
