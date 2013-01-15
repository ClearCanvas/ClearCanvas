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
using System.Windows.Forms;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.View.WinForms
{
	public partial class ImageViewerControl : UserControl
	{
		private Form _parentForm;
		private PhysicalWorkspace _physicalWorkspace;
		private ImageViewerComponent _component;
		private DelayedEventPublisher _delayedEventPublisher;

		internal ImageViewerControl(ImageViewerComponent component)
		{
			_component = component;
			_physicalWorkspace = _component.PhysicalWorkspace as PhysicalWorkspace;
			InitializeComponent();

			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

			this.BackColor = Color.Black;

			_component.Closing += new EventHandler(OnComponentClosing);
			_physicalWorkspace.Drawing += new EventHandler(OnPhysicalWorkspaceDrawing);
			_physicalWorkspace.LayoutCompleted += new EventHandler(OnLayoutCompleted);
			_physicalWorkspace.ScreenRectangleChanged += new EventHandler(OnScreenRectangleChanged);

			_delayedEventPublisher = new DelayedEventPublisher(OnRecalculateImageBoxes, 50);
		}

		internal void Draw()
		{
			foreach (ImageBoxControl control in this.Controls)
				control.Draw();

			Invalidate();
		}

		#region Protected members

		protected override void OnParentChanged(EventArgs e) {
			SetParentForm(base.ParentForm);
	
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
			this.SuspendLayout();

			foreach (ImageBoxControl control in this.Controls)
				control.ParentRectangle = this.ClientRectangle;

			this.ResumeLayout(false);

			Invalidate();
		}

		private void OnPhysicalWorkspaceDrawing(object sender, EventArgs e)
		{
			Draw();
		}

		void OnComponentClosing(object sender, EventArgs e)
		{
			while (this.Controls.Count > 0)
				this.Controls[0].Dispose();
		}

		private void OnLayoutCompleted(object sender, EventArgs e)
		{
			List<Control> oldControlList = new List<Control>();

			foreach (Control control in this.Controls)
				oldControlList.Add(control);

			// We add all the new tile controls to the image box control first,
			// then we remove the old ones. Removing them first then adding them
			// results in flickering, which we don't want.
			AddImageBoxControls(_physicalWorkspace);

			foreach (Control control in oldControlList)
			{
				this.Controls.Remove(control);
				control.Dispose();
			}
		}

		private void UpdateScreenRectangle()
		{
			_physicalWorkspace.ScreenRectangle = this.RectangleToScreen(this.ClientRectangle);
		}

		private void AddImageBoxControls(PhysicalWorkspace physicalWorkspace)
		{
			foreach (ImageBox imageBox in physicalWorkspace.ImageBoxes)
				AddImageBoxControl(imageBox);
		}

		private void AddImageBoxControl(ImageBox imageBox)
		{
			ImageBoxView view = ViewFactory.CreateAssociatedView(typeof(ImageBox)) as ImageBoxView;

			view.ImageBox = imageBox;
			view.ParentRectangle = this.ClientRectangle;
			
			ImageBoxControl control = view.GuiElement as ImageBoxControl;

			control.SuspendLayout();
			this.Controls.Add(control);
			control.ResumeLayout(false);
		}

		private void SetParentForm(Form value) {
			if (_parentForm != value) {
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
