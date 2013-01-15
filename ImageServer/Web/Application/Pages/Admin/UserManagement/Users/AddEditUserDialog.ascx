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

<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.Users.AddEditUserDialog"
            Codebehind="AddEditUserDialog.ascx.cs" %>

<script type="text/javascript">
function ValidationUsernameParams() {
    params = new Array();
    input = document.getElementById('<%=UserLoginId.ClientID%>');
    params.username = input.value;


    input = document.getElementById('<%= OriginalUserLoginId.ClientID%>');
    params.originalUsername = input.value;
    return params;
}
</script>    

<ccAsp:ModalDialog ID="ModalDialog1" runat="server" Width="450px">
    <ContentTemplate>
    
        <div class="DialogPanelContent">
    
            <asp:Table runat="server" skinID="NoSkin" CellSpacing="3" CellPadding="3">
       
                <asp:TableRow runat="server" ID="UserNameRow">
                    <asp:TableCell runat="server" Wrap="false"><asp:Label ID="Label2" runat="server" Text="<%$Resources: InputLabels, AdminUsers_AddEditDialog_UserID %>" CssClass="DialogTextBoxLabel" /></asp:TableCell>
                    <asp:TableCell><asp:TextBox runat="server" ID="UserLoginId" CssClass="DialogTextBox" Width="100%"></asp:TextBox><asp:HiddenField ID="OriginalUserLoginId" runat="server" /></asp:TableCell>
                    <asp:TableCell runat="server" HorizontalAlign="left">
                        <ccAsp:InvalidInputIndicator ID="UserLoginHelpId" runat="server" SkinID="InvalidInputIndicator" />
                        <ccValidator:ConditionalRequiredFieldValidator ID="UserNameRequiredFieldValidator" runat="server"
                                                                       ControlToValidate="UserLoginId" InvalidInputCSS="DialogTextBoxInvalidInput" ValidationGroup="AddEditUserValidationGroup"
                                                                       InvalidInputIndicatorID="UserLoginHelpId" Text="<%$Resources: InputValidation, ThisFieldIsRequired %>" Display="None"
                                                                       RequiredWhenChecked="False"/>
                        <ccValidator:DuplicateUsernameValidator ID="DuplicateUserNameValidator" runat="server"
                                                                ControlToValidate="UserLoginId" InvalidInputCSS="DialogTextBoxInvalidInput" ValidationGroup="AddEditUserValidationGroup"
                                                                InvalidInputIndicatorID="UserLoginHelpId" Text="<%$Resources: InputValidation, UserIDAlreadyExists %>" Display="None"
                                                                ServicePath="/Services/ValidationServices.asmx" ServiceOperation="ValidateUsername"
                                                                ParamsFunction="ValidationUsernameParams"/>                                                        
                    </asp:TableCell>
                </asp:TableRow>
                   
                <asp:TableRow>
                    <asp:TableCell CssClass="DialogTextBoxLabel" Wrap="false"><asp:Label ID="Label1" runat="server" Text="<%$Resources: InputLabels, AdminUsers_AddEditDialog_Name %>" CssClass="DialogTextBoxLabel" /></asp:TableCell>
                    <asp:TableCell><asp:TextBox runat="server" ID="DisplayName" CssClass="DialogTextBox" Width="100%"></asp:TextBox></asp:TableCell>
                    <asp:TableCell HorizontalAlign="left">
                        <ccAsp:InvalidInputIndicator ID="UserDisplayNameHelp" runat="server" SkinID="InvalidInputIndicator" />
                        <ccValidator:ConditionalRequiredFieldValidator ID="ConditionalRequiredFieldValidator1" runat="server"
                                                                       ControlToValidate="DisplayName" InvalidInputCSS="DialogTextBoxInvalidInput" ValidationGroup="AddEditUserValidationGroup"
                                                                       InvalidInputIndicatorID="UserDisplayNameHelp" Text="<%$Resources: InputValidation, ThisFieldIsRequired %>" Display="None"
                                                                       RequiredWhenChecked="False"/>                                                       
                    </asp:TableCell>
                </asp:TableRow>
                 
                <asp:TableRow>
                    <asp:TableCell CssClass="DialogTextBoxLabel" Wrap="false"><asp:Label runat="server" Text="<%$Resources: InputLabels, AdminUsers_AddEditDialog_EmailAddress %>" CssClass="DialogTextBoxLabel" /></asp:TableCell>
                    <asp:TableCell><asp:TextBox runat="server" ID="EmailAddressId" CssClass="DialogTextBox" Width="100%"></asp:TextBox></asp:TableCell>
                    <asp:TableCell HorizontalAlign="left">
                        <ccAsp:InvalidInputIndicator ID="UserEmailAddressHelp" runat="server" SkinID="InvalidInputIndicator" />
                        <ccValidator:RegularExpressionFieldValidator ID="UserEmailAddressValidator"
                                                                     runat="server" ControlToValidate="EmailAddressId" InvalidInputCSS="DialogTextBoxInvalidInput"
                                                                     IgnoreEmptyValue="true" ValidationGroup="AddEditUserValidationGroup"
                                                                     InvalidInputIndicatorID="UserEmailAddressHelp" ValidationExpression="^[\w-]+(\.[\w-]+)*@([a-z0-9-]+(\.[a-z0-9-]+)*?\.[a-z]{2,6}|(\d{1,3}\.){3}\d{1,3})(:\d{4})?$"
                                                                     Text="<%$Resources: InputValidation,EmailAddressInvalid %>"
                                                                     Display="None" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="EnabledRow"><asp:TableCell VerticalAlign="top" CssClass="DialogTextBoxLabel"><asp:Label ID="Label4" runat="server" Text="Enabled" CssClass="DialogTextBoxLabel" /></asp:TableCell>
                    <asp:TableCell ColumnSpan="2"><asp:CheckBox runat="server" ID="UserEnabledCheckbox" /></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell VerticalAlign="top" CssClass="DialogTextBoxLabel"><asp:Label ID="Label3" runat="server" Text="<%$Resources: InputLabels, AdminUsers_AddEditDialog_Groups %>" CssClass="DialogTextBoxLabel" /></asp:TableCell>
                    <asp:TableCell ColumnSpan="2" Width="100%">
                        <div class="DialogCheckBoxList">
                            <asp:CheckBoxList runat="server" ID="UserGroupListBox" Width="100%" />
                        </div>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
    
        </div>    
        
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td align="right">
                    <asp:Panel runat="server" CssClass="DefaultModalDialogButtonPanel">
                        <ccUI:ToolbarButton ID="UpdateButton" runat="server" SkinID="<%$Image:UpdateButton%>" ValidationGroup="AddEditUserValidationGroup" OnClick="OKButton_Click" />
                        <ccUI:ToolbarButton ID="OKButton" runat="server" SkinID="<%$Image:AddButton%>" ValidationGroup="AddEditUserValidationGroup" OnClick="OKButton_Click" />
                        <ccUI:ToolbarButton ID="CancelButton" runat="server" SkinID="<%$Image:CancelButton%>" OnClick="CancelButton_Click" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </ContentTemplate>
</ccAsp:ModalDialog>
