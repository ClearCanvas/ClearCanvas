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

using System.IO;

namespace ClearCanvas.Dicom.IO
{
	internal enum DicomWriteStatus
	{
		Success,
		UnknownError
	}

	internal class DicomStreamWriter
	{
		#region Private Members

		private const uint _undefinedLength = 0xFFFFFFFF;

		private readonly Stream _stream;
		private BinaryWriter _writer;
		private TransferSyntax _syntax;
		private Endian _endian;

		private ushort _group = 0xffff;

		#endregion

		#region Public Constructors

		public DicomStreamWriter(Stream stream)
		{
			_stream = stream;
		}

		#endregion

		#region Public Properties

		public TransferSyntax TransferSyntax
		{
			get { return _syntax; }
			set
			{
				_syntax = value;
				if (_endian != _syntax.Endian || _writer == null)
				{
					_endian = _syntax.Endian;
					_writer = EndianBinaryWriter.Create(_stream, _endian);
				}
			}
		}

		#endregion

		public DicomWriteStatus Write(TransferSyntax syntax, DicomAttributeCollection dataset, DicomWriteOptions options)
		{
			TransferSyntax = syntax;

			foreach (DicomAttribute item in dataset)
			{
				if (item.IsEmpty)
					continue;

				if (item.Tag.Element == 0x0000)
					continue;

				if (Flags.IsSet(options, DicomWriteOptions.CalculateGroupLengths)
				    && item.Tag.Group != _group)
				{
					_group = item.Tag.Group;
					_writer.Write((ushort) _group);
					_writer.Write((ushort) 0x0000);
					if (_syntax.ExplicitVr)
					{
						_writer.Write((byte) 'U');
						_writer.Write((byte) 'L');
						_writer.Write((ushort) 4);
					}
					else
					{
						_writer.Write((uint) 4);
					}
					_writer.Write((uint) dataset.CalculateGroupWriteLength(_group, _syntax, options));
				}

				_writer.Write((ushort) item.Tag.Group);
				_writer.Write((ushort) item.Tag.Element);

				if (_syntax.ExplicitVr)
				{
					_writer.Write((byte) item.Tag.VR.Name[0]);
					_writer.Write((byte) item.Tag.VR.Name[1]);
				}

				if (item is DicomAttributeSQ)
				{
					var sq = item as DicomAttributeSQ;

					if (_syntax.ExplicitVr)
						_writer.Write((ushort) 0x0000);

					if (Flags.IsSet(options, DicomWriteOptions.ExplicitLengthSequence))
					{
						int hl = _syntax.ExplicitVr ? 12 : 8;
						_writer.Write((uint) sq.CalculateWriteLength(_syntax, options & ~DicomWriteOptions.CalculateGroupLengths) - (uint) hl);
					}
					else
					{
						_writer.Write((uint) _undefinedLength);
					}

					foreach (DicomSequenceItem ids in (DicomSequenceItem[]) sq.Values)
					{
						_writer.Write((ushort) DicomTag.Item.Group);
						_writer.Write((ushort) DicomTag.Item.Element);

						if (Flags.IsSet(options, DicomWriteOptions.ExplicitLengthSequenceItem))
						{
							_writer.Write((uint) ids.CalculateWriteLength(_syntax, options & ~DicomWriteOptions.CalculateGroupLengths));
						}
						else
						{
							_writer.Write((uint) _undefinedLength);
						}

						Write(TransferSyntax, ids, options & ~DicomWriteOptions.CalculateGroupLengths);

						if (!Flags.IsSet(options, DicomWriteOptions.ExplicitLengthSequenceItem))
						{
							_writer.Write((ushort) DicomTag.ItemDelimitationItem.Group);
							_writer.Write((ushort) DicomTag.ItemDelimitationItem.Element);
							_writer.Write((uint) 0x00000000);
						}
					}

					if (!Flags.IsSet(options, DicomWriteOptions.ExplicitLengthSequence))
					{
						_writer.Write((ushort) DicomTag.SequenceDelimitationItem.Group);
						_writer.Write((ushort) DicomTag.SequenceDelimitationItem.Element);
						_writer.Write((uint) 0x00000000);
					}
				}

				else if (item is DicomFragmentSequence)
				{
					var fs = item as DicomFragmentSequence;

					if (_syntax.ExplicitVr)
						_writer.Write((ushort) 0x0000);
					_writer.Write((uint) _undefinedLength);

					_writer.Write((ushort) DicomTag.Item.Group);
					_writer.Write((ushort) DicomTag.Item.Element);

					if (Flags.IsSet(options, DicomWriteOptions.WriteFragmentOffsetTable) && fs.HasOffsetTable)
					{
						_writer.Write((uint) fs.OffsetTableBuffer.Length);
						fs.OffsetTableBuffer.CopyTo(_writer);
					}
					else
					{
						_writer.Write((uint) 0x00000000);
					}

					foreach (DicomFragment bb in fs.Fragments)
					{
						_writer.Write((ushort) DicomTag.Item.Group);
						_writer.Write((ushort) DicomTag.Item.Element);
						_writer.Write((uint) bb.Length);
						bb.GetByteBuffer(_syntax).CopyTo(_writer);
					}

					_writer.Write((ushort) DicomTag.SequenceDelimitationItem.Group);
					_writer.Write((ushort) DicomTag.SequenceDelimitationItem.Element);
					_writer.Write((uint) 0x00000000);
				}
				else
				{
					DicomAttribute de = item;
					ByteBuffer theData = de.GetByteBuffer(_syntax, dataset.SpecificCharacterSet);
					if (_syntax.ExplicitVr)
					{
						if (de.Tag.VR.Is16BitLengthField)
						{
							// #10890: Can't encode the value length if the length of the data exceeds max value for a 16-bit field
							if (theData.Length > ushort.MaxValue - 1 /* must be even length so max allowed = 65534 */)
								throw new DicomDataException(string.Format(
									"Value for {0} exceeds maximum stream length allowed for a {1} VR attribute encoded using {2}",
									de.Tag, de.Tag.VR, _syntax));
							_writer.Write((ushort) theData.Length);
						}
						else
						{
							_writer.Write((ushort) 0x0000);
							_writer.Write((uint) theData.Length);
						}
					}
					else
					{
						_writer.Write((uint) theData.Length);
					}

					if (theData.Length > 0)
						theData.CopyTo(_writer);
				}
			}

			return DicomWriteStatus.Success;
		}
	}
}