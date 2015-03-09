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
using ClearCanvas.Common;

namespace ClearCanvas.Dicom.IO
{
	internal enum DicomReadStatus
	{
		Success,
		UnknownError,
		NeedMoreData
	}

	internal class DicomStreamReader
	{
		#region Private Classes

		/// <summary>
		/// Class used to keep track of recursion within sequences
		/// </summary>
		private struct SequenceRecord
		{
			public long Pos;
			public long Len;
			public DicomAttributeCollection Parent;
			public DicomTag Tag;
			public DicomAttributeCollection Current;
			public long Curpos;
			public long Curlen;
		}

		#endregion

		#region Private Members

		private const uint _undefinedLength = 0xFFFFFFFF;

		private readonly Stream _stream;
		private BinaryReader _reader;
		private TransferSyntax _syntax;
		private Endian _endian;

		private DicomVr _vr;
		private uint _len = _undefinedLength;
		private long _pos = 0;

		private long _remain = 0;

		private long _endGroup2 = 0;
		private bool _inGroup2 = false;

		private readonly Stack<SequenceRecord> _sqrs = new Stack<SequenceRecord>();

		private DicomFragmentSequence _fragment;

		#endregion

		#region Public Constructors

		public DicomStreamReader(Stream stream)
		{
			BytesNeeded = 0;
			_stream = stream;
			TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;
			EncounteredStopTag = false;
		}

		#endregion

		#region Public Properties

		public TransferSyntax TransferSyntax
		{
			get { return _syntax; }
			set
			{
				_syntax = value;
				_endian = _syntax.Endian;
				_reader = EndianBinaryReader.Create(_stream, _endian);
			}
		}

		public DicomAttributeCollection Dataset { get; set; }

		public DicomStreamOpener StreamOpener { get; set; }

		public long BytesEstimated { get; private set; }

		public long BytesRead { get; private set; }

		public uint BytesNeeded { get; set; }

		public DicomTag LastTagRead { get; private set; }
		public DicomTag SaveTagRead { get; private set; }

		public bool EncounteredStopTag { get; private set; }

		public long EndGroupTwo
		{
			get { return _endGroup2; }
		}

		#endregion

		private DicomReadStatus NeedMoreData(long count)
		{
			BytesNeeded = (uint) count;
			return DicomReadStatus.NeedMoreData;
		}

