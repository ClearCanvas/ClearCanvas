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

using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Rules.AutoRouteAction
{
    [ExtensionOf(typeof (SampleRuleExtensionPoint))]
    public class MultiTagAutoRoute : SampleRuleBase
    {
        public MultiTagAutoRoute()
            : base("MultiTagAutoRoute",
                   "Multi-Tag AutoRoute",
                   ServerRuleTypeEnum.AutoRoute,
                   "Sample_AutoRouteMultiTag.xml")
        {
            ApplyTimeList.Add(ServerRuleApplyTimeEnum.SopProcessed);
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.SopEdited);
		}
    }

    [ExtensionOf(typeof (SampleRuleExtensionPoint))]
    public class SimpleAutoRouteSample : SampleRuleBase
    {
        public SimpleAutoRouteSample()
            : base("SimpleAutoRoute",
                   "Simple AutoRoute",
                   ServerRuleTypeEnum.AutoRoute,
                   "Sample_AutoRouteSimple.xml")
        {
            ApplyTimeList.Add(ServerRuleApplyTimeEnum.SopProcessed);
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.SopEdited);
		}
    }

	[ExtensionOf(typeof(SampleRuleExtensionPoint))]
	public class ScheduleAutoRouteSample : SampleRuleBase
	{
		public ScheduleAutoRouteSample()
			: base("ScheduleAutoRoute",
				   "Schedule AutoRoute",
				   ServerRuleTypeEnum.AutoRoute,
				   "Sample_AutoRouteSchedule.xml")
		{
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.SopProcessed);
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.SopEdited);
		}
	}

	[ExtensionOf(typeof(SampleRuleExtensionPoint))]
	public class SourceAeAutoRouteSample : SampleRuleBase
	{
		public SourceAeAutoRouteSample()
			: base("SourceAeAutoRoute",
				   "AutoRoute based on Source AE Title",
				   ServerRuleTypeEnum.AutoRoute,
				   "Sample_AutoRouteSourceAe.xml")
		{
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.SopProcessed);
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.SopEdited);
		}
	}
}