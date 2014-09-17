using ClearCanvas.Common;
using ClearCanvas.ImageServer.Enterprise.SqlServer;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Model.SqlServer.Brokers
{
	[ExtensionOf(typeof(BrokerExtensionPoint))]
	public class QueryQCStatistics : ProcedureQueryBroker<QueryQCStatisticsParameters, QueryQCStatisticsResult>, IQueryQCStatistics
	{
		public QueryQCStatistics()
			: base("QueryQCStatistics")
		{
		}
	}
}