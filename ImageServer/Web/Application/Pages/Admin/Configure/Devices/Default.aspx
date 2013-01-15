<%-- License
// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0
--%>
<%@ Import Namespace="Resources" %>

<%@ Page Language="C#" MasterPageFile="~/GlobalMasterPage.master" AutoEventWireup="true"
    EnableEventValidation="false" CodeBehind="Default.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.Devices.Default" %>

<%@ Register Src="AddEditDeviceDialog.ascx" TagName="AddEditDeviceDialog" TagPrefix="ccAddEdit" %>
<%@ Register Src="DevicePanel.ascx" TagName="DevicePanel" TagPrefix="localAsp" %>

<asp:Content ID="MainMenuContent" ContentPlaceHolderID="MainMenuPlaceHolder" runat="server">
    <asp:SiteMapDataSource ID="MainMenuSiteMapDataSource" runat="server" ShowStartingNode="False" />
    <asp:Menu runat="server" ID="MainMenu" SkinID="MainMenu" DataSourceID="MainMenuSiteMapDataSource"
        Style="font-family: Sans-Serif">
    </asp:Menu>
</asp:Content>

<asp:Content ID="TitleContent" ContentPlaceHolderID="MainContentTitlePlaceHolder" runat="server">
    <asp:Literal ID="Literal1" runat="server" Text="<%$Resources:Titles,Devices%>" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:UpdatePanel ID="PageContent" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <ccAsp:ServerPartitionSelector runat="server" id="ServerPartitionSelector" visible="true" />
            <localAsp:DevicePanel runat="server" id="SearchPanel" visible="true" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<asp:Content ContentPlaceHolderID="DialogsPlaceHolder" runat="server">
    <asp:UpdatePanel ID="DialogUpdatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
	  	    <ccAddEdit:AddEditDeviceDialog ID="AddEditDeviceControl1" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>

    <ccAsp:MessageBox ID="DeleteConfirmation" runat="server" />
    <ccAsp:MessageBox ID="DeleteMessage" runat="server" />
</asp:Content>
