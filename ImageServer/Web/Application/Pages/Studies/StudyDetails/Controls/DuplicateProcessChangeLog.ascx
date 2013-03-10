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



<%@ Import Namespace="ClearCanvas.ImageServer.Core.Data" %>
<%@ Import Namespace="ClearCanvas.ImageServer.Core.Edit" %>
<%@ Import Namespace="ClearCanvas.ImageServer.Web.Common.Utilities" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DuplicateProcessChangeLog.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.DuplicateProcessChangeLog" %>
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
    <%# String.Format("{0}", ChangeLogShortDescription)%>
    <a href="#" id="ShowHideDetails" style="margin-left: 5px;" runat="server">[<%= Labels.StudyDetails_History_ShowDetails %>]</a>
</div>
<div id="HistoryDetailsPanel" runat="server" class="HistoryDetailsPanel">
    <table width="100%" border="0" cellpadding="5">
        <tr>
            <td valign="top">
                <table border="0" width="100%">
                    <tr>
                        <td class="HistoryDetailsLabel">
                            <span style="padding-left: 5px"><%= Labels.StudyDetails_History_Duplicate_StudySnapshot %></span>
                        </td>
                    </tr>
                    <tr>
                        <td style="border: none">
                            <div>
                                <pre style="padding-left: 10px"><%= Labels.PatientID %>= <%= ChangeLog.StudySnapShot.PatientInfo.PatientId%></pre>
                                <pre style="padding-left: 10px"><%= Labels.IssuerOfPatientID %> = <%= ChangeLog.StudySnapShot.PatientInfo.IssuerOfPatientId%></pre>
                                <pre style="padding-left: 10px"><%= Labels.PatientName %>= <%= ChangeLog.StudySnapShot.PatientInfo.Name%></pre>
                                <pre style="padding-left: 10px"><%= Labels.PatientBirthdate %> = <%= ChangeLog.StudySnapShot.PatientInfo.PatientsBirthdate%></pre>
                                <pre style="padding-left: 10px"><%= Labels.PatientsSex %>= <%= ChangeLog.StudySnapShot.PatientInfo.Sex%></pre>
                                <pre style="padding-left: 10px"><%= Labels.AccessionNumber %> = <%= ChangeLog.StudySnapShot.AccessionNumber%></pre>
                                <pre style="padding-left: 10px"><%= Labels.StudyDate %> = <%= ChangeLog.StudySnapShot.StudyDate%></pre>
                                <div class="DuplicateDialogSeriesPanel">
                                    <table width="100%" class="DuplicateDialogSeriesTable">
                                        <tr style="background: #e0e0e0;">
                                            <td>
                                                <%= Labels.Description %>
                                            </td>
                                            <td>
                                                <%= Labels.Modality %>
                                            </td>
                                            <td>
                                                <%= Labels.Instances %>
                                            </td>
                                        </tr>
                                        <% foreach (SeriesInformation series in ChangeLog.StudySnapShot.Series)
                                           {%>
                                        <tr>
                                            <td>
                                                <%= series.SeriesDescription %>
                                            </td>
                                            <td>
                                                <%= series.Modality %>
                                            </td>
                                            <td>
                                                <%= series.NumberOfInstances %>
                                            </td>
                                        </tr>
                                        <% }%>
                                    </table>
                                </div>
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
            <td valign="top">
                <table border="0" width="100%">
                    <tr>
                        <td class="HistoryDetailsLabel">
                            <span style="padding-left: 5px"><%= Labels.StudyDetails_History_Duplicate_DuplicateImages%></span>
                        </td>
                    </tr>
                    <tr>
                        <td style="border: none">
                            <div style="margin-left: 2px;">
                                <%  if (ChangeLog.DuplicateDetails == null || ChangeLog.DuplicateDetails.StudyInfo == null)
                                    { %>
                                    <%= SR.NotAvailable %>
                                <%  }
                                    else
                                    { %>
                                <pre style="padding-left: 10px"><%= Labels.PatientID %> = <%= ChangeLog.DuplicateDetails.StudyInfo.PatientInfo.PatientId%></pre>
                                <pre style="padding-left: 10px"><%= Labels.IssuerOfPatientID %> = <%= ChangeLog.DuplicateDetails.StudyInfo.PatientInfo.IssuerOfPatientId%></pre>
                                <pre style="padding-left: 10px"><%= Labels.PatientName %>= <%= ChangeLog.DuplicateDetails.StudyInfo.PatientInfo.Name%></pre>
                                <pre style="padding-left: 10px"><%= Labels.PatientBirthdate %> = <%= ChangeLog.DuplicateDetails.StudyInfo.PatientInfo.PatientsBirthdate%></pre>
                                <pre style="padding-left: 10px"><%= Labels.PatientsSex %> = <%= ChangeLog.DuplicateDetails.StudyInfo.PatientInfo.Sex%></pre>
                                <pre style="padding-left: 10px"><%= Labels.AccessionNumber %> = <%= ChangeLog.DuplicateDetails.StudyInfo.AccessionNumber%></pre>
                                <pre style="padding-left: 10px"><%= Labels.StudyDate %> =  <%= ChangeLog.DuplicateDetails.StudyInfo.StudyDate%></pre>
                                <div class="DuplicateDialogSeriesPanel">
                                    <table width="100%" class="DuplicateDialogSeriesTable">
                                        <tr style="background: #e0e0e0;">
                                            <td>
                                                <%= Labels.Description %>
                                            </td>
                                            <td>
                                                <%= Labels.Modality %>
                                            </td>
                                            <td>
                                                <%= Labels.Instances %>
                                            </td>
                                        </tr>
                                        <% foreach (SeriesInformation series in ChangeLog.DuplicateDetails.StudyInfo.Series)
                                           {%>
                                        <tr>
                                            <td>
                                                <%= series.SeriesDescription %>
                                            </td>
                                            <td>
                                                <%= series.Modality %>
                                            </td>
                                            <td>
                                                <%= series.NumberOfInstances %>
                                            </td>
                                        </tr>
                                        <% }%>
                                    </table>
                                </div>
                                <%  } %>
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <table border="0" width="100%">
        <tr>
            <td>
                <%= String.Format(SR.StudyDetails_History_Duplicate_PerformedBy, ChangeLog.UserName ?? SR.Unknown) %>
            </td>
        </tr>
    </table>
    <table border="0" width="100%">
        <tr>
            <td class="HistoryDetailsLabel">
                <span style="margin-left: 5px"><%= Labels.StudyDetails_History_Duplicate_ChangesApplied %></span>
            </td>
        </tr>
        <tr>
            <td style="border: none">
                <div style="margin-left: 2px; padding-left: 5px;">
                    <table width="100%" cellspacing="0">
                        <tr style="color: #205F87; background: #eeeeee; padding-top: 2px;">
                            <td>
                                <b><%= ColumnHeaders.Tag %></b>
                            </td>
                            <td>
                                <b><%= ColumnHeaders.OldValue%></b>
                            </td>
                            <td>
                                <b><%= ColumnHeaders.NewValue%></b>
                            </td>
                        </tr>
                        <%{
                              foreach (BaseImageLevelUpdateCommand theCmd in ChangeLog.StudyUpdateCommands)
                              {
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
            </td>
        </tr>
    </table>
</div>
