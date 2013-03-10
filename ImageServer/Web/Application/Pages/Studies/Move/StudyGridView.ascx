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

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudyGridView.ascx.cs" 
Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.Move.StudyGridView" %>

            <asp:GridView ID="StudyListControl" runat="server" skinid="GlobalGridView">
                <Columns>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders, PatientName%>">
                        <itemtemplate>
                            <ccUI:PersonNameLabel ID="PatientName" runat="server" PersonName='<%# Eval("PatientsName") %>' PersonNameType="Dicom"></ccUI:PersonNameLabel>
                        </itemtemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="PatientId" HeaderText="<%$Resources: ColumnHeaders, PatientID%>"></asp:BoundField>
                    <asp:BoundField DataField="AccessionNumber" HeaderText="<%$Resources: ColumnHeaders,AccessionNumber %>" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders, StudyDate%>">
                        <itemtemplate>
                            <ccUI:DALabel ID="StudyDate" runat="server" Value='<%# Eval("StudyDate") %>' HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"></ccUI:DALabel>
                        </itemtemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="StudyDescription" HeaderText="<%$Resources: ColumnHeaders, StudyDescription%>"></asp:BoundField>
                                    </Columns>
                <EmptyDataTemplate>
                    <ccAsp:EmptySearchResultsMessage ID="EmptySearchResultsMessage1" runat="server" Message="<%$Resources: SR, NoStudiesWereFound %>" />
                </EmptyDataTemplate>
                        <RowStyle CssClass="GlobalGridViewRow"/>
                        <HeaderStyle CssClass="GlobalGridViewHeader"/>
                        <AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow" />
                        <SelectedRowStyle  CssClass="GlobalGridSelectedRow" />
                <PagerTemplate>
                </PagerTemplate>
             </asp:GridView>

