using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageServer.Common.ServiceModel
{
	[DataContract]
	public class GetSopDataRequest
	{
		[DataMember]
		public string ServerAE { get; set; }
		[DataMember]
		public string StudyInstanceUid { get; set; }
		[DataMember]
		public string SeriesInstanceUid { get; set; }
		[DataMember]
		public string SopInstanceUid { get; set; }
	}
	
	[DataContract]
	public class GetSopRequest : GetSopDataRequest
	{
		//TODO: later.
		//[DataMember]
		//public uint? StopTag { get; set; }
	}
	
	[ImageServerStreamingService]
	[Authentication(false)]
	[ServiceContract]
	public interface ISopDataService
	{
		//TODO: duplicate some of the WADO functions, like GetFramePixelData.
		[OperationContract]
		Stream GetSop(GetSopRequest request);
	}
}
