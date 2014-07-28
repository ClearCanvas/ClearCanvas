#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Text;

namespace ClearCanvas.Enterprise.Core.Msmq
{
	/// <summary>
	/// Represents the name of an MSMQ queue.
	/// </summary>
	/// <remarks>
	/// Instances of this class are immutable.
	/// </remarks>
	public class MsmqName
	{
		/// <summary>
		/// Creates an <see cref="MsmqName"/> instance for the local queue with the specified name.
		/// </summary>
		/// <param name="queueName"></param>
		/// <returns></returns>
		public static MsmqName ForLocalQueue(string queueName)
		{
			if (string.IsNullOrEmpty(queueName))
				throw new ArgumentNullException("queueName");

			return new MsmqName(null, queueName, null);
		}

		/// <summary>
		/// Creates an <see cref="MsmqName"/> instance for the local subqueue with the specified name and subqueue name.
		/// </summary>
		/// <param name="queueName"></param>
		/// <param name="subQueueName"> </param>
		/// <returns></returns>
		public static MsmqName ForLocalSubQueue(string queueName, string subQueueName)
		{
			if (string.IsNullOrEmpty(queueName))
				throw new ArgumentNullException("queueName");
			if (string.IsNullOrEmpty(subQueueName))
				throw new ArgumentNullException("subQueueName");

			return new MsmqName(null, queueName, subQueueName);
		}

		/// <summary>
		/// Creates an <see cref="MsmqName"/> instance for the remote queue with the specified name.
		/// </summary>
		/// <param name="hostName"> </param>
		/// <param name="queueName"></param>
		/// <returns></returns>
		public static MsmqName ForRemoteQueue(string hostName, string queueName)
		{
			if (string.IsNullOrEmpty(hostName))
				throw new ArgumentNullException("hostName");
			if (string.IsNullOrEmpty(queueName))
				throw new ArgumentNullException("queueName");

			return new MsmqName(hostName, queueName, null);
		}

		/// <summary>
		/// Creates an <see cref="MsmqName"/> instance for the remote subqueue with the specified name and subqueue name.
		/// </summary>
		/// <param name="hostName"> </param>
		/// <param name="queueName"></param>
		/// <param name="subQueueName"> </param>
		/// <returns></returns>
		public static MsmqName ForRemoteSubQueue(string hostName, string queueName, string subQueueName)
		{
			if (string.IsNullOrEmpty(hostName))
				throw new ArgumentNullException("hostName");
			if (string.IsNullOrEmpty(queueName))
				throw new ArgumentNullException("queueName");
			if (string.IsNullOrEmpty(subQueueName))
				throw new ArgumentNullException("subQueueName");

			return new MsmqName(hostName, queueName, subQueueName);
		}

		private readonly string _hostName;
		private readonly string _queueName;
		private readonly string _subQueueName;

		private MsmqName(string hostName, string queueName, string subQueueName)
		{
			if(string.IsNullOrEmpty(queueName))
				throw new ArgumentNullException("queueName");

			_hostName = hostName;
			_queueName = queueName;
			_subQueueName = subQueueName;
		}

		/// <summary>
		/// Gets the queue name.
		/// </summary>
		public string QueueName
		{
			get { return _queueName; }
		}

		/// <summary>
		/// Gets the subqueue name if this instance refers to a subqueue, otherwise null.
		/// </summary>
		public string SubQueueName
		{
			get { return _subQueueName; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance refers to a subqueue.
		/// </summary>
		public bool IsSubQueue
		{
			get { return !string.IsNullOrEmpty(_subQueueName); }
		}

		/// <summary>
		/// Gets the path representation of this instance.
		/// </summary>
		public string Path
		{
			get
			{
				if(IsSubQueue)
					throw new InvalidOperationException("A subqueue cannot be referred to by a path.");

				// can use . to refer to local host
				return string.Format(@"{0}\Private$\{1}", _hostName ?? ".", _queueName);
			}
		}

		/// <summary>
		/// Gets the "format name" representation of this instance.
		/// </summary>
		public string FormatName
		{
			get
			{
				var sb = new StringBuilder();
				sb.AppendFormat(@"DIRECT=OS:{0}\Private$\{1}", _hostName ?? "localhost", _queueName);

				if (!string.IsNullOrEmpty(_subQueueName))
					sb.Append(";").Append(_subQueueName);

				return sb.ToString();
			}
		}

		/// <summary>
		/// Returns a new <see cref="MsmqName"/> object that refers to the specified subqueue.
		/// </summary>
		/// <param name="subQueueName"></param>
		/// <returns></returns>
		public MsmqName GetSubQueueName(string subQueueName)
		{
			return new MsmqName(_hostName, _queueName, subQueueName);
		}
	}
}
