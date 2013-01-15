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
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod.Macros;

namespace ClearCanvas.Dicom.Iod.ContextGroups
{
	partial class ContextGroupBase<T>
	{
		/// <summary>
		/// Base class representing a single DICOM context group code item.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This class is abstract because one typically needs to know the context group under which the code is to be used.
		/// The context group itself may be extensible, in which case additional user-defined codes may be created and
		/// used, or it may be non-extensible, in which case only the codes defined in the context group may be used.
		/// Context group implementors should thus take care to implement appropriate public-scoped constructors
		/// on the code items if and only if the context group is extensible.
		/// </para>
		/// <para>For additional information, please refer to the DICOM Standard 2008 PS 3.16.</para>
		/// </remarks>
		public abstract class ContextGroupItemBase : IEquatable<ContextGroupItemBase>, IEquatable<CodeSequenceMacro>
		{
			/// <summary>
			/// Gets the designator of the coding scheme in which this code is defined.
			/// </summary>
			public readonly string CodingSchemeDesignator;

			/// <summary>
			/// Gets the version of the coding scheme in which this code is defined.
			/// </summary>
			/// <remarks>
			/// May be <code>null</code> if the version is not explicitly defined for this code.
			/// </remarks>
			public readonly string CodingSchemeVersion;

			/// <summary>
			/// Gets the value of this code.
			/// </summary>
			public readonly string CodeValue;

			/// <summary>
			/// Gets the Human-readable meaning of this code.
			/// </summary>
			public readonly string CodeMeaning;

			/// <summary>
			/// Constructs a new <see cref="ContextGroupItemBase"/> where the coding scheme version is not explicitly specified.
			/// </summary>
			/// <param name="codingSchemeDesignator">The designator of the coding scheme in which this code is defined.</param>
			/// <param name="codeValue">The value of this code.</param>
			/// <param name="codeMeaning">The Human-readable meaning of this code.</param>
			/// <exception cref="ArgumentException">Thrown if <paramref name="codingSchemeDesignator"/> or <paramref name="codeValue"/> are <code>null</code> or empty.</exception>
			protected ContextGroupItemBase(string codingSchemeDesignator, string codeValue, string codeMeaning) : this(codingSchemeDesignator, null, codeValue, codeMeaning) {}

			/// <summary>
			/// Constructs a new <see cref="ContextGroupItemBase"/>.
			/// </summary>
			/// <param name="codingSchemeDesignator">The designator of the coding scheme in which this code is defined.</param>
			/// <param name="codingSchemeVersion">The version of the coding scheme in which this code is defined, if known. Should be <code>null</code> if not explicitly specified.</param>
			/// <param name="codeValue">The value of this code.</param>
			/// <param name="codeMeaning">The Human-readable meaning of this code.</param>
			/// <exception cref="ArgumentException">Thrown if <paramref name="codingSchemeDesignator"/> or <paramref name="codeValue"/> are <code>null</code> or empty.</exception>
			protected ContextGroupItemBase(string codingSchemeDesignator, string codingSchemeVersion, string codeValue, string codeMeaning)
			{
				Platform.CheckForEmptyString(codingSchemeDesignator, "codingSchemeDesignator");
				Platform.CheckForEmptyString(codeValue, "codeValue");

				this.CodingSchemeDesignator = codingSchemeDesignator;
				this.CodingSchemeVersion = codingSchemeVersion;
				this.CodeValue = codeValue;
				this.CodeMeaning = codeMeaning;
			}

			/// <summary>
			/// Gets an appropriate hash value.
			/// </summary>
			/// <remarks>
			/// The default implementation computes the hash value based only on the coding scheme designator and the code value.
			/// </remarks>
			public override int GetHashCode()
			{
				return 0x27E9AA02 ^ this.CodingSchemeDesignator.GetHashCode() ^ this.CodeValue.GetHashCode();
			}

			/// <summary>
			/// Determines whether or not the specified <see cref="object"/> is equivalent to the current code item.
			/// </summary>
			/// <param name="obj">The object with which to compare the current code item. </param>
			/// <returns>True if the specified object is a <see cref="CodeSequenceMacro"/> or a <see cref="ContextGroupItemBase"/> and they are equivalent; False otherwise.</returns>
			/// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is null.</exception>
			/// <remarks>
			/// The default implementation compares only the coding scheme designator and code value
			/// by calling <see cref="Equals(ClearCanvas.Dicom.Iod.Macros.CodeSequenceMacro)"/> or <see cref="Equals(ClearCanvas.Dicom.Iod.ContextGroups.ContextGroupBase{T}.ContextGroupItemBase)"/>.
			/// </remarks>
			public override bool Equals(object obj)
			{
				if (obj is CodeSequenceMacro)
					return this.Equals((CodeSequenceMacro) obj);
				else if (obj is ContextGroupItemBase)
					return this.Equals((ContextGroupItemBase) obj);
				return false;
			}

