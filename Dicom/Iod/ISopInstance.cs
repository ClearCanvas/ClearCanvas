using JetBrains.Annotations;

namespace ClearCanvas.Dicom.Iod
{
	public class StorageInfo
	{
		public StorageInfo(string sourceFile, long fileSize, TransferSyntax transferSyntax)
		{
			SourceFile = sourceFile;
			FileSize = fileSize;
			TransferSyntax = transferSyntax;
		}

		/// <summary>
		/// Gets the name of the source file (or blob key).
		/// </summary>
		public string SourceFile { get; private set; }

		/// <summary>
		/// Gets the size in bytes of the associated DICOM file.
		/// </summary>
		public long FileSize { get; private set; }

		/// <summary>
		/// Gets the transfer syntax.
		/// </summary>
		public TransferSyntax TransferSyntax { get; private set; }
	}

	/// <summary>
	/// Abstract representation of a sop instance, that provides <see cref="DicomAttribute"/> objects, or can construct and return an entire header.
	/// </summary>
	public interface ISopInstance : ISopInstanceData
	{
		/// <summary>
		/// Gets the series to which this SOP instance belongs.
		/// </summary>
		ISeries ParentSeries { get; }

		/// <summary>
		/// Gets the SOP class.
		/// </summary>
		SopClass SopClass { get; }

		/// <summary>
		/// Gets the storage information if applicable, otherwise null.
		/// </summary>
		StorageInfo StorageInfo { get; }

		/// <summary>
		/// Gets the Source Application Entity Title.
		/// </summary>
		string SourceApplicationEntityTitle { get; }

		/// <summary>
		/// Gets the specified DICOM attribute.
		/// </summary>
		/// <param name="dicomTag"></param>
		/// <returns></returns>
		IReadOnlyDicomAttribute GetAttribute(uint dicomTag);

		/// <summary>
		/// Gets the specified DICOM attribute.
		/// </summary>
		/// <param name="dicomTag"></param>
		/// <returns></returns>
		IReadOnlyDicomAttribute GetAttribute(DicomTag dicomTag);

		// JR: these don't seem to be used anywhere at the moment, and they add complexity to the synchronized classes,
		// therefore leave them out until needed
		//DicomFile GetCompleteSop();
		//DicomFile GetHeader(bool forceComplete);
		//IFramePixelData GetFramePixelData(int frameNumber);
	}
}