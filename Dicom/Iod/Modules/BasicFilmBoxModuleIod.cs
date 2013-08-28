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
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod.Sequences;

namespace ClearCanvas.Dicom.Iod.Modules
{
	//TODO (CR March 2011): Just generally, it might be better to use regular expressions
	//for a lot of the parsing that's done in these classes.

	/// <summary>
	/// Basic Film Box Presentation and Relationship Module as per Part 3, C.13-3 (pg 862) and C.13.4 (pg 869)
	/// </summary>
	public class BasicFilmBoxModuleIod : IodBase
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="FilmBoxModuleIod"/> class.
		/// </summary>
		public BasicFilmBoxModuleIod() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="FilmBoxModuleIod"/> class.
		/// </summary>
		public BasicFilmBoxModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider) {}

		#endregion

		#region Public Properties

		/// <summary>
		/// Type of image display format. Enumerated Values:
		/// <para>
		/// STANDARD\C,R : film contains equal size rectangular image boxes with R rows of image boxes and C columns of image boxes; C and R are integers.
		/// </para>
		/// <para>
		/// ROW\R1,R2,R3, etc. : film contains rows with equal size rectangular image boxes with R1 image boxes in the first row, R2 image boxes in second row, 
		/// R3 image boxes in third row, etc.; R1, R2, R3, etc. are integers.
		/// </para>
		/// <para>
		/// COL\C1,C2,C3, etc.: film contains columns with equal size rectangular image boxes with C1 image boxes in the first column, C2 image boxes in second
		///  column, C3 image boxes in third column, etc.; C1, C2, C3, etc. are integers.
		/// </para>
		/// <para>
		/// SLIDE : film contains 35mm slides; the number of slides for a particular film size is configuration dependent.
		/// </para>
		/// <para>
		/// SUPERSLIDE : film contains 40mm slides; the number of slides for a particular film size is configuration dependent.
		/// </para>
		/// <para>
		/// CUSTOM\i : film contains a customized ordering of rectangular image boxes; i identifies the image display format; the definition of the image display
		/// formats is defined in the Conformance Statement; i is an integer.
		/// </para>
		/// </summary>
		/// <value></value>
		public ImageDisplayFormat ImageDisplayFormat
		{
			get { return ImageDisplayFormat.FromDicomString(base.DicomAttributeProvider[DicomTags.ImageDisplayFormat].GetString(0, String.Empty)); }
			set { base.DicomAttributeProvider[DicomTags.ImageDisplayFormat].SetStringValue(value.DicomString); }
		}

		/// <summary>
		/// Identification of annotation display format. The definition of the annotation display formats and the
		/// annotation box position sequence are defined in the Conformance Statement.
		/// </summary>
		/// <value>The annotation display format id.</value>
		public string AnnotationDisplayFormatId
		{
			get { return base.DicomAttributeProvider[DicomTags.AnnotationDisplayFormatId].GetString(0, String.Empty); }
			set { base.DicomAttributeProvider[DicomTags.AnnotationDisplayFormatId].SetString(0, value); }
		}

		/// <summary>
		/// Gets or sets the film orientation.
		/// </summary>
		/// <value>The film orientation.</value>
		public FilmOrientation FilmOrientation
		{
			get { return ParseEnum<FilmOrientation>(base.DicomAttributeProvider[DicomTags.FilmOrientation].GetString(0, String.Empty), FilmOrientation.None); }
			set { SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.FilmOrientation], value, false); }
		}

		/// <summary>
		/// Gets or sets the film size id.
		/// </summary>
		/// <value>The film size id.</value>
		public FilmSize FilmSizeId
		{
			get { return FilmSize.FromDicomString(base.DicomAttributeProvider[DicomTags.FilmSizeId].GetString(0, String.Empty)); }
			set { SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.FilmSizeId], value.DicomString, false); }
		}

		/// <summary>
		/// Gets or sets the type of the magnification.Interpolation type by which the printer magnifies or decimates the image in order to fit the image in the
		/// image box on film.
		/// </summary>
		/// <value>The type of the magnification.</value>
		public MagnificationType MagnificationType
		{
			get { return ParseEnum<MagnificationType>(base.DicomAttributeProvider[DicomTags.MagnificationType].GetString(0, String.Empty), MagnificationType.None); }
			set { SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.MagnificationType], value, false); }
		}

		/// <summary>
		/// Gets or sets the type of the smoothing.  Further specifies the type of the interpolation function. Values are defined in Conformance Statement.
		/// </summary>
		/// <value>The type of the smoothing.</value>
		public SmoothingType SmoothingType
		{
			get { return ParseEnum<SmoothingType>(base.DicomAttributeProvider[DicomTags.SmoothingType].GetString(0, String.Empty), SmoothingType.None); }
			set { SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.SmoothingType], value, false); }
		}

		/// <summary>
		/// Gets or sets the border density.  Density of the film areas surrounding and between images on the film. Defined Terms: 
		/// <para>BLACK 
		/// </para>
		/// <para>
		/// WHITE 
		/// </para>
		/// <para>
		/// i where i represents the desired density in hundreds of OD
		/// </para>
		/// </summary>
		/// <value>The border density.</value>
		public BorderDensity BorderDensity
		{
			get { return ParseEnum<BorderDensity>(base.DicomAttributeProvider[DicomTags.BorderDensity].GetString(0, String.Empty), BorderDensity.None); }
			set { SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.BorderDensity], value, false); }
		}

		/// <summary>
		/// Gets or sets the empty image density.  Density of the image box area on the film that contains no image. Defined Terms: 
		/// <para>BLACK 
		/// </para>
		/// <para>
		/// WHITE 
		/// </para>
		/// <para>
		/// i where i represents the desired density in hundreds of OD
		/// </para>
		/// </summary>
		/// <value>The empty image density.</value>
		public EmptyImageDensity EmptyImageDensity
		{
			get { return ParseEnum<EmptyImageDensity>(base.DicomAttributeProvider[DicomTags.EmptyImageDensity].GetString(0, String.Empty), EmptyImageDensity.None); }
			set { SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.EmptyImageDensity], value, false); }
		}

		/// <summary>
		/// Gets or sets the min density.  Minimum density of the images on the film, expressed in hundredths of OD. If Min Density is lower than minimum printer density than Min Density 
		/// is set to minimum printer density.
		/// </summary>
		/// <value>The min density.</value>
		public ushort MinDensity
		{
			get { return base.DicomAttributeProvider[DicomTags.MinDensity].GetUInt16(0, 0); }
			set { base.DicomAttributeProvider[DicomTags.MinDensity].SetUInt16(0, value); }
		}

		/// <summary>
		/// Gets or sets the min density.  Maximum density of the images on the film, expressed in hundredths of OD. If Max Density higher than maximum printer density than Max 
		/// Density is set to maximum printer density.
		/// </summary>
		/// <value>The min density.</value>
		public ushort MaxDensity
		{
			get { return base.DicomAttributeProvider[DicomTags.MaxDensity].GetUInt16(0, 0); }
			set { base.DicomAttributeProvider[DicomTags.MaxDensity].SetUInt16(0, value); }
		}

		/// <summary>
		/// Gets or sets the trim, YES OR NO.
		/// </summary>
		/// <value>The trim.</value>
		public DicomBoolean Trim
		{
			get { return ParseEnum<DicomBoolean>(base.DicomAttributeProvider[DicomTags.Trim].GetString(0, String.Empty), DicomBoolean.None); }
			set { SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.Trim], value, false); }
		}

		/// <summary>
		/// Gets or sets the configuration information.
		/// </summary>
		/// <value>The configuration information.</value>
		public string ConfigurationInformation
		{
			get { return base.DicomAttributeProvider[DicomTags.ConfigurationInformation].GetString(0, String.Empty); }
			set { base.DicomAttributeProvider[DicomTags.ConfigurationInformation].SetStringValue(value); }
		}

		/// <summary>
		/// Gets or sets the illumination.  Luminance of lightbox illuminating a piece of transmissive film, or for the case of reflective media, luminance obtainable from diffuse reflection of the illumination present. Expressed as L0, in candelas per square meter (cd/m2).
		/// </summary>
		/// <value>The illumination.</value>
		public ushort Illumination
		{
			get { return base.DicomAttributeProvider[DicomTags.Illumination].GetUInt16(0, 0); }
			set { base.DicomAttributeProvider[DicomTags.Illumination].SetUInt16(0, value); }
		}

		/// <summary>
		/// Gets or sets the reflected ambient light.  For transmissive film, luminance contribution due to reflected ambient light. Expressed as La, in candelas per square meter (cd/m2).
		/// </summary>
		/// <value>The reflected ambient light.</value>
		public ushort ReflectedAmbientLight
		{
			get { return base.DicomAttributeProvider[DicomTags.ReflectedAmbientLight].GetUInt16(0, 0); }
			set { base.DicomAttributeProvider[DicomTags.ReflectedAmbientLight].SetUInt16(0, value); }
		}

		/// <summary>
		/// Gets or sets the requested resolution id.  Specifies the resolution at which images in this Film Box are to be printed.
		/// </summary>
		/// <value>The requested resolution id.</value>
		public RequestedResolution RequestedResolutionId
		{
			get { return ParseEnum<RequestedResolution>(base.DicomAttributeProvider[DicomTags.RequestedResolutionId].GetString(0, String.Empty), RequestedResolution.None); }
			set { SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.RequestedResolutionId], value, false); }
		}

		public SequenceIodList<ReferencedInstanceSequenceIod> ReferencedFilmSessionSequenceList
		{
			get { return new SequenceIodList<ReferencedInstanceSequenceIod>(base.DicomAttributeProvider[DicomTags.ReferencedFilmSessionSequence] as DicomAttributeSQ); }
		}

		public SequenceIodList<ReferencedInstanceSequenceIod> ReferencedImageBoxSequenceList
		{
			get { return new SequenceIodList<ReferencedInstanceSequenceIod>(base.DicomAttributeProvider[DicomTags.ReferencedImageBoxSequence] as DicomAttributeSQ); }
		}

		public SequenceIodList<ReferencedInstanceSequenceIod> ReferencedBasicAnnotationBoxSequenceList
		{
			get { return new SequenceIodList<ReferencedInstanceSequenceIod>(base.DicomAttributeProvider[DicomTags.ReferencedBasicAnnotationBoxSequence] as DicomAttributeSQ); }
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Sets the commonly used tags in the base dicom attribute collection.
		/// </summary>
		public void SetCommonTags()
		{
			SetCommonTags(base.DicomAttributeProvider);
		}

		#endregion

		#region Public Static Methods

		/// <summary>
		/// Sets the commonly used tags in the specified dicom attribute collection.
		/// </summary>
		public static void SetCommonTags(IDicomAttributeProvider dicomAttributeProvider)
		{
			if (dicomAttributeProvider == null)
				throw new ArgumentNullException("dicomAttributeProvider");

			//dicomAttributeProvider[DicomTags.NumberOfCopies].SetNullValue();
			//dicomAttributeProvider[DicomTags.PrintPriority].SetNullValue();
			//dicomAttributeProvider[DicomTags.MediumType].SetNullValue();
			//dicomAttributeProvider[DicomTags.FilmDestination].SetNullValue();
			//dicomAttributeProvider[DicomTags.FilmSessionLabel].SetNullValue();
			//dicomAttributeProvider[DicomTags.MemoryAllocation].SetNullValue();
			//dicomAttributeProvider[DicomTags.OwnerId].SetNullValue();
		}

		#endregion
	}

	//TODO (CR March 2011): Useful as a class in Common (eventually)?

	/// <summary>
	/// Defines various length in millimeters.
	/// </summary>
	public static class LengthInMillimeter
	{
		public const float Inch = 25.4f; // 1 inch = 25.4 mm exactly.  There is no more decimal places to follow
		public const float Foot = 12*Inch;
		public const float Yard = 3*Foot;
	}

	#region FilmOrientation Enum

	/// <summary>
	/// enumeration for the Film Orientation
	/// </summary>
	[TypeConverter(typeof (BasicPrintEnumConverter<FilmOrientation>))]
	public enum FilmOrientation
	{
		/// <summary>
		/// 
		/// </summary>
		None,
		/// <summary>
		/// vertical film position
		/// </summary>
		Portrait,
		/// <summary>
		/// horizontal film position
		/// </summary>
		Landscape
	}

	#endregion

	#region MagnificationType Enum

	/// <summary>
	/// Magnification type enum.  Interpolation type by which the printer magnifies or decimates the image in order to fit the image in the
	/// image box on film.
	/// </summary>
	[TypeConverter(typeof (BasicPrintEnumConverter<MagnificationType>))]
	public enum MagnificationType
	{
		/// <summary>
		/// 
		/// </summary>
		None,
		/// <summary>
		/// 
		/// </summary>
		Replicate,
		/// <summary>
		/// 
		/// </summary>
		Bilinear,
		/// <summary>
		/// 
		/// </summary>
		Cubic
	}

	#endregion

	#region BorderDensity

	/// <summary>
	/// Defines the border density.  Density of the film areas surrounding and between images on the film.
	/// Defined Terms: Black, White or i, where i represents the desired density in hundreds of OD.
	/// </summary>
	[TypeConverter(typeof (BasicPrintEnumConverter<BorderDensity>))]
	public enum BorderDensity
	{
		None,
		Black,
		White
	}

	#endregion

	#region EmptyImageDensity Enum

	/// <summary>
	/// Defines the empty image density.  Density of the image box area on the film that contains no image.
	/// Defined Terms: Black, White or i, where i represents the desired density in hundreds of OD.
	/// </summary>
	[TypeConverter(typeof (BasicPrintEnumConverter<EmptyImageDensity>))]
	public enum EmptyImageDensity
	{
		None,
		Black,
		White
	}

	#endregion

	#region SmoothingType Enum

	/// <summary>
	/// Further specifies the type of the interpolation function. Values are defined in Conformance Statement.
	/// </summary>
	[TypeConverter(typeof (BasicPrintEnumConverter<SmoothingType>))]
	public enum SmoothingType
	{
		/// <summary>
		/// 
		/// </summary>
		None,
		/// <summary>
		/// Only valid for Magnification Type
		/// </summary>
		Cubic
	}

	#endregion

	#region DicomBoolean Enum

	public enum DicomBoolean
	{
		None,
		Yes,
		No
	}

	#endregion

	#region RequestedResolution Enum

	/// <summary>
	/// Specifies the resolution at which images in this Film Box are to be printed.
	/// </summary>
	[TypeConverter(typeof (BasicPrintEnumConverter<RequestedResolution>))]
	public enum RequestedResolution
	{
		/// <summary>
		/// 
		/// </summary>
		None,
		/// <summary>
		/// approximately 4k x 5k printable pixels on a 14 x 17 inch film
		/// </summary>
		Standard,
		/// <summary>
		/// Approximately twice the resolution of STANDARD.
		/// </summary>
		High
	}

	#endregion

	#region ImageDisplayFormat class

	[TypeConverter(typeof (DisplayValueConverter))]
	public class ImageDisplayFormat
	{
		#region Type Converter

		public class DisplayValueConverter : TypeConverter
		{
			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
			{
				if (destinationType == typeof (string))
					return true;

				return base.CanConvertTo(context, destinationType);
			}

			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
			{
				if (value is ImageDisplayFormat && destinationType == typeof (string))
					return GetDisplayString(value as ImageDisplayFormat);

				return base.ConvertTo(context, culture, value, destinationType);
			}

			public string GetDisplayString(ImageDisplayFormat imageDisplayFormat)
			{
				var formatProperCase = GetProperCasing(imageDisplayFormat.Format.ToString());
				switch (imageDisplayFormat.Format)
				{
					case FormatEnum.STANDARD:
						return string.Format(SR.FormatImageDisplayFormatStandard,
						                     imageDisplayFormat.Modifiers[0],
						                     imageDisplayFormat.Modifiers[1]);

					case FormatEnum.ROW:
						return string.Format(SR.FormatImageDisplayFormatRow,
						                     StringUtilities.Combine(imageDisplayFormat.Modifiers, ", "));

					case FormatEnum.COL:
						return string.Format(SR.FormatImageDisplayFormatColumn,
						                     StringUtilities.Combine(imageDisplayFormat.Modifiers, ", "));

					case FormatEnum.SLIDE:
					case FormatEnum.SUPERSLIDE:
					case FormatEnum.CUSTOM:
					default:
						return formatProperCase;
				}
			}

			private static string GetProperCasing(string input)
			{
				if (string.IsNullOrEmpty(input))
					return input;

				if (input.Length == 1)
					return input.ToUpper();

				return string.Format("{0}{1}",
				                     input[0].ToString().ToUpper(),
				                     input.Substring(1).ToLower());
			}
		}

		#endregion

		public class InvalidFormatException : Exception
		{
			public InvalidFormatException(string dicomString)
				: base(string.Format(SR.ExceptionInvalidImageDisplayFormat, dicomString)) {}

			public InvalidFormatException(string dicomString, Exception exception)
				: base(string.Format(SR.ExceptionInvalidImageDisplayFormat, dicomString), exception) {}
		}

		/// <summary>
		/// Type of image display format. Enumerated Values:
		/// <para>
		/// STANDARD\C,R : film contains equal size rectangular image boxes with R rows of image boxes and C columns of image boxes; C and R are integers.
		/// </para>
		/// <para>
		/// ROW\R1,R2,R3, etc. : film contains rows with equal size rectangular image boxes with R1 image boxes in the first row, R2 image boxes in second row, 
		/// R3 image boxes in third row, etc.; R1, R2, R3, etc. are integers.
		/// </para>
		/// <para>
		/// COL\C1,C2,C3, etc.: film contains columns with equal size rectangular image boxes with C1 image boxes in the first column, C2 image boxes in second
		///  column, C3 image boxes in third column, etc.; C1, C2, C3, etc. are integers.
		/// </para>
		/// <para>
		/// SLIDE : film contains 35mm slides; the number of slides for a particular film size is configuration dependent.
		/// </para>
		/// <para>
		/// SUPERSLIDE : film contains 40mm slides; the number of slides for a particular film size is configuration dependent.
		/// </para>
		/// <para>
		/// CUSTOM\i : film contains a customized ordering of rectangular image boxes; i identifies the image display format; the definition of the image display
		/// formats is defined in the Conformance Statement; i is an integer.
		/// </para>
		/// </summary>
		/// <value></value>
		public enum FormatEnum
		{
			STANDARD,
			ROW,
			COL,
			SLIDE,
			SUPERSLIDE,
			CUSTOM
		}

		// Predefined formats
		public static ImageDisplayFormat Standard_1x1 = FromDicomString(@"STANDARD\1,1");
		public static ImageDisplayFormat Standard_1x2 = FromDicomString(@"STANDARD\1,2");
		public static ImageDisplayFormat Standard_2x1 = FromDicomString(@"STANDARD\2,1");
		public static ImageDisplayFormat Standard_2x2 = FromDicomString(@"STANDARD\2,2");
		public static ImageDisplayFormat Standard_2x4 = FromDicomString(@"STANDARD\2,4");
		public static ImageDisplayFormat Standard_4x1 = FromDicomString(@"STANDARD\4,1");
		public static ImageDisplayFormat Standard_4x2 = FromDicomString(@"STANDARD\4,2");
		public static ImageDisplayFormat Standard_4x4 = FromDicomString(@"STANDARD\4,4");
		public static ImageDisplayFormat Row_1_2 = FromDicomString(@"ROW\1,2");
		public static ImageDisplayFormat COL_1_2 = FromDicomString(@"COL\1,2");

		// A list of standard formats that is supported by CC DICOM Print
		public static List<ImageDisplayFormat> StandardFormats = new List<ImageDisplayFormat>
		                                                         	{
		                                                         		Standard_1x1, Standard_1x2,
		                                                         		Standard_2x1, Standard_2x2, Standard_2x4,
		                                                         		Standard_4x1, Standard_4x2, Standard_4x4,
		                                                         		Row_1_2,
		                                                         		COL_1_2
		                                                         	};

		public static ImageDisplayFormat FromDicomString(string dicomString)
		{
			try
			{
				if (string.IsNullOrEmpty(dicomString))
					return null;

				var indexOfSeparator = dicomString.IndexOf(@"\");
				var formatString = indexOfSeparator >= 0
				                   	? dicomString.Substring(0, indexOfSeparator)
				                   	: dicomString;
				var format = (FormatEnum) Enum.Parse(typeof (FormatEnum), formatString);
				if (format == FormatEnum.SLIDE || format == FormatEnum.SUPERSLIDE || format == FormatEnum.CUSTOM)
					throw new NotSupportedException(string.Format(SR.ExceptionNotSupportedImageDisplayFormat, formatString));

				var commaSeparatedModifiers = indexOfSeparator >= 0
				                              	? dicomString.Substring(indexOfSeparator + 1)
				                              	: "";
				var modifierTokens = StringUtilities.SplitQuoted(commaSeparatedModifiers, ",");

				if (format == FormatEnum.STANDARD && modifierTokens.Length != 2 ||
				    format == FormatEnum.ROW && modifierTokens.Length < 2 ||
				    format == FormatEnum.COL && modifierTokens.Length < 2)
					throw new InvalidFormatException(dicomString);

				var imageDisplayFormat = new ImageDisplayFormat
				                         	{
				                         		_dicomString = dicomString,
				                         		Format = format,
				                         		Modifiers = CollectionUtils.Map<string, int>(modifierTokens, m => int.Parse(m))
				                         	};

				return imageDisplayFormat;
			}
			catch (NotSupportedException)
			{
				throw;
			}
			catch (InvalidFormatException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new InvalidFormatException(dicomString, e);
			}
		}

		private string _dicomString;

		#region Serialization

		/// <summary>
		/// Constructor for serialization.
		/// </summary>
		public ImageDisplayFormat() {}

		/// <summary>
		/// For serialization purpose, the is the only public property.  The setter should not be used.
		/// Instead, use <see cref="FromDicomString"/> to create a new/different FilmSize object.
		/// </summary>
		public string DicomString
		{
			get { return _dicomString; }
			set
			{
				var imageDisplayFormat = FromDicomString(value);
				_dicomString = imageDisplayFormat._dicomString;
				this.Format = imageDisplayFormat.Format;
				this.Modifiers = imageDisplayFormat.Modifiers;
			}
		}

		#endregion

		[XmlIgnore]
		public FormatEnum Format { get; private set; }

		[XmlIgnore]
		public List<int> Modifiers { get; private set; }

		[XmlIgnore]
		public int MaximumImageBoxes
		{
			get
			{
				if (this.Modifiers.Count == 0)
					return 1;

				switch (this.Format)
				{
					case FormatEnum.STANDARD:
						return this.Modifiers[0]*this.Modifiers[1];
					case FormatEnum.ROW:
					case FormatEnum.COL:
						return CollectionUtils.Reduce<int, int>(this.Modifiers, 0, (m, sum) => sum + m);

					case FormatEnum.SLIDE:
					case FormatEnum.SUPERSLIDE:
					case FormatEnum.CUSTOM:
					default:
						throw new NotSupportedException(string.Format("{0} image display format is not supported", this.Format));
				}
			}
		}
	}

	#endregion

	#region FilmSize

	/// <summary>
	/// Film size identification.
	/// </summary>
	[TypeConverter(typeof (DisplayValueConverter))]
	public class FilmSize
	{
		#region TypeConverter

		public class DisplayValueConverter : TypeConverter
		{
			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
			{
				if (destinationType == typeof (string))
					return true;

				return base.CanConvertTo(context, destinationType);
			}

			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
			{
				if (value is FilmSize && destinationType == typeof (string))
					return GetDisplayString(value as FilmSize);

				return base.ConvertTo(context, culture, value, destinationType);
			}

			private static string GetDisplayString(FilmSize filmSize)
			{
				var builder = new StringBuilder();
				builder.AppendFormat(SR.FormatFilmSize,
				                     filmSize._width,
				                     filmSize._height,
				                     filmSize._sizeUnit == FilmSizeUnit.Inch ? SR.LabelFilmSizeUnitInch : SR.LabelFilmSizeUnitCentimeter);

				return filmSize.DicomString == "A3" || filmSize.DicomString == "A4"
				       	? string.Format("{0} ({1})", filmSize.DicomString, builder)
				       	: builder.ToString();
			}
		}

		#endregion

		public class InvalidFilmSizeException : Exception
		{
			public InvalidFilmSizeException(string dicomString)
				: base(string.Format(SR.ExceptionInvalidFilmSize, dicomString)) {}

			public InvalidFilmSizeException(string dicomString, Exception exception)
				: base(string.Format(SR.ExceptionInvalidFilmSize, dicomString), exception) {}
		}

		// Predefined formats
		public static readonly FilmSize Dimension_8in_x_10in = FromDicomString("8INX10IN");
		public static readonly FilmSize Dimension_8_5in_x_11in = FromDicomString("8_5INX11IN");
		public static readonly FilmSize Dimension_10in_x_12in = FromDicomString("10INX12IN");
		public static readonly FilmSize Dimension_10in_x_14in = FromDicomString("10INX14IN"); //  corresponds with 25.7CMX36.4CM
		public static readonly FilmSize Dimension_11in_x_14in = FromDicomString("11INX14IN");
		public static readonly FilmSize Dimension_11in_x_17in = FromDicomString("11INX17IN");
		public static readonly FilmSize Dimension_14in_x_14in = FromDicomString("14INX14IN");
		public static readonly FilmSize Dimension_14in_x_17in = FromDicomString("14INX17IN");
		public static readonly FilmSize Dimension_24cm_x_24cm = FromDicomString("24CMX24CM");
		public static readonly FilmSize Dimension_24cm_x_30cm = FromDicomString("24CMX30CM");
		public static readonly FilmSize A3 = FromDicomString("A3"); // 297mm x 420mm
		public static readonly FilmSize A4 = FromDicomString("A4"); // 210mm x 297mm

		/// <summary>
		/// Gets a list of standard film sizes defined in the DICOM Standard.
		/// </summary>
		public static readonly IList<FilmSize> StandardFilmSizes = new List<FilmSize>
		                                                           	{
		                                                           		Dimension_8in_x_10in, Dimension_8_5in_x_11in,
		                                                           		Dimension_10in_x_12in, Dimension_10in_x_14in,
		                                                           		Dimension_11in_x_14in, Dimension_11in_x_17in,
		                                                           		Dimension_14in_x_14in, Dimension_14in_x_17in,
		                                                           		Dimension_24cm_x_24cm, Dimension_24cm_x_30cm,
		                                                           		A3, A4
		                                                           	}.AsReadOnly();

		// TODO CR (Mar 2013): Rewrite in a way that isn't so dependent on replacing characters then doing a parse of sorts, especially when the strings are hardcoded
		public static FilmSize FromDicomString(string dicomString)
		{
			try
			{
				if (string.IsNullOrEmpty(dicomString))
					return null;

				if (dicomString == "A3")
					return new FilmSize
					       	{
					       		_dicomString = dicomString,
					       		_sizeUnit = FilmSizeUnit.Centimeter,
					       		_width = 29.7f,
					       		_height = 42.0f
					       	};

				if (dicomString == "A4")
					return new FilmSize
					       	{
					       		_dicomString = dicomString,
					       		_sizeUnit = FilmSizeUnit.Centimeter,
					       		_width = 21.0f,
					       		_height = 29.7f
					       	};

				var indexOfX = dicomString.IndexOf('X');
				if (indexOfX < 3) // at least one char for width, and 2 chars for units
					throw new InvalidFilmSizeException(dicomString);

				var firstUnit = dicomString.Substring(indexOfX - 2, 2);
				var secondUnit = dicomString.Substring(dicomString.Length - 2);
				if (firstUnit != "CM" && firstUnit != "IN")
					throw new InvalidFilmSizeException(dicomString);
				if (firstUnit != secondUnit)
					throw new InvalidFilmSizeException(dicomString);

				var xSeparatedDimension = dicomString.Replace("IN", "").Replace("CM", "").Replace('_', '.');
				var dimensions = StringUtilities.SplitQuoted(xSeparatedDimension, "X");

				var filmSize = new FilmSize
				               	{
				               		_dicomString = dicomString,
				               		_sizeUnit = firstUnit == "IN" ? FilmSizeUnit.Inch : FilmSizeUnit.Centimeter,
				               		_width = float.Parse(dimensions[0], CultureInfo.InvariantCulture),
				               		_height = float.Parse(dimensions[1], CultureInfo.InvariantCulture),
				               	};
				return filmSize;
			}
			catch (NotSupportedException)
			{
				throw;
			}
			catch (InvalidFilmSizeException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new InvalidFilmSizeException(dicomString, e);
			}
		}

		public enum FilmSizeUnit
		{
			Inch,
			Centimeter,
		}

		private string _dicomString;
		private FilmSizeUnit _sizeUnit;
		private float _width;
		private float _height;

		#region Serialization

		/// <summary>
		/// Empty constructor for serialization
		/// </summary>
		public FilmSize() {}

		/// <summary>
		/// For serialization purpose, the is the only public property.  The setter should not be used.
		/// Instead, use <see cref="FromDicomString"/> to create a new/different FilmSize object.
		/// </summary>
		public string DicomString
		{
			get { return _dicomString; }
			set
			{
				var filmSize = FromDicomString(value);
				_dicomString = filmSize._dicomString;
				_sizeUnit = filmSize._sizeUnit;
				_width = filmSize._width;
				_height = filmSize._height;
			}
		}

		#endregion

		#region Public Methods

		public float GetHeight(FilmSizeUnit desiredUnit)
		{
			if (_sizeUnit == desiredUnit)
				return _height;

			return desiredUnit == FilmSizeUnit.Inch
			       	? ConvertToInches(_height)
			       	: ConvertToCentimeters(_height);
		}

		public float GetWidth(FilmSizeUnit desiredUnit)
		{
			if (_sizeUnit == desiredUnit)
				return _width;

			return desiredUnit == FilmSizeUnit.Inch
			       	? ConvertToInches(_width)
			       	: ConvertToCentimeters(_width);
		}

		public override int GetHashCode()
		{
			var hashCode = 0x126F24A2;
			var dicomString = DicomString;
			if (!string.IsNullOrEmpty(dicomString))
				hashCode ^= dicomString.GetHashCode();
			return hashCode;
		}

		public override bool Equals(object obj)
		{
			if (obj is FilmSize)
				return Equals(DicomString, ((FilmSize) obj).DicomString);
			return false;
		}

		public override string ToString()
		{
			return DicomString;
		}

		#endregion

		#region Private Methods

		private static float ConvertToCentimeters(float inches)
		{
			return (inches*LengthInMillimeter.Inch)/10.0f;
		}

		private static float ConvertToInches(float cm)
		{
			return 10.0f*cm/LengthInMillimeter.Inch;
		}

		#endregion
	}

	public class BasicPrintEnumConverter<TEnumType> : TypeConverter
	{
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof (string))
				return true;

			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (value is TEnumType && destinationType == typeof (string))
			{
				if (value.ToString() == "None")
					return SR.LabelDefault;

				if (value is PrintPriority)
					return ConvertPrintPriority((PrintPriority) value);

				if (value is MagnificationType)
					return ConvertMagnificationType((MagnificationType) value);

				if (value is SmoothingType)
					return ConvertSmoothingType((SmoothingType) value);

				if (value is EmptyImageDensity)
					return ConvertEmptyImageDensity((EmptyImageDensity) value);

				if (value is BorderDensity)
					return ConvertBorderDensity((BorderDensity) value);

				if (value is RequestedResolution)
					return ConvertRequestedResolution((RequestedResolution) value);

				if (value is MediumType)
					return ConvertMediumType((MediumType) value);

				if (value is FilmDestination)
					return ConvertFilmDestination((FilmDestination) value);

				if (value is FilmOrientation)
					return ConvertFilmOrientation((FilmOrientation) value);
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}

		private static string ConvertPrintPriority(PrintPriority printPriority)
		{
			switch (printPriority)
			{
				case PrintPriority.High:
					return SR.LabelPrintPriorityHigh;
				case PrintPriority.Med:
					return SR.LabelPrintPriorityMedium;
				case PrintPriority.Low:
					return SR.LabelPrintPriorityLow;
				default:
					return printPriority.ToString();
			}
		}

		private static string ConvertMagnificationType(MagnificationType magnificationType)
		{
			switch (magnificationType)
			{
				case MagnificationType.Bilinear:
					return SR.LabelFilmMagnificationTypeBilinear;
				case MagnificationType.Cubic:
					return SR.LabelFilmMagnificationTypeCubic;
				case MagnificationType.Replicate:
					return SR.LabelFilmMagnificationTypeReplicate;
				default:
					return magnificationType.ToString();
			}
		}

		private static string ConvertSmoothingType(SmoothingType smoothingType)
		{
			switch (smoothingType)
			{
				case SmoothingType.Cubic:
					return SR.LabelFilmSmoothingTypeCubic;
				default:
					return smoothingType.ToString();
			}
		}

		private static string ConvertEmptyImageDensity(EmptyImageDensity emptyImageDensity)
		{
			switch (emptyImageDensity)
			{
				case EmptyImageDensity.Black:
					return SR.LabelFilmDensityBlack;
				case EmptyImageDensity.White:
					return SR.LabelFilmDensityWhite;
				default:
					return emptyImageDensity.ToString();
			}
		}

		private static string ConvertBorderDensity(BorderDensity borderDensity)
		{
			switch (borderDensity)
			{
				case BorderDensity.Black:
					return SR.LabelFilmDensityBlack;
				case BorderDensity.White:
					return SR.LabelFilmDensityWhite;
				default:
					return borderDensity.ToString();
			}
		}

		private static string ConvertRequestedResolution(RequestedResolution requestedResolution)
		{
			switch (requestedResolution)
			{
				case RequestedResolution.Standard:
					return SR.LabelFilmRequestedResolutionStandard;
				case RequestedResolution.High:
					return SR.LabelFilmRequestedResolutionHigh;
				default:
					return requestedResolution.ToString();
			}
		}

		private static string ConvertMediumType(MediumType mediumType)
		{
			switch (mediumType)
			{
				case MediumType.Paper:
					return SR.LabelFilmMediumTypePaper;
				case MediumType.ClearFilm:
					return SR.LabelFilmMediumTypeClearFilm;
				case MediumType.BlueFilm:
					return SR.LabelFilmMediumTypeBlueFilm;
				case MediumType.MammoClearFilm:
					return SR.LabelFilmMediumTypeMammoClearFilm;
				case MediumType.MammoBlueFilm:
					return SR.LabelFilmMediumTypeMammoBlueFilm;
				default:
					return mediumType.ToString();
			}
		}

		private static string ConvertFilmDestination(FilmDestination filmDestination)
		{
			switch (filmDestination)
			{
				case FilmDestination.Magazine:
					return SR.LabelFilmDestinationMagazine;
				case FilmDestination.Processor:
					return SR.LabelFilmDestinationProcessor;
				case FilmDestination.Bin_0:
					return string.Format(SR.LabelFilmDestinationBinN, 0);
				case FilmDestination.Bin_1:
					return string.Format(SR.LabelFilmDestinationBinN, 1);
				case FilmDestination.Bin_2:
					return string.Format(SR.LabelFilmDestinationBinN, 2);
				case FilmDestination.Bin_3:
					return string.Format(SR.LabelFilmDestinationBinN, 3);
				case FilmDestination.Bin_4:
					return string.Format(SR.LabelFilmDestinationBinN, 4);
				case FilmDestination.Bin_5:
					return string.Format(SR.LabelFilmDestinationBinN, 5);
				case FilmDestination.Bin_6:
					return string.Format(SR.LabelFilmDestinationBinN, 6);
				case FilmDestination.Bin_7:
					return string.Format(SR.LabelFilmDestinationBinN, 7);
				case FilmDestination.Bin_8:
					return string.Format(SR.LabelFilmDestinationBinN, 8);
				case FilmDestination.Bin_9:
					return string.Format(SR.LabelFilmDestinationBinN, 9);
				default:
					return filmDestination.ToString();
			}
		}

		private static string ConvertFilmOrientation(FilmOrientation filmOrientation)
		{
			switch (filmOrientation)
			{
				case FilmOrientation.Portrait:
					return SR.LabelFilmOrientationPortrait;
				case FilmOrientation.Landscape:
					return SR.LabelFilmOrientationLandscape;
				default:
					return filmOrientation.ToString();
			}
		}
	}

	#endregion
}