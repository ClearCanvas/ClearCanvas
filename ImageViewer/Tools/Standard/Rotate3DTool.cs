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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics3D;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.Tools.Standard.Configuration;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[ViewerActionPermission("activate", AuthorityTokens.ViewerClinical)]
	[MenuAction("activate", "imageviewer-contextmenu/MenuRotate3D", "Select", Flags = ClickActionFlags.CheckAction)]
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuRotate3D", "Select", Flags = ClickActionFlags.CheckAction)]
	[DropDownButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarRotate3D", "Select", "DropDownMenuModel", Flags = ClickActionFlags.CheckAction)]
	[CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[MouseButtonIconSet("activate", "Icons.Rotate3DToolSmall.png", "Icons.Rotate3DToolMedium.png", "Icons.Rotate3DToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Manipulation.Orientation.3D.Rotate")]
	//
	[MenuAction("axial", _dropDownMenuActionSite + "/MenuAxialView", "ViewAxial")]
	[MenuAction("coronal", _dropDownMenuActionSite + "/MenuCoronalView", "ViewCoronal")]
	[MenuAction("sagittal", _dropDownMenuActionSite + "/MenuSagittalView", "ViewSagittal")]
	[MenuAction("reset", _dropDownMenuActionSite + "/MenuReset", "RotateReset")]
	//
	[KeyboardAction("rotatereset", "imageviewer-keyboard/ToolsStandardRotate/RotateReset", "RotateReset", KeyStroke = XKeys.NumPad5)]
	[KeyboardAction("rotateleft", "imageviewer-keyboard/ToolsStandardRotate/RotateLeft", "RotateLeft", KeyStroke = XKeys.NumPad4)]
	[KeyboardAction("rotateright", "imageviewer-keyboard/ToolsStandardRotate/RotateRight", "RotateRight", KeyStroke = XKeys.NumPad6)]
	[KeyboardAction("rotateup", "imageviewer-keyboard/ToolsStandardRotate/RotateUp", "RotateUp", KeyStroke = XKeys.NumPad8)]
	[KeyboardAction("rotatedown", "imageviewer-keyboard/ToolsStandardRotate/RotateDown", "RotateDown", KeyStroke = XKeys.NumPad2)]
	//
	[MouseToolButton(XMouseButtons.Left, false)]
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint), Enabled = false)]
	public class Rotate3DTool : MouseImageViewerTool
	{
		private const string _dropDownMenuActionSite = "rotate3d-dropdown";
		private readonly SpatialTransform3DImageOperation _operation;
		private ActionModelRoot _dropDownMenuModel;
		private MemorableUndoableCommand _memorableCommand;
		private ImageOperationApplicator _applicator;
		private ToolModalityBehaviorHelper _toolBehavior;
		private FlashOverlayController _flashOverlayController;

		public Rotate3DTool()
			: base(SR.TooltipRotate3D)
		{
			CursorToken = new CursorToken("Icons.Rotate3DToolSmall.png", GetType().Assembly);
			_operation = new SpatialTransform3DImageOperation(Apply);

			const string graphicName = "Icons.NoSpineLabeling.png";
			var iconSet = new UnavailableActionIconSet(new IconSet("Icons.Rotate3DToolSmall.png", "Icons.Rotate3DToolMedium.png", "Icons.Rotate3DToolLarge.png"));
			var resolver = new ApplicationThemeResourceResolver(GetType(), false);
			_flashOverlayController = new FlashOverlayController(iconSet, resolver) {FlashSpeed = 1500};
		}

		public override void Initialize()
		{
			base.Initialize();

			_toolBehavior = new ToolModalityBehaviorHelper(ImageViewer);
		}

		public override event EventHandler TooltipChanged
		{
			add { base.TooltipChanged += value; }
			remove { base.TooltipChanged -= value; }
		}

		public ActionModelNode DropDownMenuModel
		{
			get { return _dropDownMenuModel ?? (_dropDownMenuModel = ActionModelRoot.CreateModel(GetType().FullName, _dropDownMenuActionSite, Actions)); }
		}

		private ISpatialTransform3D GetSelectedImageTransform()
		{
			return _operation.GetOriginator(SelectedPresentationImage) as ISpatialTransform3D;
		}

		private bool CanRotate()
		{
			return GetSelectedImageTransform() != null;
		}

		private void CaptureBeginState()
		{
			if (!CanRotate())
				return;

			_applicator = new ImageOperationApplicator(SelectedPresentationImage, _operation);
			var originator = GetSelectedImageTransform();
			_memorableCommand = new MemorableUndoableCommand(originator);
			_memorableCommand.BeginState = originator.CreateMemento();
		}

		private void CaptureEndState()
		{
			if (!CanRotate() || _memorableCommand == null)
				return;

			_memorableCommand.EndState = GetSelectedImageTransform().CreateMemento();
			UndoableCommand applicatorCommand = _toolBehavior.Behavior.SelectedImageRotate3DTool ? null : _applicator.ApplyToLinkedImages();
			DrawableUndoableCommand historyCommand = new DrawableUndoableCommand(SelectedPresentationImage);

			if (!_memorableCommand.EndState.Equals(_memorableCommand.BeginState))
				historyCommand.Enqueue(_memorableCommand);
			if (applicatorCommand != null)
				historyCommand.Enqueue(applicatorCommand);

			if (historyCommand.Count > 0)
			{
				historyCommand.Name = SR.CommandRotate3D;
				Context.Viewer.CommandHistory.AddCommand(historyCommand);
			}

			_memorableCommand = null;
		}

		private void ViewAxial()
		{
			SetPatientOrientationWithUndo(Vector3D.xUnit, Vector3D.yUnit);
		}

		private void ViewCoronal()
		{
			SetPatientOrientationWithUndo(Vector3D.xUnit, -Vector3D.zUnit);
		}

		private void ViewSagittal()
		{
			SetPatientOrientationWithUndo(Vector3D.yUnit, -Vector3D.zUnit);
		}

		private void SetPatientOrientationWithUndo(Vector3D rowDirectionPatient, Vector3D columnDirectionPatient)
		{
			if (!CanRotate())
				return;

			try
			{
				CaptureBeginState();
				SetPatientOrientation(rowDirectionPatient, columnDirectionPatient);
				CaptureEndState();
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Warn, ex, "Error performing requested rotation");
			}
		}

		private void SetPatientOrientation(Vector3D rowDirectionPatient, Vector3D columnDirectionPatient)
		{
			if (!CanRotate())
				return;

			if (rowDirectionPatient == null || columnDirectionPatient == null || !rowDirectionPatient.IsOrthogonalTo(columnDirectionPatient, (float) (5/180d*Math.PI)))
				return;

			var patientPresentation = SelectedPresentationImage as IPatientPresentationProvider;
			if (patientPresentation == null || !patientPresentation.PatientPresentation.IsValid)
				return;

			// Note the inverted column orientation vectors in both matrices - this is due to implicit Y axis inversion in the 3D transform
			columnDirectionPatient = -columnDirectionPatient;

			var currentRowOrientation = patientPresentation.PatientPresentation.OrientationX;
			var currentColumnOrientation = -patientPresentation.PatientPresentation.OrientationY;
			var currentOrientation = Matrix3D.FromRows(currentRowOrientation.Normalize(), currentColumnOrientation.Normalize(), currentRowOrientation.Cross(currentColumnOrientation).Normalize());
			var requestedOrientation = Matrix3D.FromRows(rowDirectionPatient.Normalize(), columnDirectionPatient.Normalize(), rowDirectionPatient.Cross(columnDirectionPatient).Normalize());

			var transform = _operation.GetOriginator(SelectedPresentationImage);

			// (because we're dealing with rotation matrices (i.e. orthogonal!), the Inverse is just Transpose)
			var rotation = requestedOrientation*currentOrientation.Transpose();
			transform.Rotation = rotation*transform.Rotation; // this rotation is cumulative upon current rotation, since IPatientPresentationProvider is based on *current* view

			SelectedPresentationImage.Draw();
		}

		private void RotateReset()
		{
			ResetRotateWithUndo();
		}

		private void RotateLeft()
		{
			IncrementRotateWithUndo(-15, 0);
		}

		private void RotateRight()
		{
			IncrementRotateWithUndo(15, 0);
		}

		private void RotateUp()
		{
			IncrementRotateWithUndo(0, -15);
		}

		private void RotateDown()
		{
			IncrementRotateWithUndo(0, 15);
		}

		private void ResetRotateWithUndo()
		{
			if (!CanRotate())
				return;

			CaptureBeginState();
			ResetRotate();
			CaptureEndState();
		}

		private void ResetRotate()
		{
			if (!CanRotate())
				return;

			var transform = (ISpatialTransform3D) _operation.GetOriginator(SelectedPresentationImage);
			transform.Rotation = Matrix3D.GetIdentity();

			SelectedPresentationImage.Draw();
		}

		private void IncrementRotateWithUndo(int xIncrement, int yIncrement)
		{
			if (!CanRotate())
				return;

			try
			{
				CaptureBeginState();
				IncrementRotate(xIncrement, yIncrement);
				CaptureEndState();
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Warn, ex, "Error performing requested rotation");
			}
		}

		private void IncrementRotate(int xIncrement, int yIncrement)
		{
			if (!CanRotate())
				return;

			if (xIncrement == 0 && yIncrement == 0)
				return;

			var transform = (ISpatialTransform3D) _operation.GetOriginator(SelectedPresentationImage);

			// Because the rotation increment is in destination coordinates, we have to convert
			// them to source coordinates, since the transform translation is in source coordinates.
			// This will allow the rotate to work properly irrespective of the zoom, flip and rotation.
			var source = SelectedSpatialTransformProvider.SpatialTransform.ConvertToSource(new SizeF(xIncrement, yIncrement));
			var sourceX = source.Width;
			var sourceY = source.Height;

			// choose the axis of rotation to be perpendicular to the rotation increment, in the plane of the view port
			var axis = new Vector3D(-sourceY, sourceX, 0).Normalize();

			// compute the magnitude of the rotation
			var angle = (float) Math.Sqrt(sourceX*sourceX + sourceY*sourceY);

			var rotation = transform.Rotation != null ? transform.Rotation.Clone() : Matrix3D.GetIdentity();
			Rotate(rotation, angle, axis.X, axis.Y, axis.Z);
			transform.Rotation = rotation;

			SelectedPresentationImage.Draw();
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			if (!CanRotate())
				_flashOverlayController.Flash(SelectedPresentationImage, SR.Message3DOnly);

			base.Start(mouseInformation);

			CaptureBeginState();

			return true;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			base.Track(mouseInformation);

			IncrementRotate(DeltaX, DeltaY);

			return true;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			base.Stop(mouseInformation);

			CaptureEndState();

			return false;
		}

		public override void Cancel()
		{
			CaptureEndState();
		}

		private ISpatialTransform3DProvider SelectedSpatialTransform3DProvider
		{
			get { return SelectedPresentationImage as ISpatialTransform3DProvider; }
		}

		public void Apply(IPresentationImage image)
		{
			var transform = (ISpatialTransform3D) _operation.GetOriginator(image);
			var rotation = SelectedSpatialTransform3DProvider.SpatialTransform3D.Rotation;
			transform.Rotation = rotation != null ? rotation.Clone() : null;
		}

		private static void Rotate(Matrix3D transform, float angle, float x, float y, float z)
		{
			const double rads = Math.PI/180;

			var c0 = Math.Cos(rads*angle);
			var c1 = 1 - c0;
			var xc1 = x*c1;
			var yc1 = y*c1;
			var zc1 = z*c1;

			var s = Math.Sin(rads*angle);
			var xs = x*s;
			var ys = y*s;
			var zs = z*s;

			var r = new Matrix3D(new[,]
			                     	{
			                     		{(float) (x*xc1 + c0), (float) (x*yc1 - zs), (float) (x*zc1 + ys)},
			                     		{(float) (y*xc1 + zs), (float) (y*yc1 + c0), (float) (y*zc1 - xs)},
			                     		{(float) (z*xc1 - ys), (float) (z*yc1 + xs), (float) (z*zc1 + c0)}
			                     	});

			var m = r*transform;
			transform.SetRow(0, m[0, 0], m[0, 1], m[0, 2]);
			transform.SetRow(1, m[1, 0], m[1, 1], m[1, 2]);
			transform.SetRow(2, m[2, 0], m[2, 1], m[2, 2]);
		}
	}
}