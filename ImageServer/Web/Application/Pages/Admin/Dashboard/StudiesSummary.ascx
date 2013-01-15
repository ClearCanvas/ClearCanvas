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

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudiesSummary.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Dashboard.StudiesSummary" %>
<%@ Import Namespace="Resources"%>


<asp:DataList ID="StudiesDataList" runat="server" Width="100%" OnItemDataBound="Item_DataBound">
    <HeaderTemplate>
        <tr class="OverviewHeader"><td style="padding-left: 4px;"><%= ColumnHeaders.Dashboard_StudiesSummary_Partition %></td>
        <td align="center" nowrap="nowrap"><%= ColumnHeaders.Dashboard_StudiesSummary_NumberOfStudies %></td></tr>
    </HeaderTemplate>
    <ItemTemplate>
        <tr><td style="padding: 2px 2px 2px 4px;" nowrap="nowrap"><asp:LinkButton runat="server" ID="PartitionLink"><%#Eval("AETitle") %></asp:LinkButton></td><td align="center" style="padding: 2px;"><%#Eval("StudyCount", "{0:N0}")%></td></tr>
    </ItemTemplate>
    <AlternatingItemStyle CssClass="OverviewAlernateRow" />
</asp:DataList>
<div class="TotalStudiesSummary"><%=Labels.Dashboard_StudiesSummary_TotalStudies %> <asp:Label ID="TotalStudiesLabel" runat="server" Text="100,000,000" CssClass="TotalStudiesCount"/></div>