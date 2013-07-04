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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Volumes;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	public interface IMprStandardSliceSet : IMprSliceSet
	{
		bool IsReadOnly { get; }
		IVolumeSlicerParams SlicerParams { get; set; }
		event EventHandler SlicerParamsChanged;
	}

	/// <summary>
	/// A basic, mutable, single-plane slice view of an MPR <see cref="Volume"/>.
	/// </summary>
	public class MprStandardSliceSet : MprSliceSet, IMprStandardSliceSet
	{
		private event EventHandler _slicerParamsChanged;
		private IVolumeSlicerParams _slicerParams;

		public MprStandardSliceSet(Volumes.Volume volume, IVolumeSlicerParams slicerParams) : base(volume)
		{
			Platform.CheckForNullReference(slicerParams, "slicerParams");
			_slicerParams = slicerParams;

			base.Description = slicerParams.Description;
			this.Reslice();
		}

		bool IMprStandardSliceSet.IsReadOnly
		{
			get { return false; }
		}

		public IVolumeSlicerParams SlicerParams
		{
			get { return _slicerParams; }
			set
			{
				if (_slicerParams != value)
				{
					_slicerParams = value;
					this.OnSlicerParamsChanged();
				}
			}
		}

		public event EventHandler SlicerParamsChanged
		{
			add { _slicerParamsChanged += value; }
			remove { _slicerParamsChanged -= value; }
		}

		protected virtual void OnSlicerParamsChanged()
		{
			this.Reslice();
			base.Description = this.SlicerParams.Description;
			EventsHelper.Fire(_slicerParamsChanged, this, EventArgs.Empty);
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
	}
}