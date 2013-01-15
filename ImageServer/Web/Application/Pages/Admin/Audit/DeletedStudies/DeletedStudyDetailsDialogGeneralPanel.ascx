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

<%@ Import namespace="ClearCanvas.ImageServer.Web.Application.Helpers"%>
<%@ Import namespace="ClearCanvas.ImageServer.Web.Common.Data.Model"%>


<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DeletedStudyDetailsDialogGeneralPanel.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.DeletedStudyDetailsDialogGeneralPanel" %>
<asp:Panel ID="Panel3" runat="server">
    <table width="100%">
        <tr>
            <td>
                <asp:DetailsView ID="StudyDetailView" runat="server" AutoGenerateRows="False" GridLines="Horizontal"
                    CellPadding="4" CssClass="GlobalGridView" Width="100%">
                    <Fields>
                        <asp:TemplateField HeaderText="<%$Resources: DetailedViewFieldLabels, PatientName %>">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                            <ItemTemplate>
                                <ccUI:PersonNameLabel ID="PatientsName" runat="server" PersonName='<%# Eval("PatientsName") %>'
                                    PersonNameType="Dicom"></ccUI:PersonNameLabel>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="PatientId" HeaderText="<%$Resources: DetailedViewFieldLabels, PatientID %>">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                        </asp:BoundField>
                        <asp:BoundField DataField="StudyDescription" HeaderText="<%$Resources: DetailedViewFieldLabels, StudyDescription %>">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                        </asp:BoundField>
                        <asp:BoundField DataField="AccessionNumber" HeaderText="<%$Resources: DetailedViewFieldLabels, AccessionNumber %>">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="<%$Resources: DetailedViewFieldLabels, StudyDateTime %>">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                            <ItemTemplate>
                                <ccUI:DALabel ID="StudyDate" runat="server" Value='<%# Eval("StudyDate") %>'></ccUI:DALabel>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="StudyInstanceUid" HeaderText="<%$Resources: DetailedViewFieldLabels, StudyInstanceUID %>">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                        </asp:BoundField>
                        <asp:BoundField DataField="PartitionAE" HeaderText="<%$Resources: DetailedViewFieldLabels, Partition %>">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                        </asp:BoundField>
                        <asp:BoundField DataField="BackupFolderPath" HeaderText="<%$Resources: DetailedViewFieldLabels, AdminDeletedStudies_BackupLocation %>">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="<%$Resources: DetailedViewFieldLabels, AdminDeletedStudies_ReasonForDeletion %>">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                            <ItemTemplate>
                                <%# HtmlEncoder.EncodeText((Container.DataItem as DeletedStudyInfo).ReasonForDeletion)%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="<%$Resources: DetailedViewFieldLabels, AdminDeletedStudies_DeletionDateTime %> ">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                            <ItemTemplate>
                                <ccUI:DateTimeLabel ID="DeleteDate" runat="server" Value='<%# Eval("DeleteTime") %>'></ccUI:DateTimeLabel>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="<%$Resources: DetailedViewFieldLabels, AdminDeletedStudies_DeletedBy %>">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                            <ItemTemplate>
                                <%# (Container.DataItem as DeletedStudyInfo).UserName ?? SR.Unknown %> 
                                <%# String.IsNullOrEmpty((Container.DataItem as DeletedStudyInfo).UserId) 
                                        ? String.Empty 
                                        : String.Format(" (ID={0})", (Container.DataItem as DeletedStudyInfo).UserId)%>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Fields>
                    <RowStyle CssClass="GlobalGridViewRow" />
                    <AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow" />
                </asp:DetailsView>
            </td>
        </tr>
    </table>
</asp:Panel>
