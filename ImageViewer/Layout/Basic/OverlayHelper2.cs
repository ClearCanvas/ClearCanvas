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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	public interface IImageOverlays : IEnumerable<IImageOverlay>
	{
		int Count { get; }
		IImageOverlay this[string name] { get; }

		void ShowSelected(bool draw);
		void HideUnimportant(bool draw);
		void HideAll(bool draw);
	}

	public interface IImageOverlay : IOverlaySelection
	{
		string DisplayName { get; }
		bool IsImportant { get; }
		new bool IsSelected { get; set; }
		IconSet IconSet { get; }
		IResourceResolver ResourceResolver { get; }

		void ShowIfSelected();
		void Show();
		void Hide();
	}

	public static partial class OverlayHelper
	{
		private class ImageOverlay : IImageOverlay
		{
			private readonly IPresentationImage _image;
			private readonly IOverlayManager _manager;
			private readonly OverlayState _state;

			internal ImageOverlay(IPresentationImage image, OverlayState state)
			{
				_image = image;
				_manager = OverlayManagers.First(m => m.Name == state.Name);
				_state = state;
			}

			#region Implementation of IOverlaySelection

			public string Name
			{
				get { return _manager.Name; }
			}

			public bool IsSelected
			{
				get { return _state.IsSelected; }
				set { _state.IsSelected = value; }
			}

			#endregion

			#region Implementation of IOverlay

			public bool IsImportant
			{
				get { return _manager.IsImportant; }
			}

			public string DisplayName
			{
				get { return _manager.DisplayName; }
			}

			public IResourceResolver ResourceResolver
			{
				get { return _manager.ResourceResolver; }
			}

			public IconSet IconSet
			{
				get { return _manager.IconSet; }
			}

			public void ShowIfSelected()
			{
				if (_image == null) return;

				if (IsSelected)
					_manager.ShowOverlay(_image);
				else
					_manager.HideOverlay(_image);
			}

			public void Show()
			{
				if (_image != null)
					_manager.ShowOverlay(_image);
			}

			public void Hide()
			{
				if (_image != null)
					_manager.HideOverlay(_image);
			}

			#endregion
		}

		public class ImageOverlays : IImageOverlays
		{
			private readonly IPresentationImage _image;
			private readonly IList<IImageOverlay> _overlays;

			internal ImageOverlays(IPresentationImage image)
			{
				_image = image;
				_overlays = GetOverlaySelectionStates(image).Select(state => (IImageOverlay) new ImageOverlay(image, state)).ToList();
			}

			#region Implementation of IImageOverlays

			public int Count
			{
				get { return _overlays.Count; }
			}

			public IImageOverlay this[string name]
			{
				get { return _overlays.FirstOrDefault(o => o.Name == name); }
			}

			public void ShowSelected(bool draw)
			{
				foreach (var overlay in _overlays)
					overlay.ShowIfSelected();

				if (draw)
					_image.Draw();
			}

			public void HideUnimportant(bool draw)
			{
				foreach (var overlay in _overlays)
				{
					if (!overlay.IsImportant)
						overlay.Hide();
					else
						overlay.Show();
				}

				if (draw)
					_image.Draw();
			}

			public void HideAll(bool draw)
			{
				foreach (var overlay in _overlays)
					overlay.Hide();

				if (draw)
					_image.Draw();
			}

			#endregion

			#region Implementation of IEnumerable

			public IEnumerator<IImageOverlay> GetEnumerator()
			{
				return _overlays.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			#endregion
		}
	}
}