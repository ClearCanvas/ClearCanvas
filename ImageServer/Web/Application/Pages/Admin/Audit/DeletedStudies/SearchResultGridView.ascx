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
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResultGridView.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.SearchResultGridView" %>


<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
	Width="100%">
	<asp:TableRow VerticalAlign="top">
		<asp:TableCell VerticalAlign="top">
		
            <asp:ObjectDataSource ID="DataSource" runat="server" 
                TypeName="ClearCanvas.ImageServer.Web.Common.Data.DataSource.DeletedStudyDataSource"
				DataObjectTypeName="ClearCanvas.ImageServer.Web.Common.Data.Model.DeletedStudyInfo" 
				EnablePaging="true"
				SelectMethod="Select"
				StartRowIndexParameterName="startRowIndex"
				MaximumRowsParameterName="maxRows"
				SelectCountMethod="SelectCount" />
				
				<ccUI:GridView ID="ListControl" runat="server"
					SelectionMode="Single" DataKeyNames="RowKey">
					<Columns>
						<asp:TemplateField HeaderText="<%$Resources: ColumnHeaders,PatientName %>" HeaderStyle-HorizontalAlign="Left">
							<itemtemplate>
                            <ccUI:PersonNameLabel ID="PatientName" runat="server" PersonName='<%# Eval("PatientsName") %>' PersonNameType="Dicom"></ccUI:PersonNameLabel>
                        </itemtemplate>
						</asp:TemplateField>
						<asp:BoundField DataField="PatientId" HeaderText="<%$Resources: ColumnHeaders,PatientID%>" HeaderStyle-HorizontalAlign="Left">
						</asp:BoundField>
						<asp:BoundField DataField="AccessionNumber" HeaderText="<%$Resources: ColumnHeaders,AccessionNumber %>" HeaderStyle-HorizontalAlign="Center"
							ItemStyle-HorizontalAlign="Center"></asp:BoundField>
						<asp:TemplateField HeaderText="Study Date" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
							<itemtemplate>
                                <ccUI:DALabel ID="StudyDate" runat="server" Value='<%# Eval("StudyDate") %>'></ccUI:DALabel>
                            </itemtemplate>
						</asp:TemplateField>
						<asp:BoundField DataField="StudyDescription" HeaderText="<%$Resources: ColumnHeaders,StudyDescription%>" HeaderStyle-HorizontalAlign="Center"
							ItemStyle-HorizontalAlign="Center" />
				        <asp:BoundField DataField="PartitionAE" HeaderText="<%$Resources: ColumnHeaders,Partition %>" HeaderStyle-HorizontalAlign="Center"
							ItemStyle-HorizontalAlign="Center" />
					    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders,AdminDeletedStudies_DeletedBy %>" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
							<itemtemplate>
                                <%# Eval("UserName")%>
                            </itemtemplate>
						</asp:TemplateField>
					</Columns>
					<EmptyDataTemplate>				    
					<ccAsp:EmptySearchResultsMessage runat="server" ID="NoResultFoundMessage" Message="<%$Resources: SR, AdminDeletedStudies_NoStudiesFound %>">
						<SuggestionTemplate>					
						    <ul style="padding-left: 15px; margin-left: 5px; margin-top: 4px; margin-bottom: 4px;">
	                            <li><%=SR.AdminDeletedStudies_ModifySearchCriteria%></li>
	                            <li><%=SR.AdminDeletedStudies_CheckPartitionConfiguration%></li>
	                        </ul>	    
						</SuggestionTemplate>
					</ccAsp:EmptySearchResultsMessage>
					
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
