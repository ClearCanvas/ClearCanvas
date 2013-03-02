﻿#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU Lesser Public
// License as published by the Free Software Foundation, either version 3 of
// the License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that
// it will be useful, but WITHOUT ANY WARRANTY; without even the implied
// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser Public License for more details.
//
// You should have received a copy of the GNU Lesser Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Rendering
{
    /// <summary>
    /// Simple flyweight factory for fonts.
    /// </summary>
    internal class FontFactory : IDisposable
    {
        private class ItemKey
        {
            public ItemKey(string name, float size, FontStyle style, GraphicsUnit unit)
            {
                Name = name;
                //So we don't end up with an insane # of items.
                Size = (float)Math.Round(size*10)/10;
                Style = style;
                Unit = unit;
            }

            public readonly string Name;
            public readonly float Size;
            public readonly FontStyle Style;
            public readonly GraphicsUnit Unit;

            public override int GetHashCode()
            {
                var hash = 0x75417862;
                if (Name != null)
                    hash ^= Name.GetHashCode();
                hash ^= Size.GetHashCode();
                hash ^= Style.GetHashCode();
                hash ^= Unit.GetHashCode();
                return hash;
            }

            public override bool Equals(object obj)
            {
                var other = (ItemKey) obj;
                return other.Name == Name
                       && other.Style == Style
                       && other.Unit == Unit
                       && FloatComparer.AreEqual(other.Size, Size);
            }
        }

        private readonly Dictionary<ItemKey, Font> _fonts = new Dictionary<ItemKey,Font>();
        
        public Font CreateFont(string fontName, float fontSize, FontStyle fontStyle, GraphicsUnit graphicsUnit, string defaultFontName)
        {
            var key = new ItemKey(fontName, fontSize, fontStyle, graphicsUnit);
            return CreateFont(key, defaultFontName);
        }

        private Font CreateFont(ItemKey key, string defaultFontName)
        {
            if (key.Unit == GraphicsUnit.Pixel || key.Unit == GraphicsUnit.Point)
            {
                Font font;
                if (!_fonts.TryGetValue(key, out font))
                    _fonts[key] = font = CreateNewFont(key, defaultFontName);

                return font;
            }

            return CreateNewFont(key, defaultFontName);
        }

        private Font CreateNewFont(ItemKey key, string defaultFontName)
        {
            CleanupFonts();

            try
            {
                return new Font(key.Name, key.Size, key.Style, key.Unit);
            }
            catch (Exception ex)
            {
                Platform.Log(LogLevel.Error, ex);
                return new Font(defaultFontName, key.Size, FontStyle.Regular, key.Unit);
            }
        }

        private void CleanupFonts(bool force = false)
        {
            if (!force && _fonts.Count <= 50)
                return;

            foreach (var font in _fonts.Values)
                font.Dispose();
            _fonts.Clear();
        }

        #region IDisposable Members

        public void  Dispose()
        {
            CleanupFonts(true);
        }

        #endregion
    }
}
