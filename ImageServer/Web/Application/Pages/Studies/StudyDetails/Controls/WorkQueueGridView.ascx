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

<%@ Control Language="C#" AutoEventWireup="true" Codebehind="WorkQueueGridView.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.WorkQueueGridView" %>
<%@ Import Namespace="Resources"%>
<%@ Import Namespace="ClearCanvas.ImageServer.Common"%>
<%@ Import Namespace="ClearCanvas.ImageServer.Web.Common.Data"%>
<%@ Import Namespace="ClearCanvas.ImageServer.Model" %>

    
<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
</asp:ScriptManagerProxy>

<div class="GridViewBorder">
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
	Width="100%">
	<asp:TableRow VerticalAlign="top">
		<asp:TableCell VerticalAlign="top">   
        <ccUI:GridView ID="StudyWorkQueueGridView" runat="server" 
                       OnPageIndexChanged="StudyWorkQueueGridView_PageIndexChanged" 
                       OnPageIndexChanging="StudyWorkQueueGridView_PageIndexChanging" SelectionMode="Disabled"
                       MouseHoverRowHighlightEnabled="false"
                       GridLines="Horizontal" BackColor="White" >
                        <Columns>
                            <asp:BoundField DataField="WorkQueueTypeEnum" HeaderText="<%$Resources: ColumnHeaders, WorkQueueType %>">
                                <HeaderStyle wrap="False" />    
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders, WorkQueueSchedule %>">
                                <HeaderStyle wrap="False" />    
                                <ItemTemplate>
                                    <ccUI:DateTimeLabel ID="ScheduledTime" runat="server" Value='<%# Eval("ScheduledTime") %>' ></ccUI:DateTimeLabel>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="WorkQueuePriorityEnum" HeaderText="<%$Resources: ColumnHeaders,WorkQueuePriority %>">
                                <HeaderStyle wrap="False" />    
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders,WorkQueueStatus %>">
                                <HeaderStyle wrap="False" />    
                                <ItemTemplate>
                                    <table>
                                    <tr>
                                    <td style="border-bottom:none"><%# Eval("WorkQueueStatusEnum")  %></td>
                                    <td style="border-bottom:none"><asp:Image runat="server" Visible='<%# !(Container.DataItem as WorkQueue).WorkQueueStatusEnum.Equals(WorkQueueStatusEnum.Failed) && !ServerPlatform.IsActiveWorkQueue(Container.DataItem as WorkQueue) %>'  ImageAlign="AbsBottom" ID="StuckIcon" SkinID="WarningSmall" 
                                        ToolTip="<%$Resources: Tooltips, WorkQueueIsStuck %>"/></td>
                                    </tr>
                                    </table>
                                </ItemTemplate>
                            </asp:TemplateField>
                                
                            <asp:BoundField DataField="ProcessorID" HeaderText="<%$Resources: ColumnHeaders,WorkQueueProcessingServer %>">
                                <HeaderStyle wrap="False" />  
                            </asp:BoundField>
                            <asp:BoundField DataField="FailureDescription" HeaderText="<%$Resources: ColumnHeaders,WorkQueueNotes %>">
                                <HeaderStyle wrap="False" />  
                            </asp:BoundField>                            
                        </Columns>
                        <EmptyDataTemplate>
                            <asp:Table ID="Table1" runat="server" Width="100%" CellPadding="0" CellSpacing="0" >
                                <asp:TableHeaderRow CssClass="GlobalGridViewHeader">
                                    <asp:TableHeaderCell><%= ColumnHeaders.WorkQueueType %></asp:TableHeaderCell>
                                    <asp:TableHeaderCell><%= ColumnHeaders.WorkQueueSchedule %></asp:TableHeaderCell>
                                    <asp:TableHeaderCell><%= ColumnHeaders.WorkQueuePriority %></asp:TableHeaderCell>
                                    <asp:TableHeaderCell><%= ColumnHeaders.WorkQueueStatus%></asp:TableHeaderCell>
                                    <asp:TableHeaderCell><%= ColumnHeaders.WorkQueueProcessingServer%></asp:TableHeaderCell>
                                    <asp:TableHeaderCell><%= ColumnHeaders.WorkQueueNotes%></asp:TableHeaderCell>
                                </asp:TableHeaderRow>
                                <asp:TableRow>
                                    <asp:TableCell ColumnSpan="6" Height="50" HorizontalAlign="Center">
                                        <asp:panel runat="server" CssClass="GlobalGridViewEmptyText"><%= SR.StudyDetails_NoWorkQueueForThisStudy%></asp:panel>
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>
                        </EmptyDataTemplate>
                        
                        <RowStyle CssClass="GlobalGridViewRow"/>
                        <HeaderStyle CssClass="GlobalGridViewHeader"/>
                        <AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow" />
                        <SelectedRowStyle  CssClass="GlobalGridViewSelectedRow" />
                    </ccUI:GridView>   
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>
                    
    </ContentTemplate>
</asp:UpdatePanel>
</div>