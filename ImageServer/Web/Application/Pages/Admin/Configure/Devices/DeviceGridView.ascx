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
<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.Devices.DeviceGridView"
    CodeBehind="DeviceGridView.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
    Width="100%">
    <asp:TableRow VerticalAlign="top">
        <asp:TableCell VerticalAlign="top">
            <ccUI:GridView ID="GridView1" runat="server" OnRowDataBound="GridView1_RowDataBound"
                OnPageIndexChanging="GridView1_PageIndexChanging">
                <Columns>
                    <asp:BoundField DataField="AETitle" HeaderText="<%$Resources: ColumnHeaders,AETitle %>" HeaderStyle-HorizontalAlign="Left">
                    </asp:BoundField>
                    <asp:BoundField DataField="Description" HeaderText="<%$Resources: ColumnHeaders,DeviceDescription %>" HeaderStyle-HorizontalAlign="Left">
                    </asp:BoundField>
                    <asp:BoundField DataField="DeviceTypeEnum" HeaderText="<%$Resources: ColumnHeaders,DeviceType %>" HeaderStyle-HorizontalAlign="Left">
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders,IPAddress %>" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label ID="IpAddressLabel" runat="server" Text="Label"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Port" HeaderText="<%$Resources: ColumnHeaders,Port %>" HeaderStyle-HorizontalAlign="Center"
                        ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders,Enabled %>">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Enabled") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Image ID="ActiveImage" runat="server" SkinID="Unchecked" />
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders,DHCP %>">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("DHCP") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Image ID="DHCPImage" runat="server" SkinID="Unchecked" />
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders,Partition %>" Visible="False">
                        <ItemTemplate>
                            <asp:Label ID="ServerParitionLabel" runat="server" Text="Label"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders, LastAccessedTime %>">
                        <HeaderStyle Wrap="false" HorizontalAlign="Center" />
                        <ItemStyle Wrap="false" HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:Label ID="LastAccessed" runat="server" Text='<%# DateTimeFormatter.Format((DateTime)Eval("LastAccessedTime")) %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders,Features %>" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:PlaceHolder ID="FeaturePlaceHolder" runat="server"></asp:PlaceHolder>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <ccAsp:EmptySearchResultsMessage ID="EmptySearchResultsMessage1" runat="server" Message="<%$Resources: SR,NoDevicesWereFound %>" />
                </EmptyDataTemplate>
                <RowStyle CssClass="GlobalGridViewRow" />
                <AlternatingRowStyle CssClass="GlobalGridViewRow" />
                <SelectedRowStyle CssClass="GlobalGridViewSelectedRow" />
                <HeaderStyle CssClass="GlobalGridViewHeader" />
                <PagerTemplate>
                </PagerTemplate>
            </ccUI:GridView>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
