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

<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ScheduleWorkQueueDialog.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.ScheduleWorkQueueDialog" %>

<%@ Register Src="WorkQueueSettingsPanel.ascx" TagName="WorkQueueSettingsPanel" TagPrefix="localAsp" %>
<%@ Register Src="../WorkQueueItemList.ascx" TagName="WorkQueueItemList" TagPrefix="localAsp" %>

<ccAsp:ModalDialog id="ModalDialog" runat="server" title="<%$Resources: Titles,ScheduleWorkQueueDialogTitle %>" Width="900px">
<ContentTemplate>
   
    <asp:Panel ID="Panel1" runat="server" CssClass="DialogPanelContent" width="100%">
        
        <localAsp:WorkQueueItemList ID="SelectedWorkQueueItemList" runat="server" OnDataSourceCreated="SetDataSourceFilter"/>
            
        <asp:Panel runat="server" style="border-top: solid 1px #CCCCCC; padding-top: 3px; text-align: center; padding-top: 5px; padding-bottom: 5px;">
            <localAsp:WorkQueueSettingsPanel  ID="WorkQueueSettingsPanel" runat="server" />           
        </asp:Panel>
       
    </asp:Panel>
            <table cellpadding="0" cellspacing="0" width="100%">
                <tr align="right">
                    <td>
                        <asp:Panel ID="Panel5" runat="server" CssClass="DefaultModalDialogButtonPanel">
                            <ccUI:ToolbarButton ID="OKButton" runat="server" SkinID="<%$Image:UpdateButton%>" OnClick="OnApplyButtonClicked" />
                            <ccUI:ToolbarButton ID="Cancel" runat="server" SkinID="<%$Image:CancelButton%>" OnClick="OnCancelButtonClicked" />
                        </asp:Panel>
                    </td>
                </tr>
            </table>
          
    
</ContentTemplate>
</ccAsp:ModalDialog>
<ccAsp:MessageBox id="PreOpenConfirmDialog" runat="server" />
<ccAsp:MessageBox id="PreApplyChangeConfirmDialog" runat="server" />
<ccAsp:MessageBox id="MessageDialog" runat="server" />
