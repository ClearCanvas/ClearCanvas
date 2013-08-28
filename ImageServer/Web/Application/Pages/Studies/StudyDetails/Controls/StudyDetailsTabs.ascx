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

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudyDetailsTabs.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.StudyDetailsTabs" %>
<%@ Register Src="StudyDetailsView.ascx" TagName="StudyDetailsView" TagPrefix="localAsp" %>
<%@ Register Src="SeriesGridView.ascx" TagName="SeriesGridView" TagPrefix="localAsp" %>
<%@ Register Src="WorkQueueGridView.ascx" TagName="WorkQueueGridView" TagPrefix="localAsp" %>
<%@ Register Src="FileSystemQueueGridView.ascx" TagName="FileSystemQueueGridView" TagPrefix="localAsp" %>
<%@ Register Src="StudyStorageView.ascx" TagName="StudyStorageView" TagPrefix="localAsp" %>
<%@ Register Src="ArchivePanel.ascx" TagName="ArchivePanel" TagPrefix="localAsp" %>
<%@ Register Src="HistoryPanel.ascx" TagName="HistoryPanel" TagPrefix="localAsp" %>
<%@ Register Src="StudyIntegrityQueueGridView.ascx" TagName="StudyIntegrityQueueGridView" TagPrefix="localAsp" %>
<%@ Register Src="UpdateAuthorityGroupDialog.ascx" TagName="UpdateAuthorityGroupDialog" TagPrefix="localAsp" %>

<aspAjax:TabContainer ID="StudyDetailsTabContainer" runat="server" ActiveTabIndex="0"
    CssClass="TabControl" Width="100%">
    <aspAjax:TabPanel ID="StudyDetailsTab" HeaderText="<%$Resources: Titles, StudyDetails %>"
        runat="server">
        <ContentTemplate>
            <asp:Table runat="server">
                <asp:TableRow>
                    <asp:TableCell>
                        <table width="100%" cellpadding="8" cellspacing="0" class="StudyDetailsTabContent">
                            <tr>
                                <td>
                                    <localAsp:StudyDetailsView ID="StudyDetailsView" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </ContentTemplate>
    </aspAjax:TabPanel>
    <aspAjax:TabPanel ID="SeriesDetailsTab" HeaderText="<%$Resources: Titles, SeriesDetails %>"
        runat="server">
        <ContentTemplate>
            <asp:Table runat="server">
                <asp:TableRow>
                    <asp:TableCell>
                        <table width="100%" cellpadding="4" cellspacing="0"  class="StudyDetailsTabContent">
                            <tr>
                                <td>
                                    <div style="padding-top: 5px; padding-left: 1px;" />
                                    <ccUI:ToolbarButton runat="server" ID="ViewSeriesButton" SkinID="<%$Image:ViewSeriesButton%>" />&nbsp;
                                    <ccUI:ToolbarButton runat="server" ID="MoveSeriesButton" SkinID="<%$Image:MoveSeriesButton%>" />&nbsp;
                                    <ccUI:ToolbarButton runat="server" ID="DeleteSeriesButton" SkinID="<%$Image:DeleteSeriesButton%>"
                                        OnClick="DeleteSeriesButton_Click" />
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <localAsp:SeriesGridView ID="SeriesGridView" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </ContentTemplate>
    </aspAjax:TabPanel>
    <aspAjax:TabPanel ID="WorkQueueTab" HeaderText="<%$Resources: Titles, WorkQueue %>"
        runat="server">
        <HeaderTemplate>
            <asp:Label ID="WorkQueueTabTitle" runat="server" Text="<%$Resources: Titles, WorkQueue %>"></asp:Label>
            <asp:Image runat="server" Visible='<%# Study.RequiresWorkQueueAttention %>' ImageAlign="AbsBottom"
                ID="StuckIcon" SkinID="WarningSmall" />
        </HeaderTemplate>
        <ContentTemplate>
            <table width="100%" cellpadding="8" cellspacing="0"  class="StudyDetailsTabContent">
                <tr>
                    <td>
                        <localAsp:WorkQueueGridView ID="WorkQueueGridView" runat="server" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </aspAjax:TabPanel>
    <aspAjax:TabPanel ID="TabPanel4" HeaderText="<%$Resources: Titles, StudyIntegrityQueue %>"
        runat="server">
        <ContentTemplate>
            <table width="100%" cellpadding="8" cellspacing="0"  class="StudyDetailsTabContent">
                <tr>
                    <td>
                        <localAsp:StudyIntegrityQueueGridView ID="StudyIntegrityQueueGridView" runat="server" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </aspAjax:TabPanel>
    <aspAjax:TabPanel ID="FileSystemQueueTab" HeaderText="<%$Resources: Titles, FileSystemQueue %>"
        runat="server">
        <ContentTemplate>
            <table width="100%" cellpadding="8" cellspacing="0"  class="StudyDetailsTabContent">
                <tr>
                    <td>
                        <localAsp:FileSystemQueueGridView ID="FSQueueGridView" runat="server" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </aspAjax:TabPanel>
    <aspAjax:TabPanel ID="TabPanel1" HeaderText="<%$Resources: Titles, StudyStorage %>"
        runat="server">
        <ContentTemplate>
            <table width="100%" cellpadding="8" cellspacing="0" class="StudyDetailsTabContent">
                <tr>
                    <td>
                        <localAsp:StudyStorageView ID="StudyStorageView" runat="server" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </aspAjax:TabPanel>
    <aspAjax:TabPanel ID="TabPanel2" HeaderText="<%$Resources: Titles, Archive %>" runat="server">
        <ContentTemplate>
            <table width="100%" cellpadding="8" cellspacing="0"  class="StudyDetailsTabContent">
                <tr>
                    <td>
                        <localAsp:ArchivePanel ID="ArchivePanel" runat="server" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </aspAjax:TabPanel>
    <aspAjax:TabPanel ID="TabPanel3" HeaderText="<%$Resources: Titles, StudyHistory %>"
        runat="server">
        <ContentTemplate>
            <table width="100%" cellpadding="8" cellspacing="0"  class="StudyDetailsTabContent">
                <tr>
                    <td>
                        <localAsp:HistoryPanel ID="HistoryPanel" runat="server" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </aspAjax:TabPanel>
</aspAjax:TabContainer>
<ccAsp:MessageBox ID="DeleteConfirmation" runat="server" Title="Delete Series Confirmation" />
<localAsp:UpdateAuthorityGroupDialog ID="UpdateAuthorityGroupDialog" runat="server" />