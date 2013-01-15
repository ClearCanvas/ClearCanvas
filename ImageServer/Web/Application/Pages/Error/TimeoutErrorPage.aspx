<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

--%>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TimeoutErrorPage.aspx.cs" MasterPageFile="ErrorPageMaster.Master" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Error.TimeoutErrorPage" %>
<%@ Import Namespace="Resources"%>
<%@ Import namespace="ClearCanvas.ImageServer.Web.Common.Security"%>
<%@ Import namespace="System.Threading"%>


<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ErrorMessagePlaceHolder">
	    <asp:label ID="ErrorMessageLabel" runat="server">
	        <%= ErrorMessages.SessionTimedout%>
	    </asp:label>
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="DescriptionPlaceHolder">

    <asp:Label ID = "DescriptionLabel" runat="server">
            <%= String.Format(ErrorMessages.SessionTimedoutLongDescription,  Math.Round(SessionManager.SessionTimeout.TotalMinutes)) %>
    </asp:Label>
</asp:Content>

<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="UserEscapePlaceHolder">
    <table width="100%" class="UserEscapeTable"><tr><td class="UserEscapeCell" style="width: 50%"><asp:LinkButton ID="LogoutButton" runat="server" CssClass="UserEscapeLink" OnClick="Login_Click"><%= Labels.Login %></asp:LinkButton></td><td style="width: 50%"><a href="javascript:window.close()" class="UserEscapeLink"><%=Labels.Close %></a></td></tr></table>
</asp:Content>