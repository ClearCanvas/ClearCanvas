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


<%@ Control Language="C#" AutoEventWireup="true" Codebehind="StudyDetailsPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.StudyDetailsPanel" %>
<%@ Import Namespace="Resources"%>

<%@ Register Src="PatientSummaryPanel.ascx" TagName="PatientSummaryPanel" TagPrefix="localAsp" %>
<%@ Register Src="StudyDetailsTabs.ascx" TagName="StudyDetailsTabs" TagPrefix="localAsp" %>
<%@ Register Src="StudyStateAlertPanel.ascx" TagName="StudyStateAlertPanel" TagPrefix="localAsp" %>



<asp:UpdatePanel ID="SearchUpdatePanel" runat="server" UpdateMode="conditional">
    <ContentTemplate>
            <table width="100%" cellpadding="0" cellspacing="0">
            <tr>
                <td class="MainContentTitle"><%= Titles.StudyDetails%></td><td class="MainContentTitleButtonPanel">
                    <ccUI:ToolbarButton runat="server" ID="EditStudyButton" SkinID="<%$Image:EditStudyButton%>" OnClick="EditStudyButton_Click" />
                    <ccUI:ToolbarButton runat="server" ID="DeleteStudyButton" SkinID="<%$Image:DeleteStudyButton%>" OnClick="DeleteStudyButton_Click" />
                    <ccUI:ToolbarButton runat="server" ID="ReprocessStudyButton" SkinID="<%$Image:ReprocessStudyButton%>" OnClick="ReprocessButton_Click" />
                </td>
            </tr>
            <tr>
                <td class="PatientInfo" colspan="2">
                    <table width="100%" cellpadding="0" cellspacing="0" class="PatientSummaryTable">
                        <tr><td>
                            <localAsp:StudyStateAlertPanel runat="server" ID="StudyStateAlertPanel" />
                        </td></tr>
                        <tr>
                            <td>
                                <localAsp:PatientSummaryPanel ID="PatientSummaryPanel" runat="server" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
                        <tr>
                <td class="Spacer" colspan="2"><asp:Image runat="server" SkinID="Spacer" Height="3px"/></td>
            </tr>
            <tr>
                <td colspan="2">                  
                    <localAsp:StudyDetailsTabs ID="StudyDetailsTabs" runat="server" />
                </td>
            </tr>
            </table>
    </ContentTemplate>
</asp:UpdatePanel>


