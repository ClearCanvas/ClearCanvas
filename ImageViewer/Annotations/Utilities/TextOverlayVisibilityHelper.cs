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

#if UNIT_TESTS

using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Annotations.Utilities.Tests
{
	[TestFixture]
	public class TextOverlayVisibilityTests
	{
		private static AnnotationLayout CreateLayout(bool isVisible, params bool[] annotationBoxVisibility)
		{
			var layout = new AnnotationLayout {Visible = isVisible};
			if (annotationBoxVisibility != null)
				foreach (var visibility in annotationBoxVisibility)
					layout.AnnotationBoxes.Add(new AnnotationBox{ Visible = visibility });
		
			return layout;
		}

		[Test]
		public void TestRestoreVisible()
		{
			var layout = CreateLayout(true);
			var helper = new TextOverlayVisibilityHelper(layout);
			Assert.IsTrue(layout.Visible);
			helper.Hide();
			Assert.IsFalse(layout.Visible);
			helper.Restore();
			Assert.IsTrue(layout.Visible);
		}

		[Test]
		public void TestRestoreHidden()
		{
			var layout = CreateLayout(false);
			var helper = new TextOverlayVisibilityHelper(layout);
			Assert.IsFalse(layout.Visible);
			helper.Show();
			Assert.IsTrue(layout.Visible);
			helper.Restore();
			Assert.IsFalse(layout.Visible);
		}

		[Test]
		public void TestIsVisible_EntireLayoutHidden()
		{
			var layout = CreateLayout(false, true);
			Assert.IsFalse(TextOverlayVisibilityHelper.IsVisible(layout, true));
		}

		[Test]
		public void TestIsVisible_Visible_NoAnnotationBoxes()
		{
			var layout = CreateLayout(true);
			Assert.IsFalse(TextOverlayVisibilityHelper.IsVisible(layout, true));
		}

		[Test]
		public void TestIsVisible_Hidden_NoAnnotationBoxes()
		{
			var layout = CreateLayout(false);
			Assert.IsFalse(TextOverlayVisibilityHelper.IsVisible(layout, true));
		}

		[Test]
		public void TestIsVisible_AllVisible()
		{
			var layout = CreateLayout(true, true, true, true);
			Assert.IsTrue(TextOverlayVisibilityHelper.IsVisible(layout, true));
		}

		[Test]
		public void TestIsVisible_AllHidden()
		{
			var layout = CreateLayout(true, false, false, false);
			Assert.IsFalse(TextOverlayVisibilityHelper.IsVisible(layout, true));
		}

		[Test]
		public void TestIsVisible_OneVisible()
		{
			var layout = CreateLayout(true, false, false, true);
			Assert.IsTrue(TextOverlayVisibilityHelper.IsVisible(layout, true));
		}

		[Test]
		public void TestIsVisible_OneAlwaysVisibleOthersHidden()
		{
			var layout = CreateLayout(true, false, false, false);
			layout.AnnotationBoxes[1].AlwaysVisible = true;
			Assert.IsFalse(TextOverlayVisibilityHelper.IsVisible(layout, true));
		}

		[Test]
		public void TestIsVisible_OneAlwaysVisibleMixedVisibility()
		{
			var layout = CreateLayout(true, false, false, true);
			layout.AnnotationBoxes[1].AlwaysVisible = true;
			Assert.IsTrue(TextOverlayVisibilityHelper.IsVisible(layout, true));
		}

		[Test]
		public void TestIsVisible_OneAlwaysVisibleOthersVisible()
		{
			var layout = CreateLayout(true, true, false, true);
			layout.AnnotationBoxes[1].AlwaysVisible = true;
			Assert.IsTrue(TextOverlayVisibilityHelper.IsVisible(layout, true));
		}

		[Test]
		public void TestIsVisible_AllAlwaysVisible()
		{
			var layout = CreateLayout(true, false, false, false);
			layout.AnnotationBoxes[0].AlwaysVisible = true;
			layout.AnnotationBoxes[1].AlwaysVisible = true;
			layout.AnnotationBoxes[2].AlwaysVisible = true;
			Assert.IsTrue(TextOverlayVisibilityHelper.IsVisible(layout, true));
		}
	}
}
#endif

namespace ClearCanvas.ImageViewer.Annotations.Utilities
{
	public class TextOverlayVisibilityHelper
	{
		private readonly IAnnotationLayout _layout;
		private readonly bool _visible;

		public TextOverlayVisibilityHelper(IPresentationImage image)
			: this(image as IAnnotationLayoutProvider)
		{
		}

		public TextOverlayVisibilityHelper(IAnnotationLayoutProvider annotationLayoutProvider)
			: this(annotationLayoutProvider == null ? null : annotationLayoutProvider.AnnotationLayout)
		{
		}

		public TextOverlayVisibilityHelper(IAnnotationLayout annotationLayout)
		{
			_layout = annotationLayout;
			if (_layout != null)
				_visible = _layout.Visible;
		}

		public void Show()
		{
			SetVisible(true);
		}

		public void Hide()
		{
			SetVisible(false);
		}

		public void Restore()
		{
			SetVisible(_visible);
		}

		private void SetVisible(bool visible)
		{
			if (_layout != null)
				_layout.Visible = visible;
		}

		public static bool IsVisible(IPresentationImage image)
		{
			return IsVisible(image, false);
		}

		public static bool IsVisible(IPresentationImage image, bool checkAnnotationBoxVisibility)
		{
			var provider = image as IAnnotationLayoutProvider;
			return provider != null && IsVisible(provider.AnnotationLayout, checkAnnotationBoxVisibility);
		}

		public static bool IsVisible(IAnnotationLayout layout, bool checkAnnotationBoxVisibility)
		{
			var visible = layout.Visible;
			if (!visible || !checkAnnotationBoxVisibility)
				return visible;

			bool atLeastOne = false;
			bool allForcedVisible = true;
			foreach (var annotationBox in layout.AnnotationBoxes)
			{
				atLeastOne = true;
				if (annotationBox.VisibleInternal)
					return true;
				if (!annotationBox.AlwaysVisible)
					allForcedVisible = false;
			}

			return atLeastOne && allForcedVisible;
		}
	}
}
