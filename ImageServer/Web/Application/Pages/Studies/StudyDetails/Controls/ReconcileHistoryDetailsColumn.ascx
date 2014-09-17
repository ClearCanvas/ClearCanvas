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


<%@ Import namespace="ClearCanvas.ImageServer.Core.Edit"%>
<%@ Import Namespace="ClearCanvas.ImageServer.Core.Data" %>
<%@ Import Namespace="ClearCanvas.ImageServer.Web.Common.Utilities" %>
<%@ Import Namespace="ClearCanvas.Dicom.Utilities.Command" %>
<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ReconcileHistoryDetailsColumn.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.ReconcileHistoryDetailsColumn" %>
<%@ Import Namespace="ClearCanvas.Dicom"%>
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

<div id="HistoryDetailsPanel" runat="server" class="TallHistoryDetailsPanel">
    <table  width="100%">
    <tr>
        <td style="border-bottom:none; padding-bottom:5px" valign="top">
            <table border="0" width="100%">
                <tr><td class="HistoryDetailsLabel" style="border-bottom:dashed 1px #c0c0c0;"><%= Labels.StudyDetails_History_Reconcile_StudySnapshot %>:</td></tr>
                <tr><td style="border:none">
                    <div>
                        <table cellpadding="0" cellspacing="0">
                            <tr><td style="border-bottom:none"><%= HtmlUtility.Encode(DicomTagDictionary.GetDicomTag(DicomTags.PatientId).Name) %></td><td style="border-bottom:none"><pre style="padding-left:10px"><%# ReconcileHistory.ExistingStudy.PatientInfo.PatientId %></pre></td></tr>
                            <tr><td style="border-bottom:none"><%= HtmlUtility.Encode(DicomTagDictionary.GetDicomTag(DicomTags.IssuerOfPatientId).Name) %></td><td style="border-bottom:none"><pre style="padding-left:10px"><%# ReconcileHistory.ExistingStudy.PatientInfo.IssuerOfPatientId %></pre></td></tr>
                            <tr><td style="border-bottom:none"><%= HtmlUtility.Encode(DicomTagDictionary.GetDicomTag(DicomTags.PatientsName).Name) %></td><td style="border-bottom:none"><pre style="padding-left:10px"><%# ReconcileHistory.ExistingStudy.PatientInfo.Name %></pre></td></tr>
                            <tr><td style="border-bottom:none"><%= HtmlUtility.Encode(DicomTagDictionary.GetDicomTag(DicomTags.PatientsBirthDate).Name) %></td><td style="border-bottom:none"><pre style="padding-left:10px"><%# ReconcileHistory.ExistingStudy.PatientInfo.PatientsBirthdate %></pre></td></tr>
                            <tr><td style="border-bottom:none"><%= HtmlUtility.Encode(DicomTagDictionary.GetDicomTag(DicomTags.PatientsSex).Name) %></td><td style="border-bottom:none"><pre style="padding-left:10px"><%# ReconcileHistory.ExistingStudy.PatientInfo.Sex %></pre></td></tr>
                            <tr><td style="border-bottom:none"><%= HtmlUtility.Encode(DicomTagDictionary.GetDicomTag(DicomTags.AccessionNumber).Name) %></td><td style="border-bottom:none"><pre style="padding-left:10px"><%# ReconcileHistory.ExistingStudy.AccessionNumber %></pre></td></tr>
                            <tr><td style="border-bottom:none"><%= HtmlUtility.Encode(DicomTagDictionary.GetDicomTag(DicomTags.StudyDate).Name) %></td><td style="border-bottom:none"><pre style="padding-left:10px"><%# ReconcileHistory.ExistingStudy.StudyDate %></pre></td></tr>
                        </table>
                    </div>
                </td></tr>
            </table>
        </td>
        <td style="border-bottom:none" valign="top">
            <table border="0" width="100%">
                <tr><td class="HistoryDetailsLabel" style="border-bottom:dashed 1px #c0c0c0;"><%= Labels.StudyDetails_History_Reconcile_Snapshot%><td></tr>
                <tr><td style="border:none">
                <div>
                    <%
                        if (ReconcileHistory.ImageSetData.Fields == null ||
                            ReconcileHistory.ImageSetData.Fields.Length == 0)
                        {%>
                        <%= Resources.SR.NotAvailable %>
                    <% }
                        else
                        { %>
                        
                        <table cellpadding="0" cellspacing="0">                        
                            <% foreach (ImageSetField field in ReconcileHistory.ImageSetData.Fields)
                               {%>
                                    <tr>
                                        <td style="border-bottom:none"><%= HtmlUtility.Encode(field.DicomTag.Name)%></td>
                                        <td style="border-bottom:none"><pre style="padding-left:10px"><%=HtmlUtility.Encode(field.Value)%></pre></td>
                                    </tr>
                             <% }%>
                         </table>
                         <%} %>
                 </div>
                </td></tr>
            </table>        
        </td>
    </tr>
    <tr>
        <td colspan="2" style="border-top:solid 1px #cccccc; padding-top:3px;">
            <% if (!ReconcileHistory.Automatic) { %>
                    <asp:Label runat="server" CssClass="HistoryDetailsLabel" Text="<%# PerformedBy %>"></asp:Label>
            <% } %>
        </td>
    </tr>
    <tr>
        <td colspan="2" style="border-top:solid 1px #cccccc; padding-top:3px;">
            <asp:Label ID="Label1" runat="server" CssClass="HistoryDetailsLabel"><%= Labels.StudyDetails_History_Reconcile_ChangesApplied %></asp:Label>
            <div style="padding-top:5px;">
                <table width="100%" cellspacing="0" >
                            <tr style="color: #205F87; background: #eeeeee; padding-top: 2px; ">
                                <td style="border-top:dashed 1px #c0c0c0;"><b><%= ColumnHeaders.Tag%></b></td>
                                <td style="border-top:dashed 1px #c0c0c0;"><b><%= ColumnHeaders.OldValue%></b></td>
                                <td style="border-top:dashed 1px #c0c0c0;"><b><%= ColumnHeaders.NewValue%></b></td>
                            </tr>
                            <%{
                                  foreach (BaseImageLevelUpdateCommand theCmd in ReconcileHistory.Commands)
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

