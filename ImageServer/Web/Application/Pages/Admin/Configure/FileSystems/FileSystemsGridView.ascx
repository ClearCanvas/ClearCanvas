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
<%@ Import Namespace="Resources" %>

<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.FileSystems.FileSystemsGridView"
    Codebehind="FileSystemsGridView.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
    Width="100%">
    <asp:TableRow VerticalAlign="top">
        <asp:TableCell VerticalAlign="top">
            <ccUI:GridView ID="GridView1" runat="server" OnRowDataBound="GridView1_RowDataBound" 
                EmptyDataText="No filesystems found (Please check the filters!)"
                PageSize="20">
                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <Columns>
                    <asp:BoundField DataField="Description" HeaderText="<%$Resources: ColumnHeaders,FilesystemDescription %>" HeaderStyle-HorizontalAlign="Left"></asp:BoundField>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders,FilesystemReadPermission %>">
                        <ItemTemplate>
                            <asp:Image ID="ReadImage" runat="server" SkinID="Checked" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders,FilesystemWritePermission %>">
                        <ItemTemplate>
                            <asp:Image ID="WriteImage" runat="server" SkinID="Unchecked" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders,FilesystemTier %>" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label ID="FilesystemTierDescription" runat="server"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders,FilesystemPath %>" HeaderStyle-HorizontalAlign="Left">
                        <ItemTemplate>
                            <asp:Label ID="PathLabel" runat="server" Text='<%# Bind("FileSystemPath") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="HighWatermark" HeaderText="<%$Resources: ColumnHeaders,FilesystemHighWatermark %>" Visible="False">
                        <HeaderStyle Wrap="False" />
                    </asp:BoundField>
                    <asp:BoundField DataField="LowWatermark" HeaderText="<%$Resources: ColumnHeaders,FilesystemLowWatermark %>" Visible="False">
                        <HeaderStyle Wrap="False" />
                    </asp:BoundField>
                    <asp:BoundField DataField="PercentFull" HeaderText="<%$Resources: ColumnHeaders,FilesystemUsagePercentage %>" Visible="False">
                        <HeaderStyle Wrap="False" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders,FilesystemDiskUsage %>">
                        <ItemTemplate>
                            <asp:Image ID="UsageImage" runat="server" ImageAlign="AbsBottom" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle HorizontalAlign="Center" Wrap="False" />
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                   <ccAsp:EmptySearchResultsMessage ID="EmptySearchResultsMessage" runat="server" Message="<%$Resources: SR,AdminFilesystem_NoFilesystemFound %>" />
                </EmptyDataTemplate>
                <RowStyle CssClass="GlobalGridViewRow" />
                <AlternatingRowStyle CssClass="GlobalGridViewRow" />                
                <SelectedRowStyle CssClass="GlobalGridViewSelectedRow" />
                <HeaderStyle CssClass="GlobalGridViewHeader" />
            </ccUI:GridView>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
