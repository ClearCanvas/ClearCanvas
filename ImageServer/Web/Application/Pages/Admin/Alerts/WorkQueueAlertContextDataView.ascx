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


<%@ Import namespace="ClearCanvas.ImageServer.Core.Validation"%>
<%@ Import namespace="ClearCanvas.ImageServer.Services.WorkQueue"%>
<%@ Import namespace="ClearCanvas.ImageServer.Web.Common.Utilities"%>
<%@ Import Namespace="Resources" %>


<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkQueueAlertContextDataView.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Alerts.WorkQueueAlertContextDataView" %>

<%  WorkQueueAlertContextData data = this.Alert.ContextData as WorkQueueAlertContextData;
    String viewWorkQueueUrl = HtmlUtility.ResolveWorkQueueDetailsUrl(Page, data.WorkQueueItemKey);
    String viewStudyUrl = data.ValidationStudyInfo != null? HtmlUtility.ResolveStudyDetailsUrl(Page, data.ValidationStudyInfo.ServerAE, data.ValidationStudyInfo.StudyInstaneUid):null;
    
%>

<div >
<table class="WorkQueueAlertStudyTable" cellspacing="0" cellpadding="0">
<% if (data.ValidationStudyInfo!=null) { %>
<tr><td style="font-weight: bold; color: #336699"><%=SR.Partition %>:</td><td><%= data.ValidationStudyInfo.ServerAE %>&nbsp;</td></tr>
<tr><td style="font-weight: bold; color: #336699"><%=SR.PatientName %>:</td><td><pre><%= data.ValidationStudyInfo.PatientsName%>&nbsp;</pre></td></tr>
<tr><td style="font-weight: bold; color: #336699"><%=SR.PatientID %>:</td><td><pre><%= data.ValidationStudyInfo.PatientsId %>&nbsp;</pre></td></tr>
<tr><td style="font-weight: bold; color: #336699"><%=SR.StudyInstanceUID %>:</td><td><%= data.ValidationStudyInfo.StudyInstaneUid%>&nbsp;</td></tr>
<tr><td style="font-weight: bold; color: #336699"><%=SR.AccessionNumber %>:</td><td><pre><%= data.ValidationStudyInfo.AccessionNumber%>&nbsp;</pre></td></tr>
<tr><td style="font-weight: bold; color: #336699"><%=SR.StudyDate %>:</td><td><pre><%= data.ValidationStudyInfo.StudyDate%>&nbsp;</pre></td></tr>
<%} else {%>
<tr><td>
    <%=SR.AlertNoStudyInformationForThisItem %>
</td></tr>
<%} %>

</table>

<table cellpadding="0" cellspacing="0" style="margin-top: 3px;">
    <tr >
        <% if (data.ValidationStudyInfo!=null){%>
        <td><a href='<%=viewStudyUrl%>' target="_blank" style="color: #6699CC; text-decoration: none; font-weight: bold;"><%=Labels.WorkQueueAlertContextDataView_ViewStudy%></a></td>
        <td style="font-weight: bold; color: #336699;">|</td>
        <%}%>
        <td><a href='<%= viewWorkQueueUrl %>' target="_blank" style="color: #6699CC; text-decoration: none; font-weight: bold;"><%=Labels.WorkQueueAlertContextDataView_ViewWorkQueue%></a></td>
    </tr>
</table>

</div>