		public DicomReadStatus Read(DicomTag stopAtTag, DicomReadOptions options)
		{
			if (stopAtTag == null)
				stopAtTag = new DicomTag(0xFFFFFFFF, "Bogus Tag", "BogusTag", DicomVr.UNvr, false, 1, 1, false);

			// Counters:
			//  _remain - bytes remaining in stream
			//  _bytes - estimates bytes to end of dataset
			//  _read - number of bytes read from stream
			try
			{
				BytesNeeded = 0;
				_remain = _stream.Length - _stream.Position;

				while (_remain > 0)
				{
					if (_inGroup2 && BytesRead >= _endGroup2)
					{
						_inGroup2 = false;
						// Only change if we're still reading the meta info
						if (Dataset.StartTagValue < DicomTags.TransferSyntaxUid)
						{
							TransferSyntax group2Syntax =
								TransferSyntax.GetTransferSyntax(
									Dataset[DicomTags.TransferSyntaxUid].GetString(0, String.Empty));
							if (group2Syntax == null)
								throw new DicomException("Unsupported transfer syntax in group 2 elements");
							TransferSyntax = group2Syntax;
						}
					}
					uint tagValue;
					if (LastTagRead == null)
					{
						if (_remain < 4)
							return NeedMoreData(4);

						_pos = _stream.Position;
						ushort g = _reader.ReadUInt16();
						ushort e = _reader.ReadUInt16();
						tagValue = DicomTag.GetTagValue(g, e);
						if (DicomTag.IsPrivateGroup(g) && e > 0x00ff)
						{
							SaveTagRead = LastTagRead = DicomTagDictionary.GetDicomTag(g, e) ??
							                            new DicomTag((uint) g << 16 | e, "Private Tag", "PrivateTag", DicomVr.UNvr, false, 1, uint.MaxValue, false);
						}
						else
						{
							if (e == 0x0000)
								SaveTagRead = LastTagRead = new DicomTag((uint) g << 16 | e, "Group Length", "GroupLength", DicomVr.ULvr, false, 1, 1, false);
							else
							{
								SaveTagRead = LastTagRead = DicomTagDictionary.GetDicomTag(g, e) ??
								                            new DicomTag((uint) g << 16 | e, "Private Tag", "PrivateTag", DicomVr.UNvr, false, 1, uint.MaxValue, false);
							}
						}
						_remain -= 4;
						BytesEstimated += 4;
						BytesRead += 4;
					}
					else
						tagValue = LastTagRead.TagValue;

					if ((tagValue >= stopAtTag.TagValue)
					    && (_sqrs.Count == 0)) // only exit in root message when after stop tag
					{
						if (_inGroup2 && tagValue > 0x0002FFFF)
						{
							if (_endGroup2 != BytesRead - 4)
							{
								Platform.Log(LogLevel.Debug, "File Meta Info Length, {0}, not equal to actual bytes read in file, {1}, overwriting length.",
								             EndGroupTwo, BytesRead - 4);
								_endGroup2 = BytesRead - 4;
							}
							_inGroup2 = false;
						}
						EncounteredStopTag = true;
						return DicomReadStatus.Success;
					}

					bool twoByteLength;
					if (_vr == null)
					{
						if (_syntax.ExplicitVr)
						{
							if (LastTagRead == DicomTag.Item ||
							    LastTagRead == DicomTag.ItemDelimitationItem ||
							    LastTagRead == DicomTag.SequenceDelimitationItem)
							{
								_vr = DicomVr.NONE;
								twoByteLength = _vr.Is16BitLengthField;
							}
							else
							{
								if (_remain < 2)
									return NeedMoreData(2);

								string vr = new string(_reader.ReadChars(2));
								_vr = DicomVr.GetVR(vr);
								twoByteLength = _vr.Is16BitLengthField;
								_remain -= 2;
								BytesEstimated += 2;
								BytesRead += 2;
								if (LastTagRead.VR.Equals(DicomVr.UNvr))
								{
									LastTagRead = new DicomTag(LastTagRead.TagValue, "Private Tag", "PrivateTag", _vr, false, 1, uint.MaxValue, false);
									if (vr.Equals("??"))
										twoByteLength = true;
								}
								else if (!LastTagRead.VR.Equals(_vr))
								{
									if (!vr.Equals("  "))
									{
										DicomTag tag =
											new DicomTag(LastTagRead.TagValue, LastTagRead.Name, LastTagRead.VariableName, _vr, LastTagRead.MultiVR,
											             LastTagRead.VMLow, LastTagRead.VMHigh,
											             LastTagRead.Retired);
										LastTagRead = tag;

										; // TODO, log something
									}
								}
							}
						}
						else
						{
							_vr = LastTagRead.VR;
							twoByteLength = _vr.Is16BitLengthField;
						}

						if (_vr == DicomVr.UNvr)
						{
							if (LastTagRead.IsPrivate)
							{
								if (LastTagRead.Element <= 0x00ff && LastTagRead.Element >= 0x0010)
								{
									// Reset the tag with the right VR and a more descriptive name.
									LastTagRead = new DicomTag(LastTagRead.TagValue, "Private Creator Code", "PrivateCreatorCode", DicomVr.LOvr, false, 1, uint.MaxValue, false);

									// private creator id
									// Only set the VR to LO for Implicit VR, if we do it for
									// Explicit VR syntaxes, we would incorrectly read the tag 
									// length below.
									if (!_syntax.ExplicitVr)
										_vr = DicomVr.LOvr;
								}
								else if (_stream.CanSeek && Flags.IsSet(options, DicomReadOptions.AllowSeekingForContext))
								{
									// attempt to identify private sequence by checking if the tag has
									// an undefined length
									long pos = _stream.Position;

									int bytesToCheck = _syntax.ExplicitVr ? 6 : 4;

									if (_remain >= bytesToCheck)
									{
										if (_syntax.ExplicitVr)
											_reader.ReadUInt16();

										uint l = _reader.ReadUInt32();
										if (l == _undefinedLength)
											_vr = DicomVr.SQvr;
									}
									_stream.Position = pos;
								}
							}
						}
					}
					else
						twoByteLength = _vr.Is16BitLengthField;

					// Read the value length
					if (_len == _undefinedLength)
					{
						if (_syntax.ExplicitVr)
						{
							if (LastTagRead == DicomTag.Item ||
							    LastTagRead == DicomTag.ItemDelimitationItem ||
							    LastTagRead == DicomTag.SequenceDelimitationItem)
							{
								if (_remain < 4)
									return NeedMoreData(4);

								_len = _reader.ReadUInt32();
								_remain -= 4;
								BytesEstimated += 4;
								BytesRead += 4;
							}
							else
							{
								if (twoByteLength)
								{
									if (_remain < 2)
										return NeedMoreData(2);

									_len = _reader.ReadUInt16();
									_remain -= 2;
									BytesEstimated += 2;
									BytesRead += 2;
								}
								else
								{
									if (_remain < 6)
										return NeedMoreData(6);

									_reader.ReadByte();
									_reader.ReadByte();
									_len = _reader.ReadUInt32();
									_remain -= 6;
									BytesEstimated += 6;
									BytesRead += 6;
								}
							}
						}
						else
						{
							if (_remain < 4)
								return NeedMoreData(4);

							_len = _reader.ReadUInt32();
							_remain -= 4;
							BytesEstimated += 4;
							BytesRead += 4;
						}

						if ((_len != _undefinedLength)
						    && !_vr.Equals(DicomVr.SQvr)
						    && !(LastTagRead.Equals(DicomTag.Item)
						         && _fragment == null))
							BytesEstimated += _len;
					}

					// If we have a private creator code, set the VR to LO, because
					// that is what it is.  We must do this after we read the length
					// so that the 32 bit length is read properly.
					if ((LastTagRead.IsPrivate)
					    && (_vr.Equals(DicomVr.UNvr))
					    && (LastTagRead.Element <= 0x00ff))
						_vr = DicomVr.LOvr;

					if (_fragment != null)
					{
						// In the middle of parsing pixels
						if (LastTagRead == DicomTag.Item)
						{
							if (_remain < _len)
								return NeedMoreData(_remain - _len);

							if (Flags.IsSet(options, DicomReadOptions.StorePixelDataReferences)
							    && _fragment.HasOffsetTable)
							{
								FileReference reference = new FileReference(StreamOpener, _stream.Position, _len, _endian, DicomVr.OBvr);
								DicomFragment fragment = new DicomFragment(reference);
								_fragment.AddFragment(fragment);
								if (_stream.CanSeek)
									_stream.Seek(_len, SeekOrigin.Current);
								else
									ConsumeStreamBytes(_stream, _len);
							}
							else
							{
								ByteBuffer data = new ByteBuffer(_endian, _len);
								data.CopyFrom(_stream, (int) _len);

								if (!_fragment.HasOffsetTable)
									_fragment.SetOffsetTable(data);
								else
								{
									DicomFragment fragment = new DicomFragment(data);
									_fragment.AddFragment(fragment);
								}
							}

							_remain -= _len;
							BytesRead += _len;
						}
						else if (LastTagRead == DicomTag.SequenceDelimitationItem)
						{
							if (_sqrs.Count > 0)
							{
								SequenceRecord rec = _sqrs.Peek();
								DicomAttributeCollection ds = rec.Current;

								ds[_fragment.Tag] = _fragment;

								if (rec.Curlen != _undefinedLength)
								{
									long end = rec.Curpos + rec.Curlen;
									if (_stream.Position >= end)
									{
										rec.Current = null;
									}
								}
							}
							else
							{
								Dataset[_fragment.Tag] = _fragment;
							}

							_fragment = null;
						}
						else
						{
							Platform.Log(LogLevel.Error, "Encountered unexpected tag in stream: {0}", LastTagRead.ToString());
							// unexpected tag
							return DicomReadStatus.UnknownError;
						}
					}
					else if (_sqrs.Count > 0 &&
					         (LastTagRead == DicomTag.Item ||
					          LastTagRead == DicomTag.ItemDelimitationItem ||
					          LastTagRead == DicomTag.SequenceDelimitationItem))
					{
						SequenceRecord rec = _sqrs.Peek();

						if (LastTagRead.Equals(DicomTag.Item))
						{
							if (_len != _undefinedLength)
							{
								if (_len > _remain)
									return NeedMoreData(_remain - _len);
							}

							DicomSequenceItem ds;

							if (rec.Tag.TagValue.Equals(DicomTags.DirectoryRecordSequence))
							{
								DirectoryRecordSequenceItem dr = new DirectoryRecordSequenceItem
								                                 {
									                                 Offset = (uint) _pos
								                                 };

								ds = dr;
							}
							else
								ds = new DicomSequenceItem();

							rec.Current = ds;
							if (rec.Tag.VR.Equals(DicomVr.UNvr))
							{
								DicomTag tag = new DicomTag(rec.Tag.TagValue, rec.Tag.Name,
								                            rec.Tag.VariableName, DicomVr.SQvr, rec.Tag.MultiVR, rec.Tag.VMLow,
								                            rec.Tag.VMHigh, rec.Tag.Retired);
								rec.Parent[tag].AddSequenceItem(ds);
							}
							else
								rec.Parent[rec.Tag].AddSequenceItem(ds);

							// Specific character set is inherited, save it.  It will be overwritten
							// if a new value of the tag is encountered in the sequence.
							rec.Current.SpecificCharacterSet = rec.Parent.SpecificCharacterSet;

							// save the sequence length
							rec.Curpos = _pos + 8;
							rec.Curlen = _len;

							_sqrs.Pop();
							_sqrs.Push(rec);

							if (_len != _undefinedLength)
							{
								ByteBuffer data = new ByteBuffer(_endian, _len);
								data.CopyFrom(_stream, (int) _len);
								data.Stream.Position = 0;
								_remain -= _len;
								BytesRead += _len;

								DicomStreamReader idsr = new DicomStreamReader(data.Stream)
								                         {
									                         Dataset = ds,
									                         TransferSyntax = rec.Tag.VR.Equals(DicomVr.UNvr)
										                         ? TransferSyntax.ImplicitVrLittleEndian
										                         : _syntax,
									                         StreamOpener = StreamOpener
								                         };
								DicomReadStatus stat = idsr.Read(null, options & ~DicomReadOptions.StorePixelDataReferences);
								if (stat != DicomReadStatus.Success)
								{
									Platform.Log(LogLevel.Error, "Unexpected parsing error ({0}) when reading sequence attribute: {1}.", stat, rec.Tag.ToString());
									return stat;
								}
							}
						}
						else if (LastTagRead == DicomTag.ItemDelimitationItem) {}
						else if (LastTagRead == DicomTag.SequenceDelimitationItem)
						{
							SequenceRecord rec2 = _sqrs.Pop();
							if (rec2.Current == null)
								rec2.Parent[rec.Tag].SetNullValue();
						}

						if (rec.Len != _undefinedLength)
						{
							long end = rec.Pos + 8 + rec.Len;
							if (_syntax.ExplicitVr)
								end += 2 + 2;
							if (_stream.Position >= end)
							{
								_sqrs.Pop();
							}
						}
					}
					else
					{
						if (_len == _undefinedLength)
						{
							if (_vr.Equals(DicomVr.UNvr))
							{
								if (!_syntax.ExplicitVr)
								{
									_vr = DicomVr.SQvr;
									LastTagRead = LastTagRead.IsPrivate
										? new DicomTag(LastTagRead.TagValue, "Private Tag", "PrivateTag", DicomVr.SQvr, false, 1, uint.MaxValue, false)
										: new DicomTag(LastTagRead.TagValue, "Unknown Tag", "UnknownTag", DicomVr.SQvr, false, 1, uint.MaxValue, false);
								}
								else
								{
									// To handle this case, we'd have to add a new mechanism to transition the parser to implicit VR parsing,
									// and then return back to implicit once the parsing of the SQ is complete.
									Platform.Log(LogLevel.Error,
									             "Encountered unknown tag {0}, encoded as undefined length in an Explicit VR transfer syntax at offset {1}.  Unable to parse.",
									             LastTagRead, _stream.Position);
									return DicomReadStatus.UnknownError;
								}
							}

							if (_vr.Equals(DicomVr.SQvr))
							{
								SequenceRecord rec = new SequenceRecord
								                     {
									                     Parent = _sqrs.Count > 0
										                     ? _sqrs.Peek().Current
										                     : Dataset,
									                     Current = null,
									                     Tag = LastTagRead,
									                     Len = _undefinedLength
								                     };

								_sqrs.Push(rec);
							}
							else
							{
								_fragment = new DicomFragmentSequence(LastTagRead);
							}
						}
						else
						{
							if (_vr.Equals(DicomVr.SQvr))
							{
								if (_len == 0)
								{
									DicomAttributeCollection ds;
									if (_sqrs.Count > 0)
									{
										SequenceRecord rec = _sqrs.Peek();
										ds = rec.Current;
									}
									else
										ds = Dataset;

									ds[LastTagRead].SetNullValue();
								}
								else
								{
									SequenceRecord rec = new SequenceRecord
									                     {
										                     Len = _len,
										                     Pos = _pos,
										                     Tag = LastTagRead,
										                     Parent = _sqrs.Count > 0
											                     ? _sqrs.Peek().Current
											                     : Dataset
									                     };

									_sqrs.Push(rec);
								}
							}
							else
							{
								if (_remain < _len)
									return NeedMoreData(_len - _remain);

								if ((LastTagRead.TagValue == DicomTags.PixelData)
								    && Flags.IsSet(options, DicomReadOptions.DoNotStorePixelDataInDataSet))
								{
									// Skip PixelData !!
									if (_stream.CanSeek)
										_stream.Seek((int) _len, SeekOrigin.Current);
									else
										ConsumeStreamBytes(_stream, _len);

									_remain -= _len;
									BytesRead += _len;
								}
								else if ((LastTagRead.TagValue == DicomTags.PixelData) &&
								         Flags.IsSet(options, DicomReadOptions.StorePixelDataReferences))
								{
									var reference = new FileReference(StreamOpener, _stream.Position, _len, _endian, LastTagRead.VR);
									if (_stream.CanSeek)
										_stream.Seek((int) _len, SeekOrigin.Current);
									else
										ConsumeStreamBytes(_stream, _len);

									DicomAttribute elem;
									if (LastTagRead.VR.Equals(DicomVr.OWvr))
									{
										elem = new DicomAttributeOW(LastTagRead, reference);
									}
									else if (LastTagRead.VR.Equals(DicomVr.OBvr))
									{
										elem = new DicomAttributeOB(LastTagRead, reference);
									}
									else if (LastTagRead.VR.Equals(DicomVr.ODvr))
									{
										elem = new DicomAttributeOD(LastTagRead, reference);
									}
									else
									{
										elem = new DicomAttributeOF(LastTagRead, reference);
									}

									if (_sqrs.Count > 0)
									{
										SequenceRecord rec = _sqrs.Peek();
										DicomAttributeCollection ds = rec.Current;

										ds[LastTagRead] = elem;

										if (rec.Curlen != _undefinedLength)
										{
											long end = rec.Curpos + rec.Curlen;
											if (_stream.Position >= end)
											{
												rec.Current = null;
											}
										}
									}
									else
									{
										Dataset[LastTagRead] = elem;
									}

									_remain -= _len;
									BytesRead += _len;
								}
								else
								{
									ByteBuffer bb = new ByteBuffer(_len);
									// If the tag is impacted by specific character set, 
									// set the encoding properly.
									if (LastTagRead.VR.SpecificCharacterSet)
									{
										if (_sqrs.Count > 0)
										{
											SequenceRecord rec = _sqrs.Peek();
											bb.SpecificCharacterSet = rec.Current.SpecificCharacterSet;
										}
										else
										{
											bb.SpecificCharacterSet = Dataset.SpecificCharacterSet;
										}
									}
									if (LastTagRead.VR.Equals(DicomVr.UNvr)
									    && !SaveTagRead.VR.Equals(DicomVr.UNvr)
									    && !SaveTagRead.VR.Equals(DicomVr.SQvr)
									    && Flags.IsSet(options, DicomReadOptions.UseDictionaryForExplicitUN))
									{
										LastTagRead = SaveTagRead;
										bb.Endian = Endian.Little;
									}
									else
									{
										bb.Endian = _endian;
									}

									bb.CopyFrom(_stream, (int) _len);

									DicomAttribute elem = LastTagRead.CreateDicomAttribute(bb);

									_remain -= _len;
									BytesRead += _len;

									if (_sqrs.Count > 0)
									{
										SequenceRecord rec = _sqrs.Peek();
										DicomAttributeCollection ds = rec.Current;

										if (elem.Tag.TagValue == DicomTags.SpecificCharacterSet)
										{
											ds.SpecificCharacterSet = elem.ToString();
										}

										if (LastTagRead.Element == 0x0000)
										{
											if (Flags.IsSet(options, DicomReadOptions.KeepGroupLengths))
												ds[LastTagRead] = elem;
										}
										else
											ds[LastTagRead] = elem;

										if (rec.Curlen != _undefinedLength)
										{
											long end = rec.Curpos + rec.Curlen;
											if (_stream.Position >= end)
											{
												rec.Current = null;
											}
										}
									}
									else
									{
										if (LastTagRead.TagValue == DicomTags.FileMetaInformationGroupLength)
										{
											// Save the end of the group 2 elements, so that we can automatically 
											// check and change our transfer syntax when needed.
											_inGroup2 = true;
											uint group2Len;
											elem.TryGetUInt32(0, out group2Len);
											_endGroup2 = BytesRead + group2Len;
										}
										else if (LastTagRead.TagValue == DicomTags.SpecificCharacterSet)
										{
											Dataset.SpecificCharacterSet = elem.ToString();
										}

										if (LastTagRead.Element == 0x0000)
										{
											if (Flags.IsSet(options, DicomReadOptions.KeepGroupLengths))
												Dataset[LastTagRead] = elem;
										}
										else
											Dataset[LastTagRead] = elem;
									}
								}
							}
						}
					}

					LastTagRead = null;
					_vr = null;
					_len = _undefinedLength;
				}
				return DicomReadStatus.Success;
			}
			catch (EndOfStreamException e)
			{
				// should never happen
				Platform.Log(LogLevel.Error, "Unexpected exception when reading file: {0}", e.ToString());
				return DicomReadStatus.UnknownError;
			}
		}

		public static void ConsumeStreamBytes(Stream stream, long length)
		{
			const int bufferSize = 4096;
			int bytesLeft = (int) length;
			var buffer = new byte[bufferSize];
			while (bytesLeft > 0)
			{
				int count = stream.Read(buffer, 0, Math.Min(buffer.Length, bytesLeft));
				bytesLeft -= count;
			}
		}
	}
}