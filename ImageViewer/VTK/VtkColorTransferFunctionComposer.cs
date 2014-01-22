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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Imaging;
using vtk;

namespace ClearCanvas.ImageViewer.Vtk
{
	/// <summary>
	/// Combines various <see cref="IComposableLut"/> objects together in the standard grayscale image display pipeline and updates a <see cref="vtk.vtkColorTransferFunction"/>.
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
	[Cloneable]
	public class VtkColorTransferFunctionComposer
		: IVoiLutInstaller,
		  IColorMapInstaller,
		  IModalityLutProvider,
		  IVoiLutProvider,
		  IColorMapProvider,
		  ILutPipelineProvider,
		  IDisposable
	{
		private enum Luts
		{
			Modality = 1,
			Voi = 2,
		}

		private LutComposer _lutComposer;
		private LutFactory _lutFactory;
		private IVoiLutManager _voiLutManager;
		private IColorMapManager _colorMapManager;
		private IColorMap _colorMap;
		private bool _disposed;

		[CloneIgnore]
		private double[] _vtkData;

		[CloneIgnore]
		private bool _recalculationRequired = true;

		[CloneIgnore]
		private int _modifiedTime;

		#region Constructors

		public VtkColorTransferFunctionComposer(int bitsStored, bool isSigned, bool invert, double rescaleSlope, double rescaleIntercept)
		{
			BitsStored = bitsStored;
			IsSigned = isSigned;
			RescaleSlope = rescaleSlope <= double.Epsilon ? 1 : rescaleSlope;
			RescaleIntercept = rescaleIntercept;
			DefaultInvert = Invert = invert;
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected VtkColorTransferFunctionComposer(VtkColorTransferFunctionComposer source, ICloningContext context)
		{
			context.CloneFields(source, this);

			if (source.LutComposer.ModalityLut != null) //modality lut is constant; no need to clone.
				InitializeNecessaryLuts(Luts.Modality);

			if (source.LutComposer.NormalizationLut != null)
				LutComposer.NormalizationLut = source.NormalizationLut.Clone();

			if (source.LutComposer.VoiLut != null) //clone the voi lut.
				(this as IVoiLutInstaller).InstallVoiLut(source.VoiLut.Clone());

			//color map has already been cloned.
		}

		#endregion

		#region Disposal

		~VtkColorTransferFunctionComposer()
		{
			try
			{
				Dispose(false);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, "Exception thrown in Dispose");
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
				Platform.Log(LogLevel.Debug, ex, "Exception thrown in Dispose");
			}
		}

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized</param>
		protected virtual void Dispose(bool disposing)
		{
			_disposed = true;

			if (disposing)
			{
				_vtkData = null;

				if (_lutFactory != null)
				{
					_lutFactory.Dispose();
					_lutFactory = null;
				}

				if (_lutComposer != null)
				{
					_lutComposer.LutChanged -= OnLutComposerChanged;
					_lutComposer.Dispose();
					_lutComposer = null;
				}
			}
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the number of bits stored in the image.
		/// </summary>
		public int BitsStored { get; private set; }

		/// <summary>
		/// Get a value indicating whether the image's pixel data is signed.
		/// </summary>
		public bool IsSigned { get; private set; }

		/// <summary>
		/// Gets the slope of the linear modality LUT rescale function.
		/// </summary>
		public double RescaleSlope { get; private set; }

		/// <summary>
		/// Gets the intercept of the linear modality LUT rescale function.
		/// </summary>
		public double RescaleIntercept { get; private set; }

		#endregion

		#region IModalityLutProvider Members

		/// <summary>
		/// Retrieves this image's modality lut.
		/// </summary>
		public IModalityLut ModalityLut
		{
			get
			{
				InitializeNecessaryLuts(Luts.Modality);
				return LutComposer.ModalityLut;
			}
		}

		#endregion

		#region IVoiLutProvider Members

		/// <summary>
		/// Retrieves this image's <see cref="IVoiLutManager"/>.
		/// </summary>
		public IVoiLutManager VoiLutManager
		{
			get { return _voiLutManager ?? (_voiLutManager = new VoiLutManager(this, false)); }
		}

		#endregion

		#region IColorMapProvider Members

		/// <summary>
		/// Retrieves this image's <see cref="IColorMapManager"/>.
		/// </summary>
		public IColorMapManager ColorMapManager
		{
			get { return _colorMapManager ?? (_colorMapManager = new ColorMapManager(this)); }
		}

		#endregion

		#region IDrawable Members

		event EventHandler IDrawable.Drawing
		{
			add { }
			remove { }
		}

		void IDrawable.Draw() {}

		#endregion

		#region IVoiLutInstaller Members

		private bool _invert;

		/// <summary>
		/// Gets or sets a value indicating whether the image should be inverted.
		/// </summary>
		/// <remarks>
		/// Inversion is equivalent to polarity.
		/// </remarks>
		public bool Invert
		{
			get { return _invert; }
			set
			{
				if (_invert == value) return;
				_invert = value;

				Modified();
			}
		}

		/// <summary>
		/// Gets the default value of <see cref="Invert"/>.  In DICOM, this would be true
		/// for all MONOCHROME1 images.
		/// </summary>
		public bool DefaultInvert { get; private set; }

		/// <summary>
		/// Retrieves this image's Voi Lut.
		/// </summary>
		public IVoiLut VoiLut
		{
			get
			{
				InitializeNecessaryLuts(Luts.Voi);
				return LutComposer.VoiLut;
			}
		}

		void IVoiLutInstaller.InstallVoiLut(IVoiLut voiLut)
		{
			Platform.CheckForNullReference(voiLut, "voiLut");

			InitializeNecessaryLuts(Luts.Modality);

			LutComposer.VoiLut = voiLut;
		}

		#endregion

		#region IColorMapInstaller Members

		/// <summary>
		/// Retrieves this image's color map.
		/// </summary>
		public IColorMap ColorMap
		{
			get
			{
				if (_colorMap == null)
					(this as IColorMapInstaller).InstallColorMap(LutFactory.GetGrayscaleColorMap());
				return _colorMap;
			}
		}

		IEnumerable<ColorMapDescriptor> IColorMapInstaller.AvailableColorMaps
		{
			get { return LutFactory.AvailableColorMaps; }
		}

		void IColorMapInstaller.InstallColorMap(string name)
		{
			(this as IColorMapInstaller).InstallColorMap(LutFactory.GetColorMap(name));
		}

		void IColorMapInstaller.InstallColorMap(ColorMapDescriptor descriptor)
		{
			(this as IColorMapInstaller).InstallColorMap(descriptor.Name);
		}

		void IColorMapInstaller.InstallColorMap(IColorMap colorMap)
		{
			if (_colorMap == colorMap)
				return;

			_colorMap = colorMap;

			Modified();
		}

		#endregion

		#region ILutPipelineProvider Members

		public double LookupPixelValue(int rawPixelValue, LutPipelineStage outStage)
		{
			double value = rawPixelValue;

			if (outStage == LutPipelineStage.Source)
				return value;

			var modalityLut = ModalityLut;
			if (modalityLut != null)
				value = modalityLut[value];

			if (outStage == LutPipelineStage.Modality)
				return value;

			var normalizationLut = NormalizationLut;
			if (normalizationLut != null)
				value = normalizationLut[value];

			var voiLut = VoiLut;
			if (voiLut != null)
				value = voiLut[value];

			if (outStage == LutPipelineStage.Voi)
				return value;

			Platform.Log(LogLevel.Debug, "Unrecognized LUT pipeline stage");
			return value;
		}

		#endregion

		/// <summary>
		/// Gets or sets a LUT to normalize the output of the modality LUT immediately prior to the VOI LUT.
		/// </summary>
		/// <remarks>
		/// <para>
		/// In most cases, this should be left NULL. However, some PET images have a very small rescale slope (&lt;&lt; 1)
		/// and thus need this to fix the input to the VOI LUT.
		/// </para>
		/// <para>
		/// At any rate, DO NOT use the output of this LUT for any purpose other than as an input to the VOI LUT, as it is meaningless otherwise.</para>
		/// </remarks>
		public IComposableLut NormalizationLut
		{
			get { return LutComposer.NormalizationLut; }
			set { LutComposer.NormalizationLut = value; }
		}

		/// <summary>
		/// Gets a timestamp representing the last calculation time of the function, similar to the VTK GetMTime() and Modified() system.
		/// </summary>
		public int GetMTime()
		{
			return _modifiedTime;
		}

		/// <summary>
		/// Flags the last calculation of the function as invalid due to more recent changes, similar to the VTK GetMTime() and Modified() system.
		/// </summary>
		private void Modified()
		{
			_recalculationRequired = true;
			_modifiedTime = Environment.TickCount;
		}

		/// <summary>
		/// Recomputes the LUT data, if necessary.
		/// </summary>
		public unsafe void Recalculate()
		{
			if (!_recalculationRequired) return;

			var outputLut = GetOutputLut();
			var colorMap = ColorMap;
			colorMap.MinInputValue = outputLut.MinOutputValue;
			colorMap.MaxInputValue = outputLut.MaxOutputValue;

			var outputLutData = outputLut.Data;
			var colorMapData = colorMap.Data;
			var numberOfEntries = outputLutData.Length;
			if (_vtkData == null || _vtkData.Length != 3*numberOfEntries)
				_vtkData = new double[3*numberOfEntries];

			var clock = new CodeClock();
			clock.Start();

			fixed (int* pOutputLutData = outputLutData)
			fixed (int* pColorMapData = colorMapData)
			fixed (double* pFinalLutData = _vtkData)
			fixed (double* pLookup = _singleChannelByteToDouble)
			{
				var pFinalLut = pFinalLutData;
				var pOutputLut = pOutputLutData;

				if (!Invert)
				{
					var firstColorMappedPixelValue = colorMap.FirstMappedPixelValue;
					for (var i = 0; i < numberOfEntries; ++i)
					{
						var value = pColorMapData[*(pOutputLut++) - firstColorMappedPixelValue];
						*(pFinalLut++) = pLookup[(value >> 16) & 0x0FF];
						*(pFinalLut++) = pLookup[(value >> 8) & 0x0FF];
						*(pFinalLut++) = pLookup[(value) & 0x0FF];
					}
				}
				else
				{
					var lastColorMappedPixelValue = colorMap.FirstMappedPixelValue + colorMap.Data.Length - 1;
					for (var i = 0; i < numberOfEntries; ++i)
					{
						var value = pColorMapData[lastColorMappedPixelValue - *(pOutputLut++)];
						*(pFinalLut++) = pLookup[(value >> 16) & 0x0FF];
						*(pFinalLut++) = pLookup[(value >> 8) & 0x0FF];
						*(pFinalLut++) = pLookup[(value) & 0x0FF];
					}
				}
			}

			clock.Stop();
			PerformanceReportBroker.PublishReport("VtkColorTransferFunctionComposer", "Recalculate", clock.Seconds);

			_recalculationRequired = false;
			_modifiedTime = Environment.TickCount;
		}

		/// <summary>
		/// Updates the given VTK color transfer function with the current composed LUT data.
		/// </summary>
		/// <param name="tfColor"></param>
		public void UpdateTransferFunction(vtkColorTransferFunction tfColor)
		{
			if (_disposed) throw new ObjectDisposedException("The transfer function composer has already been disposed!");

			Recalculate();

			// N.B.: BuildFunctionFromTable builds the transfer function from the given array, but does not keep pointer to array!

			var outputLut = GetOutputLut();
			tfColor.SetRange(outputLut.MinInputValue, outputLut.MaxInputValue);
			tfColor.BuildFunctionFromTable(outputLut.MinInputValue, outputLut.MaxInputValue, _vtkData.Length/3, _vtkData);
		}

		#region Static Initialization

		private static readonly double[] _singleChannelByteToDouble = new double[256];

		static unsafe VtkColorTransferFunctionComposer()
		{
			fixed (double* p = _singleChannelByteToDouble)
			{
				var pData = p;
				for (var n = 0; n < 256; ++n)
					*(pData++) = n/255.0;
			}
		}

		#endregion

		/// <summary>
		/// The output lut composed of both the Modality and Voi Luts.
		/// </summary>
		private IComposedLut GetOutputLut()
		{
			InitializeNecessaryLuts(Luts.Voi);
			return LutComposer.GetOutputLut(0, byte.MaxValue);
		}

		private LutComposer LutComposer
		{
			get
			{
				if (_lutComposer == null)
				{
					_lutComposer = new LutComposer(BitsStored, IsSigned);
					_lutComposer.LutChanged += OnLutComposerChanged;
				}
				return _lutComposer;
			}
		}

		private void OnLutComposerChanged(object sender, EventArgs e)
		{
			Modified();
		}

		private LutFactory LutFactory
		{
			get { return _lutFactory ?? (_lutFactory = LutFactory.Create()); }
		}

		private void InitializeNecessaryLuts(Luts luts)
		{
			if (luts >= Luts.Modality && LutComposer.ModalityLut == null)
			{
				var lut = LutFactory.GetModalityLutLinear(BitsStored, IsSigned, RescaleSlope, RescaleIntercept);

				LutComposer.ModalityLut = lut;
			}

			if (luts >= Luts.Voi && LutComposer.VoiLut == null)
			{
				var lut = new IdentityVoiLinearLut();

				(this as IVoiLutInstaller).InstallVoiLut(lut);
			}
		}
	}
}