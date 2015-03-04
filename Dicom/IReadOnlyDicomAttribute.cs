using System;
using JetBrains.Annotations;

namespace ClearCanvas.Dicom
{
	/// <summary>
	/// Defines the interface to a read-only representation of a DICOM attribute.
	/// </summary>
	public interface IReadOnlyDicomAttribute
	{
		/// <summary>
		/// Gets the <see cref="DicomTag"/> instance for the attribute.
		/// </summary>
		DicomTag Tag { get; }

		/// <summary>
		/// Gets the description of the <see cref="DicomTag"/> instance.
		/// </summary>
		string DicomTagDescription { get; }

		/// <summary>
		/// Gets the entire attribute as a string value.
		/// </summary>
		/// <returns></returns>
		string GetStringValue();

		/// <summary>
		/// Gets a value indicating whether the attribute is null.
		/// </summary>
		bool IsNull { get; }

		/// <summary>
		/// Gets a value indicating whether the attribute is empty.
		/// </summary>
		bool IsEmpty { get; }

		/// <summary>
		/// The number of values assigned to the attribute.
		/// </summary>
		long Count { get; }

		bool TryGetUInt16(int i, out ushort value);
		bool TryGetInt16(int i, out Int16 value);
		bool TryGetUInt32(int i, out UInt32 value);
		bool TryGetInt32(int i, out Int32 value);
		bool TryGetUInt64(int i, out UInt64 value);
		bool TryGetInt64(int i, out Int64 value);
		bool TryGetFloat32(int i, out float value);
		bool TryGetFloat64(int i, out double value);
		bool TryGetString(int i, out String value);

		/// <summary>
		/// Method to retrieve a <see cref="DateTime"/> attribute for the tag.
		/// </summary>
		/// <param name="i">A zero index value to retrieve.</param>
		/// <param name="value"></param>
		/// <returns>true on success, false on failure.</returns>
		bool TryGetDateTime(int i, out DateTime value);

		/// <summary>
		/// Retrieves a <see cref="DicomUid"/> instance for a value.
		/// </summary>
		/// <remarks>This function only works for <see cref="DicomAttributeUI"/> attributes.</remarks>
		/// <param name="i"></param>
		/// <param name="value"></param>
		/// <returns>True on success, false on failure.</returns>
		bool TryGetUid(int i, out DicomUid value);

		/// <summary>
		/// Retrieve a value as a UInt16.
		/// If the value doesn't exist or cannot be converted into UInt16, the <i>defaultVal</i> will be returned.
		/// </summary>
		/// <param name="i"></param>
		/// <param name="defaultVal"></param>
		/// <returns></returns>
		UInt16 GetUInt16(int i, UInt16 defaultVal);

		/// <summary>
		/// Retrieve a value as a UInt32.
		/// If the value doesn't exist or cannot be converted into UInt32, the <i>defaultVal</i> will be returned.
		/// </summary>
		/// <param name="i"></param>
		/// <param name="defaultVal"></param>
		/// <returns></returns>
		UInt32 GetUInt32(int i, UInt32 defaultVal);

		/// <summary>
		/// Retrieve a value as a UInt64.
		/// If the value doesn't exist or cannot be converted into UInt64, the <i>defaultVal</i> will be returned.
		/// </summary>
		/// <param name="i"></param>
		/// <param name="defaultVal"></param>
		/// <returns></returns>
		UInt64 GetUInt64(int i, UInt64 defaultVal);

		/// <summary>
		/// Retrieve a value as a Int16.
		/// If the value doesn't exist or cannot be converted into Int16, the <i>defaultVal</i> will be returned.
		/// </summary>
		/// <param name="i"></param>
		/// <param name="defaultVal"></param>
		/// <returns></returns>        
		Int16 GetInt16(int i, Int16 defaultVal);

		/// <summary>
		/// Retrieve a value as a Int32.
		/// If the value doesn't exist or cannot be converted into Int32, the <i>defaultVal</i> will be returned.
		/// </summary>
		/// <param name="i"></param>
		/// <param name="defaultVal"></param>
		/// <returns></returns>
		Int32 GetInt32(int i, Int32 defaultVal);

		/// <summary>
		/// Retrieve a value as an Int64.
		/// If the value doesn't exist or cannot be converted into Int64, the <i>defaultVal</i> will be returned.
		/// </summary>
		/// <param name="i"></param>
		/// <param name="defaultVal"></param>
		/// <returns></returns>
		Int64 GetInt64(int i, Int64 defaultVal);

		/// <summary>
		/// Retrieve a value as a float.
		/// If the value doesn't exist or cannot be converted into float, the <i>defaultVal</i> will be returned.
		/// </summary>
		/// <param name="i"></param>
		/// <param name="defaultVal"></param>
		/// <returns></returns>
		float GetFloat32(int i, float defaultVal);

		/// <summary>
		/// Retrieve a value as a double.
		/// If the value doesn't exist or cannot be converted into double, the <i>defaultVal</i> will be returned.
		/// </summary>
		/// <param name="i"></param>
		/// <param name="defaultVal"></param>
		/// <returns></returns>
		double GetFloat64(int i, double defaultVal);

		/// <summary>
		/// Retrieve a value as a string.
		/// </summary>
		/// <param name="i"></param>
		/// <param name="defaultVal"></param>
		/// <returns></returns>
		String GetString(int i, String defaultVal);

		/// <summary>
		/// Retrieve a datetime value.
		/// If the value cannot be converted into a <see cref="DateTime"/> object, <i>defaultVal</i> will be returned.
		/// </summary>
		/// <param name="i"></param>
		/// <param name="defaultVal"></param>
		/// <returns></returns>
		DateTime GetDateTime(int i, DateTime defaultVal);

		/// <summary>
		/// Retrieve a datetime value.
		/// If the value cannot be converted into a <see cref="DateTime"/> object, <i>null</i> will be returned.
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		DateTime? GetDateTime(int i);

		/// <summary>
		/// Retrieve an UID value. 
		/// 
		/// <see cref="DicomUid"/> 
		/// </summary>
		/// <param name="i"></param>
		/// <param name="defaultVal"></param>
		/// <returns></returns>
		DicomUid GetUid(int i, DicomUid defaultVal);

		/// <summary>
		/// Gets the specified sequence item from the attribute (applicable only to SQ attributes).
		/// </summary>
		/// <param name="i">The index of the item to retrieve.</param>
		/// <returns>The sequence item, or null if there is no item at the specified index.</returns>
		/// <exception cref="DicomException">The attribute is not a sequence.</exception>
		[CanBeNull]
		IReadOnlyDicomSequenceItem GetSequenceItem(int i);

		/// <summary>
		/// Gets the specified sequence item from the attribute, if it exists.
		/// </summary>
		/// <remarks>
		/// For non-sequence attributes, this method always returns false.
		/// </remarks>
		bool TryGetSequenceItem(int i, out IReadOnlyDicomSequenceItem value);
	}
}