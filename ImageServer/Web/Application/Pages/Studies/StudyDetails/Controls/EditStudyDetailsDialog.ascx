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

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditStudyDetailsDialog.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.EditStudyDetailsDialog" %>
<%@ Import Namespace="Resources"%>
<%@ Import Namespace="System.Globalization"%>

<ccAsp:ModalDialog ID="EditStudyModalDialog" runat="server" Width="775px" Title='<%$ Resources:Titles, EditStudyDialog %>'>
<ContentTemplate>

        <script language="javascript" type="text/javascript">
            Sys.Application.add_load(seriesPage_load);
            function seriesPage_load()
            {            
                var listbox = $get('<%= ReasonListBox.ClientID %>');
                if (document.all) //IE6
                {
                    listbox.attachEvent('onchange', editReasonSelectionChanged);
                }
                else //Firefox
                {
                    listbox.addEventListener('change', editReasonSelectionChanged, false);
                }
            }
            
            function editReasonSelectionChanged()
            {
                var listbox = $get('<%= ReasonListBox.ClientID %>');
                var textbox = $get('<%= Comment.ClientID %>');
                textbox.value = listbox.options[listbox.selectedIndex].value;
                
            }
        </script>
        
        
        <asp:Panel runat="server" ID="AttachmentExistWarning" CssClass="EditStudyDialogStructureReportWarning">
                <asp:Label ID="Label8"  runat="server" Text="<%$ Resources: SR, WarningEditStudyWithAttachment %>" />
        </asp:Panel>

        <asp:ValidationSummary ID="EditStudyDetailsValidationSummary" ShowMessageBox="false" ShowSummary="true" DisplayMode="SingleParagraph"
