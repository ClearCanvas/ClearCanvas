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
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common.Specifications;

namespace ClearCanvas.Common.Actions
{
	/// <summary>
	/// A class used to manage a set of <see cref="IActionItem{TActionContext}"/> instances.
	/// </summary>
    public class ActionList<TActionContext> : IActionList<TActionContext>
    {
        private readonly IList<IActionItem<TActionContext>> _actionList;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="list">The list of actions in the set.</param>
        public ActionList(IList<IActionItem<TActionContext>> list)
        {
            _actionList = list;
        }

		/// <summary>
		/// Execute all actions in the list, returning an aggregate result.
		/// </summary>
		/// <param name="context">The context used by the <see cref="Action{T}"/> instances in the set.</param>
		/// <returns>A <see cref="TestResult"/> instance telling the result of executing the actions.</returns>
        public ActionExecuteResult Execute(TActionContext context)
        {
            var resultList = new List<string>();

            foreach (var item in _actionList)
            {
                try
                {
                    var tempResult = item.Execute(context);

                    if (tempResult.Fail)
                        resultList.AddRange(tempResult.FailureReasons);
                }
                catch (Exception e)
                {
                    resultList.Add(e.Message);
                }
            }

			return new ActionExecuteResult(resultList.Count == 0, resultList.ToArray());
        }

		public IEnumerator<IActionItem<TActionContext>> GetEnumerator()
		{
			return _actionList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
    }
}