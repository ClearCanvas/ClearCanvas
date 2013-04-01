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
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ErrorPage.aspx.cs" MasterPageFile="ErrorPageMaster.Master" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Error.ErrorPage" %>
<%@ Import Namespace="Resources"%>
<%@ Import namespace="System.Threading"%>


<asp:Content runat="server" ContentPlaceHolderID="ErrorMessagePlaceHolder">
	    <asp:label ID="ErrorMessageLabel" runat="server">
	    </asp:label>
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="DescriptionPlaceHolder">
   <asp:Label ID = "DescriptionLabel" runat="server">
    </asp:Label>
    <div ID="StackTraceMessage" runat="server" Visible="false" onclick="javascript:toggleLayer('StackTrace');" style="margin-top:20px">
                <%= ClearCanvas.ImageServer.Web.Common.Utilities.HtmlUtility.Encode(ErrorMessages.ErrorShowStackTraceMessage)%>
        </div>
    <div id="StackTrace" style="margin-top: 15px" visible="false">
        <asp:TextBox runat="server" ID="StackTraceTextBox" Visible="false" Rows="5" Width="90%" TextMode="MultiLine" ReadOnly="true"></asp:TextBox>
    </div>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="UserEscapePlaceHolder">
    <table width="100%" class="UserEscapeTable"><tr>
    <td class="UserEscapeCell"><a href="javascript:history.back()" class="UserEscapeLink"><%= Labels.Back %></a></td>
    <td class="UserEscapeCell"><asp:LinkButton ID="LogoutButton" runat="server" CssClass="UserEscapeLink" OnClick="Logout_Click"><%= Labels.Logout %></asp:LinkButton></td>
    <td style="width: 30%" align="center"><a href="javascript:window.close()" class="UserEscapeLink"><%= Labels.Close %></a></td></tr></table>
</asp:Content>