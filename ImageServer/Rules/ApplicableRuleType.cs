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
	}

	public static class ApplicableRuleTypeExtensions
	{
		public static ServerRuleTypeEnum ToServerRuleTypeEnum(this ApplicableRuleType ruleType)
		{
			switch (ruleType)
			{
				case ApplicableRuleType.AutoRoute:
					return ServerRuleTypeEnum.AutoRoute;
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
				default:
					throw new ArgumentOutOfRangeException("ruleType");
			}
		}
	}
}
