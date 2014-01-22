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

#pragma warning disable 1591

namespace ClearCanvas.Common.Specifications
{
	public abstract class Expression
	{
		#region NullExpression

		class NullExpression : Expression
		{
			public NullExpression()
				: base(null) { }

			public override object Evaluate(object arg)
			{
				return null;
			}
		}

		public static readonly Expression Null = new NullExpression();

		#endregion

		private readonly string _text;

		protected Expression(string text)
		{
			// treat "" as null
			_text = string.IsNullOrEmpty(text) ? null : text;
		}

		public string Text
		{
			get { return _text; }
		}

		public abstract object Evaluate(object arg);

		public override bool Equals(object obj)
		{
			var other = obj as Expression;
			return other != null && other._text == this._text;
		}

		public override int GetHashCode()
		{
			return _text == null ? 0 : _text.GetHashCode();
		}
	}
}
