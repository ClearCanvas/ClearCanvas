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

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DataAccessGroupPanel.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerPartitions.DataAccessGroupPanel" %>
<%@ Import Namespace="Resources" %>
<%@ Import Namespace="ClearCanvas.ImageServer.Model" %>

<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
</asp:ScriptManagerProxy>

<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="Panel1" runat="server" CssClass="ServerPartitionDialogTabDescription">
            <%= SR.AdminPartition_DataAccess_PleaseSelect %>
        </asp:Panel>
        <asp:Panel runat="server" CssClass="DataAccessGroupContainer">
            <asp:CheckBoxList ID="DataAccessGroupCheckBoxList" runat="server" CssClass="DataAccessGroupCheckBoxList" RepeatColumns="1" />
        </asp:Panel>
        <asp:Panel ID="Legends" runat="server" CssClass="ServerPartitionDialogTabDescription" style="text-align:center">
            <span class="GlocalSeeNotesMarker"/>*</span> <%= SR.AdminPartition_DataAccess_WarningAllStudiesAccess%>
        </asp:Panel>
        
    </ContentTemplate>
</asp:UpdatePanel>