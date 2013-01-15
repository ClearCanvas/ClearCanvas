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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.Utilities.Manifest;

namespace ClearCanvas.Desktop.Help
{
	public partial class AboutForm : Form, IAboutDialog
	{
		public AboutForm()
		{
			this.SuspendLayout();

			InitializeComponent();

			_version.Text = String.Format(AboutSettings.Default.VersionTextFormat, ProductInformation.GetVersion(true, true, true));
			_copyright.Text = ProductInformation.Copyright;
			_license.Text = ProductInformation.License;
			_closeButton.Text = SR.LabelClose;

            _manifest.Visible = !ManifestVerification.Valid;
           
			if (AboutSettings.Default.UseSettings)
			{
				try
				{
					var stream = OpenResourceStream();
					if (stream != null)
					{
						// GDI+ resource management quirk: don't dispose the source stream (or create an independent copy of the bitmap)
						BackgroundImage = new Bitmap(stream);
						ClientSize = BackgroundImage.Size;
					}
				}
				catch (Exception ex)
				{
					Platform.Log(LogLevel.Warn, ex, "Failed to resolve about dialog resources.");
				}

				this._copyright.Location = AboutSettings.Default.CopyrightLocation;
				this._copyright.Size = AboutSettings.Default.CopyrightSize;
				this._copyright.AutoSize = AboutSettings.Default.CopyrightAutoSize;
				this._copyright.ForeColor = AboutSettings.Default.CopyrightForeColor;
				this._copyright.Font = AboutSettings.Default.CopyrightFontBold ? new Font(this._copyright.Font, FontStyle.Bold) : this._copyright.Font;
				this._copyright.TextAlign = AboutSettings.Default.CopyrightTextAlign;

				this._version.Location = AboutSettings.Default.VersionLocation;
				this._version.Size = AboutSettings.Default.VersionSize;
				this._version.AutoSize = AboutSettings.Default.VersionAutoSize;
				this._version.ForeColor = AboutSettings.Default.VersionForeColor;
				this._version.Font = AboutSettings.Default.VersionFontBold ? new Font(this._version.Font, FontStyle.Bold) : this._version.Font;
				this._version.TextAlign = AboutSettings.Default.VersionTextAlign;

				this._license.Visible = AboutSettings.Default.LicenseVisible;
				this._license.Location = AboutSettings.Default.LicenseLocation;
				this._license.Size = AboutSettings.Default.LicenseSize;
				this._license.AutoSize = AboutSettings.Default.LicenseAutoSize;
				this._license.ForeColor = AboutSettings.Default.LicenseForeColor;
				this._license.Font = AboutSettings.Default.LicenseFontBold ? new Font(this._license.Font, FontStyle.Bold) : this._license.Font;
				this._license.TextAlign = AboutSettings.Default.LicenseTextAlign;

                this._manifest.Location = AboutSettings.Default.ManifestLocation;
                this._manifest.Size = AboutSettings.Default.ManifestSize;
                this._manifest.AutoSize = AboutSettings.Default.ManifestAutoSize;
                this._manifest.ForeColor = AboutSettings.Default.ManifestForeColor;
				this._manifest.Font = AboutSettings.Default.ManifestFontBold ? new Font(this._manifest.Font, FontStyle.Bold) : this._manifest.Font;
                this._manifest.TextAlign = AboutSettings.Default.ManifestTextAlign;
			    
				this._closeButton.Location = ComputeLocation(_closeButton.Size, AboutSettings.Default.CloseButtonLocation, AboutSettings.Default.CloseButtonAnchor);
				this._closeButton.LinkColor = AboutSettings.Default.CloseButtonLinkColor;

                AddExtraLabels();

			}

			this.ResumeLayout();

			this._closeButton.Click += new EventHandler(OnCloseClicked);
		}

