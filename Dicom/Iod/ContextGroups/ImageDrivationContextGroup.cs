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
using ClearCanvas.Dicom.Iod.Macros;

namespace ClearCanvas.Dicom.Iod.ContextGroups
{
	/// <summary>
	/// Image Derivation Context Group
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 16, Annex B, CID 7203</remarks>
	public sealed class ImageDerivationContextGroup : ContextGroupBase<ImageDerivation>
	{
		private ImageDerivationContextGroup()
			: base(7203, "Image Derivation", true, new DateTime(2011, 06, 09)) {}

		#region Static Instances

		public static readonly ImageDerivation LossyCompression = new ImageDerivation("113040", "Lossy Compression");
		public static readonly ImageDerivation ApparentDiffusionCoefficient = new ImageDerivation("113041", "Apparent Diffusion Coefficient");
		public static readonly ImageDerivation PixelByPixelAddition = new ImageDerivation("113042", "Pixel by pixel addition");
		public static readonly ImageDerivation DiffusionWeighted = new ImageDerivation("113043", "Diffusion weighted");
		public static readonly ImageDerivation DiffusionAnisotropy = new ImageDerivation("113044", "Diffusion Anisotropy");
		public static readonly ImageDerivation DiffusionAttenuated = new ImageDerivation("113045", "Diffusion Attenuated");
		public static readonly ImageDerivation PixelByPixelDivision = new ImageDerivation("113046", "Pixel by pixel division");
		public static readonly ImageDerivation PixelByPixelMask = new ImageDerivation("113047", "Pixel by pixel mask");
		public static readonly ImageDerivation PixelByPixelMaximum = new ImageDerivation("113048", "Pixel by pixel Maximum");
		public static readonly ImageDerivation PixelByPixelMean = new ImageDerivation("113049", "Pixel by pixel mean");
		public static readonly ImageDerivation MetaboliteMapsFromSpectroscopyData = new ImageDerivation("113050", "Metabolite Maps from spectroscopy data");
		public static readonly ImageDerivation PixelByPixelMinimum = new ImageDerivation("113051", "Pixel by pixel Minimum");
		public static readonly ImageDerivation MeanTransitTime = new ImageDerivation("113052", "Mean Transit Time");
		public static readonly ImageDerivation PixelByPixelMultiplication = new ImageDerivation("113053", "Pixel by pixel multiplication");
		public static readonly ImageDerivation NegativeEnhancementIntegral = new ImageDerivation("113054", "Negative Enhancement Integral");
		public static readonly ImageDerivation RegionalCerebralBloodFlow = new ImageDerivation("113055", "Regional Cerebral Blood Flow");
		public static readonly ImageDerivation RegionalCerebralBloodVolume = new ImageDerivation("113056", "Regional Cerebral Blood Volume");
		public static readonly ImageDerivation RCoefficientMap = new ImageDerivation("113057", "R-Coefficient Map");
		public static readonly ImageDerivation ProtonDensityMap = new ImageDerivation("113058", "Proton Density map");
		public static readonly ImageDerivation SignalChangeMap = new ImageDerivation("113059", "Signal Change Map");
		public static readonly ImageDerivation SignalToNoiseMap = new ImageDerivation("113060", "Signal to Noise Map");
		public static readonly ImageDerivation StandardDeviation = new ImageDerivation("113061", "Standard Deviation");
		public static readonly ImageDerivation PixelByPixelSubtraction = new ImageDerivation("113062", "Pixel by pixel subtraction");
		public static readonly ImageDerivation T1Map = new ImageDerivation("113063", "T1 Map");
		public static readonly ImageDerivation T2StarMap = new ImageDerivation("113064", "T2* Map");
		public static readonly ImageDerivation T2Map = new ImageDerivation("113065", "T2 Map");
		public static readonly ImageDerivation TimeCourseOfSignal = new ImageDerivation("113066", "Time Course of Signal");
		public static readonly ImageDerivation TemperatureEncoded = new ImageDerivation("113067", "Temperature encoded");
		public static readonly ImageDerivation StudentsTTest = new ImageDerivation("113068", "Student’s T-Test");
		public static readonly ImageDerivation TimeToPeakMap = new ImageDerivation("113069", "Time To Peak map");
		public static readonly ImageDerivation VelocityEncoded = new ImageDerivation("113070", "Velocity encoded");
		public static readonly ImageDerivation ZScoreMap = new ImageDerivation("113071", "Z-Score Map");
		public static readonly ImageDerivation MultiplanarReformatting = new ImageDerivation("113072", "Multiplanar reformatting");
		public static readonly ImageDerivation CurvedMultiplanarReformatting = new ImageDerivation("113073", "Curved multiplanar reformatting");
		public static readonly ImageDerivation VolumeRendering = new ImageDerivation("113074", "Volume rendering");
		public static readonly ImageDerivation SurfaceRendering = new ImageDerivation("113075", "Surface rendering");
		public static readonly ImageDerivation Segmentation = new ImageDerivation("113076", "Segmentation");
		public static readonly ImageDerivation VolumeEditing = new ImageDerivation("113077", "Volume editing");
		public static readonly ImageDerivation MaximumIntensityProjection = new ImageDerivation("113078", "Maximum intensity projection");
		public static readonly ImageDerivation MinimumIntensityProjection = new ImageDerivation("113079", "Minimum intensity projection");
		public static readonly ImageDerivation SpatialResampling = new ImageDerivation("113085", "Spatial resampling");
		public static readonly ImageDerivation EdgeEnhancement = new ImageDerivation("113086", "Edge enhancement");
		public static readonly ImageDerivation Smoothing = new ImageDerivation("113087", "Smoothing");
		public static readonly ImageDerivation GaussianBlur = new ImageDerivation("113088", "Gaussian blur");
		public static readonly ImageDerivation UnsharpMask = new ImageDerivation("113089", "Unsharp mask");
		public static readonly ImageDerivation ImageStitching = new ImageDerivation("113090", "Image stitching");
		public static readonly ImageDerivation SpatiallyRelatedFramesExtractedFromTheVolume = new ImageDerivation("113091", "Spatially-related frames extracted from the volume");
		public static readonly ImageDerivation TemporallyRelatedFramesExtractedFromTheSetOfVolumes = new ImageDerivation("113092", "Temporally-related frames extracted from the set of volumes");
		public static readonly ImageDerivation MultiEnergyProportionalWeighting = new ImageDerivation("113097", "Multi-energy proportional weighting");
		public static readonly ImageDerivation PolarToRectangularScanConversion = new ImageDerivation("113093", "Polar to Rectangular Scan Conversion");

