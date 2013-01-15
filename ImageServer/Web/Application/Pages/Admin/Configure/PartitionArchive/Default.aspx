<%--  License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

--%>

<%@ Page Language="C#" MasterPageFile="~/GlobalMasterPage.master" AutoEventWireup="true" ValidateRequest="false"
    EnableEventValidation="false" Codebehind="Default.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive.Default"
    %>

<%@ Register Src="AddEditPartitionDialog.ascx" TagName="AddEditPartitionDialog" TagPrefix="localAsp" %>
<%@ Register Src="PartitionArchivePanel.ascx" TagName="PartitionArchivePanel" TagPrefix="localAsp" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContentTitlePlaceHolder" runat="server"><asp:Literal ID="Literal1" runat="server" Text="<%$Resources:Titles,PartitionArchive%>" /></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:UpdatePanel ID="PageContent" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <ccAsp:ServerPartitionSelector runat="server" id="ServerPartitionSelector" visible="true" />
            <localAsp:PartitionArchivePanel runat="server" id="SearchPanel" visible="true" />
        </ContentTemplate>
    </asp:UpdatePanel>       
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="DialogsPlaceHolder" runat="server">
    <localAsp:AddEditPartitionDialog ID="AddEditPartitionDialog" runat="server" /> 
    <ccAsp:MessageBox ID="DeleteConfirmDialog" runat="server" />       
    <ccAsp:MessageBox ID="MessageBox" runat="server" />     
    <ccAsp:TimedDialog ID="TimedDialog" runat="server" Timeout="3500" />   
</asp:Content>