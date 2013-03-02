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


<%@ Import namespace="ClearCanvas.ImageServer.Web.Common.Data.Model"%>
<%@ Import namespace="System.ComponentModel"%>
<%@ Import namespace="ClearCanvas.ImageServer.Web.Application.Helpers"%>
<%@ Import namespace="Microsoft.JScript"%>
<%@ Control Language="C#" AutoEventWireup="true" Codebehind="DeletedStudyDetailsDialog.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.DeletedStudyDetailsDialog" %>
<%@ Register Src="DeletedStudyDetailsDialogPanel.ascx" TagName="DialogContent" TagPrefix="localAsp" %>

<ccAsp:ModalDialog ID="ModalDialog" runat="server" Width="775px">
    <ContentTemplate>
        
        <localAsp:DialogContent runat="server" ID="DialogContent" />
        
        <table cellpadding="0" cellspacing="0" width="100%">
                <tr align="right">
                    <td>
                        <asp:Panel ID="Panel1" runat="server" CssClass="DefaultModalDialogButtonPanel">
                            <ccUI:ToolbarButton ID="CloseButton" runat="server" SkinID="<%$Image:OKButton%>" ValidationGroup="DeleteStudyDialogValidationGroup" OnClick="CloseClicked" /> 
                        </asp:Panel>

                    </td>
                </tr>
            </table>
    </ContentTemplate>
</ccAsp:ModalDialog>
