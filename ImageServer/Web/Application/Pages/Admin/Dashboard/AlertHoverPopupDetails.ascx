<%--  License

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

--%>

<%@ Import namespace="ClearCanvas.ImageServer.Core.Validation"%>
<%@ Import namespace="ClearCanvas.ImageServer.Web.Common.Utilities"%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AlertHoverPopupDetails.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Dashboard.AlertHoverPopupDetails" %>
<%@ Import Namespace="Resources"%>

<asp:Panel runat="server" ID="DetailsIndicator" CssClass="MoreInfo">[<%=Labels.AlertHoverPopupDetails_MoreInfo %>]</asp:Panel>
<asp:Panel runat="server" ID="DetailsPanel" CssClass="AlertHoverPopupDetails" style="display:none">
    <asp:PlaceHolder runat="server" ID="DetailsPlaceHolder">
    </asp:PlaceHolder>    
</asp:Panel>				            


<aspAjax:DropShadowExtender runat="server" ID="Shadow" TargetControlID="DetailsPanel" Opacity="0.4" TrackPosition="true">
</aspAjax:DropShadowExtender>

<aspAjax:HoverMenuExtender  runat="server" ID="Details" 
        PopupControlID="DetailsPanel" TargetControlID="DetailsIndicator" PopupPosition="bottom">
</aspAjax:HoverMenuExtender>
