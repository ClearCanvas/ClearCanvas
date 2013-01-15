<%--  License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

--%>

<%@ Page Language="C#" MasterPageFile="~/GlobalMasterPage.master" AutoEventWireup="true"
    Codebehind="Default.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.DataRules.Default"
    ValidateRequest="false" %>

<%@ Register Src="AddEditDataRuleDialog.ascx" TagName="AddEditDataRuleDialog" TagPrefix="localAsp" %>
<%@ Register Src="DataRulePanel.ascx" TagName="SearchPanel" TagPrefix="localAsp" %>

<asp:Content ID="ContentTitle" ContentPlaceHolderID="MainContentTitlePlaceHolder" runat="server"><asp:Literal ID="Literal1" runat="server" Text="<%$Resources:Titles,DataRules%>" /></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:UpdatePanel ID="PageContent" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <ccAsp:ServerPartitionSelector runat="server" id="ServerPartitionSelector" visible="true" />
            <localAsp:SearchPanel runat="server" id="SearchPanel" visible="true" />
        </ContentTemplate>
    </asp:UpdatePanel>
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="DialogsPlaceHolder" runat="server">
    <ccAsp:MessageBox ID="ConfirmDialog" runat="server" />
    <localAsp:AddEditDataRuleDialog ID="AddEditDataRuleControl" runat="server" />
</asp:Content>


