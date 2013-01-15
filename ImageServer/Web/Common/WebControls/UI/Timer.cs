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
using System.ComponentModel;
using System.Web.UI;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageServer.Web.Common.WebControls.UI
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:Timer runat=server></{0}:Timer>")]
    public class Timer : System.Web.UI.Timer
    {
        [Bindable(true)]
        [Category("Behaviour")]
        [DefaultValue("")]
        [Localizable(true)]
        public int DisableAfter
        {
            get
            {
                if (ViewState["DisableAfter"] != null)
                {
                    return (int)ViewState["DisableAfter"];
                }
                else
                    return -1;
            }

            set
            {
                ViewState["DisableAfter"] = value;
            }
        }

        public event EventHandler<TimerEventArgs> AutoDisabled;


        private int TickCounter
        {
            get
            {
                if (ViewState["TickCounter"] != null)
                {
                    return (int)ViewState["TickCounter"];
                }
                else
                    return 0;
            }

            set
            {
                ViewState["TickCounter"] = value;
            }
        }

        public void Reset(bool enabled)
        {
            TickCounter = 0;
            Enabled = enabled;
        }

        protected override void OnTick(EventArgs e)
        {
            TickCounter++;
            if (DisableAfter > 0 && TickCounter >= DisableAfter)
            {
                Enabled = false; 
                EventsHelper.Fire(AutoDisabled, this, new TimerEventArgs());
            }
        
            base.OnTick(e);
        }

    }

    public class TimerEventArgs:EventArgs
    {
    }
}