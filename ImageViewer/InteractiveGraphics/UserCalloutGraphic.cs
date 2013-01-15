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

using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// A user-interactive version of <see cref="CalloutGraphic"/>.
	/// </summary>
	[Cloneable]
	public class UserCalloutGraphic : CalloutGraphic
	{
		/// <summary>
		/// Instantiates a new instance of <see cref="UserCalloutGraphic"/>.
		/// </summary>
		public UserCalloutGraphic() : base("") {}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected UserCalloutGraphic(UserCalloutGraphic source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		/// <summary>
		/// Gets or sets the callout text label.
		/// </summary>
		public new string Text
		{
			get { return base.Text; }
			set { base.Text = value; }
		}

		/// <summary>
		/// Gets the <see cref="IControlGraphic"/> controlling the <see cref="CalloutGraphic.TextGraphic"/>.
		/// </summary>
		protected new TextEditControlGraphic TextControlGraphic
		{
			get { return (TextEditControlGraphic) base.TextControlGraphic; }
		}

		/// <summary>
		/// Initializes the control chain for the text graphic portion of the callout.
		/// </summary>
		/// <remarks>
		/// This implementation creates a <see cref="TextEditControlGraphic"/> and a <see cref="MoveControlGraphic"/> to allow the
		/// user to interactively edit and move the callout text.
		/// </remarks>
		/// <param name="textGraphic">The text graphic to be controlled.</param>
		/// <returns>A control graphic chain controlling the text graphic.</returns>
		protected override IControlGraphic InitializeTextControlGraphic(ITextGraphic textGraphic)
		{
			return new TextEditControlGraphic(new TextPlaceholderControlGraphic(base.InitializeTextControlGraphic(textGraphic)));
		}

		/// <summary>
		/// Initializes the control chain for the anchor point of the callout.
		/// </summary>
		/// <remarks>
		/// This implementation creates an <see cref="AnchorPointControlGraphic"/> to allow the user to
		/// interactively move the <see cref="CalloutGraphic.AnchorPoint"/> of the callout.
		/// </remarks>
		/// <param name="pointGraphic">The anchor point to be controlled.</param>
		/// <returns>A control graphic chain controlling the anchor point.</returns>
		protected override IControlGraphic InitializePointControlGraphic(IPointGraphic pointGraphic)
		{
			return new AnchorPointControlGraphic(base.InitializePointControlGraphic(pointGraphic));
		}

		/// <summary>
		/// Starts the interactive edit mode of the callout.
		/// </summary>
		/// <returns>True if the callout successfully entered interactive edit mode.</returns>
		public bool StartEdit()
		{
			return this.TextControlGraphic.StartEdit();
		}

		/// <summary>
		/// Ends the interactive edit mode of the callout.
		/// </summary>
		public void EndEdit()
		{
			this.TextControlGraphic.EndEdit();
		}
	}
}