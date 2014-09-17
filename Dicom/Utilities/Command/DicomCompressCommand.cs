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
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Dicom.Codec;

namespace ClearCanvas.Dicom.Utilities.Command
{
	/// <summary>
	/// Command for compressing a DICOM Sop Instance.
	/// </summary>
	public class DicomCompressCommand : CommandBase
	{
		private readonly DicomMessageBase _file;
		private readonly IDicomCodec _codec;
		private readonly DicomCodecParameters _parms;
		private readonly TransferSyntax _syntax;
		private readonly TimeSpanStatistics _timeSpan = new TimeSpanStatistics("CompressTime");
		private readonly bool _failOnCodecException;

		public TimeSpanStatistics CompressTime
		{
			get { return _timeSpan; }
		}

		public DicomCompressCommand(DicomMessageBase file, TransferSyntax syntax, IDicomCodec codec, DicomCodecParameters parms)
			: base("DICOM Compress Command", true)
		{

			_file = file;
			_syntax = syntax;
			_codec = codec;
			_parms = parms;
			_failOnCodecException = true;
		}

		public DicomCompressCommand(DicomMessageBase file, XmlDocument parms, bool failOnCodecException)
			: base("DICOM Compress Command", true)
		{
			_file = file;
			_failOnCodecException = failOnCodecException;

			XmlElement element = parms.DocumentElement;

			string syntax = element.Attributes["syntax"].Value;

			_syntax = TransferSyntax.GetTransferSyntax(syntax);
			if (_syntax == null)
			{
				string failureDescription =
					String.Format("Invalid transfer syntax in compression command: {0}", element.Attributes["syntax"].Value);
				Platform.Log(LogLevel.Error, "Error with input syntax: {0}", failureDescription);
				throw new DicomCodecException(failureDescription);
			}

			IDicomCodecFactory[] codecs = DicomCodecRegistry.GetCodecFactories();
			IDicomCodecFactory theCodecFactory = null;
			foreach (IDicomCodecFactory codec in codecs)
				if (codec.CodecTransferSyntax.Equals(_syntax))
				{
					theCodecFactory = codec;
					break;
				}

			if (theCodecFactory == null)
			{
				string failureDescription = String.Format("Unable to find codec for compression: {0}", _syntax.Name);
				Platform.Log(LogLevel.Error, "Error with compression input parameters: {0}", failureDescription);
				throw new DicomCodecException(failureDescription);
			}

			_codec = theCodecFactory.GetDicomCodec();
			_parms = theCodecFactory.GetCodecParameters(parms);
		}

		protected override void OnExecute(CommandProcessor theProcessor)
		{
			// Check if its already in the right syntax.
			if (_file.TransferSyntax.Equals(_syntax))
				return;

			_timeSpan.Start();

			try
			{
				// Check for decompression first
				if (_file.TransferSyntax.Encapsulated)
					_file.ChangeTransferSyntax(TransferSyntax.ExplicitVrLittleEndian);

				_file.ChangeTransferSyntax(_syntax, _codec, _parms);
			}
			catch (Exception x)
			{
				if (_failOnCodecException)
					throw;

				Platform.Log(LogLevel.Warn, "Unexpected exception compressing SOP: {0}", x.Message);
			}
			finally
			{
				_timeSpan.End();
			}
		}

		protected override void OnUndo()
		{
			
		}
	}
}