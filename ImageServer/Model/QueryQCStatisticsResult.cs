using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model
{
	[Serializable]
	public partial class QueryQCStatisticsResult : ServerEntity
	{
		public QueryQCStatisticsResult()
			: base("QueryQCStatisticsResult")
		{
		}

		[EntityFieldDatabaseMappingAttribute(ColumnName = "Checking")]
		public int Checking { get; set; }

		[EntityFieldDatabaseMappingAttribute(ColumnName = "Passed")]
		public int Passed { get; set; }

		[EntityFieldDatabaseMappingAttribute(ColumnName = "Failed")]
		public int Failed { get; set; }

		[EntityFieldDatabaseMappingAttribute(ColumnName = "Incomplete")]
		public int Incomplete { get; set; }

		[EntityFieldDatabaseMappingAttribute(ColumnName = "NotApplicable")]
		public int NotApplicable { get; set; }

		[EntityFieldDatabaseMappingAttribute(ColumnName = "OrdersForQC")]
		public int OrdersForQC { get; set; }

	}
}
