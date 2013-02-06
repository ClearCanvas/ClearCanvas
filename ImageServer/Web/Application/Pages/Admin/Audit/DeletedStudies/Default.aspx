<%--  License

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

