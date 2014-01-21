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
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Rendering;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
    public partial class MagnificationTool
    {
        private string _lastRenderExceptionMessage = null;
        private PresentationImage _magnificationImage;
        private bool _firstRender;
        private Point _tileLocation;

        private void RenderImage(DrawArgs args)
        {
            if (args.DrawMode == DrawMode.Refresh)
            {
                RefreshImage(args);
                return;
            }

            if (_firstRender)
            {
                // the first time we try to render a freshly cloned image, we need to draw it twice
                // this is to make sure the client rectangle is updated when we try to compute the correct point of interest
                _firstRender = false;
                RenderImage(args);
            }

            try
            {
                var sourceTransform = (IImageSpatialTransform)((ISpatialTransformProvider)SelectedPresentationImage).SpatialTransform;
                var transform = (IImageSpatialTransform)((ISpatialTransformProvider)_magnificationImage).SpatialTransform;

                float scale = sourceTransform.Scale * ToolSettings.DefaultInstance.MagnificationFactor;
                transform.ScaleToFit = false;
                transform.Scale = scale;
                transform.TranslationX = 0;
                transform.TranslationY = 0;

                var midPoint = new PointF(args.RenderingSurface.ClientRectangle.Width / 2f, args.RenderingSurface.ClientRectangle.Height / 2f);
                var sourcePointOfInterest = sourceTransform.ConvertToSource(_tileLocation);
                // compute translation required to move the point of interest on the magnified image to the centre of the client area
                var translation = transform.ConvertToSource(midPoint) - new SizeF(sourcePointOfInterest);
                transform.TranslationX = translation.X;
                transform.TranslationY = translation.Y;

                _magnificationImage.Draw(args);

                // clear the rendering exception message
                _lastRenderExceptionMessage = null;
            }
            catch (Exception ex)
            {
                Platform.Log(LogLevel.Error, ex,
                             "An error has occured while rendering the magnified contents of the tile.");

                // a rendering exception was encountered, so set the message field
                _lastRenderExceptionMessage = ex is RenderingException
                                                  ? ((RenderingException)ex).UserMessage
                                                  : ex.Message;

                // we cannot simply pass the existing Graphics because we haven't released its hDC yet
                // if we do, we'll get a "Object is currently in use elsewhere" exception
                DrawErrorMessage(_lastRenderExceptionMessage, args.RenderingSurface.ContextID, args.RenderingSurface.ClientRectangle);
            }
        }

        private void RefreshImage(DrawArgs args)
        {
            try
            {
                // if there was an exception the last time we rendered the buffer, don't refresh from the buffer and instead redraw the error message
                if (string.IsNullOrEmpty(_lastRenderExceptionMessage))
                {
                    _magnificationImage.Draw(args);
                }
                else
                {
                    // we cannot simply pass the existing Graphics because we haven't released its hDC yet
                    // if we do, we'll get a "Object is currently in use elsewhere" exception
                    DrawErrorMessage(_lastRenderExceptionMessage, args.RenderingSurface.ContextID, args.RenderingSurface.ClientRectangle);
                }
            }
            catch (Exception ex)
            {
                Platform.Log(LogLevel.Error, ex, "An error has occured while refreshing the magnified contents of the tile.");

                var exceptionMessage = ex is RenderingException ? ((RenderingException)ex).UserMessage : ex.Message;

                // we cannot simply pass the Graphics because we haven't released its hDC yet
                // if we do, we'll get a "Object is currently in use elsewhere" exception
                DrawErrorMessage(exceptionMessage, args.RenderingSurface.ContextID, args.RenderingSurface.ClientRectangle);
            }
        }

        private static void DrawErrorMessage(string errorMessage, IntPtr hDC, Rectangle bounds)
        {
            using (var errorGraphics = System.Drawing.Graphics.FromHdc(hDC))
            {
                // don't give the user any false expectation of the validity of the magnified output by clearing any partially rendered results
                errorGraphics.FillRectangle(Brushes.Black, bounds);

                using (var format = new StringFormat
                {
                    Trimming = StringTrimming.EllipsisCharacter,
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    FormatFlags = StringFormatFlags.NoClip
                })
                {
                    // use the system-determined default font to ensure we can't fail at drawing error messages (cause some systems might not have Arial)
                    using (var font = new Font(SystemFonts.DefaultFont.Name, 12.0f))
                    {
                        errorGraphics.DrawString(errorMessage, font, Brushes.WhiteSmoke, bounds, format);
                    }
                }
            }
        }

        private void HideOverlays()
        {
            if (_magnificationImage is IAnnotationLayoutProvider)
            {
                string magFactor = String.Format("{0:F1}x", ToolSettings.DefaultInstance.MagnificationFactor);
                AnnotationLayout layout = new AnnotationLayout();
                BasicTextAnnotationItem item = new BasicTextAnnotationItem("mag", "mag", "mag", magFactor);
                AnnotationBox box = new AnnotationBox(new RectangleF(0.8F, 0F, .2F, .05F), item);
                box.Justification = AnnotationBox.JustificationBehaviour.Right;
                box.VerticalAlignment = AnnotationBox.VerticalAlignmentBehaviour.Top;
                layout.AnnotationBoxes.Add(box);
				((IAnnotationLayoutProvider)_magnificationImage).AnnotationLayout = layout;
            }

            if (_magnificationImage is IOverlayGraphicsProvider)
            {
                GraphicCollection graphics = ((IOverlayGraphicsProvider)_magnificationImage).OverlayGraphics;
                foreach (IGraphic graphic in graphics)
                    graphic.Visible = false;
            }

            if (_magnificationImage is IApplicationGraphicsProvider)
            {
                GraphicCollection graphics = ((IApplicationGraphicsProvider)_magnificationImage).ApplicationGraphics;
                foreach (IGraphic graphic in graphics)
                    graphic.Visible = false;
            }

            //we want the Dicom graphics to be visible (e.g. shutter and embedded overlays)

            //if (_magnificationImage is IDicomPresentationImage)
            //{
            //    GraphicCollection graphics = ((IDicomPresentationImage)_magnificationImage).DicomGraphics;
            //    foreach (IGraphic graphic in graphics)
            //        graphic.Visible = false;
            //}
        }

        private void InitializeMagnificationImage()
        {
            if (_magnificationImage != null)
                return;

            if (SelectedPresentationImage == null)
                throw new ArgumentException("The image cannot be null", "value");

            if (!(SelectedPresentationImage is ISpatialTransformProvider))
                throw new ArgumentException("The image must implement ISpatialTransformProvider", "value");

            if (!(((ISpatialTransformProvider)SelectedPresentationImage).SpatialTransform is IImageSpatialTransform))
                throw new ArgumentException("The image must provide an IImageSpatialTransform", "value");

            DisposeMagnificationImage();

            _firstRender = true;
            _magnificationImage = (PresentationImage)SelectedPresentationImage.Clone();

            HideOverlays();
        }

        private void DisposeMagnificationImage()
        {
            if (_magnificationImage != null)
            {
                _magnificationImage.Dispose();
                _magnificationImage = null;
            }
        }
    }
}
