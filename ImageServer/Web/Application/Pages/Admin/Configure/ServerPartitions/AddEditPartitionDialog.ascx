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


<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerPartitions.AddEditPartitionDialog"
    Codebehind="AddEditPartitionDialog.ascx.cs" %>
<%@ Import Namespace="ClearCanvas.ImageServer.Web.Common.Security"%>

<%@ Reference Control="~/Pages/Admin/Configure/ServerPartitions/DataAccessGroupPanel.ascx" %>
<%@ Register 
            Src="~/Pages/Admin/Configure/ServerPartitions/DataAccessGroupPanel.ascx" TagName="DataAccessGroupPanel" TagPrefix="cc" %>
    
<script type="text/javascript">

</script>

<ccAsp:ModalDialog runat="server" ID="ModalDialog">
    <ContentTemplate>
        <asp:Panel ID="Panel1"  runat="server" CssClass="EditPartitionDialog">
    <asp:ValidationSummary ID="EditPartitionValidationSummary" ShowMessageBox="false" ShowSummary="true" DisplayMode="SingleParagraph"
                EnableClientScript="true" runat="server" ValidationGroup="AddEditServerPartitionValidationGroup" CssClass="EditStudyDialogErrorMessage" />   			
        <asp:Panel ID="Panel3" runat="server" DefaultButton="OKButton">
            <aspAjax:TabContainer ID="ServerPartitionTabContainer" runat="server" ActiveTabIndex="0" CssClass="DialogTabControl">
                <aspAjax:TabPanel ID="GeneralTabPanel" runat="server"  CssClass="DialogTabControl">
                    <ContentTemplate>
                        <asp:Panel ID="Panel2" runat="server" CssClass="DialogTabPanelContent EditPartitionDialogTab">
                            <table id="GeneralTabTable" runat="server">
                                <tr id="Tr1" runat="server" align="left">
                                    <td id="Td1" runat="server">
                                        <table width="100%">
                                            <tr align="left">
                                                <td>
                                                    <asp:Label ID="Label4" runat="server" Text="<%$Resources: InputLabels,AETitle %>" CssClass="DialogTextBoxLabel" /><br />
                                                    <asp:TextBox ID="AETitleTextBox" runat="server" MaxLength="16" ValidationGroup="AddEditServerPartitionValidationGroup" CssClass="DialogTextBox"
                                                        ToolTip="<%$Resources:Tooltips, AdminPartition_AddEditDialog_AETitle %>"></asp:TextBox>
                                                </td>
                                                <td valign="bottom">
                                                    <ccAsp:InvalidInputIndicator ID="AETitleHelp" runat="server" SkinID="InvalidInputIndicator" />
                                                    <ccValidator:ConditionalRequiredFieldValidator ID="RequiredFieldValidator2" runat="server"
                                                        ControlToValidate="AETitleTextBox" Display="None" EnableClientScript="true" Text="<%$Resources:InputValidation, ThisFieldIsRequired %>"
                                                        InvalidInputCSS="DialogTextBoxInvalidInput" ValidationGroup="AddEditServerPartitionValidationGroup" InvalidInputIndicatorID="AETitleHelp" />
                                                    <ccValidator:RegularExpressionFieldValidator ID="RegularExpressionFieldValidator2"
                                                        runat="server" ControlToValidate="AETitleTextBox" Display="None" Text="<%$Resources:InputValidation, InvalidAETitle %>"
                                                        InvalidInputCSS="DialogTextBoxInvalidInput" ValidationExpression="^([^\\]){1,16}$" ValidationGroup="AddEditServerPartitionValidationGroup"
                                                        InvalidInputIndicatorID="AETitleHelp" />
                                                    <ccValidator:ServerPartitionValidator ID="ServerPartitionValidator" runat="server"
                                                        ControlToValidate="AETitleTextBox" Display="None" EnableClientScript="false"
                                                        Text="<%$Resources:InputValidation, DuplicateAETitle %>" InvalidInputCSS="DialogTextBoxInvalidInput" ValidationGroup="AddEditServerPartitionValidationGroup"
                                                        InvalidInputIndicatorID="AETitleHelp" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td id="Td2" runat="server" align="left">
                                        <table width="100%">
                                            <tr align="left">
                                                <td>
                                                    <asp:Label ID="Label1" runat="server" Text="<%$Resources: InputLabels,PartitionDescription %>" CssClass="DialogTextBoxLabel" /><br />
                                                    <asp:TextBox ID="DescriptionTextBox" runat="server" ToolTip="<%$Resources:Tooltips, AdminPartition_AddEditDialog_Description %>" CssClass="DialogTextBox"></asp:TextBox>
                                                </td>
                                                <td>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr id="Tr2" runat="server" align="left">
                                    <td id="Td3" runat="server">
                                        <table>
                                            <tr align="left">
                                                <td >
                                                    <asp:Label ID="Label2" runat="server" Text="<%$Resources: InputLabels,Port%>" CssClass="DialogTextBoxLabel" /><br />
                                                    <asp:TextBox ID="PortTextBox" runat="server" CssClass="DialogTextBox"></asp:TextBox>
                                                    <ccValidator:RangeValidator ID="PortValidator1" runat="server" ControlToValidate="PortTextBox"
                                                        InvalidInputCSS="DialogTextBoxInvalidInput" ValidationGroup="AddEditServerPartitionValidationGroup" MinValue="1" MaxValue="65535"
                                                        Text="<%$Resources:InputValidation, InvalidPort %>" Display="None" InvalidInputIndicatorID="PortHelp"></ccValidator:RangeValidator>
                                                </td>
                                                <td valign="bottom">
                                                    <ccAsp:InvalidInputIndicator ID="PortHelp" runat="server" SkinID="InvalidInputIndicator" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td id="Td4" runat="server">
                                        <table>
                                            <tr align="left">
                                                <td>
                                                    <asp:Label ID="Label3" runat="server" Text="<%$Resources: InputLabels, PartitionFolderName %>" CssClass="DialogTextBoxLabel" /><br />
                                                    <asp:TextBox ID="PartitionFolderTextBox" runat="server" CausesValidation="true" ValidationGroup="AddEditServerPartitionValidationGroup"
                                                        ToolTip="<%$Resources:Tooltips, AdminPartition_AddEditDialog_PartitionFolderName %>" CssClass="DialogTextBox"/>
                                                    <ccValidator:ServerPartitionFolderValidator ID="PartitionFolderValidator"
                                                        runat="server" ControlToValidate="PartitionFolderTextBox" Display="None" EnableClientScript="false"
                                                        Text="<%$Resources:InputValidation, InvalidPartitionFolderName %>" InvalidInputCSS="DialogTextBoxInvalidInput" ValidationGroup="AddEditServerPartitionValidationGroup"
                                                        InvalidInputIndicatorID="FolderHelp"/>
                                                </td>
                                                <td valign="bottom">
                                                    <ccAsp:InvalidInputIndicator ID="FolderHelp" runat="server" SkinID="InvalidInputIndicator" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr id="Tr3" runat="server" align="left">
                                    <td id="Td5" runat="server">
                                        <table width="100%">
                                            <tr>
                                                <td>
                                                    <asp:CheckBox ID="EnabledCheckBox" runat="server" Checked="True" Text="<%$Resources: InputLabels, Enabled %>"  CssClass="DialogCheckBox" />
                                                </td>
                                                <td>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td id="Td6" runat="server" valign="top">
                                        <table width="100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="DuplicateSopLabel" runat="server" Text="<%$Resources: InputLabels, DuplicateObjectPolicy %>" CssClass="DialogTextBoxLabel" /><br />
                                                    <asp:DropDownList ID="DuplicateSopDropDownList" runat="server" CssClass="DialogDropDownList" 
                                                    ToolTip="<%$Resources:Tooltips, AdminPartition_AddEditDialog_DuplicateObjectPolicy %>" />
                                                </td>
                                                <td>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td>
                                        <asp:Label ID="AcceptLatestReportLabel" runat="server" Text="<%$Resources: InputLabels, AcceptLatestReportLabel %>" CssClass="DialogTextBoxLabel" /><br />
                                          <asp:CheckBox ID="AcceptLatestReportCheckBox" runat="server" Text="<%$Resources: InputLabels, AcceptLatestReport %>" CssClass="DialogCheckBox"
                                                        ToolTip="<%$Resources:Tooltips, AdminPartition_AddEditDialog_AcceptLatestReport %>" />

                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </ContentTemplate>
                    <HeaderTemplate><%= Titles.AdminPartition_AddEditDialog_GeneralTabTitle %></HeaderTemplate>
                </aspAjax:TabPanel>
                <aspAjax:TabPanel ID="TabPanel2" runat="server" HeaderText="TabPanel2">
                    <ContentTemplate>
                        <asp:Panel runat="server" CssClass="DialogTabPanelContent EditPartitionDialogTab" >
                            <table width="100%">
                                <tr>
                                    <td align="left">
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:CheckBox ID="AcceptAnyDeviceCheckBox" runat="server" Text="<%$Resources: InputLabels, AcceptAnyDevice %>" CssClass="DialogCheckBox"
                                                        ToolTip="<%$Resources:Tooltips, AdminPartition_AddEditDialog_AcceptAnyDevice %>" />
                                                </td>
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
                                                    <asp:CheckBox ID="AutoInsertDeviceCheckBox" runat="server" Text="<%$Resources: InputLabels, AutoInsertDevices %>" CssClass="DialogCheckBox"
                                                        ToolTip="<%$Resources:Tooltips, AdminPartition_AddEditDialog_AutoInsertDevices %>" />
                                                </td>
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
                                                    <asp:Label ID="Label5" runat="server" Text="<%$Resources: InputLabels, DefaultRemotePort %>" CssClass="DialogTextBoxLabel" /><asp:TextBox ID="DefaultRemotePortTextBox" CssClass="DialogTextBox" runat="server"></asp:TextBox>
                                                    <td valign="bottom">
                                                        <ccAsp:InvalidInputIndicator ID="DefaultPortHelp" runat="server" SkinID="InvalidInputIndicator" />
                                                        <ccValidator:RangeValidator ID="DefaultRemotePortRangeValidator" runat="server"
                                                            ControlToValidate="DefaultRemotePortTextBox" InvalidInputCSS="DialogTextBoxInvalidInput" ValidationGroup="AddEditServerPartitionValidationGroup"
                                                            MinValue="1" MaxValue="65535" Text="Remote device default port must be between 1 and 65535"
                                                            Display="None" InvalidInputIndicatorID="DefaultPortHelp" />
                                                    </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </ContentTemplate>
                    <HeaderTemplate><%= Titles.AdminPartition_AddEditDialog_RemoteDevicesTabTitle %></HeaderTemplate>
                </aspAjax:TabPanel>
                 <aspAjax:TabPanel ID="TabPanel1" runat="server" HeaderText="GeneralTabPanel" CssClass="DialogTabControl">
                    <ContentTemplate>
                        <asp:Panel ID="Panel4" runat="server" CssClass="DialogTabPanelContent EditPartitionDialogTab" >
                            <div class="ServerPartitionDialogTabDescription">
                                <%= SR.AdminPartition_AddEditDialog_StudyMatchingInfo %>
                            </div>
                            
                            <table width="100%">
                            <tr>
                                <td><asp:CheckBox ID="MatchPatientName" runat="server" Text="<%$Resources: Labels, PatientName %>" CssClass="DialogCheckBox"/></td>
                                <td><asp:Image ID="Image1" runat="server" SkinID="Spacer" Width="20px" Height="1px"/></td>                                                        
                                <td><asp:CheckBox ID="MatchPatientID" runat="server" Text="<%$Resources: Labels, PatientID %>" CssClass="DialogCheckBox" /></td>

                            </tr>
                            <tr>
                                <td><asp:CheckBox ID="MatchPatientBirthDate" runat="server" Text="<%$Resources: Labels, PatientBirthDate %>" CssClass="DialogCheckBox"/></td>
                                <td></td>                                                        
                                <td><asp:CheckBox ID="MatchPatientSex" runat="server" Text="<%$Resources: Labels, PatientSex %>" CssClass="DialogCheckBox"/></td>
                                                                                        
                            </tr>
                            <tr>
                                <td><asp:CheckBox ID="MatchAccessionNumber" runat="server" Text="<%$Resources: Labels, AccessionNumber %>" CssClass="DialogCheckBox"/></td>
                                <td></td>                                                        
                                <td><asp:CheckBox ID="MatchIssuer" runat="server" Text="<%$Resources: Labels, IssuerOfPatientID %>" CssClass="DialogCheckBox"/></td>
                            </tr>
                        </table>
                        </asp:Panel>
                    
                    </ContentTemplate>
                    <HeaderTemplate><%= Titles.AdminPartition_AddEditDialog_StudyMatchingTabTitle%></HeaderTemplate>
                </aspAjax:TabPanel>
                <aspAjax:TabPanel ID="TabPanel3" runat="server" CssClass="DialogTabControl">
                    <ContentTemplate>
                        <asp:Panel ID="Panel5" runat="server" CssClass="DialogTabPanelContent EditPartitionDialogTab" >
                            <table width="100%">
                            <tr>
                                <td>
                                    <asp:CheckBox ID="AuditDeleteStudyCheckBox" runat="server" Text="<%$Resources: InputLabels, MaintainCopyOfStudyAfterDeletion%>" CssClass="DialogCheckBox" 
                                            ToolTip="<%$ Resources:Tooltips, ServerPartitionAddEditDialog_AuditDeleteStudy %>"/>
                                </td>
                            </tr>
                        </table>
                        </asp:Panel>
                    
                    </ContentTemplate>
                    <HeaderTemplate><%= Titles.AdminPartition_AddEditDialog_DeleteManagementTabTitle%></HeaderTemplate>                    
                </aspAjax:TabPanel>
                <aspAjax:TabPanel ID="DataAccessTab" runat="server" CssClass="DialogTabControl">
                    <HeaderTemplate>
                        <%= Titles.AdminPartition_AddEditDialog_DataAccessTabTitle%>
                    </HeaderTemplate>  
                    
                    <ContentTemplate> 
                        <asp:Panel ID="Panel6" runat="server" CssClass="DialogTabPanelContent EditPartitionDialogTab" >
                            <cc:DataAccessGroupPanel runat="server" ID="dataAccessPanel"></cc:DataAccessGroupPanel>
                        </asp:Panel>
                    </ContentTemplate>
                </aspAjax:TabPanel>
            </aspAjax:TabContainer>
        </asp:Panel>
            <table cellpadding="0" cellspacing="0" width="100%">
                <tr align="right">
                    <td>
                        <asp:Panel ID="ButtonPanel" runat="server" CssClass="DefaultModalDialogButtonPanel">
                            <ccUI:ToolbarButton ID="UpdateButton" runat="server" SkinID="<%$Image:UpdateButton%>" OnClick="OKButton_Click" ValidationGroup="AddEditServerPartitionValidationGroup" />
                            <ccUI:ToolbarButton ID="OKButton" runat="server" SkinID="<%$Image:AddButton%>" OnClick="OKButton_Click" ValidationGroup="AddEditServerPartitionValidationGroup" />
                            <ccUI:ToolbarButton ID="Cancel" runat="server" SkinID="<%$Image:CancelButton%>" OnClick="CancelButton_Click" />
                        </asp:Panel>
                    </td>
                </tr>
            </table>    
    </asp:Panel>
                                                                
    </ContentTemplate>
</ccAsp:ModalDialog>
