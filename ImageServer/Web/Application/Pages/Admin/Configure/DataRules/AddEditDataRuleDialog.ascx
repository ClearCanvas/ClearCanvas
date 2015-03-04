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
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddEditDataRuleDialog.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.DataRules.AddEditDataRuleDialog" %>
	

<link rel="stylesheet" href="../../../../Scripts/CodeMirror/js/lib/codemirror.css">
<script type="text/javascript" src="../../../../Scripts/CodeMirror/js/lib/CodeMirror.js"></script>
<script type="text/javascript" src="../../../../Scripts/CodeMirror/js/mode/xml/xml.js"></script>
<script type="text/javascript" src="../../../../Scripts/CodeMirror/js/addon/fold/foldcode.js"></script>
<script type="text/javascript" src="../../../../Scripts/CodeMirror/js/addon/fold/foldgutter.js"></script>
<script type="text/javascript" src="../../../../Scripts/CodeMirror/js/addon/fold/brace-fold.js"></script>
<script type="text/javascript" src="../../../../Scripts/CodeMirror/js/addon/fold/xml-fold.js"></script>
<script type="text/javascript" src="../../../../Scripts/CodeMirror/js/addon/fold/comment-fold.js"></script>

<style type="text/css">
	.CodeMirror {
		height: 200px;
		width: 600px;
	}
	.cm-s-default .cm-tag {
		color: #36880F;
		font-weight: bold;
		background: rgba(255, 0, 0, 0);
		text-shadow: 1px 1px 1px rgb(175, 175, 175);
	}
		
	.cm-s-default .cm-attribute {
		color: rgb(124, 124, 124);
	}
		.cm-s-default .cm-string {
		color: #0068DA;
		font-weight: bold;
	}
	
	.RuleName {
		width: 400px;
	}
	
	.RuleXmlPanel {
		width: 400px;
	}
</style>

<script type="text/javascript">
	function OnRuleXmlTabActivated() {

		setTimeout(function () {
			CodeMirrorEditor.refresh();
		}, 200);
	}
	function HighlightXML() {
		var textBoxId = "<%= RuleXmlTextBox.ClientID %>";
		var textbox = document.getElementById(textBoxId);
		CodeMirrorEditor = CodeMirror.fromTextArea(textbox, {
			mode: { name: "xml", alignCDATA: true },
			lineNumbers: true,
			lineWrapping: true,
			value: textbox.value
		});

	}
</script>

<asp:ScriptManagerProxy runat="server">
    <Services>
        <asp:ServiceReference Path="DataRuleSamples.asmx" />
    </Services>
