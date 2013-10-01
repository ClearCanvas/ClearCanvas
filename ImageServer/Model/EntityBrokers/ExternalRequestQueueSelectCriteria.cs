using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.EntityBrokers
{
    public partial class ExternalRequestQueueSelectCriteria
    {
        /// <summary>
        /// Used for EXISTS or NOT EXISTS subselects against the WorkQueue table.
        /// </summary>
        /// <remarks>
        /// A <see cref="WorkQueueSelectCriteria"/> instance is created with the subselect parameters, 
        /// and assigned to this Sub-Criteria.  Note that the link between the <see cref="ExternalRequestQueue"/>
        /// and <see cref="WorkQueue"/> tables is automatically added into the <see cref="ExternalRequestQueueSelectCriteria"/>
        /// instance by the broker.
        /// </remarks>
        public IRelatedEntityCondition<EntitySelectCriteria> WorkQueueRelatedEntityCondition
        {
            get
            {
                if (!SubCriteria.ContainsKey("WorkQueueRelatedEntityCondition"))
                {
                    SubCriteria["WorkQueueRelatedEntityCondition"] = new RelatedEntityCondition<EntitySelectCriteria>("WorkQueueRelatedEntityCondition",
                        "Key", "ExternalRequestQueueKey");
                }
                return (IRelatedEntityCondition<EntitySelectCriteria>)SubCriteria["WorkQueueRelatedEntityCondition"];
            }
        }

		[EntityFieldDatabaseMappingAttribute(TableName = "ExternalRequestQueue", ColumnName = "GUID")]
		public ISearchCondition<ServerEntityKey> Key
		{
			get
			{
				if (!SubCriteria.ContainsKey("Key"))
				{
					SubCriteria["Key"] = new SearchCondition<ServerEntityKey>("Key");
				}
				return (ISearchCondition<ServerEntityKey>)SubCriteria["Key"];
			}
		}
    }
}
