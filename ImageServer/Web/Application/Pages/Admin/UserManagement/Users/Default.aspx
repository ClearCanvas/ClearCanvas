<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

--%>

<%@ Page Language="C#" MasterPageFile="~/GlobalMasterPage.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.Users.Default" %>

<%@ Register Src="UserPanel.ascx" TagName="UserPanel" TagPrefix="localAsp" %>
<%@ Register Src="~/Pages/Admin/UserManagement/Users/AddEditUserDialog.ascx" TagName="AddEditUserDialog" TagPrefix="localAsp" %>

<asp:Content runat="server" ID="MainContentTitle" ContentPlaceHolderID="MainContentTitlePlaceHolder"><asp:Literal ID="Literal1" runat="server" Text="<%$Resources:Titles,UserManagement%>" /></asp:Content>
    
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:Panel runat="server" ID="PageContent">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
               <localAsp:UserPanel runat="server" ID="UserPanel" />              
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="DialogsPlaceHolder" runat="server">
    <localAsp:AddEditUserDialog ID="AddEditUserDialog" runat="server" />
    <ccAsp:MessageBox ID="DeleteConfirmation" runat="server" />   
    <ccAsp:MessageBox ID="PasswordResetConfirmation" runat="server" />   
</asp:Content>