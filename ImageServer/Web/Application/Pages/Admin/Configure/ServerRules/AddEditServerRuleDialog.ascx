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


<%@ Control Language="C#" AutoEventWireup="true" Codebehind="AddEditServerRuleDialog.ascx.cs"
	Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerRules.AddEditServerRuleDialog" %>
<%@ Import Namespace="Resources"%>
<%@ Register Src="~/Controls/UsersGuideLink.ascx" TagPrefix="cc" TagName="HelpLink" %>

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
		width: 800px;
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
</style>

<script type="text/javascript">

	window.onload = function() {
		var lastUpdate = new Date().getTime();
		var lastKeyPress = false;
		var checkInterval = setInterval(function() {
			if (lastKeyPress == true) {
				var currentTime = new Date().getTime();
				if (currentTime - lastUpdate > 5000) {
					keepSessionAlive();
					lastKeyPress = false;
					lastUpdate = currentTime;
				}
			}
		}, 2500); // 5 seconds

		$(document).keydown(function() {
			lastKeyPress = true;
		});
	};

	function OnRuleXmlTabActivated() {
		
		setTimeout(function() {
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
	
	function keepSessionAlive() {
		var url = '<%= Page.ResolveClientUrl("~/KeepSessionAlive.aspx") %>';
	    	$.ajax({ type: 'GET', url: url, async: true });
	}
</script>

<asp:ScriptManagerProxy runat="server">
	<Services>
		<asp:ServiceReference Path="ServerRuleSamples.asmx" />
	</Services>
</asp:ScriptManagerProxy>
<ccAsp:ModalDialog ID="ModalDialog" runat="server" Width="800px">
	<ContentTemplate>	
            <asp:ValidationSummary ID="EditServerRuleValidationSummary" ShowMessageBox="false" ShowSummary="true" DisplayMode="SingleParagraph"
                EnableClientScript="true" runat="server" ValidationGroup="AddEditServerRuleValidationGroup" CssClass="DialogValidationErrorMessage" />   			
			<aspAjax:TabContainer ID="ServerPartitionTabContainer" runat="server" ActiveTabIndex="0"
				CssClass="DialogTabControl">
				<aspAjax:TabPanel ID="GeneralTabPanel" runat="server" HeaderText="GeneralTabPanel" 
					TabIndex="0" CssClass="DialogTabControl">
					<ContentTemplate>
							<table id="Table1" runat="server" width="100%">
								<tr>
									<td colspan="5">
										<table width="300">
											<tr>
												<td>
													<asp:Label ID="RuleNameLabel" runat="server" Text="<%$Resources: InputLabels, ServerRuleName %>" CssClass="DialogTextBoxLabel"></asp:Label><br />
													<asp:TextBox ID="RuleNameTextBox" runat="server" Width="285" ValidationGroup="AddEditServerRuleValidationGroup" CssClass="DialogTextBox"></asp:TextBox>
												</td>
												<td valign="bottom" align="center">
													<ccAsp:InvalidInputIndicator ID="RuleNameHelp" runat="server" SkinID="InvalidInputIndicator"/>
													<ccValidator:ConditionalRequiredFieldValidator ID="RuleNameValidator" runat="server"
														ControlToValidate="RuleNameTextBox" InvalidInputCSS="DialogTextBoxInvalidInput" ValidationGroup="AddEditServerRuleValidationGroup"
														Text="Rule must have a name" InvalidInputIndicatorID="RuleNameHelp" Display="None"/>
												</td>
											</tr>
										</table>
									</td>
								</tr>
								<tr>
									<td colspan="2">
										<table width="100%">
											<tr>
												<td>
													<asp:Label ID="RuleTypeLabel" runat="server" Text="<%$Resources: InputLabels, ServerRuleType %>" CssClass="DialogTextBoxLabel"/><br />
													<asp:DropDownList ID="RuleTypeDropDownList" runat="server" Width="125" CssClass="DialogDropDownList"/>
												</td>
											</tr>
										</table>
									</td>
									<td colspan="2">
										<table width="100%">
											<tr>
												<td>
													<asp:Label ID="RuleApplyTimeLabel" runat="server" Text="<%$Resources: InputLabels, ServerRuleApplyTime %>" CssClass="DialogTextBoxLabel"/><br />
													<asp:DropDownList ID="RuleApplyTimeDropDownList" runat="server" Width="50%" CssClass="DialogDropDownList"/>
												</td>
												<td></td>
											</tr>
										</table>
									</td>
									<td></td>
								</tr>
								<tr>
									<td colspan="2">
										<asp:CheckBox ID="EnabledCheckBox" runat="server" Text="<%$Resources: InputLabels, Enabled %>" Checked="true" ToolTip="<%$Resources: Tooltips, AdminRules_AddEditDialog_Enabled %>" CssClass="DialogCheckBox"/>
									</td>
									<td>
										<asp:CheckBox ID="DefaultCheckBox" runat="server" Text="<%$Resources: InputLabels, ServerRuleDefaultRule %>" Checked="false"
											ToolTip="<% $Resources: Tooltips,AdminRules_AddEditDialog_Default %>" CssClass="DialogCheckBox" />
                                        <cc:HelpLink ID="HelpLink1" runat="server" TopicID="Rules_Engine" SubTopicID="DefaultRules" Target="_blank" />
									</td>
									<td>
										<asp:CheckBox ID="ExemptRuleCheckBox" runat="server" Text="<%$Resources: InputLabels, ServerRuleExemptRule %>" Checked="false"
											ToolTip="<%$Resources: Tooltips,AdminRules_AddEditDialog_Exempt %>" CssClass="DialogCheckBox" />
                                         <cc:HelpLink ID="HelpLink2" runat="server" TopicID="Rules_Engine" SubTopicID="DefaultRules" Target="_blank" />
									</td>
									<td></td>
								</tr>
							</table>
					</ContentTemplate>
					<HeaderTemplate><%= Titles.AdminServerRules_AddEditDialog_GeneralTabTitle%></HeaderTemplate>
				</aspAjax:TabPanel>
				<aspAjax:TabPanel ID="RuleXmlTabPanel" runat="server" HeaderText="TabPanel2" CssClass="RuleXmlTab" OnClientClick="OnRuleXmlTabActivated">
					<ContentTemplate>
							<table width="100%" cellpadding="5" cellspacing="5">
								<tr>
									<td>
										<asp:Label ID="SelectSampleRuleLabel" runat="server" Text="<%$Resources: InputLabels, SelectSampleRule %>" CssClass="DialogTextBoxLabel"></asp:Label><br />
										<asp:DropDownList ID="SampleRuleDropDownList" runat="server" CssClass="DialogDropDownList"/>
									</td>
								</tr>
								<tr>
									<td>
										<div style="border: solid 1px #618FAD;" >
										    <asp:TextBox ID="RuleXmlTextBox" runat="server" EnableViewState="true" Width="100%"
											    Rows="16" TextMode="MultiLine" CssClass="DialogTextArea" BackColor="White"></asp:TextBox>
                                        </div>											
									</td>
									<td>
										<ccAsp:InvalidInputIndicator ID="InvalidRuleHint" runat="server" SkinID="InvalidInputIndicator" />
										<ccValidator:ServerRuleValidator runat="server" ID="ServerRuleValidator" ControlToValidate="RuleXmlTextBox"
											InputName="Server Rule XML" InvalidInputCSS="DialogTextBoxInvalidInput" InvalidInputIndicatorID="InvalidRuleHint"
											ServicePath="/Services/ValidationServices.asmx" ServiceOperation="ValidateServerRule"
											ParamsFunction="ValidationServerRuleParams" Text="Invalid Server Rule"
											Display="None" ValidationGroup="AddEditServerRuleValidationGroup"  />
									</td>
								</tr>
							</table>
					</ContentTemplate>
					<HeaderTemplate><%= Titles.AdminServerRules_AddEditDialog_RuleXMLTabTitle%>
					</HeaderTemplate>
				</aspAjax:TabPanel>
			</aspAjax:TabContainer>
            <table width="100%" cellspacing="0" cellpadding="0">
                <tr align="right">
                    <td>
                            <asp:Panel ID="Panel3" runat="server" CssClass="DefaultModalDialogButtonPanel">
                                <ccUI:ToolbarButton ID="UpdateButton" runat="server" SkinID="<%$Image:UpdateButton%>" OnClick="OKButton_Click" ValidationGroup="AddEditServerRuleValidationGroup" OnClientClick="UpdateRuleXML()" />
                                <ccUI:ToolbarButton ID="OKButton" runat="server" SkinID="<%$Image:AddButton%>" OnClick="OKButton_Click" ValidationGroup="AddEditServerRuleValidationGroup" OnClientClick="UpdateRuleXML()" />
                                <ccUI:ToolbarButton ID="CancelButton" runat="server" SkinID="<%$Image:CancelButton%>" OnClick="CancelButton_Click" />
                            </asp:Panel>
                    </td>
                </tr>
            </table>
	</ContentTemplate>
</ccAsp:ModalDialog>
