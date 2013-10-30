using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Enterprise
{
	public interface IStreamingServiceLayer
	{
	}

	[ExtensionPoint]
	public class StreamingServiceExtensionPoint : ExtensionPoint<IStreamingServiceLayer>
	{
	}
}