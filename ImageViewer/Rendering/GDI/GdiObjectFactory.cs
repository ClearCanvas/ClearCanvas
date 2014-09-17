using System;
using System.Drawing;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.Rendering.GDI
{
    /// <summary>
    /// Args for creating a GDI+ <see cref="StringFormat"/>.
    /// </summary>
    public class CreateStringFormatArgs : IEquatable<CreateStringFormatArgs>
    {
        private readonly int _hash;

        public CreateStringFormatArgs(StringTrimming trimming, StringAlignment alignment, StringAlignment lineAlignment, StringFormatFlags flags)
        {
            Trimming = trimming;
            Alignment = alignment;
            LineAlignment = lineAlignment;
            Flags = flags;
            
            _hash = ComputeHash();
        }

        public CreateStringFormatArgs(AnnotationBox annotationBox)
        {
            if (annotationBox.Truncation == AnnotationBox.TruncationBehaviour.Truncate)
                Trimming = StringTrimming.Character;
            else
                Trimming = StringTrimming.EllipsisCharacter;

            if (annotationBox.FitWidth)
                Trimming = StringTrimming.None;

            if (annotationBox.Justification == AnnotationBox.JustificationBehaviour.Right)
                Alignment = StringAlignment.Far;
            else if (annotationBox.Justification == AnnotationBox.JustificationBehaviour.Center)
                Alignment = StringAlignment.Center;
            else
                Alignment = StringAlignment.Near;

            if (annotationBox.VerticalAlignment == AnnotationBox.VerticalAlignmentBehaviour.Top)
                LineAlignment = StringAlignment.Near;
            else if (annotationBox.VerticalAlignment == AnnotationBox.VerticalAlignmentBehaviour.Center)
                LineAlignment = StringAlignment.Center;
            else
                LineAlignment = StringAlignment.Far;

            //allow p's and q's, etc to extend slightly beyond the bounding rectangle.  Only completely visible lines are shown.
            Flags = StringFormatFlags.NoClip;

            if (annotationBox.NumberOfLines == 1)
                Flags |= StringFormatFlags.NoWrap;

            _hash = ComputeHash();
        }

        public readonly StringTrimming Trimming;
        public readonly StringAlignment Alignment;
        public readonly StringAlignment LineAlignment;
        public readonly StringFormatFlags Flags;
        
        private int ComputeHash()
        {
            unchecked
            {
                int hash = 27;
                hash = hash * 486187739 + Trimming.GetHashCode();
                hash = hash * 486187739 + Alignment.GetHashCode();
                hash = hash * 486187739 + LineAlignment.GetHashCode();
                hash = hash * 486187739 + Flags.GetHashCode();
                return hash;
            }
        }

        public override int GetHashCode()
        {
            return _hash;
        }

        public override bool Equals(object obj)
        {
            var other = obj as CreateStringFormatArgs;
            return (other != null && Equals(other));
        }

        public bool Equals(CreateStringFormatArgs other)
        {
            return Trimming == other.Trimming && Alignment == other.Alignment 
                && LineAlignment == other.LineAlignment && Flags == other.Flags;
        }
    }

    /// <summary>
    /// Args for creating a GDI+ <see cref="Brush"/>.
    /// </summary>
    public class CreateBrushArgs: IEquatable<CreateBrushArgs>
    {
        private static Color _lastNamedColor = System.Drawing.Color.Black;

        private readonly int _hash;

        public CreateBrushArgs(string colorName)
        {
            ColorName = colorName;
            Color = null;
            _hash = ComputeHash();
        }
        public CreateBrushArgs(Color color)
        {
            ColorName = null;
            Color = color;
            _hash = ComputeHash();
        }

        public readonly string ColorName;
        public readonly Color? Color;
        //BrushType = Solid ... always solid for now.

        private int ComputeHash()
        {
            unchecked
            {
                int hash = 31;
                hash = hash * 486187739 + (ColorName ?? String.Empty).GetHashCode();
                hash = hash * 486187739 + Color.GetHashCode();
                return hash;
            }
        }

        public override int GetHashCode()
        {
            return _hash;
        }

        public override bool Equals(object obj)
        {
            var other = obj as CreateBrushArgs;
            return (other != null && Equals(other));
        }

        public bool Equals(CreateBrushArgs other)
        {
            return Color == other.Color && (ColorName ?? String.Empty) == (other.ColorName ?? String.Empty);
        }

        internal Color GetColor()
        {
            if (Color.HasValue)
                return Color.Value;

            //NOTE: have to be careful with this variable because it's not ThreadStatic (not worth the overhead).
            var lastColor = _lastNamedColor;
            if (ColorName == lastColor.Name)
                return lastColor;

            if (ColorName == System.Drawing.Color.Black.Name)
                lastColor = System.Drawing.Color.Black;
            else if (ColorName == System.Drawing.Color.White.Name)
                lastColor = System.Drawing.Color.White;
            else
                lastColor = System.Drawing.Color.FromName(ColorName);

            _lastNamedColor = lastColor;
            return lastColor;
        }
    }

    public interface IGdiObjectFactory : IDisposable
    {
        Font CreateFont(CreateFontArgs args);
        StringFormat CreateStringFormat(CreateStringFormatArgs args);
        Brush CreateBrush(CreateBrushArgs args);
    }

    /// <summary>
    /// Simple flyweight factory for GDI+ objects.
    /// </summary>
    public class GdiObjectFactory : IGdiObjectFactory
    {
        private const int _maxFonts = 20;
        private const int _maxStringFormats = 20;
        private const int _maxBrushes = 10;

        private FontFactory _fontFactory;
        private MruFactory<CreateStringFormatArgs, StringFormat> _stringFormatFactory;
        private MruFactory<CreateBrushArgs, Brush> _brushFactory;

        public GdiObjectFactory()
        {
            _fontFactory = new FontFactory(_maxFonts);

            _stringFormatFactory = new MruFactory<CreateStringFormatArgs, StringFormat>(_maxStringFormats, 
                args => new StringFormat
            {
                Trimming = args.Trimming,
                Alignment = args.Alignment,
                LineAlignment = args.LineAlignment,
                FormatFlags = args.Flags
            });

            _brushFactory = new MruFactory<CreateBrushArgs, Brush>(_maxBrushes, args => new SolidBrush(args.GetColor()));
        }

        public Brush CreateBrush(CreateBrushArgs args)
        {
            return _brushFactory.Create(args);
        }

        public Font CreateFont(CreateFontArgs args)
        {
            return _fontFactory.CreateFont(args);
        }

        public StringFormat CreateStringFormat(CreateStringFormatArgs args)
        {
            return _stringFormatFactory.Create(args);
        }

        public void Dispose()
        {
            if (_fontFactory != null)
            {
                _fontFactory.Dispose();
                _fontFactory = null;
            }
            if (_stringFormatFactory != null)
            {
                _stringFormatFactory.Dispose();
                _stringFormatFactory = null;
            }
            if (_brushFactory != null)
            {
                _brushFactory.Dispose();
                _brushFactory = null;
            }
        }
    }
}
