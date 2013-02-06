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

<%@ Page Language="C#" MasterPageFile="~/GlobalMasterPage.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Dashboard.Default" %>
<%@ Import Namespace="Resources"%>



<%@ Register TagPrefix="localAsp" TagName="FileSystemsGridView" Src="~/Pages/Admin/Dashboard/FileSystemsGridView.ascx" %>
<%@ Register TagPrefix="localAsp" TagName="ServerPartitionGridView" Src="~/Pages/Admin/Dashboard/ServerPartitionGridView.ascx" %>
<%@ Register TagPrefix="localAsp" TagName="StudiesSummary" Src="~/Pages/Admin/Dashboard/StudiesSummary.ascx" %>
<%@ Register TagPrefix="localAsp" TagName="WorkQueueSummary" Src="~/Pages/Admin/Dashboard/WorkQueueSummary.ascx" %>
<%@ Register TagPrefix="localAsp" TagName="StudyIntegrityQueueSummary" Src="~/Pages/Admin/Dashboard/StudyIntegrityQueueSummary.ascx" %>
<%@ Register TagPrefix="localAsp" TagName="AlertsGridView" Src="~/Pages/Admin/Dashboard/AlertsGridView.ascx" %>

<asp:Content runat="server" ID="MainContentTitle" ContentPlaceHolderID="MainContentTitlePlaceHolder"><asp:Literal ID="Literal1" runat="server" Text="<%$Resources:Titles,Dashboard%>" /></asp:Content>
    
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<div class="DashboardBody">

<table width="100%" border="0" >
<tr><td valign="top" style="border-right: dashed 1px #cccccc;">

<div class="DashboardItemContainer">
<div class="DashboardItemHeader"><a href="../../Studies/Default.aspx"><%= Labels.Dashboard_Studies%></a></div>
<div class="SidebarInfoContainer">
<localAsp:StudiesSummary runat="server" ID="StudiesSummary" />
</div>
</div>

<div class="DashboardItemContainer">
<div class="DashboardItemHeader"><a href="../../Queues/WorkQueue/Default.aspx"><%= Labels.Dashboard_WorkQueue%></a></div>
<div class="SidebarInfoContainer">
<localAsp:WorkQueueSummary runat="server" ID="WorkQueueSummary" />
</div>
</div>

<div class="DashboardItemContainer">
<div class="DashboardItemHeader"><a href="../../Queues/StudyIntegrityQueue/Default.aspx"><%= Labels.Dashboard_StudyIntegrityQueue%></a></div>
<div class="SidebarInfoContainer" style="padding-top: 4px; padding-left: 4px;">
<localAsp:StudyIntegrityQueueSummary runat="server" ID="StudyIntegrityQueueSummary" />
</div>
</div>
</td>
<td valign="top">
<div class="DashboardItemContainer">
<div class="DashboardItemHeader"><a href="../Configure/FileSystems/Default.aspx"><%= Labels.Dashboard_FileSystems%></a></div>
<div class="DashboardInfoContainer">
<localAsp:FileSystemsGridView runat="server" ID="FileSystemsGridView" />
</div>
</div>


<div class="DashboardItemContainer">
<div class="DashboardItemHeader"><a href="../Configure/ServerPartitions/Default.aspx"><%= Labels.Dashboard_ServerPartitions%></a></div>
<div class="DashboardInfoContainer">
<localAsp:ServerPartitionGridView runat="server" ID="ServerPartitionGridView" />
</div>
</div>

<div class="DashboardItemContainer">
<div class="DashboardItemHeader"><a href="../Alerts/Default.aspx"><%= Labels.Dashboard_Alerts%></a></div>
<div class="DashboardInfoContainer">
<localAsp:AlertsGridView runat="server" ID="AlertsGridView" />
</div>
</div>

</td>
</tr>

</table>

</div>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="DialogsPlaceHolder" runat="server">

</asp:Content>