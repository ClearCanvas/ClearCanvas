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
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PasswordConfirmDialog.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups.PasswordConfirmDialog" %>
    <ccAsp:ModalDialog ID="ModalDialog" runat="server"></ccAsp:ModalDialog>
    
<ccAsp:ModalDialog ID="ModalDialog1" runat="server" Width="350px" Title="<%$Resources: SR, AddEditUserGroups_PasswordConfirmTitle %>">
    <ContentTemplate>
        <div class="DialogPanelContent">
            <table cellpadding="5">
                <tr>
                    <td class="DataAccessChangeDialogLabel" nowrap="nowrap" colspan="2">      
                        <%= SR.AddEditUserGroups_DataAccessChanged %>
                    </td>
                </tr>
                
               
                <tr>
                    <td class="DialogTextBoxLabel" nowrap="nowrap">
                        <asp:Label ID="Label2" runat="server" Text="<%$Resources: Labels, Password %>" CssClass="DialogTextBoxLabel" />
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="Password" TextMode="password" CssClass="LoginTextInput" Width="250px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="DataAccessChangeDialogWarningLabel" colspan="2">
                        <%= SR.AddEditUserGroups_DataAccessChangedDelayWarning %>
                    </td>
                </tr>
            </table>
        </div>
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td align="right">
                    <asp:Panel ID="Panel1" runat="server" CssClass="DefaultModalDialogButtonPanel">
                        <ccUI:ToolbarButton ID="OKButton" runat="server" SkinID="<%$Image:OkButton%>" OnClick="OKButton_Click" />
                        <ccUI:ToolbarButton ID="CancelButton" runat="server" SkinID="<%$Image:CancelButton%>"
                            OnClick="CancelButton_Click" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </ContentTemplate>
</ccAsp:ModalDialog>
