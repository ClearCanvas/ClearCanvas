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

<%@ Import namespace="ClearCanvas.ImageServer.Web.Common.Utilities"%>
<%@ Import namespace="ClearCanvas.Common.Utilities"%>
<%@ Import namespace="ClearCanvas.ImageServer.Web.Common.Data.DataSource"%>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.StudyIntegrityQueueGridView"
	Codebehind="StudyIntegrityQueueGridView.ascx.cs" %>
	
	
<div class="GridViewBorder">
<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
	Width="100%">
	<asp:TableRow VerticalAlign="top">
		<asp:TableCell VerticalAlign="top">
			<asp:ObjectDataSource ID="StudyIntegrityQueueDataSourceObject" runat="server" TypeName="ClearCanvas.ImageServer.Web.Common.Data.DataSource.StudyIntegrityQueueDataSource"
				DataObjectTypeName="ClearCanvas.ImageServer.Web.Common.Data.DataSource.StudyIntegrityQueueSummary" EnablePaging="true"
				SelectMethod="Select" SelectCountMethod="SelectCount" OnObjectCreating="GetStudyIntegrityQueueDataSource"
				OnObjectDisposing="DisposeDataSource"/>
				<ccUI:GridView ID="StudyIntegrityQueueList" runat="server"
					OnSelectedIndexChanged="StudyIntegrityQueueGridView_SelectedIndexChanged"
					OnPageIndexChanging="StudyIntegrityQueueGridView_PageIndexChanging"
					SelectionMode="Disabled" 
					MouseHoverRowHighlightEnabled="false"
					OnRowDataBound="StudyIntegrityQueueItemList_RowDataBound">
					<Columns>
					    <asp:BoundField DataField="Reason" HeaderText="" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left">
						</asp:BoundField>
						<asp:TemplateField HeaderText="<%$Resources: ColumnHeaders,StudyInstanceUID %>" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
							<itemtemplate>
							    <asp:Label runat="server" ID="ExistingStudyInstanceUidLabel" Text='<%# (bool)Eval("StudyExists")? Eval("StudyInstanceUid"): "N/A" %>'></asp:Label>
							</itemtemplate>
					    </asp:TemplateField>
						<asp:TemplateField HeaderText="<%$Resources: ColumnHeaders,SIQExistingStudy %>" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
							<itemtemplate>							    
							    <asp:Table runat="server" ID="ExistingStudyTable">
							        <asp:TableRow>
							            <asp:TableCell style="border:none; vertical-align:top">
							            </asp:TableCell>
							            <asp:TableCell style="border:none; vertical-align:top; text-align:left">
							                 <asp:Label runat="server" ID="ExistingPatientId" CssClass="StudyField" Text='<%# Eval("ExistingPatientId")%>'></asp:Label> / 
							                <asp:Label runat="server" ID="ExistingPatientName" CssClass="StudyField" Text='<%# Eval("ExistingPatientName")%>'></asp:Label>
							            </asp:TableCell>
							        </asp:TableRow>
							        <asp:TableRow>
							            <asp:TableCell  style="border:none; vertical-align:top">
							            </asp:TableCell>
							            <asp:TableCell style="border:none; vertical-align:top; text-align:left; font-size:80%;">
							                <asp:Label runat="server" ID="ExistingAccessionNumber" CssClass="StudyField" Text='<%# Eval("ExistingAccessionNumber")%>'></asp:Label> / 
                                            <asp:Label runat="server" ID="ExistingStudyDescription" CssClass="StudyField" Text='<%# Eval("StudySummary.StudyDescription")%>'></asp:Label> / 
							                <asp:Label runat="server" ID="ExistingStudyDate" CssClass="StudyField" Text='<%# Eval("StudySummary.StudyDate")%>'></asp:Label> / 
							                <asp:Label runat="server" ID="ExistingModalitiesInStudy" CssClass="StudyField" Text='<%# StringUtilities.EmptyIfNull(Eval("StudySummary.ModalitiesInStudy") as String).Replace("\\", ",") %>'></asp:Label>
							            </asp:TableCell>
							        </asp:TableRow>
							    </asp:Table>    
							    <asp:Label runat="server" ID="StudyNotAvailableLabel" Text="<%$Resources: SR, NotAvailable %>"></asp:Label>
                            </itemtemplate>
						</asp:TemplateField>
		                <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders,SIQConflictingImageInfo %>" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
							<itemtemplate>                               
                               <table width="100%">
							    <tr>
							        <td style="border:none; vertical-align:top"></td>
							        <td style="border:none; vertical-align:top; text-align:left;">
							            <asp:Label runat="server" ID="ConflictingPatientId" CssClass="StudyField" Text='<%# Eval("ConflictingPatientId")%>'></asp:Label> / 
							            <asp:Label runat="server" ID="ConflictingPatientName" CssClass="StudyField" Text='<%# Eval("ConflictingPatientName")%>'></asp:Label>
							        </td>							        
							    </tr>
							    <tr >
							        <td style="border:none; vertical-align:top"></td>
							        <td style="border:none; vertical-align:top; text-align:left; font-size:80%;">
							            <asp:Label runat="server" ID="ConflictingAccessionNumber" CssClass="StudyField" Text='<%# Eval("ConflictingAccessionNumber")%>'></asp:Label> / <asp:Label runat="server" ID="ConflictingStudyDescription" CssClass="StudyField" Text='<%# Eval("ConflictingStudyDescription")%>'></asp:Label> / 
                                        <asp:Label runat="server" ID="ConflictingStudyDate" CssClass="StudyField"  Text='<%# Eval("ConflictingStudyDate")%>'></asp:Label> / 
                                        <asp:Label runat="server" ID="ConflictingModalities" CssClass="StudyField" Text='<%# StringUtilities.Combine(Eval("ConflictingModalities") as string[], ",") %>'></asp:Label>
							        </td>
							    </tr>
							    </table>    
                            </itemtemplate>
						</asp:TemplateField>
			            <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders,SIQTimeReceived %>" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" >
                                <ItemTemplate>
                                    <ccUI:DateTimeLabel ID="TimeReceived" runat="server" Value='<%# Eval("ReceivedTime") %>' ></ccUI:DateTimeLabel>
                                </ItemTemplate>
                        </asp:TemplateField>   
					</Columns>
				    <EmptyDataTemplate>
                        <ccAsp:EmptySearchResultsMessage ID="EmptySearchResultsMessage" runat="server" Message="<%$Resources: SR, StudyDetails_NoSIQForThisStudy %>" />
					</EmptyDataTemplate>					
					<RowStyle CssClass="GlobalGridViewRow StudyIntegrityQueueRow" />
					<AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow StudyIntegrityQueueRow" />
					<SelectedRowStyle CssClass="GlobalGridViewSelectedRow StudyIntegrityQueueRow" />
					<HeaderStyle CssClass="GlobalGridViewHeader" />
					<PagerTemplate>
					</PagerTemplate>
				</ccUI:GridView>
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>
</div>