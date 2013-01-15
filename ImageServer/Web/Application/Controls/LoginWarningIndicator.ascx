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

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LoginWarningIndicator.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Controls.LoginWarningIndicator" %>
<%@ Import Namespace="ClearCanvas.ImageServer.Web.Common.Security" %>


<div runat="server" id="LoginLinkPanel" >| <asp:Label ID="LoginWarningLink" runat="server" CssClass="UserInformationLink"><%= Labels.LoginWarnings %> <asp:Label ID="WarningsCount" runat="server" /></asp:Label>
<div id="LoginDetailsPanel" class="AlertDetailsPanel" style="display: none">
    <div>
        <asp:Table runat="server" ID="WarningTable" style="background: white; border: lightsteelblue 1px solid; padding: 2px;">
            <asp:TableRow CssClass="AlertTableHeaderCell">
            <asp:TableCell><%=ColumnHeaders.LoginWarningComponent %></asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </div>
    <div style="text-align: right; padding: 0px 2px 0px 0px; margin-top: 2px; font-weight: bold;">
        <table>
        <tr>
            <td><a id="CloseButton" href="" style="color: #6699CC; text-decoration: none;"><%= Labels.Close %></a></td>
        </tr>
        </table>
    </div>
</div>
</div>

<% if (SessionManager.Current.User.WarningMessages.Count > 0)
   { %>        
<script type="text/javascript">

    $(document).ready(function () {

        $("#<%=LoginWarningLink.ClientID %>").mouseover(function () {
            var e = $('#LoginDetailsPanel');
            e.show();
        });
        $("#CloseButton").click(function (event) {
            event.preventDefault();
            $("#LoginDetailsPanel:visible").slideUp("fast");
        });
    });
</script>

<%} else { %>

<script type="text/javascript">
    $("#<%=LoginLinkPanel.ClientID %>").hide();
</script>

<% } %>