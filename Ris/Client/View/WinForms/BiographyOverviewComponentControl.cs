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

using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="BiographyOverviewComponent"/>
    /// </summary>
    public partial class BiographyOverviewComponentControl : ApplicationComponentUserControl
    {
        private readonly BiographyOverviewComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public BiographyOverviewComponentControl(BiographyOverviewComponent component)
            : base(component)
        {
            InitializeComponent();
            _component = component;

            _overviewLayoutPanel.RowStyles[0].Height = _component.BannerHeight; 

            Control banner = (Control)_component.BannerComponentHost.ComponentView.GuiElement;
            banner.Dock = DockStyle.Fill;
            _bannerPanel.Controls.Add(banner);

            Control content = (Control)_component.ContentComponentHost.ComponentView.GuiElement;
            content.Dock = DockStyle.Fill;
            _contentPanel.Controls.Add(content);
        }
    }
}
