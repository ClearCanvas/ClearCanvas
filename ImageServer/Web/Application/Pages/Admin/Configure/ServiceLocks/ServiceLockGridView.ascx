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

<%@ Import namespace="ClearCanvas.ImageServer.Web.Common.Utilities"%>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServiceLocks.ServiceLockGridView"
    Codebehind="ServiceLockGridView.ascx.cs" %>

<asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
<ContentTemplate>
<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
    Width="100%">
    <asp:TableRow VerticalAlign="top">
        <asp:TableCell VerticalAlign="top">
        
            <ccUI:GridView ID="GridView" runat="server" 
                OnRowDataBound="GridView_RowDataBound" OnDataBound="GridView_DataBound"
                OnPageIndexChanging="GridView_PageIndexChanging"
                PageSize="20">
                <Columns>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders, ServiceType %>">
                        <ItemTemplate>
                           <asp:Label ID="Type" runat="server"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders, ServiceDescription %>">
                        <ItemTemplate>
                           <asp:Label ID="Description" runat="server"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders, Enabled %>">
                        <ItemTemplate>
                            <asp:Image ID="EnabledImage" runat="server" SkinId="Unchecked" />
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders, ServiceLocked %>">
                        <ItemTemplate>
                            <asp:Image ID="LockedImage" runat="server" SkinId="Unchecked" />
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                     <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders, ServiceSchedule %>">
                        <ItemTemplate>
                            <ccUI:DateTimeLabel ID="Schedule" runat="server" Value='<%# Eval("ScheduledTime") %>' ></ccUI:DateTimeLabel>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" Wrap="true" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders, ServiceFilesystem %>">
                        <ItemTemplate>
                            <asp:Label ID="Filesystem" runat="server" Text=""/>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center"/>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders, ServiceProcessorID %>">
                        <ItemTemplate>
                            <asp:Label ID="ProcessorID" runat="server" Text='<%# Eval("ProcessorID") %>' style="padding-right: 5px;"></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" Wrap="false" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <ccAsp:EmptySearchResultsMessage ID="EmptySearchResultsMessage" runat="server" Message="<%$Resources: SR, NoServicesFound %>" />
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
</ContentTemplate>
</asp:UpdatePanel>
