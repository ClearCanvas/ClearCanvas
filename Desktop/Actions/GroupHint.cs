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
namespace ClearCanvas.Desktop.Actions
{
	/// <summary>
	/// The GroupHint is used to determine a reasonably appropriate point in the 
	/// action model to put an action that does not yet exist in the stored model.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The action (call it Left-Hand Action) whose position in the store is to be 
	/// determined is compared with each action in the store (Right-Hand Action).
	/// The comparison of the Left-Hand Action to the Right-Hand Action is given
	/// a score.  The score is based on the GroupHint and the algorithm works 
	/// as follows:
	/// </para>
	/// <para>
	///		LHS										RHS										Score
	///		-----------------------------------------------------------------------------------------
	///	1.	Tools.Image.Manipulation.Zoom			""										1
	///	2.	Tools.Image.Manipulation.Zoom			Tools.Image.Manipulation.Pan			4
	///	3.	Tools.Image.Manipulation.Zoom			DisplaySets								0
	/// 4.  ""										""										1
	/// 5.  ""										DisplaySets								0
    ///	6.	Tools.Image.Manipulation.Pan			Tools.Image.Manipulation.Zoom			-4
    /// </para>
	/// <para>
	/// A brief explanation of the logic:
	/// <list type="bullet">
	/// <item>
	/// For backward compatibility, actions with a non-empty GroupHint, when compared to an 
	/// existing action in the store whose GroupHint="", the score is 1 because it is considered
	/// a better match than 2 actions whose GroupHints are non-empty and are completely different.
	/// </item>
	/// <item>
	/// Actions with GroupHints that have similar components (separated by '.') are given a score
	/// equal to the number of (consecutive) matching components + 1.  The +1 accounts for the fact 
	/// that any number of equal components is a better score than the first example, whose score is 1.
	/// When all components in LHS are a match, but RHS has more components, the score is negative (less).
	/// When all components in RHS are a match, meaning LHS has at least the same number of components,
	/// the score is positive (greater).  Otherwise, the sign of the score is determined by examining the
	/// first non-matching component (same position in LHS and RHS) encountered and performing a string
	/// comparison of the two.  If the component in LHS is "less" than RHS, the return value is negative (less).
	/// </item>
	///  <item>
	/// Actions with completely different components are given an automatic score of zero (0).
	/// Two actions with GroupHints = "" are considered equal, so a score of 1 is given.
	/// </item>
	/// <item>
	/// In the case where an existing action with an empty GroupHint is being matched to a non-empty
	/// GroupHint, the LHS cannot be considered at all similar to RHS and the score is automatically zero (0).
	/// </item>
	/// </list>
	/// </para>
	/// </remarks>
	public class GroupHint
	{
		private const char SEPARATOR = '.';
		private readonly string _hint;
		private readonly string[] _components;

        /// <summary>
        /// Constructor.
        /// </summary>
		public GroupHint(string groupHint)
		{
			if (groupHint == null)
				groupHint = "";

			_hint = groupHint;
			_components = _hint.Split(new char[] { SEPARATOR });
		}

        /// <summary>
        /// Gets the hint path.
        /// </summary>
		public string Hint
		{
			get { return _hint; }
		}

        /// <summary>
        /// Gets an array containing the components of the hint path.
        /// </summary>
		protected string[] Components
		{
			get { return _components; }
		}

        /// <summary>
        /// Performs matching based on the algorithm described in <see cref="GroupHint"/>'s class summary.
        /// </summary>
		public int MatchScore(GroupHint other)
		{
			int i = 0;

			// the group "" is considered the default, and all group hints are considered
			// to be a match for the default group, so the score is 1.
			if (other.Components[0] == "")
				return 1;

			foreach (string otherComponent in other.Components)
			{
			    if (_components.Length <= i)
			    {
                    if (i > 0) //at least one component the same, and "other" has more components.
                        i = -i;

                    break;
			    }

		        if (otherComponent == _components[i])
		        {
		            ++i;
		        }
		        else if (i > 0) //at least one component the same
		        {
		            //Use alphabetical comparison to determine sign.
		            var compare = String.Compare(_components[i], otherComponent, true);
		            if (compare < 0)
		                i = -i;
		            break;
		        }
		        else
		            break;
			}

			// if there were any matching components, the score is increased by 1 (because
			// the 'default' score is 1.  If the 'other component' is not a default component
			// then the score remains at 0, because there are no matches.
			if (i > 0)
				++i;
            else if (i < 0)
                --i;

			return i;
		}

        public override string ToString()
        {
            return _hint;
        }
	}
}
