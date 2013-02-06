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

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ThrottleSettingsTab.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.Devices.ThrottleSettingsTab" %>
    
    

<script type="text/javascript">
    Sys.Application.add_load(page_load);
    function page_load() {
        var limitedCB = $("#<%= LimitedCheckBox.ClientID %>");
        var unlimitedCB = $("#<%= UnlimitedCheckBox.ClientID %>");
        var textInput = $("#<%=MaxConnectionTextBox.ClientID %>");

        if (unlimitedCB.is(":checked")) {
            textInput.hide();
        }

        unlimitedCB.click(function(ev) {
            $("#<%=MaxConnectionTextBox.ClientID %>").hide();
            $("#<%=InvalidRangeIndicator.ClientID %>").hide();
            $("#<%=MaxConnectionTextBox.ClientID %>").val("");
        });

        limitedCB.click(function(ev) {
            var textInput = $("#<%=MaxConnectionTextBox.ClientID %>");
            textInput.show();
            textInput.focus();
            textInput.select();
        });
    }
</script>

<asp:Panel runat="server" CssClass="DeviceSettingThrottleTab-GroupPanel">
    <div class="DialogMessagePanel" style="width: 460px;">
        <%=SR.AdminDevices_ThrottleSettings_Info %>
    </div>
    <table width="100%">
        <tr>
            <td>
                <span style="white-space: nowrap"><%= InputLabels.AdminDevices_ThrottleSettings_NumConnectionsAllowed%></span>
            </td>
            <td>
                <table>
                    <tr>
                        <td>
                            <asp:RadioButton runat="server" ID="UnlimitedCheckBox" GroupName="MaxConnection"
                                Text="<%$Resources: InputLabels,AdminDevices_ThrottleSettings_Unlimited%>" />
                        </td>
                        <td>
                            <asp:RadioButton runat="server" ID="LimitedCheckBox" GroupName="MaxConnection" Text="<%$Resources: InputLabels,AdminDevices_ThrottleSettings_Limited%>" />
                        </td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:TextBox runat="server" Width="20" ID="MaxConnectionTextBox" CssClass="DialogTextBox"
                                            ValidationGroup="ThrottleSettingsValidationGroup"></asp:TextBox>
                                    </td>
                                    <td>
                                        <ccAsp:InvalidInputIndicator ID="InvalidRangeIndicator" runat="server" SkinID="InvalidInputIndicator" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Panel>
<ccValidator:RangeValidator ID="MaxConnectionTextBoxValidator" runat="server" ConditionalCheckBoxID="LimitedCheckBox"
    ControlToValidate="MaxConnectionTextBox" InvalidInputIndicatorID="InvalidRangeIndicator"
    ValidationGroup="ThrottleSettingsValidationGroup" Display="None" InvalidInputCSS="DialogTextBoxInvalidInput"
    MinValue="1" MaxValue="100" Text="<%$Resources: InputValidation,AdminDevices_ThrottleSettings_InvalidNumOfConnections%>"></ccValidator:RangeValidator>
