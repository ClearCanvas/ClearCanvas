using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.ExternalRequest;

namespace ClearCanvas.ImageServer.Model.EntityBrokers
{
    public partial class ExternalRequestQueueUpdateColumns
    {
        public ImageServerExternalRequestState SerializeState
        {           
            set { StateXml = ImageServerSerializer.SerializeExternalRequestStateToXmlDocument(value); }
        }

        public ImageServerExternalRequest SerializeRequest
        {
            set { RequestXml = ImageServerSerializer.SerializeExternalRequestToXmlDocument(value); }
        }
    }
}
