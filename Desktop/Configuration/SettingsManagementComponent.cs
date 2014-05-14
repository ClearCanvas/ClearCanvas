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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.Configuration
{

	/// <summary>
	/// Extension point for views onto <see cref="SettingsManagementComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class SettingsManagementComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// The <see cref="SettingsManagementComponent"/> allows editing of
	/// the application and default user profile settings through a generic UI.
	/// </summary>
	[AssociateView(typeof(SettingsManagementComponentViewExtensionPoint))]
	public partial class SettingsManagementComponent : ApplicationComponent
	{
		#region Group class

		/// <summary>
		/// Base class for entries in the settings group table.
		/// </summary>
		internal abstract class Group
		{
			private List<Property> _properties;

			protected Group(SettingsGroupDescriptor descriptor)
			{
				Descriptor = descriptor;
			}

			public SettingsGroupDescriptor Descriptor { get; private set; }

			public string Name
			{
				get { return Descriptor.Name; }
			}

			public Version Version
			{
				get { return Descriptor.Version; }
			}

			public string Description
			{
				get { return Descriptor.Description; }
			}

			public IList<Property> Properties
			{
				get { return _properties ?? (_properties = LoadProperties().ToList()); }
			}

			public void Save()
			{
				if (_properties == null)
					return;

				SaveProperties(_properties);

				// mark all properties as clean again
				foreach (var p in _properties)
					p.MarkClean();
			}

			protected abstract IEnumerable<Property> LoadProperties();

			protected abstract void SaveProperties(IList<Property> properties);
		}

		#endregion

		#region SettingsStoreGroup class

		/// <summary>
		/// Represents a settings group that was retrieved from a settings store and 
		/// may not exist in the locally installed plugins.
		/// </summary>
		internal class SettingsStoreGroup : Group
		{
			private readonly ISettingsStore _store;

			internal SettingsStoreGroup(SettingsGroupDescriptor descriptor, ISettingsStore store)
				: base(descriptor)
			{
				_store = store;
			}

			protected override IEnumerable<Property> LoadProperties()
			{
				var values = _store.GetSettingsValues(this.Descriptor, null, null);

				return from pi in _store.ListSettingsProperties(this.Descriptor)
					   select new Property(pi, values.ContainsKey(pi.Name) ? values[pi.Name] : pi.DefaultValue);
			}

			protected override void SaveProperties(IList<Property> properties)
			{
				// fill a dictionary with all dirty values
				var values = properties.Where(p => p.Dirty).ToDictionary(p => p.Name, p => p.Value);

				// save to the default profile
				_store.PutSettingsValues(this.Descriptor, null, null, values);
			}
		}

		#endregion

		#region Property class

		/// <summary>
		/// Defines a settings property for presentation in properties table.
		/// </summary>
		internal class Property
		{
			private readonly SettingsPropertyDescriptor _descriptor;
			private string _value;
			private string _startingValue;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="descriptor">The descriptor for the property.</param>
			/// <param name="value">The current value of the property.</param>
			public Property(SettingsPropertyDescriptor descriptor, string value)
			{
				_descriptor = descriptor;
				_startingValue = _value = value;
			}

			/// <summary>
			/// Gets the name of the settings property.
			/// </summary>
			public string Name
			{
				get { return _descriptor.Name; }
			}

			/// <summary>
			/// Gets the type name of the settings property.
			/// </summary>
			public string TypeName
			{
				get { return _descriptor.TypeName; }
			}

			/// <summary>
			/// Gets a description of the settings property.
			/// </summary>
			public string Description
			{
				get { return _descriptor.Description; }
			}

			/// <summary>
			/// Gets an enum describing the scope of the settings property.
			/// </summary>
			public SettingScope Scope
			{
				get { return _descriptor.Scope; }
			}

			/// <summary>
			/// Gets the default value of the settings property.
			/// </summary>
			public string DefaultValue
			{
				get { return _descriptor.DefaultValue; }
			}

			/// <summary>
			/// Gets/sets the current value of the settings property.
			/// </summary>
			public string Value
			{
				get { return _value; }
				set
				{
					if (value != _value)
					{
						_value = value;
						EventsHelper.Fire(ValueChanged, this, EventArgs.Empty);
					}
				}
			}

			/// <summary>
			/// Raised when <see cref="Value"/> has changed.
			/// </summary>
			public event EventHandler ValueChanged;

			/// <summary>
			/// Resets the value to the default value.
			/// </summary>
			public void ResetValue()
			{
				this.Value = this.DefaultValue;
			}

			/// <summary>
			/// Gets whether or not the default value matches the current value.
			/// </summary>
			public bool UsingDefaultValue
			{
				get { return _value == _descriptor.DefaultValue; }
			}

			/// <summary>
			/// Gets whether or not the property setting value has been modified.
			/// </summary>
			public bool Dirty
			{
				get { return _value != _startingValue; }
			}

			/// <summary>
			/// Marks this property as being clean again.
			/// </summary>
			public void MarkClean()
			{
				_startingValue = _value;
			}
		}

		#endregion

		private readonly ISettingsStore _settingsStore;

		private readonly Table<Group> _settingsGroupTable;
		private Group _selectedSettingsGroup;

		private readonly Table<Property> _settingsPropertiesTable;
		private Property _selectedSettingsProperty;

		private readonly SimpleActionModel _settingsPropertiesActionModel;
		private readonly ClickAction _saveAllAction;
		private readonly ClickAction _resetAllAction;
		private readonly ClickAction _resetAction;
		private readonly ClickAction _editAction;

		private readonly SimpleActionModel _settingsGroupsActionModel;
		private readonly ClickAction _importAction;


		/// <summary>
		/// Constructor.
		/// </summary>
		public SettingsManagementComponent(ISettingsStore settingsStore)
		{
			_settingsStore = settingsStore;

			// define the structure of the settings group table
			ITableColumn groupNameColumn;
			_settingsGroupTable = new Table<Group>();
			_settingsGroupTable.Columns.Add(groupNameColumn = new TableColumn<Group, string>(SR.TitleGroup, t => t.Name, 0.4f));
			_settingsGroupTable.Columns.Add(new TableColumn<Group, string>(SR.TitleVersion, t => t.Version.ToString(), 0.1f));
			_settingsGroupTable.Columns.Add(new TableColumn<Group, string>(SR.TitleDescription, t => t.Description, 0.5f));
			_settingsGroupTable.Sort(new TableSortParams(groupNameColumn, true));

			// define the settings properties table
			ITableColumn propertyNameColumn;
			_settingsPropertiesTable = new Table<Property>();
			_settingsPropertiesTable.Columns.Add(propertyNameColumn = new TableColumn<Property, string>(SR.TitleProperty, p => p.Name));
			_settingsPropertiesTable.Columns.Add(new TableColumn<Property, string>(SR.TitleDescription, p => p.Description));
			_settingsPropertiesTable.Columns.Add(new TableColumn<Property, string>(SR.TitleScope, p => p.Scope.ToString()));
			_settingsPropertiesTable.Columns.Add(new TableColumn<Property, string>(SR.TitleType, p => p.TypeName));
			_settingsPropertiesTable.Columns.Add(new TableColumn<Property, string>(SR.TitleDefaultValue, p => p.DefaultValue));
			_settingsPropertiesTable.Columns.Add(new TableColumn<Property, string>(SR.TitleValue, p => p.Value, (p, text) => p.Value = text));
			_settingsPropertiesTable.Sort(new TableSortParams(propertyNameColumn, true));

			_settingsGroupsActionModel = new SimpleActionModel(new ApplicationThemeResourceResolver(this.GetType().Assembly));
			_importAction = _settingsGroupsActionModel.AddAction("import", SR.LabelImport, "ImportToolSmall.png",
				SR.TooltipImportSettingsMetaData,
				Import);
			_importAction.Visible = _settingsStore.SupportsImport;

			_settingsPropertiesActionModel = new SimpleActionModel(new ApplicationThemeResourceResolver(this.GetType().Assembly));
			_saveAllAction = _settingsPropertiesActionModel.AddAction("saveall", SR.LabelSaveAll, "SaveToolSmall.png", () => SaveModifiedSettings(false));
			_editAction = _settingsPropertiesActionModel.AddAction("edit", SR.LabelEdit, "EditToolSmall.png", () => EditProperty(_selectedSettingsProperty));
			_resetAction = _settingsPropertiesActionModel.AddAction("reset", SR.LabelReset, "ResetToolSmall.png", () => ResetPropertyValue(_selectedSettingsProperty));
			_resetAllAction = _settingsPropertiesActionModel.AddAction("resetall", SR.LabelResetAll, "ResetAllToolSmall.png", ResetAllPropertyValues);
		}

		/// <summary>
		/// Called by the host to initialize the application component.
		/// </summary>
		///  <remarks>
		/// Override this method to implement custom initialization logic.  Overrides must be sure to call the base implementation.
		/// </remarks>
		public override void Start()
		{
			try
			{
				FillSettingsGroupTable();
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}

			base.Start();
		}

		/// <summary>
		/// Determines whether the component can exit without any user interaction.
		/// </summary>
		/// <returns>True if no properties are dirty, otherwise false.</returns>
		public override bool CanExit()
		{
			// return false if anything modified
			return _selectedSettingsGroup == null || !_selectedSettingsGroup.Properties.Any(p => p.Dirty);
		}

		/// <summary>
		/// Saves the changes and returns true.
		/// </summary>
		/// <returns></returns>
		public override bool PrepareExit()
		{
			SaveModifiedSettings(true);
			return true;
		}

		#region Presentation Model

		/// <summary>
		/// Gets the action model for the settings groups.
		/// </summary>
		public ActionModelRoot SettingsGroupsActionModel
		{
			get { return _settingsGroupsActionModel; }
		}

		/// <summary>
		/// Gets the currently selected settings group table.
		/// </summary>
		public ITable SettingsGroupTable
		{
			get { return _settingsGroupTable; }
		}

		/// <summary>
		/// Gets the currently selected settings group (aka settings class or <see cref="SettingsGroupDescriptor"/>)
		/// as an <see cref="ISelection"/>.
		/// </summary>
		public ISelection SelectedSettingsGroup
		{
			get { return new Selection(_selectedSettingsGroup); }
			set
			{
				var group = (Group)value.Item;
				if (group != _selectedSettingsGroup)
				{
					// save any changes before changing _selectedSettingsGroup
					SaveModifiedSettings(true);

					SelectGroup(group);
					EventsHelper.Fire(SelectedSettingsGroupChanged, this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Raised when <see cref="SelectedSettingsGroup"/> has changed.
		/// </summary>
		public event EventHandler SelectedSettingsGroupChanged;

		/// <summary>
		/// Gets a table of settings properties (<see cref="Property"/>) for the
		/// currently selected settings group.
		/// </summary>
		public ITable SettingsPropertiesTable
		{
			get { return _settingsPropertiesTable; }
		}

		/// <summary>
		/// Gets the action model for the settings properties.
		/// </summary>
		public ActionModelRoot SettingsPropertiesActionModel
		{
			get { return _settingsPropertiesActionModel; }
		}

		/// <summary>
		/// Gets or sets the currently selected <see cref="Property"/> as an <see cref="ISelection"/>.
		/// </summary>
		public ISelection SelectedSettingsProperty
		{
			get { return new Selection(_selectedSettingsProperty); }
			set
			{
				var p = (Property)value.Item;
				if (p != _selectedSettingsProperty)
				{
					_selectedSettingsProperty = p;
					UpdateActionEnablement();
					EventsHelper.Fire(SelectedSettingsPropertyChanged, this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Raised when <see cref="SelectedSettingsProperty"/> has changed.
		/// </summary>
		public event EventHandler SelectedSettingsPropertyChanged;

		/// <summary>
		/// Executed when the <see cref="SelectedSettingsProperty"/> has been double-clicked in the view.
		/// </summary>
		public void SettingsPropertyDoubleClicked()
		{
			if (_selectedSettingsProperty != null)
			{
				EditProperty(_selectedSettingsProperty);
			}
		}

		#endregion

		private void FillSettingsGroupTable()
		{
			_settingsGroupTable.Items.Clear();
			foreach (var group in GetSettingsGroups())
			{
				_settingsGroupTable.Items.Add(group);
			}
			_settingsGroupTable.Sort();
		}

		private void SelectGroup(Group group)
		{
			// unsubscribe from pervious Property objects
			if (_selectedSettingsGroup != null)
			{
				foreach (var property in _selectedSettingsGroup.Properties)
				{
					property.ValueChanged -= SettingsPropertyValueChangedEventHandler;
				}
			}

			_settingsPropertiesTable.Items.Clear();

			_selectedSettingsGroup = group;

			if (_selectedSettingsGroup == null)
				return;

			try
			{
				// the call to _selectedSettingsGroup.Properties can throw
				foreach (var property in _selectedSettingsGroup.Properties)
				{
					property.ValueChanged += SettingsPropertyValueChangedEventHandler; //todo
					_settingsPropertiesTable.Items.Add(property);
				}
				_settingsPropertiesTable.Sort();
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}

			UpdateActionEnablement();
		}

		private void Import()
		{
			try
			{
				var action = this.Host.ShowMessageBox(SR.MessageConfirmImportSettingsMetaData, MessageBoxActions.OkCancel);
				if (action == DialogBoxAction.Ok)
				{
					DoImport();
				}

				// update groups table
				FillSettingsGroupTable();

			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		private void DoImport()
		{
			var groups = SettingsGroupDescriptor.ListInstalledSettingsGroups(SettingsGroupFilter.SupportEnterpriseStorage);
			var task = new BackgroundTask(
				delegate(IBackgroundTaskContext context)
				{
					for (var i = 0; i < groups.Count; i++)
					{
						if (context.CancelRequested)
						{
							context.Cancel();
							break;
						}

						var group = groups[i];
						context.ReportProgress(new BackgroundTaskProgress(i, groups.Count, string.Format("Importing {0}", group.Name)));

						var props = SettingsPropertyDescriptor.ListSettingsProperties(group);
						_settingsStore.ImportSettingsGroup(group, props);
					}
				},
				true) { ThreadUICulture = Desktop.Application.CurrentUICulture };

			ProgressDialog.Show(task, this.Host.DesktopWindow);
		}

		private void SaveModifiedSettings(bool confirmationRequired)
		{
			// if no dirty properties, nothing to save
			if (_selectedSettingsGroup == null || !_selectedSettingsGroup.Properties.Any(p => p.Dirty))
				return;

			if (confirmationRequired && !ConfirmSave())
				return;

			try
			{
				_selectedSettingsGroup.Save();

				UpdateActionEnablement();

                try
                {
                    //Settings classes cache values and don't automatically reload, so try to reload at least the static default instance.
                    var settingsClass = ApplicationSettingsHelper.GetSettingsClass(_selectedSettingsGroup.Descriptor);
                    var defaultInstance = ApplicationSettingsHelper.GetDefaultInstance(settingsClass);
                    if (defaultInstance != null)
                        defaultInstance.Reload();
                }
                catch(Exception e)
                {
                    //Shouldn't really happen, and definitely not the end of the world.
                    Platform.Log(LogLevel.Debug, e);
                }

			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.MessageSaveSettingFailed, this.Host.DesktopWindow);
			}
		}

		private void ResetAllPropertyValues()
		{
			if (_selectedSettingsGroup == null)
				return;

			var action = this.Host.ShowMessageBox(SR.MessageResetAll, MessageBoxActions.YesNo);
			if (action == DialogBoxAction.Yes)
			{
				foreach (var property in _selectedSettingsGroup.Properties)
				{
					property.ResetValue();
					_settingsPropertiesTable.Items.NotifyItemUpdated(property);
				}
			}
		}

		private void ResetPropertyValue(Property property)
		{
			var action = this.Host.ShowMessageBox(SR.MessageReset, MessageBoxActions.YesNo);
			if (action == DialogBoxAction.Yes)
			{
				property.ResetValue();
				_settingsPropertiesTable.Items.NotifyItemUpdated(property);
			}
		}

		private void EditProperty(Property property)
		{
			if (property == null)
				return;

			try
			{
				var editor = new SettingEditorComponent(property.DefaultValue, property.Value);
				var exitCode = LaunchAsDialog(this.Host.DesktopWindow, new DialogBoxCreationArgs(editor, SR.TitleEditValue, null, true));
				if (exitCode == ApplicationComponentExitCode.Accepted)
				{
					property.Value = editor.CurrentValue;

					// update the table to reflect the changed value
					_settingsPropertiesTable.Items.NotifyItemUpdated(property);
				}
			}
			catch (Exception e)
			{
				// failed to launch editor
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		private void UpdateActionEnablement()
		{
			_resetAction.Enabled = (_selectedSettingsProperty != null && !_selectedSettingsProperty.UsingDefaultValue);
			_editAction.Enabled = (_selectedSettingsProperty != null);
			_saveAllAction.Enabled = (_selectedSettingsGroup != null && _selectedSettingsGroup.Properties.Any(p => p.Dirty));
			_resetAllAction.Enabled = _selectedSettingsGroup != null && _selectedSettingsGroup.Properties.Any(p => !p.UsingDefaultValue);
		}

		private IEnumerable<Group> GetSettingsGroups()
		{
			var groups = new List<Group>();

			// add all groups from the enterprise settings store
			groups.AddRange(_settingsStore.ListSettingsGroups().Select(d => new SettingsStoreGroup(d, _settingsStore)));

			// add locally installed groups that support editing of shared profiles
			// note that many groups may be both locally installed and in the settings store, so we need to filter duplicates
			var locals = SettingsGroupDescriptor.ListInstalledSettingsGroups(SettingsGroupFilter.SupportsEditingOfSharedProfile);
			groups.AddRange(locals.Where(l => !groups.Any(g => Equals(g.Descriptor, l))).Select(d => new LocallyInstalledGroup(d)));

			return groups;
		}

		private void SettingsPropertyValueChangedEventHandler(object sender, EventArgs args)
		{
			UpdateActionEnablement();
		}

		private bool ConfirmSave()
		{
			var action = this.Host.ShowMessageBox(SR.MessageSaveModified, MessageBoxActions.YesNo);
			return action == DialogBoxAction.Yes;
		}
	}
}
