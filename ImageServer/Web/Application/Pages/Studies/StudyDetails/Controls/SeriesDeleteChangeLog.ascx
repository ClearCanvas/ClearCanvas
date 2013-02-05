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

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SeriesDeleteChangeLog.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.SeriesDeleteChangeLog" %>
<%@ Import Namespace="Resources"%>

<%@ Import Namespace="ClearCanvas.ImageServer.Core.Edit" %>
<%@ Import Namespace="ClearCanvas.ImageServer.Core.Data" %>
<%@ Import Namespace="ClearCanvas.ImageServer.Web.Common.Utilities" %>
<%@ Import Namespace="ClearCanvas.Dicom.Utilities.Command" %>

<script type="text/javascript">

    $(document).ready(function() {
        $("#<%=HistoryDetailsPanel.ClientID%>").hide();
        $("#<%=ShowHideDetails.ClientID%>").click(function() {
        if ($("#<%=ShowHideDetails.ClientID%>").text() == "[<%= Labels.StudyDetails_History_ShowDetails %>]") {
                $("#<%=HistoryDetailsPanel.ClientID%>").show();
                $("#<%=ShowHideDetails.ClientID%>").text("[<%= Labels.StudyDetails_History_HideDetails %>]");
                $("#<%=SummaryPanel.ClientID %>").css("font-weight", "bold");
                $("#<%=SummaryPanel.ClientID %>").css("margin-top", "5px");
                $("#<%=ShowHideDetails.ClientID%>").css("font-weight", "normal");
            } else {
                $("#<%=HistoryDetailsPanel.ClientID%>").hide();
                $("#<%=ShowHideDetails.ClientID%>").text("[<%= Labels.StudyDetails_History_ShowDetails %>]");
                $("#<%=SummaryPanel.ClientID %>").css("font-weight", "normal");
                $("#<%=SummaryPanel.ClientID %>").css("margin-top", "0px");
                $("#<%=ShowHideDetails.ClientID%>").css("font-weight", "normal");
            }
            return false;
        });
    });

</script>

<div runat="server" id="SummaryPanel">
    <%= ChangeSummaryText %>
    <a href="#" id="ShowHideDetails" style="margin-left: 5px;" runat="server">[<%= Labels.StudyDetails_History_ShowDetails %>]</a>
</div>
<div id="HistoryDetailsPanel" runat="server" class="HistoryDetailsPanel">
    <table class="ReasonSummary" cellspacing="0" cellpadding="0">
        <tr>
            <td class="HistoryDetailsLabel">
                <%= Labels.SeriesDeleteDialog_Reason %>
            </td>
            <td align="left">
                <%= GetReason(ChangeLog.Reason) %>
            </td>
        </tr>
        <tr>
            <td class="HistoryDetailsLabel">
                <%= Labels.SeriesDeleteDialog_Comment%>
            </td>
            <td align="left">
                <%= GetComment(ChangeLog.Reason) %>
            </td>
        </tr>
        <tr>
            <td class="HistoryDetailsLabel" colspan="2" nowrap="nowrap" style="padding-top: 8px;">
                <%= Labels.StudyDetails_History_SeriesDeleted %>
            </td>
        </tr>
    </table>
    <div style="border-bottom: dashed 1px #999999; margin-top: 3px;">
    </div>
    <div style="padding: 2px;">
    <table width="100%" cellspacing="0">
        <tr style="color: #205F87; background: #eeeeee; padding-top: 2px;">
            <td>
                <b><%= ColumnHeaders.SeriesDescription %></b>
            </td>
            <td>
                <b><%= ColumnHeaders.Modality%></b>
            </td>
            <td>
                <b><%= ColumnHeaders.Instances%></b>
            </td>
        </tr>
        <% foreach (SeriesInformation series in ChangeLog.Series)
           {%>
        <tr style="background: #fefefe">
            <td style="border-bottom: solid 1px #dddddd">
                <%= series.SeriesDescription %>
            </td>
            <td style="border-bottom: solid 1px #dddddd">
                <%= series.Modality %>
            </td>
            <td style="border-bottom: solid 1px #dddddd">
                <%= series.NumberOfInstances %>
            </td>
        </tr>
        <% }%>
    </table>
    </div>
</div>