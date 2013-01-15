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

<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.Devices.AddEditDeviceDialog"
    CodeBehind="AddEditDeviceDialog.ascx.cs" %>
<%@ Register Src="ThrottleSettingsTab.ascx" TagName="ThrottleSettingsTab" TagPrefix="localAsp" %>


<ccAsp:ModalDialog ID="ModalDialog1" runat="server" Width="450px">
    <ContentTemplate>
        <script type="text/javascript">

            function AllowStorage_Changed() {
                if ($("#<%=AllowStorageCheckBox.ClientID %>").attr('checked') == false) {
                    $("#<%=AcceptKOPR.ClientID %>").attr('disabled', 'true');
                    $("#<%=AcceptKeyObjectStatesLabel.ClientID %>").css("color", "#bbbbbb");
                } else {
                    $("#<%=AcceptKOPR.ClientID %>").removeAttr('disabled');
                    $("#<%=AcceptKeyObjectStatesLabel.ClientID %>").css("color", "#16425D");
                }
            }

            function AcceptKOPR_Changed() {
                if ($("#<%=AcceptKOPR.ClientID %>").attr('checked') == true) {
                    $("#<%=AllowStorageCheckBox.ClientID %>").attr('checked', true);
                }
            }

            Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(SetupCheckBoxes);            

            function SetupCheckBoxes() {
                if ($("#<%=AllowStorageCheckBox.ClientID %>").attr('checked') == false) {
                    $("#<%=AcceptKOPR.ClientID %>").attr('disabled', 'true');
                    $("#<%=AcceptKeyObjectStatesLabel.ClientID %>").css("color", "#bbbbbb");
                }
                if ($("#<%=AcceptKOPR.ClientID %>").attr('checked') == true)
                    $("#<%=AllowStorageCheckBox.ClientID %>").attr('checked', true);
            }       


        </script>

        <asp:ValidationSummary ID="EditDeviceValidationSummary" ShowMessageBox="false" ShowSummary="true"
            DisplayMode="SingleParagraph" EnableClientScript="true" runat="server" ValidationGroup="AddEditDeviceValidationGroup"
            CssClass="DialogValidationErrorMessage" />
        <aspAjax:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" CssClass="DialogTabControl">
            <aspAjax:TabPanel ID="TabPanel1" runat="server" HeaderText="TabPanel1" CssClass="DialogTabControl">
                <ContentTemplate>
                    <table id="Table2" runat="server">
                        <tr id="Tr1" runat="server" align="left">
                            <td id="Td1" runat="server" valign="bottom">
                                <table>
                                    <tr align="left">
                                        <td>
                                            <asp:Label ID="Label1" runat="server" Text="<%$Resources: InputLabels, AETitle %>" CssClass="DialogTextBoxLabel" /><br />
                                            <asp:TextBox ID="AETitleTextBox" runat="server" ValidationGroup="AddEditDeviceValidationGroup"
                                                MaxLength="16" CssClass="DialogTextBox"></asp:TextBox>
                                        </td>
                                        <td valign="bottom">
                                            <ccAsp:InvalidInputIndicator ID="AETitleHelp" runat="server" SkinID="InvalidInputIndicator" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td id="Td2" runat="server" align="left" valign="bottom">
                                <table width="100%">
                                    <tr align="left">
                                        <td>
                                            <asp:Label ID="Label2" runat="server" Text="<%$Resources: InputLabels, DeviceDescription %>" CssClass="DialogTextBoxLabel" /><br />
                                            <asp:TextBox ID="DescriptionTextBox" runat="server" CssClass="DialogTextBox"></asp:TextBox>
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr id="Tr2" runat="server" align="left" valign="bottom">
                            <td id="Td4" valign="bottom">
                                <table>
                                    <tr align="left">
                                        <td>
                                            <asp:Label ID="Label3" runat="server" Text="<%$Resources: InputLabels, IPAddress %>" CssClass="DialogTextBoxLabel" />
                                            <asp:CheckBox ID="DHCPCheckBox" runat="server" Text="<%$Resources: InputLabels, DHCP %>" CssClass="DialogCheckBox" /><br />
                                            <asp:TextBox ID="IPAddressTextBox" runat="server" ValidationGroup="AddEditDeviceValidationGroup"
                                                CssClass="DialogTextBox">
                                            </asp:TextBox>
                                        </td>
                                        <td align="left" valign="bottom">
                                            <ccAsp:InvalidInputIndicator ID="IPAddressHelp" runat="server" SkinID="InvalidInputIndicator">
                                            </ccAsp:InvalidInputIndicator>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td id="Td6" runat="server" align="left" valign="bottom">
                                <table width="100%">
                                    <tr align="left">
                                        <td>
                                            <asp:Label ID="Label6" runat="server" Text="<%$Resources: InputLabels, DeviceType %>" CssClass="DialogTextBoxLabel" /><br />
                                            <asp:DropDownList ID="DeviceTypeDropDownList" runat="server" Width="100%" CssClass="DialogDropDownList">
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr id="Tr3" runat="server" align="left" valign="bottom">
                            <td id="Td7" runat="server" valign="bottom">
                                <table>
                                    <tr align="left">
                                        <td>
                                            <asp:Label ID="Label5" runat="server" Text="<%$Resources: InputLabels, Port %>" CssClass="DialogTextBoxLabel" /><br />
                                            <asp:TextBox ID="PortTextBox" runat="server" CssClass="DialogTextBox" />
                                        </td>
                                        <td valign="bottom">
                                            <ccAsp:InvalidInputIndicator ID="PortHelp" runat="server" SkinID="InvalidInputIndicator">
                                            </ccAsp:InvalidInputIndicator>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td id="Td9" runat="server" valign="bottom">
                                <table width="100%">
                                    <tr align="left">
                                        <td>
                                            <asp:CheckBox ID="ActiveCheckBox" runat="server" Checked="True" Text="<%$Resources: InputLabels, Enabled %>" CssClass="DialogCheckBox" />
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                    <ccValidator:RegularExpressionFieldValidator ID="AETitleTextBoxValidator" runat="server"
                        ControlToValidate="AETitleTextBox" InvalidInputCSS="DialogTextBoxInvalidInput"
                        ValidationGroup="AddEditDeviceValidationGroup" InvalidInputIndicatorID="AETitleHelp"
                        ValidationExpression="^([^\\]){1,16}$" Text="<%$Resources: InputValidation,InvalidAETitle %>" Display="None"></ccValidator:RegularExpressionFieldValidator>
                    <ccValidator:DeviceValidator ID="DeviceValidator" runat="server"
                        ControlToValidate="AETitleTextBox" Display="None" EnableClientScript="false"
                        Text="<%$Resources: InputValidation, DuplicateAETitle %>" InvalidInputCSS="DialogTextBoxInvalidInput" 
                        ValidationGroup="AddEditDeviceValidationGroup" InvalidInputIndicatorID="AETitleHelp"></ccValidator:DeviceValidator>
                    <ccValidator:ConditionalRequiredFieldValidator ID="IPAddressValidator" runat="server"
                        ControlToValidate="IPAddressTextBox" ValidateWhenUnchecked="true" ConditionalCheckBoxID="DHCPCheckBox" InvalidInputCSS="DialogTextBoxInvalidInput"
                        ValidationGroup="AddEditDeviceValidationGroup" Text="<%$Resources: InputValidation, ThisFieldIsRequired %>"
                        InvalidInputIndicatorID="IPAddressHelp" Display="None"></ccValidator:ConditionalRequiredFieldValidator>
                    <ccValidator:RangeValidator ID="PortValidator" runat="server" ControlToValidate="PortTextBox"
                        ValidationGroup="AddEditDeviceValidationGroup" MinValue="1" MaxValue="65535"
                        InvalidInputCSS="DialogTextBoxInvalidInput" Text="<%$Resources: InputValidation, InvalidPort %>"
                        InvalidInputIndicatorID="PortHelp" Display="None"></ccValidator:RangeValidator>
                </ContentTemplate>
                <HeaderTemplate><%= Titles.AdminDevices_AddEditDialog_GeneralTabTitle%></HeaderTemplate>
            </aspAjax:TabPanel>
            <aspAjax:TabPanel ID="TabPanel2" runat="server" HeaderText="TabPanel2" Height="200px">
                <ContentTemplate>
                    <table width="100%">
                        <tr>
                            <td>
                            </td>
                            <td>
                                <asp:CheckBox ID="AllowStorageCheckBox" runat="server" Text="<%$Resources: InputLabels, DeviceFeatures_Storage %>" 
                                    ToolTip="<%$Resources: Tooltips, DeviceFeatures_Store %>"
                                    CssClass="DialogCheckBox" />
                                <div style="padding-left: 18px; padding-top: 3px; padding-bottom: 2px;">
                                    <asp:CheckBox ID="AcceptKOPR" runat="server" Checked="False" Text="" CssClass="DialogCheckBox" /><asp:Label
                                        ID="AcceptKeyObjectStatesLabel" runat="server" Text="<%$Resources: InputLabels, DeviceFeatures_AcceptKOPRFeature %>"
                                        CssClass="DialogCheckBox" />
                                </div>
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td>
                                <asp:CheckBox ID="AllowAutoRouteCheckBox" runat="server" Text="<%$Resources: InputLabels, DeviceFeatures_AutoRoute %>" 
                                    ToolTip="<%$Resources: Tooltips, DeviceFeatures_AutoRoute %>"
                                    CssClass="DialogCheckBox" />
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td>
                                <asp:CheckBox ID="AllowQueryCheckBox" runat="server" Text="<%$Resources: InputLabels, DeviceFeatures_Query %>" 
                                    ToolTip="<%$Resources: Tooltips, DeviceFeatures_Query%>"
                                    CssClass="DialogCheckBox" />
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td>
                                <asp:CheckBox ID="AllowRetrieveCheckBox" runat="server" Text="<%$Resources: InputLabels, DeviceFeatures_Retrieve %>" ToolTip="<%$Resources: Tooltips, DeviceFeatures_Retrieve %>"
                                    CssClass="DialogCheckBox" />
                            </td>
                            <td>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
                <HeaderTemplate><%= Titles.AdminDevices_AddEditDialog_FeaturesTabTitle%></HeaderTemplate>
            </aspAjax:TabPanel>
            <aspAjax:TabPanel ID="TabPanel3" runat="server" HeaderText="TabPanel2" Height="200px">
                <ContentTemplate>
                    <localAsp:ThrottleSettingsTab runat="server" ID="ThrottleSettingsTab" />
                </ContentTemplate>
                <HeaderTemplate><%= Titles.AdminDevices_AddEditDialog_ThrottleTabTitle%></HeaderTemplate>
            </aspAjax:TabPanel>
        </aspAjax:TabContainer>
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td align="right">
                    <asp:Panel runat="server" CssClass="DefaultModalDialogButtonPanel">
                        <ccUI:ToolbarButton ID="UpdateButton" runat="server" SkinID="<%$Image:UpdateButton%>" OnClick="OKButton_Click"
                            ValidationGroup="AddEditDeviceValidationGroup" />
                        <ccUI:ToolbarButton ID="OKButton" runat="server" SkinID="<%$Image:AddButton%>" OnClick="OKButton_Click"
                            ValidationGroup="AddEditDeviceValidationGroup" />
                        <ccUI:ToolbarButton ID="CancelButton" runat="server" SkinID="<%$Image:CancelButton%>" OnClick="CancelButton_Click" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </ContentTemplate>
</ccAsp:ModalDialog>
