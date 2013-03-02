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
0
--%>

<%@ Page Language="C#" MasterPageFile="~/GlobalMasterPage.master" AutoEventWireup="true"
    EnableEventValidation="false" Codebehind="Default.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Default"
    Title="Work Queue | ClearCanvas ImageServer" %>
  
<%@ MasterType  virtualPath="~/GlobalMasterPage.master"%>    

<%@ Register Src="Edit/ScheduleWorkQueueDialog.ascx" TagName="ScheduleWorkQueueDialog" TagPrefix="localAsp" %>
<%@ Register Src="Edit/ResetWorkQueueDialog.ascx" TagName="ResetWorkQueueDialog"    TagPrefix="localAsp" %>        
<%@ Register Src="Edit/DeleteWorkQueueDialog.ascx" TagName="DeleteWorkQueueDialog"    TagPrefix="localAsp" %>        
<%@ Register Src="SearchPanel.ascx" TagName="SearchPanel" TagPrefix="localAsp" %>

<asp:Content ID="ContentTitle" ContentPlaceHolderID="MainContentTitlePlaceHolder" runat="server">

<script type="text/javascript">
    function KeyPress() {
        var isEnter = window.event == null ? e.keyCode == 13 : window.event.keyCode == 13;
        if(isEnter) {
            __doPostBack('<%=RefreshRateTextBox.ClientID %>','<%=RefreshRateTextBox.Text %>');     
        }   
    }
    function Blur() {
        __doPostBack('<%=RefreshRateTextBox.ClientID %>','<%=RefreshRateTextBox.Text %>');     
    }
</script>

<asp:UpdatePanel runat="server">
    <ContentTemplate>
    <asp:Panel ID="Panel1" runat="server" style="position: relative">
        <table width="100%" cellspacing="0" cellpadding="0">
            <tr>
                <td><asp:Literal ID="Literal1" runat="server" Text="<%$Resources:Titles,WorkQueue%>" /></td>
                <td>
                    <div class="RefreshContainer">
                        <table>
                            <tr>
                                <td>
                                    <span class="SearchTextBoxLabel" style="color: white"><%= Labels.Refresh %>:</span>
                                </td>
                                <td>
                                    <asp:DropDownList ID="RefreshRateEnabled" runat="server" CssClass="SearchDropDownList" OnSelectedIndexChanged="RefreshRate_IndexChanged" AutoPostBack="true">
                                        <asp:ListItem Selected="True" Value="Y" Text="<%$Resources: Labels,Yes %>"/>
                                        <asp:ListItem Value="N" Selected="True" Text="<%$Resources: Labels,No %>"/>
                                    </asp:DropDownList> 
                                </td>
                                <td>
                                    <asp:Panel runat="server" ID="RefreshIntervalPanel">
                                    <asp:TextBox ID="RefreshRateTextBox" runat="server" Width="30" Text="20" CssClass="SearchTextBoxLabel" onkeypress="javascript: KeyPress()" onblur="javascript: Blur()" style="padding-left: 2px;" />
                                    <span class="SearchTextBoxLabel" style="color: white">s</span>
                                    </asp:Panel>
                                </td>
                            </tr>
                        </table>
                    </div> 
                </td>
            </tr>
        </table>
    </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>

</asp:Content>
  
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:UpdatePanel ID="PageContent" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <ccAsp:ServerPartitionSelector runat="server" ID="ServerPartitionSelector" Visible="true" />
            <localAsp:SearchPanel runat="server" id="SearchPanel" visible="true" />
                      
            <asp:Label ID="Label1" runat="server" Style="left: 70px; position: relative;" Text="Label"
                                Visible="False" Width="305px"></asp:Label>
                                
            <ccAsp:MessageBox runat="server" ID="MessageBox"/>
            <ccAsp:MessageBox runat="server" ID="ConfirmRescheduleDialog"/>
            <ccAsp:MessageBox runat="server" ID="InformationDialog" MessageType="INFORMATION" Title=""/>
            <localAsp:ScheduleWorkQueueDialog runat="server" ID="ScheduleWorkQueueDialog"/>
            <localAsp:ResetWorkQueueDialog ID="ResetWorkQueueDialog" runat="server" OnError="OnResetWorkQueueError"/>
            <localAsp:DeleteWorkQueueDialog ID="DeleteWorkQueueDialog" runat="server" />
            <!-- the timer should be inside the update panel so that it doesn't continue running while postback is happening -->
            <ccUI:Timer ID="RefreshTimer" runat="server" OnTick="RefreshTimer_Tick" DisableAfter="15"></ccUI:Timer>
        </ContentTemplate>
    </asp:UpdatePanel>  
</asp:Content>
