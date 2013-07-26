using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Core.Web
{
	/// <summary>
	/// Specialization of <see cref="RequestValidationException"/> that indicates that the requested
	/// REST resource was not found.
	/// </summary>
	public class ResourceNotFoundException : RequestValidationException
	{
		public ResourceNotFoundException(string message)
			: base(message)
		{
		}
	}
}