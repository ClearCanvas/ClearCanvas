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

<%@ Control Language="C#" AutoEventWireup="true" Codebehind="PatientSummaryPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.PatientSummaryPanel" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI.HtmlControls" Assembly="System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" %>

<table width="100%">
        <tr>
            <td colspan="4">
                <ccUI:PersonNameLabel ID="personName" runat="server" PersonNameType="Dicom" CssClass="PatientName" /></td>
        </tr>
        <tr style="font-weight: bold; font-size: 14px;">
            <td>
                <asp:Label ID="Label2" runat="server" Text="<%$Resources: Labels, PatientSummary_ID %>" CssClass="PatientInfo"></asp:Label>
                <asp:Label ID="PatientId" runat="server" Text="PatientId" CssClass="PatientInfo"></asp:Label>
            </td>
            <td>
                <asp:Label ID="Label3" runat="server" Text="<%$Resources: Labels, PatientSummary_DOB %>" CssClass="PatientInfo"></asp:Label>
                <ccUI:DALabel ID="PatientDOB" runat="server" EmptyValueText="<%$Resources: SR, Unknown %>" CssClass="PatientInfo"></ccUI:DALabel>
            </td>
            <td>
                <asp:Label ID="Label4" runat="server" Text="<%$Resources: Labels, PatientSummary_Age %>" CssClass="PatientInfo"></asp:Label>
                <asp:Label ID="PatientAge" runat="server" Text="PatientAge" CssClass="PatientInfo"></asp:Label>
            </td>
            <td>
                <asp:Label ID="Label5" runat="server" Text="<%$Resources: Labels, PatientSummary_Gender %>" CssClass="PatientInfo"></asp:Label>
                <asp:Label ID="PatientSex" runat="server" Text="PatientSex" CssClass="PatientInfo"></asp:Label>
            </td>
            <td>
                <asp:Panel runat="server" ID="QCPanel">
                    <asp:Label ID="QCReportLabel" runat="server" Text="<%$Resources: Labels, PatientSummary_QCReport %>" CssClass="PatientInfo"></asp:Label>
				    <a runat="server" ID="QCReportLink" target="_blank" class="PatientInfo"></a>
                    <asp:Image runat="server" Visible="False" ID="RequiresQCWarningIcon" SkinID="WarningSmall" ToolTip="This study is expected to be QC'ed" />
                </asp:Panel>
            </td>
        </tr>
    </table>
