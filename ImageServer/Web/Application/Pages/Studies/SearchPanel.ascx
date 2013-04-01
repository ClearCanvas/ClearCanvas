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


<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SearchPanel.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.SearchPanel" %>

<%@ Register Src="StudyListGridView.ascx" TagName="StudyListGridView" TagPrefix="localAsp" %>
<%@ Register Src="StudyDetails/Controls/DeleteStudyConfirmDialog.ascx" TagName="DeleteStudyConfirmDialog" TagPrefix="localAsp" %>



<asp:UpdatePanel ID="SearchUpdatePanel" runat="server" UpdateMode="conditional">
    <ContentTemplate>
        <script type="text/Javascript">

            Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(MultiSelect);
            Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(InputHover);

            function MultiSelect() {

                $("#<%=ModalityListBox.ClientID %>").multiSelect({
                    selectAllText: "<%= SR.All %>",
                    noneSelected: '',
                    oneOrMoreSelected: '*',
                    dropdownStyle: 'width: 100px;' /* The list is long and there will  be a vertial scrollbar. 
                                            It's ok to set the width explicitly here so that the items are not obscured by the scrollbar. 
                                            Note: the text for the each item in this list does not change */
                });

                $("#<%=StatusListBox.ClientID %>").multiSelect({
                    noneSelected: '',
                    oneOrMoreSelected: '*'
                });
            }
        </script>

        <asp:Table runat="server">
            <asp:TableRow>
                <asp:TableCell HorizontalAlign="right" VerticalAlign="Bottom">
                    <asp:Table runat="server" CellPadding="0" CellSpacing="0">
                        <asp:TableRow>
                            <asp:TableCell runat="server" HorizontalAlign="Left">
                                <asp:Panel runat="server" ID="SearchFieldsContainer" CssClass="SearchPanelContent" DefaultButton="SearchButton">
                                    <table cellpadding="0" cellspacing="0" border="0"> <%-- dummy table used to "clear" the default width for inner table tags--%><tr><td>
                                                <asp:Table ID="Table1" runat="server" CellPadding="0" CellSpacing="0" Width="0%">
                                                    <asp:TableRow>
                                                        <asp:TableCell>
                                                            <asp:Table ID="Table2" runat="server" CellPadding="0" CellSpacing="0" BorderWidth="0">
                                                                <asp:TableRow>
                                            <asp:TableCell runat="server" ID="OrganizationFilter" HorizontalAlign="left" Visible="False" VerticalAlign="bottom">
                                                <asp:Label ID="ResponsibleOrganizationLabel" runat="server" Text="<%$Resources: SearchFieldLabels,ResponsibleOrganization%>" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False" /><br />
                                                <ccUI:TextBox ID="ResponsibleOrganization" runat="server" CssClass="SearchTextBox" ToolTip="<%$Resources: Tooltips,SearchByResponsibleOrganization%>"/>
                                                                    </asp:TableCell>
                                            <asp:TableCell runat="server" ID="ResponsiblePersonFilter" HorizontalAlign="left" Visible="False" VerticalAlign="bottom">
                                                <asp:Label ID="ResponsiblePersonLabel" runat="server" Text="<%$Resources: SearchFieldLabels,ResponsiblePerson%>" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False" /><br />
                                                <ccUI:TextBox ID="ResponsiblePerson" runat="server" CssClass="SearchTextBox" ToolTip="<%$Resources: Tooltips,SearchByResponsiblePerson%>"/>
                                                                    </asp:TableCell>
                                                                    <asp:TableCell HorizontalAlign="left" VerticalAlign="bottom">
                                                <asp:Label ID="Label1" runat="server" Text="<%$Resources: SearchFieldLabels,PatientName %>" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False" /><br />
                                                <ccUI:TextBox ID="PatientName" runat="server" CssClass="SearchTextBox" ToolTip="<%$Resources: Tooltips,SearchByPatientName %>" />
                                                                    </asp:TableCell>
                                                                    <asp:TableCell HorizontalAlign="left" VerticalAlign="bottom">
                                                <asp:Label ID="Label2" runat="server" Text="<%$Resources: SearchFieldLabels, PatientID%>" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False" /><br />
                                                <ccUI:TextBox ID="PatientId" runat="server" CssClass="SearchTextBox" ToolTip="<%$Resources: Tooltips,SearchByPatientID%>" />
                                                                    </asp:TableCell>
                                                                    <asp:TableCell HorizontalAlign="left" VerticalAlign="bottom">
                                                <asp:Label ID="Label3" runat="server" Text="<%$Resources: SearchFieldLabels, AccessionNumber%>" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False" /><br />
                                                <ccUI:TextBox ID="AccessionNumber" runat="server" CssClass="SearchTextBox" ToolTip="<%$Resources: Tooltips,SearchByAccessionNumber%>"/>
                                                                    </asp:TableCell>
                                                                    <asp:TableCell HorizontalAlign="left" VerticalAlign="bottom">
                                                <asp:Label ID="Label5" runat="server" Text="<%$Resources: SearchFieldLabels,FromDate %>" CssClass="SearchTextBoxLabel" EnableViewState="false"/>
                                                <asp:LinkButton ID="ClearFromStudyDateButton" runat="server" Text="X" CssClass="SmallLink" style="margin-left: 0px;"/><br />
                                                <ccUI:TextBox ID="FromStudyDate" runat="server" CssClass="SearchDateBox" ReadOnly="true" ToolTip="<%$Resources: Tooltips,SearchByStudyDate%>"  />
                                                                    </asp:TableCell>
                                                                    <asp:TableCell HorizontalAlign="left" VerticalAlign="bottom">
                                                <asp:Label ID="Label7" runat="server" Text="<%$Resources: SearchFieldLabels,ToDate %>" CssClass="SearchTextBoxLabel" EnableViewState="false"/>
                                                <asp:LinkButton ID="ClearToStudyDateButton" runat="server" Text="X" CssClass="SmallLink" style="margin-left: 0px;"/><br />
                                                <ccUI:TextBox ID="ToStudyDate" runat="server" CssClass="SearchDateBox" ReadOnly="true" ToolTip="<%$Resources: Tooltips,SearchByStudyDate%>" />                                                
                                                                    </asp:TableCell>
                                                                    <asp:TableCell HorizontalAlign="left" VerticalAlign="bottom">
                                                <asp:Label ID="Label4" runat="server" Text="<%$Resources: SearchFieldLabels,Description%>" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False" /><br />
                                                <ccUI:TextBox ID="StudyDescription" runat="server"  CssClass="SearchTextBox" ToolTip="<%$Resources: Tooltips,SearchByDescription%>" />
                                                                    </asp:TableCell>
                                                                    <asp:TableCell HorizontalAlign="left" VerticalAlign="bottom">
                                                <asp:Label ID="Label9" runat="server" Text="<%$Resources: SearchFieldLabels,ReferringPhysician%>" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False" /><br />
                                                <ccUI:TextBox ID="ReferringPhysiciansName" runat="server"  CssClass="SearchTextBox" ToolTip="<%$Resources: Tooltips,SearchByRefPhysician%>" />
                                                                    </asp:TableCell>
                                                                    <asp:TableCell HorizontalAlign="left" VerticalAlign="bottom">
                                                <asp:Label ID="Label6" runat="server" Text="<%$Resources: SearchFieldLabels,Modality%>" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False" /><br />
                                                <asp:ListBox runat="server" id="ModalityListBox" SelectionMode="Multiple">
                                                                            <asp:ListItem Value="CR">CR</asp:ListItem>
                                                                            <asp:ListItem Value="CT">CT</asp:ListItem>
                                                                            <asp:ListItem Value="DX">DX</asp:ListItem>
                                                                            <asp:ListItem Value="ES">ES</asp:ListItem>
                                                                            <asp:ListItem Value="KO">KO</asp:ListItem>
                                                                            <asp:ListItem Value="MG">MG</asp:ListItem>
                                                                            <asp:ListItem Value="MR">MR</asp:ListItem>
                                                                            <asp:ListItem Value="NM">NM</asp:ListItem>
                                                                            <asp:ListItem Value="OT">OT</asp:ListItem>
                                                                            <asp:ListItem Value="PR">PR</asp:ListItem>
                                                                            <asp:ListItem Value="PT">PT</asp:ListItem>
                                                                            <asp:ListItem Value="RF">RF</asp:ListItem>
                                                                            <asp:ListItem Value="SC">SC</asp:ListItem>
                                                                            <asp:ListItem Value="US">US</asp:ListItem>
                                                                            <asp:ListItem Value="XA">XA</asp:ListItem>
                                                                        </asp:ListBox>
                                                                    </asp:TableCell>
                                            <asp:TableCell ID="TableCell1" runat="server" HorizontalAlign="left" VerticalAlign="bottom">
                                                <asp:Label ID="Label8" runat="server" Text="<%$Resources: SearchFieldLabels,StudyStatus%>" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False" /><br />
                                                <asp:ListBox runat="server" id="StatusListBox" SelectionMode="Multiple">                                       
                                                </asp:ListBox>
                                                                    </asp:TableCell>
                                                                    <asp:TableCell VerticalAlign="bottom">
                                                <asp:Panel ID="Panel1" runat="server" CssClass="SearchButtonPanel"><ccUI:ToolbarButton ID="SearchButton" runat="server" SkinID="<%$Image:SearchIcon%>" OnClick="SearchButton_Click" /></asp:Panel>
                                                                    </asp:TableCell>
                                                                </asp:TableRow>
                                                            </asp:Table>
                                                        </asp:TableCell>
                                                    </asp:TableRow>
                                                </asp:Table>
                                    </td></tr></table>
                                    
                                </asp:Panel>
                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>

                        <ccUI:CalendarExtender ID="FromStudyDateCalendarExtender" runat="server" TargetControlID="FromStudyDate"
                            CssClass="Calendar">
                        </ccUI:CalendarExtender>
                        <ccUI:CalendarExtender ID="ToStudyDateCalendarExtender" runat="server" TargetControlID="ToStudyDate"
                            CssClass="Calendar">
                        </ccUI:CalendarExtender>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>
                    <table width="100%" cellpadding="3" cellspacing="0" class="ToolbarButtonPanel">
                            <tr><td >
                                <asp:UpdatePanel ID="ToolBarUpdatePanel" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Panel ID="ToolbarButtons" runat="server" CssClass="ToolbarButtons">
                                        <ccUI:ToolbarButton ID="ViewImagesButton" runat="server" SkinID="<%$Image:ViewImagesButton%>" />
                                        <ccUI:ToolbarButton ID="ViewStudyDetailsButton" runat="server" SkinID="<%$Image:ViewDetailsButton%>" />
                                        <ccUI:ToolbarButton ID="MoveStudyButton" runat="server" SkinID="<%$Image:MoveButton%>" />
                                        <ccUI:ToolbarButton ID="DeleteStudyButton" runat="server" SkinID="<%$Image:DeleteButton%>" OnClick="DeleteStudyButton_Click" />
                                        <ccUI:ToolbarButton ID="RestoreStudyButton" runat="server" SkinID="<%$Image:RestoreButton%>" OnClick="RestoreStudyButton_Click" />
                                        <ccUI:ToolbarButton ID="AssignAuthorityGroupsButton" runat="server" SkinID="<%$Image:AddDataAccessButton%>" OnClick="AssignAuthorityGroupsButton_Click" />
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                        </td></tr>
                        <tr><td>

                                <asp:Panel ID="Panel2" runat="server" CssClass="SearchPanelResultContainer">
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                 <tr><td><ccAsp:GridPager ID="GridPagerTop" runat="server" /></td></tr>                        
                                <tr><td style="background-color: white;">
                                <localAsp:StudyListGridView id="StudyListGridView" runat="server" Height="500px"></localAsp:StudyListGridView></td></tr>
                                    </table>
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="SearchButton" EventName="Click" />
    </Triggers>
</asp:UpdatePanel>

<ccAsp:MessageBox ID="MessageBox" runat="server" />
<ccAsp:MessageBox ID="RestoreMessageBox" runat="server" />   
<ccAsp:MessageBox ID="ConfirmStudySearchMessageBox" runat="server" MessageType="YESNO" />

