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
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	/// <summary>
	/// Represents a collection of available display shutters.
	/// </summary>
	public interface IDicomGraphicsPlaneShutters : IList<IShutterGraphic>
	{
		/// <summary>
		/// Activates the specified display shutter graphic.
		/// </summary>
		/// <param name="shutter">The display shutter to activate. This shutter must be a part of the collection.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="shutter"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="shutter"/> is not part of the collection.</exception>
		void Activate(IShutterGraphic shutter);

		/// <summary>
		/// Activates the specified display shutter graphic.
		/// </summary>
		/// <param name="index">The index of the display shutter to activate.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the index is out of range.</exception>
		void Activate(int index);

		/// <summary>
		/// Activates the first display shutter graphic in the collection.
		/// </summary>
		void ActivateFirst();

		/// <summary>
		/// Deactivates the specified display shutter graphic.
		/// </summary>
		/// <param name="shutter">The display shutter to deactivate. This shutter must be a part of the collection.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="shutter"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="shutter"/> is not part of the collection.</exception>
		void Deactivate(IShutterGraphic shutter);

		/// <summary>
		/// Deactivates the specified display shutter graphic.
		/// </summary>
		/// <param name="index">The index of the display shutter to deactivate.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the index is out of range.</exception>
		void Deactivate(int index);

		/// <summary>
		/// Deactivates any and all active display shutter graphics.
		/// </summary>
		void DeactivateAll();

		/// <summary>
		/// Gets the currently activate display shutter graphic.
		/// </summary>
		IShutterGraphic ActiveShutter { get; }

		/// <summary>
		/// Gets the display shutter graphic at the specified index.
		/// </summary>
		/// <param name="index">The index of the display shutter to deactivate.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the index is out of range.</exception>
		new IShutterGraphic this[int index] { get; }

		/// <summary>
		/// Enables or disables all display shutters.
		/// </summary>
		bool Enabled { get; set; }
	}

	public partial class DicomGraphicsPlane
	{
		[Cloneable(true)]
		private class ShutterCollection : CompositeGraphic, IDicomGraphicsPlaneShutters, IEnumerable<IGraphic>
		{
			public bool Enabled
			{
				get { return base.Visible; }
				set { base.Visible = value; }
			}

			public IShutterGraphic ActiveShutter
			{
				get { return CollectionUtils.SelectFirst(base.Graphics, delegate(IGraphic g) { return g.Visible; }) as IShutterGraphic; }
			}

			public IShutterGraphic this[int index]
			{
				get { return (IShutterGraphic) base.Graphics[index]; }
			}

			public int Count
			{
				get { return base.Graphics.Count; }
			}

			public void Activate(IShutterGraphic shutter)
			{
				Platform.CheckTrue(base.Graphics.Contains(shutter), "Shutter must be part of the collection.");
				foreach (IShutterGraphic graphic in this)
					graphic.Visible = (graphic == shutter);
			}

			public void Activate(int index)
			{
				Platform.CheckArgumentRange(index, 0, base.Graphics.Count - 1, "index");
				for (int n = 0; n < base.Graphics.Count; n++)
					base.Graphics[n].Visible = (n == index);
			}

			public void ActivateFirst()
			{
				if (base.Graphics.Count > 0)
					this.Activate(0);
			}

			public void Deactivate(IShutterGraphic shutter)
			{
				Platform.CheckTrue(base.Graphics.Contains(shutter), "Shutter must be part of the collection.");
				foreach (IShutterGraphic graphic in this)
					if (graphic == shutter)
						graphic.Visible = false;
			}

			public void Deactivate(int index)
			{
				Platform.CheckArgumentRange(index, 0, base.Graphics.Count - 1, "index");
				for (int n = 0; n < base.Graphics.Count; n++)
					if (n == index)
						base.Graphics[n].Visible = false;
			}

			public void DeactivateAll()
			{
				foreach (IShutterGraphic graphic in this)
					graphic.Visible = false;
			}

			public void Add(IShutterGraphic shutter)
			{
				base.Graphics.Add(shutter);
				shutter.Visible = false;
			}

			public bool Remove(IShutterGraphic shutter)
			{
				return base.Graphics.Remove(shutter);
			}

			public void Insert(int index, IShutterGraphic shutter)
			{
				base.Graphics.Insert(0, shutter);
				shutter.Visible = false;
			}

			public void RemoveAt(int index)
			{
				base.Graphics.RemoveAt(index);
			}

			public int IndexOf(IShutterGraphic shutter)
			{
				return base.Graphics.IndexOf(shutter);
			}

			public void Clear()
			{
				base.Graphics.Clear();
			}

			public bool Contains(IShutterGraphic shutter)
			{
				return base.Graphics.Contains(shutter);
			}

			public IEnumerator<IShutterGraphic> GetEnumerator()
			{
				foreach (IGraphic graphic in base.Graphics)
					yield return (IShutterGraphic) graphic;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			#region IList<IShutterGraphic> Members

			IShutterGraphic IList<IShutterGraphic>.this[int index]
			{
				get { return this[index]; }
				set { throw new NotSupportedException(); }
			}

			void ICollection<IShutterGraphic>.CopyTo(IShutterGraphic[] array, int arrayIndex)
			{
				foreach (IShutterGraphic graphic in this)
					array[arrayIndex++] = graphic;
			}

			bool ICollection<IShutterGraphic>.IsReadOnly
			{
				get { return false; }
			}

			#endregion

			#region IEnumerable<IGraphic> Members

			IEnumerator<IGraphic> IEnumerable<IGraphic>.GetEnumerator()
			{
				return base.Graphics.GetEnumerator();
			}

			#endregion
		}
	}
}