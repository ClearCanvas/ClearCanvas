<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

--%>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="JavascriptRequired.aspx.cs" MasterPageFile="ErrorPageMaster.Master" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Error.JavascriptRequired" %>
<%@ Import Namespace="Resources"%>


<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="ErrorMessagePlaceHolder">
	    <asp:label ID="Label2" runat="server">
	    <%= ErrorMessages.JavascriptIsDisabled %>
	    </asp:label>

</asp:Content>

<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="DescriptionPlaceHolder">
        <div id="Div1" runat="server" onclick="window.location='../../Default.aspx'">
		    <%= ErrorMessages.JavascriptIsDisabledLongDescription%>
		</div>
</asp:Content>