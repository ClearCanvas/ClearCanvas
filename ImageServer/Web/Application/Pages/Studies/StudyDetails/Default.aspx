<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
   