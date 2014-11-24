namespace ClearCanvas.Dicom
{
	/// <summary>
	/// Provides read-only access to a <see cref="DicomSequenceItem"/> instance.
	/// </summary>
	/// <remarks>
	/// A <see cref="DicomSequenceItem"/> instance that is accessed solely via this interface is guaranteed
	/// to be externally immutable. However, no guarantee can be made as to the immutability of the internal
	/// state of the object, and therefore synchronization is required if the object is to be safely used
	/// by multiple threads.
	/// </remarks>
	public interface IReadOnlyDicomSequenceItem
	{
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
	}
}