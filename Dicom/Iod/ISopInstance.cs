namespace ClearCanvas.Dicom.Iod
{
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