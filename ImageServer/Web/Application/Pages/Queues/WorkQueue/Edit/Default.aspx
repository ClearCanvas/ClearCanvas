<%-- License

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

<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Pages/Common/MainContentSection.Master" Codebehind="Default.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.Default" %>

<%@ Register Src="WorkQueueItemDetailsPanel.ascx" TagName="WorkQueueItemDetailsPanel"
    TagPrefix="localAsp" %>
<%@ Register Src="ScheduleWorkQueueDialog.ascx" TagName="ScheduleWorkQueueDialog"
    TagPrefix="localAsp" %>
<%@ Register Src="ResetWorkQueueDialog.ascx" TagName="ResetWorkQueueDialog" TagPrefix="localAsp" %>
<%@ Register Src="DeleteWorkQueueDialog.ascx" TagName="DeleteWorkQueueDialog" TagPrefix="localAsp" %>

<asp:Content runat="server" ID="MainMenuContent" contentplaceholderID="MainMenuPlaceHolder">
    <asp:Table ID="Table1" runat="server">
        <asp:TableRow>
        <asp:TableCell HorizontalAlign="right" style="padding-top: 12px;">
            <asp:LinkButton ID="LinkButton1" runat="server" SkinId="CloseButton" Text="<%$Resources: Labels,Close %>" OnClientClick="javascript: window.close(); return false;" />
        </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="MainContentSectionPlaceHolder">
            <asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <localAsp:WorkQueueItemDetailsPanel ID="WorkQueueItemDetailsPanel" runat="server" AutoRefresh="true" />
                    <localAsp:ScheduleWorkQueueDialog ID="ScheduleWorkQueueDialog" runat="server" />
                    <localAsp:ResetWorkQueueDialog ID="ResetWorkQueueDialog" runat="server" />
                    <localAsp:DeleteWorkQueueDialog ID="DeleteWorkQueueDialog" runat="server" EnableViewState="true" />
                    <ccAsp:MessageBox runat="server" ID="MessageBox" MessageType="INFORMATION" />
                    <div style="text-align:center;">
                        <asp:Label ID="Message" runat="server" Text="Label"></asp:Label>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
 </asp:Content>