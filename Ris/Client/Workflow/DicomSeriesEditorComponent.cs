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
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Ris.Client.Workflow
{
	/// <summary>
	/// Extension point for views onto <see cref="DicomSeriesEditorComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class DicomSeriesEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// DicomSeriesEditorComponent class.
	/// </summary>
	[AssociateView(typeof(DicomSeriesEditorComponentViewExtensionPoint))]
	public class DicomSeriesEditorComponent : ApplicationComponent
	{
		private readonly DicomSeriesDetail _dicomSeries;
		private bool _isNew;

		public DicomSeriesEditorComponent(DicomSeriesDetail dicomSeries, bool isNew)
		{
			_dicomSeries = dicomSeries;
			_isNew = isNew;
		}

		#region Presentation Model

		public string StudyInstanceUID
		{
			get { return _dicomSeries.StudyInstanceUID; }
		}

		public string SeriesInstanceUID
		{
			get { return _dicomSeries.SeriesInstanceUID; }
		}

		[ValidateNotNull]
		public string SeriesNumber
		{
			get { return _dicomSeries.SeriesNumber; }
			set
			{
				_dicomSeries.SeriesNumber = value;
				this.Modified = true;
			}
		}

		public bool IsSeriesNumberReadOnly
		{
			get { return !_isNew; }
		}

		public string SeriesDescription
		{
			get { return _dicomSeries.SeriesDescription; }
			set
			{
				_dicomSeries.SeriesDescription = value;
				this.Modified = true;
			}
		}

		[ValidateGreaterThan(0)]
		public int NumberOfSeriesRelatedInstances
		{
			get { return _dicomSeries.NumberOfSeriesRelatedInstances; }
			set
			{
				_dicomSeries.NumberOfSeriesRelatedInstances = value;
				this.Modified = true;
			}
		}

		public bool AcceptEnabled
		{
			get { return this.Modified; }
		}

		public void Accept()
		{
			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
				return;
			}

			this.Exit(ApplicationComponentExitCode.Accepted);
		}

		public void Cancel()
		{
			this.Exit(ApplicationComponentExitCode.None);
		}

		#endregion
	}
}