			/// <summary>
			/// Determines whether or not the specified <see cref="CodeSequenceMacro"/> is equivalent to the current code item.
			/// </summary>
			/// <param name="codeSequence">The code sequence with which to compare the current code item. </param>
			/// <returns>True if the specified code sequence is equivalent to the current code item; False otherwise.</returns>
			/// <exception cref="ArgumentNullException">Thrown if <paramref name="codeSequence"/> is null.</exception>
			/// <remarks>
			/// The default implementation compares only the coding scheme designator and code value
			/// by calling <see cref="Equals(ClearCanvas.Dicom.Iod.Macros.CodeSequenceMacro,bool)"/>.
			/// </remarks>
			public virtual bool Equals(CodeSequenceMacro codeSequence)
			{
				return this.Equals(codeSequence, false);
			}

			/// <summary>
			/// Determines whether or not the specified <see cref="CodeSequenceMacro"/> is equivalent to the current code item.
			/// </summary>
			/// <param name="codeSequence">The code sequence with which to compare the current code item. </param>
			/// <param name="compareCodingSchemeVersion">A value indicating whether or not the coding scheme version should be compared.</param>
			/// <returns>True if the specified code sequence is equivalent to the current code item; False otherwise.</returns>
			/// <exception cref="ArgumentNullException">Thrown if <paramref name="codeSequence"/> is null.</exception>
			/// <remarks>
			/// The default implementation compares only the coding scheme designator and code value
			/// by calling <see cref="Equals(string,string,string,string,bool)"/>.
			/// </remarks>
			public virtual bool Equals(CodeSequenceMacro codeSequence, bool compareCodingSchemeVersion)
			{
				Platform.CheckForNullReference(codeSequence, "codeSequence");
				return this.Equals(
					codeSequence.CodingSchemeDesignator,
					codeSequence.CodeValue,
					codeSequence.CodeMeaning,
					codeSequence.CodingSchemeVersion,
					compareCodingSchemeVersion);
			}

			/// <summary>
			/// Determines whether or not the specified <see cref="ContextGroupItemBase"/> is equivalent to the current code item.
			/// </summary>
			/// <param name="contextGroupItem">The code item with which to compare the current code item. </param>
			/// <returns>True if the specified code item is equivalent to the current code item; False otherwise.</returns>
			/// <exception cref="ArgumentNullException">Thrown if <paramref name="contextGroupItem"/> is null.</exception>
			/// <remarks>
			/// The default implementation compares only the coding scheme designator and code value
			/// by calling <see cref="Equals(ClearCanvas.Dicom.Iod.ContextGroups.ContextGroupBase{T}.ContextGroupItemBase,bool)"/>.
			/// </remarks>
			public virtual bool Equals(ContextGroupItemBase contextGroupItem)
			{
				return this.Equals(contextGroupItem, false);
			}

			/// <summary>
			/// Determines whether or not the specified <see cref="ContextGroupItemBase"/> is equivalent to the current code item.
			/// </summary>
			/// <param name="contextGroupItem">The code item with which to compare the current code item. </param>
			/// <param name="compareCodingSchemeVersion">A value indicating whether or not the coding scheme version should be compared.</param>
			/// <returns>True if the specified code item is equivalent to the current code item; False otherwise.</returns>
			/// <exception cref="ArgumentNullException">Thrown if <paramref name="contextGroupItem"/> is null.</exception>
			/// <remarks>
			/// The default implementation compares only the coding scheme designator and code value
			/// by calling <see cref="Equals(string,string,string,string,bool)"/>.
			/// </remarks>
			public virtual bool Equals(ContextGroupItemBase contextGroupItem, bool compareCodingSchemeVersion)
			{
				Platform.CheckForNullReference(contextGroupItem, "contextGroupItem");
				return this.Equals(
					contextGroupItem.CodingSchemeDesignator,
					contextGroupItem.CodeValue,
					contextGroupItem.CodeMeaning,
					contextGroupItem.CodingSchemeVersion,
					compareCodingSchemeVersion);
			}

