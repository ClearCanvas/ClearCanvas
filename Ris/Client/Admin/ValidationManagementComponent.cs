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
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Admin
{
	//JR: don't really need this because you can access validation from the live component directly
    //[MenuAction("launch", "global-menus/Admin/UI Validation", "Launch")]
	[ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.Desktop.UIValidationRules)]
	[ExtensionOf(typeof(DesktopToolExtensionPoint), FeatureToken = FeatureTokens.RIS.Core)]
    public class ValidationManagementTool : Tool<IDesktopToolContext>
    {
        private Workspace _workspace;

        public void Launch()
        {
            if (_workspace == null)
            {
                try
                {
                    ValidationManagementComponent component = new ValidationManagementComponent();

                    _workspace = ApplicationComponent.LaunchAsWorkspace(
                        this.Context.DesktopWindow,
                        component,
                        SR.TitleUiValidationManagement);
                    _workspace.Closed += delegate { _workspace = null; };

                }
                catch (Exception e)
                {
                    // could not launch component
                    ExceptionHandler.Report(e, this.Context.DesktopWindow);
                }
            }
            else
            {
                _workspace.Activate();
            }
        }
    }

    /// <summary>
    /// Extension point for views onto <see cref="ValidationManagementComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ValidationManagementComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ValidationManagementComponent class
    /// </summary>
    [AssociateView(typeof(ValidationManagementComponentViewExtensionPoint))]
    public class ValidationManagementComponent : ApplicationComponent
    {
        private readonly Table<Type> _applicationComponents;
        private Type _selectedComponent;


        /// <summary>
        /// Constructor
        /// </summary>
        public ValidationManagementComponent()
        {
            _applicationComponents = new Table<Type>();
            _applicationComponents.Columns.Add(new TableColumn<Type, string>(SR.ColumnComponent,
                delegate(Type t) { return t.Name; }));
            _applicationComponents.Columns.Add(new TableColumn<Type, string>(SR.ColumnNamespace,
                delegate(Type t) { return t.Namespace; }));
        }

        public override void Start()
        {
            foreach (PluginInfo plugin in Platform.PluginManager.Plugins)
            {
                foreach (Type t in plugin.Assembly.Resolve().GetExportedTypes())
                {
                    try
                    {
                        // exclude abstract types, since there is currently no concept of "inheritance" for validation rules
                        // exclude types that are part of the framework (Desktop)
                        if (!t.IsAbstract && !t.Namespace.StartsWith("ClearCanvas.Desktop") &&
                            CollectionUtils.Contains(t.GetInterfaces(),
                                delegate(Type i) { return i.Equals(typeof(IApplicationComponent)); }))
                        {
                            _applicationComponents.Items.Add(t);
                        }
                    }
                    catch (Exception e)
                    {
                        // some weird types seem to misbehave - who cares, they aren't useful to us
                        Platform.Log(LogLevel.Warn, e);
                    }
                }
            }

            base.Start();
        }

        #region Presentation Model

        public ITable ApplicationComponents
        {
            get { return _applicationComponents; }
        }

        public ISelection SelectedComponent
        {
            get { return new Selection(_selectedComponent); }
            set
            {
                Type selected = (Type)value.Item;
                if (!Equals(_selectedComponent, selected))
                {
                    _selectedComponent = selected;
                }
            }
        }

        public void ApplicationComponentDoubleClicked()
        {
            try
            {
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                    this.Host.DesktopWindow,
                    new ValidationEditorComponent(_selectedComponent),
                    string.Format("{0} Rules Editor", _selectedComponent.Name));
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        #endregion
    }
}
