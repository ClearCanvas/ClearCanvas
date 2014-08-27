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
using System.Threading;

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// A delegate for use by a <see cref="Timer"/> object.
	/// </summary>
	public delegate void TimerDelegate(object state);

	/// <summary>
	/// Implements a simple timer class that handles marshalling delegates back to the thread on which
	/// this object was allocated (usually the main UI thread).
	/// </summary>
	/// <remarks>
	/// This class <B>must</B> be instantiated from within a UI thread, otherwise an exception
	/// could be thrown upon construction (unless the thread has a custom <see cref="SynchronizationContext"/>).  
	/// This class relies on <see cref="SynchronizationContext.Current"/> being non-null in order to do the marshalling.
	/// Also, this class is very simple and may not be as accurate as other timer classes.
	/// </remarks>
	public sealed class Timer : IDisposable
	{
		private readonly SynchronizationContext _synchronizationContext;
		private readonly object _stateObject;
		private readonly TimerDelegate _elapsedDelegate;

	    private System.Threading.Timer _timer;
		private volatile int _intervalMilliseconds;
        private int _processing;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="elapsedDelegate">The delegate to execute on a timer.</param>
		public Timer(TimerDelegate elapsedDelegate)
			: this(elapsedDelegate, null)
		{ 
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="elapsedDelegate">The delegate to execute on a timer.</param>
		/// <param name="stateObject">A user defined state object.</param>
		public Timer(TimerDelegate elapsedDelegate, object stateObject)
			: this(elapsedDelegate, stateObject, 1000)
		{
		}

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="elapsedDelegate">The delegate to execute on a timer.</param>
        /// <param name="stateObject">A user defined state object.</param>
        /// <param name="interval">The timer interval.</param>
        public Timer(TimerDelegate elapsedDelegate, object stateObject, TimeSpan interval)
			: this(elapsedDelegate, stateObject, (int)interval.TotalMilliseconds)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="elapsedDelegate">The delegate to execute on a timer.</param>
		/// <param name="stateObject">A user defined state object.</param>
		/// <param name="intervalMilliseconds">The time to wait in milliseconds.</param>
		public Timer(TimerDelegate elapsedDelegate, object stateObject, int intervalMilliseconds)
		{
			_synchronizationContext = SynchronizationContext.Current;

			Platform.CheckForNullReference(_synchronizationContext, "SynchronizationContext.Current");
			Platform.CheckForNullReference(elapsedDelegate, "elapsedDelegate");
			
			_stateObject = stateObject; 
			_elapsedDelegate = elapsedDelegate;

			_intervalMilliseconds = intervalMilliseconds;
		}

		/// <summary>
		/// Gets whether or not the timer is currently running.
		/// </summary>
		public bool Enabled
		{
            get { return _timer != null; }
		}

		/// <summary>
		/// Sets the timer interval in milliseconds.
		/// </summary>
		/// <remarks>
		/// The default value is 1000 milliseconds, or 1 second.
		/// </remarks>
		public int IntervalMilliseconds
		{
			get { return _intervalMilliseconds; }
			set { _intervalMilliseconds = value; }
		}

		/// <summary>
		/// Starts the timer.
		/// </summary>
		public void Start()
		{
		    if (_timer == null)
                _timer = new System.Threading.Timer(OnTimer, _stateObject, _intervalMilliseconds, _intervalMilliseconds);
		}

		/// <summary>
		/// Stops the timer.
		/// </summary>
		public void Stop()
		{
            if (_timer == null)return;
		    _timer.Dispose();
		    _timer = null;
		}

		#region IDisposable Members

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern.
		/// </summary>
		public void Dispose()
		{
			try
			{
				Stop();
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}

		#endregion

		private void OnElapsed(object nothing)
		{
		    try
		    {
                if (Enabled)
                    _elapsedDelegate(_stateObject);
		    }
		    finally
		    {
                //The next one can be posted.
		        Interlocked.Exchange(ref _processing, 0);
		    }
		}

		private void OnTimer(object nothing)
		{
		    //A couple things:
            //1. System.Threading.Timer allows re-entrancy, so you can actually get
            //   multiple callbacks executing at once on different thread pool threads.
            //2. Depending on how long the "elapsed" callback takes, things can get pretty
            //   out of hand with the message pump filling up, so we actually delay
            //   posting again until the current user callback is done.
            if (0 == Interlocked.Exchange(ref _processing, 1))
                _synchronizationContext.Post(OnElapsed, null);
		}
	}
}
