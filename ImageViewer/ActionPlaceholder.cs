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

using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer
{
	public sealed class ActionPlaceholderAttribute : ActionInitiatorAttribute
	{
		private readonly string _pathHint;
		private readonly string _groupHint = string.Empty;
		private bool _initiallyAvailable = true;

		public ActionPlaceholderAttribute(string actionId, string pathHint, string groupHint)
			: base(actionId)
		{
			_pathHint = pathHint;
			_groupHint = groupHint;
		}

		public bool InitiallyAvailable
		{
			get { return _initiallyAvailable; }
			set { _initiallyAvailable = value; }
		}

		public override void Apply(IActionBuildingContext builder)
		{
			ActionPath path = new ActionPath(_pathHint, builder.ResourceResolver);
			builder.Action = new ActionPlaceholder(builder.ActionID, path, builder.ResourceResolver);
			builder.Action.Available = _initiallyAvailable;
			builder.Action.Persistent = true;
			builder.Action.Visible = false;
			builder.Action.Label = path.LastSegment.LocalizedText;
			builder.Action.GroupHint = new GroupHint(_groupHint ?? string.Empty);
		}
	}

	public class ActionPlaceholder : ClickAction
	{
		public ActionPlaceholder(string actionId, ActionPath path, IResourceResolver resourceResolver)
			: base(actionId, path, ClickActionFlags.None, resourceResolver) {}

		public static ActionPlaceholder GetPlaceholderAction(string site, IActionSet actions, string placeholderActionId)
		{
			string actionId = ":" + placeholderActionId;
			return (ActionPlaceholder) CollectionUtils.SelectFirst(actions, x => x is ActionPlaceholder && x.Path.StartsWith(new Path(site)) && x.ActionID.EndsWith(actionId));
		}

		public MenuAction CreateMenuAction(string actionId, string pathSuffix, ClickActionFlags flags)
		{
			return new DynamicMenuAction(actionId, pathSuffix, this, flags, this.ResourceResolver);
		}

		public MenuAction CreateMenuAction(string actionId, string pathSuffix, ClickActionFlags flags, IResourceResolver resourceResolver)
		{
			return new DynamicMenuAction(actionId, pathSuffix, this, flags, resourceResolver);
		}

		private class DynamicMenuAction : MenuAction
		{
			private readonly ActionPlaceholder _actionPlaceholder;
			private readonly Path _pathSuffix;

			public DynamicMenuAction(string actionId, string pathSuffix, ActionPlaceholder actionPlaceholder, ClickActionFlags flags, IResourceResolver resourceResolver)
				: base(actionId, actionPlaceholder.Path, flags, resourceResolver)
			{
				_actionPlaceholder = actionPlaceholder;
				_pathSuffix = new Path(pathSuffix);
			}

			private string BuildPath()
			{
				return _actionPlaceholder.Path.SubPath(0, _actionPlaceholder.Path.Segments.Count - 1).Append(_pathSuffix).ToString();
			}

			public override sealed ActionPath Path
			{
				get { return new ActionPath(this.BuildPath(), this.ResourceResolver); }
				set { }
			}

			public override sealed GroupHint GroupHint
			{
				get { return _actionPlaceholder.GroupHint; }
				set { }
			}

			public override sealed bool Available
			{
				get { return _actionPlaceholder.Available; }
				set { }
			}

			public override sealed bool Persistent
			{
				get { return false; }
			}
		}
	}
}