        private void AddExtraLabels()
        {
            var text = ProductStateInfo.GetProductLicenseStateDescription();
            if (!string.IsNullOrEmpty(text))
            {
                var label = new Label()
                {
                    Text = text,
                    Visible = AboutSettings.Default.EvaluationVisible,
                    BackColor = System.Drawing.Color.Transparent,
                    Location = AboutSettings.Default.EvaluationLocation,
                    Size = AboutSettings.Default.EvaluationSize,
                    AutoSize = AboutSettings.Default.EvaluationAutoSize,
                    ForeColor = AboutSettings.Default.EvaluationForeColor,
                    TextAlign = AboutSettings.Default.EvaluationTextAlign
                };
                if (AboutSettings.Default.EvaluationFontBold)
                    label.Font = new Font(label.Font, FontStyle.Bold);

                this.Controls.Add(label);
            }
            

            if (LicenseInformation.DiagnosticUse != LicenseDiagnosticUse.Allowed)
            {
                text = LicenseInformation.DiagnosticUse == LicenseDiagnosticUse.None
                               ? SR.LabelNotForClinicalUse
                               : SR.LabelNotForHumanDiagnosticUse;

                var notForDiagnosticUseLabel = new Label()
                {
                    Text = text,
                    Visible = AboutSettings.Default.NotForDiagnosticUseVisible,
                    BackColor = System.Drawing.Color.Transparent,
                    Location = AboutSettings.Default.NotForDiagnosticUseLocation,
                    Size = AboutSettings.Default.NotForDiagnosticUseSize,
                    AutoSize = AboutSettings.Default.NotForDiagnosticUseAutoSize,
                    ForeColor = AboutSettings.Default.NotForDiagnosticUseForeColor,
                    TextAlign = AboutSettings.Default.NotForDiagnosticUseTextAlign
                };
                if (AboutSettings.Default.NotForDiagnosticUseFontBold)
                    notForDiagnosticUseLabel.Font = new Font(notForDiagnosticUseLabel.Font, FontStyle.Bold);

                this.Controls.Add(notForDiagnosticUseLabel);

            }
        }

		private static Point ComputeLocation(Size size, Point referencePoint, ContentAlignment referenceRelation)
		{
			var offset = new Size();
			switch (referenceRelation)
			{
				case ContentAlignment.TopCenter:
				case ContentAlignment.MiddleCenter:
				case ContentAlignment.BottomCenter:
					offset.Width = size.Width/2;
					break;
				case ContentAlignment.TopRight:
				case ContentAlignment.MiddleRight:
				case ContentAlignment.BottomRight:
					offset.Width = size.Width;
					break;
			}
			switch (referenceRelation)
			{
				case ContentAlignment.MiddleLeft:
				case ContentAlignment.MiddleCenter:
				case ContentAlignment.MiddleRight:
					offset.Height = size.Height/2;
					break;
				case ContentAlignment.BottomLeft:
				case ContentAlignment.BottomCenter:
				case ContentAlignment.BottomRight:
					offset.Height = size.Height;
					break;
			}
			return referencePoint - offset;
		}

		private static Stream OpenResourceStream()
		{
			var oemPath = System.IO.Path.Combine(Platform.InstallDirectory, @"oem\about.png");
			if (File.Exists(oemPath))
				return File.OpenRead(oemPath);

			var assemblyName = AboutSettings.Default.BackgroundImageAssemblyName;
			var resourceName = AboutSettings.Default.BackgroundImageResourceName;
			if (!string.IsNullOrEmpty(assemblyName) && !string.IsNullOrEmpty(resourceName))
			{
				var assembly = Assembly.Load(assemblyName);
				if (assembly != null)
				{
					var streamName = assemblyName + @"." + resourceName;
					return assembly.GetManifestResourceStream(streamName);
				}
			}

			return null;
		}

		private void OnCloseClicked(object sender, EventArgs e)
		{
			Close();
		}

		private void OnClosePreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				e.IsInputKey = true;
				Close();
			}
	}

		protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
		{
			base.OnPreviewKeyDown(e);
			if (e.KeyCode == Keys.Escape)
			{
				e.IsInputKey = true;
				Close();
			}
		}
	}
}