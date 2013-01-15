#region License

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

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Rules;
using Resources;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerRules
{
    public partial class AddEditServerRuleDialog : UserControl
    {
        #region private variables

        // The server partitions that the new device can be associated with
        // This list will be determined by the user level permission.

        private bool _editMode;
        private ServerPartition _partition;
        private ServerRule _rule;

        #endregion

        #region public members

        /// <summary>
        /// Sets the list of partitions users allowed to pick.
        /// </summary>
        public ServerPartition Partition
        {
            set
            {
                _partition = value;
                ViewState["_ServerPartition"] = value;
            }

            get { return _partition; }
        }

        /// <summary>
        /// Sets or gets the value which indicates whether the dialog is in edit mode.
        /// </summary>
        public bool EditMode
        {
            get { return _editMode; }
            set
            {
                _editMode = value;
                ViewState["_EditMode"] = value;
            }
        }

        /// <summary>
        /// Sets/Gets the current editing device.
        /// </summary>
        public ServerRule ServerRule
        {
            set
            {
                _rule = value;
                // put into viewstate to retrieve later
                if (_rule != null)
                    ViewState["_EdittedRule"] = _rule.GetKey();
            }
            get { return _rule; }
        }

        #endregion // public members

        #region Events

        #region Delegates

        /// <summary>
        /// Defines the event handler for <seealso cref="OKClicked"/>.
        /// </summary>
        /// <param name="rule">The device being added.</param>
        public delegate void OnOKClickedEventHandler(ServerRule rule);

        #endregion

        /// <summary>
        /// Occurs when users click on "OK".
        /// </summary>
        public event OnOKClickedEventHandler OKClicked;

        #endregion Events

        #region Protected Methods

        private static Dictionary<ServerRuleTypeEnum, IList<ServerRuleApplyTimeEnum>> LoadRuleTypes(object[] extensions)
        {
            var ruleTypeList = new Dictionary<ServerRuleTypeEnum, IList<ServerRuleApplyTimeEnum>>();
            foreach (ISampleRule extension in extensions)
            {
                if (!ruleTypeList.ContainsKey(extension.Type)  && !extension.Type.Equals(ServerRuleTypeEnum.DataAccess))
                    ruleTypeList.Add(extension.Type, extension.ApplyTimeList);
            }

            return ruleTypeList;
        }

        private static string GetJavascriptForSampleRule(ServerRuleTypeEnum typeEnum, object[] extensions)
        {
            string sampleList = string.Empty;

            foreach (ISampleRule extension in extensions)
            {
                if (extension.Type.Equals(typeEnum))
                {
                    sampleList +=
                        String.Format(
                            @"        myEle = document.createElement('option') ;
                    myEle.value = '{0}';
                    myEle.text = '{1}' ;
                    if(navigator.appName == 'Microsoft Internet Explorer') sampleList.add(myEle);
                    else sampleList.add(myEle, null);",
                            extension.Name, extension.Description);
                }
            }
            string enumList = string.Empty;

            foreach (ISampleRule extension in extensions)
            {
                if (extension.Type.Equals(typeEnum))
                {
                    foreach (ServerRuleApplyTimeEnum applyTimeEnum in extension.ApplyTimeList)
                    {
                        enumList +=
                            String.Format(
                                @"myEle = document.createElement('option') ;
                    myEle.value = '{0}';
                    myEle.text = '{1}';
                    if(navigator.appName == 'Microsoft Internet Explorer') applyTimeList.add(myEle);
                    else applyTimeList.add(myEle, null);",
                                applyTimeEnum.Lookup, ServerEnumDescription.GetLocalizedDescription(applyTimeEnum));
                    }
                    break;
                }
            }

            return
                String.Format(
                    @"if (val == '{0}')
                {{
					{1}
                    myEle = document.createElement('option') ;
                    myEle.value = '';
                    myEle.text = '' ;
                    if(navigator.appName == 'Microsoft Internet Explorer') sampleList.add(myEle);
                    else sampleList.add(myEle, null);
                    {2}
                }}",
                    typeEnum.Lookup, enumList, sampleList);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            var ep = new SampleRuleExtensionPoint();
            object[] extensions = ep.CreateExtensions();

            ServerPartitionTabContainer.ActiveTabIndex = 0;

            Dictionary<ServerRuleTypeEnum, IList<ServerRuleApplyTimeEnum>> ruleTypeList = LoadRuleTypes(extensions);


            SampleRuleDropDownList.Attributes.Add("onchange", "webServiceScript(this, this.SelectedIndex);");
            RuleTypeDropDownList.Attributes.Add("onchange", "selectRuleType(this);");

            RuleTypeDropDownList.TextChanged += delegate
                                                    {
                                                        ServerRuleValidator.RuleTypeControl = RuleTypeDropDownList.SelectedValue;
                                                    };

            string javascript =
                @"<script type='text/javascript'>
            function ValidationServerRuleParams()
            {
                control = document.getElementById('" +RuleXmlTextBox.ClientID +@"');
                params = new Array();
                params.serverRule=escape(CodeMirrorEditor.getCode());
				var oList = document.getElementById('" +RuleTypeDropDownList.ClientID +@"');
				params.ruleType = oList.options[oList.selectedIndex].value;
                return params;
            }

            function selectRuleType(oList, selectedIndex)
            {                         
                var val = oList.value; 
                var sampleList = document.getElementById('" +SampleRuleDropDownList.ClientID +@"');
                var applyTimeList = document.getElementById('" +RuleApplyTimeDropDownList.ClientID +@"');
                
                for (var q=sampleList.options.length; q>=0; q--) sampleList.options[q]=null;
                for (var q=applyTimeList.options.length; q>=0; q--) applyTimeList.options[q]=null;
				";

            bool first = true;
            foreach (ServerRuleTypeEnum type in ruleTypeList.Keys)
            {
                if (!first)
                {
                    javascript += "else ";
                }
                else
                    first = false;

                javascript += GetJavascriptForSampleRule(type, extensions);
            }

            javascript +=
            @"}

            // This function calls the Web Service method.  
            function webServiceScript(oList)
            {
                var type = oList.value;
             
                ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerRules.ServerRuleSamples.GetXml(type,
                    OnSucess, OnError);
            }
            function OnError(result)
            {
                alert('Error: ' + result.get_message());
            }

            // This is the callback function that
            // processes the Web Service return value.
            function OnSucess(result)
            {
                var oList = document.getElementById('" +SampleRuleDropDownList.ClientID +@"');
                var sValue = oList.options[oList.selectedIndex].value;
             
                RsltElem = document.getElementById('" +RuleXmlTextBox.ClientID +@"');

                //Set the value on the TextArea and then set the value in the Editor.
                //CodeMirror doesn't monitor changes to the textarea.
                RsltElem.value = result;
                CodeMirrorEditor.setCode(RsltElem.value);
            }
           
            function pageLoad(){
                $find('" + ModalDialog.PopupExtenderID + @"').add_shown(HighlightXML);
            }

            function HighlightXML() {
                CodeMirrorEditor = CodeMirror.fromTextArea('" + RuleXmlTextBox.ClientID + @"', {parserfile: 'parsexml.js',path: '../../../../Scripts/CodeMirror/js/', stylesheet: '../../../../Scripts/CodeMirror/css/xmlcolors.css'});
            }

	        function UpdateRuleXML() {
                RsltElem = document.getElementById('" + RuleXmlTextBox.ClientID + @"');	
                RsltElem.value = CodeMirrorEditor.getCode();    
	        }
  
            var CodeMirrorEditor = null;
            </script>";

            Page.ClientScript.RegisterClientScriptBlock(GetType(), ClientID, javascript);

            Page.ClientScript.RegisterClientScriptInclude(GetType(), "CodeMirrorLibrary",
                                                          "../../../../Scripts/CodeMirror/js/codemirror.js");

            EditServerRuleValidationSummary.HeaderText = ErrorMessages.EditServerRuleValidationError;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                if (ViewState["_EditMode"] != null)
                    _editMode = (bool) ViewState["_EditMode"];

                if (ViewState["_ServerPartition"] != null)
                    _partition = (ServerPartition) ViewState["_ServerPartition"];

                if (ViewState["_EdittedRule"] != null)
                {
                    var ruleKey = ViewState["_EdittedRule"] as ServerEntityKey;
                    _rule = ServerRule.Load(ruleKey);
                }
            }
        }

        protected void OKButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                SaveData();
                if (OKClicked != null)
                {
                    OKClicked(ServerRule);
                }

                Close();
            }
            else
            {
                Show();
            }
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion Protected Methods

        #region Private Methods

        private void SaveData()
        {
            if (_rule == null)
            {
                _rule = new ServerRule();
            }


            if (RuleXmlTextBox.Text.Length > 0)
            {
                _rule.RuleXml = new XmlDocument();
                _rule.RuleXml.Load(new StringReader(RuleXmlTextBox.Text));
            }

            _rule.RuleName = RuleNameTextBox.Text;

            _rule.ServerRuleTypeEnum = ServerRuleTypeEnum.GetEnum(RuleTypeDropDownList.SelectedItem.Value);

            var ep = new SampleRuleExtensionPoint();
            object[] extensions = ep.CreateExtensions();

            Dictionary<ServerRuleTypeEnum, IList<ServerRuleApplyTimeEnum>> ruleTypeList = LoadRuleTypes(extensions);

            if (ruleTypeList.ContainsKey(_rule.ServerRuleTypeEnum))
            {
                string val = Request[RuleApplyTimeDropDownList.UniqueID];
                foreach (ServerRuleApplyTimeEnum applyTime in ruleTypeList[_rule.ServerRuleTypeEnum])
                {
                    _rule.ServerRuleApplyTimeEnum = applyTime;
                    if (val.Equals(applyTime.Lookup))
                    {
                        _rule.ServerRuleApplyTimeEnum = applyTime;
                        break;
                    }
                }
            }

            _rule.Enabled = EnabledCheckBox.Checked;
            _rule.DefaultRule = DefaultCheckBox.Checked;
            _rule.ServerPartitionKey = Partition.GetKey();
            _rule.ExemptRule = ExemptRuleCheckBox.Checked;
        }

        #endregion Private Methods

        #region Public methods

        /// <summary>
        /// Displays the add/edit device dialog box.
        /// </summary>
        public void Show()
        {
            //If the validation failed, keep everything as is, and 
            //make sure the dialog stays visible.
            if (!Page.IsValid)
            {
                RuleXmlTabPanel.TabIndex = 0;
                ModalDialog.Show();
                return;
            }

            // update the dropdown list
            RuleApplyTimeDropDownList.Items.Clear();
            RuleTypeDropDownList.Items.Clear();
            RuleXmlTabPanel.TabIndex = 0;
            ServerPartitionTabContainer.ActiveTabIndex = 0;

            var ep = new SampleRuleExtensionPoint();
            object[] extensions = ep.CreateExtensions();

            Dictionary<ServerRuleTypeEnum, IList<ServerRuleApplyTimeEnum>> ruleTypeList = LoadRuleTypes(extensions);

            if (EditMode)
            {
                ModalDialog.Title = SR.DialogEditServerRuleTitle;
                OKButton.Visible = false;
                UpdateButton.Visible = true;

                DefaultCheckBox.Checked = _rule.DefaultRule;
                EnabledCheckBox.Checked = _rule.Enabled;
                ExemptRuleCheckBox.Checked = _rule.ExemptRule;

                //if (_rule.DefaultRule)
                //	DefaultCheckBox.Enabled = false;

                RuleNameTextBox.Text = _rule.RuleName;

                SampleRuleDropDownList.Visible = false;
                SelectSampleRuleLabel.Visible = false;

                // Fill in the drop down menus
                RuleTypeDropDownList.Items.Add(new ListItem(
                                                   ServerEnumDescription.GetLocalizedDescription(_rule.ServerRuleTypeEnum),
                                                   _rule.ServerRuleTypeEnum.Lookup));

                IList<ServerRuleApplyTimeEnum> list = new List<ServerRuleApplyTimeEnum>();


                if (ruleTypeList.ContainsKey(_rule.ServerRuleTypeEnum))
                    list = ruleTypeList[_rule.ServerRuleTypeEnum];

                foreach (ServerRuleApplyTimeEnum applyTime in list)
                    RuleApplyTimeDropDownList.Items.Add(new ListItem(
                                                            ServerEnumDescription.GetLocalizedDescription(applyTime),
                                                            applyTime.Lookup));


                if (RuleApplyTimeDropDownList.Items.FindByValue(_rule.ServerRuleApplyTimeEnum.Lookup) != null)
                    RuleApplyTimeDropDownList.SelectedValue = _rule.ServerRuleApplyTimeEnum.Lookup;

                RuleTypeDropDownList.SelectedValue = _rule.ServerRuleTypeEnum.Lookup;


                // Fill in the Rule XML
                var sw = new StringWriter();

                var xmlSettings = new XmlWriterSettings();

                xmlSettings.Encoding = Encoding.UTF8;
                xmlSettings.ConformanceLevel = ConformanceLevel.Fragment;
                xmlSettings.Indent = true;
                xmlSettings.NewLineOnAttributes = false;
                xmlSettings.CheckCharacters = true;
                xmlSettings.IndentChars = "  ";

                XmlWriter tw = XmlWriter.Create(sw, xmlSettings);

                _rule.RuleXml.WriteTo(tw);

                tw.Close();

                RuleXmlTextBox.Text = sw.ToString();

                ServerRuleValidator.RuleTypeControl = RuleTypeDropDownList.SelectedValue;
            }
            else
            {
                ModalDialog.Title = SR.DialogAddServerRuleTitle;
                OKButton.Visible = false;
                UpdateButton.Visible = true;

                DefaultCheckBox.Checked = false;
                EnabledCheckBox.Checked = true;
                ExemptRuleCheckBox.Checked = false;

                RuleNameTextBox.Text = string.Empty;
                RuleXmlTextBox.Text = string.Empty;

                SampleRuleDropDownList.Visible = true;
                SelectSampleRuleLabel.Visible = true;

                // Do the drop down lists
                bool first = true;
                var list = new List<ServerRuleTypeEnum>();
                list.AddRange(ruleTypeList.Keys);

                // Sort the list by description
                list.Sort(
                    new Comparison<ServerRuleTypeEnum>(
                        delegate(ServerRuleTypeEnum type1, ServerRuleTypeEnum type2) { return type1.Description.CompareTo(type2.Description); }));

                foreach (ServerRuleTypeEnum type in list)
                {
                    if (first)
                    {
                        first = false;
                        SampleRuleDropDownList.Items.Clear();
                        SampleRuleDropDownList.Items.Add(new ListItem(string.Empty, string.Empty));

                        foreach (ISampleRule extension in extensions)
                        {
                            if (extension.Type.Equals(type))
                            {
                                SampleRuleDropDownList.Items.Add(new ListItem(extension.Description, extension.Name));
                            }
                        }

                        foreach (ServerRuleApplyTimeEnum applyTime in ruleTypeList[type])
                            RuleApplyTimeDropDownList.Items.Add(new ListItem(
                                                                    ServerEnumDescription.GetLocalizedDescription(applyTime),
                                                                    applyTime.Lookup));
                    }

                    RuleTypeDropDownList.Items.Add(new ListItem(
                                                       ServerEnumDescription.GetLocalizedDescription(type), type.Lookup));
                }

                ServerRuleValidator.RuleTypeControl = RuleTypeDropDownList.SelectedValue;
            }

            ModalDialog.Show();
            return;
        }

        public void Close()
        {
            ModalDialog.Hide();
        }

        #endregion
    }
}