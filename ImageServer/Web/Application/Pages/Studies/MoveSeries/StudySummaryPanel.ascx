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

<%@ Control Language="C#" AutoEventWireup="true" Codebehind="StudySummaryPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.MoveSeries.StudySummaryPanel" %>


    
<table width="100%" class="StudySummary" cellpadding="0" cellspacing="0">
    <tr>
        <td class="StudySummaryRow">
            <table width="100%" cellpadding="0" cellspacing="0" border="0">
                <tr>
                    <td class="StudySummaryHeader">
                        <asp:Label ID="Label1" runat="server" Text="<%$Resources: DetailedViewFieldLabels,AccessionNumber %>" Style="white-space: nowrap"></asp:Label></td>
                    <td>
                        <asp:Label ID="AccessionNumber" runat="server" Text=""></asp:Label></td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td class="StudySummaryRow">
            <table width="100%" cellpadding=02" cellspacing="0" border="0">
                <tr>
                    <td class="StudySummaryHeader">
                        <asp:Label ID="Label2" runat="server" Text="<%$Resources: DetailedViewFieldLabels,StudyDescription %>" Style="white-space: nowrap"></asp:Label></td>
                    <td style="border-bottom: solid 2px #eeeeee">
                        <asp:Label ID="StudyDescription" runat="server" Text="Study Description"></asp:Label></td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td class="StudySummaryRow">
            <table width="100%" cellpadding="0" cellspacing="0" border="0">
                <tr>
                    <td class="StudySummaryHeader">
                        <asp:Label ID="Label3" runat="server" Text="<%$Resources: DetailedViewFieldLabels,StudyDate%>" Style="white-space: nowrap"></asp:Label></td>
                    <td style="border-bottom: solid 2px #eeeeee">
                        <ccUI:DALabel ID="StudyDate" runat="server" InvalidValueText="[Invalid date: {0}]"></ccUI:DALabel></td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>
            <table width="100%" cellpadding="0" cellspacing="0" border="0">
                <tr>
                    <td class="StudySummaryHeader">
                        <asp:Label ID="Label4" runat="server" Text="<%$Resources: DetailedViewFieldLabels,ReferringPhysician%>" Style="white-space: nowrap"></asp:Label></td>
                    <td>
                        <ccUI:PersonNameLabel ID="ReferringPhysician" runat="server" PersonNameType="Dicom"></ccUI:PersonNameLabel></td>
            </table>
        </td>
    </tr>
</table>
