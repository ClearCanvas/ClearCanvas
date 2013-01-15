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
    EnableEventValidation="false" Codebehind="Default.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.Default"
%>

<%@ Register Src="DeletedStudiesSearchPanel.ascx" TagName="DeletedStudiesSearchPanel" TagPrefix="localAsp" %>
<%@ Register Src="DeletedStudyDetailsDialog.ascx" TagName="DetailsDialog" TagPrefix="localAsp" %>

<asp:Content ID="ContentTitle" ContentPlaceHolderID="MainContentTitlePlaceHolder" runat="server"><asp:Literal ID="Literal1" runat="server" Text="<%$Resources:Titles,DeletedStudies%>" /></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="Panel1" runat="server">
                <localAsp:DeletedStudiesSearchPanel ID="SearchPanel" runat="server"/>
            </asp:Panel>
            
        </ContentTemplate>
      
    </asp:UpdatePanel>
    
      <localAsp:DetailsDialog runat="server" ID="DetailsDialog" />
      <ccAsp:MessageBox runat="server" ID="DeleteConfirmMessageBox" 
                Title="<%$Resources: Titles, AdminDeletedStudies_DeleteConfirmationDialogTitle %>" 
                Message="<%$Resources: SR, AdminDeletedStudies_DeleteConfirmationDialog_AreYouSure %>" 
                MessageType="OKCANCEL" />
</asp:Content>

