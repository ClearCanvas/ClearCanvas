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

namespace ClearCanvas.Desktop.Actions
{
	/// <summary>
	/// Associates a feature token with an action.
	/// </summary>
	/// <remarks>
	/// This attribute sets a feature identification token with an action, allowing the permissibility of the action
	/// to be controlled by application licensing in addition to the other rules of permissibility such as user
	/// authority tokens.
	/// </remarks>
	public class ActionFeatureAttribute : ActionDecoratorAttribute
	{
		private readonly string _featureToken;

		/// <summary>
		/// Associates a feature token with the specified action ID.
		/// </summary>
		/// <param name="actionId">String identifying the action to which this attribute applies.</param>
		/// <param name="featureToken">Feature identification token to be checked against application licensing.</param>
		public ActionFeatureAttribute(string actionId, string featureToken)
			: base(actionId)
		{
			_featureToken = featureToken;
		}

		/// <summary>
		/// Gets the feature identification token to be checked against application licensing.
		/// </summary>
		public string FeatureToken
		{
			get { return _featureToken; }
		}

		public override void Apply(IActionBuildingContext builder)
		{
			if (!string.IsNullOrEmpty(FeatureToken))
			{
				builder.Action.FeatureSpecification = new FeatureAuthorizationSpecification(FeatureToken);
			}
		}
	}
}