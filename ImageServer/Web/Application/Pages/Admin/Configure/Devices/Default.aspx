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
