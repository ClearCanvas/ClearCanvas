using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClearCanvas.ImageServer.Common.WorkQueue
{
    [WorkQueueDataType("D87D0483-7405-4B8F-B9AA-59B9E0D941CD")]
    public class StudyProcessWorkQueueData : WorkQueueData
    {
        public string ReceivingAeTitle { get; set; }
    }
}
