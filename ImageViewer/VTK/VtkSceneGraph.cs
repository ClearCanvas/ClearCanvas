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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Graphics3D;
using vtk;

namespace ClearCanvas.ImageViewer.Vtk
{
	/// <summary>
	/// Represents the 3D scene graph of an <see cref="IVtkPresentationImage"/>.
	/// </summary>
	/// <remarks>
	/// Since each instance of a VTK prop may only be used in a single rendering context at a time,
	/// this class manages all the VTK props used in a single rendering context. If an image needs to
	/// be rendered in multiple contexts at once, additional <see cref="VtkSceneGraph"/>s will be
	/// requested via the <see cref="IVtkPresentationImage.CreateSceneGraph"/> method.
	/// </remarks>
	public abstract class VtkSceneGraph : IDisposable
	{
		private VtkObjectsList _vtkObjectsList;

		/// <summary>
		/// Initializes a new <see cref="VtkSceneGraph"/>.
		/// </summary>
		/// <param name="owner">The owner <see cref="IVtkPresentationImage"/> of the scene graph.</param>
		protected VtkSceneGraph(IVtkPresentationImage owner)
		{
			Platform.CheckForNullReference(owner, "owner");
			Owner = owner;

			_vtkObjectsList = new VtkObjectsList();
		}

		~VtkSceneGraph()
		{
			try
			{
				Dispose(false);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex);
			}
		}

		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex);
			}
		}

		/// <summary>
		/// Called when the scene graph is disposed or finalized.
		/// </summary>
		/// <param name="disposing">True if the graph is being disposed; False otherwise.</param>
		protected virtual void Dispose(bool disposing)
		{
			Disposed = true;
			Owner = null;

			if (disposing && _vtkObjectsList != null)
			{
				_vtkObjectsList.Dispose();
				_vtkObjectsList = null;
			}
		}

		/// <summary>
		/// Gets a value indicating whether or not the scene graph was disposed.
		/// </summary>
		protected bool Disposed { get; private set; }

		/// <summary>
		/// Gets the owner image of the scene graph.
		/// </summary>
		protected IVtkPresentationImage Owner { get; private set; }

		/// <summary>
		/// Gets the 3D spatial transform of the model.
		/// </summary>
		public ISpatialTransform3D ModelSpatialTransform
		{
			get { return Owner.SpatialTransform3D; }
		}

		/// <summary>
		/// Gets the 2D spatial transform of the view port.
		/// </summary>
		public ISpatialTransform ViewPortSpatialTransform
		{
			get { return Owner.SpatialTransform; }
		}

		/// <summary>
		/// Called to setup and populate the VTK renderer with the parameters and model of the scene graph.
		/// </summary>
		/// <param name="vtkRenderer"></param>
		public abstract void InitializeSceneGraph(vtkRenderer vtkRenderer);

		/// <summary>
		/// Called just before each frame is rendered to update the VTK renderer with any changes in the scene graph.
		/// </summary>
		/// <param name="vtkRenderer"></param>
		public virtual void UpdateSceneGraph(vtkRenderer vtkRenderer) {}

		/// <summary>
		/// Called to take down and depopulate the VTK renderer.
		/// </summary>
		/// <param name="vtkRenderer"></param>
		public virtual void DeinitializeSceneGraph(vtkRenderer vtkRenderer) {}

		public override int GetHashCode()
		{
			return 0x3C979C9E ^ Owner.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is VtkSceneGraph && ReferenceEquals(Owner, ((VtkSceneGraph) obj).Owner);
		}

		public override string ToString()
		{
			return string.Format("VtkSceneGraph {{Type: {0}, Owner: {1}}}", GetType().FullName, Owner);
		}

		#region VTK Modification Time

		private int _modifiedTime;

		/// <summary>
		/// Gets a timestamp indicating the last time the VTK scene graph was modified.
		/// </summary>
		/// <returns></returns>
		public int GetMTime()
		{
			return _modifiedTime;
		}

		/// <summary>
		/// Updates the timestamp indicating that the VTK scene graph has been modified
		/// </summary>
		protected void Modified()
		{
			_modifiedTime = Environment.TickCount;
		}

		#endregion

		#region VTK Object Lifetime Helpers

		/// <summary>
		/// Creates and registers a new VTK object will be automatically disposed when the scene graph is disposed.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		protected T AddNewVtkObject<T>()
			where T : vtkObjectBase, new()
		{
			return AddNewVtkObject<T>(Guid.NewGuid().ToString());
		}

		/// <summary>
		/// Creates and registers a new VTK object will be automatically disposed when the scene graph is disposed.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="id"></param>
		/// <returns></returns>
		protected T AddNewVtkObject<T>(string id)
			where T : vtkObjectBase, new()
		{
			Platform.CheckFalse(_vtkObjectsList.ContainsKey(id), "The specified id is already in use.");
			var vtkObject = new T();
			_vtkObjectsList.Add(id, vtkObject);
			return vtkObject;
		}

		/// <summary>
		/// Gets a registered VTK object.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		protected vtkObjectBase GetVtkObject(string id)
		{
			return _vtkObjectsList[id];
		}

		/// <summary>
		/// Gets a registered VTK object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="id"></param>
		/// <returns></returns>
		protected T GetVtkObject<T>(string id)
			where T : vtkObjectBase
		{
			var vtkObject = _vtkObjectsList[id];
			return (T) vtkObject;
		}

		/// <summary>
		/// Unregisters and immediately disposes an existing registered VTK object.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		protected bool DeleteVtkObject(string id)
		{
			vtkObjectBase vtkObject;
			if (_vtkObjectsList.TryGetValue(id, out vtkObject))
			{
				_vtkObjectsList.Remove(id);
				vtkObject.Dispose();
				return true;
			}
			return false;
		}

		private class VtkObjectsList : Dictionary<string, vtkObjectBase>, IDisposable
		{
			public void Dispose()
			{
				foreach (var x in this.Reverse())
					x.Value.Dispose();
				Clear();
			}
		}

		#endregion
	}

	/// <summary>
	/// Represents the 3D scene graph of an <see cref="IVtkPresentationImage"/>.
	/// </summary>
	public abstract class VtkSceneGraph<T> : VtkSceneGraph
		where T : IVtkPresentationImage
	{
		/// <summary>
		/// Initializes a new <see cref="VtkSceneGraph"/>.
		/// </summary>
		/// <param name="owner">The owner <see cref="IVtkPresentationImage"/> of the scene graph.</param>
		protected VtkSceneGraph(T owner)
			: base(owner) {}

		/// <summary>
		/// Gets the owner image of the scene graph.
		/// </summary>
		protected new T Owner
		{
			get { return (T) base.Owner; }
		}
	}
}