EnableClientScript="true" runat="server" ValidationGroup="EditStudyValidationGroup" CssClass="EditStudyDialogErrorMessage" />
        <asp:Panel ID="Panel3" runat="server" DefaultButton="OKButton">
        
        
            <aspAjax:TabContainer ID="EditStudyDetailsTabContainer" runat="server" ActiveTabIndex="0" CssClass="EditStudyDialogTabControl" ForeColor="red">
                <aspAjax:TabPanel ID="PatientTabPanel" runat="server" HeaderText="PatientTabPanel" CssClass="EdityStudyDialogTabControl">
                    <ContentTemplate>
                        <table cellpadding="2" cellspacing="5" width="100%" style="background-color: #eeeeee; border: solid 1px #cccccc;">
                            
                            <tr>
                                <td valign="top" class="DialogLabelBackground"><asp:Label ID="Label6" runat="server" Text="<%$Resources: Labels, PatientName %>" CssClass="DialogTextBoxLabel" /></td>
                                <td><ccAsp:PersonNameInputPanel runat="server" ID="PatientNamePanel"  Required="true" ValidationGroup="EditStudyValidationGroup"/></td>
                            </tr>
                            <tr>
                                <td class="DialogLabelBackground"><asp:Label ID="Label2" runat="server" Text="<%$Resources: Labels, PatientID %>" CssClass="DialogTextBoxLabel" /></td>
                                <td>
                                <table cellpadding="0" cellspacing="0">
                                        <tr><td><asp:TextBox ID="PatientID" runat="server" CssClass="DialogTextBox" MaxLength="64" CausesValidation="true" ValidationGroup="EditStudyValidationGroup"></asp:TextBox>
                                    </td>
                                    <td valign="bottom">
                                        <ccAsp:InvalidInputIndicator ID="PatientIDHelp" runat="server" SkinID="InvalidInputIndicator" />
                                        <ccValidator:RegularExpressionFieldValidator
                                                        ID="RegularExpressionFieldValidator14" runat="server" ControlToValidate="PatientID"
                                                        InvalidInputCSS="DialogTextBoxInvalidInput" ValidationGroup="EditStudyValidationGroup" InvalidInputIndicatorID="PatientIDHelp"
                                                        ValidationExpression="^([^\\]){0,64}$" Text="<%$Resources: InputValidation, InvalidPatientID %>" Display="None">
                                        </ccValidator:RegularExpressionFieldValidator>
                                    </td></tr></table>
                                 </td>
                            <tr>
                                <td class="DialogLabelBackground"><asp:Label ID="GenderLabel" runat="server" Text="<%$Resources: Labels, PatientGender %>" CssClass="DialogTextBoxLabel" /></td>
                                <td>
                                    <table cellpadding="0" cellspacing="0">
                                        <tr><td>
                                            <asp:DropDownList ID="PatientGender" runat="server" CssClass="DialogDropDownList" CausesValidation="true" ValidationGroup="EditStudyValidationGroup"  >
                                                <asp:ListItem Selected="True" Text=" " Value=" " />
                                                <asp:ListItem Text="<%$Resources: SR, Male %>" Value="M" />
                                                <asp:ListItem Text="<%$Resources: SR, Female %>" Value="F" />
                                                <asp:ListItem Text="<%$Resources: SR, Other %>" Value="O" />
                                            </asp:DropDownList>
                                        </td>
                                        <td valign="bottom">
                                            <ccAsp:InvalidInputIndicator ID="PatientGenderHelp" runat="server" SkinID="InvalidInputIndicator" />
                                            <ccValidator:RegularExpressionFieldValidator
                                                        ID="RegularExpressionFieldValidator15" runat="server" ControlToValidate="PatientGender"
                                                        InvalidInputCSS="DialogTextBoxInvalidInput" 
                                                        ValidationGroup="EditStudyValidationGroup" InvalidInputIndicatorID="PatientGenderHelp"
                                                        IgnoreEmptyValue="True"
                                                        ValidationExpression="M|F|O" Text="<%$Resources: InputValidation, ThisFieldIsRequired %>" Display="None">
                                            </ccValidator:RegularExpressionFieldValidator>
                                        </td>
                                        </tr>
                                     </table>
                                </td>
                            </tr>
                            <tr>
                                <td class="DialogLabelBackground"><asp:Label ID="Label1" runat="server" Text="<%$Resources: Labels, DateOfBirth%>" CssClass="DialogTextBoxLabel" /></td>
                                <td>
                                    <table cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td valign="bottom">
                                                <ccUI:TextBox ID="PatientBirthDate" runat="server" CausesValidation="true" ValidationGroup="EditStudyValidationGroup" CssClass="DialogTextBox"></ccUI:TextBox>
                                                <ccUI:CalendarExtender ID="PatientBirthDateCalendarExtender" runat="server" TargetControlID="PatientBirthDate" CssClass="Calendar"></ccUI:CalendarExtender>
                                            </td>
                                            
                                            <td>    
                                                 <ccAsp:InvalidInputIndicator ID="PatientBirthDateHelp" runat="server" 
                                                            SkinID="InvalidInputIndicator"/>
                                            </td>
                                            <td>
                                                <asp:Label runat="server" ID="DateExampleLabel" ForeColor="Black" Font-Size="Small"/>
                                            </td>
                                            
                                            <td>
                                                <asp:LinkButton ID="ClearPatientBirthDateButton" 
                                                         Text="<%$Resources: Labels, Clear %>" runat="server" 
                                                         CssClass="DialogLinkButton" />                                         
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            <tr>
                                <td class="DialogLabelBackground"><asp:Label ID="Label3" runat="server" Text="<%$Resources: Labels, Age%>" CssClass="DialogTextBoxLabel" /></td>
                                <td>
                                    <table cellpadding="0" cellspacing="0">
                                        <tr><td>
                                        <ccUI:TextBox ID="PatientAge" runat="server" CausesValidation="true" ValidationGroup="EditStudyValidationGroup" CssClass="DialogTextBox" MaxLength="3"></ccUI:TextBox>
                                        <asp:DropDownList ID="PatientAgePeriod" runat="server" CssClass="DialogDropDownList">
                                            <asp:ListItem Value="Y" Text="<%$Resources: SR,Years %>"></asp:ListItem>
                                            <asp:ListItem Value="M" Text="<%$Resources: SR,Months %>"></asp:ListItem>
                                            <asp:ListItem Value="W" Text="<%$Resources: SR,Weeks %>"></asp:ListItem>
                                            <asp:ListItem Value="D" Text="<%$Resources: SR,Days %>"></asp:ListItem>
                                        </asp:DropDownList>
                                        </td><td valign="bottom">
                                            <ccAsp:InvalidInputIndicator ID="PatientAgeHelp" runat="server" SkinID="InvalidInputIndicator" />
                                            <ccValidator:RegularExpressionFieldValidator
                                                        ID="PatientAgeValidator" runat="server" ControlToValidate="PatientAge"
                                                        InvalidInputCSS="DialogTextBoxInvalidInput" ValidationGroup="EditStudyValidationGroup" InvalidInputIndicatorID="PatientAgeHelp"
                                                        ValidationExpression="^[^-][0-9]*$" Text="<%$Resources: InputValidation,EditStudyDialog_InvalidPatientAge %>" IgnoreEmptyValue="true" Display="None">
                                            </ccValidator:RegularExpressionFieldValidator>
                                        </td></tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                    <HeaderTemplate>
                        <%= Labels.EditStudyDialog_PatientInformation %>
                    </HeaderTemplate>
                </aspAjax:TabPanel>
                <aspAjax:TabPanel ID="TabPanel2" runat="server" HeaderText="TabPanel2">
                    <ContentTemplate>
                        <table cellpadding="2" cellspacing="5" width="100%" style="background-color: #eeeeee; border: solid 1px #cccccc;">
                            <tr>
                                <td valign="top" class="DialogLabelBackground"><asp:Label ID="Label21" runat="server" Text="<%$Resources: Labels, ReferringPhysician%>" CssClass="DialogTextBoxLabel" /></td>
                                <td>
                                    <ccAsp:PersonNameInputPanel runat="server" ID="ReferringPhysicianNamePanel" Required="false" ValidationGroup="EditStudyValidationGroup"/>
                                </td>
                            </tr>
                                <tr>
                                <td class="DialogLabelBackground"><asp:Label ID="Label14" runat="server" Text="<%$Resources: Labels, StudyDescription%>" CssClass="DialogTextBoxLabel" /></td>
                                <td>
                                    <table cellpadding="0" cellspacing="0">
                                        <tr><td><asp:TextBox ID="StudyDescription" runat="server" CausesValidation="true" MaxLength="64" ValidationGroup="EditStudyValidationGroup" CssClass="DialogTextBox" />
                                    </td>
                                    <td valign="bottom">
                                        <ccAsp:InvalidInputIndicator ID="StudyDescriptionHelp" runat="server" SkinID="InvalidInputIndicator" />
                                        <ccValidator:RegularExpressionFieldValidator
                                                        ID="RegularExpressionFieldValidator1" runat="server" ControlToValidate="StudyDescription"
                                                        InvalidInputCSS="DialogTextBoxInvalidInput" ValidationGroup="EditStudyValidationGroup" InvalidInputIndicatorID="StudyDescriptionHelp"
                                                        ValidationExpression="^([^\\]){0,64}$" 
                                                        Text="Invalid Study Description"
                                                         Display="None">
                                        </ccValidator:RegularExpressionFieldValidator>
                                    </td></tr></table>
                                </td>
                                </tr>
                                <tr>
                                <td class="DialogLabelBackground"><asp:Label ID="Label12" runat="server" Text="<%$Resources: Labels, AccessionNumber%>" CssClass="DialogTextBoxLabel" /></td>
                                <td>
                                    <table cellpadding="0" cellspacing="0"><tr><td><asp:TextBox ID="AccessionNumber" runat="server" MaxLength="16" CausesValidation="true" ValidationGroup="EditStudyValidationGroup" CssClass="DialogTextBox" /></td>
                                    <td valign="bottom"><ccAsp:InvalidInputIndicator ID="AccessionNumberHelp" runat="server" SkinID="InvalidInputIndicator" />
                                        <ccValidator:RegularExpressionFieldValidator
                                                        ID="RegularExpressionFieldValidator2" runat="server" ControlToValidate="AccessionNumber"
                                                        InvalidInputCSS="DialogTextBoxInvalidInput" ValidationGroup="EditStudyValidationGroup" InvalidInputIndicatorID="AccessionNumberHelp"
                                                        ValidationExpression="^([^\\]){0,16}$" 
                                                        Text="Invalid Accession Number" 
                                                        Display="None">
                                        </ccValidator:RegularExpressionFieldValidator></td></tr></table>
                                </td>
                                </tr>
                                <tr>
                                <td class="DialogLabelBackground"><asp:Label ID="Label11" runat="server" Text="<%$Resources: Labels, StudyDateTime%>" CssClass="DialogTextBoxLabel" /></td>
                                <td>
                                    <table cellpadding="0" cellspacing="0">
                                        <tr>
                                        <td>
                                            <ccUI:TextBox ID="StudyDate" runat="server" CausesValidation="true" CssClass="DialogTextBox" ReadOnly="true" />
                                            <ccUI:TextBox ID="StudyTimeHours" runat="server" CausesValidation="true" CssClass="DialogTextBox" Width="17" MaxLength="2" />
                                            <span style="color: black">:</span><asp:TextBox ID="StudyTimeMinutes" runat="server" CausesValidation="true" CssClass="DialogTextBox" Width="17" MaxLength="2" />
                                            <span style="color:Black ">:</span><asp:TextBox ID="StudyTimeSeconds" runat="server" CausesValidation="true" CssClass="DialogTextBox" Width="17" MaxLength="2" />
                                            <asp:LinkButton ID="ClearStudyDateTimeButton" Text="<%$Resources: Labels, Clear %>"  runat="server" CssClass="DialogLinkButton" />
                                        </td>
                                        <td>
                                        <ccAsp:InvalidInputIndicator ID="StudyDateHelp" runat="server" SkinID="InvalidInputIndicator" />
                                        <ccValidator:DateValidator
                                                        ID="StudyDateValidator" runat="server" ControlToValidate="StudyDate" 
                                                        InvalidInputCSS="DialogTextBoxInvalidInput" 
                                                        IgnoreEmptyValue="True"
                                                        Text="<%$Resources: InputValidation, InvalidDate %>"
                                                        ValidationGroup="EditStudyValidationGroup" 
                                                        InvalidInputIndicatorID="StudyDateHelp" 
                                                        Display="None"
                                                        >
                                        </ccValidator:DateValidator>
                                        <ccValidator:RegularExpressionFieldValidator
                                            ID="RegularExpressionFieldValidator16" runat="server" ControlToValidate="StudyTimeHours"
                                            InvalidInputCSS="DialogTextBoxInvalidInput" ValidationGroup="EditStudyValidationGroup" 
                                            InvalidInputIndicatorID="StudyDateHelp"
                                            ValidationExpression="^([0-9]|0[0-9]*|1[0-9]*|2[0-3])$" 
                                            IgnoreEmptyValue="true" 
                                            Text="<%$Resources: InputValidation, InvalidTime %>"
                                            Display="None">
                                        </ccValidator:RegularExpressionFieldValidator>
                                        <ccValidator:RegularExpressionFieldValidator
                                            ID="RegularExpressionFieldValidator17" runat="server" ControlToValidate="StudyTimeMinutes"
                                            InvalidInputCSS="DialogTextBoxInvalidInput" ValidationGroup="EditStudyValidationGroup" 
                                            InvalidInputIndicatorID="StudyDateHelp"
                                            IgnoreEmptyValue="true"
                                            ValidationExpression="^([0-5][0-9])*$"  
                                            Text="<%$Resources: InputValidation, InvalidTime %>"
                                            Display="None">
                                        </ccValidator:RegularExpressionFieldValidator>
                                        <ccValidator:RegularExpressionFieldValidator
                                            ID="RegularExpressionFieldValidator18" runat="server" ControlToValidate="StudyTimeSeconds"
                                            InvalidInputCSS="DialogTextBoxInvalidInput" 
                                            ValidationGroup="EditStudyValidationGroup" InvalidInputIndicatorID="StudyDateHelp"
                                            ValidationExpression="^([0-5][0-9])*$" 
                                            IgnoreEmptyValue="true"
                                            Text="<%$Resources: InputValidation, InvalidTime %>"
                                            Display="None" 
                                            >
                                        </ccValidator:RegularExpressionFieldValidator>
                                        </td>
                                        </tr>
                                        </table>
                                </tr>
                                <tr>
                                <td class="DialogLabelBackground"><asp:Label ID="Label13" runat="server" Text="<%$Resources: Labels, StudyID%>" CssClass="DialogTextBoxLabel" /></td>
                                <td>
                                    <table cellpadding="0" cellspacing="0">
                                    <tr><td><asp:TextBox ID="StudyID" runat="server" CausesValidation="true" ValidationGroup="EditStudyValidationGroup" MaxLength="16" CssClass="DialogTextBox" /></td>
                                    <td valign="bottom">
                                       <ccAsp:InvalidInputIndicator ID="StudyIDHelp" runat="server" SkinID="InvalidInputIndicator" />
                                        <ccValidator:RegularExpressionFieldValidator
                                                        ID="RegularExpressionFieldValidator3" runat="server" ControlToValidate="StudyID"
                                                        InvalidInputCSS="DialogTextBoxInvalidInput" ValidationGroup="EditStudyValidationGroup" InvalidInputIndicatorID="StudyIDHelp"
                                                        ValidationExpression="^([^\\]){0,16}$" 
                                                        Text="Invalid Study ID" Display="None">
                                        </ccValidator:RegularExpressionFieldValidator>
                                    </td></tr></table>
                                </td>
                                </tr>
                             </table>
                             <ccUI:CalendarExtender ID="StudyDateCalendarExtender" runat="server" TargetControlID="StudyDate"
                                CssClass="Calendar">
                            </ccUI:CalendarExtender>
                        
                    </ContentTemplate>
                    <HeaderTemplate>
                        <%= Labels.EditStudyDialog_StudyInformation %>
                    </HeaderTemplate>
                </aspAjax:TabPanel>
            </aspAjax:TabContainer>
            
            <div id="ReasonPanel" class="EditStudyReasonPanel">
                <table border="0">
                    
                    <tr valign="top">
                        <td>
                            <asp:Label ID="Label4" runat="server" CssClass="DialogTextBoxLabel" Text="<%$ Resources: Labels, EditStudyDialog_Reason %>"></asp:Label>                            
                        </td>
                        <td>
                        <table cellpadding="0" cellspacing="0">
                                <tr valign="top">
                                    <td>
                            <asp:DropDownList runat="server" ID="ReasonListBox" style="font-family: Arial, Sans-Serif; font-size: 14px;" />                                        
                            </td><td style="padding-left: 2px;">
                                <ccAsp:InvalidInputIndicator ID="InvalidReasonIndicator" runat="server" SkinID="InvalidInputIndicator" />
                            </td>
                            </tr></table>
                        </td>
                   </tr>
                   <tr>
                        <td valign="top">
                            <asp:Label ID="Label5" runat="server" CssClass="DialogTextBoxLabel" 
                                            Text="<%$ Resources: Labels, EditStudyDialog_Comment %>"></asp:Label> 
                             
                        </td>
                        <td>
                            <table cellpadding="0" cellspacing="0">
                                <tr valign="top">
                                    <td>
                                        <asp:TextBox  Width="400px" Rows="3" ID="Comment" runat="server" TextMode="MultiLine" style="font-family: Arial, Sans-Serif; font-size: 14px;" />                                            
                                    </td>
                                    <td valign="middle" style="padding-left: 8px;">
                                        <ccAsp:InvalidInputIndicator ID="InvalidCommentIndicator" runat="server" SkinID="InvalidInputIndicator" />
                                    </td>

                                </tr>
                            </table>
                            
                            
                        </td>
                    </tr>
                    <tr id="ReasonSavePanel" runat="server">
                        <td>
                            <asp:Label ID="Label7" runat="server" CssClass="DialogTextBoxLabel" 
                                                Text="<%$ Resources: Labels, EditStudyDialog_SaveReasonAs %>"></asp:Label> 
                                 
                        </td>
                        <td>
                            <table cellpadding="0" cellspacing="0">
                              <tr valign="top">
                                <td>
                                    <asp:TextBox runat="server" ID="SaveReasonAsName" style="font-family: Arial, Sans-Serif; font-size: 14px;"/>
                                </td>
                                <td valign="middle" style="padding-left: 8px;">
                                    <ccAsp:InvalidInputIndicator ID="InvalidSaveReasonAsNameInputIndicator" runat="server" SkinID="InvalidInputIndicator" />
                                </td>
                              </tr>
                            </table>
                        </td>
                </tr>
                </table>
            </div>                
        </div>
        </asp:Panel>
            <table cellpadding="0" cellspacing="0" width="100%">
                <tr align="right">
                    <td>
                        <asp:Panel ID="Panel1" runat="server" CssClass="DefaultModalDialogButtonPanel">
                            <ccUI:ToolbarButton ID="OKButton" runat="server" SkinID="<%$Image:UpdateButton%>" OnClick="OKButton_Click" ValidationGroup="EditStudyValidationGroup" />
                            <ccUI:ToolbarButton ID="Cancel" runat="server" SkinID="<%$Image:CancelButton%>" OnClick="CancelButton_Click" />
                        </asp:Panel>

                    </td>
                </tr>
            </table>

       <ccValidator:ConditionalRequiredFieldValidator ID="ReasonValidator" runat="server"
                                                ControlToValidate="ReasonListBox" InvalidInputIndicatorID="InvalidReasonIndicator" 
                                                ValidationGroup='EditStudyValidationGroup'
                                                Text="<%$Resources: InputValidation, EditStudyDialog_MissingReason %>" Display="None" InvalidInputCSS="DialogTextBoxInvalidInput"></ccValidator:ConditionalRequiredFieldValidator>
       <ccValidator:ConditionalRequiredFieldValidator ID="CommentValidator" runat="server"
                                                ControlToValidate="Comment" InvalidInputIndicatorID="InvalidCommentIndicator" 
                                                ValidationGroup='EditStudyValidationGroup'
                                                Text="<%$Resources: InputValidation, EditStudyDialog_MissingComment %>" Display="None" InvalidInputCSS="DialogTextBoxInvalidInput"></ccValidator:ConditionalRequiredFieldValidator>
       <ccValidator:ConditionalRequiredFieldValidator ID="SaveReasonAsNameValidator" runat="server"
                                                ControlToValidate="SaveReasonAsName" InvalidInputIndicatorID="InvalidSaveReasonAsNameInputIndicator" 
                                                ValidationGroup='EditStudyValidationGroup'
                                                Text="<%$Resources: InputValidation, EditStudyDialog_MissingName %>" Display="None" InvalidInputCSS="DialogTextBoxInvalidInput"></ccValidator:ConditionalRequiredFieldValidator>
</ContentTemplate>
</ccAsp:ModalDialog>