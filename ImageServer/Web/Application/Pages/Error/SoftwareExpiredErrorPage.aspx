<%-- License
// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SoftwareExpiredErrorPage.aspx.cs" MasterPageFile="ErrorPageMaster.Master" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Error.SoftwareExpiredErrorPage" %>
<%@ Import Namespace="Resources"%>
<%@ Import namespace="System.Threading"%>


<asp:Content runat="server" ContentPlaceHolderID="ErrorMessagePlaceHolder">
	    <asp:label ID="ErrorMessageLabel" runat="server">
	        <%= ErrorMessages.MessageSoftwareExpired%>
	    </asp:label>
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="DescriptionPlaceHolder">
   
    
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="UserEscapePlaceHolder">
</asp:Content>