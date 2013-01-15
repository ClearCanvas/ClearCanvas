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

using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	[Cloneable]
	public class PETFusionDisplaySetDescriptor : DicomDisplaySetDescriptor, IFusionDisplaySetDescriptor
	{
		[CloneCopyReference]
		private readonly ISeriesIdentifier _petSeries;

		private readonly bool _attenuationCorrection;
		private readonly string _fusionSeriesInstanceUid;

		public PETFusionDisplaySetDescriptor(ISeriesIdentifier baseSeries, ISeriesIdentifier ptSeries, bool attenuationCorrection)
			: base(baseSeries, null)
		{
			_petSeries = ptSeries;
			_attenuationCorrection = attenuationCorrection;
			_fusionSeriesInstanceUid = DicomUid.GenerateUid().UID;

		    IsComposite = true;
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected PETFusionDisplaySetDescriptor(PETFusionDisplaySetDescriptor source, ICloningContext context) : base(source, context)
		{
			context.CloneFields(source, this);
		}

		public ISeriesIdentifier PETSeries
		{
			get { return _petSeries; }
		}

		ISeriesIdentifier IFusionDisplaySetDescriptor.OverlaySeries
		{
			get { return PETSeries; }
		}

		public bool AttenuationCorrection
		{
			get { return _attenuationCorrection; }
		}

		protected override string GetName()
		{
			return string.Format(SR.FormatPETFusionDisplaySet, GetSeriesDisplaySetName(base.SourceSeries), GetSeriesDisplaySetName(this.PETSeries),
			                     GetAttentuationCorrectionLabel()
				);
		}

		protected override string GetDescription()
		{
			return string.Format(SR.FormatPETFusionDisplaySet, GetSeriesDisplaySetDescription(base.SourceSeries), GetSeriesDisplaySetDescription(this.PETSeries),
			                     GetAttentuationCorrectionLabel()
				);
		}

		protected override string GetUid()
		{
			return _fusionSeriesInstanceUid;
		}

		private string GetAttentuationCorrectionLabel()
		{
			return AttenuationCorrection ? SR.LabelAttenuationCorrection : SR.LabelNoAttentuationCorrection;
		}

		private static string GetSeriesDisplaySetName(ISeriesIdentifier series)
		{
			if (string.IsNullOrEmpty(series.SeriesDescription))
				return string.Format("{0}", series.SeriesNumber);
			else
				return string.Format("{0} - {1}", series.SeriesNumber, series.SeriesDescription);
		}

		private static string GetSeriesDisplaySetDescription(ISeriesIdentifier series)
		{
			if (string.IsNullOrEmpty(series.SeriesDescription))
				return string.Format("{0}", series.SeriesNumber);
			else
				return string.Format("{0}", series.SeriesDescription);
		}
	}
}