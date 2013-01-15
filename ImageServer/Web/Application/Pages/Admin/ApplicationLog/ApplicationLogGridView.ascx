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

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ApplicationLogGridView.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.ApplicationLog.ApplicationLogGridView" %>
<%@ Import Namespace="Resources" %>
<%@ Import namespace="ClearCanvas.ImageServer.Web.Common.Utilities"%>


<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
    Width="100%">
    <asp:TableRow VerticalAlign="top">
        <asp:TableCell VerticalAlign="top">
        <asp:ObjectDataSource ID="ApplicationLogDataSourceObject" runat="server" TypeName="ClearCanvas.ImageServer.Web.Common.Data.DataSource.ApplicationLogDataSource"
				DataObjectTypeName="ClearCanvas.ImageServer.Model.ApplicationLog" EnablePaging="true"
				SelectMethod="Select" SelectCountMethod="SelectCount" OnObjectCreating="GetApplicationLogDataSource"
				OnObjectDisposing="DisposeApplicationLogDataSource"/>
			
            <ccUI:GridView ID="ApplicationLogListControl" runat="server"
				OnPageIndexChanging="ApplicationLogListControl_PageIndexChanging" 
				OnRowDataBound="GridView_RowDataBound"
				EmptyDataText="No logs found (Please check the filters.)">
                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <Columns>
                    <asp:BoundField DataField="Host" HeaderText="<%$Resources:ColumnHeaders, AppLogHost%>" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"/>
					<asp:TemplateField HeaderText="<%$Resources:ColumnHeaders, AppLogTimestamp%>">
						<HeaderStyle Wrap="false" HorizontalAlign="Center" />
						<ItemStyle Wrap="false" HorizontalAlign="Center" />
						<ItemTemplate>
							<asp:Label ID="Timestamp" runat="server" Text='<%# DateTimeFormatter.Format((DateTime)Eval("Timestamp"),DateTimeFormatter.Style.Timestamp) %>'/>
						</ItemTemplate>
					</asp:TemplateField>
                    <asp:BoundField DataField="Thread" HeaderText="<%$Resources:ColumnHeaders, AppLogThread%>" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                    	<ItemStyle Wrap="false" HorizontalAlign="Center" />
					</asp:BoundField>
                    <asp:BoundField DataField="LogLevel" HeaderText="<%$Resources:ColumnHeaders, AppLogLogLevel%>" HeaderStyle-Wrap="false" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"/>
                    <asp:BoundField DataField="MessageException" HeaderText="<%$Resources:ColumnHeaders, AppLogMessage%>" HeaderStyle-HorizontalAlign="Left" HtmlEncode="false"  />
                </Columns>
                <EmptyDataTemplate>
                   <ccAsp:EmptySearchResultsMessage ID="EmptySearchResultsMessage" runat="server" Message="<%$Resources: SR,AppLogNotFound %>" />
                </EmptyDataTemplate>
                <RowStyle CssClass="GlobalGridViewRow" />
                <AlternatingRowStyle CssClass="GlobalGridViewRow" />
                <SelectedRowStyle CssClass="GlobalGridViewSelectedRow" />
                <HeaderStyle CssClass="GlobalGridViewHeader" />
            </ccUI:GridView>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
