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
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Base implementation of a <see cref="IDataLut"/> lookup table mapping input pixel values to output pixel values.
	/// </summary>
	/// <remarks>
	/// Normally, you should not have to inherit directly from this class.
	/// <see cref="SimpleDataLut"/> or <see cref="GeneratedDataLut"/> should cover
	/// most, if not all, common use cases.
	/// </remarks>
	[Cloneable(true)]
	public abstract class DataLut : IDataLut, IMemorable
	{
		private event EventHandler _lutChanged;

		private int _minInputValue;
		private int _maxInputValue;
		private int _minOutputValue;
		private int _maxOutputValue;

		public virtual int MinInputValue
		{
			get { return _minInputValue; }
			set
			{
				if (value == _minInputValue)
					return;

				_minInputValue = value;
				OnLutChanged();
			}
		}

		public virtual int MaxInputValue
		{
			get { return _maxInputValue; }
			set
			{
				if (value == _maxInputValue)
					return;

				_maxInputValue = value;
				OnLutChanged();
			}
		}

		public virtual int MinOutputValue
		{
			get { return _minOutputValue; }
			protected set
			{
				if (_minOutputValue == value)
					return;

				_minOutputValue = value;
				OnLutChanged();
			}
		}

		public virtual int MaxOutputValue
		{
			get { return _maxOutputValue; }
			protected set
			{
				if (value == _maxOutputValue)
					return;

				_maxOutputValue = value;
				OnLutChanged();
			}
		}

		public virtual int this[int index]
		{
			get
			{
				if (index <= FirstMappedPixelValue)
					return Data[0];
				else if (index >= LastMappedPixelValue)
					return Data[Length - 1];
				else
					return Data[index - FirstMappedPixelValue];
			}
			protected set
			{
				if (index < FirstMappedPixelValue || index > LastMappedPixelValue)
					return;

				Data[index - FirstMappedPixelValue] = value;
			}
		}

		public event EventHandler LutChanged
		{
			add { _lutChanged += value; }
			remove { _lutChanged -= value; }
		}

		///<summary>
		/// Gets the length of <see cref="Data"/>.
		///</summary>
		/// <remarks>
		/// The reason for this member's existence is that <see cref="Data"/> may
		/// not yet exist; this value is based solely on <see cref="FirstMappedPixelValue"/>
		/// and <see cref="LastMappedPixelValue"/>.
		/// </remarks>
		public int Length
		{
			get { return 1 + LastMappedPixelValue - FirstMappedPixelValue; }
		}

		public abstract int FirstMappedPixelValue { get; }

		/// <summary>
		/// Gets the last mapped pixel value.
		/// </summary>
		public abstract int LastMappedPixelValue { get; }

		public abstract int[] Data { get; }

		public abstract string GetKey();

		public abstract string GetDescription();

		public IDataLut Clone()
		{
			return CloneBuilder.Clone(this) as IDataLut;
		}

		/// <summary>
		/// Fires the <see cref="LutChanged"/> event.
		/// </summary>
		/// <remarks>
		/// Inheritors should call this method when any property of the lookup table has changed.
		/// </remarks>
		protected virtual void OnLutChanged()
		{
			EventsHelper.Fire(_lutChanged, this, EventArgs.Empty);
		}

		#region IMemorable Members

		/// <summary>
		/// Captures the state of the lookup table.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The implementation should return an object containing enough state information so that,
		/// when <see cref="SetMemento"/> is called, the lookup table can be restored to the original state.
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
		/// Restores the state of the lookup table.
		/// </summary>
		/// <param name="memento">An object that was originally created by <see cref="CreateMemento"/>.</param>
		/// <remarks>
		/// <para>
		/// The implementation should return the lookup table to the original state captured by <see cref="CreateMemento"/>.
		/// </para>
		/// <para>
		/// If you implement <see cref="CreateMemento"/> to capture the lookup table's state, you must also implement this method
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