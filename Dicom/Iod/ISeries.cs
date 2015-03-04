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
		/// Gets the collection of SOP instances in this series.
		/// </summary>
		ISopInstanceCollection SopInstances { get; }

		string StationName { get; }
		string Manufacturer { get; }
		string ManufacturersModelName { get; }

		string InstitutionName { get; }
		string InstitutionAddress { get; }
		string InstitutionalDepartmentName { get; }
	}
}