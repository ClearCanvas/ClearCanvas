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

#if DEBUG
// ReSharper disable LocalizableElement

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Configuration.ActionModel;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop.Configuration.Tools
{
	[MenuAction("configure", "global-menus/MenuTools/MenuUtilities/Configure Action Model", "Configure")]
	[ExtensionOf(typeof (DesktopToolExtensionPoint))]
	public sealed class ActionModelsTool : Tool<IDesktopToolContext>
	{
		public void Configure()
		{
			try
			{
				var component = new ActionModelsToolComponent();
				var args = new DialogBoxCreationArgs(component, "Action Model Configurator", "actionModelConfiugrator", true);
				Context.DesktopWindow.ShowDialogBox(args);
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, Context.DesktopWindow);
			}
		}
	}

	[ExtensionPoint]
	public sealed class ActionModelsToolComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (ActionModelsToolComponentViewExtensionPoint))]
	public class ActionModelsToolComponent : ApplicationComponentContainer
	{
		private class HostImplementation : ContainedComponentHost
		{
			internal HostImplementation(ApplicationComponentContainer owner, IApplicationComponent component)
				: base(owner, component) {}

			/// <summary>
			/// Contained components will use the comand history provided by the host that 
			/// owns the container.
			/// </summary>
			public override CommandHistory CommandHistory
			{
				get { return OwnerHost.CommandHistory; }
			}

			/// <summary>
			/// Gets or sets the title displayed in the user-interface.
			/// </summary>
			public override string Title
			{
				set { OwnerHost.Title = value; }
			}
		}

		private static readonly Regex _actionModelIdRegex = new Regex(@"^(?:([^:]+)\:)?([^:]+)$", RegexOptions.Compiled);
		private readonly Dictionary<string, ApplicationComponentHost> _componentHosts = new Dictionary<string, ApplicationComponentHost>();
		private ApplicationComponentHost _activeComponentHost;
		private string _selectedActionModel;

		public ApplicationComponentHost ActiveComponentHost
		{
			get { return _activeComponentHost; }
			private set
			{
				if (_activeComponentHost != value)
				{
					_activeComponentHost = value;

					if (_activeComponentHost != null && !_activeComponentHost.IsStarted) _activeComponentHost.StartComponent();

					EventsHelper.Fire(ActiveComponentChanged, this, new EventArgs());
				}
			}
		}

		public ActionModelConfigurationComponent ActiveComponent
		{
			get { return _activeComponentHost != null ? (ActionModelConfigurationComponent) _activeComponentHost.Component : null; }
		}

		public event EventHandler ActiveComponentChanged;

		public string[] AvailableActionModels
		{
			get
			{
                var nodes = ActionModelSettings.DefaultInstance.ActionModelsXml.SelectNodes("/action-models/action-model");
				return nodes == null ? new string[0] : nodes.OfType<XmlElement>().Select(x => x.GetAttribute("id")).ToArray();
			}
		}

		public string SelectedActionModel
		{
			get { return _selectedActionModel; }
			set
			{
				if (_selectedActionModel != value)
				{
					_selectedActionModel = value;

					if (!string.IsNullOrEmpty(_selectedActionModel))
					{
						var m = _actionModelIdRegex.Match(_selectedActionModel);
						if (m.Success)
						{
							SelectActionModel(m.Groups[1].Value, m.Groups[2].Value, CreateAbstractActionSet(_selectedActionModel));
						}
					}
				}
			}
		}

		public void SelectActionModel(string @namespace, string site, IActionSet actionSet)
		{
			var actionModelId = string.Format(@"{0}:{1}", @namespace, site);
			if (!_componentHosts.ContainsKey(actionModelId))
			{
				var component = new ActionModelConfigurationComponent(@namespace, site, actionSet, Host.DesktopWindow);
				_componentHosts.Add(actionModelId, new HostImplementation(this, component));
			}
			ActiveComponentHost = _componentHosts[actionModelId];
		}

		private static IActionSet CreateAbstractActionSet(string actionModelId)
		{
			var dummyResourceResolver = new ResourceResolver(typeof (ActionModelsTool), false);
			var actionNodes = ActionModelSettings.DefaultInstance.ActionModelsXml.SelectNodes(string.Format("/action-models/action-model[@id='{0}']/action", actionModelId));
			return actionNodes != null ? new ActionSet((from XmlElement action in actionNodes select AbstractAction.Create(action.GetAttribute("id"), action.GetAttribute("path"), true, dummyResourceResolver)).ToList()) : new ActionSet();
		}

		/// <summary>
		/// Starts this component and the <see cref="ActiveComponentHost"/>.
		/// </summary>
		///  <remarks>
		/// Override this method to implement custom initialization logic.  Overrides must be sure to call the base implementation.
		/// </remarks>
		public override void Start()
		{
			base.Start();

			foreach (var x in _componentHosts.Values.Where(x => !x.IsStarted))
				x.StartComponent();
		}

		/// <summary>
		/// Stops this component and the <see cref="ActiveComponentHost"/>.
		/// </summary>
		/// <remarks>
		/// Override this method to implement custom termination logic.  Overrides must be sure to call the base implementation.
		/// </remarks>
		public override void Stop()
		{
			foreach (var x in _componentHosts.Values.Where(x => x.IsStarted))
				x.StopComponent();

			base.Stop();
		}

		/// <summary>
		/// Gets a value indicating whether there are any data validation errors.
		/// </summary>
		public override bool HasValidationErrors
		{
			get { return (_activeComponentHost != null && _activeComponentHost.Component.HasValidationErrors) || base.HasValidationErrors; }
		}

		/// <summary>
		/// Sets the <see cref="ApplicationComponent.ValidationVisible"/> property and raises the 
		/// <see cref="ApplicationComponent.ValidationVisibleChanged"/> event.
		/// </summary>
		public override void ShowValidation(bool show)
		{
			base.ShowValidation(show);

			if (_activeComponentHost != null) _activeComponentHost.Component.ShowValidation(show);
		}

		/// <summary>
		/// Gets an enumeration of the contained components.
		/// </summary>
		public override IEnumerable<IApplicationComponent> ContainedComponents
		{
			get { return _componentHosts.Values.Select(x => x.Component); }
		}

		/// <summary>
		/// Gets an enumeration of the components that are currently visible.
		/// </summary>
		public override IEnumerable<IApplicationComponent> VisibleComponents
		{
			get { if (_activeComponentHost != null) yield return _activeComponentHost.Component; }
		}

		/// <summary>
		/// Does nothing, since the hosted component is started by default.
		/// </summary>
		public override void EnsureStarted(IApplicationComponent component)
		{
			if (!IsStarted)
				throw new InvalidOperationException(SR.ExceptionContainerNeverStarted);

			// nothing to do, since the hosted component is started by default
		}

		/// <summary>
		/// Does nothing, since the hosted component is visible by default.
		/// </summary>
		public override void EnsureVisible(IApplicationComponent component)
		{
			if (!IsStarted)
				throw new InvalidOperationException(SR.ExceptionContainerNeverStarted);

			// nothing to do, since the hosted component is visible by default
		}

		public void Accept()
		{
			if (HasValidationErrors)
			{
				ShowValidation(true);
				return;
			}

			foreach (var x in _componentHosts.Values.Where(x => x.IsStarted))
				((ActionModelConfigurationComponent) x.Component).Save();

			base.ExitCode = ApplicationComponentExitCode.Accepted;
			Host.Exit();
		}

		public void Cancel()
		{
			base.ExitCode = ApplicationComponentExitCode.None;
			Host.Exit();
		}
	}
}

// ReSharper restore LocalizableElement
#endif