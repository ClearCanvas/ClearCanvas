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
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// An interactive graphic that controls the text content of an <see cref="ITextGraphic"/>.
	/// </summary>
	[Cloneable]
	public class TextEditControlGraphic : ControlGraphic, ITextGraphic, IMemorable
	{
		private bool _multiline = true;

		private bool _deleteOnEmpty = false;

		[CloneIgnore]
		private EditBox _currentCalloutEditBox;

		/// <summary>
		/// Constructs a new <see cref="TextEditControlGraphic"/>.
		/// </summary>
		/// <param name="subject">An <see cref="ITextGraphic"/> or an <see cref="IControlGraphic"/> chain whose subject is an <see cref="ITextGraphic"/>.</param>
		public TextEditControlGraphic(IGraphic subject) : base(subject)
		{
			Platform.CheckExpectedType(base.Subject, typeof (ITextGraphic));
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected TextEditControlGraphic(TextEditControlGraphic source, ICloningContext context) : base(source, context)
		{
			context.CloneFields(source, this);
		}

		/// <summary>
		/// Gets the subject that this graphic controls.
		/// </summary>
		public new ITextGraphic Subject
		{
			get { return base.Subject as ITextGraphic; }
		}

		/// <summary>
		/// Gets a string that describes the type of control operation that this graphic provides.
		/// </summary>
		public override string CommandName
		{
			get { return SR.CommandEditText; }
		}

		/// <summary>
		/// Gets or sets a value indicating if multiline input should be accepted in the editor.
		/// </summary>
		public bool Multiline
		{
			get { return _multiline; }
			set { _multiline = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating if the graphic should be automatically deleted if the accepted text input is empty.
		/// </summary>
		public bool DeleteOnEmpty
		{
			get { return _deleteOnEmpty; }
			set { _deleteOnEmpty = value; }
		}

		/// <summary>
		/// Starts edit mode on the callout graphic by installing a <see cref="EditBox"/> on the
		/// <see cref="Tile"/> of the <see cref="Graphic.ParentPresentationImage">parent PresentationImage</see>.
		/// </summary>
		/// <returns>True if edit mode was successfully started; False otherwise.</returns>
		public bool StartEdit()
		{
			// remove any pre-existing edit boxes
			EndEdit();

			bool result = false;
			this.CoordinateSystem = CoordinateSystem.Destination;
			try
			{
				EditBox editBox = new EditBox(this.Text ?? string.Empty);
				editBox.Location = Point.Round(this.Location);
				editBox.Size = Rectangle.Round(this.BoundingBox).Size;
				editBox.Multiline = this.Multiline;
				editBox.FontName = this.FontName;
				editBox.FontSize = this.FontSize;
				editBox.ValueAccepted += OnEditBoxComplete;
				editBox.ValueCancelled += OnEditBoxComplete;
				InstallEditBox(_currentCalloutEditBox = editBox);
				result = true;
			}
			finally
			{
				this.ResetCoordinateSystem();
			}

			return result;
		}

		/// <summary>
		/// Ends edit mode on the callout graphic if it is currently being edited. Has no effect otherwise.
		/// </summary>
		public void EndEdit()
		{
			if (_currentCalloutEditBox != null)
			{
				_currentCalloutEditBox.ValueAccepted -= OnEditBoxComplete;
				_currentCalloutEditBox.ValueCancelled -= OnEditBoxComplete;
				_currentCalloutEditBox = null;
			}
			InstallEditBox(null);
		}

		private void InstallEditBox(EditBox editBox)
		{
			if (base.ParentPresentationImage != null)
			{
				if (base.ParentPresentationImage.Tile != null)
					base.ParentPresentationImage.Tile.EditBox = editBox;
			}
		}

		private void OnEditBoxComplete(object sender, EventArgs e)
		{
			bool removeFromParent = false;
			if (_currentCalloutEditBox != null)
			{
				removeFromParent = _deleteOnEmpty && string.IsNullOrEmpty(_currentCalloutEditBox.LastAcceptedValue);
				if (!removeFromParent)
				{
					object state = this.CreateMemento();
					this.Text = _currentCalloutEditBox.LastAcceptedValue;
					AddToCommandHistory(this, state, this.CreateMemento());
					this.Draw();
				}
			}
			EndEdit();

			if (removeFromParent && base.ImageViewer != null)
			{
				// find the highest-level control graphic
				IGraphic graphic = this;
				while (graphic != null && graphic.ParentGraphic is IControlGraphic)
					graphic = graphic.ParentGraphic;

				if (graphic != null && graphic.ParentPresentationImage != null)
				{
					IImageViewer imageViewer = base.ImageViewer;
					DrawableUndoableCommand command = new DrawableUndoableCommand(graphic.ParentPresentationImage);
					command.Name = SR.CommandDelete;
					command.Enqueue(new RemoveGraphicUndoableCommand(graphic));
					command.Execute();
					imageViewer.CommandHistory.AddCommand(command);
				}
			}
		}

		/// <summary>
		/// Gets a set of exported <see cref="IAction"/>s.
		/// </summary>
		/// <param name="site">The action model site at which the actions should reside.</param>
		/// <param name="mouseInformation">The mouse input when the action model was requested, such as in response to a context menu request.</param>
		/// <returns>A set of exported <see cref="IAction"/>s.</returns>
		public override IActionSet GetExportedActions(string site, IMouseInformation mouseInformation)
		{
            if (!HitTest(mouseInformation.Location))
                return new ActionSet();

			IResourceResolver resolver = new ApplicationThemeResourceResolver(this.GetType(), true);
			string @namespace = typeof(TextEditControlGraphic).FullName;
			MenuAction action = new MenuAction(@namespace + ":edit", new ActionPath(site + "/MenuEditText", resolver), ClickActionFlags.None, resolver);
			action.GroupHint = new GroupHint("Tools.Graphics.Edit");
			action.Label = SR.MenuEditText;
			action.Persistent = true;
			action.SetClickHandler(delegate { this.StartEdit(); });
			return base.GetExportedActions(site, mouseInformation).Union(new ActionSet(new IAction[] {action}));
		}

		/// <summary>
		/// Called by <see cref="ControlGraphic"/> in response to a mouse button click via <see cref="ControlGraphic.Start"/>.
		/// </summary>
		/// <param name="mouseInformation">The mouse input information.</param>
		/// <returns>True if the <see cref="ControlGraphic"/> did something as a result of the call and hence would like to receive capture; False otherwise.</returns>
		protected override bool Start(IMouseInformation mouseInformation)
		{
			if (mouseInformation.ClickCount == 2 && mouseInformation.ActiveButton == XMouseButtons.Left)
			{
				this.CoordinateSystem = CoordinateSystem.Destination;
				try
				{
					if (this.HitTest(mouseInformation.Location))
					{
						this.StartEdit();
						return true;
					}
					else
					{
						this.EndEdit();
					}
				}
				finally
				{
					this.ResetCoordinateSystem();
				}
			}
			return base.Start(mouseInformation);
		}

		#region ITextGraphic Members

		/// <summary>
		/// Gets or sets the size in points.
		/// </summary>
		/// <remarks>
		/// Default value is 10 points.
		/// </remarks>
		public float FontSize
		{
			get { return this.Subject.SizeInPoints; }
			set { this.Subject.SizeInPoints = value; }
		}

		/// <summary>
		/// Gets or sets the font.
		/// </summary>
		/// <remarks>
		/// Default value is "Arial".
		/// </remarks>
		public string FontName
		{
			get { return this.Subject.Font; }
			set { this.Subject.Font = value; }
		}

		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		public string Text
		{
			get { return this.Subject.Text; }
			set { this.Subject.Text = value; }
		}

		/// <summary>
		/// Gets or sets the size in points.
		/// </summary>
		/// <remarks>
		/// Default value is 10 points.
		/// </remarks>
		float ITextGraphic.SizeInPoints
		{
			get { return this.FontSize; }
			set { this.FontSize = value; }
		}

		/// <summary>
		/// Gets or sets the font.
		/// </summary>
		/// <remarks>
		/// Default value is "Arial".
		/// </remarks>
		string ITextGraphic.Font
		{
			get { return this.FontName; }
			set { this.FontName = value; }
		}

		/// <summary>
		/// Gets or sets the location of the center of the text.
		/// </summary>
		/// <remarks>
		/// This property is in either source or destination coordinates depending on the value of <see cref="IGraphic.CoordinateSystem"/>.
		/// </remarks>
		public PointF Location
		{
			get { return this.Subject.Location; }
			set { this.Subject.Location = value; }
		}

		/// <summary>
		/// Gets the dimensions of the smallest rectangle that bounds the text.
		/// </summary>
		/// <remarks>
		/// This property is in either source or destination coordinates depending on the value of <see cref="IGraphic.CoordinateSystem"/>.
		/// </remarks>
		public SizeF Dimensions
		{
			get { return this.Subject.Dimensions; }
		}

		/// <summary>
		/// Gets or sets the line style.
		/// </summary>
		public LineStyle LineStyle
		{
			get { return this.Subject.LineStyle; }
			set { this.Subject.LineStyle = value; }
		}

		#endregion

		#region IMemorable Members

		/// <summary>
		/// Captures the state of an object.
		/// </summary>
		/// <remarks>
		/// The implementation of <see cref="IMemorable.CreateMemento"/> should return an
		/// object containing enough state information so that
		/// when <see cref="IMemorable.SetMemento"/> is called, the object can be restored
		/// to the original state.
		/// </remarks>
		public object CreateMemento()
		{
			return this.Subject.Text;
		}

		/// <summary>
		/// Restores the state of an object.
		/// </summary>
		/// <param name="memento">The object that was
		/// originally created with <see cref="IMemorable.CreateMemento"/>.</param>
		/// <remarks>
		/// The implementation of <see cref="IMemorable.SetMemento"/> should return the 
		/// object to the original state captured by <see cref="IMemorable.CreateMemento"/>.
		/// </remarks>
		public void SetMemento(object memento)
		{
			if (!(memento is string))
				throw new ArgumentException("The provided memento is not the expected type.", "memento");

			this.Subject.Text = (string) memento;
		}

		#endregion
	}
}