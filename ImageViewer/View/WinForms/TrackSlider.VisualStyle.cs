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
using System.Drawing;
using System.IO;
using System.Reflection;

namespace ClearCanvas.ImageViewer.View.WinForms
{
	public enum TrackSliderVisualElement
	{
		Thumb,
		Track,
		TrackStart,
		TrackEnd
	}

	public interface ITrackSliderVisualStyle : INotifyPropertyChanged
	{
		Image GetVisualElement(TrackSliderVisualElement element, bool vertical);
		ITrackSliderVisualStyleReference CreateReference();
	}

	public interface ITrackSliderVisualStyleReference : IDisposable
	{
		Image GetVisualElement(TrackSliderVisualElement element, bool vertical);
	}

	public abstract class TrackSliderVisualStyle : ITrackSliderVisualStyle
	{
		private event PropertyChangedEventHandler _propertyChanged;
		private readonly IList<TrackSliderVisualStyleReference> _childReferences = new List<TrackSliderVisualStyleReference>();

		protected TrackSliderVisualStyle() {}

		public event PropertyChangedEventHandler PropertyChanged
		{
			add { _propertyChanged += value; }
			remove { _propertyChanged -= value; }
		}

		public abstract Image GetVisualElement(TrackSliderVisualElement element, bool vertical);

		public ITrackSliderVisualStyleReference CreateReference()
		{
			return new TrackSliderVisualStyleReference(this);
		}

		protected void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (_propertyChanged != null)
				_propertyChanged.Invoke(this, e);
		}

		protected void ResetCachedReferences()
		{
			foreach (TrackSliderVisualStyleReference reference in _childReferences)
				reference.ClearCache();
		}

		public override string ToString()
		{
			return this.GetType().Name;
		}

		protected class TrackSliderVisualStyleReference : ITrackSliderVisualStyleReference
		{
			private TrackSliderVisualStyle _parent;
			private Dictionary<int, Image> _elements;

			public TrackSliderVisualStyleReference(TrackSliderVisualStyle parent)
			{
				_parent = parent;
				_parent._childReferences.Add(this);
				_elements = new Dictionary<int, Image>();
			}

			public void Dispose()
			{
				this.ClearCache();
				_elements = null;
				_parent._childReferences.Remove(this);
				_parent = null;
			}

			public Image GetVisualElement(TrackSliderVisualElement element, bool vertical)
			{
				int key = (int) element + (vertical ? 10 : 0);
				if (!_elements.ContainsKey(key))
				{
					_elements.Add(key, _parent.GetVisualElement(element, vertical));
				}
				return _elements[key];
			}

			public void ClearCache()
			{
				foreach (Image value in _elements.Values)
				{
					value.Dispose();
				}
				_elements.Clear();
			}
		}
	}

	public sealed class StandardTrackSliderVisualStyle : TrackSliderVisualStyle
	{
		private Color _hue = Color.Silver;

		[Category("Appearance")]
		[Description("Specifies the color hue with which to colorize the visual style elements.")]
		public Color Hue
		{
			get { return _hue; }
			set
			{
				if (_hue != value)
				{
					_hue = value;
					base.ResetCachedReferences();
					base.OnPropertyChanged(new PropertyChangedEventArgs("Hue"));
				}
			}
		}

		private void ResetHue()
		{
			this.Hue = Color.Silver;
		}

		private bool ShouldSerializeHue()
		{
			return this.Hue != Color.Silver;
		}

		private static Bitmap ReadTrackSliderResource(string key)
		{
			Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Format("ClearCanvas.ImageViewer.View.WinForms.TrackSliderResources.{0}.png", key));
			if (stream == null)
				throw new ArgumentException("Specified resource does not exist!", key);
			return new Bitmap(stream);
		}

		public override Image GetVisualElement(TrackSliderVisualElement element, bool vertical)
		{
			Bitmap image;
			switch (element)
			{
				case TrackSliderVisualElement.Track:
					image = vertical ? ReadTrackSliderResource("TrackVertical") : ReadTrackSliderResource("TrackHorizontal");
					break;
				case TrackSliderVisualElement.TrackStart:
					image = vertical ? ReadTrackSliderResource("TrackEndUp") : ReadTrackSliderResource("TrackEndLeft");
					break;
				case TrackSliderVisualElement.TrackEnd:
					image = vertical ? ReadTrackSliderResource("TrackEndDown") : ReadTrackSliderResource("TrackEndRight");
					break;
				case TrackSliderVisualElement.Thumb:
					image = vertical ? ReadTrackSliderResource("ThumbVertical") : ReadTrackSliderResource("ThumbHorizontal");
					break;
				default:
					throw new ArgumentOutOfRangeException("element");
			}

			for (int y = 0; y < image.Height; y++)
			{
				for (int x = 0; x < image.Width; x++)
				{
					Color c = image.GetPixel(x, y);
					Color d = Color.FromArgb(c.A,
					                         (int) (_hue.R*(1 - c.R/255f)),
					                         (int) (_hue.G*(1 - c.G/255f)),
					                         (int) (_hue.B*(1 - c.B/255f)));
					image.SetPixel(x, y, d);
				}
			}

			return image;
		}

		public override int GetHashCode()
		{
			return _hue.GetHashCode() ^ -0x515897BC;
		}

		public override bool Equals(object obj)
		{
			StandardTrackSliderVisualStyle other = obj as StandardTrackSliderVisualStyle;
			return other != null && this._hue == other._hue;
		}
	}
}