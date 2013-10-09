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
using ClearCanvas.ImageViewer.Graphics3D;
using ClearCanvas.ImageViewer.Mathematics;
using vtk;

namespace ClearCanvas.ImageViewer.Vtk
{
	/// <summary>
	/// Basic implementation of the 3D scene graph of an <see cref="IVtkPresentationImage"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class BasicVtkSceneGraph<T> : VtkSceneGraph<T>
		where T : IVtkPresentationImage
	{
		private vtkProp3D _modelRootProp;
		private Color _backgroundColor = Color.Black;
		private uint _lastMTime;

		/// <summary>
		/// Initializes the <see cref="BasicVtkSceneGraph{T}"/>.
		/// </summary>
		/// <param name="owner"></param>
		protected BasicVtkSceneGraph(T owner)
			: base(owner) {}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_modelRootProp != null)
				{
					_modelRootProp.Dispose();
					_modelRootProp = null;
				}
			}
			base.Dispose(disposing);
		}

		/// <summary>
		/// Gets the <see cref="vtk.vtkProp3D"/> at the root of the scene graph.
		/// </summary>
		protected vtkProp3D ModelRootProp
		{
			get { return _modelRootProp ?? (_modelRootProp = CreateModelRootProp()); }
		}

		/// <summary>
		/// Gets or sets the background color of the scene graph.
		/// </summary>
		/// <remarks>
		/// Transparent colors are not supported. Values must have full opacity (i.e. A=255).
		/// </remarks>
		protected Color BackgroundColor
		{
			get { return _backgroundColor; }
			set
			{
				if (_backgroundColor == value) return;

				Platform.CheckTrue(_backgroundColor.A == 255, "Transparent background colors are not supported.");
				_backgroundColor = value;
				Modified();
			}
		}

		/// <summary>
		/// Called to create the <see cref="vtk.vtkProp3D"/> at the root of the scene graph.
		/// </summary>
		/// <returns></returns>
		protected abstract vtkProp3D CreateModelRootProp();

		/// <summary>
		/// Called just before the image is rendered to update the properties of the <see cref="vtk.vtkProp3D"/> at the root of the scene graph.
		/// </summary>
		/// <param name="modelRoot"></param>
		protected virtual void UpdateModelRootProp(vtkProp3D modelRoot) {}

		public override sealed void InitializeSceneGraph(vtkRenderer vtkRenderer)
		{
			if (Disposed) return;

			var rootProp = ModelRootProp;
			var origin = Owner.Origin ?? Vector3D.Null;
			rootProp.SetPosition(origin.X, origin.Y, origin.Z);

			// preinitialize the prop transform matrix
			using (var userMatrix = new vtkMatrix4x4())
				rootProp.SetUserMatrix(userMatrix);

			vtkRenderer.AddViewProp(rootProp);

			using (var renderWindow = vtkRenderer.GetRenderWindow())
			{
				var cameraDistance = GetNominalCameraDistance();
				var clientSize = renderWindow.GetSize();
				var camera = AddNewVtkObject<vtkCamera>();
				camera.ParallelProjectionOn();
				camera.SetParallelScale(0.5*clientSize[1]);
				camera.SetPosition(0, 0, cameraDistance);
				camera.SetFocalPoint(0, 0, 0);
				camera.SetClippingRange(10, 2*cameraDistance);
				camera.ComputeViewPlaneNormal();
				camera.SetViewUp(0, 1, 0);
				vtkRenderer.SetActiveCamera(camera);
			}
		}

		public override sealed void UpdateSceneGraph(vtkRenderer vtkRenderer)
		{
			if (Disposed) return;

			vtkRenderer.SetBackground(VtkHelper.ConvertToVtkColor(_backgroundColor));

			uint mTime;

			using (var camera = vtkRenderer.GetActiveCamera())
			using (var vtkRenderWindow = vtkRenderer.GetRenderWindow())
			{
				var clientSize = vtkRenderWindow.GetSize2();
				var scale = ViewPortSpatialTransform.Scale;
				if (FloatComparer.AreEqual(0, scale)) scale = 1;

				camera.SetParallelScale(0.5*clientSize.Height/scale);

				var cameraDistance = GetNominalCameraDistance();
				var pos = ViewPortSpatialTransform.ConvertToSource(new PointF(clientSize.Width/2f, clientSize.Height/2f));
				camera.SetPosition(pos.X, pos.Y, cameraDistance);
				camera.SetFocalPoint(pos.X, pos.Y, 0);
				camera.SetClippingRange(10, 2*cameraDistance);
				camera.ComputeViewPlaneNormal();

				var up = ViewPortSpatialTransform.ConvertToSource(new SizeF(0, -1));
				camera.SetViewUp(up.Width, up.Height, 0);

				mTime = camera.GetMTime();
			}

			var rootProp = ModelRootProp;
			using (var vtkTransform = rootProp.GetUserMatrix())
			{
				var modelCumulativeTransform = GetModelCumulativeTransform();
				vtkTransform.SetElements(modelCumulativeTransform);
			}

			UpdateModelRootProp(rootProp);

			var propMTime = rootProp.GetMTime();
			if (mTime < propMTime)
				mTime = propMTime;

			if (_lastMTime < mTime)
			{
				_lastMTime = mTime;
				Modified();
			}
		}

		private double GetNominalCameraDistance()
		{
			// determine a suitable distance from the camera where the model will be positioned
			const int padding = 10; // extra padding to ensure model won't be clipped by the backplane of the scene
			return Math.Max(100, (int) Owner.Dimensions.Magnitude + padding);
		}

		public override sealed void DeinitializeSceneGraph(vtkRenderer vtkRenderer)
		{
			if (Disposed) return;

			base.DeinitializeSceneGraph(vtkRenderer);
		}

		private Matrix GetModelCumulativeTransform()
		{
			var modelSpatialTransform = ModelSpatialTransform as SpatialTransform3D;
			if (modelSpatialTransform == null)
			{
				Platform.Log(LogLevel.Debug, "Unsupported model spatial transform type");
				return Matrix.GetIdentity(4);
			}
			return modelSpatialTransform.CumulativeTransform.Clone();
		}

		private IList<IGraphic3D> ListPrimitives(IEnumerable<IGraphic3D> collection, IList<IGraphic3D> primitives = null)
		{
			if (primitives == null) primitives = new List<IGraphic3D>();
			if (collection != null)
			{
				foreach (var graphic in collection)
				{
					if (graphic is CompositeGraphic3D)
						ListPrimitives(((CompositeGraphic3D) graphic).Graphics, primitives);
					else
						primitives.Add(graphic);
				}
			}
			return primitives;
		}
	}
}