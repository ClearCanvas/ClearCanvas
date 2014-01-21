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
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Definition of an <see cref="IDisplaySetDescriptor"/> whose contents are based on
	/// a DICOM Series.
	/// </summary>
	public interface IDicomDisplaySetDescriptor : IDisplaySetDescriptor
	{
		/// <summary>
		/// Gets the <see cref="ISeriesIdentifier"/> for the series used to 
		/// generate the <see cref="IDisplaySet"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		///  The series used to create the <see cref="IDisplaySet"/> is not necessarily
		/// the same as the series that the contained <see cref="IPresentationImage"/>s
		/// belong to.  For example, the <see cref="SourceSeries"/> could be a Key Object
		/// series that simply references images in other series.
		/// </para>
		/// <para>
		/// Additionally, this property can also be null if, for example, the images are
		/// a composition of images from many series in the same study.
		/// </para>
		/// </remarks>
		ISeriesIdentifier SourceSeries { get; }

		//TODO (CR July 2011): Not sure about this ...

		/// <summary>
		/// Gets whether or not the display set is comprised of simple DICOM images, taken from a single
		/// series, or whether it is a composite of images from multiple series.
		/// </summary>
		bool IsComposite { get; }

		/// <summary>
		/// Gets whether or not the pixel data in the display set is derived in some manner from the pixel data of another
		/// (i.e. the image type of the images in the display set is effectively DERIVED rather than ORIGINAL)
		/// </summary>
		bool IsDerived { get; }

		/// <summary>
		/// Gets the original <see cref="IDisplaySetDescriptor"/> of the display set from which this display set was derived.
		/// </summary>
		IDisplaySetDescriptor DerivationSourceDescriptor { get; }

		//TODO: put this stuff back when we actually support dynamically updating the viewer.
		//bool Update(Sop sop);
	}

	/// <summary>
	/// Definition of an object that describes the contents of an <see cref="IDisplaySet"/>.
	/// </summary>
	public interface IDisplaySetDescriptor
	{
		/// <summary>
		/// Gets the descriptive name of the <see cref="IDisplaySet"/>.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets a description of the <see cref="IDisplaySet"/>.
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Gets a numeric identifier for the <see cref="IDisplaySet"/>, which usually corresponds
		/// to a DICOM Series Number.
		/// </summary>
		int Number { get; }

		/// <summary>
		/// Gets the unique identifier for the <see cref="IDisplaySet"/>.
		/// </summary>
		string Uid { get; }
	}

	/// <summary>
	/// Abstract base implementation of an <see cref="IDicomDisplaySetDescriptor"/>.
	/// </summary>
	[Cloneable(false)]
	public abstract class DicomDisplaySetDescriptor : DisplaySetDescriptor, IDicomDisplaySetDescriptor
	{
		[CloneCopyReference]
		private readonly ISeriesIdentifier _sourceSeries;

		[CloneCopyReference]
		private readonly IPresentationImageFactory _presentationImageFactory;

		[CloneCopyReference]
		private IDisplaySetDescriptor _derivationSourceDescriptor;

		private string _name;
		private string _description;
		private int? _number;
		private string _uid;

		/// <summary>
		/// Protected constructor.
		/// </summary>
		protected DicomDisplaySetDescriptor(ISeriesIdentifier sourceSeries)
			: this(sourceSeries, null) {}

		/// <summary>
		/// Protected constructor.
		/// </summary>
		protected DicomDisplaySetDescriptor(ISeriesIdentifier sourceSeries, IPresentationImageFactory presentationImageFactory)
		{
			_sourceSeries = sourceSeries;
			_presentationImageFactory = presentationImageFactory;
		}

		/// <summary>
		/// Protected constructor.
		/// </summary>
		protected DicomDisplaySetDescriptor(DicomDisplaySetDescriptor source, ICloningContext context)
		{
			context.CloneFields(source, this);
		}

		#region IDicomDisplaySetDescriptor Members

		/// <summary>
		/// Gets the <see cref="ISeriesIdentifier"/> for the series used to 
		/// generate the <see cref="IDisplaySet"/>.
		/// </summary>
		/// <remarks>
		/// The series used to create the <see cref="IDisplaySet"/> is not necessarily
		/// the same as the series that the contained <see cref="IPresentationImage"/>s
		/// belong to.  For example, the <see cref="IDicomDisplaySetDescriptor.SourceSeries"/> could be a Key Object
		/// series that simply references images in other series.
		/// </remarks>
		public ISeriesIdentifier SourceSeries
		{
			get { return _sourceSeries; }
		}

		public bool IsComposite { get; protected set; }

		public bool IsDerived { get; protected set; }

		public IDisplaySetDescriptor DerivationSourceDescriptor
		{
			get { return _derivationSourceDescriptor; }
			protected set { _derivationSourceDescriptor = value; }
		}

		#endregion

		/// <summary>
		/// Gets the descriptive name of the <see cref="IDisplaySet"/>.
		/// </summary>
		public override string Name
		{
			get
			{
				if (_name == null)
					_name = GetName() ?? "";
				return _name;
			}
			set { throw new InvalidOperationException("The Name property cannot be set publicly."); }
		}

		/// <summary>
		/// Gets a description of the <see cref="IDisplaySet"/>.
		/// </summary>
		public override string Description
		{
			get
			{
				if (_description == null)
					_description = GetDescription() ?? "";
				return _description;
			}
			set { throw new InvalidOperationException("The Description property cannot be set publicly."); }
		}

		/// <summary>
		/// Gets the unique identifier for the <see cref="IDisplaySet"/>.
		/// </summary>
		public override string Uid
		{
			get
			{
				if (_uid == null)
					_uid = GetUid() ?? "";
				return _uid;
			}
			set { throw new InvalidOperationException("The Uid property cannot be set publicly."); }
		}

		/// <summary>
		/// Gets a numeric identifier for the <see cref="IDisplaySet"/>, which usually corresponds
		/// to a DICOM Series Number.
		/// </summary>
		public override int Number
		{
			get
			{
				if (!_number.HasValue)
					_number = GetNumber();
				return _number.Value;
			}
			set { throw new InvalidOperationException("The Uid property cannot be set publicly."); }
		}

		/// <summary>
		/// Gets the descriptive name of the <see cref="IDisplaySet"/>.
		/// </summary>
		protected abstract string GetName();

		/// <summary>
		/// Gets a description of the <see cref="IDisplaySet"/>.
		/// </summary>
		protected abstract string GetDescription();

		/// <summary>
		/// Gets the unique identifier for the <see cref="IDisplaySet"/>.
		/// </summary>
		protected abstract string GetUid();

		/// <summary>
		/// Gets a numeric identifier for the <see cref="IDisplaySet"/>, which usually corresponds
		/// to a DICOM Series Number.
		/// </summary>
		protected virtual int GetNumber()
		{
			if (SourceSeries == null)
				return default(int);

			return SourceSeries.SeriesNumber ?? default(int);
		}

		internal virtual bool ShouldAddSop(Sop sop)
		{
			return false;
		}

		//Add this when we actually support it.
		internal virtual bool Update(Sop sop)
		{
			throw new NotSupportedException();
		}
	}

	/// <summary>
	/// Default implementation of <see cref="IDisplaySetDescriptor"/>.
	/// </summary>
	[Cloneable(true)]
	public class BasicDisplaySetDescriptor : DisplaySetDescriptor
	{
		private string _name = "";
		private string _description = "";
		private int _number;
		private string _uid = "";

		/// <summary>
		/// Constructor.
		/// </summary>
		public BasicDisplaySetDescriptor() {}

		/// <summary>
		/// Gets or sets the descriptive name of the <see cref="IDisplaySet"/>.
		/// </summary>
		public override string Name
		{
			get { return _name ?? ""; }
			set { _name = value; }
		}

		/// <summary>
		/// Gets or sets the description of the <see cref="IDisplaySet"/>.
		/// </summary>
		public override string Description
		{
			get { return _description ?? ""; }
			set { _description = value; }
		}

		/// <summary>
		/// Gets or sets the numeric identifier for the <see cref="IDisplaySet"/>.
		/// </summary>
		public override int Number
		{
			get { return _number; }
			set { _number = value; }
		}

		/// <summary>
		/// Gets or sets the unique identifier for the <see cref="IDisplaySet"/>.
		/// </summary>
		public override string Uid
		{
			get { return _uid ?? ""; }
			set { _uid = value; }
		}
	}

	/// <summary>
	/// Abstract base implementation of <see cref="IDisplaySetDescriptor"/>.
	/// </summary>
	[Cloneable(true)]
	public abstract class DisplaySetDescriptor : IDisplaySetDescriptor
	{
		/// <summary>
		/// Protected constructor.
		/// </summary>
		protected DisplaySetDescriptor() {}

		#region IDisplaySetDescriptor Members

		/// <summary>
		/// Gets the descriptive name of the <see cref="IDisplaySet"/>.
		/// </summary>
		public abstract string Name { get; set; }

		/// <summary>
		/// Gets a description of the <see cref="IDisplaySet"/>.
		/// </summary>
		public abstract string Description { get; set; }

		/// <summary>
		/// Gets a numeric identifier for the <see cref="IDisplaySet"/>, which usually corresponds
		/// to a DICOM Series Number.
		/// </summary>
		public abstract int Number { get; set; }

		/// <summary>
		/// Gets the unique identifier for the <see cref="IDisplaySet"/>.
		/// </summary>
		public abstract string Uid { get; set; }

		#endregion

		///<summary>
		/// Creates a copy of this object.
		///</summary>
		public DisplaySetDescriptor Clone()
		{
			return (DisplaySetDescriptor) CloneBuilder.Clone(this);
		}

		/// <summary>
		/// Gets a text description of this object.
		/// </summary>
		public override string ToString()
		{
			return StringUtilities.Combine(new string[] {Name, Uid}, " | ", true);
		}
	}
}