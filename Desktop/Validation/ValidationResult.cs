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

using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ClearCanvas.Desktop.Validation
{
	/// <summary>
	/// Represents the result of an <see cref="IValidationRule"/> evaluation.
	/// </summary>
	public class ValidationResult
	{
		/// <summary>
		/// Combines a collection of validation results into a single result.
		/// </summary>
		public static ValidationResult Combine(IEnumerable<ValidationResult> results)
		{
			List<string> messages = new List<string>();
			foreach (ValidationResult result in results)
			{
				if (!result.Success)
					messages.AddRange(result.Messages);
			}

			return new ValidationResult(messages.Count == 0, messages.ToArray());
		}

		private readonly bool _success;
		private readonly string[] _messages;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="success">Indicates whether the validation succeeded.</param>
		/// <param name="messages">When validation fails, a set of messages indicating why the validation failed.</param>
		public ValidationResult(bool success, [param : Localizable(true)] params string[] messages)
		{
			_success = success;
			_messages = messages ?? new string[0];
		}

		/// <summary>
		/// True if the validation was successful.
		/// </summary>
		public bool Success
		{
			get { return _success; }
		}

		/// <summary>
		/// Messages that describe why validation was not successful.
		/// </summary>
		public string[] Messages
		{
			get { return _messages; }
		}

		/// <summary>
		/// Concatenates the elements of the <see cref="Messages"/> property into a single message using the specified separator.
		/// </summary>
		public string GetMessageString(string separator)
		{
			StringBuilder sb = new StringBuilder();
			foreach (string msg in _messages)
			{
				if (sb.Length > 0)
					sb.Append(separator);
				sb.Append(msg);
			}
			return sb.ToString();
		}
	}
}