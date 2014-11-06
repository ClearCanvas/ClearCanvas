#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS
//
// The ClearCanvas RIS/PACS is free software: you can redistribute it 
// and/or modify it under the terms of the GNU General Public License 
// as published by the Free Software Foundation, either version 3 of 
// the License, or (at your option) any later version.
//
// ClearCanvas RIS/PACS is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with ClearCanvas RIS/PACS.  If not, 
// see <http://www.gnu.org/licenses/>.

#endregion

#if UNIT_TESTS

using System.Xml;
using System.IO;

namespace ClearCanvas.Dicom.Codec.Tests
{
	/// <summary>
	/// A null implementation of a <see cref="IDicomCodec"/> that simply stores the frame data in frame fragments and calls it &quot;compressed&quot;.
	/// </summary>
	public sealed class NullDicomCodec : IDicomCodec
	{
		public const string TransferSyntaxUid = "1.3.6.1.4.1.25403.2200303521616.1888.20130201032029.1";
		private const string _codecName = "Null Compression Codec Implementation for Unit Tests";

		public static readonly TransferSyntax TransferSyntax = new TransferSyntax(_codecName,
		                                                                          TransferSyntaxUid,
		                                                                          true, true, true, false, false, true);

		public static readonly IDicomCodecFactory DicomCodecFactory = new CodecFactory(_codecName);

		public static void Register()
		{
			TransferSyntax.RegisterTransferSyntax(TransferSyntax);
			DicomCodecRegistry.SetCodec(TransferSyntax, DicomCodecFactory);
		}

		public static void Unregister()
		{
			DicomCodecRegistry.SetCodec(TransferSyntax, null);
			TransferSyntax.UnregisterTransferSyntax(TransferSyntax);
		}

		public string Name
		{
			get { return TransferSyntax.Name; }
		}

		public TransferSyntax CodecTransferSyntax
		{
			get { return TransferSyntax; }
		}

		public void Encode(DicomUncompressedPixelData oldPixelData, DicomCompressedPixelData newPixelData, DicomCodecParameters parameters)
		{
			if (oldPixelData.UncompressedFrameSize%2 == 1)
			{
				for (var n = 0; n < oldPixelData.NumberOfFrames; ++n)
				{
					using (var stream = new MemoryStream())
					{
						var data = oldPixelData.GetFrame(n);
						stream.Write(data, 0, data.Length);
						stream.WriteByte(0); // must pad fragments to even length
						newPixelData.AddFrameFragment(stream.ToArray());
					}
				}
			}
			else
			{
				for (var n = 0; n < oldPixelData.NumberOfFrames; ++n)
				{
					newPixelData.AddFrameFragment(oldPixelData.GetFrame(n));
				}
			}
		}

		public void Decode(DicomCompressedPixelData oldPixelData, DicomUncompressedPixelData newPixelData, DicomCodecParameters parameters)
		{
			for (var n = 0; n < oldPixelData.NumberOfFrames; ++n)
				DecodeFrame(n, oldPixelData, newPixelData, parameters);
		}

		public void DecodeFrame(int frame, DicomCompressedPixelData oldPixelData, DicomUncompressedPixelData newPixelData, DicomCodecParameters parameters)
		{
			if (oldPixelData.UncompressedFrameSize%2 == 1)
			{
				using (var stream = new MemoryStream(oldPixelData.GetFrameFragmentData(frame)))
				{
					var data = new byte[oldPixelData.UncompressedFrameSize];
					stream.Read(data, 0, data.Length);
					newPixelData.AppendFrame(data);
				}
			}
			else
			{
				newPixelData.AppendFrame(oldPixelData.GetFrameFragmentData(frame));
			}
		}

		private class CodecFactory : IDicomCodecFactory
		{
			private readonly string _name;

			public CodecFactory(string name)
			{
				_name = name;
			}

			public string Name
			{
				get { return _name; }
			}

			public bool Enabled
			{
				get { return true; }
			}

			public TransferSyntax CodecTransferSyntax
			{
				get { return TransferSyntax; }
			}

			public DicomCodecParameters GetCodecParameters(DicomAttributeCollection dataSet)
			{
				return null;
			}

			public DicomCodecParameters GetCodecParameters(XmlDocument parms)
			{
				return null;
			}

			public IDicomCodec GetDicomCodec()
			{
				return new NullDicomCodec();
			}
		}
	}
}

#endif