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

<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ServerPartitionGridPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerPartitions.ServerPartitionGridPanel" %>

<%@ Register Src="~/Controls/UsersGuideLink.ascx" TagPrefix="cc" TagName="HelpLink" %>

<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
    Width="100%">
    <asp:TableRow VerticalAlign="top">
        <asp:TableCell VerticalAlign="top">   
            <ccUI:GridView ID="PartitionGridView" runat="server" 
                OnRowDataBound="PartitionGridView_RowDataBound" 
                PageSize="20">
                <Columns>
                    <asp:BoundField DataField="AeTitle" HeaderText="<%$Resources: ColumnHeaders,AETitle %>" HeaderStyle-HorizontalAlign="Left"/>
                    <asp:BoundField DataField="Description" HeaderText="<%$Resources: ColumnHeaders, PartitionDescription %>" HeaderStyle-HorizontalAlign="Left" />
                    <asp:BoundField DataField="Port" HeaderText="<%$Resources: ColumnHeaders,Port %>" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="PartitionStorageConfigColumnContent">
                                <%=ColumnHeaders.PartitionStorageConfiguration %>
                               <cc:HelpLink runat="server" TopicID="Partition_Storage_Configuration" Target="_blank" />
                            </div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <div class="PartitionStorageConfigColumnContent">
                                <asp:Label ID="PartitionStorageConfigurationLabel" runat="server" />
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders,Enabled %>">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Enabled") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Image ID="ActiveImage" runat="server" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders, PartitionAcceptAnyDevice %>">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("AcceptAnyDeviceImage") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Image ID="AcceptAnyDeviceImage" runat="server" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders,PartitionDuplicateObjectPolicy %>" HeaderStyle-HorizontalAlign="Left">
                        <ItemTemplate>
                            <asp:Label ID="DuplicateSopDescription" runat="server"/>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders, PartitionStudiesCount %>">
                        <ItemTemplate>
                            <asp:Label ID="Studies" runat="server" Text='<%# Eval("StudyCount","{0:N0}") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <ccAsp:EmptySearchResultsMessage ID="EmptySearchResultsMessage" runat="server" Message="<%$Resources: SR, AdminPartition_NoPartitionsFound %>" />
                </EmptyDataTemplate>
                <RowStyle CssClass="GlobalGridViewRow" />
                <HeaderStyle CssClass="GlobalGridViewHeader" />
                <SelectedRowStyle CssClass="GlobalGridViewSelectedRow" />
                <AlternatingRowStyle CssClass="GlobalGridViewRow" />                                
            </ccUI:GridView>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
