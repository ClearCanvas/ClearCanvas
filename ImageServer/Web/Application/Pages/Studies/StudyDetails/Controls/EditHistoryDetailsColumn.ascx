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



<%@ Import Namespace="ClearCanvas.ImageServer.Core.Edit" %>
<%@ Import Namespace="ClearCanvas.ImageServer.Web.Common.Utilities" %>
<%@ Import Namespace="ClearCanvas.Dicom.Utilities.Command" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditHistoryDetailsColumn.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.EditHistoryDetailsColumn" %>
<%@ Import Namespace="Resources"%>

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

<div id="SummaryPanel" runat="server">
    <%= ChangeSummaryText %>
    <a href="#" id="ShowHideDetails" style="margin-left: 5px;" runat="server">[<%= Labels.StudyDetails_History_ShowDetails %>]</a>
</div>

<div id="HistoryDetailsPanel" runat="server" class="HistoryDetailsPanel">
    <table class="ReasonSummary" cellspacing="0" cellpadding="0">
        <tr>
            <td class="HistoryDetailsLabel">
                <%= Labels.StudyDetails_History_Reason %>
            </td>
            <td align="left">
                <%= GetReason(EditHistory.Reason) %>
            </td>
        </tr>
        <tr>
            <td class="HistoryDetailsLabel">
                <%= Labels.StudyDetails_History_Comment %>
            </td>
            <td align="left">
                <%= GetComment(EditHistory.Reason) %>
            </td>
        </tr>
        <tr>
            <td class="HistoryDetailsLabel" style="padding-top: 8px;">
                <%= Labels.StudyDetails_History_Changes %>
            </td>
            <td align="left">
            </td>
        </tr>
    </table>
    <div style="border-bottom: dashed 1px #999999; margin-top: 3px;">
    </div>

                    <% if (EditHistory.UpdateCommands == null || EditHistory.UpdateCommands.Count == 0)
                       {%>
    <table class="ChangeHistorySummary" width="100%" cellspacing="0" cellpadding="0">
        <tr>
            <td>
                    <pre style="padding-left: 10px"><%= SR.StudyDetails_StudyWasNoChanged %></pre>
            </td>
        </tr>
   </table>
                    <%}
                       else
                       {%>
                    <div style="padding: 2px;">                       
                    <table width="100%" cellspacing="0" >
                        <tr style="color: #205F87; background: #eeeeee; padding-top: 2px;">
                            <td>
                                <b><%= ColumnHeaders.Tag%></b>
                            </td>
                            <td>
                                <b><%= ColumnHeaders.OldValue%></b>
                            </td>
                            <td>
                                <b><%= ColumnHeaders.NewValue%></b>
                            </td>
                        </tr>
                        <%{
                              foreach (BaseImageLevelUpdateCommand cmd in EditHistory.UpdateCommands)
                              {
                                  IUpdateImageTagCommand theCmd = cmd as IUpdateImageTagCommand;
                                  if (theCmd != null)
                                  { %><tr style="background: #fefefe">
                                                      <td style="border-bottom: solid 1px #dddddd">
                                                          <pre><%= HtmlUtility.Encode(theCmd.UpdateEntry.TagPath.Tag) %></pre>
                                                      </td>
                                                      <td style="border-bottom: solid 1px #dddddd">
                                                          <pre><%= HtmlUtility.Encode(theCmd.UpdateEntry.OriginalValue) %></pre>
                                                      </td>
                                                      <td style="border-bottom: solid 1px #dddddd">
                                                          <pre><%= HtmlUtility.Encode(theCmd.UpdateEntry.Value!=null? theCmd.UpdateEntry.Value.ToString(): "") %></pre>
                                                      </td>
                                                  </tr>
                                <%} %>
                            <%}%>
                        <%}%>
                    </table>
                    </div>
                    <%}%>
</div>
