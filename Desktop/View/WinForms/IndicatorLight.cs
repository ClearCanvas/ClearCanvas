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

using System.ComponentModel;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
	/// <summary>
	/// Provides a visual indicator light control.
	/// </summary>
	[DefaultEvent("LinkClicked")]
	public partial class IndicatorLight : UserControl
	{
		private IndicatorLightStatus _status;
		private readonly string[] _lightHoverText = new string[3];
		private readonly string[] _linkHoverText = new string[3];

		/// <summary>
		/// Constructor.
		/// </summary>
		public IndicatorLight()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Gets or sets the text that is displayed next to the indicator light.
		/// </summary>
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Description("The text that is displayed next to the indicator light.")]
		public override string Text
		{
			get { return _link.Text; }
			set { _link.Text = value; }
		}

		/// <summary>
		/// Gets or sets the hover-text displayed over the indicator light when it is in the green state.
		/// </summary>
		[Description("Hover-text displayed over the indicator light when it is in the green state.")]
		public string LightHoverTextGreen
		{
			get { return GetLightHoverText(IndicatorLightStatus.Green); }
			set
			{
				SetLightHoverText(IndicatorLightStatus.Green, value);
				UpdateToolTips();
			}
		}

		/// <summary>
		/// Gets or sets the hover-text displayed over the indicator light when it is in the yellow state.
		/// </summary>
		[Description("Hover-text displayed over the indicator light when it is in the yellow state.")]
		public string LightHoverTextYellow
		{
			get { return GetLightHoverText(IndicatorLightStatus.Yellow); }
			set
			{
				SetLightHoverText(IndicatorLightStatus.Yellow, value);
				UpdateToolTips();
			}
		}

		/// <summary>
		/// Gets or sets the hover-text displayed over the indicator light when it is in the red state.
		/// </summary>
		[Description("Hover-text displayed over the indicator light when it is in the red state.")]
		public string LightHoverTextRed
		{
			get { return GetLightHoverText(IndicatorLightStatus.Red); }
			set
			{
				SetLightHoverText(IndicatorLightStatus.Red, value);
				UpdateToolTips();
			}
		}

		[Description("Specifies whether the link portion of the control is visible or not.")]
		public bool LinkVisible
		{
			get { return _link.Visible; }
			set { _link.Visible = value; }
		}

		/// <summary>
		/// Gets or sets the hover-text displayed over the link portion of the control, when in the green state.
		/// </summary>
		[Description("Hover-text displayed over the link portion of the control, when in the green state.")]
		public string LinkHoverTextGreen
		{
			get { return GetLinkHoverText(IndicatorLightStatus.Green); }
			set
			{
				SetLinkHoverText(IndicatorLightStatus.Green, value);
				UpdateToolTips();
			}
		}

		/// <summary>
		/// Gets or sets the hover-text displayed over the link portion of the control, when in the yellow state.
		/// </summary>
		[Description("Hover-text displayed over the link portion of the control, when in the yellow state.")]
		public string LinkHoverTextYellow
		{
			get { return GetLinkHoverText(IndicatorLightStatus.Yellow); }
			set
			{
				SetLinkHoverText(IndicatorLightStatus.Yellow, value);
				UpdateToolTips();
			}
		}

		/// <summary>
		/// Gets or sets the hover-text displayed over the link portion of the control, when in the red state.
		/// </summary>
		[Description("Hover-text displayed over the link portion of the control, when in the red state.")]
		public string LinkHoverTextRed
		{
			get { return GetLinkHoverText(IndicatorLightStatus.Red); }
			set
			{
				SetLinkHoverText(IndicatorLightStatus.Red, value);
				UpdateToolTips();
			}
		}

		/// <summary>
		/// Gets or sets the status of the light (Green, Red or Yellow).
		/// </summary>
		[DefaultValue(IndicatorLightStatus.Green)]
		[Description("Status of the light (Green, Red or Yellow).")]
		public IndicatorLightStatus Status
		{
			get { return _status; }
			set
			{
				_status = value;
				_iconLabel.ImageIndex = (int)_status;
				UpdateToolTips();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the light appears enabled or greyed out.
		/// </summary>
		[DefaultValue(true)]
		[Description("Determines whether the light appears enabled or greyed out.")]
		public bool LightEnabled
		{
			get { return _iconLabel.Enabled; }
			set { _iconLabel.Enabled = value;}
		}

		/// <summary>
		/// Occurs when the link portion of the control is clicked.
		/// </summary>
		public event LinkLabelLinkClickedEventHandler LinkClicked
		{
			add { _link.LinkClicked += value; }
			remove { _link.LinkClicked -= value; }
		}

		#region Helpers

		private void UpdateToolTips()
		{
			_toolTip.SetToolTip(_iconLabel, GetLightHoverText(_status));
			_toolTip.SetToolTip(_link, GetLinkHoverText(_status));
		}

		private string GetLightHoverText(IndicatorLightStatus status)
		{
			return _lightHoverText[(int) status];
		}

		private void SetLightHoverText(IndicatorLightStatus status, string text)
		{
			_lightHoverText[(int) status] = text;
		}

		private string GetLinkHoverText(IndicatorLightStatus status)
		{
			return _linkHoverText[(int)status];
		}

		private void SetLinkHoverText(IndicatorLightStatus status, string text)
		{
			_linkHoverText[(int)status] = text;
		}

		#endregion
	}

	/// <summary>
	/// Status of an <see cref="IndicatorLight"/> control.
	/// </summary>
	public enum IndicatorLightStatus
	{
		/// <summary>
		/// Green
		/// </summary>
		Green,

		/// <summary>
		/// Yellow
		/// </summary>
		Yellow,

		/// <summary>
		/// Red
		/// </summary>
		Red
	}
}
