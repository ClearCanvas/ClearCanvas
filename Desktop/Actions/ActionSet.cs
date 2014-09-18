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
using System.Linq;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Default implementation of <see cref="IActionSet"/>.
    /// </summary>
    public class ActionSet : IActionSet
    {
        private readonly List<IAction> _actions;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ActionSet()
            : this(null)
        {
        }

        /// <summary>
        /// Constructs an action set containing the specified actions.
        /// </summary>
        public ActionSet(IEnumerable<IAction> actions)
        {
            _actions =  new List<IAction>(actions ?? new IAction[0]);
        }

        #region IActionSet members

        /// <summary>
        /// Returns a subset of this set containing only the elements for which the predicate is true.
        /// </summary>
        public IActionSet Select(Predicate<IAction> predicate)
        {
            return new ActionSet(_actions.Where(a => predicate(a)));
        }

        /// <summary>
        /// Gets the number of actions in the set.
        /// </summary>
        public int Count
        {
            get { return _actions.Count; }
        }

        /// <summary>
        /// Returns a set that corresponds to the union of this set with another set.
        /// </summary>
        public IActionSet Union(IActionSet other)
        {
            //This is done for reasons of efficiency. Populating an array of know size is way faster
            //than populating a list that keeps having to adjust its size.
            var union = new IAction[other.Count + Count];
            _actions.CopyTo(union);
            int index = Count;
            foreach (var action in other)
                union[index++] = action;
            
            return new ActionSet(union);
        }

        #endregion

        #region IEnumerable<IAction> Members

		/// <summary>
		/// Gets an enumerator for the <see cref="IAction"/>s in the set.
		/// </summary>
        public IEnumerator<IAction> GetEnumerator()
        {
            return _actions.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

		/// <summary>
		/// Gets an enumerator for the <see cref="IAction"/>s in the set.
		/// </summary>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _actions.GetEnumerator();
        }

        #endregion

    }
}
