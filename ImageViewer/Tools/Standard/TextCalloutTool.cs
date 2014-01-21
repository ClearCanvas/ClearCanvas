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
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Automation;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("activateTextCallout", "imageviewer-contextmenu/MenuTextCallout", "SelectTextCallout", InitiallyAvailable = false)]
	[MenuAction("activateTextCallout", "global-menus/MenuTools/MenuStandard/MenuTextCallout", "SelectTextCallout")]
	[CheckedStateObserver("activateTextCallout", "IsTextCalloutModeActive", "ModeOrActiveChanged")]
	[Tooltip("activateTextCallout", "TooltipTextCallout")]
	[MouseButtonIconSet("activateTextCallout", SmallTextCalloutIcon, MediumTextCalloutIcon, LargeTextCalloutIcon)]
	[GroupHint("activateTextCallout", "Tools.Image.Annotations.Text.Callout")]
	//
	[ButtonAction("selectTextCallout", "textcallouttool-dropdown/ToolbarTextCallout", "SelectTextCallout", Flags = ClickActionFlags.CheckAction)]
	[CheckedStateObserver("selectTextCallout", "IsTextCalloutModeSelected", "ModeChanged")]
	[Tooltip("selectTextCallout", "TooltipTextCallout")]
	[MouseButtonIconSet("selectTextCallout", SmallTextCalloutIcon, MediumTextCalloutIcon, LargeTextCalloutIcon)]
	[GroupHint("selectTextCallout", "Tools.Image.Annotations.Text.Callout")]
	//
	[MenuAction("activateTextArea", "imageviewer-contextmenu/MenuTextArea", "SelectTextArea", InitiallyAvailable = false)]
	[MenuAction("activateTextArea", "global-menus/MenuTools/MenuStandard/MenuTextArea", "SelectTextArea")]
	[CheckedStateObserver("activateTextArea", "IsTextAreaModeActive", "ModeOrActiveChanged")]
	[Tooltip("activateTextArea", "TooltipTextArea")]
	[MouseButtonIconSet("activateTextArea", SmallTextAreaIcon, MediumTextAreaIcon, LargeTextAreaIcon)]
	[GroupHint("activateTextArea", "Tools.Image.Annotations.Text.Area")]
	//
	[ButtonAction("selectTextArea", "textcallouttool-dropdown/ToolbarTextArea", "SelectTextArea", Flags = ClickActionFlags.CheckAction)]
	[CheckedStateObserver("selectTextArea", "IsTextAreaModeSelected", "ModeChanged")]
	[Tooltip("selectTextArea", "TooltipTextArea")]
	[MouseButtonIconSet("selectTextArea", SmallTextAreaIcon, MediumTextAreaIcon, LargeTextAreaIcon)]
	[GroupHint("selectTextArea", "Tools.Image.Annotations.Text.Area")]
	//
	[DropDownButtonAction("activate", "global-toolbars/ToolbarAnnotation/ToolbarTextCallout", "Select", "DropDownMenuModel", Flags = ClickActionFlags.CheckAction)]
	[CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[IconSetObserver("activate", "IconSet", "ModeChanged")]
	[GroupHint("activate", "Tools.Image.Annotations.Text.Callout")]
	//
	[MouseToolButton(XMouseButtons.Left, false)]
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class TextCalloutTool : MouseImageViewerTool, IDrawTextCallout, IDrawTextArea
	{
		#region Icon Resource Constants

		public const string SmallTextCalloutIcon = "Icons.TextCalloutToolSmall.png";
		public const string MediumTextCalloutIcon = "Icons.TextCalloutToolMedium.png";
		public const string LargeTextCalloutIcon = "Icons.TextCalloutToolLarge.png";
		public const string SmallTextAreaIcon = "Icons.TextAreaToolSmall.png";
		public const string MediumTextAreaIcon = "Icons.TextAreaToolMedium.png";
		public const string LargeTextAreaIcon = "Icons.TextAreaToolLarge.png";

		private readonly IconSet _textCalloutIconSet = new MouseButtonIconSet(SmallTextCalloutIcon, MediumTextCalloutIcon, LargeTextCalloutIcon, XMouseButtons.Left);
		private readonly IconSet _textAreaIconSet = new MouseButtonIconSet(SmallTextAreaIcon, MediumTextAreaIcon, LargeTextAreaIcon, XMouseButtons.Left);

		#endregion

		private DrawableUndoableCommand _undoableCommand;
		private InteractiveTextGraphicBuilder _graphicBuilder;

		public TextCalloutTool() : base(SR.TooltipTextCallout)
		{
			Behaviour |= MouseButtonHandlerBehaviour.SuppressContextMenu | MouseButtonHandlerBehaviour.SuppressOnTileActivate;
			_iconSet = _textCalloutIconSet;
		}

		#region Tool Mode Support

		private delegate InteractiveTextGraphicBuilder CreateInteractiveGraphicBuilderDelegate(IControlGraphic graphic);

		private delegate IControlGraphic CreateGraphicDelegate();

		/// <summary>
		/// Fired when the tool's <see cref="Mode"/> changes.
		/// </summary>
		public event EventHandler ModeChanged;

		/// <summary>
		/// Fired when the tool's <see cref="Mode"/> or <see cref="MouseImageViewerTool.Active"/> properties change.
		/// </summary>
		public event EventHandler ModeOrActiveChanged;

		private ActionModelNode _actionModel;
		private ToolSettings _settings;

		// fields specific to a particular mode
		private TextCalloutMode _mode = TextCalloutMode.TextCallout;
		private IconSet _iconSet = null;
		private CreateInteractiveGraphicBuilderDelegate _interactiveGraphicBuilderDelegate;
		private CreateGraphicDelegate _graphicDelegateCreatorDelegate;
		private string _commandCreationName = "";

		/// <summary>
		/// Performs initialization related to the tool's mode support.
		/// </summary>
		protected virtual void InitializeMode()
		{
			_settings = ToolSettings.DefaultInstance;

			try
			{
				_mode = (TextCalloutMode) Enum.Parse(typeof (TextCalloutMode), _settings.TextCalloutMode, true);
			}
			catch (Exception)
			{
				_mode = TextCalloutMode.TextCallout;
			}

			this.OnModeChanged();
			this.OnModeOrActiveChanged();
		}

		/// <summary>
		/// Gets the drop down action model for the tool.
		/// </summary>
		public ActionModelNode DropDownMenuModel
		{
			get
			{
				if (_actionModel == null)
					_actionModel = ActionModelRoot.CreateModel("ClearCanvas.ImageViewer.Tools.Standard", "textcallouttool-dropdown", this.Actions);
				return _actionModel;
			}
		}

		/// <summary>
		/// Gets or sets the tool's currently selected mode.
		/// </summary>
		public TextCalloutMode Mode
		{
			get { return _mode; }
			set
			{
				if (_mode != value)
				{
					_mode = value;
					this.OnModeChanged();
					this.OnModeOrActiveChanged();
				}
			}
		}

		/// <summary>
		/// Gets the icon set for the tool in the currently selected mode.
		/// </summary>
		public IconSet IconSet
		{
			get { return _iconSet; }
		}

		/// <summary>
		/// Gets the creation command name for the tool in the currently selected mode.
		/// </summary>
		public string CreationCommandName
		{
			get { return _commandCreationName; }
		}

		/// <summary>
		/// Gets a value indicating if the tool is <see cref="MouseImageViewerTool.Active"/> and is in the <see cref="TextCalloutMode.TextCallout"/> mode.
		/// </summary>
		public bool IsTextCalloutModeActive
		{
			get { return this.IsTextCalloutModeSelected && this.Active; }
		}

		/// <summary>
		/// Gets a value indicating if the tool is <see cref="MouseImageViewerTool.Active"/> and is in the <see cref="TextCalloutMode.TextArea"/> mode.
		/// </summary>
		public bool IsTextAreaModeActive
		{
			get { return this.IsTextAreaModeSelected && this.Active; }
		}

		/// <summary>
		/// Gets a value indicating if the tool is in the <see cref="TextCalloutMode.TextCallout"/> mode.
		/// </summary>
		public bool IsTextCalloutModeSelected
		{
			get { return _mode == TextCalloutMode.TextCallout; }
		}

		/// <summary>
		/// Gets a value indicating if the tool is in the <see cref="TextCalloutMode.TextArea"/> mode.
		/// </summary>
		public bool IsTextAreaModeSelected
		{
			get { return _mode == TextCalloutMode.TextArea; }
		}

		/// <summary>
		/// Switches the tool's mode to <see cref="TextCalloutMode.TextCallout"/> and invokes <see cref="MouseImageViewerTool.Select"/>.
		/// </summary>
		public void SelectTextCallout()
		{
			this.Mode = TextCalloutMode.TextCallout;
			this.Select();
		}

		/// <summary>
		/// Switches the tool's mode to <see cref="TextCalloutMode.TextArea"/> and invokes <see cref="MouseImageViewerTool.Select"/>.
		/// </summary>
		public void SelectTextArea()
		{
			this.Mode = TextCalloutMode.TextArea;
			this.Select();
		}

		/// <summary>
		/// Called when the tool's <see cref="Mode"/> changes.
		/// </summary>
		protected virtual void OnModeChanged()
		{
			switch (_mode)
			{
				case TextCalloutMode.TextCallout:
					this.TooltipPrefix = SR.TooltipTextCallout;
					this._commandCreationName = SR.CommandCreateTextCallout;
					this._iconSet = _textCalloutIconSet;
					this._interactiveGraphicBuilderDelegate = CreateInteractiveTextCalloutBuilder;
					this._graphicDelegateCreatorDelegate = CreateTextCalloutGraphic;
					break;
				case TextCalloutMode.TextArea:
					this.TooltipPrefix = SR.TooltipTextArea;
					this._commandCreationName = SR.CommandCreateTextArea;
					this._iconSet = _textAreaIconSet;
					this._interactiveGraphicBuilderDelegate = CreateInteractiveTextAreaBuilder;
					this._graphicDelegateCreatorDelegate = CreateTextAreaGraphic;
					break;
			}
			_settings.TextCalloutMode = _mode.ToString();
			EventsHelper.Fire(ModeChanged, this, new EventArgs());
		}

		/// <summary>
		/// Called when the tool's <see cref="Mode"/> or <see cref="MouseImageViewerTool.Active"/> properties change.
		/// </summary>
		protected virtual void OnModeOrActiveChanged()
		{
			EventsHelper.Fire(ModeOrActiveChanged, this, new EventArgs());
		}

		protected virtual bool CanStart(IPresentationImage image)
		{
			return image != null && image is IOverlayGraphicsProvider;
		}

		#endregion

		public override void Initialize()
		{
			base.Initialize();
			InitializeMode();
		}

		protected override void OnActivationChanged()
		{
			base.OnActivationChanged();
			this.OnModeOrActiveChanged();
		}

		public override CursorToken GetCursorToken(Point point)
		{
			if (_graphicBuilder != null)
				return _graphicBuilder.GetCursorToken(point);
			return base.GetCursorToken(point);
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			base.Start(mouseInformation);

			if (_graphicBuilder != null)
				return _graphicBuilder.Start(mouseInformation);

			IPresentationImage image = mouseInformation.Tile.PresentationImage;
			if (!CanStart(image))
				return false;

			var provider = (IOverlayGraphicsProvider) image;
			IControlGraphic graphic = _graphicDelegateCreatorDelegate();

			_graphicBuilder = _interactiveGraphicBuilderDelegate(graphic);
			_graphicBuilder.GraphicComplete += OnGraphicBuilderInitiallyDone;
			_graphicBuilder.GraphicCancelled += OnGraphicBuilderInitiallyDone;
			_graphicBuilder.GraphicFinalComplete += OnGraphicFinalComplete;
			_graphicBuilder.GraphicFinalCancelled += OnGraphicFinalCancelled;

			AddGraphic(image, graphic, provider);

			if (_graphicBuilder.Start(mouseInformation))
				return true;

			this.Cancel();
			return false;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			if (_graphicBuilder != null)
				return _graphicBuilder.Track(mouseInformation);

			return false;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			if (_graphicBuilder == null)
				return false;

			if (_graphicBuilder.Stop(mouseInformation))
				return true;

			_graphicBuilder.Graphic.ImageViewer.CommandHistory.AddCommand(_undoableCommand);
			_graphicBuilder = null;
			_undoableCommand = null;
			return false;
		}

		public override void Cancel()
		{
			if (_graphicBuilder == null)
				return;

			_graphicBuilder.Cancel();
		}

		protected void AddGraphic(IPresentationImage image, IControlGraphic graphic, IOverlayGraphicsProvider provider)
		{
			_undoableCommand = new DrawableUndoableCommand(image);
			_undoableCommand.Enqueue(new AddGraphicUndoableCommand(graphic, provider.OverlayGraphics));
			_undoableCommand.Name = CreationCommandName;
			_undoableCommand.Execute();
		}

		/// <summary>
		/// Fired when the graphic builder is done placing the graphic, and hence does not need mouse capture anymore, but in the
		/// text graphic is not technically complete yet and thus we do not insert into the command history yet.
		/// </summary>
		private void OnGraphicBuilderInitiallyDone(object sender, GraphicEventArgs e)
		{
			_graphicBuilder.GraphicComplete -= OnGraphicBuilderInitiallyDone;
			_graphicBuilder.GraphicCancelled -= OnGraphicBuilderInitiallyDone;
			_graphicBuilder = null;
		}

		/// <summary>
		/// Fired when the graphic builder is also done setting text in the graphic, and thus we can decide if we want to unexecute
		/// the insert or save the command into the history.
		/// </summary>
		private void OnGraphicFinalComplete(object sender, GraphicEventArgs e)
		{
			// fired 
			InteractiveTextGraphicBuilder graphicBuilder = sender as InteractiveTextGraphicBuilder;
			if (graphicBuilder != null)
			{
				graphicBuilder.GraphicFinalComplete -= OnGraphicFinalComplete;
				graphicBuilder.GraphicFinalCancelled -= OnGraphicFinalCancelled;
			}

			ImageViewer.CommandHistory.AddCommand(_undoableCommand);
			_undoableCommand = null;
		}

		/// <summary>
		/// Fired when the graphic builder is also done setting text in the graphic, and thus we can decide if we want to unexecute
		/// the insert or save the command into the history.
		/// </summary>
		private void OnGraphicFinalCancelled(object sender, GraphicEventArgs e)
		{
			InteractiveTextGraphicBuilder graphicBuilder = sender as InteractiveTextGraphicBuilder;
			if (graphicBuilder != null)
			{
				graphicBuilder.GraphicFinalComplete -= OnGraphicFinalComplete;
				graphicBuilder.GraphicFinalCancelled -= OnGraphicFinalCancelled;
			}

			_undoableCommand.Unexecute();
			_undoableCommand = null;
		}

		private static IControlGraphic CreateTextCalloutGraphic()
		{
			UserCalloutGraphic callout = new UserCalloutGraphic();
			callout.LineStyle = LineStyle.Solid;
			callout.ShowArrowhead = true;

			StandardStatefulGraphic statefulGraphic = new StandardStatefulGraphic(callout);
			statefulGraphic.State = statefulGraphic.CreateInactiveState();

			/// TODO (CR Oct 2011): This is the wrong order - the "selected" graphic (stateful) gets
			/// deleted by the "delete graphics" tool, and then the top-level context menu
			/// graphic gets left dangling.
			ContextMenuControlGraphic contextGraphic = new ContextMenuControlGraphic(typeof (TextCalloutTool).FullName, "basicgraphic-menu", null, statefulGraphic);
			contextGraphic.Actions = new ToolSet(new GraphicToolExtensionPoint(), new GraphicToolContext(contextGraphic)).Actions;

			return contextGraphic;
		}

		private static InteractiveTextGraphicBuilder CreateInteractiveTextCalloutBuilder(IControlGraphic graphic)
		{
			return new InteractiveTextCalloutBuilder(graphic.Subject as UserCalloutGraphic);
		}

		private static IControlGraphic CreateTextAreaGraphic()
		{
			InvariantTextPrimitive textArea = new InvariantTextPrimitive();
			TextEditControlGraphic controlGraphic = new TextEditControlGraphic(new MoveControlGraphic(textArea));
			controlGraphic.DeleteOnEmpty = true;

			StandardStatefulGraphic statefulGraphic = new StandardStatefulGraphic(controlGraphic);
			statefulGraphic.State = statefulGraphic.CreateInactiveState();

			ContextMenuControlGraphic contextGraphic = new ContextMenuControlGraphic(typeof (TextCalloutTool).FullName, "basicgraphic-menu", null, statefulGraphic);
			contextGraphic.Actions = new ToolSet(new GraphicToolExtensionPoint(), new GraphicToolContext(contextGraphic)).Actions;

			return contextGraphic;
		}

		private static InteractiveTextGraphicBuilder CreateInteractiveTextAreaBuilder(IControlGraphic graphic)
		{
			return new InteractiveTextAreaBuilder(graphic.Subject as ITextGraphic);
		}

		#region Implementation of IDrawTextCallout, IDrawTextArea

		IControlGraphic IDrawTextCallout.Draw(CoordinateSystem coordinateSystem, string name, PointF anchorPoint, string text, PointF textLocation)
		{
			var image = Context.Viewer.SelectedPresentationImage;
			if (!CanStart(image))
				throw new InvalidOperationException("Can't draw a text callout at this time.");

			var imageGraphic = ((IImageGraphicProvider) image).ImageGraphic;
			if (coordinateSystem == CoordinateSystem.Destination)
			{
				//Use the image graphic to get the "source" coordinates because it's already in the scene.
				anchorPoint = imageGraphic.SpatialTransform.ConvertToSource(anchorPoint);
				textLocation = imageGraphic.SpatialTransform.ConvertToSource(textLocation);
			}

			var overlayProvider = (IOverlayGraphicsProvider) image;
			var graphic = CreateTextCalloutGraphic();
			graphic.Name = name;
			AddGraphic(image, graphic, overlayProvider);

			var subject = (UserCalloutGraphic) graphic.Subject;

			subject.AnchorPoint = anchorPoint;
			subject.Text = text;
			subject.TextLocation = textLocation;

			graphic.Draw();
			return graphic;
		}

		IControlGraphic IDrawTextArea.Draw(CoordinateSystem coordinateSystem, string name, string text, PointF textLocation)
		{
			var image = Context.Viewer.SelectedPresentationImage;
			if (!CanStart(image))
				throw new InvalidOperationException("Can't draw a text callout at this time.");

			var imageGraphic = ((IImageGraphicProvider) image).ImageGraphic;
			if (coordinateSystem == CoordinateSystem.Destination)
			{
				//Use the image graphic to get the "source" coordinates because it's already in the scene.
				textLocation = imageGraphic.SpatialTransform.ConvertToSource(textLocation);
			}

			var overlayProvider = (IOverlayGraphicsProvider) image;
			var graphic = CreateTextAreaGraphic();
			graphic.Name = name;
			AddGraphic(image, graphic, overlayProvider);

			var subject = (ITextGraphic) graphic.Subject;

			subject.Text = text;
			subject.Location = textLocation;

			graphic.Draw();
			return graphic;
		}

		#endregion
	}

	/// <summary>
	/// Specifies the creation mode of the <see cref="TextCalloutTool"/>.
	/// </summary>
	public enum TextCalloutMode
	{
		/// <summary>
		/// Specifies that the tool should create a text annotation graphic with an arrow indicating a point of interest.
		/// </summary>
		TextCallout,

		/// <summary>
		/// Specifies that the tool should create a standalone text annotation graphic.
		/// </summary>
		TextArea
	}
}