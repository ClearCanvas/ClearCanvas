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

<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive.AddEditPartitionDialog"
    Codebehind="AddEditPartitionDialog.ascx.cs" %>
    
    

<ccAsp:ModalDialog ID="ModalDialog" runat="server">
    <ContentTemplate>
        <asp:Panel ID="Panel3" runat="server" CssClass="DialogPanelContent" style="padding-top: 10px;" DefaultButton="OKButton">
            <table width="100%">
                <tr>
                    <td align="left">
                        <table>
                            <tr>
                                <td><asp:Label ID="Label1" runat="server" Text="<%$Resources: InputLabels, PartitionArchiveDescription%>" CssClass="DialogTextBoxLabel" /></td>
                                <td><asp:TextBox ID="Description" runat="server" ValidationGroup="AddEditPartitionValidationGroup" MaxLength="128" Width="300" CssClass="DialogTextBox"></asp:TextBox></td>
                                <td>
                                    <ccAsp:InvalidInputIndicator ID="DescriptionHelp" runat="server" SkinID="InvalidInputIndicator"></ccAsp:InvalidInputIndicator>
                                    <ccValidator:ConditionalRequiredFieldValidator ID="ConditionalRequiredFieldValidator1"
                                         runat="server" ControlToValidate="Description" InvalidInputCSS="DialogTextBoxInvalidInput"                                                        
                                         ValidationGroup="AddEditPartitionValidationGroup"
                                         Text="<%$Resources: InputValidation, ThisFieldIsRequired %>" InvalidInputIndicatorID="DescriptionHelp"
                                         Display="None"></ccValidator:ConditionalRequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td><asp:Label ID="Label2" runat="server" Text="<%$Resources: InputLabels, PartitionArchiveArchiveDelay%>" CssClass="DialogTextBoxLabel" /></td>
                                <td colspan="2">
                                    <table>
                                        <tr>
                                            <td><asp:TextBox ID="ArchiveDelay" runat="server" ValidationGroup="AddEditPartitionValidationGroup" MaxLength="4" Width="30" CssClass="DialogTextBox"></asp:TextBox> <%= Resources.SR.Hours %></td>
                                            <td><ccAsp:InvalidInputIndicator ID="ArchiveDelayHelp" runat="server" SkinID="InvalidInputIndicator"></ccAsp:InvalidInputIndicator>                                    
                                                <ccValidator:RegularExpressionFieldValidator ID="ArchiveDelayValidator"
                                                     runat="server" ControlToValidate="ArchiveDelay" InvalidInputCSS="DialogTextBoxInvalidInput"
                                                     IgnoreEmptyValue="false" ValidationGroup="AddEditPartitionValidationGroup" ValidationExpression="^[+]?[1-9]([0-9]*)$"
                                                     Text="<%$Resources: InputValidation, AdminPartitionArchive_InvalidDelay %>" 
                                                     Display="None" InvalidInputIndicatorID="ArchiveDelayHelp"></ccValidator:RegularExpressionFieldValidator> 
                                             </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>                                         
                                 </td>
                            </tr>
                            <tr>
                                <td><asp:Label ID="Label6" runat="server" Text="<%$Resources: InputLabels, PartitionArchiveArchiveType%>" CssClass="DialogTextBoxLabel" /></td>
                                <td><asp:DropDownList ID="ArchiveTypeDropDownList" runat="server" Width="125" CssClass="DialogDropDownList"/></td>
                                <td>

                                </td>
                            </tr>
                            <tr>
                                <td valign="top"><asp:Label ID="Label3" runat="server" Text="<%$Resources: InputLabels, PartitionArchiveConfigurationXML%>" CssClass="DialogTextBoxLabel" /></td>
                                <td><asp:TextBox ID="ConfigurationXML" TextMode="MultiLine" runat="server" ValidationGroup="AddEditPartitionValidationGroup" MaxLength="4" Width="300" CssClass="DialogTextBox" Rows="5"></asp:TextBox></td>
                                <td>      
                                    <ccAsp:InvalidInputIndicator ID="ConfigurationXMLHelp" runat="server" SkinID="InvalidInputIndicator"></ccAsp:InvalidInputIndicator>                                    
                                    <ccValidator:ConditionalRequiredFieldValidator ID="ConfigurationXMLValidator" runat="server"
                                                        ControlToValidate="ConfigurationXML" Display="None" EnableClientScript="true" 
                                                        Text="<%$Resources:InputValidation, ThisFieldIsRequired %>"
                                                        IgnoreEmptyValue="false" InvalidInputCSS="DialogTextBoxInvalidInput" 
                                                        ValidationGroup="AddEditPartitionValidationGroup" InvalidInputIndicatorID="ConfigurationXMLHelp" />
                                                    
                                 </td>
                            </tr>
                            <tr>
                                <td><asp:Label ID="Label4" runat="server" Text="<%$Resources: InputLabels, Enabled%>" CssClass="DialogTextBoxLabel" /></td>
                                <td><asp:CheckBox ID="EnabledCheckBox" runat="server" CssClass="DialogCheckBox" /></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td><asp:Label ID="Label5" runat="server" Text="<%$Resources: InputLabels, PartitionArchiveReadOnly%>" CssClass="DialogTextBoxLabel" /></td>
                                <td><asp:CheckBox ID="ReadOnlyCheckBox" runat="server" CssClass="DialogCheckBox" /></td>
                                <td>                                   
                                 </td>
                            </tr>
                         </table>
                     </td>
                 </tr>
                 <tr>
                    <td align="left">
                        <table>
                            <tr>
                                <td>
                                         
                                </td>
                                <td>
                                    
                                </td>
                            </tr>
                        </table>
                     </td>
                  </tr>
               </table>
        </asp:Panel>
                    <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td align="right">
                            <asp:Panel ID="Panel1" runat="server" CssClass="DefaultModalDialogButtonPanel">
                                <ccUI:ToolbarButton ID="UpdateButton" runat="server" SkinID="<%$Image:UpdateButton%>" OnClick="OKButton_Click" ValidationGroup="AddEditPartitionValidationGroup" />
                                <ccUI:ToolbarButton ID="OKButton" runat="server" SkinID="<%$Image:AddButton%>" OnClick="OKButton_Click" ValidationGroup="AddEditPartitionValidationGroup" />
                                <ccUI:ToolbarButton ID="CancelButton" runat="server" SkinID="<%$Image:CancelButton%>" OnClick="CancelButton_Click" />
                            </asp:Panel>
                        </td>
                    </tr>
                </table>

    </ContentTemplate>
</ccAsp:ModalDialog>
