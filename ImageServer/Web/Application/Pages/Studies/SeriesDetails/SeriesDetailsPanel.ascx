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



<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SeriesDetailsPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.SeriesDetails.SeriesDetailsPanel" %>

<%@ Register Src="StudySummaryPanel.ascx" TagName="StudySummaryPanel" TagPrefix="localAsp" %>
<%@ Register Src="SeriesDetailsView.ascx" TagName="SeriesDetailsView" TagPrefix="localAsp" %>
<%@ Register Src="PatientSummaryPanel.ascx" TagName="PatientSummaryPanel" TagPrefix="localAsp" %>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="conditional">
        <ContentTemplate>
              
  <table cellpadding="0" cellspacing="0" width="100%">
  
  <tr>
  <td class="MainContentTitle"><%= Titles.SeriesDetails %></td>
  </tr>
    
  <tr>
  <td class="PatientInfo"><localAsp:PatientSummaryPanel ID="PatientSummary" runat="server" /></td>
  </tr>
  
  <tr><td><asp:Image runat="server" SkinID="Spacer" Height="4" /></td></tr>
  
  <tr>
  <td>
      <table width="100%" cellpadding="2" cellspacing="0" class="ToolbarButtonPanel">
        <tr><td class="MainContentSubTitle"><%= Labels.SeriesDetails_StudySummary%></td></tr>
        <tr><td>
        <localAsp:StudySummaryPanel ID="StudySummary" runat="server" />
        </td></tr>
    </table>
  </td>
  </tr>
  
  <tr><td><asp:Image ID="Image1" runat="server" SkinID="Spacer" Height="4" /></td></tr>
  
  <tr>
  <td>
    <table width="100%" cellpadding="2" cellspacing="0" class="ToolbarButtonPanel">
        <tr><td class="MainContentSubTitle"><%= Labels.SeriesDetails_SeriesSummary %></td></tr>
        <tr><td>
        <localAsp:SeriesDetailsView ID="SeriesDetails" runat="server" />
        </td></tr>
    </table>
  </tr>
  
  
  </table>
  
        </ContentTemplate>
    </asp:UpdatePanel>

