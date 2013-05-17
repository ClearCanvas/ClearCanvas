using System;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.ImageServer.Common.WorkQueue
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class WorkQueueDataTypeAttribute : PolymorphicDataContractAttribute
    {
        public WorkQueueDataTypeAttribute(string dataContractGuid)
            : base(dataContractGuid)
        {
        }
    }
}
