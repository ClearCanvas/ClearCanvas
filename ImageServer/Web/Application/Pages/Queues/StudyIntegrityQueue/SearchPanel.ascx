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

<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SearchPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue.SearchPanel" %>

<%@ Register Src="StudyIntegrityQueueItemList.ascx" TagName="StudyIntegrityQueueItemList" TagPrefix="localAsp" %>

<asp:UpdatePanel ID="SearchUpdatePanel" runat="server" UpdateMode="conditional">
    <ContentTemplate>
    
<script type="text/Javascript">

Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(MultiSelect);
Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(InputHover);

function MultiSelect() {
       
        $("#<%=ReasonListBox.ClientID %>").multiSelect({
            noneSelected: '',
            oneOrMoreSelected: '*'
        });   

}
    </script>    

            <asp:Table runat="server">
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="right" VerticalAlign="Bottom" >
                    
                       <table cellpadding="0" cellspacing="0"  width="100%">
                            <!-- need this table so that the filter panel container is fit to the content -->
                            <tr>
                                <td align="left">
                                <asp:Panel ID="Panel6" runat="server" CssClass="SearchPanelContent" DefaultButton="SearchButton">
                                    <table cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label1" runat="server" Text="<%$Resources: SearchFieldLabels,PatientName %>" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False" /><br />
                                                <ccUI:TextBox ID="PatientName" runat="server" CssClass="SearchTextBox" />
                                            </td>
                                             <td align="left" valign="bottom">
                                                <asp:Label ID="Label4" runat="server" Text="<%$Resources: SearchFieldLabels,PatientID %>" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False" /><br />
                                                <ccUI:TextBox ID="PatientId" runat="server" CssClass="SearchTextBox" ToolTip="<%$Resources: Tooltips,SearchByPatientID %>" />
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label3" runat="server" Text="<%$Resources: SearchFieldLabels,AccessionNumber%>" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False" /><br />
                                                <ccUI:TextBox ID="AccessionNumber" runat="server" CssClass="SearchTextBox" />
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label2" runat="server" Text="<%$Resources: SearchFieldLabels,FromDate%>" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False" />
                                                <asp:LinkButton ID="ClearFromDateButton" runat="server" Text="X" CssClass="SmallLink" style="margin-left: 8px;"/><br />
                                                <ccUI:TextBox ID="FromDate" runat="server" CssClass="SearchDateBox" ReadOnly="true" ToolTip="<%$Resources: Tooltips,SearchByStudyDate %>"/>
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label6" runat="server" Text="<%$Resources: SearchFieldLabels,ToDate%>" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False" />
                                                <asp:LinkButton ID="ClearToDateButton" runat="server" Text="X" CssClass="SmallLink" style="margin-left: 22px;"/><br />
                                                <ccUI:TextBox ID="ToDate" runat="server" CssClass="SearchDateBox" ReadOnly="true" ToolTip="<%$Resources: Tooltips,SearchByStudyDate %>"/>
                                            </td>                                            
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label5" runat="server" Text="<%$Resources: SearchFieldLabels,SIQReason%>" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False" /><br />
                                                <asp:ListBox runat="server" id="ReasonListBox" SelectionMode="Multiple">                                             
                                                </asp:ListBox>
                                            </td>
                                            <td valign="bottom">
                                                <asp:Panel ID="Panel1" runat="server" CssClass="SearchButtonPanel"><ccUI:ToolbarButton ID="SearchButton" runat="server" SkinID="<%$Image:SearchIcon%>" OnClick="SearchButton_Click" /></asp:Panel>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                                    <ccUI:CalendarExtender ID="FromDateCalendarExtender" runat="server" TargetControlID="FromDate"
                                        CssClass="Calendar">
                                    </ccUI:CalendarExtender>
                                    <ccUI:CalendarExtender ID="ToDateCalendarExtender" runat="server" TargetControlID="ToDate"
                                        CssClass="Calendar">
                                    </ccUI:CalendarExtender>                                    
                                </td>
                            </tr>
                        </table>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>
                        <table width="100%" cellpadding="2" cellspacing="0" class="ToolbarButtonPanel">
                            <tr><td >
                            <asp:UpdatePanel ID="ToolBarUpdatePanel" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Panel ID="ToolbarButtons" runat="server" CssClass="ToolbarButtons">
                                        <ccUI:ToolbarButton ID="ReconcileButton" runat="server" SkinID="<%$Image:ReconcileButton%>" OnClick="ReconcileButton_Click" />
                                    </asp:Panel>
                             </ContentTemplate>
                          </asp:UpdatePanel>                  
                        </td></tr>
                        <tr><td>

                         <asp:Panel ID="Panel2" runat="server" CssClass="SearchPanelResultContainer">
                            <table width="100%" cellpadding="0" cellspacing="0">
                                <tr><td><ccAsp:GridPager ID="GridPagerTop" runat="server" /></td></tr>                        
                                <tr><td style="background-color: white;"><localAsp:StudyIntegrityQueueItemList id="StudyIntegrityQueueItemList" runat="server" Height="500px"></localAsp:StudyIntegrityQueueItemList></td></tr>
                            </table>                        
                        </asp:Panel>
                        </td>
                        </tr>
                        </table>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>

    </ContentTemplate>
</asp:UpdatePanel>
