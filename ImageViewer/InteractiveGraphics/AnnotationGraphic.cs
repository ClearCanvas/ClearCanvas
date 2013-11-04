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
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// An standard, stateful interactive graphic that consists of some subject of interest graphic
	/// and a <see cref="ICalloutGraphic">text callout</see> that describes the subject.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="AnnotationGraphic"/> essentially acts as a template for any kind
	/// of interactive graphic defining some object of interest. The subject of interest
	/// can be any graphic primitive such as a <see cref="LinePrimitive">line</see>, a
	/// <see cref="RectanglePrimitive">rectangle</see>, an <see cref="EllipsePrimitive">ellipse</see>,
	/// etc., or some hierarchy of <see cref="ControlGraphic"/>s decorating a primitive graphic.
	/// </para>
	/// <para>
	/// By default, the callout line will snap to the nearest point on the <see cref="ControlGraphic.Subject"/>.
	/// </para>
	/// </remarks>
	[DicomSerializableGraphicAnnotation(typeof (StandardAnnotationGraphicSerializer))]
	[Cloneable]
	public class AnnotationGraphic : StandardStatefulGraphic, IAnnotationGraphic, IContextMenuProvider
	{
		#region Private fields

		[CloneIgnore]
		private bool _notifyOnSubjectChanged = true;

		[CloneIgnore]
		private ICalloutGraphic _calloutGraphic;

		[CloneIgnore]
		private IToolSet _toolSet;

		[CloneIgnore]
		private bool _settingCalloutLocation = false;

		private IAnnotationCalloutLocationStrategy _calloutLocationStrategy;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of <see cref="AnnotationGraphic"/>.
		/// </summary>
		public AnnotationGraphic(IGraphic subjectGraphic)
			: this(subjectGraphic, null) {}

		/// <summary>
		/// Initializes a new instance of <see cref="AnnotationGraphic"/> with the given <see cref="IAnnotationCalloutLocationStrategy"/>.
		/// </summary>
		public AnnotationGraphic(IGraphic subjectGraphic, IAnnotationCalloutLocationStrategy calloutLocationStrategy)
			: base(subjectGraphic)
		{
			Initialize(calloutLocationStrategy);
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected AnnotationGraphic(AnnotationGraphic source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		#endregion

		/// <summary>
		/// Gets the <see cref="ICalloutGraphic"/> associated with the subject of interest..
		/// </summary>
		public ICalloutGraphic Callout
		{
			get { return _calloutGraphic; }
		}

		#region Virtual Members

		/// <summary>
		/// Gets the namespace with which to qualify the action model site of any context menus on this graphic.
		/// </summary>
		/// <remarks>
		/// <para>The default implementation uses the fully qualified name of the <see cref="AnnotationGraphic"/> type as a namespace.</para>
		/// <para>An implementation of <see cref="AnnotationGraphic"/> can override this property to specify that an alternate action model be used instead.</para>
		/// </remarks>
		protected virtual string ContextMenuNamespace
		{
			get { return typeof (AnnotationGraphic).FullName; }
		}

		/// <summary>
		/// Refreshes the annotation graphic by recomputing the callout position and redrawing the graphic.
		/// </summary>
		public virtual void Refresh()
		{
			this.SetCalloutLocation();
			this.Draw();
		}

		/// <summary>
		/// Called by <see cref="AnnotationGraphic"/> to create the <see cref="ICalloutGraphic"/> to be used by this annotation.
		/// </summary>
		/// <remarks>
		/// <para>The default implementation creates a plain <see cref="CalloutGraphic"/> with no text and which is not user-modifiable.</para>
		/// <para>Subclasses can override this method to provide callouts with automatically computed text content or which is user-interactive.</para>
		/// </remarks>
		/// <returns>The <see cref="ICalloutGraphic"/> to be used.</returns>
		protected virtual ICalloutGraphic CreateCalloutGraphic()
		{
			return new CalloutGraphic(string.Empty);
		}

		/// <summary>
		/// Forces a recomputation of the callout line.
		/// </summary>
		protected void RecomputeCalloutLine()
		{
			this.SetCalloutEndPoint();
		}

		#endregion

		private void Initialize(IAnnotationCalloutLocationStrategy calloutLocationStrategy)
		{
			if (_calloutGraphic == null)
			{
				_calloutGraphic = this.CreateCalloutGraphic();
				base.Graphics.Add(_calloutGraphic);
			}

			_calloutGraphic.TextLocationChanged += OnCalloutLocationChanged;

			if (_calloutLocationStrategy == null)
				_calloutLocationStrategy = calloutLocationStrategy ?? new AnnotationCalloutLocationStrategy();

			_calloutLocationStrategy.SetAnnotationGraphic(this);

			this.Subject.VisualStateChanged += OnSubjectVisualStateChanged;
		}

		/// <summary>
		/// Releases all resources used by this <see cref="AnnotationGraphic"/>.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_calloutLocationStrategy != null)
				{
					_calloutLocationStrategy.Dispose();
					_calloutLocationStrategy = null;
				}

				Subject.VisualStateChanged -= OnSubjectVisualStateChanged;
			}
			base.Dispose(disposing);
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_calloutGraphic = CollectionUtils.SelectFirst(base.Graphics,
			                                              delegate(IGraphic test) { return test is ICalloutGraphic; }) as ICalloutGraphic;
			Platform.CheckForNullReference(_calloutGraphic, "_calloutGraphic");

			this.Initialize(null);

			//the roi and callout may have been selected, so we force a state change
			this.State = this.CreateInactiveState();
		}

		#region Annotation Subject Change Notification

		private void OnSubjectVisualStateChanged(object sender, VisualStateChangedEventArgs e)
		{
			if (_notifyOnSubjectChanged)
				OnSubjectChanged(e.PropertyKind);
		}

		/// <summary>
		/// Temporarily suspends recomputation of the annotation in response to
		/// property change events on the <see cref="IControlGraphic.Subject"/>.
		/// </summary>
		public void Suspend()
		{
			_notifyOnSubjectChanged = false;
		}

		/// <summary>
		/// Resumes recomputation of the annotation in response to
		/// property change events on the <see cref="IControlGraphic.Subject"/>.
		/// </summary>
		/// <param name="notifyNow">True if the recomputation is to be carried out immediately.</param>
		public void Resume(bool notifyNow)
		{
			_notifyOnSubjectChanged = true;

			if (notifyNow)
				OnSubjectVisualStateChanged(this, new VisualStateChangedEventArgs(this, string.Empty));
		}

		/// <summary>
		/// Called when properties on the <see cref="ControlGraphic.Subject"/> have changed.
		/// </summary>
		protected virtual void OnSubjectChanged(VisualStatePropertyKind propertyKind)
		{
			if (propertyKind == VisualStatePropertyKind.Geometry || propertyKind == VisualStatePropertyKind.Unspecified)
				SetCalloutLocation();
		}

		#endregion

		private void SetCalloutEndPoint()
		{
			// We're attaching the callout to the ROI, so make sure the two
			// graphics are in the same coordinate system before we do that.
			// This sets all the graphics coordinate systems to be the same.
			this.CoordinateSystem = Subject.CoordinateSystem;

			PointF endPoint;
			CoordinateSystem coordinateSystem;
			_calloutLocationStrategy.CalculateCalloutEndPoint(out endPoint, out coordinateSystem);

			this.ResetCoordinateSystem();

			_calloutGraphic.CoordinateSystem = coordinateSystem;
			_calloutGraphic.AnchorPoint = endPoint;
			_calloutGraphic.ResetCoordinateSystem();
		}

		private void SetCalloutLocation()
		{
			this.CoordinateSystem = Subject.CoordinateSystem;

			PointF location;
			CoordinateSystem coordinateSystem;
			if (_calloutLocationStrategy.CalculateCalloutLocation(out location, out coordinateSystem))
			{
				_settingCalloutLocation = true;

				_calloutGraphic.CoordinateSystem = coordinateSystem;
				_calloutGraphic.TextLocation = location;
				_calloutGraphic.ResetCoordinateSystem();

				_settingCalloutLocation = false;
			}

			this.ResetCoordinateSystem();

			SetCalloutEndPoint();
		}

		private void OnCalloutLocationChanged(object sender, EventArgs e)
		{
			if (!_settingCalloutLocation)
				_calloutLocationStrategy.OnCalloutLocationChangedExternally();
			SetCalloutEndPoint();
		}

		protected override bool Start(IMouseInformation mouseInformation)
		{
			if (mouseInformation.ActiveButton == XMouseButtons.Right)
			{
				CoordinateSystem = CoordinateSystem.Destination;
				try
				{
					if (HitTest(mouseInformation.Location)) return true;
				}
				finally
				{
					ResetCoordinateSystem();
				}
			}
			return base.Start(mouseInformation);
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

			if (_toolSet == null)
				_toolSet = new ToolSet(new GraphicToolExtensionPoint(), new GraphicToolContext(this));

			return base.GetExportedActions(site, mouseInformation).Union(_toolSet.Actions);
		}

		#region IContextMenuProvider Members

		/// <summary>
		/// Gets the context menu <see cref="ActionModelNode"/> based on the current state of the mouse.
		/// </summary>
		public virtual ActionModelNode GetContextMenuModel(IMouseInformation mouseInformation)
		{
			IActionSet actions = this.GetExportedActions("basicgraphic-menu", mouseInformation);
			if (actions == null || actions.Count == 0)
				return null;
			return ActionModelRoot.CreateModel(this.ContextMenuNamespace, "basicgraphic-menu", actions);
		}

		#endregion
	}
}