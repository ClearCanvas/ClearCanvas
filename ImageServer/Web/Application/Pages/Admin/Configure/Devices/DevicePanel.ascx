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

<%@ Import Namespace="Resources" %>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DevicePanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.Devices.DevicePanel" %>
<%@ Register Src="DeviceGridView.ascx" TagName="DeviceGridView" TagPrefix="localAsp" %>
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

        <asp:Table ID="Table" runat="server">
            <asp:TableRow>
                <asp:TableCell>
                    <asp:Panel ID="Panel6" runat="server" CssClass="SearchPanelContent" DefaultButton="SearchButton">
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td align="left">
                                    <asp:Label ID="Label1" runat="server" Text="<%$Resources: SearchFieldLabels, AETitle %>" CssClass="SearchTextBoxLabel"
                                        EnableViewState="False"></asp:Label><br />
                                    <ccUI:TextBox ID="AETitleFilter" runat="server" CssClass="SearchTextBox" ToolTip="Search the list by AE Title"></ccUI:TextBox>
                                </td>
                                <td align="left" valign="bottom">
                                    <asp:Label ID="Label6" runat="server" Text="<%$Resources: SearchFieldLabels, DeviceDescription %>" CssClass="SearchTextBoxLabel"
                                        EnableViewState="False"></asp:Label><br />
                                    <ccUI:TextBox ID="DescriptionFilter" runat="server" CssClass="SearchTextBox" ToolTip="Search the list by Description"></ccUI:TextBox>
                                </td>
                                <td align="left">
                                    <asp:Label ID="Label2" runat="server" Text="<%$Resources: SearchFieldLabels, IPAddress %>" CssClass="SearchTextBoxLabel"
                                        EnableViewState="False"></asp:Label><br />
                                    <ccUI:TextBox ID="IPAddressFilter" runat="server" CssClass="SearchTextBox" ToolTip="Search the list by IP Address"></ccUI:TextBox>
                                </td>
                                <td align="left" valign="bottom">
                                    <asp:Label ID="Label3" runat="server" Text="<%$Resources: SearchFieldLabels, DeviceStatus %>" CssClass="SearchTextBoxLabel"></asp:Label><br />
                                    <asp:DropDownList ID="StatusFilter" runat="server" CssClass="SearchDropDownList">
                                    </asp:DropDownList>
                                </td>
                                <td align="left" valign="bottom">
                                    <asp:Label ID="Label4" runat="server" Text="<%$Resources: SearchFieldLabels, DHCP %>" CssClass="SearchTextBoxLabel"></asp:Label><br />
                                    <asp:DropDownList ID="DHCPFilter" runat="server" CssClass="SearchDropDownList">
                                    </asp:DropDownList>
                                </td>
                                <td align="left" valign="bottom">
                                    <asp:Label ID="Label5" runat="server" Text="<%$Resources: SearchFieldLabels, DeviceType %>" CssClass="SearchTextBoxLabel"></asp:Label><br />
                                    <asp:ListBox ID="DeviceTypeFilter" runat="server" CssClass="SearchDropDownList" SelectionMode="Multiple">
                                    </asp:ListBox>
                                </td>
                                <td align="right" valign="bottom">
                                    <asp:Panel ID="Panel3" runat="server" CssClass="SearchButtonPanel">
                                        <ccUI:ToolbarButton ID="SearchButton" runat="server" SkinID="<%$Image:SearchIcon%>" OnClick="SearchButton_Click" /></asp:Panel>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow Height="100%">
                <asp:TableCell>
                    <table width="100%" cellpadding="3" cellspacing="0" class="ToolbarButtonPanel">
                        <tr>
                            <td>
                                <asp:UpdatePanel ID="ToolBarUpdatePanel" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Panel ID="ToolbarButtons" runat="server" CssClass="ToolbarButtons">
                                            <ccUI:ToolbarButton ID="AddDeviceButton" runat="server" SkinID="<%$Image:AddButton%>" OnClick="AddDeviceButton_Click" />
                                            <ccUI:ToolbarButton ID="EditDeviceButton" runat="server" SkinID="<%$Image:EditButton%>" OnClick="EditDeviceButton_Click" />
                                            <ccUI:ToolbarButton ID="DeleteDeviceButton" runat="server" SkinID="<%$Image:DeleteButton%>"
                                                OnClick="DeleteDeviceButton_Click" />
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Panel ID="Panel2" runat="server"  CssClass="SearchPanelResultContainer">
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <ccAsp:GridPager ID="GridPagerTop" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="background-color: white;">
                                                <localAsp:DeviceGridView ID="DeviceGridViewControl1" Height="500px" runat="server" />
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
        <div> <b class="roundedCorners"><b class="roundedCorners5"></b><b class="roundedCorners4">
        </b><b class="roundedCorners3"></b><b class="roundedCorners2"><b></b></b><b class="roundedCorners1">
            <b></b></b></b></div>
    </ContentTemplate>
</asp:UpdatePanel>
