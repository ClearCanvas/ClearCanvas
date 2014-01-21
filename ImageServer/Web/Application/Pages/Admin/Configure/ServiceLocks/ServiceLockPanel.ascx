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
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ServiceLockPanel.ascx.cs"
	Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServiceLocks.ServiceLockPanel" %>
<%@ Register Src="ServiceLockGridView.ascx" TagName="ServiceLockGridView" TagPrefix="localAsp" %>
<%@ Register Src="EditServiceLockDialog.ascx" TagName="EditServiceLockDialog" TagPrefix="localAsp" %>
<asp:UpdatePanel ID="SearchUpdatePanel" runat="server" UpdateMode="Conditional">
	<ContentTemplate>
		<script type="text/Javascript">

			Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(MultiSelect);
			Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(InputHover);

			function MultiSelect() {

				$("#<%=FileSystemFilter.ClientID %>").multiSelect({
					noneSelected: '',
					oneOrMoreSelected: '*'
				});
				$("#<%=PartitionFilter.ClientID %>").multiSelect({
					noneSelected: '',
					oneOrMoreSelected: '*'
				});
			}
    
		</script>
		<asp:Table ID="Table" runat="server" Width="100%" CellPadding="0">
			<asp:TableRow>
				<asp:TableCell VerticalAlign="Bottom" Wrap="false">
					<asp:Panel ID="Panel6" runat="server" CssClass="SearchPanelContentWithoutTabs" DefaultButton="SearchButton">
						<div class="SearchPanelParentDiv">
							<div class="SearchPanelChildDiv">
								<asp:Label ID="Label1" runat="server" Text="<%$Resources: SearchFieldLabels, ServiceType %>"
									CssClass="SearchTextBoxLabel" EnableViewState="False" /><br />
								<asp:DropDownList ID="TypeDropDownList" runat="server" CssClass="SearchDropDownList" />
							</div>
							<div class="SearchPanelChildDiv">
								<asp:Label ID="Label3" runat="server" Text="<%$Resources: SearchFieldLabels, ServiceStatus %>"
									CssClass="SearchTextBoxLabel" /><br />
								<asp:DropDownList ID="StatusFilter" runat="server" CssClass="SearchDropDownList" />
							</div>
							<div class="SearchPanelChildDiv">
								<asp:Label ID="Label2" runat="server" Text="<%$Resources: SearchFieldLabels, ServiceFileSystem %>"
									CssClass="SearchTextBoxLabel" /><br />
								<asp:ListBox ID="FileSystemFilter" runat="server" CssClass="SearchDropDownList" SelectionMode="Multiple" />
							</div>
							<div class="SearchPanelChildDiv">
								<asp:Label ID="Label4" runat="server" Text="<%$Resources: SearchFieldLabels, ServicePartition %>"
									CssClass="SearchTextBoxLabel" /><br />
								<asp:ListBox ID="PartitionFilter" runat="server" CssClass="SearchDropDownList" SelectionMode="Multiple" />
							</div>
							<div class="SearchPanelChildDiv">
								<asp:Panel ID="Panel1" runat="server" CssClass="SearchButtonPanel">
									<ccUI:ToolbarButton ID="SearchButton" runat="server" SkinID="<%$Image:SearchIcon%>"
										OnClick="SearchButton_Click" />
								</asp:Panel>
							</div>
						</div>
					</asp:Panel>
				</asp:TableCell>
			</asp:TableRow>
			<asp:TableRow>
				<asp:TableCell>
					<table width="100%" cellpadding="3" cellspacing="0" class="ToolbarButtonPanel">
						<tr>
							<td>
								<asp:UpdatePanel ID="ToolbarUpdatePanel" runat="server" UpdateMode="Conditional">
									<ContentTemplate>
										<asp:Panel ID="ToolbarButtons" runat="server" CssClass="ToolbarButtons">
											<ccUI:ToolbarButton ID="EditServiceScheduleButton" runat="server" SkinID="<%$Image:EditButton%>"
												OnClick="EditServiceScheduleButton_Click" />
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
												<localAsp:ServiceLockGridView ID="ServiceLockGridViewControl" Height="500px" runat="server" />
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
<ccAsp:MessageBox ID="ConfirmEditDialog" runat="server" />
<localAsp:EditServiceLockDialog ID="EditServiceLockDialog" runat="server" />