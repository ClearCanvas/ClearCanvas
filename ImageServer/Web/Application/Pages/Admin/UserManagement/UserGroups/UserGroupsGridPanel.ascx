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

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserGroupsGridPanel.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups.UserGroupsGridPanel" %>

<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0" Width="100%">
    <asp:TableRow VerticalAlign="top">
        <asp:TableCell>
        <asp:ObjectDataSource ID="UserGroupDataSourceObject" runat="server" TypeName="ClearCanvas.ImageServer.Web.Common.Data.DataSource.UserGroupDataSource"
				DataObjectTypeName="ClearCanvas.ImageServer.Web.Common.Data.DataSource.UserGroupRowData" EnablePaging="true"
				SelectMethod="Select" SelectCountMethod="SelectCount" OnObjectCreating="GetUserGroupDataSource"
				OnObjectDisposing="DisposeUserGroupsDataSource"/>
            <ccUI:GridView ID="UserGroupsGridView" runat="server" OnRowDataBound="UserGroupsGridView_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="Name" HeaderText="<%$Resources: ColumnHeaders, AdminUserGroups_GroupName %>" HeaderStyle-HorizontalAlign="Left" >
                        <itemstyle width="200px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Description" HeaderText="<%$Resources: ColumnHeaders, AdminUserGroups_GroupDescription %>" HeaderStyle-HorizontalAlign="Left" >
                        <itemstyle width="275px" />
                    </asp:BoundField>
                     <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders,AdminUserGroups_DataGroup %>">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("DataGroup") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Image ID="DataGroupImage" runat="server" SkinID="Unchecked" />
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders, AdminUserGroups_Tokens %>" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left">
                        <ItemTemplate>
                            <asp:TextBox ID="TokensTextBox" runat="server" TextMode="multiline" rows="3" columns="100" CssClass="TokenTextArea" ></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>                    
                </Columns>
                <EmptyDataTemplate>
                    <ccAsp:EmptySearchResultsMessage ID="EmptySearchResultsMessage" runat="server" Message="<%$Resources: SR, NoUserGroupsFound %>" />
                </EmptyDataTemplate>
                <RowStyle CssClass="GlobalGridViewRow" />
                <AlternatingRowStyle CssClass="GlobalGridViewRow" />
                <HeaderStyle CssClass="GlobalGridViewHeader" />
                <SelectedRowStyle CssClass="GlobalGridViewSelectedRow" />
            </ccUI:GridView>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
