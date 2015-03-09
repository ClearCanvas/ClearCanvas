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
using System.IO;
using System.Text;
using ClearCanvas.Dicom.IO;

namespace ClearCanvas.Dicom
{
	/// <summary>
	/// Compressed DICOM fragment.
	/// </summary>
	public class DicomFragment
	{
		#region Private members

		private readonly FileReference _reference = null;
		private readonly ByteBuffer _bb = null;

		#endregion

		#region Constructors

		internal DicomFragment(FileReference reference)
		{
			_reference = reference;
		}

		internal DicomFragment(DicomFragment source)
		{
			if (source._reference != null)
				_reference = source._reference;
			else
			{
				_bb = source._bb;
			}
		}

		public DicomFragment(ByteBuffer bb)
		{
			_bb = bb;
		}

		#endregion

		#region Properties

		public uint Length
		{
			get
			{
				if (_reference != null)
					return _reference.Length;

				return (uint) _bb.Length;
			}
		}

		#endregion

		#region Private Methods

		private ByteBuffer Load()
		{
			if (_reference != null)
			{
				ByteBuffer bb;
				using (var fs = _reference.StreamOpener.Open())
				{
					fs.Seek(_reference.Offset, SeekOrigin.Begin);

					bb = new ByteBuffer();
					bb.Endian = _reference.Endian;
					bb.CopyFrom(fs, (int) _reference.Length);
					fs.Close();
				}
				return bb;
			}

			return null;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Get a byte array for the frame's data.
		/// </summary>
		/// <returns></returns>
		public byte[] GetByteArray()
		{
			if (_reference != null)
				return Load().ToBytes();

			return _bb.ToBytes();
		}

		/// <summary>
		/// Get a <see cref="ByteBuffer"/> for the frame's data.
		/// </summary>
		/// <returns></returns>
		public ByteBuffer GetByteBuffer(TransferSyntax syntax)
		{
			if (_reference != null)
				return Load(); // no need to swap, always OB

			return new ByteBuffer(GetByteArray(), ByteBuffer.LocalMachineEndian);
		}

		#endregion

		#region Overrides

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType()) return false;

			DicomFragment frame = (DicomFragment) obj;

			byte[] source = this.GetByteArray();
			byte[] dest = frame.GetByteArray();

			for (int index = 0; index < source.Length; index++)
				if (!source[index].Equals(dest[index]))
					return false;

			return true;
		}

		public override int GetHashCode()
		{
			byte[] source = this.GetByteArray();
			int hash = 0;
			for (int index = 0; index < source.Length; index++)
			{
				hash += (index + 1)*source[index].GetHashCode();
			}
			return hash;
		}

