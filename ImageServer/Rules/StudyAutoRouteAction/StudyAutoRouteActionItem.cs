#region License

// Copyright (c) 2014, ClearCanvas Inc.
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
using System.Linq;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Actions;
using ClearCanvas.Dicom.Utilities.Rules;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Rules.StudyAutoRouteAction
{
    /// <summary>
    /// Class for implementing auto-route action as specified by <see cref="IActionItem{TActionContext}"/>
    /// </summary>
    public class StudyAutoRouteActionItem : ActionItemBase<ServerActionContext>
    {
        readonly private string _device;
    	private readonly DateTime? _startTime;
		private readonly DateTime? _endTime;
	    private readonly QCStatusEnum _qcStatusEnum;
        #region Constructors

        public StudyAutoRouteActionItem(string device)
            : base("Study AutoRoute Action")
        {
            _device = device;
        }

		public StudyAutoRouteActionItem(string device, DateTime startTime, DateTime endTime)
			: base("Study AutoRoute Action")
		{
			_device = device;
			_startTime = startTime;
			_endTime = endTime;
		}

		public StudyAutoRouteActionItem(string device, DateTime startTime, DateTime endTime, QCStatusEnum e)
			: base("Study AutoRoute Action")
		{
			_device = device;
			_startTime = startTime;
			_endTime = endTime;
			_qcStatusEnum = e;
		}


		public StudyAutoRouteActionItem(string device, DateTime startTime, QCStatusEnum e)
			: base("Study AutoRoute Action")
		{
			_device = device;
			_startTime = startTime;
			_qcStatusEnum = e;
		}

		public StudyAutoRouteActionItem(string device, QCStatusEnum e)
			: base("Study AutoRoute Action")
		{
			_device = device;
			_qcStatusEnum = e;
		}

        #endregion

        #region Public Properties

        #endregion

        #region Public Methods

        protected override bool OnExecute(ServerActionContext context)
        {
            InsertStudyAutoRouteCommand command;

			if (_startTime!=null && _endTime!=null)
			{
				DateTime now = Platform.Time;
				TimeSpan nowTimeOfDay = now.TimeOfDay;
				if (_startTime.Value > _endTime.Value)
				{
					if (nowTimeOfDay > _startTime.Value.TimeOfDay
						|| nowTimeOfDay < _endTime.Value.TimeOfDay)
					{
						command = new InsertStudyAutoRouteCommand(context, _device, _qcStatusEnum);		
					}
					else
					{
						DateTime scheduledTime = now.Date.Add(_startTime.Value.TimeOfDay);
						command = new InsertStudyAutoRouteCommand(context, _device, scheduledTime, _qcStatusEnum);
					}
				}
				else
				{
					if (nowTimeOfDay > _startTime.Value.TimeOfDay
						&& nowTimeOfDay < _endTime.Value.TimeOfDay )
					{
						command = new InsertStudyAutoRouteCommand(context, _device, _qcStatusEnum);
					}
					else
					{
						if (nowTimeOfDay < _startTime.Value.TimeOfDay)
						{
							DateTime scheduledTime = now.Date.Add(_startTime.Value.TimeOfDay);
							command = new InsertStudyAutoRouteCommand(context, _device, scheduledTime, _qcStatusEnum);
						}
						else
						{
							DateTime scheduledTime = now.Date.Date.AddDays(1d).Add(_startTime.Value.TimeOfDay);
							command = new InsertStudyAutoRouteCommand(context, _device, scheduledTime, _qcStatusEnum);
						}
					}
				}
			}
			else 	if (_startTime != null && _endTime == null)
			{
				command = new InsertStudyAutoRouteCommand(context, _device, _startTime.Value, _qcStatusEnum);
			}
			else
				command = new InsertStudyAutoRouteCommand(context, _device, _qcStatusEnum);

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
