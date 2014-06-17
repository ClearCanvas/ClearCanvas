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
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Rules
{
	public enum ApplicableRuleType
	{
		AutoRoute,
		StudyDelete,
		Tier1Retention,
		OnlineRetention,
		StudyCompress,
		SopCompress,
		DataAccess,
		StudyQualityControl,
		StudyAutoRoute,
	}

	public static class ApplicableRuleTypeExtensions
	{
		public static ServerRuleTypeEnum ToServerRuleTypeEnum(this ApplicableRuleType ruleType)
		{
			switch (ruleType)
			{
				case ApplicableRuleType.AutoRoute:
					return ServerRuleTypeEnum.AutoRoute;
				case ApplicableRuleType.StudyAutoRoute:
					return ServerRuleTypeEnum.StudyAutoRoute;
				case ApplicableRuleType.StudyDelete:
					return ServerRuleTypeEnum.StudyDelete;
				case ApplicableRuleType.Tier1Retention:
					return ServerRuleTypeEnum.Tier1Retention;
				case ApplicableRuleType.OnlineRetention:
					return ServerRuleTypeEnum.OnlineRetention;
				case ApplicableRuleType.StudyCompress:
					return ServerRuleTypeEnum.StudyCompress;
				case ApplicableRuleType.SopCompress:
					return ServerRuleTypeEnum.SopCompress;
				case ApplicableRuleType.DataAccess:
					return ServerRuleTypeEnum.DataAccess;
				case ApplicableRuleType.StudyQualityControl:
					return ServerRuleTypeEnum.StudyQualityControl;
				default:
					throw new ArgumentOutOfRangeException("ruleType");
			}
		}
	}
}
