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



<%@ Control Language="C#" AutoEventWireup="true" Codebehind="PasswordExpiredDialog.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Login.PasswordExpiredDialog" %>
<%@ Import Namespace="Resources"%>



<ccAsp:ModalDialog ID="ModalDialog1" runat="server" Width="500px" Title="<%$Resources: Titles, PasswordExpiredDialogTitle %>">
    <ContentTemplate>
    
    <asp:Panel runat="server" Visible="false" ID="ErrorMessagePanel" CssClass="ErrorMessage" style="margin-bottom: 10px;">
        <asp:Label runat="server" ID="ErrorMessage" ></asp:Label>
    </asp:Panel>
    
    <asp:Panel ID="Panel1" runat="server" CssClass="DialogPanelContent" Width="100%">

        <asp:Panel runat="server" CssClass="PasswordExpiredMessage">
            <asp:Label runat="server" ID="Label1">
                <%= SR.YourPasswordHasExpired %>
            </asp:Label>
    
        <table style="margin-top: 10px; margin-bottom: 10px;">
        <tr><td class="ChangePasswordLabel"><%= Labels.UserID %>:</td><td><asp:TextBox runat="server" Width="150px" ID="Username" AutoCompleteType="Disabled"/></td></tr>
        <tr><td class="ChangePasswordLabel"><%= Labels.NewPassword %>:</td><td><asp:TextBox TextMode="Password" runat="server"  Width="150px" ID="NewPassword" AutoCompleteType="Disabled"/></td></tr>
        <tr><td class="ChangePasswordLabel"><%= Labels.RetypeNewPassword %>:</td><td><asp:TextBox TextMode="Password" runat="server"  Width="150px" ID="ConfirmNewPassword"  AutoCompleteType="Disabled"/></td></tr>
        </table>
        
        <input type="hidden" runat="server" id="OriginalPassword" />
           
        </asp:Panel>
    
        <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td align="right" style="padding:10px">
                            <asp:Panel ID="Panel2" runat="server" CssClass="DefaultModalDialogButtonPanel">
                                <ccUI:ToolbarButton ID="OKButton" runat="server" SkinID="<%$Image:OKButton%>" OnClick="ChangePassword_Click" />
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
    </asp:Panel>
    </ContentTemplate>
</ccAsp:ModalDialog>


