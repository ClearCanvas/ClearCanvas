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

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserGridPanel.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.Users.UserGridPanel" %>

<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0" Width="100%">
    <asp:TableRow VerticalAlign="top">
        <asp:TableCell>
        <asp:ObjectDataSource ID="UserDataSourceObject" runat="server" TypeName="ClearCanvas.ImageServer.Web.Common.Data.DataSource.UserDataSource"
				DataObjectTypeName="ClearCanvas.ImageServer.Web.Common.Data.DataSource.UserRowData" EnablePaging="true"
				SelectMethod="Select" SelectCountMethod="SelectCount" OnObjectCreating="GetUserDataSource"
				OnObjectDisposing="DisposeUserDataSource"/>
            <ccUI:GridView ID="UserGridView" runat="server" OnRowDataBound="UserGridView_RowDataBound"
                OnSelectedIndexChanged="UserGridView_SelectedIndexChanged" SelectionMode="Single">
                <Columns>
                    <asp:BoundField DataField="UserName" HeaderText="<%$Resources: ColumnHeaders, UserID %>" HeaderStyle-HorizontalAlign="Left" >
                        <itemstyle width="200px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="DisplayName" HeaderText="<%$Resources: ColumnHeaders, UserDisplayName %>" HeaderStyle-HorizontalAlign="Left" />
                    <asp:BoundField DataField="EmailAddress" HeaderText="<%$Resources: ColumnHeaders, UserEmailAddress %>" HeaderStyle-HorizontalAlign="Left" />
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders, UserGroups %>" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:TextBox ID="UserGroupTextBox" runat="server" TextMode="multiline" rows="2" columns="35" CssClass="UserGroupTextArea" ReadOnly="true"></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Enabled" HeaderText="<%$Resources: ColumnHeaders, Enabled %>" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="LastLoginTime" HeaderText="<%$Resources: ColumnHeaders, LastLogin %>" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" >
                        <itemstyle width="175px" />
                    </asp:BoundField>
                </Columns>
                <EmptyDataTemplate>
                    <ccAsp:EmptySearchResultsMessage ID="EmptySearchResultsMessage" runat="server" Message="<%$Resources: SR, NoUsersFound %>" />
                </EmptyDataTemplate>
                <RowStyle CssClass="GlobalGridViewRow" />
                <AlternatingRowStyle CssClass="GlobalGridViewRow" />
                <HeaderStyle CssClass="GlobalGridViewHeader" />
                <SelectedRowStyle CssClass="GlobalGridViewSelectedRow" />
            </ccUI:GridView>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
