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
using ClearCanvas.Common;
using ClearCanvas.Common.Actions;
using ClearCanvas.Dicom.Utilities.Rules;

namespace ClearCanvas.ImageServer.Rules.AutoRouteAction
{
    /// <summary>
    /// Class for implementing auto-route action as specified by <see cref="IActionItem{T}"/>
    /// </summary>
    public class AutoRouteActionItem : ActionItemBase<ServerActionContext>
    {
        readonly private string _device;
    	private readonly DateTime? _startTime;
		private readonly DateTime? _endTime;
        #region Constructors

        public AutoRouteActionItem(string device)
            : base("AutoRoute Action")
        {
            _device = device;
        }

		public AutoRouteActionItem(string device, DateTime startTime, DateTime endTime)
			: base("AutoRoute Action")
		{
			_device = device;
			_startTime = startTime;
			_endTime = endTime;
		}

        #endregion

        #region Public Properties

        #endregion

        #region Public Methods

        protected override bool OnExecute(ServerActionContext context)
        {
            InsertAutoRouteCommand command;

			if (_startTime!=null && _endTime!=null)
			{
				DateTime now = Platform.Time;
				TimeSpan nowTimeOfDay = now.TimeOfDay;
				if (_startTime.Value > _endTime.Value)
				{
					if (nowTimeOfDay > _startTime.Value.TimeOfDay
						|| nowTimeOfDay < _endTime.Value.TimeOfDay)
					{
						command = new InsertAutoRouteCommand(context, _device);		
					}
					else
					{
						DateTime scheduledTime = now.Date.Add(_startTime.Value.TimeOfDay);
						command = new InsertAutoRouteCommand(context, _device, scheduledTime);
					}
				}
				else
				{
					if (nowTimeOfDay > _startTime.Value.TimeOfDay
						&& nowTimeOfDay < _endTime.Value.TimeOfDay )
					{
						command = new InsertAutoRouteCommand(context, _device);
					}
					else
					{
						if (nowTimeOfDay < _startTime.Value.TimeOfDay)
						{
							DateTime scheduledTime = now.Date.Add(_startTime.Value.TimeOfDay);
							command = new InsertAutoRouteCommand(context, _device, scheduledTime);
						}
						else
						{
							DateTime scheduledTime = now.Date.Date.AddDays(1d).Add(_startTime.Value.TimeOfDay);
							command = new InsertAutoRouteCommand(context, _device, scheduledTime);
						}
					}
				}				
			}
			else
				command = new InsertAutoRouteCommand(context, _device);

            if (context.CommandProcessor != null)
                context.CommandProcessor.AddCommand(command);
            else
            {
                try
                {
                    command.Execute(context.CommandProcessor);
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Error, e, "Unexpected exception when inserting auto-route request");

                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}