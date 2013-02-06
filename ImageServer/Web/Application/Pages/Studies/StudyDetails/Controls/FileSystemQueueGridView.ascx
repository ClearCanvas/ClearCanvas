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

<%@ Control Language="C#" AutoEventWireup="true" Codebehind="FileSystemQueueGridView.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.FileSystemQueueGridView" %>
<%@ Import Namespace="Resources"%>
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
        <ccUI:GridView ID="FSQueueGridView" runat="server" 
                       OnPageIndexChanged="FSQueueGridView_PageIndexChanged" 
                       OnPageIndexChanging="FSQueueGridView_PageIndexChanging" SelectionMode="Disabled"
                       OnRowDataBound="FSQueueGridView_RowDataBound"
                       MouseHoverRowHighlightEnabled="false"
                       GridLines="Horizontal" BackColor="White" >
                        <Columns>
                            <asp:BoundField DataField="FilesystemQueueTypeEnum" HeaderText="<%$Resources: ColumnHeaders, FS_Type %>">
                                <HeaderStyle wrap="False" />    
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders, FS_ScheduledTime %>">
                                <ItemTemplate>
                                    <ccUI:DateTimeLabel ID="ScheduledTime" runat="server" Value='<%# Eval("ScheduledTime") %>' ></ccUI:DateTimeLabel>
                                </ItemTemplate>
                                <HeaderStyle wrap="False" />    
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders, FS_QueueXML %>">
                                <ItemTemplate>
                                    <asp:Label ID="XmlText" runat="server"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <asp:Table ID="Table1" runat="server" Width="100%" CellPadding="0" CellSpacing="0" >
                                <asp:TableHeaderRow CssClass="GlobalGridViewHeader">
                                    <asp:TableHeaderCell><%=ColumnHeaders.FS_Type%></asp:TableHeaderCell>
                                    <asp:TableHeaderCell><%=ColumnHeaders.FS_ScheduledTime%></asp:TableHeaderCell>
                                    <asp:TableHeaderCell><%=ColumnHeaders.FS_QueueXML%></asp:TableHeaderCell>
                                </asp:TableHeaderRow>
                                <asp:TableRow>
                                    <asp:TableCell ColumnSpan="3" Height="50" HorizontalAlign="Center">
                                        <asp:panel ID="Panel1" runat="server" CssClass="GlobalGridViewEmptyText"><%= SR.StudyDetails_NoFileSystemForThisStudy%></asp:panel>
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