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
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Volume.Mpr.Utilities;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	public interface IMprVolume : IDisposable
	{
		string Uid { get; }
		string Description { get; }
		Volumes.Volume Volume { get; }
		IList<IMprSliceSet> SliceSets { get; }
	}

	public class MprVolume : IMprVolume
	{
		private readonly string _uid = DicomUid.GenerateUid().UID;

		private readonly string _description;

		private Volumes.Volume _volume;
		private ObservableDisposableList<IMprSliceSet> _sliceSets;

		public MprVolume(Volumes.Volume volume) : this(volume, CreateDefaultSliceSets(volume)) {}

		public MprVolume(Volumes.Volume volume, IEnumerable<IVolumeSlicerParams> slicerParams) : this(volume, CreateStandardSliceSets(volume, slicerParams)) {}

		public MprVolume(Volumes.Volume volume, IEnumerable<IMprSliceSet> sliceSets)
		{
			Platform.CheckForNullReference(volume, "volume");

			// MprVolume is the de jure owner of the Volume
			// Everything else (like the SOPs) just hold transient references
			_volume = volume;

			_sliceSets = new ObservableDisposableList<IMprSliceSet>();
			if (sliceSets != null)
			{
				foreach (IMprSliceSet sliceSet in sliceSets)
				{
					if (sliceSet is IInternalMprSliceSet)
						((IInternalMprSliceSet) sliceSet).Parent = this;
					_sliceSets.Add(sliceSet);
				}
			}
			_sliceSets.EnableEvents = true;
			_sliceSets.ItemAdded += OnItemAdded;
			_sliceSets.ItemChanged += OnItemAdded;
			_sliceSets.ItemChanging += OnItemRemoved;
			_sliceSets.ItemRemoved += OnItemRemoved;

			// Generate a descriptive name for the volume
			PersonName patientName = new PersonName(_volume.DataSet[DicomTags.PatientsName].ToString());
			string patientId = _volume.DataSet[DicomTags.PatientId].ToString();
			string seriesDescription = _volume.DataSet[DicomTags.SeriesDescription].ToString();
			if (string.IsNullOrEmpty(seriesDescription))
				_description = string.Format(SR.FormatVolumeLabel, patientName.FormattedName, patientId, seriesDescription);
			else
				_description = string.Format(SR.FormatVolumeLabelWithSeries, patientName.FormattedName, patientId, seriesDescription);
		}

		public Volumes.Volume Volume
		{
			get { return _volume; }
		}

		public IList<IMprSliceSet> SliceSets
		{
			get { return _sliceSets; }
		}

		public string Uid
		{
			get { return _uid; }
		}

		public string Description
		{
			get { return _description; }
		}

		private static IEnumerable<IMprSliceSet> CreateDefaultSliceSets(Volumes.Volume volume)
		{
			// The default slice sets consist of a fixed view of the original image plane,
			// and three mutable slice sets showing the other two planes perpendicular to the original
			// plus one oblique slice set halfway in between these two perpendicular planes.
			if (volume != null)
			{
				yield return MprStaticSliceSet.CreateIdentitySliceSet(volume);
				yield return new MprStandardSliceSet(volume, VolumeSlicerParams.OrthogonalX);
				yield return new MprStandardSliceSet(volume, new VolumeSlicerParams(90, 0, 270));
				yield return new MprStandardSliceSet(volume, new VolumeSlicerParams(90, 0, 315));
			}
		}

		private static IEnumerable<IMprSliceSet> CreateStandardSliceSets(Volumes.Volume volume, IEnumerable<IVolumeSlicerParams> slicerParams)
		{
			if (volume != null && slicerParams != null)
			{
				foreach (IVolumeSlicerParams slicerParam in slicerParams)
					yield return new MprStandardSliceSet(volume, slicerParam);
			}
		}

		protected virtual void OnSliceSetRemoved(IMprSliceSet item)
		{
			if (item is IInternalMprSliceSet)
				((IInternalMprSliceSet) item).Parent = null;
		}

		protected virtual void OnSliceSetAdded(IMprSliceSet item)
		{
			if (item is IInternalMprSliceSet)
				((IInternalMprSliceSet)item).Parent = this;
		}

		private void OnItemRemoved(object sender, ListEventArgs<IMprSliceSet> e)
		{
			this.OnSliceSetRemoved(e.Item);
		}

		private void OnItemAdded(object sender, ListEventArgs<IMprSliceSet> e)
		{
			this.OnSliceSetAdded(e.Item);
		}

		#region Disposal

		public void Dispose()
		{
			try
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Warn, e);
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_sliceSets != null)
				{
					_sliceSets.ItemAdded -= OnItemAdded;
					_sliceSets.ItemChanged -= OnItemAdded;
					_sliceSets.ItemChanging -= OnItemRemoved;
					_sliceSets.ItemRemoved -= OnItemRemoved;
					_sliceSets.Dispose();
					_sliceSets = null;
				}

				if (_volume != null)
				{
					_volume.Dispose();
					_volume = null;
				}
			}
		}

		#endregion
	}

	/// <summary>
	/// Same as <see cref="IMprSliceSet"/>, but adds an internal <see cref="Parent"/> setter.
	/// </summary>
	/// <remarks>
	/// This internal interface is only used to let <see cref="MprVolume"/> decide whether or not
	/// to automatically manage the parent relationship of an <see cref="IMprSliceSet"/>. If a
	/// class implements the <see cref="IMprSliceSet"/> interface directly, it is responsible for
	/// managing the parent relationship on its own. Do <b><i>not</i></b> make this interface
	/// public just to <see cref="MprVolume"/> do your work for you.
	/// </remarks>
	internal interface IInternalMprSliceSet : IMprSliceSet
	{
		new IMprVolume Parent { get; set; }
	}
}