		#endregion

		#region Singleton Instancing

		private static readonly ImageDerivationContextGroup _contextGroup = new ImageDerivationContextGroup();

		public static ImageDerivationContextGroup Instance
		{
			get { return _contextGroup; }
		}

		#endregion

		#region Static Enumeration of Values

		public static IEnumerable<ImageDerivation> Values
		{
			get
			{
				yield return LossyCompression;
				yield return ApparentDiffusionCoefficient;
				yield return PixelByPixelAddition;
				yield return DiffusionWeighted;
				yield return DiffusionAnisotropy;
				yield return DiffusionAttenuated;
				yield return PixelByPixelDivision;
				yield return PixelByPixelMask;
				yield return PixelByPixelMaximum;
				yield return PixelByPixelMean;
				yield return MetaboliteMapsFromSpectroscopyData;
				yield return PixelByPixelMinimum;
				yield return MeanTransitTime;
				yield return PixelByPixelMultiplication;
				yield return NegativeEnhancementIntegral;
				yield return RegionalCerebralBloodFlow;
				yield return RegionalCerebralBloodVolume;
				yield return RCoefficientMap;
				yield return ProtonDensityMap;
				yield return SignalChangeMap;
				yield return SignalToNoiseMap;
				yield return StandardDeviation;
				yield return PixelByPixelSubtraction;
				yield return T1Map;
				yield return T2StarMap;
				yield return T2Map;
				yield return TimeCourseOfSignal;
				yield return TemperatureEncoded;
				yield return StudentsTTest;
				yield return TimeToPeakMap;
				yield return VelocityEncoded;
				yield return ZScoreMap;
				yield return MultiplanarReformatting;
				yield return CurvedMultiplanarReformatting;
				yield return VolumeRendering;
				yield return SurfaceRendering;
				yield return Segmentation;
				yield return VolumeEditing;
				yield return MaximumIntensityProjection;
				yield return MinimumIntensityProjection;
				yield return SpatialResampling;
				yield return EdgeEnhancement;
				yield return Smoothing;
				yield return GaussianBlur;
				yield return UnsharpMask;
				yield return ImageStitching;
				yield return SpatiallyRelatedFramesExtractedFromTheVolume;
				yield return TemporallyRelatedFramesExtractedFromTheSetOfVolumes;
				yield return MultiEnergyProportionalWeighting;
				yield return PolarToRectangularScanConversion;
			}
		}

