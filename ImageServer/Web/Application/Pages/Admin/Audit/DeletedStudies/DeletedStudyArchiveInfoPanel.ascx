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

<%@ Import namespace="ClearCanvas.Dicom"%>
<%@ Import Namespace="Resources" %>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DeletedStudyArchiveInfoPanel.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.DeletedStudyArchiveInfoPanel" %>


<asp:Panel ID="Panel3" runat="server">
    <asp:Panel ID="NoArchiveMessagePanel" runat="server" CssClass="EmptySearchResultsMessage">
        <asp:Label ID="Label1" runat="server"  Text = "This study was not archived." />
    </asp:Panel>
    
    <asp:PlaceHolder runat="server" ID="ArchiveViewPlaceHolder">
    <div class="AdditionalInfoSectionHeader" style="margin-top:10px;margin-bottom:5px;">Latest Archive:</div>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="AdditionalArchivePlaceHolder">
    <div class="AdditionalInfoSectionHeader" style="margin-top:10px;margin-bottom:0px;">Other version(s):</div>
    <p></p>
    </asp:PlaceHolder>
    
</asp:Panel>