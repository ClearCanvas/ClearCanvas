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

<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups.AddEditUserGroupsDialog"
    Codebehind="AddEditUserGroupsDialog.ascx.cs" %>
<%@ Register Src="~/Pages/Admin/UserManagement/UserGroups/PasswordConfirmDialog.ascx" TagName="PasswordConfirmDialog" TagPrefix="localAsp" %>

<script type="text/javascript">
function ValidationUserGroupNameParams()
{
    params = new Array();
    
    input = document.getElementById('<%= GroupName.ClientID %>');
    params.userGroupName=input.value;
    
    input = document.getElementById('<%= OriginalGroupName.ClientID %>');
    params.originalGroupName=input.value;
    
    return params;
}
</script>

<ccAsp:ModalDialog ID="ModalDialog1" runat="server" Width="750px">
    <ContentTemplate>
    
        <div class="DialogPanelContent">
    
        <table cellpadding="5">           
            <tr>
                <td class="DialogTextBoxLabel" nowrap="nowrap"><asp:Label ID="Label1" runat="server" Text="<%$Resources: InputLabels, AdminUserGroups_GroupName %>" CssClass="DialogTextBoxLabel" /></td><td><asp:TextBox runat="server" ID="GroupName" CssClass="DialogTextBox" Width="350px"></asp:TextBox><asp:HiddenField ID="OriginalGroupName" runat="server" /></td>
                <td >
                    <ccAsp:InvalidInputIndicator ID="GroupNameHelpId" runat="server" SkinID="InvalidInputIndicator" />
                    <ccValidator:ConditionalRequiredFieldValidator ID="RequiredFieldValidator2" runat="server"
                                                        ControlToValidate="GroupName" InvalidInputCSS="DialogTextBoxInvalidInput" ValidationGroup="AddEditUserGroupsValidationGroup"
                                                        InvalidInputIndicatorID="GroupNameHelpId" Text="<%$Resources: InputValidation, ThisFieldIsRequired %>" Display="None"
                                                        RequiredWhenChecked="False"/>
                    <ccValidator:DuplicateUsergroupValidator ID="DuplicateUsergroupValidator" runat="server"
                                                        ControlToValidate="GroupName" InvalidInputCSS="DialogTextBoxInvalidInput" ValidationGroup="AddEditUserGroupsValidationGroup"
                                                        InvalidInputIndicatorID="GroupNameHelpId" Text="<%$Resources: InputValidation, AdminUserGroups_UserGroupAlreadyExists %>" Display="None"
                                                        ServicePath="/Services/ValidationServices.asmx" ServiceOperation="ValidateUserGroupName"
                                                        ParamsFunction="ValidationUserGroupNameParams"/>                                                        
                                                            
                </td>
            </tr>
            <tr>
                <td class="DialogTextBoxLabel" nowrap="nowrap">
                <asp:Label ID="Label2" runat="server" Text="<%$Resources: InputLabels, AdminUserGroups_GroupDescription %>" CssClass="DialogTextBoxLabel" />
                </td>
                <td><asp:TextBox runat="server" ID="GroupDescription" CssClass="DialogTextBox" Width="350px"></asp:TextBox>
                </td>
                <td >
                    <ccAsp:InvalidInputIndicator ID="GroupDescriptionHelpId" runat="server" SkinID="InvalidInputIndicator" />
                    <ccValidator:ConditionalRequiredFieldValidator ID="ConditionalRequiredFieldValidator1" runat="server"
                                                        ControlToValidate="GroupDescription" InvalidInputCSS="DialogTextBoxInvalidInput" ValidationGroup="AddEditUserGroupsValidationGroup"
                                                        InvalidInputIndicatorID="GroupDescriptionHelpId" Text="<%$Resources: InputValidation, ThisFieldIsRequired %>" Display="None"
                                                        RequiredWhenChecked="False"/>                                                            
                </td>
            </tr>
            <tr>
                <td class="DialogTextBoxLabel" nowrap="nowrap">
                <asp:Label ID="Label4" runat="server" Text="<%$Resources: InputLabels, AdminUserGroups_DataGroup %>" CssClass="DialogTextBoxLabel" />
                </td>
                <td> <asp:CheckBox ID="DataGroupCheckBox" runat="server" Text="" Checked="false" CssClass="DialogCheckBox" />
                </td>
                <td width="100%"></td>
            </tr>
            <tr>
                <td valign="top" class="DialogTextBoxLabel"><asp:Label ID="Label3" runat="server" Text="<%$Resources: InputLabels, AdminUserGroups_Tokens %>" CssClass="DialogTextBoxLabel" /></td>
                <td valign="top" colspan="2">                    
                    <div  class="DialogCheckBoxList">
                       <asp:CheckBoxList id="TokenCheckBoxList" runat="server" TextAlign="Right" RepeatColumns="1"></asp:CheckBoxList>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="2">

                </td>
            </tr>
            
        </table>
    
        </div>    

        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td align="right">
                    <asp:Panel runat="server" CssClass="DefaultModalDialogButtonPanel">
                        <ccUI:ToolbarButton ID="OKButton" runat="server" SkinID="<%$Image:AddButton%>" ValidationGroup="AddEditUserGroupsValidationGroup" OnClick="OKButton_Click"/>
                        <ccUI:ToolbarButton ID="UpdateButton" runat="server" SkinID="<%$Image:UpdateButton%>" ValidationGroup="AddEditUserGroupsValidationGroup" OnClick="OKButton_Click"/>
                        <ccUI:ToolbarButton ID="CancelButton" runat="server" SkinID="<%$Image:CancelButton%>" OnClick="CancelButton_Click"/>
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </ContentTemplate>
</ccAsp:ModalDialog>

<localAsp:PasswordConfirmDialog ID="PasswordConfirmDialog" runat="server" />
<ccAsp:MessageBox ID="PasswordFailErrorMessage" runat="server" />  
