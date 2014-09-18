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




<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudyStorageView.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.StudyStorageView" %>

<div class="GridViewBorder">
    <asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
        Width="100%">
        <asp:TableRow VerticalAlign="top">
            <asp:TableCell VerticalAlign="top">
                <asp:DetailsView ID="StudyStorageViewControl" runat="server" AutoGenerateRows="False" GridLines="Horizontal" CellPadding="4" OnDataBound="StudyStorageView_DataBound"
                    CssClass="GlobalGridView" Width="100%">
                    <Fields>
                        <asp:TemplateField HeaderText="<%$Resources: DetailedViewFieldLabels, StudyStorage_InsertTime %>">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                            <ItemTemplate>
                                <ccUI:DateTimeLabel ID="InsertTime" runat="server" Value='<%# Eval("InsertTime") %>'></ccUI:DateTimeLabel>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="<%$Resources: DetailedViewFieldLabels, StudyStorage_LastAccessed %>">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                            <ItemTemplate>
                                <ccUI:DateTimeLabel ID="LastAccessedTime" runat="server" Value='<%# Eval("LastAccessedTime") %>'></ccUI:DateTimeLabel>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:CheckBoxField DataField="WriteLock" HeaderText="<%$Resources: DetailedViewFieldLabels, StudyStorage_WriteLock %>">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                        </asp:CheckBoxField>
                        <asp:BoundField DataField="ReadLock" HeaderText="<%$Resources: DetailedViewFieldLabels, StudyStorage_ReadLock %>">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="<%$Resources: DetailedViewFieldLabels, StudyStorage_Status %>">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                            <ItemTemplate>
                                <asp:Label ID="Status" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="<%$Resources: DetailedViewFieldLabels, StudyStorage_QueueStatus %>">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                            <ItemTemplate>
                                <asp:Label ID="QueueState" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="<%$Resources: DetailedViewFieldLabels, StudyStorage_TransferSyntax %>">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                            <ItemTemplate>
                                <asp:Label ID="TransferSyntaxUID" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="<%$Resources: DetailedViewFieldLabels, StudyStorage_Tier %>">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                            <ItemTemplate>
                                <asp:Label ID="Tier" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="<%$Resources: DetailedViewFieldLabels, StudyStorage_StudyFolder%>">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                            <ItemTemplate>
                                <asp:Label ID="StudyFolder" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="<%$Resources: DetailedViewFieldLabels, StudyStorage_StudySize %>">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                            <ItemTemplate>
                                <asp:Label ID="StudySize" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Fields>
                    <RowStyle CssClass="GlobalGridViewRow" />
                    <AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow" />
                    <EmptyDataTemplate>
                        <asp:Table ID="Table1" runat="server" Width="100%" CellPadding="0" CellSpacing="0">
                            <asp:TableRow>
                                <asp:TableCell ColumnSpan="3" Height="50" HorizontalAlign="Center">
                                    <asp:Panel ID="Panel1" runat="server" CssClass="GlobalGridViewEmptyText"><%= SR.StudyDetails_NoStudyStorageForThisStudy %></asp:Panel>
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                    </EmptyDataTemplate>
                </asp:DetailsView>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</div>
