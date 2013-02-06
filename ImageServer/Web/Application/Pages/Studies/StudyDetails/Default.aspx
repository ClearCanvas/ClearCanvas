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

<%@ Page Language="C#" AutoEventWireup="true"  MasterPageFile="~/Pages/Common/MainContentSection.Master" Codebehind="Default.aspx.cs" 
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Default" %>

<%@ Register Src="Controls/PatientSummaryPanel.ascx" TagName="PatientSummaryPanel" TagPrefix="localAsp" %>
<%@ Register Src="Controls/StudyDetailsPanel.ascx" TagName="StudyDetailsPanel" TagPrefix="localAsp" %>
<%@ Register Src="Controls/EditStudyDetailsDialog.ascx" TagName="EditStudyDetailsDialog" TagPrefix="localAsp" %>
<%@ Register Src="Controls/StudyDetailsTabs.ascx" TagName="StudyDetailsTabs" TagPrefix="localAsp" %>
<%@ Register Src="Controls/DeleteStudyConfirmDialog.ascx" TagName="DeleteStudyConfirmDialog" TagPrefix="localAsp" %>
<%@ Register Src="Controls/DeleteSeriesConfirmDialog.ascx" TagName="DeleteSeriesConfirmDialog" TagPrefix="localAsp" %>

<asp:Content runat="server" ID="MainMenuContent" contentplaceholderID="MainMenuPlaceHolder">
    <asp:Table ID="Table1" runat="server" Width="100%" ><asp:TableRow><asp:TableCell HorizontalAlign="right" style="padding-top: 12px;"><asp:LinkButton ID="LinkButton1" runat="server" SkinId="CloseButton" Text="<%$Resources: Labels, Close %>" OnClientClick="javascript: window.close(); return false;" /></asp:TableCell></asp:TableRow></asp:Table>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContentSectionPlaceHolder" runat="server">
            <asp:UpdatePanel runat="server" ID="updatepanel" UpdateMode="Conditional">
                <ContentTemplate>
                    <localAsp:StudyDetailsPanel ID="StudyDetailsPanel" runat="server" />
                    </ContentTemplate>
            </asp:UpdatePanel>
</asp:Content>

<asp:Content ContentPlaceHolderID="DialogsSectionPlaceHolder" runat="server">
    <localAsp:EditStudyDetailsDialog ID="EditStudyDialog" runat="server" />
    <ccAsp:MessageBox ID="MessageDialog" runat="server" />
    <ccAsp:MessageBox ID="ReprocessConfirmationDialog" runat="server" />
    <localAsp:DeleteStudyConfirmDialog ID="DeleteStudyConfirmDialog" runat="server"/>
    <localAsp:DeleteSeriesConfirmDialog ID="DeleteSeriesConfirmDialog" runat="server"/>    
</asp:Content>
   