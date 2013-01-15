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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Abstract class providing a base <see cref="IColorMap"/> implementation that defines the mapping of single-channel input pixel values to ARGB color values..
	/// </summary>
	/// <seealso cref="IColorMap"/>
	[Cloneable(true)]
	public abstract class ColorMapBase : IColorMap
	{
		private event EventHandler _lutChanged;

		/// <summary>
		/// Fires the <see cref="LutChanged"/> event.
		/// </summary>
		/// <remarks>
		/// Inheritors should call this method when any property of the color map has changed.
		/// </remarks>
		protected virtual void OnLutChanged()
		{
			EventsHelper.Fire(_lutChanged, this, EventArgs.Empty);
		}

		#region IColorMap Members

		/// <summary>
		/// Gets or sets the minimum input value.
		/// </summary>
		/// <remarks>
		/// This value should not be modified by your code.  It will be set internally by the framework.
		/// </remarks>
		public abstract int MinInputValue { get; set; }

		/// <summary>
		/// Gets or sets the maximum input value.
		/// </summary>
		/// <remarks>
		/// This value should not be modified by your code.  It will be set internally by the framework.
		/// </remarks>
		public abstract int MaxInputValue { get; set; }

		/// <summary>
		/// Gets the output ARGB color at a given input index.
		/// </summary>
		public abstract int this[int index] { get; protected set; }

		/// <summary>
		/// Gets the first mapped pixel value.
		/// </summary>
		public abstract int FirstMappedPixelValue { get; }

		/// <summary>
		/// Gets the color map data as a lookup table.
		/// </summary>
		public abstract int[] Data { get; }

		/// <summary>
		/// Fired when the color map has changed in some way.
		/// </summary>
		public event EventHandler LutChanged
		{
			add { _lutChanged += value; }
			remove { _lutChanged -= value; }
		}

		/// <summary>
		/// Gets a string key that identifies this particular color map's characteristics.
		/// </summary>
		/// <remarks>
		/// This method is not to be confused with <b>equality</b>, since some color maps can be
		/// dependent upon the actual image to which it belongs.
		/// </remarks>
		public abstract string GetKey();

		/// <summary>
		/// Gets an abbreviated description of the color map.
		/// </summary>
		public abstract string GetDescription();

		/// <summary>
		/// Creates a deep-copy of the <see cref="IColorMap"/>.
		/// </summary>
		/// <remarks>
		/// <see cref="IColorMap"/> implementations may return NULL from this method when appropriate.
		/// </remarks>
		public IColorMap Clone()
		{
			return CloneBuilder.Clone(this) as IColorMap;
		}

		#endregion

		#region IMemorable Members

		/// <summary>
		/// Captures the state of the color map.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The implementation should return an object containing enough state information so that,
		/// when <see cref="SetMemento"/> is called, the color map can be restored to the original state.
		/// </para>
		/// <para>
		/// If the method is implemented, <see cref="SetMemento"/> must also be implemented.
		/// </para>
		/// </remarks>
		public virtual object CreateMemento()
		{
			return null;
		}

		/// <summary>
		/// Restores the state of the color map.
		/// </summary>
		/// <param name="memento">An object that was originally created by <see cref="CreateMemento"/>.</param>
		/// <remarks>
		/// <para>
		/// The implementation should return the color map to the original state captured by <see cref="CreateMemento"/>.
		/// </para>
		/// <para>
		/// If you implement <see cref="CreateMemento"/> to capture the color map's state, you must also implement this method
		/// to allow the state to be restored. Failure to do so will result in a <see cref="InvalidOperationException"/>.
		/// </para>
		/// </remarks>
		public virtual void SetMemento(object memento)
		{
			if (memento != null)
				throw new InvalidOperationException(SR.ExceptionMustOverrideSetMemento);
		}

		#endregion
	}
}