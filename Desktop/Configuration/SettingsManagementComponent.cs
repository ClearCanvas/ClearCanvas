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

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop.Configuration
{
	/// <summary>
	/// Launches the <see cref="SettingsManagementComponent"/>.
	/// </summary>
	[MenuAction("activate", "global-menus/MenuTools/MenuUtilities/MenuConfigureSettings", "Activate")]
	[ActionPermission("activate", AuthorityTokens.Desktop.SettingsManagement)]
	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class SettingsManagementLaunchTool : Tool<IDesktopToolContext>
	{
		private IWorkspace _workspace;

		/// <summary>
		/// Constructor.
		/// </summary>
		public SettingsManagementLaunchTool()
		{
		}

		/// <summary>
		/// Launches the <see cref="SettingsManagementComponent"/> or activates it if it's already open.
		/// </summary>
		/// <remarks>
		/// This method first looks for a valid extension of <see cref="SettingsStoreExtensionPoint"/> and
		/// with which to initialize the <see cref="SettingsManagementComponent"/>.  If one is not found,
		/// an instance of <see cref="LocalSettingsStore"/> is instantiated and passed to the
		/// <see cref="SettingsManagementComponent"/>.  The <see cref="LocalSettingsStore"/> allows
		/// the local application settings to be modified, where by default they cannot be.
		/// </remarks>
		public void Activate()
		{
			if (_workspace != null)
			{
				_workspace.Activate();
				return;
			}

			ISettingsStore store;
			try
			{
				// if this throws an exception, only the default LocalFileSettingsProvider can be used.
				store = SettingsStore.Create();
			}
			catch (NotSupportedException)
			{
				//allow editing of the app.config file via the LocalSettingsStore.
				store = new LocalSettingsStore();
			}

			if (!store.IsOnline)
			{
				base.Context.DesktopWindow.ShowMessageBox(SR.MessageSettingsStoreOffline, MessageBoxActions.Ok);
				return;
			}

			_workspace = ApplicationComponent.LaunchAsWorkspace(
				this.Context.DesktopWindow,
				new SettingsManagementComponent(store),
				SR.TitleSettingsEditor,
				"Settings Management");

			_workspace.Closed += OnWorkspaceClosed;
		}

		private void OnWorkspaceClosed(object sender, ClosedEventArgs e)
		{
			_workspace = null;
		}

		protected override void Dispose(bool disposing)
		{
			if (_workspace != null)
				_workspace.Closed -= OnWorkspaceClosed;

			base.Dispose(disposing);
		}
	}

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
	public class SettingsManagementComponent : ApplicationComponent
	{
		#region SettingsProperty class

		/// <summary>
		/// Defines a settings property for presentation in the <see cref="SettingsManagementComponent"/> view.
		/// </summary>
		public class SettingsProperty
		{
			private SettingsPropertyDescriptor _descriptor;
			private string _value;
			private string _startingValue;

			private event EventHandler _valueChanged;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="descriptor">The descriptor for the property.</param>
			/// <param name="value">The current value of the property.</param>
			public SettingsProperty(SettingsPropertyDescriptor descriptor, string value)
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
						EventsHelper.Fire(_valueChanged, this, EventArgs.Empty);
					}
				}
			}

			/// <summary>
			/// Raised when <see cref="Value"/> has changed.
			/// </summary>
			public event EventHandler ValueChanged
			{
				add { _valueChanged += value; }
				remove { _valueChanged -= value; }
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

		//TODO (CR Sept 2010): Change this to use ApplicationSettingsExtensions to set the shared property values.
		//That way, we can edit enterprise and local settings at the same time.
		private ISettingsStore _configStore;

		private Table<SettingsGroupDescriptor> _settingsGroupTable;
		private SettingsGroupDescriptor _selectedSettingsGroup;
		private event EventHandler _selectedSettingsGroupChanged;

		private Table<SettingsProperty> _settingsPropertiesTable;
		private SettingsProperty _selectedSettingsProperty;
		private event EventHandler _selectedSettingsPropertyChanged;

		private SimpleActionModel _settingsPropertiesActionModel;
		private ClickAction _saveAllAction;
		private ClickAction _resetAllAction;
		private ClickAction _resetAction;
		private ClickAction _editAction;

		private SimpleActionModel _settingsGroupsActionModel;
		private ClickAction _importAction;


		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="configStore">The <see cref="ISettingsStore"/> for which the default values will be modified.</param>
		public SettingsManagementComponent(ISettingsStore configStore)
		{
			_configStore = configStore;

			// define the structure of the settings group table
			ITableColumn _groupNameColumn;
			_settingsGroupTable = new Table<SettingsGroupDescriptor>();
			_settingsGroupTable.Columns.Add(_groupNameColumn = new TableColumn<SettingsGroupDescriptor, string>(SR.TitleGroup,
				delegate(SettingsGroupDescriptor t) { return t.Name; }));
			_settingsGroupTable.Columns.Add(new TableColumn<SettingsGroupDescriptor, string>(SR.TitleVersion,
				delegate(SettingsGroupDescriptor t) { return t.Version.ToString(); }));
			_settingsGroupTable.Columns.Add(new TableColumn<SettingsGroupDescriptor, string>(SR.TitleDescription,
				delegate(SettingsGroupDescriptor t) { return t.Description; }));
			_settingsGroupTable.Sort(new TableSortParams(_groupNameColumn, true));

			// define the settings properties table
			ITableColumn _propertyNameColumn;
			_settingsPropertiesTable = new Table<SettingsProperty>();
			_settingsPropertiesTable.Columns.Add(_propertyNameColumn = new TableColumn<SettingsProperty, string>(SR.TitleProperty,
				delegate(SettingsProperty p) { return p.Name; }));
			_settingsPropertiesTable.Columns.Add(new TableColumn<SettingsProperty, string>(SR.TitleDescription,
				delegate(SettingsProperty p) { return p.Description; }));
			_settingsPropertiesTable.Columns.Add(new TableColumn<SettingsProperty, string>(SR.TitleScope,
				delegate(SettingsProperty p) { return p.Scope.ToString(); }));
			_settingsPropertiesTable.Columns.Add(new TableColumn<SettingsProperty, string>(SR.TitleType,
				delegate(SettingsProperty p) { return p.TypeName; }));
			_settingsPropertiesTable.Columns.Add(new TableColumn<SettingsProperty, string>(SR.TitleDefaultValue,
				delegate(SettingsProperty p) { return p.DefaultValue; }));
			_settingsPropertiesTable.Columns.Add(new TableColumn<SettingsProperty, string>(SR.TitleValue,
				delegate(SettingsProperty p) { return p.Value; },
				delegate(SettingsProperty p, string text) { p.Value = text; }));
			_settingsPropertiesTable.Sort(new TableSortParams(_propertyNameColumn, true));

			_settingsGroupsActionModel = new SimpleActionModel(new ApplicationThemeResourceResolver(this.GetType().Assembly));
			_importAction = _settingsGroupsActionModel.AddAction("import", SR.LabelImport, "ImportToolSmall.png",
				SR.TooltipImportSettingsMetaData,
				Import);
			_importAction.Visible = _configStore.SupportsImport;

			_settingsPropertiesActionModel = new SimpleActionModel(new ApplicationThemeResourceResolver(this.GetType().Assembly));

			_saveAllAction = _settingsPropertiesActionModel.AddAction("saveall", SR.LabelSaveAll, "SaveToolSmall.png",
				delegate() { SaveModifiedSettings(false); });

			_editAction = _settingsPropertiesActionModel.AddAction("edit", SR.LabelEdit, "EditToolSmall.png",
			   delegate() { EditProperty(_selectedSettingsProperty); });

			_resetAction = _settingsPropertiesActionModel.AddAction("reset", SR.LabelReset, "ResetToolSmall.png",
			   delegate() { ResetPropertyValue(_selectedSettingsProperty); });

			_resetAllAction = _settingsPropertiesActionModel.AddAction("resetall", SR.LabelResetAll, "ResetAllToolSmall.png",
				delegate() { ResetAllPropertyValues(); });

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
		/// Called by the host when the application component is being terminated.
		/// </summary>
		/// <remarks>
		/// Override this method to implement custom termination logic.  Overrides must be sure to call the base implementation.
		/// </remarks>
		public override void Stop()
		{
			base.Stop();
		}

		/// <summary>
		/// Determines whether the component can exit without any user interaction.
		/// </summary>
		/// <returns>True if no properties are dirty, otherwise false.</returns>
		public override bool CanExit()
		{
			// return false if anything modified
			return _selectedSettingsGroup == null || !IsAnyPropertyDirty();
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
				SettingsGroupDescriptor settingsClass = (SettingsGroupDescriptor)value.Item;
				if (settingsClass != _selectedSettingsGroup)
				{
					// save any changes before changing _selectedSettingsGroup
					SaveModifiedSettings(true);

					_selectedSettingsGroup = settingsClass;
					LoadSettingsProperties();
					UpdateActionEnablement();
					EventsHelper.Fire(_selectedSettingsGroupChanged, this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Raised when <see cref="SelectedSettingsGroup"/> has changed.
		/// </summary>
		public event EventHandler SelectedSettingsGroupChanged
		{
			add { _selectedSettingsGroupChanged += value; }
			remove { _selectedSettingsGroupChanged -= value; }
		}

		/// <summary>
		/// Gets a table of settings properties (<see cref="SettingsProperty"/>) for the
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
		/// Gets or sets the currently selected <see cref="SettingsProperty"/> as an <see cref="ISelection"/>.
		/// </summary>
		public ISelection SelectedSettingsProperty
		{
			get { return new Selection(_selectedSettingsProperty); }
			set
			{
				SettingsProperty p = (SettingsProperty)value.Item;
				if (p != _selectedSettingsProperty)
				{
					_selectedSettingsProperty = p;
					UpdateActionEnablement();
					EventsHelper.Fire(_selectedSettingsPropertyChanged, this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Raised when <see cref="SelectedSettingsProperty"/> has changed.
		/// </summary>
		public event EventHandler SelectedSettingsPropertyChanged
		{
			add { _selectedSettingsPropertyChanged += value; }
			remove { _selectedSettingsPropertyChanged -= value; }
		}

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
			foreach (SettingsGroupDescriptor group in _configStore.ListSettingsGroups())
			{
				_settingsGroupTable.Items.Add(group);
			}
			_settingsGroupTable.Sort();
		}

		private void LoadSettingsProperties()
		{
			_settingsPropertiesTable.Items.Clear();

			if (_selectedSettingsGroup != null)
			{
				try
				{
					Dictionary<string, string> values = _configStore.GetSettingsValues(
							_selectedSettingsGroup,
							null, null // load the default profile
							);

					FillSettingsPropertiesTable(values);
				}
				catch (Exception e)
				{
					ExceptionHandler.Report(e, this.Host.DesktopWindow);
				}
			}
		}

		private void Import()
		{
			try
			{
				DialogBoxAction action = this.Host.ShowMessageBox(SR.MessageConfirmImportSettingsMetaData,
								 MessageBoxActions.OkCancel);
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
			List<SettingsGroupDescriptor> groups = SettingsGroupDescriptor.ListInstalledSettingsGroups(true);
			BackgroundTask task = new BackgroundTask(
				delegate(IBackgroundTaskContext context)
				{
					for (int i = 0; i < groups.Count; i++)
					{
						if (context.CancelRequested)
						{
							context.Cancel();
							break;
						}

						SettingsGroupDescriptor group = groups[i];
						context.ReportProgress(new BackgroundTaskProgress(i, groups.Count, string.Format("Importing {0}", group.Name)));

						List<SettingsPropertyDescriptor> props = SettingsPropertyDescriptor.ListSettingsProperties(group);
						_configStore.ImportSettingsGroup(group, props);

					}
				},
				true);

			ProgressDialog.Show(task, this.Host.DesktopWindow, true);
		}

		private void SaveModifiedSettings(bool confirmationRequired)
		{
			// if no dirty properties, nothing to save
			if (_selectedSettingsGroup != null && IsAnyPropertyDirty())
			{
				if (confirmationRequired && !ConfirmSave())
					return;

				// fill a dictionary with all dirty values
				Dictionary<string, string> values = new Dictionary<string, string>();
				foreach (SettingsProperty p in _settingsPropertiesTable.Items)
				{
					if (p.Dirty)
					{
						values[p.Name] = p.Value;
					}
				}

				try
				{
					// save to the default profile
					_configStore.PutSettingsValues(_selectedSettingsGroup,null, null, values);

					// mark all properties as clean again
					foreach (SettingsProperty p in _settingsPropertiesTable.Items)
						p.MarkClean();

					UpdateActionEnablement();

					// update any loaded instances
					ApplicationSettingsRegistry.Instance.Reload(_selectedSettingsGroup);

				}
				catch (Exception e)
				{
					ExceptionHandler.Report(e, SR.MessageSaveSettingFailed, this.Host.DesktopWindow);
				}
			}
		}

		private void ResetAllPropertyValues()
		{
			DialogBoxAction action = this.Host.ShowMessageBox(SR.MessageResetAll, MessageBoxActions.YesNo);
			if (action == DialogBoxAction.Yes)
			{
				foreach (SettingsProperty property in _settingsPropertiesTable.Items)
				{
					property.Value = property.DefaultValue;
					_settingsPropertiesTable.Items.NotifyItemUpdated(property);
				}
			}
		}

		private void ResetPropertyValue(SettingsProperty property)
		{
			DialogBoxAction action = this.Host.ShowMessageBox(SR.MessageReset, MessageBoxActions.YesNo);
			if (action == DialogBoxAction.Yes)
			{
				property.Value = property.DefaultValue;
				_settingsPropertiesTable.Items.NotifyItemUpdated(property);
			}
		}

		private void EditProperty(SettingsProperty property)
		{
			try
			{
				if (property != null)
				{
					SettingEditorComponent editor = new SettingEditorComponent(property.DefaultValue, property.Value);
					ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleEditValue);
					if (exitCode == ApplicationComponentExitCode.Accepted)
					{
						property.Value = editor.CurrentValue;

						// update the table to reflect the changed value
						_settingsPropertiesTable.Items.NotifyItemUpdated(property);
					}
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
			_saveAllAction.Enabled = (_selectedSettingsGroup != null && IsAnyPropertyDirty());
			_resetAllAction.Enabled = CollectionUtils.Contains<SettingsProperty>(_settingsPropertiesTable.Items,
				delegate(SettingsProperty p) { return !p.UsingDefaultValue; });
		}

		private void FillSettingsPropertiesTable(IDictionary<string, string> storedValues)
		{
			_settingsPropertiesTable.Items.Clear();
			foreach (SettingsPropertyDescriptor pi in _configStore.ListSettingsProperties(_selectedSettingsGroup))
			{
				string value = storedValues.ContainsKey(pi.Name) ? storedValues[pi.Name] : pi.DefaultValue;
				SettingsProperty property = new SettingsProperty(pi, value);
				property.ValueChanged += SettingsPropertyValueChangedEventHandler;
				_settingsPropertiesTable.Items.Add(property);
			}
			_settingsPropertiesTable.Sort();
		}

		private void SettingsPropertyValueChangedEventHandler(object sender, EventArgs args)
		{
			UpdateActionEnablement();
		}

		private bool IsAnyPropertyDirty()
		{
			return CollectionUtils.Contains<SettingsProperty>(_settingsPropertiesTable.Items,
				delegate(SettingsProperty p) { return p.Dirty; });
		}

		private bool ConfirmSave()
		{
			DialogBoxAction action = this.Host.ShowMessageBox(SR.MessageSaveModified, MessageBoxActions.YesNo);
			return action == DialogBoxAction.Yes;
		}

	}
}
