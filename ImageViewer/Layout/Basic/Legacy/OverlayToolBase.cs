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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	public abstract class OverlayToolBase : Tool<IImageViewerToolContext>
	{
        //TODO (Phoenix5): #10730 - remove this when it's fixed.
        [ThreadStatic]
		private static IList<OverlayToolBase> _toolRegistry = new List<OverlayToolBase>();
		private event EventHandler _checkedChanged;
		private bool _checked;

		protected OverlayToolBase()
			: this(true)
		{
		}

		protected OverlayToolBase(bool @checked)
		{
			_checked = @checked;
		}

		private static IList<OverlayToolBase> ToolRegistry
		{
			get
			{
				if (_toolRegistry == null)
					_toolRegistry = new List<OverlayToolBase>();
				return _toolRegistry;
			}
		}

        private ActionSet _nilActions;

        public override IActionSet Actions
        {
            get
            {
                //These actions only exist in "legacy" mode.
                if (!Layout.Basic.ShowHideOverlaysTool.LegacyMode)
                {
                    if (_nilActions == null)
                    {
                        _nilActions = new ActionSet();
                        base.Actions = _nilActions;
                    }
                }

                return base.Actions;
            }
            protected set
            {
                base.Actions = value;
            }
        }

		public override void Initialize()
		{
			base.Initialize();

			ToolRegistry.Add(this);

			this.Context.Viewer.EventBroker.DisplaySetChanged += OnDisplaySetChanged;
		}

		protected override void Dispose(bool disposing)
		{
			this.Context.Viewer.EventBroker.DisplaySetChanged -= OnDisplaySetChanged;

			ToolRegistry.Remove(this);

			base.Dispose(disposing);
		}

		//NOTE: checked is changed internally, or by the parent overlay tool.
		//So, when it is changed externally, we don't draw because presumably the parent tool will do that.
		public bool Checked
		{
			get { return _checked; }
			set
			{
				if (_checked == value)
					return;
				
				_checked = value;
				OnCheckedChanged();
			}
		}

		public event EventHandler CheckedChanged
		{
			add { _checkedChanged += value; }
			remove { _checkedChanged -= value; }
		}

		protected virtual void OnCheckedChanged()
		{
			UpdateVisibility();
			EventsHelper.Fire(_checkedChanged, this, new EventArgs());
		}

		public void ShowHide()
		{
			//NOTE: When this method is called directly, the tool was invoked directly, so we draw after.
			Checked = !Checked; //changing this updates visibility.
			Context.Viewer.PhysicalWorkspace.Draw();
		}

		private void OnDisplaySetChanged(object sender, DisplaySetChangedEventArgs e)
		{
			if (e.NewDisplaySet == null)
				return;

			CodeClock clock = new CodeClock();
			clock.Start();

			UpdateVisibility(e.NewDisplaySet, Checked);
			
			clock.Stop();
			Platform.Log(LogLevel.Debug, "{0} - UpdateVisibility took {1}", GetType().FullName, clock.Seconds);

			//The display set will be drawn externally because it just changed.
		}

		private void UpdateVisibility()
		{
			if (Context == null)
				return;

			foreach (var imageBox in Context.Viewer.PhysicalWorkspace.ImageBoxes)
			{
				if (imageBox.DisplaySet != null)
					UpdateVisibility(imageBox.DisplaySet, Checked);
			}
		}

		protected virtual void UpdateVisibility(IDisplaySet displaySet, bool visible)
		{
			foreach (var image in displaySet.PresentationImages)
				UpdateVisibility(image, visible);
		}

		protected abstract void UpdateVisibility(IPresentationImage image, bool visible);

		public static IEnumerable<OverlayToolBase> EnumerateTools(IImageViewer imageViewer)
		{
			foreach (OverlayToolBase overlayTool in ToolRegistry)
			{
				if (overlayTool.Context != null && overlayTool.Context.Viewer == imageViewer)
					yield return overlayTool;
			}
		}
	}
}