		/// <summary>
		/// Gets an enumerator that iterates through the defined codes.
		/// </summary>
		/// <returns>A <see cref="IEnumerator{T}"/> object that can be used to iterate through the defined codes.</returns>
		public override IEnumerator<ImageDerivation> GetEnumerator()
		{
			return Values.GetEnumerator();
		}

		public static ImageDerivation LookupCode(CodeSequenceMacro codeSequence)
		{
			return Instance.Lookup(codeSequence);
		}

		#endregion
	}

	/// <summary>
	/// Represents an Image Derivation code.
	/// </summary>
	public class ImageDerivation : ContextGroupBase<ImageDerivation>.ContextGroupItemBase
	{
		/// <summary>
		/// Constructor for codes defined in DICOM 2011, Part 16, Annex B, CID 7203.
		/// </summary>
		internal ImageDerivation(string codeValue, string codeMeaning) : base("DCM", codeValue, codeMeaning) {}

		/// <summary>
		/// Constructs a new image derivation code.
		/// </summary>
		/// <param name="codingSchemeDesignator">The designator of the coding scheme in which this code is defined.</param>
		/// <param name="codeValue">The value of this code.</param>
		/// <param name="codeMeaning">The Human-readable meaning of this code.</param>
		/// <exception cref="ArgumentException">Thrown if <paramref name="codingSchemeDesignator"/> or <paramref name="codeValue"/> are <code>null</code> or empty.</exception>
		public ImageDerivation(string codingSchemeDesignator, string codeValue, string codeMeaning)
			: base(codingSchemeDesignator, codeValue, codeMeaning) {}

		/// <summary>
		/// Constructs a new image derivation code.
		/// </summary>
		/// <param name="codingSchemeDesignator">The designator of the coding scheme in which this code is defined.</param>
		/// <param name="codingSchemeVersion">The version of the coding scheme in which this code is defined, if known. Should be <code>null</code> if not explicitly specified.</param>
		/// <param name="codeValue">The value of this code.</param>
		/// <param name="codeMeaning">The Human-readable meaning of this code.</param>
		/// <exception cref="ArgumentException">Thrown if <paramref name="codingSchemeDesignator"/> or <paramref name="codeValue"/> are <code>null</code> or empty.</exception>
		public ImageDerivation(string codingSchemeDesignator, string codingSchemeVersion, string codeValue, string codeMeaning)
			: base(codingSchemeDesignator, codingSchemeVersion, codeValue, codeMeaning) {}

		/// <summary>
		/// Constructs a new image derivation code.
		/// </summary>
		/// <param name="codeSequence">The code sequence attributes macro.</param>
		/// <exception cref="ArgumentException">Thrown if <paramref name="codeSequence.CodingSchemeDesignator"/> or <paramref name="codeSequence.CodeValue"/> are <code>null</code> or empty.</exception>
		public ImageDerivation(CodeSequenceMacro codeSequence)
			: base(codeSequence.CodingSchemeDesignator, codeSequence.CodingSchemeVersion, codeSequence.CodeValue, codeSequence.CodeMeaning) {}

		public static bool TryParse(CodeSequenceMacro codeSequence, out ImageDerivation imageDerivation)
		{
			try
			{
				imageDerivation = new ImageDerivation(codeSequence);
				return true;
			}
			catch (Exception)
			{
				imageDerivation = null;
				return false;
			}
		}
	}
}