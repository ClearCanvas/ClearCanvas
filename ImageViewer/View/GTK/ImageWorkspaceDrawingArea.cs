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
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

using ClearCanvas.Common;
//using ClearCanvas.Workstation.Model;
using ClearCanvas.Desktop;

using Gtk;

/**
 * A brief note on event handling in GTK *********
 * ********************************************************
 *
 * Widget events in Gtk can be handled in two ways: either by adding an event handler, or by
 * overriding a protected method (presumably the base class Widget adds an event handler which calls
 * the protected method, so by overriding the protected method, you are taking advantage of an existing
 * event handler).
 *
 * If you choose to add a separate event handler, be aware that it will by default be added to the
 * end of the invocation list, and earlier event handlers have the power to cancel the event.  In other
 * words, your handler may never get called.
 *
 * To cause your event handler to be added to the beginning of the invocation list, use the attribute
 * [GLib.ConnectBefore] on the event handler declaration. This means that your handler gets the event
 * before anyone else has the chance to veto it.
 *
 * In general it is probably easier just to override the protected method to do your processing and then
 * call the base class method.  In some cases the protected method will have a boolean return value.  In
 * this case, you can cancel the event by returning true - returning true basically means "I've taken
 * care of it, no other event handlers should be called".  Returning false will allow other event handlers
 * in the invocation list to be called.  Unless there is a good reason to explicitly return a true or false,
 * the best practice is to let the base class implementation handle the return value.
 *
 */

namespace ClearCanvas.ImageViewer.View.GTK
{
//    public class WorkspaceDrawingArea : Gtk.DrawingArea
    public class ImageWorkspaceDrawingArea : Gtk.DrawingArea
    {

        private ImageWorkspace _workspace;
        private PhysicalWorkspace _physicalWorkspace;
        private IRenderer _renderer;
        private uint _lastButtonPressed;


        //public WorkspaceDrawingArea(Workspace workspace)
        public ImageWorkspaceDrawingArea(Workspace workspace)
		{
            // set background to black
            this.ModifyBg(StateType.Normal, new Gdk.Color(0, 0, 0));

            // tell GTK that we intend to handle painting ourselves
            // hopefully this will suppress the "flashing", but doesn't seem to help much
            this.AppPaintable = true;

			CreateRenderer();

            _workspace = (ImageWorkspace)workspace;
            _physicalWorkspace = _workspace.PhysicalWorkspace;
            _physicalWorkspace.ImageDrawing += new EventHandler<ImageDrawingEventArgs>(OnDrawImage);

            // tell Gtk that we are interested in receiving these events
            this.AddEvents((int)Gdk.EventMask.PointerMotionMask);
			this.AddEvents((int)Gdk.EventMask.PointerMotionHintMask);	// suppress queueing of motion messages
			this.AddEvents((int)Gdk.EventMask.ButtonPressMask);
            this.AddEvents((int)Gdk.EventMask.ButtonReleaseMask);
            this.AddEvents((int)Gdk.EventMask.KeyPressMask);

            _lastButtonPressed = 0; // no mouse button pressed
		}
		
		public override void Dispose()
		{
			_workspace = null;
			_physicalWorkspace = null;
			_renderer = null;
		}
		
        public Workspace Workspace
        {
            get { return _workspace; }
        }
		
		public bool Active
		{
			get { return _workspace.IsActivated; }
			set { _workspace.IsActivated = true; }
		}
	
        private void CreateRenderer()
        {
			RendererExtensionPoint xp = new RendererExtensionPoint();
            _renderer = (IRenderer)xp.CreateExtension();
        }

        private Rectangle GdkToClientRect(Gdk.Rectangle grect, bool zeroOffset)
        {
            return new Rectangle(zeroOffset ? 0 : grect.X, zeroOffset ? 0 : grect.Y, grect.Width, grect.Height);
        }

        private void OnDrawImage(object sender, ImageDrawingEventArgs e)
        {

            try
            {
                _renderer.Draw(null, e);

                // When we call IRenderer.Draw, the normal (non-custom) meaning
                // is that we're drawing to a memory buffer.  IRenderer.Paint
                // "flips" the buffer to the screen.  So, we need to invalidate here
                // so that OnPaint can call IRenderer.Paint

                // If the image box layout has changed, invalidate the
                // entire window; otherwise, just invalidate the tile
                Rectangle area = Rectangle.Empty;
                if (e.ImageBoxLayoutChanged)
                {
                    area = _physicalWorkspace.ClientRectangle;
                }
                else if (e.TileLayoutChanged)
                {

                    area = e.ImageBox.DrawableClientRectangle;
                }
                else
                {
                    area = Rectangle.Inflate(e.ImageBox.DrawableClientRectangle, 1, 1);
                }

                // decide whether to redraw immediately, or simply queue a redraw
				Gdk.Rectangle grect = new Gdk.Rectangle(area.X, area.Y, area.Width, area.Height);
				this.GdkWindow.InvalidateRect(grect, false);
				
                if (e.PaintNow)
                {
					this.GdkWindow.ProcessUpdates(false);
                }
            }
            catch (Exception x)
            {
                Platform.Log(x);
            }

        }

