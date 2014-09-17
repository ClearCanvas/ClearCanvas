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
using System.Collections.Generic;
using System.ComponentModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Represents a record of an alert.
	/// </summary>
	internal class Alert
	{
		public Alert(AlertLevel level, DateTime time, [param : Localizable(true)] string message)
		{
			Level = level;
			Time = time;
			Message = message;
		}

		public AlertLevel Level { get; private set; }
		public DateTime Time { get; private set; }
		public string Message { get; private set; }
		public bool Acknowledged { get; set; }
	}

	/// <summary>
	/// Maintains a log of alerts that have occured during process execution.
	/// </summary>
	internal class AlertLog
	{
		private static readonly AlertLog _instance = new AlertLog();

		/// <summary>
		/// Gets the singleton instance of the alert log.
		/// </summary>
		internal static AlertLog Instance
		{
			get { return _instance; }
		}

		private const int MaxLogSize = 500;
		private readonly Queue<Alert> _alerts = new Queue<Alert>();

		private AlertLog() {}

		/// <summary>
		/// Occurs when a new alert is logged.
		/// </summary>
		public event EventHandler<ItemEventArgs<Alert>> AlertLogged;

		/// <summary>
		/// Logs a new alert.
		/// </summary>
		/// <param name="args"></param>
		public void Log(AlertNotificationArgs args)
		{
			var alert = new Alert(args.Level, Platform.Time, args.Message)
			            	{
			            		Acknowledged = args.Level == AlertLevel.Info // info alerts are "pre-acknowledged" (do not require acknowledgement)
			            	};

			_alerts.Enqueue(alert);
			while (_alerts.Count > MaxLogSize)
				_alerts.Dequeue();

			EventsHelper.Fire(AlertLogged, this, new ItemEventArgs<Alert>(alert));
		}

		/// <summary>
		/// Marks any unacknowledged alerts as being acknowledged.
		/// </summary>
		public void AcknowledgeAll()
		{
			foreach (var alert in _alerts)
			{
				alert.Acknowledged = true;
			}
		}

		/// <summary>
		/// Returns the alert log entries in chronological order.
		/// </summary>
		public IEnumerable<Alert> Entries
		{
			get { return _alerts; }
		}
	}
}