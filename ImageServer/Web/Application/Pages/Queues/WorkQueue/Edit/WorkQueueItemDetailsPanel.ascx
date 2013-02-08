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


<%@ Control Language="C#" AutoEventWireup="true" Codebehind="WorkQueueItemDetailsPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.WorkQueueItemDetailsPanel" %>

<%@ Register Src="~/Pages/Queues/WorkQueue/WorkQueueAlertPanel.ascx" TagName="WorkQueueAlertPanel" TagPrefix="localAsp" %>


<script type="text/javascript">
    Sys.Application.add_load(function(){
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_initializeRequest(function(){
            $("#<%= AutoRefreshIndicator.ClientID %>").hide();
        });
        prm.add_endRequest(function(){
            $("#<%= AutoRefreshIndicator.ClientID %>").show();
        });
    });
    
</script>
<asp:Panel ID="Panel1" runat="server">
     <asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>   
         
           <table width="100%" cellpadding="0" cellspacing="0">
                <tr>
                    <td class="MainContentTitle">
                        <asp:Label ID="WorkQueueItemTitle" runat="server" Text="<%$Resources: Titles, WorkQueueItemDetails %>"></asp:Label>
                    </td>
                    <td align="right" class="MainContentTitle">                        
                         <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel" DisplayAfter="0">
                            <ProgressTemplate>
                                <asp:Image ID="RefreshingIndicator" runat="server" SkinID="AjaxLoadingBlue" />
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                        <asp:Image ID="AutoRefreshIndicator" runat="server" SkinID="RefreshEnabled" />
                            
                    </td>
                </tr>
                <tr runat="server" id="WorkQueueAlertPanelRow"><td colspan="2"><localAsp:WorkQueueAlertPanel runat="server" ID="WorkQueueAlertPanel" /></td>
                </tr>
                <tr><td colspan="2">
                        <table width="100%" cellpadding="2" cellspacing="0" class="ToolbarButtonPanel" >
                            <tr><td style="padding-top: 5px; padding-left: 5px;">
                                    <ccUI:ToolbarButton ID="RescheduleToolbarButton" runat="server" SkinID="<%$Image:RescheduleButton%>" OnClick="Reschedule_Click"/>
                                    <ccUI:ToolbarButton ID="ResetButton" runat="server" SkinID="<%$Image:ResetButton%>" OnClick="Reset_Click"/>
                                    <ccUI:ToolbarButton ID="DeleteButton" runat="server" SkinID="<%$Image:DeleteButton%>" OnClick="Delete_Click"/>
                                    <ccUI:ToolbarButton ID="ReprocessButton" runat="server" SkinID="<%$Image:ReprocessButton%>" OnClick="Reprocess_Click"/>
                                    <ccUI:ToolbarButton ID="StudyDetailsButton" runat="server" SkinID="<%$Image:ViewStudyButton%>" />      
                            </td></tr>
                            <tr><td><asp:PlaceHolder ID="WorkQueueDetailsViewPlaceHolder" runat="server"></asp:PlaceHolder></td></tr>
                       </table>
                  </td></tr>
              </table>
              
              
            <ccUI:Timer ID="RefreshTimer" runat="server" Interval="10000" OnTick="RefreshTimer_Tick" OnAutoDisabled="OnAutoRefreshDisabled" DisableAfter="3"></ccUI:Timer>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Panel>
