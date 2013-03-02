<%-- License

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

<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Queues.ArchiveQueue.ArchiveQueueItemList"
	Codebehind="ArchiveQueueItemList.ascx.cs" %>
<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
	Width="100%">
	<asp:TableRow VerticalAlign="top">
		<asp:TableCell VerticalAlign="top">
			<asp:ObjectDataSource ID="ArchiveQueueDataSourceObject" runat="server" TypeName="ClearCanvas.ImageServer.Web.Common.Data.DataSource.ArchiveQueueDataSource"
				DataObjectTypeName="ClearCanvas.ImageServer.Web.Common.Data.DataSource.ArchiveQueueSummary" EnablePaging="true"
				SelectMethod="Select" SelectCountMethod="SelectCount" OnObjectCreating="GetArchiveQueueDataSource"
				OnObjectDisposing="DisposeDataSource"/>
				<ccUI:GridView ID="ArchiveQueueGridView" runat="server"
					OnSelectedIndexChanged="ArchiveQueueGridView_SelectedIndexChanged"
					OnPageIndexChanging="ArchiveQueueGridView_PageIndexChanging"
					OnRowDataBound="GridView_RowDataBound"
					SelectionMode="Multiple">
					<Columns>
						<asp:TemplateField HeaderText="<%$Resources: ColumnHeaders, PatientName %>" HeaderStyle-HorizontalAlign="Left">
							<itemtemplate>
                            <ccUI:PersonNameLabel ID="PatientName" runat="server" PersonName='<%# Eval("PatientsName") %>' PersonNameType="Dicom"></ccUI:PersonNameLabel>
                        </itemtemplate>
						</asp:TemplateField>
						<asp:BoundField DataField="PatientID" HeaderText="<%$Resources: ColumnHeaders, PatientID %>" HeaderStyle-HorizontalAlign="Left">
						</asp:BoundField>
						<asp:TemplateField HeaderText="<%$Resources: ColumnHeaders, ArchiveQueueSchedule %>" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
							<itemtemplate>
                                <ccUI:DateTimeLabel ID="ScheduledDateTime" Value='<%# Eval("ScheduledDateTime") %>' runat="server"></ccUI:DateTimeLabel>
                            </itemtemplate>
						</asp:TemplateField>
						<asp:BoundField DataField="StatusString" HeaderText="<%$Resources: ColumnHeaders, ArchiveQueueStatus %>" HeaderStyle-HorizontalAlign="Center"
							ItemStyle-HorizontalAlign="Center" />
						<asp:BoundField DataField="Notes" HeaderText="Notes" HeaderStyle-HorizontalAlign="Left">
						</asp:BoundField>
					</Columns>
					<EmptyDataTemplate>
                        <ccAsp:EmptySearchResultsMessage ID="EmptySearchResultsMessage" runat="server" Message="<%$Resources: SR, NoArchiveQueueItemFound %>" />
					</EmptyDataTemplate>
					<RowStyle CssClass="GlobalGridViewRow" />
					<AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow" />
					<SelectedRowStyle CssClass="GlobalGridViewSelectedRow" />
					<HeaderStyle CssClass="GlobalGridViewHeader" />
					<PagerTemplate>
					</PagerTemplate>
				</ccUI:GridView>
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>
