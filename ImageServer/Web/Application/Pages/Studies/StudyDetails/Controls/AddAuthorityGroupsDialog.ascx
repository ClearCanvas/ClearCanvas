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
<%@ Import Namespace="ClearCanvas.ImageServer.Web.Common.Utilities" %>
<%@ Import Namespace="ClearCanvas.ImageServer.Web.Application.Helpers" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddAuthorityGroupsDialog.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.AddAuthorityGroupsDialog" %>
<%@ Import Namespace="Resources" %>
<ccAsp:ModalDialog ID="ModalDialog" runat="server" Title="<%$ Resources:Titles, AddAuthorityGroupsDialogTitle %>"
    Width="800px">
    <ContentTemplate>
        <div class="ContentPanel">
            <div class="DialogPanelContent">
                <table border="0" cellspacing="5" width="100%">
                    <tr>
                        <td>
                            <table width="100%">
                                <tr>
                                    <td>
                                        <asp:Label runat="server" CssClass="DialogTextBoxLabel" Text="<%$ Resources:Labels, AddAuthorityGroupsDialog_StudyListingLabel %>"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <div class="DeleteStudiesTableContainer" style="background: white">
                                            <asp:Repeater runat="server" ID="StudyListing">
                                                <HeaderTemplate>
                                                    <table cellspacing="0" width="100%" class="DeleteStudiesConfirmTable">
                                                        <tr>
                                                            <th style="white-space: nowrap" class="GlobalGridViewHeader">
                                                                <%= ColumnHeaders.PatientName %>
                                                            </th>
                                                            <th style="white-space: nowrap" class="GlobalGridViewHeader">
                                                                <%= ColumnHeaders.PatientID%>
                                                            </th>
                                                            <th style="white-space: nowrap" class="GlobalGridViewHeader">
                                                                <%= ColumnHeaders.StudyDate%>
                                                            </th>
                                                            <th style="white-space: nowrap" class="GlobalGridViewHeader">
                                                                <%= ColumnHeaders.StudyDescription%>
                                                            </th>
                                                            <th style="white-space: nowrap" class="GlobalGridViewHeader">
                                                                <%= ColumnHeaders.AccessionNumber%>
                                                            </th>
                                                        </tr>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <tr class="GlobalGridViewRow">
                                                        <td>
                                                            <%# HtmlUtility.GetEvalValue(Container.DataItem, "PatientsName", "&nbsp;")%>
                                                        </td>
                                                        <td>
                                                            <%# HtmlUtility.GetEvalValue(Container.DataItem, "PatientId", "&nbsp;")%>
                                                        </td>
                                                        <td>
                                                            <%# HtmlUtility.GetEvalValue(Container.DataItem, "StudyDate", "&nbsp;")%>
                                                        </td>
                                                        <td>
                                                            <%# HtmlUtility.GetEvalValue(Container.DataItem, "StudyDescription", "&nbsp;")%>
                                                        </td>
                                                        <td>
                                                            <%# HtmlUtility.GetEvalValue(Container.DataItem, "AccessionNumber", "&nbsp;")%>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    </table>
                                                </FooterTemplate>
                                            </asp:Repeater>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label runat="server" CssClass="DialogTextBoxLabel" Text="<%$ Resources:Labels, AddAuthorityGroupsDialog_AuthorityGroupsLabel %>"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <div class="DeleteStudiesTableContainer">
                                            <asp:CheckBoxList ID="AuthorityGroupCheckBoxList" runat="server" TextAlign="Right"
                                                Width="100%" RepeatColumns="1">
                                            </asp:CheckBoxList>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
            <table cellpadding="0" cellspacing="0" width="100%">
                <tr align="right">
                    <td>
                        <asp:Panel ID="Panel1" runat="server" CssClass="DefaultModalDialogButtonPanel">
                            <ccUI:ToolbarButton ID="AddButton" runat="server" SkinID="<%$Image:OKButton%>" OnClick="AddButton_Clicked" />
                            <ccUI:ToolbarButton ID="CancelButton" runat="server" SkinID="<%$Image:CancelButton%>"
                                OnClick="CancelButton_Clicked" />
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </div>
    </ContentTemplate>
</ccAsp:ModalDialog>
