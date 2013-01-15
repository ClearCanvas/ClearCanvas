<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

--%>
<%@ Import Namespace="Resources" %>

<%@ Page Language="C#" MasterPageFile="~/GlobalMasterPage.master" AutoEventWireup="true"
    EnableEventValidation="false" Codebehind="Default.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.FileSystems.Default"
    %>

<%@ Register Src="AddEditFileSystemDialog.ascx" TagName="AddEditFileSystemDialog"
    TagPrefix="localAsp" %>
<%@ Register Src="FileSystemsPanel.ascx" TagName="FileSystemsPanel" TagPrefix="localAsp" %>

<asp:Content ID="ContentTitle" ContentPlaceHolderID="MainContentTitlePlaceHolder" runat="server"><asp:Literal ID="Literal1" runat="server" Text="<%$Resources:Titles,FileSystems%>" /></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="Panel1" runat="server">
                <localAsp:FileSystemsPanel ID="FileSystemsPanel1" runat="server"></localAsp:FileSystemsPanel>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    
    <localAsp:AddEditFileSystemDialog ID="AddEditFileSystemDialog1" runat="server" />
</asp:Content>

  
