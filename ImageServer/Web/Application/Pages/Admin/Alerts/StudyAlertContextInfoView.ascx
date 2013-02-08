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


<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudyAlertContextInfoView.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Alerts.StudyAlertContextInfoView" %>
<%@ Import Namespace="ClearCanvas.ImageServer.Model"%>

<%  StudyAlertContextInfo data = this.Alert.ContextData as StudyAlertContextInfo;
    String viewStudyUrl = HtmlUtility.ResolveStudyDetailsUrl(Page, data.ServerPartitionAE, data.StudyInstanceUid);
%>

<div >
<table cellpadding="0" cellspacing="0" style="margin-top: 3px;">
    <tr >
        <td><a href='<%=viewStudyUrl%>' target="_blank" style="color: #6699CC; text-decoration: none; font-weight: bold;"><%=Labels.WorkQueueAlertContextDataView_ViewStudy%></a></td>        
    </tr>
</table>

</div>
