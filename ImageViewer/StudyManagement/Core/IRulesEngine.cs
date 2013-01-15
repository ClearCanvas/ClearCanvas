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
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyManagement.Core
{
	public class RulesEngineExtensionPoint : ExtensionPoint<IRulesEngine>
	{ }

	public interface IRulesEngine
	{
		/// <summary>
		/// Apply the Study level rules to a Study.
		/// </summary>
		/// <param name="study">The study to apply the rules to.</param>
		/// <param name="options"> </param>
		void ApplyStudyRules(StudyEntry study, RulesEngineOptions options);

		/// <summary>
		/// Apply the specified rule to the specified study.
		/// </summary>
		/// <param name="study">The study to apply the rule to</param>
		/// <param name="ruleId"> The rule to apply.</param>
		/// <param name="options"> </param>
		void ApplyStudyRule(StudyEntry study, string ruleId, RulesEngineOptions options);
	}
}
