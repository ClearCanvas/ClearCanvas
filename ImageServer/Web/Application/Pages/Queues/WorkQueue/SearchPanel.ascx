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
<%@ Import Namespace="ClearCanvas.ImageServer.Web.Common.WebControls.UI" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchPanel.ascx.cs"
	Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.SearchPanel" %>
<%@ Register Src="WorkQueueItemList.ascx" TagName="WorkQueueItemList" TagPrefix="localAsp" %>
<asp:UpdatePanel ID="SearchUpdatePanel" runat="server" UpdateMode="Conditional">
	<ContentTemplate>
		<script type="text/Javascript">
			Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(MultiSelect);
			Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(InputHover);

			function MultiSelect() {
				$("#<%=TypeListBox.ClientID %>").multiSelect({
					noneSelected: '',
					oneOrMoreSelected: '*'
				});
				$("#<%=StatusListBox.ClientID %>").multiSelect({
					noneSelected: '',
					oneOrMoreSelected: '*'
				});
			}
		</script>
		<asp:Table ID="Table" runat="server" Width="100%" CellPadding="0" CellSpacing="0"
			BorderWidth="0px">
			<asp:TableRow>
				<asp:TableCell>
					<asp:Panel ID="Panel6" runat="server" CssClass="SearchPanelContent" DefaultButton="SearchButton">
						<div class="SearchPanelParentDiv">
							<div class="SearchPanelChildDiv">
								<asp:Label ID="Label3" runat="server" Text="<%$Resources: SearchFieldLabels, PatientName%>"
									CssClass="SearchTextBoxLabel" EnableViewState="False" /><br />
								<ccUI:TextBox ID="PatientName" runat="server" CssClass="SearchTextBox" ToolTip="<%$Resources: Tooltips, SearchByPatientName %>" />
							</div>
							<div class="SearchPanelChildDiv">
								<asp:Label ID="Label2" runat="server" Text="<%$Resources: SearchFieldLabels, PatientID%>"
									CssClass="SearchTextBoxLabel" EnableViewState="False" /><br />
								<ccUI:TextBox ID="PatientId" runat="server" CssClass="SearchTextBox" ToolTip="<%$Resources: Tooltips, SearchByPatientID %>" />
							</div>
							<div class="SearchPanelChildDivCancelX">
								<asp:Label ID="Label1" runat="server" Text="<%$Resources: SearchFieldLabels, WorkQueueScheduledDate%>"
									CssClass="SearchTextBoxLabel" />&nbsp;&nbsp;
								<asp:LinkButton ID="ClearScheduleDateButton" runat="server" Text="X" CssClass="SmallLink" /><br />
								<ccUI:TextBox ID="ScheduleDate" runat="server" ReadOnly="true" CssClass="SearchDateBox"
									ToolTip="<%$Resources: Tooltips,  SearchByScheduledDate %>" />
							</div>
							<div class="SearchPanelChildDiv">
								<asp:Label ID="Label5" runat="server" Text="<%$Resources: SearchFieldLabels, WorkQueueType%>"
									CssClass="SearchTextBoxLabel" EnableViewState="False" /><br />
								<asp:ListBox ID="TypeListBox" runat="server" SelectionMode="Multiple" />
							</div>
							<div class="SearchPanelChildDiv">
								<asp:Label ID="Label6" runat="server" Text="<%$Resources: SearchFieldLabels, WorkQueueStatus%>"
									CssClass="SearchTextBoxLabel" EnableViewState="False" /><br />
								<asp:ListBox ID="StatusListBox" runat="server" SelectionMode="Multiple" />
							</div>
							<div class="SearchPanelChildDiv">
								<asp:Label ID="Label7" runat="server" Text="<%$Resources: SearchFieldLabels, WorkQueuePriority%>"
									CssClass="SearchTextBoxLabel" EnableViewState="False" /><br />
								<asp:DropDownList ID="PriorityDropDownList" runat="server" CssClass="SearchDropDownList" />
							</div>
							<div class="SearchPanelChildDiv">
								<asp:Label ID="Label4" runat="server" Text="<%$Resources: SearchFieldLabels, WorkQueueProcessingServer%>"
									CssClass="SearchTextBoxLabel" EnableViewState="False" /><br />
								<ccUI:TextBox ID="ProcessingServer" runat="server" CssClass="SearchTextBox" ToolTip="<%$Resources: Tooltips, SearchByProcessingServer %>" />
							</div>
							<div class="SearchPanelChildDiv">
								<asp:Panel ID="Panel1" runat="server" CssClass="SearchButtonPanel">
									<ccUI:ToolbarButton ID="SearchButton" runat="server" SkinID="<%$Image:SearchIcon%>"
										OnClick="SearchButton_Click" /></asp:Panel>
							</div>
						</div>
					</asp:Panel>
					<ccUI:CalendarExtender ID="ScheduleCalendarExtender" runat="server" TargetControlID="ScheduleDate"
						CssClass="Calendar">
					</ccUI:CalendarExtender>
				</asp:TableCell>
			</asp:TableRow>
			<asp:TableRow>
				<asp:TableCell>
					<table width="100%" cellpadding="3" cellspacing="0" class="ToolbarButtonPanel">
						<tr>
							<td>
								<asp:UpdatePanel ID="ToolBarUpdatePanel" runat="server" UpdateMode="Conditional">
									<ContentTemplate>
										<asp:Panel ID="ToolbarButtons" runat="server" CssClass="ToolbarButtons" Style="position: relative;">
											<ccUI:ToolbarButton ID="ViewItemDetailsButton" runat="server" SkinID="<%$Image:ViewDetailsButton%>"
												Roles='<%= ClearCanvas.ImageServer.Common.Authentication.AuthorityTokens.WorkQueue.View %>' />
											<ccUI:ToolbarButton ID="RescheduleItemButton" runat="server" SkinID="<%$Image:RescheduleButton%>"
												OnClick="RescheduleItemButton_Click" Roles='<%= ClearCanvas.ImageServer.Common.Authentication.AuthorityTokens.WorkQueue.Reschedule %>' />
											<ccUI:ToolbarButton ID="ResetItemButton" runat="server" SkinID="<%$Image:ResetButton%>"
												OnClick="ResetItemButton_Click" Roles='<%= ClearCanvas.ImageServer.Common.Authentication.AuthorityTokens.WorkQueue.Reset %>' />
											<ccUI:ToolbarButton ID="DeleteItemButton" runat="server" SkinID="<%$Image:DeleteButton%>"
												OnClick="DeleteItemButton_Click" Roles='<%= ClearCanvas.ImageServer.Common.Authentication.AuthorityTokens.WorkQueue.Delete %>' />
											<ccUI:ToolbarButton ID="ReprocessItemButton" runat="server" SkinID="<%$Image:ReprocessButton%>"
												OnClick="ReprocessItemButton_Click" Roles='<%= ClearCanvas.ImageServer.Common.Authentication.AuthorityTokens.WorkQueue.Reprocess %>' />
										</asp:Panel>
									</ContentTemplate>
								</asp:UpdatePanel>
							</td>
						</tr>
						<tr>
							<td>
								<asp:Panel ID="Panel2" runat="server" CssClass="SearchPanelResultContainer">
									<table class="SearchPanelResultContainerTable">
										<tr>
											<td>
												<ccAsp:GridPager ID="GridPagerTop" runat="server" />
											</td>
										</tr>
										<tr>
											<td class="SearchPanelResultContainerTd">
												<localAsp:WorkQueueItemList ID="workQueueItemList" Height="500px" runat="server"/>
											</td>
										</tr>
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
<ccAsp:MessageBox runat="server" ID="MessageBox" />
