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
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Annotations.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Annotations
{
	public static class AnnotationLayoutFactory
	{
		[Cloneable(false)]
		private class StoredAnnotationLayoutProxy : IAnnotationLayout
		{
			private readonly string _layoutId;
			//This works with cloning because, when this is null, it means it hasn't been initialized/touched yet,
			//so we get it from the store, just as the source one would do.  If it's not null, it gets cloned.
			private IAnnotationLayout _realLayout;
			private bool _visible = true;

			public StoredAnnotationLayoutProxy(string layoutId)
			{
				_layoutId = layoutId;
			}

			private StoredAnnotationLayoutProxy(StoredAnnotationLayoutProxy source, ICloningContext context)
			{
				_layoutId = source._layoutId;
				_visible = source.Visible;
				_realLayout = source._realLayout == null ? null : source._realLayout.Clone();
			}

			private IAnnotationLayout RealLayout
			{
				get { return _realLayout ?? (_realLayout = CreateRealLayout(_layoutId)); }
			}

			#region IAnnotationLayout Members

			public IEnumerable<AnnotationBox> AnnotationBoxes
			{
				get { return RealLayout.AnnotationBoxes; }
			}

			public bool Visible
			{
				get { return _visible; }
				set { _visible = value; }
			}

			public IAnnotationLayout Clone()
			{
				return new StoredAnnotationLayoutProxy(this, null);
			}

			#endregion
		}

		private static readonly List<IAnnotationItemProvider> _providers;

		private static readonly object _syncLock = new object();
		private static readonly Dictionary<string, StoredAnnotationLayout> _layoutCache;

		static AnnotationLayoutFactory()
		{
			_layoutCache = new Dictionary<string, StoredAnnotationLayout>();
			AnnotationLayoutStore.Instance.StoreChanged += delegate { ClearCache(); };

			_providers = new List<IAnnotationItemProvider>();

			try
			{
				foreach (object extension in new AnnotationItemProviderExtensionPoint().CreateExtensions())
					_providers.Add((IAnnotationItemProvider) extension);
			}
			catch (NotSupportedException e)
			{
				Platform.Log(LogLevel.Info, e);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}

		private static void ClearCache()
		{
			lock (_syncLock)
			{
				_layoutCache.Clear();
			}
		}

		private static StoredAnnotationLayout GetStoredLayout(string layoutId)
		{
			lock (_syncLock)
			{
				if (_layoutCache.ContainsKey(layoutId))
					return _layoutCache[layoutId];

				StoredAnnotationLayout layout = AnnotationLayoutStore.Instance.GetLayout(layoutId, AvailableAnnotationItems);
				if (layout != null)
					_layoutCache[layoutId] = layout;

				return layout;
			}
		}

		private static IEnumerable<IAnnotationItem> AvailableAnnotationItems
		{
			get
			{
				foreach (IAnnotationItemProvider provider in _providers)
				{
					foreach (IAnnotationItem item in provider.GetAnnotationItems())
						yield return item;
				}
			}
		}

		internal static IAnnotationItem GetAnnotationItem(string annotationItemIdentifier)
		{
			foreach (IAnnotationItemProvider provider in _providers)
			{
				foreach (IAnnotationItem item in provider.GetAnnotationItems())
				{
					if (item.GetIdentifier() == annotationItemIdentifier)
						return item;
				}
			}

			return null;
		}

		private static IAnnotationLayout CreateRealLayout(string storedLayoutId)
		{
			try
			{
				StoredAnnotationLayout storedLayout = GetStoredLayout(storedLayoutId);
				if (storedLayout != null)
					return storedLayout.Clone();

				//just return an empty layout.
				return new AnnotationLayout();
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);

				var layout = new AnnotationLayout();
				var item = new BasicTextAnnotationItem("errorbox", "errorbox", @"LabelError",
				                                       SR.MessageErrorLoadingAnnotationLayout);
				var box = new AnnotationBox(new RectangleF(0.5F, 0.90F, 0.5F, 0.10F), item)
				          	{
				          		Bold = true,
				          		Color = "Red",
				          		Justification = AnnotationBox.JustificationBehaviour.Right,
				          		NumberOfLines = 5,
				          		VerticalAlignment = AnnotationBox.VerticalAlignmentBehaviour.Bottom
				          	};

				layout.AnnotationBoxes.Add(box);
				return layout;
			}
		}

		public static IAnnotationLayout CreateLayout(string storedLayoutId)
		{
			return new StoredAnnotationLayoutProxy(storedLayoutId);
		}

		public static IAnnotationLayout CreateLayoutByModality(string modality)
		{
			return DicomAnnotationLayoutFactory.CreateLayout(new List<KeyValuePair<string, string>> {new KeyValuePair<string, string>(@"Modality", modality)});
		}

		public static IAnnotationLayout CreateLayoutByImageSop(IDicomAttributeProvider dicomAttributeProvider)
		{
			return DicomAnnotationLayoutFactory.CreateLayout(dicomAttributeProvider);
		}

		public static IAnnotationLayout CreateLayoutByImageSop(IImageSopProvider imageSopProvider)
		{
			return DicomAnnotationLayoutFactory.CreateLayout(imageSopProvider);
		}

		#region Unit Test Support

#if UNIT_TESTS

		/// <summary>
		/// Forces the <see cref="AnnotationLayoutFactory"/> to be reinitialized.
		/// </summary>
		/// <remarks>
		/// This may be necessary because the list of <see cref="IAnnotationLayoutProvider"/>s as well as individual layouts are cached.
		/// Unit tests relying on <see cref="IAnnotationItem"/>s may need to reset the caches to a pristine state, particularly if other
		/// unit tests have been using different extension factories.
		/// </remarks>
		public static void Reinitialize()
		{
			ClearCache();
			try
			{
				_providers.Clear();
				foreach (object extension in new AnnotationItemProviderExtensionPoint().CreateExtensions())
					_providers.Add((IAnnotationItemProvider) extension);
			}
			catch (NotSupportedException e)
			{
				Platform.Log(LogLevel.Info, e);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}

#endif

		#endregion
	}
}