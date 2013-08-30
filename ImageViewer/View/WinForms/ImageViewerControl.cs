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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.View.WinForms
{
	public class ImageViewerControl : UserControl
	{
		private Form _parentForm;
		private PhysicalWorkspace _physicalWorkspace;
		private ImageViewerComponent _component;
		private DelayedEventPublisher _delayedEventPublisher;

		internal ImageViewerControl(ImageViewerComponent component)
		{
			_component = component;
			_physicalWorkspace = (PhysicalWorkspace) _component.PhysicalWorkspace;

			SuspendLayout();
			Name = "ImageViewerControl";
			ResumeLayout(false);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);

			base.BackColor = Color.Black;

			_component.Closing += OnComponentClosing;
			_physicalWorkspace.Drawing += OnPhysicalWorkspaceDrawing;
			_physicalWorkspace.LayoutCompleted += OnLayoutCompleted;
			_physicalWorkspace.ScreenRectangleChanged += OnScreenRectangleChanged;

			_delayedEventPublisher = new DelayedEventPublisher(OnRecalculateImageBoxes, 50);
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_parentForm != null)
				{
					_parentForm.Move -= OnParentMoved;
					_parentForm = null;
				}

				if (_component != null)
				{
					_component.Closing -= OnComponentClosing;
					_component = null;
				}

				if (_delayedEventPublisher != null)
				{
					_delayedEventPublisher.Dispose();
					_delayedEventPublisher = null;
				}

				if (_physicalWorkspace != null)
				{
					_physicalWorkspace.Drawing -= OnPhysicalWorkspaceDrawing;
					_physicalWorkspace.LayoutCompleted -= OnLayoutCompleted;
					_physicalWorkspace.ScreenRectangleChanged -= OnScreenRectangleChanged;

					_physicalWorkspace = null;
				}
			}

			base.Dispose(disposing);
		}

		internal void Draw()
		{
			foreach (ImageBoxControl control in Controls)
				control.Draw();

			Invalidate();
		}

		#region Protected members

		protected override void OnParentChanged(EventArgs e)
		{
			SetParentForm(ParentForm);

			base.OnParentChanged(e);
		}

		protected override void OnLoad(EventArgs e)
		{
			AddImageBoxControls(_physicalWorkspace);

			base.OnLoad(e);
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			UpdateScreenRectangle();
		}

		#endregion

		#region Private members

		private void OnParentMoved(object sender, EventArgs e)
		{
			UpdateScreenRectangle();
		}

		private void OnScreenRectangleChanged(object sender, EventArgs e)
		{
			_delayedEventPublisher.Publish(this, EventArgs.Empty);
		}

		private void OnRecalculateImageBoxes(object sender, EventArgs e)
		{
			SuspendLayout();

			foreach (ImageBoxControl control in Controls)
				control.ParentRectangle = ClientRectangle;

			ResumeLayout(false);

			Invalidate();
		}

		private void OnPhysicalWorkspaceDrawing(object sender, EventArgs e)
		{
			Draw();
		}

		private void OnComponentClosing(object sender, EventArgs e)
		{
			List<Control> oldControlList = Controls.Cast<Control>().ToList();

			foreach (Control control in oldControlList)
			{
				Controls.Remove(control);
				control.Dispose();
			}

			// must call to force the UserControl's cachedLayoutEventArgs field to be cleared, otherwise it ends up leaking the viewer component
			PerformLayout();
		}

		private void OnLayoutCompleted(object sender, EventArgs e)
		{
			List<Control> oldControlList = Controls.Cast<Control>().ToList();

			// We add all the new tile controls to the image box control first,
			// then we remove the old ones. Removing them first then adding them
			// results in flickering, which we don't want.
			AddImageBoxControls(_physicalWorkspace);

			foreach (Control control in oldControlList)
			{
				Controls.Remove(control);
				control.Dispose();
			}
		}

		private void UpdateScreenRectangle()
		{
			_physicalWorkspace.ScreenRectangle = RectangleToScreen(ClientRectangle);
		}

		private void AddImageBoxControls(PhysicalWorkspace physicalWorkspace)
		{
			foreach (ImageBox imageBox in physicalWorkspace.ImageBoxes)
				AddImageBoxControl(imageBox);
		}

		private void AddImageBoxControl(ImageBox imageBox)
		{
			ImageBoxView view = (ImageBoxView) ViewFactory.CreateAssociatedView(typeof (ImageBox));

			view.ImageBox = imageBox;
			view.ParentRectangle = ClientRectangle;

			ImageBoxControl control = (ImageBoxControl) view.GuiElement;

			control.SuspendLayout();
			Controls.Add(control);
			control.ResumeLayout(false);
		}

		private void SetParentForm(Form value)
		{
			if (_parentForm != value)
			{
				if (_parentForm != null)
					_parentForm.Move -= OnParentMoved;

				_parentForm = value;

				if (_parentForm != null)
					_parentForm.Move += OnParentMoved;
			}
		}

		#endregion
	}
}