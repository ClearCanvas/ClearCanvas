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
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	public class SopDataSourceStudyItem : StudyItem
	{
		private readonly string _filename;
		private ISopReference _sopReference;

		public SopDataSourceStudyItem(Sop sop)
		{
			Platform.CheckTrue(sop.DataSource is ILocalSopDataSource, "Sop must be local");
			{
				_filename = ((ILocalSopDataSource) sop.DataSource).Filename;
				_sopReference = sop.CreateTransientReference();
			}
		}

		public SopDataSourceStudyItem(ILocalSopDataSource sopDataSource)
		{
			_filename = sopDataSource.Filename;
			using (Sop sop = new Sop(sopDataSource))
			{
				_sopReference = sop.CreateTransientReference();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_sopReference != null)
				{
					_sopReference.Dispose();
					_sopReference = null;
				}
			}
			base.Dispose(disposing);
		}

		public override string Filename
		{
			get { return _filename; }
		}

		public override DicomAttribute this[uint tag]
		{
			get { return _sopReference.Sop[tag]; }
		}
	}

	public class LocalStudyItem : StudyItem
	{
		private readonly string _filename;
		private readonly DicomFile _dcf;

		public LocalStudyItem(string filename)
		{
			_filename = filename;
			_dcf = new DicomFile(filename);
			_dcf.Load(DicomReadOptions.Default | DicomReadOptions.StorePixelDataReferences);
		}

		public override string Filename
		{
			get { return _filename; }
		}

		public override DicomAttribute this[uint tag]
		{
			get
			{
				DicomAttribute attribute;
				if (!_dcf.DataSet.TryGetAttribute(tag, out attribute))
				{
					if (!_dcf.MetaInfo.TryGetAttribute(tag, out attribute))
						return null;
				}
				return attribute;
			}
		}
	}

	public abstract partial class StudyItem : IStudyItem
	{
		protected StudyItem()
		{
			IncrementInstanceCount();
		}

		~StudyItem()
		{
			try
			{
				Dispose(false);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Warn, e);
			}
		}

		public abstract string Filename { get; }

		public abstract DicomAttribute this[uint tag] { get; }

		protected virtual void Dispose(bool disposing) {}

		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Warn, e);
			}

			DecrementInstanceCount();
		}

		static partial void IncrementInstanceCount();
		static partial void DecrementInstanceCount();
	}
}