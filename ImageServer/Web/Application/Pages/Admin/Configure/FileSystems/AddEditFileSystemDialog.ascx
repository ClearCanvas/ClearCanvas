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

<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.FileSystems.AddFilesystemDialog"
    Codebehind="AddEditFileSystemDialog.ascx.cs" %>

<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
    <Services>
        <asp:ServiceReference Path="~/Services/FilesystemInfoService.asmx" />
    </Services>
</asp:ScriptManagerProxy>
<ccAsp:ModalDialog ID="ModalDialog" runat="server" Width="450px">
    <ContentTemplate>
    
        <script type="text/javascript">
    Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(InputHover);
    </script>
    
            <asp:ValidationSummary ID="EditFileSystemValidationSummary" ShowMessageBox="false" ShowSummary="true" DisplayMode="SingleParagraph"
                EnableClientScript="true" runat="server" ValidationGroup="AddEditFileSystemValidationGroup" CssClass="DialogValidationErrorMessage" />   			
            <aspAjax:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" CssClass="DialogTabControl">
                <aspAjax:TabPanel ID="TabPanel1" runat="server" CssClass="DialogTabControl">
                    <ContentTemplate>
                        <asp:Panel ID="Panel1" runat="server" CssClass="CSSDialogTabPanelContent">
                            <table id="TABLE1" runat="server" cellspacing="4" width="100%">
                                <tr id="Tr1" align="left" valign="bottom" runat="server">
                                    <td id="Td1" runat="server">
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label1" runat="server" Text="<%$Resources: InputLabels, FilesystemDescription %>" CssClass="DialogTextBoxLabel" /><br />
                                                    <asp:TextBox ID="DescriptionTextBox" runat="server" Width="220px" BorderColor="LightSteelBlue"
                                                        BorderWidth="1px" MaxLength="128" ValidationGroup="AddEditFileSystemValidationGroup" CssClass="DialogTextBox"></asp:TextBox>
                                                </td>
                                                <td valign="bottom">
                                                    <ccAsp:InvalidInputIndicator ID="InvalidDescriptionHint" runat="server" SkinID="InvalidInputIndicator"></ccAsp:InvalidInputIndicator>
                                                    <ccValidator:ConditionalRequiredFieldValidator ID="ConditionalRequiredFieldValidator1"
                                                        runat="server" ControlToValidate="DescriptionTextBox" InvalidInputCSS="DialogTextBoxInvalidInput"
                                                        ValidationGroup="AddEditFileSystemValidationGroup" Text="<%$Resources: InputValidation, ThisFieldIsRequired %>" Display="None"
                                                        InvalidInputIndicatorID="InvalidDescriptionHint" RequiredWhenChecked="False">
                                                    </ccValidator:ConditionalRequiredFieldValidator>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td id="Td2" runat="server">
                                        <table width="100px">
                                            <tr>
                                                <td>
                                                    <asp:CheckBox ID="ReadCheckBox" runat="server"  Text="<%$Resources: InputLabels, FilesystemReadPermission%>"
                                                        Checked="True" TextAlign="Right" CssClass="DialogCheckBox" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr valign="bottom">
                                    <td id="Td3" runat="server">
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label2" runat="server" Text="<%$Resources: InputLabels, FilesystemPath%>" CssClass="DialogTextBoxLabel" /><br />
                                                    <asp:TextBox ID="PathTextBox" runat="server" Width="220px" BorderColor="LightSteelBlue"
                                                        BorderWidth="1px" ValidationGroup="AddEditFileSystemValidationGroup" MaxLength="256" CssClass="DialogTextBox"></asp:TextBox>
                                                </td>
                                                <td valign="bottom">
                                                    <ccAsp:InvalidInputIndicator ID="InvalidPathHint" runat="server" SkinID="InvalidInputIndicator" />
                                                    <ccValidator:FilesystemPathValidator runat="server" ID="PathValidator" ControlToValidate="PathTextBox"
                                                        InputName="Filesystem Path" InvalidInputCSS="DialogTextBoxInvalidInput" InvalidInputIndicatorID="InvalidPathHint"
                                                        ServicePath="/Services/ValidationServices.asmx" ServiceOperation="ValidateFilesystemPath"
                                                        ParamsFunction="ValidationFilesystemPathParams" Display="None" ValidationGroup="AddEditFileSystemValidationGroup" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td id="Td4" runat="server">
                                        <table width="100px">
                                            <tr>
                                                <td>
                                                    <asp:CheckBox ID="WriteCheckBox" runat="server" Text="<%$Resources: InputLabels, FilesystemWritePermission%>" Checked="True" CssClass="DialogCheckBox" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr valign="bottom">
                                    <td>
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label3" runat="server" Text="<%$Resources: InputLabels, FilesystemTier%>" CssClass="DialogTextBoxLabel" /><br />
                                                    <asp:DropDownList ID="TiersDropDownList" runat="server" Width="220px" CssClass="DialogDropDownList">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </ContentTemplate>
                    <HeaderTemplate><%= Titles.AdminFilesystem_AddEditDialog_GeneralTabTitle%>
                    </HeaderTemplate>
                </aspAjax:TabPanel>
                <aspAjax:TabPanel ID="TabPanel2" runat="server"  OnClientClick="LoadFilesystemInfo">
                    <ContentTemplate>
                            <table id="TABLE2" runat="server" cellspacing="4" border="0">
                                <!-- total size -->
                                <tr id="Tr4" align="left" valign="bottom">
                                    <td>
                                        <asp:Panel runat="server" ID="TotalSizePanel">
                                            <table>
                                                <tr>
                                                    <td width="120px" align="left" valign="bottom">
                                                        <asp:Label ID="Label7" runat="server" Text="<%$Resources: Labels, AdminFilesystem_AddEditDialog_TotalSize%>" CssClass="DialogTextBoxLabel" />
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="TotalSizeIndicator" runat="server" Text="??? KB" CssClass="DialogTextBoxLabel" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                    </td>
                                </tr>
                                <!-- available size -->
                                <tr id="Tr3" align="left" valign="bottom">
                                    <td>
                                        <asp:Panel runat="server" ID="AvailableSizePanel">
                                            <table width="100%">
                                                <tr>
                                                    <td width="120px" align="left" valign="bottom">
                                                        <asp:Label ID="Label8" runat="server" Text="<%$Resources: Labels, AdminFilesystem_AddEditDialog_Used%>" CssClass="DialogTextBoxLabel" />
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="UsedSizeIndicator" runat="server" Text="??? KB" CssClass="DialogTextBoxLabel" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                    </td>
                                </tr>
                                <!-- highwatermark -->
                                <tr id="Tr5" align="left" valign="bottom">
                                    <td>
                                        <asp:Panel runat="server" ID="HighWatermarkPanel">
                                            <table>
                                                <tr>
                                                    <td width="150px" align="left" valign="bottom">
                                                        <asp:Label ID="Label4" runat="server" Text="<%$Resources: InputLabels, FilesystemHighWatermark%>" CssClass="DialogTextBoxLabel" /><br />
                                                        <asp:TextBox ID="HighWatermarkTextBox" runat="server" CssClass="DialogTextBox"
                                                            ValidationGroup="AddEditFileSystemValidationGroup" MaxLength="8" />%
                                                    </td>
                                                    <td align="left" valign="bottom">
                                                        <asp:TextBox runat="server" ID="HighWatermarkSize" CssClass="DialogTextBox"
                                                            Text="???.??? GB" Enabled="false"  Style="text-align: right" />
                                                    </td>
                                                    <td align="left" valign="bottom">
                                                        <ccAsp:InvalidInputIndicator ID="HighWatermarkHelp" runat="server" SkinID="InvalidInputIndicator">
                                                        </ccAsp:InvalidInputIndicator>
                                                        <ccValidator:RangeComparisonValidator ID="HighWatermarkValidator" runat="server"
                                                            ControlToValidate="HighWatermarkTextBox" InputName="High watermark" 
                                                            ControlToCompare="LowWatermarkTextBox" GreaterThan="true" 
                                                            Text="<%$Resources: InputValidation, HighWatermark_InvalidRange%>"
                                                            InvalidInputCSS="DialogTextBoxInvalidInput" 
                                                            ValidationGroup="AddEditFileSystemValidationGroup" 
                                                            MinValue="1" MaxValue="99"  Display="None"
                                                            InvalidInputIndicatorID="HighWatermarkHelp" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                    </td>
                                </tr>
                                <!-- low w -->
                                <tr id="Tr2" align="left" valign="bottom">
                                    <td>
                                        <asp:Panel runat="server" ID="Panel4">
                                            <table>
                                                <tr>
                                                    <td width="150px" align="left" valign="bottom">
                                                        <asp:Label ID="Label5" runat="server" Text="<%$Resources: InputLabels, FilesystemLowWatermark%>" CssClass="DialogTextBoxLabel" /><br />
                                                        <asp:TextBox ID="LowWatermarkTextBox" runat="server" CssClass="DialogTextBox" ValidationGroup="AddEditFileSystemValidationGroup" MaxLength="8" />%
                                                    </td>
                                                    <td align="left" valign="bottom">
                                                        <asp:TextBox runat="server" ID="LowWaterMarkSize" CssClass="DialogTextBox"
                                                            Text="???.??? GB" Enabled="false" Style="text-align: right" />
                                                    </td>
                                                    <td align="left" valign="bottom">
                                                        <ccAsp:InvalidInputIndicator ID="LowWatermarkHelp" runat="server" SkinID="InvalidInputIndicator">
                                                        </ccAsp:InvalidInputIndicator>
                                                        <ccValidator:RangeComparisonValidator ID="LowWatermarkValidator" EnableClientScript="true"
                                                            runat="server" ControlToValidate="LowWatermarkTextBox" ControlToCompare="HighWatermarkTextBox"
                                                            Text="<%$Resources: InputValidation, LowWatermark_InvalidRange%>"
                                                            GreaterThan="false" InvalidInputCSS="DialogTextBoxInvalidInput" ValidationGroup="AddEditFileSystemValidationGroup" MinValue="1"
                                                            MaxValue="99" InputName="Low watermark" Display="None"
                                                            InvalidInputIndicatorID="LowWatermarkHelp" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                    </td>
                                </tr>
                            </table>
                    </ContentTemplate>
                    <HeaderTemplate><%= Titles.AdminFilesystem_AddEditDialog_WatermarksTabTitle%>
                    </HeaderTemplate>
                </aspAjax:TabPanel>
            </aspAjax:TabContainer>

            <table cellspacing="0" cellpadding="0" width="100%">
                <tr align="right">
                    <td>
                            <asp:Panel ID="Panel2" runat="server" CssClass="DefaultModalDialogButtonPanel">
                                <ccUI:ToolbarButton ID="UpdateButton" runat="server" SkinID="<%$Image:UpdateButton%>" OnClick="OKButton_Click" ValidationGroup="AddEditFileSystemValidationGroup" />
                                <ccUI:ToolbarButton ID="OKButton" runat="server" SkinID="<%$Image:AddButton%>" OnClick="OKButton_Click" ValidationGroup="AddEditFileSystemValidationGroup" />
                                <ccUI:ToolbarButton ID="CancelButton" runat="server" SkinID="<%$Image:CancelButton%>" OnClick="CancelButton_Click" />
                            </asp:Panel>
                    </td>
                </tr>
            </table>
        <asp:HiddenField ID="TotalSize" runat="server" />
        <asp:HiddenField ID="AvailableSize" runat="server" />
    </ContentTemplate>
</ccAsp:ModalDialog>
