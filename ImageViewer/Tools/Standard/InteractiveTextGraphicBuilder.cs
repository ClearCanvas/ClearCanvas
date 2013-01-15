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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	/// <summary>
	/// Adds two events to mark completion or cancellation of the text graphic after the graphic builder loses
	/// mouse input capture and enters text edit mode (still part of text graphic building process).
	/// </summary>
	internal abstract class InteractiveTextGraphicBuilder : InteractiveGraphicBuilder
	{
		private event EventHandler<GraphicEventArgs> _graphicFinalComplete;
		private event EventHandler<GraphicEventArgs> _graphicFinalCancelled;

		private EditBox _currentCalloutEditBox;
		private ITextGraphic _textGraphic;

		protected InteractiveTextGraphicBuilder(IGraphic graphic) : base(graphic) {}

		public event EventHandler<GraphicEventArgs> GraphicFinalComplete
		{
			add { _graphicFinalComplete += value; }
			remove { _graphicFinalComplete -= value; }
		}

		public event EventHandler<GraphicEventArgs> GraphicFinalCancelled
		{
			add { _graphicFinalCancelled += value; }
			remove { _graphicFinalCancelled -= value; }
		}

		public override sealed void Reset()
		{
			throw new NotSupportedException();
		}

		protected override void Rollback()
		{
			throw new NotSupportedException();
		}

		protected abstract ITextGraphic FindTextGraphic();

		protected override void OnGraphicComplete()
		{
			_textGraphic = this.FindTextGraphic();

			base.OnGraphicComplete();
			this.StartEdit();
		}

		protected override void OnGraphicCancelled()
		{
			EventsHelper.Fire(_graphicFinalCancelled, this, new GraphicEventArgs(this.Graphic));
			base.OnGraphicCancelled();
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
			this.Graphic.CoordinateSystem = CoordinateSystem.Destination;
			try
			{
				EditBox editBox = new EditBox(_textGraphic.Text ?? string.Empty);
				if (string.IsNullOrEmpty(_textGraphic.Text))
					editBox.Value = SR.LabelEnterText;
				editBox.Multiline = true;
				editBox.Location = Point.Round(_textGraphic.Location);
				editBox.Size = Rectangle.Round(_textGraphic.BoundingBox).Size;
				editBox.FontName = _textGraphic.Font;
				editBox.FontSize = _textGraphic.SizeInPoints;
				editBox.ValueAccepted += OnEditBoxComplete;
				editBox.ValueCancelled += OnEditBoxComplete;
				InstallEditBox(_currentCalloutEditBox = editBox);
				result = true;
			}
			finally
			{
				this.Graphic.ResetCoordinateSystem();
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
			if (this.Graphic.ParentPresentationImage != null)
			{
				if (this.Graphic.ParentPresentationImage.Tile != null)
					this.Graphic.ParentPresentationImage.Tile.EditBox = editBox;
			}
		}

		private void OnEditBoxComplete(object sender, EventArgs e)
		{
			bool cancelled = false;
			if (_currentCalloutEditBox != null)
			{
				cancelled = string.IsNullOrEmpty(_currentCalloutEditBox.LastAcceptedValue);
				if (!cancelled)
				{
					_textGraphic.Text = _currentCalloutEditBox.Value;
					_textGraphic.Draw();
				}
			}
			EndEdit();

			if (cancelled)
				EventsHelper.Fire(_graphicFinalCancelled, this, new GraphicEventArgs(this.Graphic));
			else
				EventsHelper.Fire(_graphicFinalComplete, this, new GraphicEventArgs(this.Graphic));
		}
	}
}