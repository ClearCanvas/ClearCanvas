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

<%@ Control Language="C#" AutoEventWireup="true" Codebehind="PartitionArchiveGridPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive.PartitionArchiveGridPanel" %>

<asp:UpdatePanel ID="SearchUpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>       
<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
    Width="100%">
    <asp:TableRow VerticalAlign="top">
        <asp:TableCell VerticalAlign="top">             
            <ccUI:GridView ID="PartitionGridView" runat="server" 
                OnRowDataBound="PartitionGridView_RowDataBound"
                SelectionMode="Single">
                <Columns>
                                    <asp:BoundField DataField="Description" HeaderText="<%$Resources: ColumnHeaders, PartitionArchiveDescription %>" HeaderStyle-HorizontalAlign="Left" />
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders,PartitionArchiveType %>">
                        <ItemTemplate>
                             <asp:Label ID="ArchiveType" runat="server" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="ArchiveDelayHours" HeaderText="<%$Resources: ColumnHeaders,PartitionArchiveDelayHours %>">
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders,Enabled %>">
                        <ItemTemplate>
                             <asp:Image ID="EnabledImage" runat="server" SkinId="<%$Image:Unchecked %>" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders,PartitionArchiveReadonly%>">
                        <ItemTemplate>
                             <asp:Image ID="ReadOnlyImage" runat="server" SkinId="<%$Image:Unchecked%>" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders,PartitionArchiveConfigurationXML %>">
                         <ItemTemplate>
                             <asp:Label ID="ConfigurationXML" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                  <ccAsp:EmptySearchResultsMessage ID="EmptySearchResultsMessage1" runat="server" Message="<%$Resources: SR,AdminPartitionArchive_NoArchiveFound %>" />
                </EmptyDataTemplate>
                <RowStyle CssClass="GlobalGridViewRow" />
                <AlternatingRowStyle CssClass="GlobalGridViewRow" />                
                <HeaderStyle CssClass="GlobalGridViewHeader" />
                <SelectedRowStyle CssClass="GlobalGridViewSelectedRow" />
            </ccUI:GridView>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
</ContentTemplate>
</asp:UpdatePanel>