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

<%@ Import Namespace="Resources" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DeletedStudyDetailsDialogPanel.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.DeletedStudyDetailsDialogPanel" %>

<%@ Register Src="DeletedStudyDetailsDialogGeneralPanel.ascx" TagName="GeneralInfoPanel" TagPrefix="localAsp" %>
<%@ Register Src="DeletedStudyArchiveInfoPanel.ascx" TagName="ArchiveInfoPanel" TagPrefix="localAsp" %>
    
<asp:Panel ID="Panel3" runat="server">
    <aspAjax:TabContainer ID="TabContainer" runat="server" ActiveTabIndex="0" CssClass="DialogTabControl">
        <aspAjax:TabPanel ID="StudyInfoTabPanel" runat="server" HeaderText="<%$Resources: Titles, AdminDeleteStudies_DetailsDialog_StudyInfoTabTitle %>" CssClass="DialogTabControl">
            <ContentTemplate>
                <localAsp:GeneralInfoPanel runat="server" ID="GeneralInfoPanel" />
            </ContentTemplate>
        </aspAjax:TabPanel>
        
        <aspAjax:TabPanel ID="ArchiveInfoTabPanel" runat="server" HeaderText="<%$Resources: Titles, AdminDeleteStudies_DetailsDialog_ArchiveInfoTabTitle %>" CssClass="DialogTabControl">
            <ContentTemplate>
             <localAsp:ArchiveInfoPanel runat="server" ID="ArchiveInfoPanel" />
            </ContentTemplate>
        </aspAjax:TabPanel>
    </aspAjax:TabContainer>
    
</asp:Panel>