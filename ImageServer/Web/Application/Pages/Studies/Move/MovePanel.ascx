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
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MovePanel.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.Move.MovePanel" %>
<%@ Import Namespace="Resources" %>
<%@ Register Src="DeviceGridView.ascx" TagName="DeviceGridView" TagPrefix="localAsp" %>
<%@ Register Src="StudyGridView.ascx" TagName="StudyGridView" TagPrefix="localAsp" %>
<asp:UpdatePanel ID="SearchUpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>

        <script>
            Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(InputHover);
            Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(MultiSelect);

            function MultiSelect() {
                $("#<%=DeviceTypeFilter.ClientID %>").multiSelect({
                    noneSelected: '',
                    oneOrMoreSelected: '*'
                });
            }
        </script>

        <asp:Panel runat="server" DefaultButton="SearchButton">
            <table cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td class="SeriesDetailsContent">
                        <table width="100%" cellpadding="2" cellspacing="0" class="ToolbarButtonPanel">
                            <tr>
                                <td class="MainContentSubTitle" style="vertical-align: bottom; padding-top: 5px;">
                                    <asp:Literal runat="server" Text="<%$Resources:Titles,Studies%>" />
                                </td>
                                <td align="right" valign="bottom">
                                    <asp:Panel CssClass="ToolbarButtons" Style="padding-right: 4px;" runat="server">
                                        <ccUI:ToolbarButton runat="server" SkinID="<%$Image:MoveButton%>" ID="MoveButton" OnClick="MoveButton_Click" />
                                        <ccUI:ToolbarButton runat="server" SkinID="<%$Image:DoneButton%>" ID="DoneButton" OnClientClick="self.close();" />
                                    </asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <asp:Panel ID="Panel1" runat="server" CssClass="MovePanel">
                                        <localAsp:StudyGridView ID="StudyGridPanel" runat="server" />
                                    </asp:Panel>
                                </td>
                            </tr>
                        </table>
                </tr>
                <tr>
                    <td class="MovePanelTd">
                        <asp:Image ID="Image2" runat="server" SkinID="Spacer" Height="4" />
                    </td>
                </tr>
                <tr>
                    <td class="ToolbarLabelPanel">
                        <%=Labels.DestinationDevices %>
                    </td>
                </tr>
                <tr>
                    <td class="MovePanelTd">
                        <table cellpadding="2" cellspacing="0" class="ToolbarButtonPanel" width="100%">
                            <tr>
                                <td style="padding-top: 5px; vertical-align: bottom;">
                                    <asp:Label runat="server" Text="<%$Resources: SearchFieldLabels, AETitle%>" CssClass="SearchTextBoxLabel" /><br />
                                    <ccUI:TextBox ID="AETitleFilter" runat="server" CssClass="SearchTextBox" ToolTip="<%$ Resources: Tooltips, SearchByAETitle %>" />
                                </td>
                                <td style="vertical-align: bottom;">
                                    <asp:Label runat="server" Text="<%$Resources: SearchFieldLabels, DeviceDescription %>" CssClass="SearchTextBoxLabel" /><br />
                                    <ccUI:TextBox ID="DescriptionFilter" runat="server" CssClass="SearchTextBox" ToolTip="<%$ Resources: Tooltips, SearchByAeDescription %>" />
                                </td>
                                <td style="vertical-align: bottom;">
                                    <asp:Label runat="server" Text="<%$Resources: SearchFieldLabels, IPAddress %>" CssClass="SearchTextBoxLabel" /><br />
                                    <ccUI:TextBox ID="IPAddressFilter" runat="server" CssClass="SearchTextBox" ToolTip="<%$Resources: Tooltips, SearchByIpAddress %>" />
                                </td>
                                <td style="vertical-align: bottom;">
                                    <asp:Label runat="server" Text="<%$Resources: SearchFieldLabels, DHCP%>" CssClass="SearchTextBoxLabel" /><br />
                                    <asp:DropDownList ID="DHCPFilter" runat="server" CssClass="SearchDropDownList" />
                                </td>
                                <td style="vertical-align: bottom;">
                                    <asp:Label runat="server" Text="<%$Resources: SearchFieldLabels, DeviceType%>" CssClass="SearchTextBoxLabel" /><br />
                                    <asp:ListBox ID="DeviceTypeFilter" runat="server" CssClass="SearchDropDownList" SelectionMode="Multiple" />
                                </td>
                                <td style="padding-right: 6px; vertical-align: bottom; padding-left: 5px;">
                                    <ccUI:ToolbarButton runat="server" ID="SearchButton" SkinID="<%$Image:SearchIcon%>" OnClick="SearchButton_Click" ImageAlign="Bottom" />
                                </td>
                                <td width="100%">
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td colspan="7">
                                    <asp:Panel runat="server" CssClass="MovePanel">
                                        <table width="100%" style="background-color: #E1EFF8;" cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td>
                                                    <ccAsp:GridPager ID="GridPagerTop" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="background-color: White;">
                                                    <localAsp:DeviceGridView ID="DeviceGridPanel" runat="server" Height="500px" />
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
<ccAsp:MessageBox ID="MoveConfirmation" runat="server" Title="<%$Resources: Titles, MoveStudy_Dialog_Confirmation %>" />