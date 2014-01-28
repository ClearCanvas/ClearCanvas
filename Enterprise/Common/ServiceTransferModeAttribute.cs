using System;
using System.ServiceModel;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// When applied to a service contract interface, specifies the <see cref="TransferMode"/> to be used.
	/// </summary>
	[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
	public class ServiceTransferModeAttribute : Attribute
	{
		/// <summary>
		/// Tests a service contract to see the required <see cref="TransferMode"/>.
		/// </summary>
		/// <param name="serviceContract"></param>
		/// <returns></returns>
		public static TransferMode GetTransferMode(Type serviceContract)
		{
			var authAttr = AttributeUtils.GetAttribute<ServiceTransferModeAttribute>(serviceContract);
			return authAttr != null ? authAttr.Mode : TransferMode.Buffered;
		}

		private readonly TransferMode _mode;

		public ServiceTransferModeAttribute(TransferMode mode)
        {
            _mode = mode;
        }

        /// <summary>
        /// Gets a value indicating whether authentication is required.
        /// </summary>
		public TransferMode Mode
        {
            get { return _mode; }
        }
	}
}
