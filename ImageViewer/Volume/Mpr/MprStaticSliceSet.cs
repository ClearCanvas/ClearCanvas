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
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Volumes;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	/// <summary>
	/// A basic, immutable, single-plane slice view of an MPR <see cref="Volume"/>.
	/// </summary>
	public class MprStaticSliceSet : MprSliceSet, IMprStandardSliceSet
	{
		private readonly IVolumeSlicerParams _slicerParams;

		public MprStaticSliceSet(Volumes.Volume volume, IVolumeSlicerParams slicerParams)
			: base(volume)
		{
			Platform.CheckForNullReference(slicerParams, "slicerParams");
			_slicerParams = slicerParams;

			base.Description = slicerParams.Description;
			this.Reslice();
		}

		public IVolumeSlicerParams SlicerParams
		{
			get { return _slicerParams; }
		}

		bool IMprStandardSliceSet.IsReadOnly
		{
			get { return true; }
		}

		IVolumeSlicerParams IMprStandardSliceSet.SlicerParams
		{
			get { return this.SlicerParams; }
			set { throw new NotSupportedException(); }
		}

		event EventHandler IMprStandardSliceSet.SlicerParamsChanged
		{
			add { }
			remove { }
		}

		protected void Reslice()
		{
			base.SuspendSliceSopsChangedEvent();
			try
			{
				base.ClearAndDisposeSops();

				using (VolumeSlicer slicer = new VolumeSlicer(base.Volume, _slicerParams))
				{
					foreach (ISopDataSource dataSource in slicer.CreateSliceSops(Uid))
					{
						base.SliceSops.Add(new MprSliceSop(dataSource));
					}
				}
			}
			finally
			{
				base.ResumeSliceSopsChangedEvent(true);
			}
		}

		public static MprStaticSliceSet CreateIdentitySliceSet(Volumes.Volume volume)
		{
			return new MprStaticSliceSet(volume, VolumeSlicerParams.Identity);
		}
	}
}