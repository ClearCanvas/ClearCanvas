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

using ClearCanvas.Common.Specifications;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Associates authority tokens with a viewer-specific action.
	/// </summary>
	/// <remarks>
	/// <para>
	/// All tokens specified will be ANDed with <see cref="AuthorityTokens.ViewerVisible"/>, which is a global token
	/// intended to limit access to all viewer-specific functionality.
	/// </para>
	/// <para>
	/// This attribute sets the action permissibility via the <see cref="Action.SetPermissibility(ISpecification)"/> method.
	/// If multiple authority tokens are supplied in an array to a single instance of the attribute, those tokens will be combined using AND.  If
	/// multiple instances of this attribute are specified, the tokens associated with each instance are combined
	/// using OR logic.  This allows for the possibility of constructing a permission specification based on a complex boolean
	/// combination of authority tokens.
	/// </para>
	/// </remarks>
	public class ViewerActionPermissionAttribute : ActionPermissionAttribute
	{
		/// <summary>
		/// Constructor - the specified authority token will be associated with the specified action ID.
		/// </summary>
		public ViewerActionPermissionAttribute(string actionID, string authorityToken)
            : this(actionID, new string[] { authorityToken })
        {
        }

		/// <summary>
		/// Constructor - all of the specified tokens will combined using AND and associated with the specified action ID.
		/// </summary>
		public ViewerActionPermissionAttribute(string actionID, params string[] authorityTokens)
			: base(actionID, CreateViewerTokens(authorityTokens))
        {
        }

		private static string[] CreateViewerTokens(string[] authorityTokens)
		{
			authorityTokens = authorityTokens ?? new string[0];
			string[] viewerTokens = new string[authorityTokens.Length + 1];
			
			viewerTokens[0] = AuthorityTokens.ViewerVisible;
			
			for (int i = 0; i < authorityTokens.Length; ++i)
				viewerTokens[i + 1] = authorityTokens[i];

			return viewerTokens;
		}
	}
}
