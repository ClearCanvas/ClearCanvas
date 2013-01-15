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

using System.Text;

namespace ClearCanvas.Ris.Client.Formatting
{
	public static class StringMask
	{
		/// <summary>
		/// Fills the mask with the supplied text.
		/// </summary>
		/// <remarks>
		/// If the supplied text is longer than the mask, the mask is applied to the right-most characters of the text.
		/// </remarks>
		/// <param name="text"></param>
		/// <param name="mask"></param>
		/// <returns></returns>
		public static string Apply(string text, string mask)
		{
			if (string.IsNullOrEmpty(text)) return string.Empty;

			StringBuilder maskedText = new StringBuilder();

			// Fill the mask from right to left
			string reverseMask = Reverse(mask);
			string reverseText = Reverse(text);
			int reverseTextIndex = 0;

			foreach (char c in reverseMask)
			{
				if (reverseTextIndex >= reverseText.Length)
				{
					break;
				}

				// Are there additional mask characters that need to be specified?
				if (c == '0')
				{
					maskedText.Append(reverseText[reverseTextIndex++]);
				}
				else
				{
					maskedText.Append(c);
				}
			}

			if (reverseTextIndex < reverseText.Length)
			{
				maskedText.Append(reverseText.Substring(reverseTextIndex));
			}

			return Reverse(maskedText.ToString());
		}

		// Why doesn't .Net include this ??
		private static string Reverse(string forward)
		{
			char[] reverse = new char[forward.Length];
			for (int i = 0; i < forward.Length; i++)
			{
				reverse[i] = forward[forward.Length - 1 - i];
			}
			return new string(reverse);
		}
	}
}