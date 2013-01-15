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

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AlertIndicator.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Controls.AlertIndicator" %>
<%@ Import Namespace="Resources" %>

<div runat="server" id="AlertLinkPanel" >| <asp:LinkButton ID="AlertLink" runat="server" PostBackUrl="~/Pages/Admin/Alerts/Default.aspx" CssClass="UserInformationLink"><%= Labels.CriticalAlerts %> <asp:Label ID="AlertsCount" runat="server" /></asp:LinkButton>
<div id="AlertDetailsPanel" class="AlertDetailsPanel" style="display: none">
    <div>
        <asp:Table runat="server" ID="AlertTable" style="background: white; border: lightsteelblue 1px solid; padding: 2px;">
            <asp:TableRow CssClass="AlertTableHeaderCell">
            <asp:TableCell><%=ColumnHeaders.AlertComponent %></asp:TableCell>
            <asp:TableCell><%=ColumnHeaders.AlertSource%></asp:TableCell>
            <asp:TableCell><%=ColumnHeaders.AlertDescription %></asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </div>
    <div style="text-align: right; padding: 0px 2px 0px 0px; margin-top: 2px; font-weight: bold;">
    <table>
    <tr>
    <td nowrap="nowrap"><asp:LinkButton ID="LinkButton2" runat="server" PostBackUrl="~/Pages/Admin/Alerts/Default.aspx" style="color: #6699CC; text-decoration: none;"><%=Labels.AlertIndicator_ViewAllAlerts %></asp:LinkButton></td>
    <td> | </td>
    <td><a id="CloseButton" href="" style="color: #6699CC; text-decoration: none;"><%= Labels.Close %></a></td></tr></table></div>
</div>
</div>

<% if(alerts.Count > 0) { %>        
<script type="text/javascript">

    $(document).ready(function() {

        $("#<%=AlertLink.ClientID %>").mouseover(function() {
            $(".AlertDetailsPanel:hidden").show();
        });
        $("#CloseButton").click(function(event) {
            event.preventDefault();
            $("#AlertDetailsPanel:visible").slideUp("fast");
        });
    });
</script>

<%} else { %>

<script type="text/javascript">
        $("#<%=AlertLinkPanel.ClientID %>").hide();
</script>

<% } %>