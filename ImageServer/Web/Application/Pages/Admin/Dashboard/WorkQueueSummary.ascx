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

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkQueueSummary.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Dashboard.WorkQueueSummary" %>
<%@ Import Namespace="Resources"%>


<asp:DataList ID="WorkQueueDataList" runat="server" Width="100%" OnItemDataBound="Item_DataBound">
    <HeaderTemplate>
        <tr class="OverviewHeader"><td style="padding-left: 4px;"><%= ColumnHeaders.Dashboard_WorkQueueSummary_Server%></td>
        <td align="center" nowrap="nowrap"><%= ColumnHeaders.Dashboard_WorkQueueSummary_NumberOfItems %></td></tr>
    </HeaderTemplate>
    <ItemTemplate>
        <tr><td style="padding: 2px 2px 2px 4px;" nowrap="nowrap"><asp:LinkButton runat="server" ID="WorkQueueLink"><%#Eval("Server") %></asp:LinkButton></td><td align="center" style="padding: 2px;"><%#Eval("ItemCount") %></td></tr>
    </ItemTemplate>
</asp:DataList>