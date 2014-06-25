#region License

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
using System.Drawing;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Rendering.GDI
{
    public class CreateFontArgs
    {
        public readonly string Name;
        public readonly float Size;
        public readonly FontStyle Style;
        public readonly GraphicsUnit Unit;
        public string DefaultFontName;

        public CreateFontArgs(string name, float size, FontStyle style, GraphicsUnit unit)
        {
            Name = String.IsNullOrWhiteSpace(name) ? FontFactory.GenericSansSerif : name;
            Size = size;
            Style = style;
            Unit = unit;
        }
    }

    public interface IFontFactory
    {
        Font GetFont(string fontName, float fontSize, FontStyle fontStyle = FontStyle.Regular,
            GraphicsUnit graphicsUnit = GraphicsUnit.Point, string defaultFontName = null);

        Font CreateFont(CreateFontArgs args);
    }

    /// <summary>
	/// Simple flyweight factory for GDI+ <see cref="Font"/>s.
	/// </summary>
	public sealed class FontFactory : IDisposable
	{
		private class ItemKey
		{
            private readonly int _hash;
		    private readonly int _sizeEquality;

			public ItemKey(string name, float size, FontStyle style, GraphicsUnit unit)
			{
				Name = name;
				//So we don't end up with an insane # of items.
			    _sizeEquality = (int)Math.Round(size*10);
				Size = _sizeEquality/10F;
				Style = style;
				Unit = unit;
			    _hash = ComputeHash();
			}

			public readonly string Name;
			public readonly float Size;
			public readonly FontStyle Style;
			public readonly GraphicsUnit Unit;

			public override int GetHashCode()
			{
			    return _hash;
			}

            private int ComputeHash()
            {
                var hash = 27;
                if (Name != null)
                    hash = hash * 486187739 + Name.GetHashCode();
                hash = hash * 486187739 + _sizeEquality.GetHashCode();
                hash = hash * 486187739 + Style.GetHashCode();
                hash = hash * 486187739 + Unit.GetHashCode();
                return hash;
            }

			public override bool Equals(object obj)
			{
				var other = (ItemKey) obj;
                //More efficient than constantly using FloatComparer on the size.
				return other._sizeEquality == _sizeEquality
				       && other.Style == Style
				       && other.Unit == Unit
                       && other.Name == Name;
			}
		}

        private readonly MruFactory<ItemKey, Font> _fonts;

		/// <summary>
		/// Gets the font name for a generic serif font (e.g. Times New Roman) available on the system.
		/// </summary>
		public static readonly string GenericSerif = FontFamily.GenericSerif.Name;

		/// <summary>
		/// Gets the font name for a generic sans serif font (e.g. MS Sans Serif) available on the system.
		/// </summary>
		public static readonly string GenericSansSerif = FontFamily.GenericSansSerif.Name;

		/// <summary>
		/// Gets the font name for a generic monospace font (e.g. Courier New) available on the system.
		/// </summary>
		public static readonly string GenericMonospace = FontFamily.GenericMonospace.Name;

        public FontFactory(int maxObjects = 20)
        {
            _fonts = new MruFactory<ItemKey, Font>(maxObjects, key => new Font(key.Name, key.Size, key.Style, key.Unit));
        }

		/// <summary>
		/// Gets a <see cref="Font"/> object for the specified typeface, size, style and unit.
		/// </summary>
		/// <param name="fontName">The name of the typeface.</param>
		/// <param name="fontSize">The size of the font.</param>
		/// <param name="fontStyle">The style of the font.</param>
		/// <param name="graphicsUnit">The units in which <paramref name="fontSize"/> is expressed.</param>
		/// <param name="defaultFontName">The name of a default typeface, in case the font referred to by <paramref name="fontName"/> does not exist.</param>
		/// <returns></returns>
		public Font GetFont(string fontName, float fontSize, FontStyle fontStyle = FontStyle.Regular, GraphicsUnit graphicsUnit = GraphicsUnit.Point, string defaultFontName = null)
		{
		    var args = new CreateFontArgs(fontName, fontSize, fontStyle, graphicsUnit);
            return CreateFont(args);
		}

        /// <summary>
        /// Gets/Creates a <see cref="Font"/> given the specified arguments.
        /// </summary>
        public Font CreateFont(CreateFontArgs args)
        {
            var key = new ItemKey(args.Name, args.Size, args.Style, args.Unit);
            var defaultFontName = !string.IsNullOrWhiteSpace(args.DefaultFontName) ? args.DefaultFontName : GenericSansSerif;
            return GetFont(key, defaultFontName);
        }

		private Font GetFont(ItemKey key, string defaultFontName)
		{
		    if (key.Unit != GraphicsUnit.Pixel && key.Unit != GraphicsUnit.Point)
		        throw new NotSupportedException("Only Pixel and Point GraphicsUnits are supported.");
		    
            try
		    {
		        return _fonts.Create(key);
		    }
		    catch (Exception e)
		    {
		        Platform.Log(LogLevel.Error, e);
		        return _fonts.Create(new ItemKey(defaultFontName, key.Size, FontStyle.Regular, key.Unit));
		    }
		}

		#region IDisposable Members

		public void Dispose()
		{
			_fonts.Dispose();
		}

		#endregion
	}
}