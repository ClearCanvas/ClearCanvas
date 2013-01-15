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

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudyIntegrityQueueSummary.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Dashboard.StudyIntegrityQueueSummary" %>
<%@ Import Namespace="Resources"%>


<table cellpadding="2" width="100%">
    <tr><td style="color: #205F87;"><%= ColumnHeaders.Dashboard_SIQSummary_Duplicates%></td><td align="right" style="padding-right: 15px;"><asp:LinkButton runat="server" ID="DuplicateLink"><%=Duplicates %></asp:LinkButton></td></tr>
    <tr><td style="color: #205F87;"><%= ColumnHeaders.Dashboard_SIQSummary_InconsistentData%></td><td align="right" style="padding-right: 15px;"><asp:LinkButton runat="server" ID="InconsistentDataLink"><%=InconsistentData %></asp:LinkButton></td></tr>
    <tr><td align="right" style="padding-right: 15px; color: #205F87;"><b><%= ColumnHeaders.Dashboard_SIQSummary_Total%></b></td><td align="right" style="padding-right: 15px;"><b><asp:LinkButton runat="server" ID="TotalLinkButton"><%= Duplicates + InconsistentData %></asp:LinkButton></b></td></tr>
</table>