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
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Combines various <see cref="IComposableLut"/> objects together in the standard grayscale image display pipeline.
	/// </summary>
	/// <seealso cref="IComposableLut"/>
	/// <remarks>
	/// <para>
	/// The sub-functions of the standard imaging display pipeline are, in order:
	/// <list type="table">
	/// <listheader>
	/// <name>Name</name>
	/// <description>Description</description>
	/// </listheader>
	/// <item>
	/// <name>Modality LUT</name>
	/// <description>Transforms stored pixel values to manufacturer-independent values.</description>
	/// </item>
	/// <item>
	/// <name>Normalization LUT</name>
	/// <description>Performs any additional transformation prior to selecting the VOI range, as may be necessary in some PET images.</description>
	/// </item>
	/// <item>
	/// <name>Values-of-Interest (VOI) LUT</name>
	/// <description>Selects range from manufacturer-independent values for display.</description>
	/// </item>
	/// </list>
	/// </para>
	/// </remarks>
	public class LutComposer : IComposedLut, IDisposable
	{
		#region Private Fields

		private LutCollection _lutCollection;
		private IComposableLut _normalizationLut;
		private IModalityLut _modalityLut;
		private IVoiLut _voiLut;
		private bool _recalculate = true;
		private ComposedLutCache.ICachedLut _cachedLut;
		private int _minInputValue = int.MinValue;
		private int _maxInputValue = int.MaxValue;

		#endregion

		/// <summary>
		/// Initializes a new instance of <see cref="LutComposer"/>.
		/// </summary>
		public LutComposer() {}

		/// <summary>
		/// Initializes a new instance of <see cref="LutComposer"/>.
		/// </summary>
		/// <param name="minInputValue">The smallest input value that can be used to perform a lookup in the composed table.</param>
		/// <param name="maxInputValue">The largest input value that can be used to perform a lookup in the composed table.</param>
		public LutComposer(int minInputValue, int maxInputValue) : this()
		{
			_minInputValue = minInputValue;
			_maxInputValue = maxInputValue;
		}

		/// <summary>
		/// Initializes a new instance of <see cref="LutComposer"/>.
		/// </summary>
		/// <param name="inputBits">The number of bits used by the input values to the composed lookup table.</param>
		/// <param name="inputIsSigned">A value indicating whether or not the input values are signed.</param>
		public LutComposer(int inputBits, bool inputIsSigned) : this()
		{
			if (inputIsSigned)
			{
				_minInputValue = -(1 << (inputBits - 1));
				_maxInputValue = (1 << (inputBits - 1)) - 1;
			}
			else
			{
				_minInputValue = 0;
				_maxInputValue = (1 << inputBits) - 1;
			}
		}

		/// <summary>
		/// Gets the assembled collection of <see cref="IComposableLut"/>s.
		/// </summary>
		private LutCollection LutCollection
		{
			get
			{
				if (_lutCollection == null)
				{
					_lutCollection = new LutCollection();

					if (_modalityLut != null) _lutCollection.Add(_modalityLut);
					if (_normalizationLut != null) _lutCollection.Add(_normalizationLut);
					if (_voiLut != null) _lutCollection.Add(_voiLut);
				}

				return _lutCollection;
			}
		}

		#region Public Properties

		/// <summary>
		/// Gets or sets the modality LUT in the grayscale image display pipeline, which transforms stored pixel values to manufacturer-independent values.
		/// </summary>
		/// <seealso cref="LutComposer"/>
		public IModalityLut ModalityLut
		{
			get { return _modalityLut; }
			set { SetLutField(ref _modalityLut, value); }
		}

		/// <summary>
		/// Gets or sets the normalization LUT in the grayscale image display pipeline, which additional transformation of manufacturer-independent values prior to selecting a dynamic range for display.
		/// </summary>
		/// <seealso cref="LutComposer"/>
		public IComposableLut NormalizationLut
		{
			get { return _normalizationLut; }
			set { SetLutField(ref _normalizationLut, value); }
		}

		/// <summary>
		/// Gets or sets the VOI (values of interest) LUT in the grayscale image display pipeline, which selects a range from the manufacturer-independent values for display.
		/// </summary>
		/// <seealso cref="LutComposer"/>
		public IVoiLut VoiLut
		{
			get { return _voiLut; }
			set { SetLutField(ref _voiLut, value); }
		}

		#endregion

		/// <summary>
		/// Sets the <see cref="IComposableLut"/> field and sets up the LutChanged event handler.
		/// </summary>
		private void SetLutField<T>(ref T field, T value)
			where T : class, IComposableLut
		{
			if (Equals(field, value))
				return;

			if (field != null)
				field.LutChanged -= OnLutValuesChanged;

			field = value;

			if (field != null)
				field.LutChanged += OnLutValuesChanged;

			OnLutChanged();
		}

		private void OnLutChanged()
		{
			// clear the LUT pipeline so that it will be reassembled
			if (_lutCollection != null)
			{
				_lutCollection.Clear();
				_lutCollection = null;
			}

			SyncMinMaxValues();
			_recalculate = true;
		}

		private void SyncMinMaxValues()
		{
			if (LutCollection.Count > 0)
			{
				IComposableLut firstLut = LutCollection[0];
				firstLut.MinInputValue = _minInputValue;
				firstLut.MaxInputValue = _maxInputValue;
			}

			LutCollection.SyncMinMaxValues();
		}

		private void DisposeCachedLut()
		{
			if (_cachedLut != null)
			{
				_cachedLut.Dispose();
				_cachedLut = null;
			}
		}

		#region Event Handlers

		private void OnLutValuesChanged(object sender, EventArgs e)
		{
			OnLutChanged();
		}

		#endregion

		#region Properties

		/// <summary>
		/// The output LUT of the pipeline.
		/// </summary>
		private IComposedLut ComposedLut
		{
			get
			{
				if (_recalculate)
				{
					DisposeCachedLut();
					_recalculate = false;
				}

				if (_cachedLut == null)
					_cachedLut = ComposedLutCache.GetLut(LutCollection);

				return _cachedLut;
			}
		}

		private IComposableLut LastLut
		{
			get
			{
				LutCollection.Validate();
				return LutCollection[LutCollection.Count - 1];
			}
		}

		#endregion

		#region IComposedLut Members

		/// <summary>
		/// Gets the composed lut data.
		/// </summary>
		/// <remarks>
		/// This property should be considered readonly and is only 
		/// provided for fast (unsafe) iteration over the array.
		/// </remarks>
		public int[] Data
		{
			get { return ComposedLut.Data; }
		}

		#endregion

		#region ILut Members

		/// <summary>
		/// Gets or sets the minimum input value.
		/// </summary>
		public int MinInputValue
		{
			get { return _minInputValue; }
			set
			{
				if (_minInputValue != value)
				{
					_minInputValue = value;
					OnLutChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets the maximum input value.
		/// </summary>
		public int MaxInputValue
		{
			get { return _maxInputValue; }
			set
			{
				if (_maxInputValue != value)
				{
					_maxInputValue = value;
					OnLutChanged();
				}
			}
		}

		/// <summary>
		/// Gets the minimum output value.
		/// </summary>
		public int MinOutputValue
		{
			get { return (int) Math.Round(LastLut.MinOutputValue); }
		}

		/// <summary>
		/// Gets the maximum output value.
		/// </summary>
		public int MaxOutputValue
		{
			get { return (int) Math.Round(LastLut.MaxOutputValue); }
		}

		/// <summary>
		/// Gets the output value of the lut at a given input index.
		/// </summary>
		public int this[int index]
		{
			get { return ComposedLut[index]; }
		}

		#region Disposal

		#region IDisposable Members

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern.
		/// </summary>
		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				// shouldn't throw anything from inside Dispose()
				Platform.Log(LogLevel.Debug, e);
			}
		}

		#endregion

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern.
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				DisposeCachedLut();

				if (_lutCollection != null)
					_lutCollection.Clear();
			}
		}

		#endregion

		#endregion
	}
}