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

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudyDetailsView.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.StudyDetailsView" %>

<div class="GridViewBorder">
<asp:DetailsView ID="StudyDetailView" runat="server" AutoGenerateRows="False" GridLines="Horizontal" CellPadding="4" 
     CssClass="GlobalGridView" Width="100%">
    <Fields>
        <asp:BoundField DataField="StudyDescription" HeaderText="<%$Resources: DetailedViewFieldLabels, StudyDescription%>">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:BoundField DataField="AccessionNumber" HeaderText="<%$Resources: DetailedViewFieldLabels, AccessionNumber%>">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
        </asp:BoundField>
            
        <asp:TemplateField HeaderText="<%$Resources: DetailedViewFieldLabels, ReferringPhysician%>">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
            <ItemTemplate>
                <ccUI:PersonNameLabel ID="ReferringPhysiciansName" runat="server" PersonName='<%# Eval("ReferringPhysiciansName") %>' PersonNameType="Dicom"></ccUI:PersonNameLabel>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="StudyInstanceUid" HeaderText="<%$Resources: DetailedViewFieldLabels, StudyInstanceUID%>">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:BoundField DataField="StudyStatusEnumString" HeaderText="<%$Resources: DetailedViewFieldLabels, StudyStatus%>">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:TemplateField HeaderText="<%$Resources: DetailedViewFieldLabels, StudyDateTime%>">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
            <ItemTemplate>
                <ccUI:DALabel ID="StudyDate" runat="server"  Value='<%# Eval("StudyDate") %>' ></ccUI:DALabel>
                <ccUI:TMLabel ID="StudyTime" runat="server"  Value='<%# Eval("StudyTime") %>'  ></ccUI:TMLabel>
            </ItemTemplate>
        </asp:TemplateField>  
        <asp:BoundField DataField="StudyID" HeaderText="<%$Resources: DetailedViewFieldLabels, StudyID%>">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:TemplateField Visible="False" HeaderText="<%$Resources: DetailedViewFieldLabels, ResponsiblePerson%>" >
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
            <ItemTemplate>
                <ccUI:PersonNameLabel ID="ResponsiblePersonName" runat="server" PersonName='<%# Eval("ResponsiblePerson") %>' PersonNameType="Dicom"></ccUI:PersonNameLabel>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="ResponsiblePersonRole" Visible="false" HeaderText="<%$Resources: DetailedViewFieldLabels, ResponsiblePersonRole%>">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:BoundField DataField="ResponsibleOrganization" Visible="false" HeaderText="<%$Resources: DetailedViewFieldLabels, ResponsibleOrganization%>">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:BoundField DataField="Species" Visible="false" HeaderText="<%$Resources: DetailedViewFieldLabels, Species%>">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:BoundField DataField="Breed" Visible="false" HeaderText="<%$Resources: DetailedViewFieldLabels, Breed%>">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:BoundField DataField="StudyID" HeaderText="<%$Resources: DetailedViewFieldLabels, StudyID%>">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:BoundField DataField="NumberOfStudyRelatedSeries" HeaderText="<%$Resources: DetailedViewFieldLabels, SeriesCount%>">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:BoundField DataField="NumberOfStudyRelatedInstances" HeaderText="<%$Resources: DetailedViewFieldLabels, Instances%>">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
        </asp:BoundField>
    </Fields>
    <RowStyle CssClass="GlobalGridViewRow"/>
    <AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow" />
    
</asp:DetailsView>
</div>