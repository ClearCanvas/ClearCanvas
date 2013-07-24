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
using System.Collections.Generic;
using ClearCanvas.Common.Specifications;

namespace ClearCanvas.Common.Actions
{
	/// <summary>
	/// A class used to manage and execute a set of <see cref="IActionItem{TActionContext}"/> instances.
	/// </summary>
    public class ActionSet<TActionContext> : IActionSet<TActionContext>
    {
        private readonly IList<IActionItem<TActionContext>> _actionList;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="list">The list of actions in the set.</param>
        public ActionSet(IList<IActionItem<TActionContext>> list)
        {
            _actionList = list;
        }

		/// <summary>
		/// Execute the actions associated with the set.
		/// </summary>
		/// <param name="context">The context used by the <see cref="Action{T}"/> instances in the set.</param>
		/// <returns>A <see cref="TestResult"/> instance telling the result of executing the actions.</returns>
        public TestResult Execute(TActionContext context)
        {
            var resultList = new List<TestResultReason>();

            foreach (var item in _actionList)
            {
                try
                {
                    var tempResult = item.Execute(context);

                    if (!tempResult)
                        resultList.Add(new TestResultReason(item.FailureReason));
                }
                catch (Exception e)
                {
                    resultList.Add(new TestResultReason(e.Message));
                }
            }

            if (resultList.Count == 0)
                return new TestResult(true);

            return new TestResult(false, resultList.ToArray());
        }
    }
}