        protected override void OnRealized()
        {
            Gdk.Rectangle size = this.Allocation;
            if (size.Height * size.Width > 1)
            {
                _physicalWorkspace.ClientRectangle = GdkToClientRect(size, true);
                _physicalWorkspace.Draw(false);
            }
            base.OnRealized();
        }

        protected override bool OnDestroyEvent(Gdk.Event evnt)
        {
            if (_renderer != null)
            {
                _renderer.Dispose();
                _renderer = null;
            }

            return base.OnDestroyEvent(evnt);
        }

        protected override void OnSizeAllocated(Gdk.Rectangle size)
        {
            // window has been sized or resized
            if (size.Height * size.Width > 1)
            {
                _physicalWorkspace.ClientRectangle = GdkToClientRect(size, true);
                _physicalWorkspace.Draw(false);
                QueueDraw();
            }
            base.OnSizeAllocated(size);
        }

	
        protected override bool OnExposeEvent(Gdk.EventExpose args)
        {
			
            using (Graphics g = Gtk.DotNet.Graphics.FromDrawable(args.Window))
            {
#if LINUX
                // For performance reasons we should only update the Area specified
                // in the event args
                Gdk.Rectangle grect = args.Area;
                Rectangle rect = GdkToClientRect(grect, false);
#else
                // However, there is a bug on Windows when trying to access args.Area
                // Therefore, on Windows we must use the entire drawable rectangle
                Gdk.Rectangle grect = this.Allocation;
                Rectangle rect = GdkToClientRect(grect, true);
#endif

                _renderer.Paint(g, rect);

                g.Dispose();
            }

            // return true to indicate that the event has been handled
            // don't allow other event handlers to paint over our image
            return true;
        }



        #region Mouse and Key handlers
        protected override bool OnButtonPressEvent(Gdk.EventButton evnt)
        {
            _physicalWorkspace.OnMouseDown(ConvertToXMouseEventArgs(evnt.Button, evnt.Type, evnt.State, evnt.X, evnt.Y));
            _lastButtonPressed = evnt.Button;
            return base.OnButtonPressEvent(evnt);
        }

        protected override bool OnButtonReleaseEvent(Gdk.EventButton evnt)
        {
            _physicalWorkspace.OnMouseUp(ConvertToXMouseEventArgs(evnt.Button, evnt.Type, evnt.State, evnt.X, evnt.Y));
            _lastButtonPressed = 0; // no button pressed
            return base.OnButtonReleaseEvent(evnt);
        }

        protected override bool OnMotionNotifyEvent(Gdk.EventMotion evnt)
        {
			// copied this code from the net
			// don't really understand it, but for some reason if the event is a hint then
			// the pointer coords/state must be read using the GetPointer method rather than from the
			// event itself
			double x, y;
			Gdk.ModifierType state = Gdk.ModifierType.None;
			if(evnt.IsHint)
			{
				int ix=0, iy=0;
				evnt.Window.GetPointer(out ix, out iy, out state);
				x = ix;
				y = iy;
			}
			else
			{
				x = evnt.X;
				y = evnt.Y;
				state = evnt.State;
			}
			
            _physicalWorkspace.OnMouseMove(ConvertToXMouseEventArgs(_lastButtonPressed, evnt.Type, state, x, y));
            return base.OnMotionNotifyEvent(evnt);
        }

        protected override bool OnKeyPressEvent(Gdk.EventKey evnt)
        {
            // TODO handle key events
            return base.OnKeyPressEvent(evnt);
        }

        protected override bool OnKeyReleaseEvent(Gdk.EventKey evnt)
        {
            // TODO handle key events
            return base.OnKeyPressEvent(evnt);
        }

        private XMouseEventArgs ConvertToXMouseEventArgs(uint gdkButton, Gdk.EventType type, Gdk.ModifierType state, double x, double y)
        {
            XMouseButtons mb = XMouseButtons.None;
            switch (gdkButton)
            {
                case 1:
                    mb = XMouseButtons.Left;
                    break;
                case 2:
                    mb = XMouseButtons.Middle;
                    break;
                case 3:
                    mb = XMouseButtons.Right;
                    break;
            }

            int clickCount = 0;
            switch(type)
            {
                case Gdk.EventType.ButtonPress:
                    clickCount = 1;
                    break;
                case Gdk.EventType.TwoButtonPress:
                    clickCount = 2;
                    break;
                case Gdk.EventType.ThreeButtonPress:
                    clickCount = 3;
                    break;
            }

            XKeys modifiers = XKeys.None;
            if ((state & Gdk.ModifierType.ShiftMask) != 0)
            {
                modifiers |= XKeys.Shift;
            }
            if ((state & Gdk.ModifierType.ControlMask) != 0)
            {
                modifiers |= XKeys.Control;
            }
            if ((state & Gdk.ModifierType.Mod1Mask) != 0)
            {   // ALT key
                modifiers |= XKeys.Alt;
            }
            
            return new XMouseEventArgs(mb, clickCount, (int)x, (int)y, 0, modifiers);
        }
/*
        private XKeyEventArgs ConvertToXKeyEventArgs(Gdk.EventKey e)
        {
            XKeyEventArgs args = new XKeyEventArgs((XKeys)e.KeyData);
            return args;
        }
*/
        #endregion


    }
}
