<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

--%>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CookiesRequired.aspx.cs" MasterPageFile="ErrorPageMaster.Master" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Error.CookiesRequired" %>
<%@ Import Namespace="Resources"%>


<asp:Content runat="server" ContentPlaceHolderID="ErrorMessagePlaceHolder">
	    <asp:label ID="Label1" runat="server">
	    <%= ErrorMessages.CookiesAreDisabled %>
	    </asp:label>

</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="DescriptionPlaceHolder">
        <div runat="server" onclick="window.location='../../Default.aspx'">
		    <%= ErrorMessages.CookiesAreDisabledLongDescription %>
		</div>
</asp:Content>