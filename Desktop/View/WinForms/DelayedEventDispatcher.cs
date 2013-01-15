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

/* ==============================================================================

 THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
 ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
 THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
 PARTICULAR PURPOSE.

 � 2005, LaMarvin. All Rights Reserved.
 ============================================================================== */

// Author: Palo Mraz
// Code taken from http://www.vbinfozine.com/a_deh.shtml

using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.View.WinForms
{

  /// <summary>
  /// The class implements 
  /// </summary>
	public sealed class DelayedEventDispatcher
	{

    /// <summary>
    /// Initializes a new instance of the <see cref="EventDispatcher"/> class with the specified
    /// event delegate.
    /// </summary>
    /// <param name="processEventDelegate">
    /// The delegate that actually raises or processes the delayed event.
    /// </param>
    public DelayedEventDispatcher(
      Delegate processEventDelegate) : this(processEventDelegate, 350, false)
    {
    }



    /// <summary>
    /// Initializes a new instance of the <see cref="EventDispatcher"/> class with the specified
    /// event delegate and the amount of milliseconds the event has to be postponed.
    /// </summary>
    /// <param name="processEventDelegate">
    /// The delegate that actually raises or processes the delayed event.
    /// </param>
    /// <param name="delayMilliseconds">
    /// The number of milliseconds the event has to be postponed.
    /// </param>
    public DelayedEventDispatcher(
      Delegate processEventDelegate,
      int delayMilliseconds) : this(processEventDelegate, delayMilliseconds, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventDispatcher"/> class with the specified
    /// event delegate and the amount of milliseconds the event has to be postponed.
    /// </summary>
    /// <param name="processEventDelegate">
    /// The delegate that actually raises or processes the delayed event.
    /// </param>
    /// <param name="delayMilliseconds">
    /// The number of milliseconds the event has to be postponed.
    /// </param>
    /// <param name="delayMouseEvents">
    /// The flag indicates whether events should be delayed for events caused by mouse
    /// input.
    /// </param>
		public DelayedEventDispatcher(
      Delegate processEventDelegate,
      int delayMilliseconds, 
      bool delayMouseEvents)
		{
      // Validate the event delegate.
      if (processEventDelegate == null)
        throw new ArgumentNullException("processEventDelegate");
      
      ParameterInfo[] parameters = processEventDelegate.Method.GetParameters();
      if (parameters == null || parameters.Length != 2 || !typeof(EventArgs).IsAssignableFrom(parameters[1].ParameterType))
		  throw new ArgumentException(SR.ExceptionDelayedEventDelegateArgument, "processEventDelegate");

      if (delayMilliseconds < 0)
		  throw new ArgumentException(SR.ExceptionValueMustNotBeNegative, "delayMilliseconds");

      this._processEventDelegate = processEventDelegate;
      this._delay = TimeSpan.FromMilliseconds(delayMilliseconds);
      this._delayMouseEvents = delayMouseEvents;

      // Save the value to be passed to the GetAsyncKeyState API.
      if (SystemInformation.MouseButtonsSwapped)
        this._vkLeftButton = VK_RBUTTON;
      else
        this._vkLeftButton = VK_LBUTTON;
      
      this._timer = new Timer();
      this._timer.Interval = 20;
      this._timer.Tick += new EventHandler(_timer_Tick);
      this._timer.Enabled = true;
		}


    /// <summary>
    /// 
    /// </summary>
    /// <param name="args">
    /// </param>
    public void RegisterAuthenticEvent(object sender, EventArgs args)
    {
      // Save the event state.
      this._recentEventArgs = args;
      this._recentSender = sender;

	  //TODO (Time Review): Use Environment.TickCount, or just delete this and use the DelayedEventPublisher.
		this._recentEventTime = Platform.Time;
      
      // If postponing is disabled, raise the event directly.
      if (!this.DelayEnabled)
      {
        this.HandleEvent();
        return;
      }

      // If the event was caused by a mouse, raise the event "almost" immediately (by pretending
      // the event occured at leat this._delay milliseconds in the past).
      // Please note: I'm saying almost because we're still using the timer to raise the
      // event on the next Tick (at most 20 milliseconds from now). This ensures that
      // a TreeView, for example, will display the new node in a selected state before 
      // the AfterClick event is raised.
      if (!this._delayMouseEvents && (GetAsyncKeyState(this._vkLeftButton) != 0))
		  this._recentEventTime = Platform.Time - this._delay - TimeSpan.FromMilliseconds(100);
    }


    /// <summary>
    /// Sets or returns the interval the associated event has to be delayed.
    /// </summary>
    public TimeSpan Delay
    {
      get
      {
        return this._delay;
      }
      set
      {
        this._delay = value;
      }
    }


    /// <summary>
    /// Sets or returns a flag indicating whether the delayed event handling
    /// feature is active.
    /// </summary>
    public bool DelayEnabled
    {
      get
      {
        return this._timer.Enabled;
      }
      set
      {
        this._timer.Enabled = value;
      }
    }


    /// <summary>
    /// Sets or returns a flag indicating whether the delayed event handling
    /// feature is active for events caused by mouse input.
    /// </summary>
    public bool DelayMouseEvents
    {
      get
      {
        return this._delayMouseEvents;
      }
      set
      {
        this._delayMouseEvents = value;
      }
    }


    /// <summary>
    /// Checks to see if an event to be raised is available and the required amount
    /// of time already passed and raises the event.
    /// </summary>
    private void _timer_Tick(object sender, EventArgs e)
    {
      if (this._recentEventArgs == null)
        return;

	TimeSpan elapsed = Platform.Time - this._recentEventTime;
      if (elapsed < this._delay)
        return;
    
      this.HandleEvent();
    }


    private void HandleEvent()
    {
      Debug.Assert(this._recentEventArgs != null);

      try
      {
        // Prepare the argument array and discard the recent event args BEFORE 
        // invoking the event delegate. Otherwise reentrant calls could occur if the
        // event handling code yields (for instance calls Application.DoEvents).
        // Thanks to Matt Stone [MAStone@osmh.on.ca] to pointing this out to me!
        object[] args = {this._recentSender, this._recentEventArgs};
        this._recentSender = this._recentEventArgs = null;

        // Now safely invoke the delegate.
        this._processEventDelegate.DynamicInvoke(args);
      }
      catch(Exception ex)
      {
        Trace.WriteLine(ex.ToString());
      }
      finally
      {
        // Discard the recent event args so the event won't be raised again.
        this._recentEventArgs = null;
        this._recentSender = null;
      }
    }


    [DllImport("user32")]
    private extern static short GetAsyncKeyState(int nVirtKey);
    
    private const int VK_LBUTTON = 0x01;
    private const int VK_RBUTTON = 0x02;
    private int _vkLeftButton;

    private EventArgs _recentEventArgs;
    private object _recentSender;
    private DateTime _recentEventTime;
    private Timer _timer;

    private bool _delayMouseEvents;

    private TimeSpan _delay;
    private Delegate _processEventDelegate;
  }
}
