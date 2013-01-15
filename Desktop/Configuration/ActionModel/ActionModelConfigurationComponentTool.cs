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
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop.Configuration.ActionModel
{
	[ExtensionPoint]
	public sealed class ActionModelConfigurationComponentToolExtensionPoint : ExtensionPoint<ITool> {}

	public interface IActionModelConfigurationComponentToolContext : IToolContext
	{
		ActionModelConfigurationComponent Component { get; }
		IDesktopWindow DesktopWindow { get; }
	}

	public abstract class ActionModelConfigurationComponentTool : Tool<IActionModelConfigurationComponentToolContext>
	{
		public override void Initialize()
		{
			base.Initialize();

			this.Component.SelectedNodeChanged += OnComponentSelectedNodeChanged;
		}

		private void OnComponentSelectedNodeChanged(object sender, EventArgs e)
		{
			this.OnSelectedNodeChanged();
		}

		protected override void Dispose(bool disposing)
		{
			this.Component.SelectedNodeChanged -= OnComponentSelectedNodeChanged;

			base.Dispose(disposing);
		}

		protected AbstractActionModelTreeNode SelectedNode
		{
			get { return this.Component.SelectedNode; }
		}

		protected ActionModelConfigurationComponent Component
		{
			get { return base.Context.Component; }
		}

		protected virtual void OnSelectedNodeChanged() {}
	}

	partial class ActionModelConfigurationComponent
	{
		protected class ActionModelConfigurationComponentToolContext : IActionModelConfigurationComponentToolContext
		{
			private readonly ActionModelConfigurationComponent _component;

			public ActionModelConfigurationComponentToolContext(ActionModelConfigurationComponent component)
			{
				_component = component;
			}

			public ActionModelConfigurationComponent Component
			{
				get { return _component; }
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _component.Host.DesktopWindow; }
			}
		}
	}
}