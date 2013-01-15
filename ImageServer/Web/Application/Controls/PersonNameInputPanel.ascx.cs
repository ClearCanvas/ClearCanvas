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
using System.Web.UI;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageServer.Web.Common.WebControls;

[assembly: WebResource("ClearCanvas.ImageServer.Web.Application.Controls.PersonNameInputPanel.js", "application/x-javascript")]

namespace ClearCanvas.ImageServer.Web.Application.Controls
{
    public partial class PersonNameInputPanel : UserControl
    {
        private PersonName _personName;
        private string _validationGroup = "PersonNameInputValidationGroup";
        public PersonName PersonName
        {
            set { _personName = value; }
            get
            {
                string singlebyte =
                    StringUtilities.Combine(new[]
                                                        {
                                                            PersonLastName.Text,
                                                            PersonGivenName.Text,
                                                            PersonMiddleName.Text,
                                                            PersonTitle.Text,
                                                            PersonSuffix.Text
                                                        }, DicomConstants.DicomSeparator, false);

                singlebyte = singlebyte.TrimEnd('^');

                string ideographicName =
                    StringUtilities.Combine(new[]
                                                        {
                                                            IdeographicLastName.Text,
                                                            IdeographicGivenName.Text,
                                                            IdeographicMiddleName.Text,
                                                            IdeographicTitle.Text,
                                                            IdeographicSuffix.Text
                                                        }, DicomConstants.DicomSeparator, false);

                ideographicName = ideographicName.TrimEnd('^');

                string phoneticName =
                    StringUtilities.Combine(new[]
                                                        {
                                                            PhoneticLastName.Text,
                                                            PhoneticGivenName.Text,
                                                            PhoneticMiddleName.Text,
                                                            PhoneticTitle.Text,
                                                            PhoneticSuffix.Text
                                                        }, DicomConstants.DicomSeparator, false);

                phoneticName = phoneticName.TrimEnd('^');


                string dicomName = StringUtilities.Combine(new[]
                                                        {
                                                            singlebyte,
                                                            ideographicName,
                                                            phoneticName
                                                        }, "=", false);

                dicomName = dicomName.TrimEnd('=');

                return new PersonName(dicomName);
            }
        }

        public bool Required { get; set; }

        public string ValidationGroup
        {
            get
            {
                return _validationGroup;
            }
            set { _validationGroup = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PhoneticNameRowIndicator.ImageUrl = ImageServerConstants.ImageURLs.PhoneticName;
            IdeographyNameIndicator.ImageUrl = ImageServerConstants.ImageURLs.IdeographyName;
            var script =
                new ScriptTemplate(typeof(PersonNameInputPanel).Assembly,
                                   "ClearCanvas.ImageServer.Web.Application.Controls.PersonNameInputPanel.js");
            script.Replace("@@CLIENTID@@", ClientID);
            script.Replace("@@PHONETIC_ROW_CLIENTID@@", PhoneticRow.ClientID);
            script.Replace("@@IDEOGRAPHY_ROW_CLIENTID@@", IdeographicRow.ClientID);


            ShowOtherNameFormatButton.OnClientClick = ClientID + "_ShowOtherNameFormats(); return false;";

            Page.ClientScript.RegisterClientScriptBlock(GetType(), ClientID, script.Script, true);

            PersonGivenNameValidator.IgnoreEmptyValue = !Required;
            PersonLastNameValidator.IgnoreEmptyValue = !Required;

            //Dynamically set all of the Validation Groups
            PersonTitle.ValidationGroup = ValidationGroup;
            PersonTitleValidator.ValidationGroup = ValidationGroup;
            PersonGivenName.ValidationGroup = ValidationGroup;
            PersonGivenNameValidator.ValidationGroup = ValidationGroup;
            PersonMiddleName.ValidationGroup = ValidationGroup;
            PersonMiddleNameValidator.ValidationGroup = ValidationGroup;
            PersonLastName.ValidationGroup = ValidationGroup;
            PersonLastNameValidator.ValidationGroup = ValidationGroup;
            PersonSuffix.ValidationGroup = ValidationGroup;
            PersonSuffixValidator.ValidationGroup = ValidationGroup;
            PhoneticTitle.ValidationGroup = ValidationGroup;
            PhoneticGivenName.ValidationGroup = ValidationGroup;
            PhoneticMiddleName.ValidationGroup = ValidationGroup;
            PhoneticLastName.ValidationGroup = ValidationGroup;
            PhoneticSuffix.ValidationGroup = ValidationGroup;
            IdeographicTitle.ValidationGroup = ValidationGroup;
            IdeographicGivenName.ValidationGroup = ValidationGroup;
            IdeographicMiddleName.ValidationGroup = ValidationGroup;
            IdeographicLastName.ValidationGroup = ValidationGroup;
            IdeographicSuffix.ValidationGroup = ValidationGroup;
        }

        public override void DataBind()
        {
            base.DataBind();

            if (_personName != null)
            {
                PersonLastName.Text = _personName.LastName;
                PersonMiddleName.Text = _personName.MiddleName;
                PersonGivenName.Text = _personName.FirstName;
                PersonTitle.Text = _personName.Title;
                PersonSuffix.Text = _personName.Suffix;

                if (_personName.Phonetic.IsEmpty)
                {
                    //PhoneticRow.Visible = false;
                    PhoneticRow.Style.Add(HtmlTextWriterStyle.Visibility, "hidden");
                    PhoneticRow.Style.Add(HtmlTextWriterStyle.Display, "none");

                }
                else
                {
                    PhoneticLastName.Text = _personName.Phonetic.FamilyName;
                    PhoneticGivenName.Text = _personName.Phonetic.GivenName;
                    PhoneticMiddleName.Text = _personName.Phonetic.MiddleName;
                    PhoneticTitle.Text = _personName.Phonetic.Prefix;
                    PhoneticSuffix.Text = _personName.Phonetic.Suffix;
                }

                if (_personName.Ideographic.IsEmpty)
                {
                    //IdeographicRow.Visible = false;

                    IdeographicRow.Style.Add(HtmlTextWriterStyle.Visibility, "hidden");
                    IdeographicRow.Style.Add(HtmlTextWriterStyle.Display, "none");

                }
                else
                {
                    IdeographicLastName.Text = _personName.Ideographic.FamilyName;
                    IdeographicGivenName.Text = _personName.Ideographic.GivenName;
                    IdeographicMiddleName.Text = _personName.Ideographic.MiddleName;
                    IdeographicTitle.Text = _personName.Ideographic.Prefix;
                    IdeographicSuffix.Text = _personName.Ideographic.Suffix;
                }
            }
            else
            {
                // only show the single byte row
                PhoneticRow.Style.Add(HtmlTextWriterStyle.Visibility, "hidden");
                PhoneticRow.Style.Add(HtmlTextWriterStyle.Display, "none");

                IdeographicRow.Style.Add(HtmlTextWriterStyle.Visibility, "hidden");
                IdeographicRow.Style.Add(HtmlTextWriterStyle.Display, "none");
            }
        }
    }
}