			/// <summary>
			/// Determines whether or not the specified code is equivalent to the current code item.
			/// </summary>
			/// <param name="codingSchemeDesignator">The coding scheme designator with which to compare with the current code item.</param>
			/// <param name="codingSchemeVersion">The coding scheme version with which to compare with the current code item.</param>
			/// <param name="codeValue">The code value with which to compare with the current code item.</param>
			/// <param name="codeMeaning">The code meaning with which to compare with the current code item.</param>
			/// <param name="compareCodingSchemeVersion">A value indicating whether or not the coding scheme version should be compared.</param>
			/// <returns>True if the specified code is equivalent to the current code item; False otherwise.</returns>
			/// <remarks>
			/// The default implementation compares only the coding scheme designator and code value.
			/// The coding scheme version is also compared if the flag is specified.
			/// </remarks>
			public virtual bool Equals(string codingSchemeDesignator, string codeValue, string codeMeaning, string codingSchemeVersion, bool compareCodingSchemeVersion)
			{
				StringComparer comparer = StringComparer.InvariantCultureIgnoreCase;
				bool result = comparer.Equals(this.CodeValue, codeValue);
				result = result && comparer.Equals(this.CodingSchemeDesignator, codingSchemeDesignator);
				if (compareCodingSchemeVersion)
					result = result && comparer.Equals(this.CodingSchemeVersion, codingSchemeVersion);
				return result;
			}

			/// <summary>
			/// Sets the attributes of the given code sequence according to the current code item.
			/// </summary>
			/// <param name="codeSequence">A code sequence object.</param>
			/// <exception cref="ArgumentNullException">Thrown if the code sequence object is null.</exception>
			public virtual void WriteToCodeSequence(CodeSequenceMacro codeSequence)
			{
				Platform.CheckForNullReference(codeSequence, "codeSequence");

				codeSequence.CodeMeaning = this.CodeMeaning;
				codeSequence.CodeValue = this.CodeValue;
				codeSequence.CodingSchemeDesignator = this.CodingSchemeDesignator;

				if (!string.IsNullOrEmpty(this.CodingSchemeVersion))
					codeSequence.CodingSchemeVersion = this.CodingSchemeVersion;
			}

			private const string _formatValueAndSchemeDesignator = "{0} ({1})";
			private const string _formatValueMeaningAndSchemeDesignator = "{0} {1} ({2})";

			/// <summary>
			/// Gets a coded string representation of this code item.
			/// </summary>
			/// <returns>A coded string representation of this code item.</returns>
			public virtual string ToCodeString()
			{
				return string.Format(_formatValueAndSchemeDesignator, this.CodeValue, this.CodingSchemeDesignator);
			}

			/// <summary>
			/// Gets a plain string representation of this code item.
			/// </summary>
			/// <returns>A coded string representation of this code item.</returns>
			/// <remarks>
			/// The default implementation attempts to use the code meaning if available.
			/// If the code meaning is not specified, it defaults to the coded string repsentation.
			/// </remarks>
			public virtual string ToPlainString()
			{
				return string.IsNullOrEmpty(this.CodeMeaning) ? string.Format(_formatValueAndSchemeDesignator, this.CodeValue, this.CodingSchemeDesignator) : this.CodeMeaning;
			}

			/// <summary>
			/// Gets a string representation of this code item.
			/// </summary>
			/// <returns>A string representation of this code item.</returns>
			/// <remarks>
			/// The default implementation returns a representation consisting of the code value, meaning, and scheme designator.
			/// </remarks>
			public override string ToString()
			{
				if (string.IsNullOrEmpty(this.CodeMeaning))
					return string.Format(_formatValueAndSchemeDesignator, this.CodeValue, this.CodingSchemeDesignator);
				return string.Format(_formatValueMeaningAndSchemeDesignator, this.CodeValue, this.CodeMeaning, this.CodingSchemeDesignator);
			}

			/// <summary>
			/// Casts a context group item as a code sequence.
			/// </summary>
			/// <param name="value">A context group item.</param>
			/// <returns>A <see cref="CodeSequenceMacro"/> whose attribute values are those of the specified context group item.</returns>
			public static implicit operator CodeSequenceMacro(ContextGroupItemBase value)
			{
				if (value == null)
					return null;
				CodeSequenceMacro codeSequence = new CodeSequenceMacro();
				value.WriteToCodeSequence(codeSequence);
				return codeSequence;
			}

			/// <summary>
			/// Casts a context group item as a string.
			/// </summary>
			/// <param name="value">A context group item.</param>
			/// <returns>The string representation of the code item as given by <see cref="ToString"/>.</returns>
			public static implicit operator string(ContextGroupItemBase value)
			{
				if (value == null)
					return null;
				return value.ToString();
			}
		}
	}
}