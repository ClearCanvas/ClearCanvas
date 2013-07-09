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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Volume.Mpr.Utilities;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Tools
{
	[DropDownButtonAction("dropdown", "global-toolbars/ToolbarMpr/ToolbarReslice", "ActivateLastSelected", "DropDownModel")]
	[MouseButtonIconSet("dropdown", "Icons.ResliceToolSmall.png", "Icons.ResliceToolMedium.png", "Icons.ResliceToolLarge.png")]
	[CheckedStateObserver("dropdown", "Active", "ActivationChanged")]
	[EnabledStateObserver("dropdown", "Visible", "VisibleChanged")]
	[VisibleStateObserver("dropdown", "Visible", "VisibleChanged")]
	[LabelValueObserver("dropdown", "Label", "SelectedToolChanged")]
	[TooltipValueObserver("dropdown", "Tooltip", "TooltipChanged")]
	[GroupHint("dropdown", "Tools.Volume.MPR.Reslicing")]
	[MouseToolButton(XMouseButtons.Left, false)]
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public partial class ResliceToolGroup : MouseImageViewerToolGroup<MprViewerTool>
	{
		private static readonly Color[,] _colors = {
		                                           	{Color.Red, Color.Salmon},
		                                           	{Color.DodgerBlue, Color.DeepSkyBlue},
		                                           	{Color.GreenYellow, Color.Lime},
		                                           	{Color.Yellow, Color.Gold},
		                                           	{Color.Magenta, Color.Violet},
		                                           	{Color.Aqua, Color.Turquoise},
		                                           	{Color.White, Color.LightGray}
		                                           };

		private ResliceTool _lastSelectedTool;
		private bool _visible = true;

		public bool Visible
		{
			get { return _visible; }
		}

		public event EventHandler VisibleChanged
		{
			add { }
			remove { }
		}

		protected override IEnumerable<MprViewerTool> CreateTools()
		{
			int index = 0;

			if (this.ImageViewer == null)
				yield break;

			// create one instance of the slave tool for each mutable slice set
			foreach (IMprVolume volume in this.ImageViewer.Volumes)
			{
				foreach (IMprSliceSet sliceSet in volume.SliceSets)
				{
					IMprStandardSliceSet standardSliceSet = sliceSet as IMprStandardSliceSet;
					if (standardSliceSet != null && !standardSliceSet.IsReadOnly)
					{
						ResliceTool tool = new ResliceTool(this);
						tool.SliceSet = standardSliceSet;
						tool.HotColor = _colors[index, 0];
						tool.NormalColor = _colors[index, 1];
						index = (index + 1)%_colors.Length; // advance to next color
						yield return tool;
					}
				}
			}
		}

		public new MprViewerComponent ImageViewer
		{
			get { return base.ImageViewer as MprViewerComponent; }
		}

		public override void Initialize()
		{
			base.Initialize();
			base.TooltipPrefix = SR.MenuReslice;

			_visible = this.ImageViewer != null;
			_resliceToolGroupState = this.ImageViewer != null ? new ResliceToolGroupState(this) : null;

			this.InitializeResetAll();
			if (this.ImageViewer != null)
			{
				this.ImageViewer.PhysicalWorkspace.LayoutCompleted += PhysicalWorkspace_LayoutCompleted;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.ImageViewer != null)
				{
					this.ImageViewer.PhysicalWorkspace.LayoutCompleted -= PhysicalWorkspace_LayoutCompleted;
				}

				if (_resliceToolGroupState != null)
				{
					_resliceToolGroupState.Dispose();
					_resliceToolGroupState = null;
				}
			}
			this.DisposeResetAll();
			base.Dispose(disposing);
		}

		private void PhysicalWorkspace_LayoutCompleted(object sender, EventArgs e)
		{
			this.ReinitializeTools();
		}

		protected override void OnToolSelected(MprViewerTool tool)
		{
			base.OnToolSelected(tool);
			_lastSelectedTool = (ResliceTool) tool;

			if (_lastSelectedTool == null)
				base.TooltipPrefix = SR.ToolbarReslice;
			else
				base.TooltipPrefix = _lastSelectedTool.Label;
		}

		public void ActivateLastSelected()
		{
			IList<MprViewerTool> tools = base.SlaveTools;
			if (tools == null || tools.Count == 0)
				return;
			if (_lastSelectedTool == null)
				tools[0].Select();
			else
				_lastSelectedTool.Select();
		}

		public string Label
		{
			get
			{
				if (_lastSelectedTool == null)
					return SR.ToolbarReslice;
				return _lastSelectedTool.Label;
			}
		}

		public ActionModelNode DropDownModel
		{
			get { return ActionModelRoot.CreateModel(this.GetType().FullName, "mprviewer-reslicemenu", this.Actions); }
		}

		#region Reslice Tool Group State

		private ResliceToolGroupState _resliceToolGroupState;

		public IMemorable ToolGroupState
		{
			get { return _resliceToolGroupState; }
		}

		public object InitialToolGroupStateMemento
		{
			get { return _resliceToolGroupState.InitialState; }
		}

		private class ResliceToolGroupState : IMemorable, IDisposable
		{
			private ResliceToolGroup _owner;
			private object _initialState;

			public ResliceToolGroupState(ResliceToolGroup owner)
			{
				_owner = owner;
				_initialState = this.CreateMemento();
			}

			public void Dispose()
			{
				_owner = null;
				_initialState = null;
			}

			public object InitialState
			{
				get { return _initialState; }
			}

			public object CreateMemento()
			{
				ResliceToolGroupStateMemento state = new ResliceToolGroupStateMemento();

				foreach (IImageSet imageSet in _owner.ImageViewer.MprWorkspace.ImageSets)
				{
					foreach (MprDisplaySet displaySet in imageSet.DisplaySets)
						state.SlicingStates.Add(displaySet, displaySet.CreateMemento());
				}

				foreach (ResliceTool tool in _owner.SlaveTools)
				{
					if (tool.SliceSet != null)
						state.ToolStates.Add(tool, tool.CreateMemento());
				}

				return state;
			}

			public void SetMemento(object memento)
			{
				ResliceToolGroupStateMemento state = memento as ResliceToolGroupStateMemento;
				if (state == null)
					return;

				foreach (KeyValuePair<MprDisplaySet, object> pair in state.SlicingStates)
				{
					pair.Key.SetMemento(pair.Value);
				}

				foreach (ResliceTool tool in _owner.SlaveTools)
				{
					if (tool.SliceSet != null && state.ToolStates.ContainsKey(tool))
						tool.SetMemento(state.ToolStates[tool]);
				}
			}

			private class ResliceToolGroupStateMemento
			{
				public readonly Dictionary<ResliceTool, object> ToolStates = new Dictionary<ResliceTool, object>();
				public readonly Dictionary<MprDisplaySet, object> SlicingStates = new Dictionary<MprDisplaySet, object>();
			}
		}

		private class ImageHint
		{
			private readonly int _imageIndex = -1;
			private readonly MprDisplaySet _displaySet = null;

			public ImageHint(IPresentationImage image)
			{
				if (image != null)
				{
					_displaySet = image.ParentDisplaySet as MprDisplaySet;
					if (_displaySet != null)
						_imageIndex = _displaySet.PresentationImages.IndexOf(image);
				}
			}

			public IPresentationImage Image
			{
				get
				{
					if (_displaySet != null && _imageIndex >= 0 && _imageIndex < _displaySet.PresentationImages.Count)
						return _displaySet.PresentationImages[_imageIndex];
					return null;
				}
			}
		}

		#endregion

		#region Static Helpers - Finding ImageBoxes

		/// <summary>
		/// Finds the ImageBox displaying the specified slice set
		/// </summary>
		protected static IImageBox FindImageBox(IMprSliceSet sliceSet, MprViewerComponent viewer)
		{
			if (sliceSet == null || viewer == null)
				return null;

			foreach (IImageBox imageBox in viewer.PhysicalWorkspace.ImageBoxes)
			{
				if (imageBox.DisplaySet != null && imageBox.DisplaySet.Uid == sliceSet.Uid)
					return imageBox;
			}
			return null;
		}

		#endregion

		#region Static Helpers - Colourising Annotation Items

		/// <summary>
		/// Colourises the display set description annotation item in the specified image
		/// </summary>
		private static void ColorizeDisplaySetDescription(IPresentationImage image, Color color)
		{
			if (image is IAnnotationLayoutProvider)
			{
				IAnnotationLayoutProvider provider = (IAnnotationLayoutProvider) image;
				foreach (AnnotationBox annotationBox in provider.AnnotationLayout.AnnotationBoxes)
				{
					if (annotationBox.AnnotationItem != null && annotationBox.AnnotationItem.GetIdentifier() == "Presentation.DisplaySetDescription")
					{
						annotationBox.Color = color.Name;
					}
				}
			}
		}

		#endregion

		#region Static Helpers - Manipulating Graphics

		/// <summary>
		/// Moves the graphic from where ever it is to the target image.
		/// </summary>
		private static void TranslocateGraphic(IGraphic graphic, IPresentationImage targetImage)
		{
			IPresentationImage oldImage = graphic.ParentPresentationImage;
			if (oldImage != targetImage)
			{
				IApplicationGraphicsProvider imageOnUnexecute = oldImage as IApplicationGraphicsProvider;
				if (imageOnUnexecute != null)
					imageOnUnexecute.ApplicationGraphics.Remove(graphic);

				IApplicationGraphicsProvider imageOnExecute = targetImage as IApplicationGraphicsProvider;
				if (imageOnExecute != null)
					imageOnExecute.ApplicationGraphics.Add(graphic);

				if (oldImage != null)
					oldImage.Draw();
			}
		}

		#endregion

		#region Static Helpers - Reslicing Math

		/// <summary>
		/// Sets the slicing plane for the specified slice set based on two points on the specified source image.
		/// </summary>
		private static void SetSlicePlane(IMprStandardSliceSet sliceSet, IPresentationImage sourceImage, Vector3D startPoint, Vector3D endPoint)
		{
			IImageSopProvider imageSopProvider = sourceImage as IImageSopProvider;
			if (imageSopProvider == null)
				return;

			ImageOrientationPatient orientation = imageSopProvider.Frame.ImageOrientationPatient;
			Vector3D orientationRow = new Vector3D((float) orientation.RowX, (float) orientation.RowY, (float) orientation.RowZ);
			Vector3D orientationColumn = new Vector3D((float) orientation.ColumnX, (float) orientation.ColumnY, (float) orientation.ColumnZ);

			if (sliceSet != null && !sliceSet.IsReadOnly)
			{
				IImageBox imageBox = FindImageBox(sliceSet, sourceImage.ImageViewer as MprViewerComponent);
				sliceSet.SlicerParams = VolumeSlicerParams.Create(sliceSet.VolumeHeader, orientationColumn, orientationRow, startPoint, endPoint);

				IPresentationImage closestImage = GetClosestSlice(startPoint + (endPoint - startPoint) * 2, imageBox.DisplaySet);
				if (closestImage == null)
					imageBox.TopLeftPresentationImageIndex = imageBox.DisplaySet.PresentationImages.Count/2;
				else
					imageBox.TopLeftPresentationImage = closestImage;
			}
		}

		/// <summary>
		/// Computes the closest image in a display set to the specified position in patient coordinates.
		/// </summary>
		private static IPresentationImage GetClosestSlice(Vector3D positionPatient, IDisplaySet displaySet)
		{
			float closestDistance = float.MaxValue;
			IPresentationImage closestImage = null;

			foreach (IPresentationImage image in displaySet.PresentationImages)
			{
				if (image is IImageSopProvider)
				{
					Frame frame = (image as IImageSopProvider).Frame;
					Vector3D positionCenterOfImage = frame.ImagePlaneHelper.ConvertToPatient(new PointF((frame.Columns - 1)/2F, (frame.Rows - 1)/2F));
					Vector3D distanceVector = positionCenterOfImage - positionPatient;
					float distance = distanceVector.Magnitude;

					if (distance <= closestDistance)
					{
						closestDistance = distance;
						closestImage = image;
					}
				}
			}

			return closestImage;
		}

		#endregion
	}
}