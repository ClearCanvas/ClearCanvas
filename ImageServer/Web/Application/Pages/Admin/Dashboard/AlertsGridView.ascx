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
<%@ Import Namespace="Microsoft.JScript" %>
<%@ Import Namespace="ClearCanvas.ImageServer.Web.Common.Data.DataSource" %>
<%@ Import Namespace="ClearCanvas.ImageServer.Web.Common.Utilities" %>
<%@ Import Namespace="System.Xml" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AlertsGridView.ascx.cs"
	Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Dashboard.AlertsGridView" %>
<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
	Width="100%">
     <asp:TableRow VerticalAlign="top">
        <asp:TableCell VerticalAlign="top">
            <ccAsp:GridPager ID="GridPagerTop" runat="server" />   
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow VerticalAlign="top">
		<asp:TableCell VerticalAlign="top">
			<asp:ObjectDataSource ID="AlertDataSourceObject" runat="server" TypeName="ClearCanvas.ImageServer.Web.Common.Data.DataSource.AlertDataSource"
				DataObjectTypeName="ClearCanvas.ImageServer.Web.Common.Data.DataSource.AlertSummary"
				EnablePaging="true" SelectMethod="Select" SelectCountMethod="SelectCount" OnObjectCreating="GetAlertDataSource"
				OnObjectDisposing="DisposeAlertDataSource" />
			<ccUI:GridView ID="AlertGridView" runat="server" OnRowDataBound="AlertGridView_RowDataBound"
				SelectionMode="Single" DataKeyNames="Key" PageSize="10">
				<Columns>
					<asp:TemplateField HeaderText="<%$Resources:ColumnHeaders,AlertContent %>">
						<ItemTemplate>
							<%# Eval("Message") %>
							<asp:LinkButton runat="server" ID="AppLogLink" Text="<%$Resources: Labels,AlertGridPanel_LinkToLogs%>"
								CssClass="LogInfo" />
							<asp:PlaceHolder runat="server" ID="DetailsHoverPlaceHolder"></asp:PlaceHolder>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:BoundField DataField="Component" HeaderText="<%$Resources:ColumnHeaders,AlertComponent %>"
						HeaderStyle-HorizontalAlign="Left" />
					<asp:BoundField DataField="Source" HeaderText="<%$Resources:ColumnHeaders,AlertSource %>"
						HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
					<asp:TemplateField HeaderText="<%$Resources:ColumnHeaders,AlertInsertDate%>" HeaderStyle-HorizontalAlign="Center"
						ItemStyle-HorizontalAlign="Center">
						<ItemTemplate>
							<ccUI:DateTimeLabel ID="InserTime" runat="server" Value='<%# Eval("InsertTime") %>' />
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="<%$Resources:ColumnHeaders,AlertLevel %>" HeaderStyle-HorizontalAlign="Center"
						ItemStyle-HorizontalAlign="Center">
						<ItemTemplate>
							<asp:Label ID="Level" Text="" runat="server" />
						</ItemTemplate>
					</asp:TemplateField>
					<asp:BoundField DataField="Category" HeaderText="<%$Resources:ColumnHeaders,AlertCategory %>"
						HeaderStyle-HorizontalAlign="Left" />
				</Columns>
				<EmptyDataTemplate>
					<ccAsp:EmptySearchResultsMessage runat="server" Message="<%$Resources: SR,NoAlertsFound %>" />
				</EmptyDataTemplate>
				<RowStyle CssClass="GlobalGridViewRow" />
				<AlternatingRowStyle CssClass="GlobalGridViewRow" />
				<HeaderStyle CssClass="GlobalGridViewHeader" />
				<SelectedRowStyle CssClass="GlobalGridViewSelectedRow" />
			</ccUI:GridView>
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>