</asp:ScriptManagerProxy>
<ccAsp:ModalDialog ID="ModalDialog" runat="server">
    <ContentTemplate>
        <asp:ValidationSummary ID="EditDataRuleValidationSummary" ShowMessageBox="false"
            ShowSummary="true" DisplayMode="SingleParagraph" EnableClientScript="true" runat="server"
            ValidationGroup="AddEditDataRuleValidationGroup" CssClass="DialogValidationErrorMessage" />
        <div class="DialogPanelContent">
            <asp:Table ID="Table2" runat="server" SkinID="NoSkin" CellSpacing="3" CellPadding="3">
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server" VerticalAlign="Top">
                        <asp:Label ID="RuleNameLabel" runat="server" Text="<%$Resources: InputLabels, ServerRuleName %>"
                            CssClass="DialogTextBoxLabel"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell runat="server" HorizontalAlign="left" ColumnSpan="2">
                    	<table>
                    		<tr>
                    			<td>
                    				<asp:TextBox ID="RuleNameTextBox" runat="server" ValidationGroup="AddEditDataRuleValidationGroup"
										CssClass="DialogTextBox RuleName"></asp:TextBox>
								</td>
								<td>
									<ccAsp:InvalidInputIndicator ID="RuleNameHelp" runat="server" SkinID="InvalidInputIndicator" />
									<ccValidator:ConditionalRequiredFieldValidator ID="RuleNameValidator" runat="server"
										ControlToValidate="RuleNameTextBox" InvalidInputCSS="DialogTextBoxInvalidInput RuleName"
										ValidationGroup="AddEditDataRuleValidationGroup" Text="<%$Resources: InputValidation,InvalidRuleName %>"
										InvalidInputIndicatorID="RuleNameHelp" Display="None" />
								</td>
                    		</tr>
                    	</table>
                       
                        
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow1" runat="server">
                    <asp:TableCell runat="server" VerticalAlign="Top">
                        <asp:Label runat="server" Text="<%$Resources: InputLabels, Enabled %>"
                            CssClass="DialogTextBoxLabel" />
                    </asp:TableCell>
                    <asp:TableCell runat="server" HorizontalAlign="left" Width="100%">
                        <asp:CheckBox ID="EnabledCheckBox" runat="server" Text="" Checked="true" ToolTip="<%$Resources: Tooltips, AdminRules_AddEditDialog_Enabled %>"
                            CssClass="DialogCheckBox" />
                    </asp:TableCell>
                    <asp:TableCell runat="server"></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server" VerticalAlign="Top">
                    <asp:Label runat="server" Text="<%$Resources: InputLabels, ServerRuleDefaultRule %>"
                            CssClass="DialogTextBoxLabel" />
                    </asp:TableCell>
                    <asp:TableCell runat="server" HorizontalAlign="left" Width="100%">
                        <asp:CheckBox ID="DefaultCheckBox" runat="server" Text="" Checked="false" ToolTip="<% $Resources: Tooltips,AdminRules_AddEditDialog_Default %>"
                            CssClass="DialogCheckBox" />
                    </asp:TableCell>
                    <asp:TableCell runat="server"></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server" VerticalAlign="Top">
                    <asp:Label runat="server" Text="<%$Resources: InputLabels, ServerRuleExemptRule %>"
                            CssClass="DialogTextBoxLabel" />
                    </asp:TableCell>
                    <asp:TableCell runat="server" HorizontalAlign="left" Width="100%">
                        <asp:CheckBox ID="ExemptRuleCheckBox" runat="server" Text="" Checked="false" ToolTip="<%$Resources: Tooltips,AdminRules_AddEditDialog_Exempt %>"
                            CssClass="DialogCheckBox" />
                    </asp:TableCell>
                    <asp:TableCell runat="server"></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server" VerticalAlign="Top">
                        <asp:Label ID="Label3" runat="server" Text="<%$Resources: InputLabels, AuthorityGroupsDataAccess %>"
                            CssClass="DialogTextBoxLabel" />
                    </asp:TableCell>
                    <asp:TableCell runat="server" HorizontalAlign="left" Width="100%">
                        <div class="DialogCheckBoxList">
                            <asp:CheckBoxList ID="AuthorityGroupCheckBoxList" runat="server" TextAlign="Right"
                                RepeatColumns="1">
                            </asp:CheckBoxList>
                        </div>
                    </asp:TableCell>
                    <asp:TableCell runat="server" />
                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server" VerticalAlign="Top">
                        <asp:Label ID="SelectSampleRuleLabel" runat="server" Text="<%$Resources: InputLabels, SelectSampleRule %>"
                            CssClass="DialogTextBoxLabel"></asp:Label>
                        <br />
                        <asp:DropDownList ID="SampleRuleDropDownList" runat="server" CssClass="DialogDropDownList" />
                    </asp:TableCell>
                    <asp:TableCell runat="server" HorizontalAlign="left" Width="100%">
                        <div class="DialogCheckBoxList" style="overflow:hidden">
                            <asp:TextBox ID="RuleXmlTextBox" runat="server" EnableViewState="true" Width="100%"
                               Height="100%" TextMode="MultiLine" CssClass="DialogTextBox RuleXmlPanel" BackColor="White"></asp:TextBox>
                        </div>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <ccAsp:InvalidInputIndicator ID="InvalidRuleHint" runat="server" SkinID="InvalidInputIndicator" />
                        <ccValidator:ServerRuleValidator runat="server" ID="DataRuleValidator" ControlToValidate="RuleXmlTextBox"
                            InputName="Data Access Rule XML" InvalidInputCSS="DialogTextBoxInvalidInput" InvalidInputIndicatorID="InvalidRuleHint"
                            ServicePath="/Services/ValidationServices.asmx" ServiceOperation="ValidateServerRule"
                            ParamsFunction="ValidationServerRuleParams" Text="<%$Resources: InputValidation,InvalidDataRule %>" Display="None"
                            ValidationGroup="AddEditDataRuleValidationGroup" />
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </div>
        <table width="100%" cellspacing="0" cellpadding="0">
            <tr align="right">
                <td>
                    <asp:Panel ID="Panel3" runat="server" CssClass="DefaultModalDialogButtonPanel">
                        <ccUI:ToolbarButton ID="UpdateButton" runat="server" SkinID="<%$Image:UpdateButton%>"
                            OnClick="OKButton_Click" ValidationGroup="AddEditDataRuleValidationGroup" OnClientClick="UpdateRuleXML()" />
                        <ccUI:ToolbarButton ID="OKButton" runat="server" SkinID="<%$Image:AddButton%>" OnClick="OKButton_Click"
                            ValidationGroup="AddEditDataRuleValidationGroup" OnClientClick="UpdateRuleXML()" />
                        <ccUI:ToolbarButton ID="CancelButton" runat="server" SkinID="<%$Image:CancelButton%>"
                            OnClick="CancelButton_Click" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </ContentTemplate>
</ccAsp:ModalDialog>
