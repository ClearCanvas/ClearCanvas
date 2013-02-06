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


<%@ Import namespace="ClearCanvas.ImageServer.Web.Common.Utilities"%>

<%@ Import Namespace="ClearCanvas.ImageServer.Web.Application.Helpers" %>
<%@ Control Language="C#" AutoEventWireup="true" Codebehind="DeleteStudyConfirmDialog.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.DeleteStudyConfirmDialog" %>
<%@ Import Namespace="Resources"%>

<ccAsp:ModalDialog ID="ModalDialog" runat="server" Title="<%$ Resources:Titles, DeleteStudyConfirmDialogTitle %>" Width="800px">
    <ContentTemplate>
    
        <script language="javascript" type="text/javascript">
            Sys.Application.add_load(page_load);
            function page_load()
            {            
                var listbox = $get('<%= ReasonListBox.ClientID %>');
                if (document.all) //IE6
                {
                    listbox.attachEvent('onchange', studyReasonSelectionChanged);
                }
                else //Firefox
                {
                    listbox.addEventListener('change', studyReasonSelectionChanged, false);
                }
            }
            
            function studyReasonSelectionChanged()
            {
                var listbox = $get('<%= ReasonListBox.ClientID %>');
                var textbox = $get('<%= Comment.ClientID %>');
                
                textbox.value = listbox.options[listbox.selectedIndex].value;
                
            }
        </script>
    
        <div class="ContentPanel">
        <div class="DialogPanelContent">
            <div id="StudyList">
                <table border="0" cellspacing="5" width="100%">
                    <tr>
                    <td>
                        <table width="100%">
                            <tr>
                                <td>
                                    <asp:Label ID="Label1" runat="server" CssClass="DialogTextBoxLabel" 
                                                    Text="<%$ Resources:Labels, DeleteStudyConfirmDialog_StudyListingLabel %>"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <div class="DeleteStudiesTableContainer" style="background: white">
                                    <asp:Repeater runat="server" ID="StudyListing">
                                        <HeaderTemplate>
                                            <table  cellspacing="0" width="100%" class="DeleteStudiesConfirmTable">
                                                <tr>
                                                    <th style="white-space:nowrap" class="GlobalGridViewHeader"><%= ColumnHeaders.PatientName %></th>
                                                    <th style="white-space:nowrap" class="GlobalGridViewHeader"><%= ColumnHeaders.PatientID%></th>
                                                    <th style="white-space:nowrap" class="GlobalGridViewHeader"><%= ColumnHeaders.StudyDate%></th>
                                                    <th style="white-space:nowrap" class="GlobalGridViewHeader"><%= ColumnHeaders.StudyDescription%></th>
                                                    <th style="white-space:nowrap" class="GlobalGridViewHeader"><%= ColumnHeaders.AccessionNumber%></th>
                                                    <th style="white-space:nowrap" class="GlobalGridViewHeader"><%= ColumnHeaders.Modality%></th>
                                                </tr>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <tr class="GlobalGridViewRow">
                                                <td>
                                                    <%# HtmlUtility.GetEvalValue(Container.DataItem, "PatientsName", "&nbsp;")%>
                                                </td>
                                                <td>
                                                    <%# HtmlUtility.GetEvalValue(Container.DataItem, "PatientId", "&nbsp;")%>
                                                </td>
                                                <td>
                                                    <%# HtmlUtility.GetEvalValue(Container.DataItem, "StudyDate", "&nbsp;")%>
                                                </td>
                                                <td>
                                                    <%# HtmlUtility.GetEvalValue(Container.DataItem, "StudyDescription", "&nbsp;")%>
                                                </td>
                                                <td>
                                                    <%# HtmlUtility.GetEvalValue(Container.DataItem, "AccessionNumber", "&nbsp;")%>
                                                </td>
                                                <td>
                                                    <%# HtmlUtility.GetEvalValue(Container.DataItem, "Modalities", "&nbsp;")%>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            </table>
                                        </FooterTemplate>
                                    </asp:Repeater>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                </table>
            </div>
            <div id="ReasonPanel" class="DeleteStudyReasonPanel">
                <table border="0">
                   <tr valign="top">
                        <td>
                            <asp:Label ID="Label5" runat="server" CssClass="DialogTextBoxLabel" Text="<%$Resources: Labels, DeleteStudyConfirmDialog_Reason %>"></asp:Label>                            
                        </td>
                        <td>
                            <table cellpadding="0" cellspacing="0">
                                <tr valign="top">
                                    <td>
                                        <asp:DropDownList runat="server" ID="ReasonListBox" style="font-family: Arial, Sans-Serif; font-size: 14px;" Width="175"/>                                        
                                    </td><td style="padding-left: 2px;">
                                        <ccAsp:InvalidInputIndicator ID="InvalidReasonIndicator" runat="server" SkinID="InvalidInputIndicator" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                   </tr>
                   <tr>
                        <td valign="top">
                            <asp:Label ID="Label6" runat="server" CssClass="DialogTextBoxLabel" Text="<%$Resources: Labels, DeleteStudyConfirmDialog_Comment %>"></asp:Label> 
                        </td>
                        <td>
                            <table cellpadding="0" cellspacing="0">
                                <tr valign="top">
                                    <td>
                                        <asp:TextBox  Width="400px" Rows="3" ID="Comment" runat="server" TextMode="MultiLine" style="font-family: Arial, Sans-Serif; font-size: 14px;" />                                            
                                    </td>
                                    <td>
                                        <ccAsp:InvalidInputIndicator ID="InvalidCommentIndicator" runat="server" SkinID="InvalidInputIndicator" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr id="ReasonSavePanel" runat="server">
                        <td>
                            <asp:Label ID="Label4" runat="server" CssClass="DialogTextBoxLabel" 
                                                Text="<%$Resources: Labels, DeleteStudyConfirmDialog_SaveReasonAs %>"></asp:Label> 
                                 
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
        
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr align="right">
                <td>
                    <asp:Panel ID="Panel1" runat="server" CssClass="DefaultModalDialogButtonPanel">
                        <ccUI:ToolbarButton ID="DeleteButton" runat="server" SkinID="<%$Image:OKButton%>" 
                            OnClick="DeleteButton_Clicked" ValidationGroup='StudyGroup'/>
                        <ccUI:ToolbarButton ID="CancelButton" runat="server" SkinID="<%$Image:CancelButton%>"
                            OnClick="CancelButton_Clicked" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
        </div>
       <ccValidator:ConditionalRequiredFieldValidator ID="ReasonValidator" runat="server"
                                                ControlToValidate="ReasonListBox" InvalidInputIndicatorID="InvalidReasonIndicator" 
                                                ValidationGroup='StudyGroup'
                                                Text="<%$Resources: InputValidation, DeleteStudyConfirmDialog_MissingReason %>" Display="None" InvalidInputCSS="DialogTextBoxInvalidInput"></ccValidator:ConditionalRequiredFieldValidator>
       <ccValidator:ConditionalRequiredFieldValidator ID="CommentValidator" runat="server"
                                                ControlToValidate="Comment" InvalidInputIndicatorID="InvalidCommentIndicator" 
                                                ValidationGroup='StudyGroup'
                                                Text="<%$Resources: InputValidation,DeleteStudyConfirmDialog_MissingComment %>" Display="None" InvalidInputCSS="DialogTextBoxInvalidInput"></ccValidator:ConditionalRequiredFieldValidator>                                                
       <ccValidator:ConditionalRequiredFieldValidator ID="SaveReasonAsNameValidator" runat="server"
                                                ControlToValidate="SaveReasonAsName" InvalidInputIndicatorID="InvalidSaveReasonAsNameInputIndicator" 
                                                ValidationGroup='StudyGroup'
                                                Text="<%$Resources: InputValidation,DeleteStudyConfirmDialog_MissingName %>" Display="None" InvalidInputCSS="DialogTextBoxInvalidInput"></ccValidator:ConditionalRequiredFieldValidator>           
    </ContentTemplate>
</ccAsp:ModalDialog>
