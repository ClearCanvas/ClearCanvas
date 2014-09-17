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
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompressionHistory.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.CompressionHistory" %>

<script type="text/javascript">

	$(document).ready(function () {
		$("#<%=HistoryDetailsPanel.ClientID%>").hide();
    	$("#<%=ShowHideDetails.ClientID%>").click(function () {
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
    <%= SummaryText %>
    <a href="#" id="ShowHideDetails" style="margin-left: 5px;" runat="server">[<%= Labels.StudyDetails_History_ShowDetails %>]</a>
</div>
<div id="HistoryDetailsPanel" runat="server" class="HistoryDetailsPanel">
    <table class="ReasonSummary" cellspacing="0" cellpadding="0">
        <tr>
            <td class="HistoryDetailsLabel">
                <%= Labels.OriginalTransferSyntax %>
            </td>
            <td align="left">
                <%= OriginalTransferSyntax %>
            </td>
        </tr>
        <tr>
            <td class="HistoryDetailsLabel">
                <%= Labels.FinalTransferSyntax%>
            </td>
            <td align="left">
                <%= FinalTransferSyntax %>
            </td>
        </tr>
    </table>
</div>
