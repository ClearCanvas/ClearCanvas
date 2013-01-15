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

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DataRuleGridView.ascx.cs" 
Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.DataRules.DataRuleGridView" %>

<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
    Width="100%">
    <asp:TableRow VerticalAlign="top">
        <asp:TableCell VerticalAlign="top">
        
            <ccUI:GridView ID="GridView" runat="server" 
                OnRowDataBound="GridView_RowDataBound"               
                OnPageIndexChanging="GridView1_PageIndexChanging" PageSize="20">
                <Columns>
                    <asp:BoundField DataField="RuleName" HeaderText="<%$Resources: ColumnHeaders, ServerRuleName  %>" HeaderStyle-HorizontalAlign="Left"></asp:BoundField>
                    <asp:TemplateField HeaderText="Type" HeaderStyle-HorizontalAlign="Left">
                        <ItemTemplate>
                            <asp:Label ID="ServerRuleTypeEnum" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders, ServerRuleApplyTime  %>" >
                        <ItemTemplate>
                            <asp:Label ID="ServerRuleApplyTimeEnum" runat="server"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders, Enabled  %>">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Enabled") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Image ID="EnabledImage" runat="server" SkinId="Unchecked" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle  HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders, ServerRuleDefault  %>">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Default") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Image ID="DefaultImage" runat="server" SkinId="Unchecked" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders, ServerRuleExcempt  %>">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("Exempt") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Image ID="ExemptImage" runat="server" SkinId="Unchecked" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <ccAsp:EmptySearchResultsMessage ID="EmptySearchResultsMessage" runat="server" Message="<%$Resources: SR,NoServerRulesFound %>" />
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
