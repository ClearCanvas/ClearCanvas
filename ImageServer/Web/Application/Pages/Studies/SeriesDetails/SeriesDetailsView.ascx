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

<%@ Import namespace="System.ComponentModel"%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SeriesDetailsView.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.SeriesDetails.SeriesDetailsView" %>

<asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False" CellPadding="2" 
    GridLines="Horizontal" Width="100%" CssClass="GlobalGridView" OnDataBound="DetailsView1_DataBound">
    <Fields>
        <asp:BoundField DataField="SeriesInstanceUid" HeaderText="<%$Resources: DetailedViewFieldLabels, SeriesInstanceUID %>">
            <HeaderStyle CssClass="SeriesDetailsGridViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:BoundField DataField="Modality" HeaderText="<%$Resources: DetailedViewFieldLabels, Modality%>">
            <HeaderStyle CssClass="SeriesDetailsGridViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:BoundField DataField="SeriesNumber" HeaderText="<%$Resources: DetailedViewFieldLabels, SeriesNumber%>">
            <HeaderStyle CssClass="SeriesDetailsGridViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:BoundField DataField="SeriesDescription" HeaderText="<%$Resources: DetailedViewFieldLabels, SeriesDescription%>">
            <HeaderStyle CssClass="SeriesDetailsGridViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:TemplateField HeaderText="<%$Resources: DetailedViewFieldLabels, PerformedOn%>">
            <HeaderStyle CssClass="SeriesDetailsGridViewHeader" Wrap="false" />
            <ItemTemplate>
                <ccUI:DALabel ID="PerformedDate" runat="server" Text="{0}" Value='<%# Eval("PerformedDate") %>' InvalidValueText="<i style='color:red'>[Invalid date:{0}]</i>"></ccUI:DALabel>
                <ccUI:TMLabel ID="PerformedTime" runat="server" Text="{0}" Value='<%# Eval("PerformedTime") %>' InvalidValueText="<i style='color:red'>[Invalid time:{0}]</i>"></ccUI:TMLabel>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="NumberOfSeriesRelatedInstances"  HeaderText="<%$Resources: DetailedViewFieldLabels, Instances%>">
            <HeaderStyle CssClass="SeriesDetailsGridViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:BoundField DataField="SourceApplicationEntityTitle" HeaderText="<%$Resources: DetailedViewFieldLabels, SourceAE%>">
            <HeaderStyle CssClass="SeriesDetailsGridViewHeader" Wrap="false" />
        </asp:BoundField>
        
        
    </Fields>
    <RowStyle CssClass="GlobalGridViewRow"/>
</asp:DetailsView>

