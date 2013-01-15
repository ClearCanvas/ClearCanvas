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
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.ImageViewer.Utilities.StudyComposer.Adapters.SimpleComposerAdapter;

namespace ClearCanvas.ImageViewer.Utilities.StudyComposer.View.WinForms {
	public partial class SimpleComposerAdapterComponentPanel : UserControl {
		private readonly SimpleComposerAdapterComponent _component;
		private ToolStripMenuItem _lastPublishItem;

		public SimpleComposerAdapterComponentPanel() {
			InitializeComponent();

			_lastPublishItem = mnuPublishLocal;
			_lastPublishItem.Checked = true;
		}

		public SimpleComposerAdapterComponentPanel(SimpleComposerAdapterComponent component)
			: this() {
			Size size = new Size(64, 64);

			_component = component;
			_component.RefreshIcons(size);

			Control ctlPatient = (Control)_component.PatientGalleryComponentHost.ComponentView.GuiElement;
			ctlPatient.Dock = DockStyle.Fill;
			pnlPatients.Controls.Add(ctlPatient);

			Control ctlStudy = (Control)_component.StudyGalleryComponentHost.ComponentView.GuiElement;
			ctlStudy.Dock = DockStyle.Fill;
			pnlStudies.Controls.Add(ctlStudy);

			Control ctlSeries = (Control)_component.SeriesGalleryComponentHost.ComponentView.GuiElement;
			ctlSeries.Dock = DockStyle.Fill;
			pnlSeries.Controls.Add(ctlSeries);

			Control ctlImage = (Control)_component.ImageGalleryComponentHost.ComponentView.GuiElement;
			ctlImage.Dock = DockStyle.Fill;
			pnlImages.Controls.Add(ctlImage);

			//glvPatients.ImageSize = size;
			//glvStudies.ImageSize = size;
			//glvSeries.ImageSize = size;
			//glvImages.ImageSize = size;

			//glvPatients.DataSource = _component.Patients;
			//glvStudies.DataSource = _component.Studies;
			//glvSeries.DataSource = _component.Series;
			//glvImages.DataSource = _component.Images;

			//_component.StudyTreeUpdated += new EventHandler(_component_StudyTreeUpdated);
			//_component.SelectedPatientChanged += new EventHandler(_component_SelectedPatientChanged);
			//_component.SelectedStudyChanged += new EventHandler(_component_SelectedStudyChanged);
			//_component.SelectedSeriesChanged += new EventHandler(_component_SelectedSeriesChanged);
		}

		private void btnRefresh_Click(object sender, EventArgs e) {
			_component.RefreshIcons(new Size(64, 64));
		}

		private void btnPublish_Click(object sender, EventArgs e) {
			_lastPublishItem.PerformClick();
		}

		private void btnPublishDropDown_Click(object sender, EventArgs e) {
			Point p = new Point(btnPublishDropDown.Left, btnPublishDropDown.Top + btnPublishDropDown.Height);
			mnuPublish.Show(xbtnPublish.PointToScreen( p));
		}

		private void mnuPublishLocal_Click(object sender, EventArgs e) {
			_lastPublishItem = mnuPublishLocal;
			mnuPublishFolder.Checked = mnuPublishRemote.Checked = !(mnuPublishLocal.Checked = true);

			this.Cursor = Cursors.WaitCursor;
			_component.PublishToLocalDataStore();
			this.Cursor = Cursors.Default ;
		}

		private void mnuPublishRemote_Click(object sender, EventArgs e) {
			_lastPublishItem = mnuPublishLocal;
			mnuPublishFolder.Checked = mnuPublishLocal.Checked = !(mnuPublishRemote.Checked = true);

			this.Cursor = Cursors.WaitCursor;
			_component.PublishToServer();
			this.Cursor = Cursors.Default;
		}

		private void mnuPublishFolder_Click(object sender, EventArgs e) {
			_lastPublishItem = mnuPublishLocal;
			mnuPublishLocal.Checked = mnuPublishRemote.Checked = !(mnuPublishFolder.Checked = true);

			if (dlgExport.ShowDialog() == DialogResult.OK) {
				this.Cursor = Cursors.WaitCursor;
				_component.Export(dlgExport.SelectedPath);
				this.Cursor = Cursors.Default;
			}
		}

		//private void glvPatients_SelectionChanged(object sender, EventArgs e) {
		//    if (glvPatients.Selection.Item != null)
		//        _component.SelectedPatient = (PatientItem)glvPatients.Selection.Item;
		//}

		//private void glvStudies_SelectionChanged(object sender, EventArgs e) {
		//    if (glvStudies.Selection.Item != null)
		//        _component.SelectedStudy = (StudyItem)glvStudies.Selection.Item;
		//}

		//private void glvSeries_SelectionChanged(object sender, EventArgs e) {
		//    if (glvSeries.Selection.Item != null)
		//        _component.SelectedSeries = (SeriesItem)glvSeries.Selection.Item;
		//}

		//private void glvImages_SelectionChanged(object sender, EventArgs e) {
		//    if (glvImages.Selection.Item != null)
		//        _component.SelectedImage = (ImageItem)glvImages.Selection.Item;
		//}

		//private bool CheckAllowedMove(IDataObject data, object sender)
		//{
		//    bool allowed = false;
		//    if (data.GetDataPresent(typeof(IStudyComposerItem))) {
		//        IStudyComposerItem item = (IStudyComposerItem)data.GetData(typeof(IStudyComposerItem));
		//        if (item is ImageItem) {
		//            allowed = (sender == glvPatients || sender == glvStudies || sender == glvSeries);
		//        } else if (item is SeriesItem) {
		//            allowed = (sender == glvPatients || sender == glvStudies);
		//        } else if (item is StudyItem) {
		//            allowed = (sender == glvPatients);
		//        }
		//    }
		//    return allowed;
		//}

		//private IStudyComposerItem GetItemAtPoint(Control reference, int x, int y)
		//{
		//    Point p = reference.PointToClient(new Point(x, y));
		//    glvSeries.
		//}

		//private void OnGalleryItemDragOver(object sender, DragEventArgs e)
		//{
		//    if (CheckAllowedMove(e.Data, sender)) {
		//        if (((KeyState)e.KeyState & KeyState.Shift) == KeyState.Shift) {
		//            e.Effect = e.AllowedEffect & DragDropEffects.Move;
		//        } else if (((KeyState)e.KeyState & KeyState.Shift) == KeyState.Shift) {
		//            e.Effect = e.AllowedEffect & DragDropEffects.Copy;
		//        }
		//    }
		//}

		//private void OnGalleryItemDragDrop(object sender, DragEventArgs e) {
		//    //if(CheckAllowedMove(e.Data, sender))
		//    //{
		//    //    IStudyComposerItem item = (IStudyComposerItem) e.Data.GetData(typeof (IStudyComposerItem));
		//    //    IStudyComposerItem target = GetItemAtPoint((Control)sender, e.X, e.Y);
		//    //    if(e.Effect == DragDropEffects.Move)
		//    //    {
					
		//    //    } else if(e.Effect == DragDropEffects.Copy)
		//    //    {
		//    //        item = item.Clone();
		//    //    }
		//    //}
		//    //glvSeries.
		//}

		//[Flags]
		//private enum KeyState : int {
		//    LeftButton = 1,
		//    RightButton = 2,
		//    Shift = 4,
		//    Ctrl = 8,
		//    MiddleButton = 16,
		//    Alt = 32
		//}

		//private void panel1_Paint(object sender, PaintEventArgs e) {

		//}
	}
}
