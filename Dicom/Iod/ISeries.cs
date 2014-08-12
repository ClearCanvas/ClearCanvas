using System.Collections.Generic;

namespace ClearCanvas.Dicom.Iod
{
	/// <summary>
	/// Abstract representation of a series with child sops.
	/// </summary>
	public interface ISeries : ISeriesData
	{
		/// <summary>
		/// Gets the study to which this series belongs.
		/// </summary>
		IStudy ParentStudy { get; }

		/// <summary>
		/// Gets the total number of SOP instances in this series.
		/// </summary>
		int SopInstanceCount { get; }

		/// <summary>
		/// Gets the specified SOP instance, or null if the instance does not exist.
		/// </summary>
		/// <param name="sopInstanceUid"></param>
		/// <returns></returns>
		ISopInstance GetSopInstance(string sopInstanceUid);

		/// <summary>
		/// Gets the first SOP instance in this series.
		/// </summary>
		ISopInstance FirstSopInstance { get; }

		/// <summary>
		/// Enumerates the SOP instances in this study.
		/// </summary>
		/// <returns></returns>
		IEnumerable<ISopInstance> EnumerateSopInstances();

		string StationName { get; }
		string Manufacturer { get; }
		string ManufacturersModelName { get; }

		string InstitutionName { get; }
		string InstitutionAddress { get; }
		string InstitutionalDepartmentName { get; }
	}
}