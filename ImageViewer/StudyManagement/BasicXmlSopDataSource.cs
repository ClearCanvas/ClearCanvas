using System;
using System.IO;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// An <see cref="XmlSopDataSource"/> where the full SOP, including pixel data, can be
	/// accessed via a path to a file, or a re-openable stream.
	/// </summary>
	/// <remarks>When data not in the <see cref="InstanceXml"/> is needed, the SOP is
	/// loaded using the <see cref="DicomReadOptions.StorePixelDataReferences"/>, which
	/// loads the complete DICOM header, but stores only references to the pixel data so
	/// it can be loaded more quickly at a later time.</remarks>
	public class BasicXmlSopDataSource : XmlSopDataSource
	{
		private readonly Func<Stream> _streamOpener;

		public BasicXmlSopDataSource(InstanceXml instanceXml, string path)
			: this(instanceXml, () => File.OpenRead(path))
		{
		}

		public BasicXmlSopDataSource(InstanceXml instanceXml, Func<Stream> streamOpener)
			: base(instanceXml)
		{
			_streamOpener = streamOpener;
		}

		/// <summary>
		/// Loads the SOP into a <see cref="DicomFile"/> using the <see cref="DicomReadOptions.StorePixelDataReferences"/> option.
		/// </summary>
		protected override DicomFile GetFullHeader()
		{
			var file = new DicomFile();
			file.Load(_streamOpener, null, DicomReadOptions.Default | DicomReadOptions.StorePixelDataReferences);
			return file;
		}

		protected override StandardSopFrameData CreateFrameData(int frameNumber)
		{
			//Force the full header to load before returning frame data.
			LoadFullHeader();
			return base.CreateFrameData(frameNumber);
		}
	}
}
