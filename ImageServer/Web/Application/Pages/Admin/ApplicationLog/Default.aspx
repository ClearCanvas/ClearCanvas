<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

--%>

<%@ Import Namespace="Resources" %>
<%@ Page Language="C#" MasterPageFile="~/GlobalMasterPage.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.ApplicationLog.Default" %>

<%@ Register Src="ApplicationLogPanel.ascx" TagName="ApplicationLogPanel" TagPrefix="localAsp" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContentTitlePlaceHolder" runat="server">
<asp:Literal ID="Literal1" runat="server" Text="<%$Resources:Titles,ApplicationLog%>" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
	<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="Panel1" runat="server">
                <localAsp:ApplicationLogPanel ID="ApplicationLogPanel" runat="server"/>
            </asp:Panel>
            
        </ContentTemplate>
      
    </asp:UpdatePanel>

</asp:Content>
