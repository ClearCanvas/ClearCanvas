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

<%@ Import Namespace="ClearCanvas.Dicom" %>
<%@ Import Namespace="Resources" %>
<%@ Control Language="C#" AutoEventWireup="true" Codebehind="HsmArchiveInfoPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.HsmArchiveInfoPanel" %>
<asp:Panel ID="Panel3" runat="server">
    <table width="100%">
        <tr>
            <td>
                <asp:DetailsView ID="ArchiveInfoView" runat="server" AutoGenerateRows="False" GridLines="Horizontal"
                    CellPadding="4" CssClass="GlobalGridView" Width="100%">
                    <Fields>
                        <asp:TemplateField HeaderText="<%$Resources: DetailedViewFieldLabels, AdminDeletedStudies_Hsm_Archive %>">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                            <ItemTemplate>
                                <asp:Label runat="server" ID="ArchiveType" Text="Hsm Archive" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="<%$Resources: DetailedViewFieldLabels, AdminDeletedStudies_Hsm_DateTime %>">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                            <ItemTemplate>
                                <ccUI:DateTimeLabel ID="ArchiveTime" runat="server" Value='<%# Eval("ArchiveTime") %>'></ccUI:DateTimeLabel>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="<%$Resources: DetailedViewFieldLabels, TransferSyntax %>">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                            <ItemTemplate>
                                <asp:Label ID="TransferSyntaxUid" runat="server" Text='<%# String.Format("{1} ({0})", Eval("TransferSyntaxUid"), TransferSyntax.GetTransferSyntax( (string) Eval("TransferSyntaxUid") ).Name) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="<%$Resources: DetailedViewFieldLabels, AdminDeletedStudies_Hsm_Location %>">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                            <ItemTemplate>
                                <asp:Label ID="TransferSyntaxUid" runat="server" Text='<%# Eval("ArchiveFolderPath" ) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Fields>
                    <RowStyle CssClass="GlobalGridViewRow" />
                    <AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow" />
                    <EmptyDataTemplate>
                        <asp:Panel ID="Panel1" runat="server" CssClass="EmptySearchResultsMessage">
                            <asp:Label runat="server" Text="<%$Resources: SR, AdminDeletedStudies_StudyWasNotArchived %>" />
                        </asp:Panel>
                    </EmptyDataTemplate>
                </asp:DetailsView>
            </td>
        </tr>
    </table>
</asp:Panel>
