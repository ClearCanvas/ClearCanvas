<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

--%>

<%@ Page Language="C#" MasterPageFile="~/GlobalMasterPage.master" AutoEventWireup="true"
    EnableEventValidation="false" Codebehind="Default.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerPartitions.Default"
 %>

<%@ Register Src="AddEditPartitionDialog.ascx" TagName="AddEditPartitionDialog" TagPrefix="uc2" %>
<%@ Register Src="ServerPartitionPanel.ascx" TagName="ServerPartitionPanel" TagPrefix="uc1" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContentTitlePlaceHolder" runat="server">
    <asp:Literal ID="Literal1" runat="server" Text="<%$Resources:Titles,ServerPartitions%>" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <uc1:ServerPartitionPanel ID="ServerPartitionPanel" runat="server"></uc1:ServerPartitionPanel>
		    <uc2:AddEditPartitionDialog ID="AddEditPartitionDialog" runat="server" /> 
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="DialogsPlaceHolder" runat="server">

    <ccAsp:TimedDialog ID="TimedDialog" runat="server" Timeout="3500" /> 
    <ccAsp:MessageBox ID="deleteConfirmBox" runat="server" />       
    <ccAsp:MessageBox ID="MessageBox" runat="server" />     
</asp:Content>



