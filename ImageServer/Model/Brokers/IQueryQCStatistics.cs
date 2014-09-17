using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Model.Brokers
{
	public interface IQueryQCStatistics: IProcedureQueryBroker<QueryQCStatisticsParameters, QueryQCStatisticsResult>{}
}