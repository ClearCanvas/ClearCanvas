using System;

namespace ClearCanvas.Enterprise.Core.Msmq
{
	/// <summary>
	/// Defines an interface that extends a message queue to support the Move operation.
	/// </summary>
	internal interface IMoveTargetableQueue
	{
		/// <summary>
		/// Gets the queue handle to which move operations should be directed.
		/// </summary>
		IntPtr MoveHandle { get; }
	}
}
