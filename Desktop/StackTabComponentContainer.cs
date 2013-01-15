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
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Extension point for views onto <see cref="StackTabComponentContainer"/>.
    /// </summary>
    [ExtensionPoint]
	public sealed class StackTabComponentContainerViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

	/// <summary>
	/// An enumeration describing the style of a <see cref="StackTabComponentContainer"/>.
	/// </summary>
    public enum StackStyle
    {
        /// <summary>
        /// Only one stack can be open at the same time.  
        /// </summary>
        /// <remarks>
		/// Each stack can be open/closed by clicking on the title bar itself, which act as a button.
		/// </remarks>
        ShowOneOnly = 0,

        /// <summary>
        /// Multiple stack can be open at the same time.  
        /// </summary>
        /// <remarks>
		/// Each stack can be open/closed by clicking on the Down/Up arrow on the title bar.
		/// </remarks>
        ShowMultiple = 1
    }

    /// <summary>
    /// The <see cref="StackTabComponentContainer"/> hosts <see cref="IApplicationComponent"/>s in a
    /// 'stacked' UI representation.
    /// </summary>
    [AssociateView(typeof(StackTabComponentContainerViewExtensionPoint))]
    public class StackTabComponentContainer : PagedComponentContainer<StackTabPage>
    {
        private readonly StackStyle _stackStyle;
		private readonly bool _openAllTabsInitially;

        /// <summary>
        /// Constructor.
        /// </summary>
		public StackTabComponentContainer(StackStyle stackStyle, bool openAllTabsInitially)
        {
            _stackStyle = stackStyle;
        	_openAllTabsInitially = openAllTabsInitially;
        }

		/// <summary>
		/// Gets the <see cref="StackStyle"/> of the container.
		/// </summary>
        public StackStyle StackStyle
        {
            get { return _stackStyle; }
        }

		/// <summary>
		/// Gets the settings for opening all tabs initially.  This is applicable to StackStyle.ShowMultiple only
		/// </summary>
		public bool OpenAllTabsInitially
		{
			get { return _openAllTabsInitially; }
		}
	}
}
