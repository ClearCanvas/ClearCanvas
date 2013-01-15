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

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UpdateAuthorityGroupDialog.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.AddAuthorityGroupDialog" %>

<ccAsp:ModalDialog ID="ModalDialog" runat="server" Width="650px" Title="<%$ Resources:Titles, UpdateAuthorityGroupDialog %>">
    <ContentTemplate>
        <div class="DialogPanelContent">
            <asp:Table ID="Table2" runat="server" SkinID="NoSkin" CellSpacing="3" CellPadding="3">
                <asp:TableRow ID="TableRow5" runat="server">
                    <asp:TableCell ID="TableCell13" runat="server" VerticalAlign="Top">
                        <asp:Label ID="Label4" runat="server" Text="<%$Resources: InputLabels, AuthorityGroupsDataAccess %>"
                            CssClass="DialogTextBoxLabel" />
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell14" runat="server" HorizontalAlign="left" Width="100%">
                        <div class="DialogCheckBoxList">
                            <asp:CheckBoxList ID="AuthorityGroupCheckBoxList" runat="server" TextAlign="Right"
                                RepeatColumns="1">
                            </asp:CheckBoxList>
                        </div>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell15" runat="server" />
                </asp:TableRow>               
            </asp:Table>
        </div>
        <table width="100%" cellspacing="0" cellpadding="0">
            <tr align="right">
                <td>
                    <asp:Panel ID="Panel3" runat="server" CssClass="DefaultModalDialogButtonPanel">
                        <ccUI:ToolbarButton ID="UpdateButton" runat="server" SkinID="<%$Image:UpdateButton%>"
                            OnClick="UpdateButton_Click"  />
                        <ccUI:ToolbarButton ID="CancelButton" runat="server" SkinID="<%$Image:CancelButton%>"
                            OnClick="CancelButton_Click" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </ContentTemplate>
</ccAsp:ModalDialog>