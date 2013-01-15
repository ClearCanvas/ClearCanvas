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
using System.Collections;
using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	internal class DicomPresentationImageCollection<T> : IEnumerable<T> where T : IDicomPresentationImage
	{
		private readonly List<T> _images;
		private string _studyUid = null;
		private Dictionary<string, List<T>> _dictionary = null;

		public DicomPresentationImageCollection()
		{
			_images = new List<T>();
		}

		public DicomPresentationImageCollection(IEnumerable<T> images)
		{
			_images = new List<T>(images);

			if (_images.Count > 0)
				_studyUid = _images[0].ImageSop.StudyInstanceUid;
		}

		private Dictionary<string, List<T>> Dictionary
		{
			get
			{
				if (_dictionary == null)
				{
					_dictionary = new Dictionary<string, List<T>>();
					foreach (T image in _images)
					{
						string seriesUid = image.ImageSop.SeriesInstanceUid;
						if (!_dictionary.ContainsKey(seriesUid))
							_dictionary.Add(seriesUid, new List<T>());
						_dictionary[seriesUid].Add(image);
					}
				}
				return _dictionary;
			}
		}

		public void Add(T image)
		{
			if (_dictionary != null)
				throw new InvalidOperationException();
			if (_studyUid != null && _studyUid != image.ImageSop.StudyInstanceUid)
				throw new ArgumentException();
			else if (_studyUid == null)
				_studyUid = image.ImageSop.StudyInstanceUid;

			_images.Add(image);
		}

		public int Count
		{
			get { return _images.Count; }
		}

		public T FirstImage
		{
			get
			{
				if (_images.Count == 0)
					return default(T);
				return _images[0];
			}
		}

		public IEnumerable<string> EnumerateSeries()
		{
			return this.Dictionary.Keys;
		}

		public IEnumerable<T> EnumerateImages()
		{
			return _images;
		}

		public IEnumerable<T> EnumerateImages(string seriesUid)
		{
			if (_dictionary.ContainsKey(seriesUid))
			{
				foreach (T image in _dictionary[seriesUid])
					yield return image;
			}
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return this.EnumerateImages().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.EnumerateImages().GetEnumerator();
		}
	}
}