		#endregion
	}

	/// <summary>
	/// <see cref="DicomAttribute"/> representing compressed pixel data encoding rules.
	/// </summary>
	public class DicomFragmentSequence : DicomAttribute
	{
		#region Protected Members

		protected List<uint> _table;
		protected List<DicomFragment> _fragments = new List<DicomFragment>();
		private bool _isNull = false;

		#endregion

		#region Constructors

		public DicomFragmentSequence(uint tag) : base(tag) {}

		public DicomFragmentSequence(DicomTag tag) : base(tag) {}

		internal DicomFragmentSequence(DicomFragmentSequence attrib)
			: base(attrib)
		{
			_isNull = attrib._isNull;
			foreach (DicomFragment fragment in attrib._fragments)
			{
				_fragments.Add(new DicomFragment(fragment));
			}
		}

		#endregion

		#region Public Properties

		public bool HasOffsetTable
		{
			get { return _table != null; }
		}

		public ByteBuffer OffsetTableBuffer
		{
			get
			{
				ByteBuffer offsets = new ByteBuffer();
				if (_table != null)
				{
					foreach (uint offset in _table)
					{
						offsets.Writer.Write(offset);
					}
				}
				return offsets;
			}
		}

		public List<uint> OffsetTable
		{
			get
			{
				if (_table == null)
					_table = new List<uint>();
				return _table;
			}
		}

		public IList<DicomFragment> Fragments
		{
			get { return _fragments; }
		}

		#endregion

		#region Public Methods

		public void SetOffsetTable(ByteBuffer table)
		{
			_table = new List<uint>();
			_table.AddRange(table.ToUInt32s());
		}

		public void SetOffsetTable(List<uint> table)
		{
			_table = new List<uint>(table);
		}

		public void AddFragment(DicomFragment fragment)
		{
			_fragments.Add(fragment);
		}

		#endregion

		#region Overrides

		public override string ToString()
		{
			return Tag + " Compressed with " + _fragments.Count + " fragments";
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType()) return false;

			DicomFragmentSequence sq = (DicomFragmentSequence) obj;

			if (sq.Count != this.Count)
				return false;

			for (int i = 0; i < Count; i++)
			{
				if (!_fragments[i].Equals(sq._fragments[i]))
					return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return 0;
		}

		public override long Count
		{
			get { return _fragments.Count; }
			protected set { }
		}

		public override bool IsNull
		{
			get { return _isNull; }
		}

		public override bool IsEmpty
		{
			get { return _fragments.Count == 0; }
		}

		public override object Values
		{
			get { return _fragments.ToArray(); }
			set
			{
				_fragments.Clear();
				_fragments.AddRange((DicomFragment[]) value);
			}
		}

		public override DicomAttribute Copy()
		{
			return new DicomFragmentSequence(this);
		}

		public override Type GetValueType()
		{
			return typeof (byte);
		}

		public override void SetNullValue()
		{
			_fragments.Clear();
			_isNull = true;
		}

		public override void SetEmptyValue()
		{
			_fragments.Clear();
			_isNull = false;
		}

		internal override ByteBuffer GetByteBuffer(TransferSyntax syntax, string specificCharacterSet)
		{
			throw new NotImplementedException();
		}

		internal override DicomAttribute Copy(bool copyBinary)
		{
			return new DicomFragmentSequence(this);
		}

		internal override uint CalculateWriteLength(TransferSyntax syntax, DicomWriteOptions options)
		{
			uint length = 0;
			length += 4; // element tag
			if (syntax.ExplicitVr)
			{
				length += 2; // vr
				length += 6; // length
			}
			else
			{
				length += 4; // length
			}

			// write the offset table (item tag is always present, though content may be zero length)
			length += 4 + 4; // item tag and length
			if (Flags.IsSet(options, DicomWriteOptions.WriteFragmentOffsetTable) && _table != null)
				length += (uint) (_table.Count*4);

			foreach (DicomFragment fragment in this._fragments)
			{
				length += 4; // item tag
				length += 4; // fragment length
				length += fragment.Length;

				// no Item Delimitation Item (fragments are always explicit length)
			}

			length += 4 + 4; // Sequence Delimitation Item (fragment sequences are always undefined length)

			return length;
		}

		#endregion

		#region Dump

		/// <summary>
		/// Method for dumping the contents of the attribute to a string.
		/// </summary>
		/// <param name="sb">The StringBuilder to write the attribute to.</param>
		/// <param name="prefix">A prefix to place before the value.</param>
		/// <param name="options">The <see cref="DicomDumpOptions"/> to use for the output string.</param>
		public override void Dump(StringBuilder sb, string prefix, DicomDumpOptions options)
		{
			int ValueWidth = 40 - prefix.Length;
			int SbLength = sb.Length;

			sb.Append(prefix);
			sb.AppendFormat("({0:x4},{1:x4}) {2} ", Tag.Group, Tag.Element, Tag.VR.Name);
			long length = 0;
			foreach (DicomFragment frag in Fragments)
			{
				length += frag.Length;
			}

			String value = string.Format("{0} encapsulated fragment{1} of {2} bytes", Fragments.Count, Fragments.Count > 1 ? "s" : "", length);
			sb.Append(value.PadRight(ValueWidth, ' '));

			sb.AppendFormat(" # {0,4} {2} {1}", StreamLength, Tag.VM, Tag.Name.Replace("&apos;", "'"));

			if (Flags.IsSet(options, DicomDumpOptions.Restrict80CharactersPerLine))
			{
				if (sb.Length > (SbLength + 79))
				{
					sb.Length = SbLength + 79;
					//sb.Append(">");
				}
			}
		}

		#endregion
	}
}