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

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// Specifies when the real event handler will be triggered by the <see cref="DelayedEventPublisher"/>.
	/// </summary>
	public enum DelayedEventPublisherTriggerMode
	{
		/// <summary>
		/// The real event handler will be triggered after a period of inactivity.
		/// </summary>
		Inactivity,

		/// <summary>
		/// The real event handler will be triggered periodically.
		/// </summary>
		Periodic
	}

	/// <summary>
	/// Base class for <see cref="DelayedEventPublisher"/> and <see cref="DelayedEventPublisher{T}"/>.
	/// </summary>
	/// <seealso cref="DelayedEventPublisher"/>
	/// <seealso cref="DelayedEventPublisher{T}"/>
	public abstract class DelayedEventPublisherBase : IDisposable
	{
		protected const int DefaultTimeout = 350;
		protected const DelayedEventPublisherTriggerMode DefaultTrigger = DelayedEventPublisherTriggerMode.Inactivity;

		private Timer _timer;
		private readonly EventHandler _realEventHandler;

		private int _timeoutMilliseconds;
		private readonly DelayedEventPublisherTriggerMode _trigger;

		private int _lastPublishTicks;
		private object _lastSender;
		private EventArgs _lastArgs;

		/// <summary>
		/// Initializes a new instance of <see cref="DelayedEventPublisher"/>.
		/// </summary>
		/// <param name="realEventHandler">The event handler which will be called when the timeout has elapsed.</param>
		/// <param name="timeoutMilliseconds">The timeout period, in milliseconds, for triggering the real event handler. The default is 350 ms.</param>
		/// <param name="trigger">Specifies when the real event handler is called after a period of inactivity, or periodically after the last event was raised. The default is to trigger on inactivity.</param>
		internal DelayedEventPublisherBase(EventHandler realEventHandler, int timeoutMilliseconds = DefaultTimeout, DelayedEventPublisherTriggerMode trigger = DefaultTrigger)
		{
			Platform.CheckForNullReference(realEventHandler, "realEventHandler");
			Platform.CheckPositive(timeoutMilliseconds, "timeoutMilliseconds");

			_realEventHandler = realEventHandler;
			_timeoutMilliseconds = Math.Max(10, timeoutMilliseconds);
			_trigger = trigger;

			_timer = new Timer(OnTimer) {IntervalMilliseconds = 10};
		}

		private void OnTimer(object nothing)
		{
			if (_timer == null || !_timer.Enabled)
				return;

			if (Math.Abs(Environment.TickCount - _lastPublishTicks) >= _timeoutMilliseconds)
				Publish();
		}

		private void Publish()
		{
			_timer.Stop();

			EventsHelper.Fire(_realEventHandler, _lastSender, _lastArgs);
			_lastSender = null;
			_lastArgs = null;

			// for period trigger, reset the timeout now
			if (_trigger == DelayedEventPublisherTriggerMode.Periodic) _lastPublishTicks = Environment.TickCount;
		}

		/// <summary>
		/// Gets and sets the timeout period, in milliseconds, for triggering the real event handler. The default is 350 ms.
		/// </summary>
		public int TimeoutMilliseconds
		{
			get { return _timeoutMilliseconds; }
			set { _timeoutMilliseconds = value; }
		}

		/// <summary>
		/// Cancels the currently pending delayed event, if one exists.
		/// </summary>
		public void Cancel()
		{
			_timer.Stop();
			_lastSender = null;
			_lastArgs = null;
		}

		/// <summary>
		/// Publishes the currently pending delayed event immediately. This method does nothing if there is no pending event.
		/// </summary>
		public void PublishNow()
		{
			if (_timer.Enabled)
				Publish();
		}

		/// <summary>
		/// Publishes an event with the specified input parameters immediately. If there is a pending delayed event, it is discarded.
		/// </summary>
		/// <param name="sender">The apparent sender of the event to be passed to the real event handler.</param>
		/// <param name="args">The <see cref="EventArgs"/> to be passed to the real event handler.</param>
		protected void PublishNowCore(object sender, EventArgs args)
		{
			// for inactivity trigger, reset the timeout now
			if (_trigger == DelayedEventPublisherTriggerMode.Inactivity) _lastPublishTicks = Environment.TickCount;

			_lastSender = sender;
			_lastArgs = args;

			Publish();
		}

		/// <summary>
		/// Delay-publishes an event with the specified input parameters.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Repeated calls to <see cref="PublishCore"/> will cause
		/// only the most recent event parameters to be remembered until the delay
		/// timeout has expired, at which time only those event parameters will
		/// be used to publish the delayed event.
		/// </para>
		/// <para>
		/// When a delayed event is published, the <see cref="DelayedEventPublisher"/>
		/// goes into an idle state.  The next call to <see cref="PublishCore"/>
		/// starts the delayed publishing process over again.
		/// </para>
		/// </remarks>
		/// <param name="sender">The apparent sender of the event to be passed to the real event handler.</param>
		/// <param name="args">The <see cref="EventArgs"/> to be passed to the real event handler.</param>
		protected void PublishCore(object sender, EventArgs args)
		{
			// for inactivity trigger, reset the timeout now
			if (_trigger == DelayedEventPublisherTriggerMode.Inactivity) _lastPublishTicks = Environment.TickCount;

			_lastSender = sender;
			_lastArgs = args;

			_timer.Start();
		}

		/// <summary>
		/// Disposes the <see cref="DelayedEventPublisher"/>.
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing && _timer != null)
			{
				_timer.Dispose();
				_timer = null;
			}
		}

		#region IDisposable Members

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern.
		/// </summary>
		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}

		#endregion
	}

	/// <summary>
	/// A utility for delaying or throttling the publication of an event.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This utility is typically used to delay and/or throttle an event that tends to fire many times within a short period of time, triggering
	/// numerous UI updates too fast for the user to register, and potentially even impacting on UI responsiveness. 
	/// </para>
	/// <para>
	/// When triggered on inactivity, the timeout period speciifes the time after which, if <see cref="Publish(object, EventArgs)"/> has not been called, 
	/// the delayed event will be published. An example of this usage is on property change events to delay notification until the property value has stabilized.
	/// </para>
	/// <para>
	/// When triggered periodically, the timeout period specifies the minimum interval between publishing consecutive events while <see cref="Publish(object, EventArgs)"/>
	/// is being called. An example of this usage is on a progress provider to throttle notification while still allowing periodic notification.
	/// </para>
	/// <para>
	/// As this class uses a <see cref="Timer"/> internally, it <b>must</b> be instantiated from a UI thread; see <see cref="Timer"/> for more details.
	/// </para>
	/// </remarks>
	/// <seealso cref="Timer"/>
	public sealed class DelayedEventPublisher : DelayedEventPublisherBase
	{
		/// <summary>
		/// Initializes a new instance of <see cref="DelayedEventPublisher"/>.
		/// </summary>
		/// <param name="realEventHandler">The event handler which will be called when the timeout has elapsed.</param>
		/// <param name="timeoutMilliseconds">The timeout period, in milliseconds, for triggering the real event handler. The default is 350 ms.</param>
		/// <param name="trigger">Specifies when the real event handler is called after a period of inactivity, or periodically after the last event was raised. The default is to trigger on inactivity.</param>
		public DelayedEventPublisher(EventHandler realEventHandler, int timeoutMilliseconds = DefaultTimeout, DelayedEventPublisherTriggerMode trigger = DefaultTrigger)
			: base(realEventHandler, timeoutMilliseconds, trigger) {}

		/// <summary>
		/// Delay-publishes an event with the specified input parameters.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Repeated calls to <see cref="Publish(object, EventArgs)"/> will cause
		/// only the most recent event parameters to be remembered until the delay
		/// timeout has expired, at which time only those event parameters will
		/// be used to publish the delayed event.
		/// </para>
		/// <para>
		/// When a delayed event is published, the <see cref="DelayedEventPublisher"/>
		/// goes into an idle state.  The next call to <see cref="Publish(object, EventArgs)"/>
		/// starts the delayed publishing process over again.
		/// </para>
		/// </remarks>
		/// <param name="sender">The apparent sender of the event to be passed to the real event handler.</param>
		/// <param name="args">The <see cref="EventArgs"/> to be passed to the real event handler.</param>
		public void Publish(object sender, EventArgs args)
		{
			PublishCore(sender, args);
		}

		/// <summary>
		/// Publishes an event with the specified input parameters immediately. If there is a pending delayed event, it is discarded.
		/// </summary>
		/// <param name="sender">The apparent sender of the event to be passed to the real event handler.</param>
		/// <param name="args">The <see cref="EventArgs"/> to be passed to the real event handler.</param>
		public void PublishNow(object sender, EventArgs args)
		{
			PublishNowCore(sender, args);
		}
	}

	/// <summary>
	/// A utility for delaying or throttling the publication of an event.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This utility is typically used to delay and/or throttle an event that tends to fire many times within a short period of time, triggering
	/// numerous UI updates too fast for the user to register, and potentially even impacting on UI responsiveness. 
	/// </para>
	/// <para>
	/// When triggered on inactivity, the timeout period speciifes the time after which, if <see cref="Publish(object, T)"/> has not been called, 
	/// the delayed event will be published. An example of this usage is on property change events to delay notification until the property value has stabilized.
	/// </para>
	/// <para>
	/// When triggered periodically, the timeout period specifies the minimum interval between publishing consecutive events while <see cref="Publish(object, T)"/>
	/// is being called. An example of this usage is on a progress provider to throttle notification while still allowing periodic notification.
	/// </para>
	/// <para>
	/// As this class uses a <see cref="Timer"/> internally, it <b>must</b> be instantiated from a UI thread; see <see cref="Timer"/> for more details.
	/// </para>
	/// </remarks>
	/// <seealso cref="Timer"/>
	public sealed class DelayedEventPublisher<T> : DelayedEventPublisherBase
		where T : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of <see cref="DelayedEventPublisher{T}"/>.
		/// </summary>
		/// <param name="realEventHandler">The event handler which will be called when the timeout has elapsed.</param>
		/// <param name="timeoutMilliseconds">The timeout period, in milliseconds, for triggering the real event handler. The default is 350 ms.</param>
		/// <param name="trigger">Specifies when the real event handler is called after a period of inactivity, or periodically after the last event was raised. The default is to trigger on inactivity.</param>
		public DelayedEventPublisher(EventHandler<T> realEventHandler, int timeoutMilliseconds = DefaultTimeout, DelayedEventPublisherTriggerMode trigger = DefaultTrigger)
			: base((s, e) => realEventHandler.Invoke(s, (T) e), timeoutMilliseconds, trigger) {}

		/// <summary>
		/// Delay-publishes an event with the specified input parameters.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Repeated calls to <see cref="Publish(object, T)"/> will cause
		/// only the most recent event parameters to be remembered until the delay
		/// timeout has expired, at which time only those event parameters will
		/// be used to publish the delayed event.
		/// </para>
		/// <para>
		/// When a delayed event is published, the <see cref="DelayedEventPublisher{T}"/>
		/// goes into an idle state.  The next call to <see cref="Publish(object, T)"/>
		/// starts the delayed publishing process over again.
		/// </para>
		/// </remarks>
		/// <param name="sender">The apparent sender of the event to be passed to the real event handler.</param>
		/// <param name="args">The <see cref="EventArgs"/> to be passed to the real event handler.</param>
		public void Publish(object sender, T args)
		{
			PublishCore(sender, args);
		}

		/// <summary>
		/// Publishes an event with the specified input parameters immediately. If there is a pending delayed event, it is discarded.
		/// </summary>
		/// <param name="sender">The apparent sender of the event to be passed to the real event handler.</param>
		/// <param name="args">The <see cref="EventArgs"/> to be passed to the real event handler.</param>
		public void PublishNow(object sender, T args)
		{
			PublishNowCore(sender, args);
		}
	}
}