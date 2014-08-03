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
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ServerRulePanel.ascx.cs"
	Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerRules.ServerRulePanel" %>
<%@ Register Src="ServerRuleGridView.ascx" TagName="ServerRuleGridView" TagPrefix="localAsp" %>
<asp:UpdatePanel ID="SearchUpdatePanel" runat="server" UpdateMode="Conditional">
	<ContentTemplate>
		<script>
			Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(InputHover);
		</script>
		<asp:Table ID="Table" runat="server" Width="100%" CellPadding="2" Style="border-color: #6699CC"
			BorderWidth="2px">
			<asp:TableRow>
				<asp:TableCell VerticalAlign="Bottom" Wrap="false">
					<asp:Panel ID="Panel6" runat="server" CssClass="SearchPanelContent" DefaultButton="SearchButton">
						<div class="SearchPanelParentDiv">
							<div class="SearchPanelChildDiv">
								<asp:Label ID="Label1" runat="server" Text="<%$Resources: SearchFieldLabels, ServerRuleType %>"
									CssClass="SearchTextBoxLabel" EnableViewState="False" /><br />
								<asp:DropDownList ID="RuleTypeDropDownList" runat="server" CssClass="SearchDropDownList"
									ToolTip="<%$Resources: Tooltips, SearchByType %>" />
							</div>
							<div class="SearchPanelChildDiv">
								<asp:Label ID="Label2" runat="server" Text="<%$Resources: SearchFieldLabels, ServerRuleApplyType %>"
									CssClass="SearchTextBoxLabel" EnableViewState="False"></asp:Label><br />
								<asp:DropDownList ID="RuleApplyTimeDropDownList" runat="server" CssClass="SearchDropDownList"
									ToolTip="<%$Resources: Tooltips, SearchByApplyTime %>" />
							</div>
							<div class="SearchPanelChildDiv">
								<asp:Label ID="Label3" runat="server" Text="<%$Resources: SearchFieldLabels, ServerRuleStatus %>"
									CssClass="SearchTextBoxLabel"></asp:Label><br />
								<asp:DropDownList ID="StatusFilter" runat="server" CssClass="SearchDropDownList">
								</asp:DropDownList>
							</div>
							<div class="SearchPanelChildDiv">
								<asp:Label ID="Label4" runat="server" Text="<%$Resources: SearchFieldLabels, ServerRuleDefault %>"
									CssClass="SearchTextBoxLabel"></asp:Label><br />
								<asp:DropDownList ID="DefaultFilter" runat="server" CssClass="SearchDropDownList">
								</asp:DropDownList>
							</div>
							<div class="SearchPanelChildDiv">
								<asp:Panel ID="Panel2" runat="server" CssClass="SearchButtonPanel">
									<ccUI:ToolbarButton ID="SearchButton" runat="server" SkinID="<%$Image:SearchIcon%>"
										OnClick="SearchButton_Click" /></asp:Panel>
							</div>
						</div>
					</asp:Panel>
				</asp:TableCell>
			</asp:TableRow>
			<asp:TableRow>
				<asp:TableCell>
					<table width="100%" class="ToolbarButtonPanel">
						<tr>
							<td>
								<asp:UpdatePanel ID="ToolbarUpdatePanel" runat="server" UpdateMode="Conditional">
									<ContentTemplate>
										<asp:Panel ID="ToolbarButtons" runat="server" CssClass="ToolbarButtons">
											<ccUI:ToolbarButton ID="AddServerRuleButton" runat="server" SkinID="<%$Image:AddButton%>"
												OnClick="AddServerRuleButton_Click" />
											<ccUI:ToolbarButton ID="EditServerRuleButton" runat="server" SkinID="<%$Image:EditButton%>"
												OnClick="EditServerRuleButton_Click" />
											<ccUI:ToolbarButton ID="DeleteServerRuleButton" runat="server" SkinID="<%$Image:DeleteButton%>"
												OnClick="DeleteServerRuleButton_Click" />
										</asp:Panel>
									</ContentTemplate>
								</asp:UpdatePanel>
							</td>
						</tr>
						<tr>
							<td>
								<asp:Panel ID="Panel1" runat="server" CssClass="SearchPanelResultContainer">
									<table class="SearchPanelResultContainerTable">
										<tr>
											<td>
												<ccAsp:GridPager ID="GridPagerTop" runat="server" />
											</td>
										</tr>
										<tr>
											<td class="SearchPanelResultContainerTd">
												<localAsp:ServerRuleGridView ID="ServerRuleGridViewControl" runat="server" Height="500px" />
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
