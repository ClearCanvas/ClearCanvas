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
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Annotations
{
	/// <summary>
	/// An <see cref="AnnotationBox"/> is rendered to the screen by an <see cref="ClearCanvas.ImageViewer.Rendering.IRenderer"/>.
	/// </summary>
	/// <seealso cref="AnnotationItemConfigurationOptions"/>
	/// <seealso cref="IAnnotationItem"/>
	[Cloneable(true)]
	public sealed class AnnotationBox
	{
		/// <summary>
		/// Defines the available truncation behaviours for strings that will extend beyond <see cref="AnnotationBox.NormalizedRectangle"/>.
		/// </summary>
		public enum TruncationBehaviour
		{
			/// <summary>
			/// Specifies that the string should just be cut off at the edge of the <see cref="AnnotationBox.NormalizedRectangle"/>.
			/// </summary>
			Truncate, 
			
			/// <summary>
			/// Specifies that the string should have an ellipses (...) at the edge of the <see cref="AnnotationBox.NormalizedRectangle"/>.
			/// </summary>
			Ellipsis
		};

		/// <summary>
		/// Defines the available horizontal justifications.
		/// </summary>
		public enum JustificationBehaviour
		{
			/// <summary>
			/// Specifies that the string should be left-justified in the <see cref="AnnotationBox.NormalizedRectangle"/>.
			/// </summary>
			Left, 

			/// <summary>
			/// Specifies that the string should be centred horizontally in the <see cref="AnnotationBox.NormalizedRectangle"/>.
			/// </summary>
			Center, 
			
			/// <summary>
			/// Specifies that the string should be right-justified in the <see cref="AnnotationBox.NormalizedRectangle"/>.
			/// </summary>
			Right
		};

		/// <summary>
		/// Defines the available vertical alignments.
		/// </summary>
		public enum VerticalAlignmentBehaviour
		{
			/// <summary>
			/// Specifies that the string should be aligned along the top of the <see cref="AnnotationBox.NormalizedRectangle"/>.
			/// </summary>
			Top,

			/// <summary>
			/// Specifies that the string should be centered in the <see cref="AnnotationBox.NormalizedRectangle"/>.
			/// </summary>
			Center,

			/// <summary>
			/// Specifies that the string should be aligned along the bottom of the <see cref="AnnotationBox.NormalizedRectangle"/>.
			/// </summary>
			Bottom
		};

		#region Private Fields

		[CloneCopyReference]
		private IAnnotationItem _annotationItem;
		private AnnotationItemConfigurationOptions _annotationItemConfigurationOptions;

		private RectangleF _normalizedRectangle;
		
		private byte _numberOfLines = 1;

		private static readonly string _defaultFont = "Arial";
		private static readonly string _defaultColor = "WhiteSmoke";

		private string _font = _defaultFont;
		private string _color = _defaultColor; 
		
		private bool _bold = false;
		private bool _italics = false;
		private bool _fitWidth = false;
		private bool _alwaysVisible = false;

		private TruncationBehaviour _truncation = TruncationBehaviour.Ellipsis;
		private JustificationBehaviour _justification = JustificationBehaviour.Left;
		private VerticalAlignmentBehaviour _verticalAlignment = VerticalAlignmentBehaviour.Center;

		private bool _visible = true;

		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public AnnotationBox()
		{
		}

		/// <summary>
		/// Constructor that initializes the <see cref="NormalizedRectangle"/> and <see cref="AnnotationItem"/> properties.
		/// </summary>
		/// <exception cref="ArgumentException">Thrown when the input <paramref name="normalizedRectangle"/> is not normalized.</exception>
		public AnnotationBox(RectangleF normalizedRectangle, IAnnotationItem annotationItem)
		{
			this.NormalizedRectangle = normalizedRectangle;
			_annotationItem = annotationItem;
		}

		/// <summary>
		/// Gets the text to be rendered into the area defined by <see cref="NormalizedRectangle"/> for the input <paramref name="presentationImage"/>.
		/// </summary>
		/// <param name="presentationImage">The presentation image.</param>
		public string GetAnnotationText(IPresentationImage presentationImage)
		{
			if (_annotationItem == null)
				return string.Empty;

			string annotationText = _annotationItem.GetAnnotationText(presentationImage);
			string annotationLabel = _annotationItem.GetLabel();

			if (string.IsNullOrEmpty(annotationText))
			{
				if (this.ConfigurationOptions.ShowLabelIfValueEmpty)
					annotationText = string.Format(SR.FormatAnnotationItem, annotationLabel, SR.ValueNil);
			}
			else if (this.ConfigurationOptions.ShowLabel)
			{
				annotationText = string.Format(SR.FormatAnnotationItem, annotationLabel, annotationText);
			}

			return annotationText;
		}

		/// <summary>
		/// Gets the associated <see cref="IAnnotationItem"/> that provides the annotation text.
		/// </summary>
		/// <remarks>
		/// It is permissible for this value to be null.  A value of "" will always be returned from <see cref="GetAnnotationText"/>.
		/// </remarks>
		public IAnnotationItem AnnotationItem
		{
			get { return _annotationItem; }
			set { _annotationItem = value; }
		}

		/// <summary>
		/// Defines the normalized rectangle in which the <see cref="ClearCanvas.ImageViewer.Rendering.IRenderer"/> should render the text.
		/// </summary>
		/// <exception cref="ArgumentException">Thrown when setting the property if the value is not normalized.</exception>
		public RectangleF NormalizedRectangle
		{
			get { return _normalizedRectangle; }
			set { _normalizedRectangle = value; }
		}

		/// <summary>
		/// Defines configuration options for how <see cref="GetAnnotationText"/> should format its return value.
		/// </summary>
		public AnnotationItemConfigurationOptions ConfigurationOptions
		{
			get
			{
				if (_annotationItemConfigurationOptions == null)
					_annotationItemConfigurationOptions = new AnnotationItemConfigurationOptions();

				return _annotationItemConfigurationOptions;
			}
			set { _annotationItemConfigurationOptions = value; }
		}

		/// <summary>
		/// Gets the default font ("Arial") by name.
		/// </summary>
		public static string DefaultFont
		{
			get { return _defaultFont; }
		}

		/// <summary>
		/// Gets the default color ("WhiteSmoke") by name.
		/// </summary>
		public static string DefaultColor
		{
			get { return _defaultColor; }
		}

		/// <summary>
		/// Gets or sets the font (by name) that should be used to render the text.
		/// </summary>
		/// <remarks>
		/// The default value is "Arial".
		/// </remarks>
		public string Font
		{
			get { return _font; }
			set
			{
				Platform.CheckForEmptyString(value, "value");
				_font = value;
			}
		}

		/// <summary>
		/// Gets or sets the color (by name) that should be used to render the text.
		/// </summary>
		/// <remarks>
		/// The default value is "WhiteSmoke".
		/// </remarks>
		public string Color
		{
			get { return _color; }
			set
			{
				Platform.CheckForEmptyString(value, "value");
				_color = value;
			}
		}

		/// <summary>
		/// Gets or sets whether the text should be in italics.
		/// </summary>
		/// <remarks>
		/// The default value is false.
		/// </remarks>
		public bool Italics
		{
			get { return _italics; }
			set { _italics = value; }
		}

		/// <summary>
		/// Gets or sets whether the text should be in bold.
		/// </summary>
		/// <remarks>
		/// The default value is false.
		/// </remarks>
		public bool Bold
		{
			get { return _bold; }
			set { _bold = value; }
		}

		/// <summary>
		/// Gets or sets the number of lines of to render.
		/// </summary>
		/// <remarks>
		/// The default value is 1.
		/// </remarks>
		public byte NumberOfLines
		{
			get { return _numberOfLines; }
			set
			{
				//you cannot have multiple lines in the 'fit width' scenario.
				if (value > 1 && _fitWidth)
					return;
					
				_numberOfLines = Math.Max((byte)1, value);
			}
		}

		/// <summary>
		/// Gets or sets whether the text should be fit to the width of the <see cref="NormalizedRectangle"/>.
		/// </summary>
		/// <remarks>
		/// The default value is false.
		/// </remarks>
		public bool FitWidth
		{
			get { return _fitWidth; }
			set
			{
				_fitWidth = value;
				
				//you cannot have multiple lines in the 'fit width' scenario.
				if (_numberOfLines > 1)
					_numberOfLines = 1;
			}
		}

		/// <summary>
		/// Gets or sets whether or not the item can be made invisible.
		/// </summary>
		public bool AlwaysVisible
		{
			get { return _alwaysVisible; }
			set { _alwaysVisible = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="TruncationBehaviour"/>.
		/// </summary>
		/// <remarks>
		/// The default value is <see cref="TruncationBehaviour.Ellipsis"/>.
		/// </remarks>
		public TruncationBehaviour Truncation
		{
			get { return _truncation; }
			set { _truncation = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="JustificationBehaviour"/>.
		/// </summary>
		/// <remarks>
		/// The default value is <see cref="JustificationBehaviour.Left"/>.
		/// </remarks>
		public JustificationBehaviour Justification
		{
			get { return _justification; }
			set { _justification = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="VerticalAlignmentBehaviour"/>.
		/// </summary>
		/// <remarks>
		/// The default value is <see cref="VerticalAlignmentBehaviour.Center"/>.
		/// </remarks>
		public VerticalAlignmentBehaviour VerticalAlignment
		{
			get { return _verticalAlignment; }
			set { _verticalAlignment = value; }
		}

		/// <summary>
		/// Gets or sets whether or not the item is visible.
		/// </summary>
		/// <remarks>Takes into account the value of <see cref="AlwaysVisible"/> when returning a value;
		/// however, internally, the value is always set.</remarks>
		public bool Visible
		{
			get { return _alwaysVisible || _visible; }
			set{ _visible = value; }
		}

		/// <summary>
		/// Gets the internal value of <see cref="Visible"/>.
		/// </summary>
		/// <returns>Returns the true value of <see cref="Visible"/> regardless of the value of <see cref="AlwaysVisible"/>.</returns>
		public bool VisibleInternal
		{
			get { return _visible; }	
		}

		/// <summary>
		/// Creates a deep clone of this object.
		/// </summary>
		public AnnotationBox Clone()
		{
			var clone = new AnnotationBox();
			clone._alwaysVisible = this._alwaysVisible; // bool
			clone._annotationItem = this._annotationItem; // clone copy reference
			if (this._annotationItemConfigurationOptions != null)
				clone._annotationItemConfigurationOptions = this._annotationItemConfigurationOptions.Clone();
			clone._bold = this._bold; // bool
			clone._color = this._color; // string
			clone._fitWidth = this._fitWidth; // bool
			clone._font = this._font; // string
			clone._italics = this._italics; // bool
			clone._justification = this._justification; // enum
			clone._normalizedRectangle = this._normalizedRectangle; // rect
			clone._numberOfLines = this._numberOfLines; // byte
			clone._truncation = this._truncation; //  enum
			clone._verticalAlignment = this._verticalAlignment; // enum
			clone._visible = this._visible; // bool
			return clone;
		}
	}
}
