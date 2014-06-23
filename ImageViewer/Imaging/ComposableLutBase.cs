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
	/// Abstract base implementation for a lookup table that comprises part of the standard grayscale image display pipeline.
	/// </summary>
	/// <remarks>
	/// This class contains abstract internal members and cannot be directly subclassed. Instead, subclass either
	/// <see cref="ComposableModalityLut"/>, <see cref="ComposableVoiLut"/> or <see cref="ComposableLut"/> depending
	/// on the intended use of the lookup table in the grayscale image display pipeline.
	/// </remarks>
	/// <seealso cref="LutComposer"/>
	/// <seealso cref="IComposableLut"/>
	/// <seealso cref="ComposableLut"/>
	/// <seealso cref="ComposableModalityLut"/>
	/// <seealso cref="ComposableVoiLut"/>
	[Cloneable(true)]
	public abstract class ComposableLutBase : IComposableLut
	{
	    #region Private Fields

        private string _key;
        private event EventHandler _lutChanged;

		#endregion

		#region Protected Constructor

		/// <summary>
		/// Initializes a new instance of <see cref="ComposableLutBase"/>.
		/// </summary>
		internal ComposableLutBase() {}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Fires the <see cref="LutChanged"/> event.
		/// </summary>
		/// <remarks>
		/// Inheritors should call this method when any property of the lookup table has changed.
		/// </remarks>
		protected virtual void OnLutChanged()
		{
		    _key = null;
			EventsHelper.Fire(_lutChanged, this, EventArgs.Empty);
		}

		#endregion

		#region IComposableLut Members

		/// <summary>
		/// Gets or sets the minimum input value.
		/// </summary>
		/// <remarks>
		/// This value is set internally by the framework and should not be modified by client code.
		/// </remarks>
		internal abstract double MinInputValueCore { get; set; }

		/// <summary>
		/// Gets or sets the maximum input value.
		/// </summary>
		/// <remarks>
		/// This value is set internally by the framework and should not be modified by client code.
		/// </remarks>
		internal abstract double MaxInputValueCore { get; set; }

		/// <summary>
		/// Gets the minimum output value.
		/// </summary>
		internal abstract double MinOutputValueCore { get; }

		/// <summary>
		/// Gets the maximum output value.
		/// </summary>
		internal abstract double MaxOutputValueCore { get; }

		double IComposableLut.MinInputValue
		{
			get { return MinInputValueCore; }
			set { MinInputValueCore = value; }
		}

		double IComposableLut.MaxInputValue
		{
			get { return MaxInputValueCore; }
			set { MaxInputValueCore = value; }
		}

		double IComposableLut.MinOutputValue
		{
			get { return MinOutputValueCore; }
		}

		double IComposableLut.MaxOutputValue
		{
			get { return MaxOutputValueCore; }
		}

		/// <summary>
		/// Gets the output value of the lookup table for a given input value.
		/// </summary>
		internal abstract double LookupCore(double input);

		/// <summary>
		/// Performs bulk lookup for a set of input values.
		/// </summary>
		/// <param name="input">Source array of input values to be transformed.</param>
		/// <param name="output">Destination array where transformed output values will be written (may be same array as <paramref name="input"/>).</param>
		/// <param name="count">Number of values in the arrays to transform (contiguous entries from start of array).</param>
		internal abstract void LookupCore(double[] input, double[] output, int count);

		double IComposableLut.this[double input]
		{
			get { return LookupCore(input); }
		}

		void IComposableLut.LookupValues(double[] input, double[] output, int count)
		{
			LookupCore(input, output, count);
		}

		/// <summary>
		/// Fired when the lookup table has changed in some way.
		/// </summary>
		public event EventHandler LutChanged
		{
			add { _lutChanged += value; }
			remove { _lutChanged -= value; }
		}

		/// <summary>
		/// Gets a string key that identifies this particular lookup table's characteristics.
		/// </summary>
		/// <remarks>
		/// This method is not to be confused with <b>equality</b>, since some lookup tables can be
		/// dependent upon the actual image to which it belongs.
		/// </remarks>
		public abstract string GetKey();

	    string IComposableLut.GetKey()
	    {
            if (String.IsNullOrWhiteSpace(_key))
	            _key = GetKey();
	        
            return _key;
	    }

		/// <summary>
		/// Gets an abbreviated description of the lookup table.
		/// </summary>
		public abstract string GetDescription();

		#endregion

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

		/// <summary>
		/// Creates a deep-copy of the <see cref="IComposableLut"/>.
		/// </summary>
		/// <remarks>
		/// Implementations may return null from this method when appropriate.
		/// </remarks>
		public IComposableLut Clone()
		{
			return CloneBuilder.Clone(this) as IComposableLut;
		}
	}
}