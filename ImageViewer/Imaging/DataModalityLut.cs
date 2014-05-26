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
	/// Base implementation of a lookup table in the standard grayscale image display pipeline used to transform stored pixel values to manufacturer-independent values implemented as an array of values.
	/// </summary>
	/// <remarks>
	/// Normally, you should not have to inherit directly from this class.
	/// <see cref="SimpleDataModalityLut"/> or <see cref="GeneratedDataModalityLut"/> should cover
	/// most, if not all, common use cases.
	/// </remarks>
	[Cloneable(true)]
	public abstract class DataModalityLut : IDataModalityLut
	{
		private event EventHandler _lutChanged;

		private int _minInputValue;
		private int _maxInputValue;
		private double _minOutputValue;
		private double _maxOutputValue;

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

		/// <summary>
		/// Gets or sets the minimum output value.
		/// </summary>
		public virtual double MinOutputValue
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

		/// <summary>
		/// Gets or sets the maximum output value.
		/// </summary>
		public virtual double MaxOutputValue
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

		/// <summary>
		/// Gets or sets the output value of the lookup table for a given input value.
		/// </summary>
		public virtual double this[int input]
		{
			get
			{
				if (input <= FirstMappedPixelValue)
					return Data[0];
				else if (input >= LastMappedPixelValue)
					return Data[Length - 1];
				else
					return Data[input - FirstMappedPixelValue];
			}
			protected set
			{
				if (input < FirstMappedPixelValue || input > LastMappedPixelValue)
					return;

				Data[input - FirstMappedPixelValue] = value;
			}
		}

		/// <summary>
		/// Performs bulk lookup for a set of input values.
		/// </summary>
		/// <param name="input">Source array of input values to be transformed.</param>
		/// <param name="output">Destination array where transformed output values will be written (may be same array as <paramref name="input"/>).</param>
		/// <param name="count">Number of values in the arrays to transform (contiguous entries from start of array).</param>
		public virtual void LookupValues(double[] input, double[] output, int count)
		{
			LutFunctions.LookupLut(input, output, count, Data, FirstMappedPixelValue, LastMappedPixelValue);
		}

		double IComposableLut.MinInputValue
		{
			get { return MinInputValue; }
			set { MinInputValue = (int) Math.Round(value); }
		}

		double IComposableLut.MaxInputValue
		{
			get { return MaxInputValue; }
			set { MaxInputValue = (int) Math.Round(value); }
		}

		double IComposableLut.MinOutputValue
		{
			get { return MinOutputValue; }
		}

		double IComposableLut.MaxOutputValue
		{
			get { return MaxOutputValue; }
		}

		double IComposableLut.this[double input]
		{
			get { return this[(int) Math.Round(input)]; }
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
		/// The reason for this member's existence is that <see cref="Data"/> is lazily-initialized, and thus may
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

		public abstract double[] Data { get; }

		public abstract string GetKey();

		public abstract string GetDescription();

		IComposableLut IComposableLut.Clone()
		{
			return Clone();
		}

		IModalityLut IModalityLut.Clone()
		{
			return Clone();
		}

		public IDataModalityLut Clone()
		{
			return CloneBuilder.Clone(this) as IDataModalityLut;
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