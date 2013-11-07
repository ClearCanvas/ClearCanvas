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
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Rendering;
using ClearCanvas.ImageViewer.Rendering.GDI;
using vtk;

namespace ClearCanvas.ImageViewer.Vtk.Rendering
{
	/// <summary>
	/// A Win32 implementation of <see cref="IVtkRenderingSurface"/>.
	/// </summary>
	/// <remarks>
	/// Implements VTK rendering using a combination of GDI+ and OpenGL technologies.
	/// </remarks>
	internal class Win32VtkRenderingSurface : IVtkRenderingSurface
	{
		[ThreadStatic]
		private static Font _gdiFont;

		[ThreadStatic]
		private static CodeClock _renderClock;

		private static readonly bool _reportRenderingPerformance = Settings.Default.ReportRendererPerformance;

		private const double _dynamicFrameRate = 20;
		private const double _stillFrameRate = 0.0001;

		private readonly object _lockRender = new object();

		private event EventHandler _invalidated;

		private int _lastRenderTime;

		private float _statRenderDuration;
		private int _statRenderFrameCount;

		private BitmapBuffer _imageBuffer;
		private BitmapBuffer _overlayBuffer;
		private BackBuffer _finalBuffer;
		private IntPtr _contextId;

		private Rectangle _clientRectangle;

		private vtkWin32OpenGLRenderWindow _vtkRenderWindow;
		private vtkRenderer _vtkRenderer;

		private DelayedEventPublisher _dynamicRenderEventPublisher;
		private VtkSceneGraph _sceneGraphRoot;

		public Win32VtkRenderingSurface(IntPtr windowId, bool offscreen)
		{
			_imageBuffer = new BitmapBuffer(PixelFormat.Format32bppRgb);
			_overlayBuffer = new BitmapBuffer(PixelFormat.Format32bppArgb);
			_finalBuffer = new BackBuffer();

			_vtkRenderer = new vtkRenderer();
			_vtkRenderer.SetBackground(0.0f, 0.0f, 0.0f);

			_vtkRenderWindow = new vtkWin32OpenGLRenderWindow();
			_vtkRenderWindow.OffScreenRenderingOn();
			_vtkRenderWindow.DoubleBufferOff();
			_vtkRenderWindow.EraseOn();
			_vtkRenderWindow.SwapBuffersOff();
			_vtkRenderWindow.SetDesiredUpdateRate(_dynamicFrameRate);
			_vtkRenderWindow.AddRenderer(_vtkRenderer);

			_dynamicRenderEventPublisher = !offscreen ? new DelayedEventPublisher((s, e) => Render(true, null)) : null;

			WindowID = windowId;
		}

		#region Destructor/Dispose

		~Win32VtkRenderingSurface()
		{
			try
			{
				Dispose(false);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}

		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}

		#endregion

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_dynamicRenderEventPublisher != null)
				{
					_dynamicRenderEventPublisher.Cancel();
					_dynamicRenderEventPublisher.Dispose();
					_dynamicRenderEventPublisher = null;
				}

				if (_vtkRenderWindow != null)
				{
					_vtkRenderWindow.Dispose();
					_vtkRenderWindow = null;
				}

				if (_vtkRenderer != null)
				{
					_vtkRenderer.Dispose();
					_vtkRenderer = null;
				}

				if (_sceneGraphRoot != null)
				{
					_sceneGraphRoot.Dispose();
					_sceneGraphRoot = null;
				}

				if (_imageBuffer != null)
				{
					_imageBuffer.Dispose();
					_imageBuffer = null;
				}

				if (_overlayBuffer != null)
				{
					_overlayBuffer.Dispose();
					_overlayBuffer = null;
				}

				if (_finalBuffer != null)
				{
					_finalBuffer.Dispose();
					_finalBuffer = null;
				}
			}
		}

		public IntPtr WindowID { get; set; }

		public IntPtr ContextID
		{
			get { return _contextId; }
			set { _contextId = _finalBuffer.ContextId = value; }
		}

		public Rectangle ClientRectangle
		{
			get { return _clientRectangle; }
			set
			{
				if (_clientRectangle != value)
				{
					_clientRectangle = value;

					var width = value.Width != 0 ? Math.Abs(value.Width) : 10;
					var height = value.Height != 0 ? Math.Abs(value.Height) : 10;
					_vtkRenderWindow.SetSize(width, height);
					_imageBuffer.Size = _overlayBuffer.Size = new Size(width, height);
					_finalBuffer.ClientRectangle = value;
				}
			}
		}

		public BitmapBuffer ImageBuffer
		{
			get { return _imageBuffer; }
		}

		public BitmapBuffer OverlayBuffer
		{
			get { return _overlayBuffer; }
		}

		public BackBuffer FinalBuffer
		{
			get { return _finalBuffer; }
		}

		public RenderingSurfaceType Type
		{
			get { return _dynamicRenderEventPublisher != null ? RenderingSurfaceType.Onscreen : RenderingSurfaceType.Offscreen; }
		}

		public Rectangle ClipRectangle { get; set; }

		public event EventHandler Invalidated
		{
			add { _invalidated += value; }
			remove { _invalidated -= value; }
		}

		public void SetSceneRoot(VtkSceneGraph sceneGraphRoot)
		{
			if (!Equals(_sceneGraphRoot, sceneGraphRoot))
			{
				if (_sceneGraphRoot != null)
				{
					_sceneGraphRoot.DeinitializeSceneGraph(_vtkRenderer);
					_sceneGraphRoot.Dispose();
					_sceneGraphRoot = null;
				}

				_vtkRenderer.RemoveAllViewProps();
				_vtkRenderer.ResetCamera();

				_sceneGraphRoot = sceneGraphRoot;

				if (_sceneGraphRoot != null)
				{
					_sceneGraphRoot.InitializeSceneGraph(_vtkRenderer);
				}
			}
			else
			{
				sceneGraphRoot.Dispose();
			}
		}

		public void Render(UpdateOverlayCallback updateOverlayCallback)
		{
			if (_dynamicRenderEventPublisher != null)
			{
				_dynamicRenderEventPublisher.Publish(null, null);

				Render(false, updateOverlayCallback);
			}
			else
			{
				Render(true, updateOverlayCallback);
			}
		}

		public void Refresh()
		{
			lock (_lockRender)
			{
				try
				{
					FinalBuffer.RenderToScreen();
				}
				catch (Exception ex)
				{
					Platform.Log(LogLevel.Error, ex, "Buffer Refresh Exception");
				}
			}
		}

		private void Render(bool fullQuality, UpdateOverlayCallback updateOverlayCallback)
		{
			lock (_lockRender)
			{
				try
				{
					var mirrored = false;
					var mTime = -1;
					if (_sceneGraphRoot != null)
					{
						_sceneGraphRoot.UpdateSceneGraph(_vtkRenderer);
						mirrored = _sceneGraphRoot.ViewPortSpatialTransform.FlipX ^ _sceneGraphRoot.ViewPortSpatialTransform.FlipY;
						mTime = _sceneGraphRoot.GetMTime();
					}

					_vtkRenderWindow.SetDesiredUpdateRate(fullQuality ? _stillFrameRate : _dynamicFrameRate);

					// decide whether or not to render the VTK layer of the image based on the last modification time of the scene graph
					// do not use the renderer or renderwindow's MTime because they are affected by the update rate change above
					var renderTime = -1f;
					if (mTime > _lastRenderTime)
					{
						var renderClock = _renderClock ?? (_renderClock = new CodeClock());
						renderClock.Clear();
						renderClock.Start();

						var bmpData = ImageBuffer.Bitmap.LockBits(new Rectangle(0, 0, _clientRectangle.Width, _clientRectangle.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
						try
						{
							_vtkRenderWindow.Render();

							// since we just rendered, the correct OpenGL context is still 'current' on this thread so we don't need to makeCurrent
							glReadBuffer(GL_FRONT_LEFT); // select the front buffer
							glDisable(GL_TEXTURE_2D); // according to VTK code, some video drivers have issues with this feature
							glPixelStorei(GL_PACK_ALIGNMENT, 4); // align to 4 byte boundaries (since we're copying 32-bit pixels anyway)

							// now read from the OpenGL buffer directly into our surface buffer
							glReadPixels(0, 0, _clientRectangle.Width, _clientRectangle.Height, GL_BGRA, OpenGlImplementation.ReadPixelsTypeBgra, bmpData.Scan0);

							// OpenGL buffer data is a bottom-up image, and the GDI+ memory bitmap might be top-bottom, so we flip the scan lines here
							if (bmpData.Stride > 0)
								FlipImage(bmpData.Scan0, bmpData.Height, bmpData.Stride);
						}
						finally
						{
							ImageBuffer.Bitmap.UnlockBits(bmpData);

							// only record the last render time for full quality renders only
							if (fullQuality) _lastRenderTime = mTime;
						}

						renderClock.Stop();
						renderTime = renderClock.Seconds;

						// perform a single horizontal flip here if necessary, since the VTK camera does not support a mirrorred view port
						ImageBuffer.Bitmap.RotateFlip(mirrored ? RotateFlipType.RotateNoneFlipX : RotateFlipType.RotateNoneFlipNone);
					}

					if (renderTime >= 0)
					{
						if (VtkPresentationImageRenderer.ShowFps)
						{
							var font = _gdiFont ?? (_gdiFont = new Font(FontFamily.GenericMonospace, 12, FontStyle.Bold, GraphicsUnit.Point));
							var msg = string.Format("FPS: {0,6}", renderTime >= 0.000001 ? (1/renderTime).ToString("f1") : "------");
							ImageBuffer.Graphics.DrawString(msg, font, Brushes.Black, 11, 11);
							ImageBuffer.Graphics.DrawString(msg, font, Brushes.White, 10, 10);

							if (fullQuality)
							{
								msg = string.Format("TTI: {0,6} ms", renderTime >= 0.000001 ? (renderTime*1000).ToString("f1") : "------");
								ImageBuffer.Graphics.DrawString(msg, font, Brushes.Black, 11, 15 + 11);
								ImageBuffer.Graphics.DrawString(msg, font, Brushes.White, 10, 15 + 10);
							}
						}
					}

					if (fullQuality)
					{
						if (_reportRenderingPerformance && _statRenderFrameCount > 0 && _statRenderDuration > 0.000001)
						{
							var avgLowFrameRate = _statRenderFrameCount/_statRenderDuration;
							Platform.Log(LogLevel.Info, "VTKRenderer: LOD FPS: {0:f1} ({1} frame(s) in {2:f1} ms); FINAL: {3:f1} ms", avgLowFrameRate, _statRenderFrameCount, _statRenderDuration*1000, renderTime*1000);
						}
						_statRenderFrameCount = 0;
						_statRenderDuration = 0;
					}
					else
					{
						_statRenderDuration += renderTime;
						++_statRenderFrameCount;
					}

					if (updateOverlayCallback != null)
						updateOverlayCallback.Invoke();

					FinalBuffer.RenderImage(ImageBuffer);
					FinalBuffer.RenderImage(OverlayBuffer);
					FinalBuffer.RenderToScreen();

					EventsHelper.Fire(_invalidated, this, new EventArgs());
				}
				catch (Exception ex)
				{
					Platform.Log(LogLevel.Error, ex, "VTK Rendering Exception");
				}
			}
		}

		/// <summary>
		/// Flips an image vertically to switch between top-bottom and bottom-top memory representations.
		/// </summary>
		/// <param name="scan0">The address of the first scan line of the image.</param>
		/// <param name="height">The number of scan lines in the image (i.e. height).</param>
		/// <param name="stride">The number of bytes per scan line in the image.</param>
		private static void FlipImage(IntPtr scan0, int height, int stride)
		{
			var count = height >> 1; // truncated, so an odd height would leave the middle scan line untouched
			var scanN = scan0 + (height - 1)*stride; // compute address to last scan line of image
			var buffer = Marshal.AllocHGlobal(stride); // allocate a temporary buffer
			try
			{
				for (var n = 0; n < count; ++n)
				{
					CopyMemory(buffer, scan0, stride);
					CopyMemory(scan0, scanN, stride);
					CopyMemory(scanN, buffer, stride);

					scan0 += stride;
					scanN -= stride;
				}
			}
			finally
			{
				Marshal.FreeHGlobal(buffer);
			}
		}

		internal static string ReportCapabilities()
		{
			using (var surface = new Win32VtkRenderingSurface(IntPtr.Zero, true))
			{
				return surface._vtkRenderWindow.ReportCapabilities();
			}
		}

		#region Win32 API

		// ReSharper disable InconsistentNaming

		private const uint GL_BGRA = 0x80E1;
		private const uint GL_TEXTURE_2D = 0x0DE1;
		private const uint GL_PACK_ALIGNMENT = 0x0D05;
		private const uint GL_FRONT_LEFT = 0x0400;

		[DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
		private static extern void CopyMemory(IntPtr dest, IntPtr src, int size);

		[DllImport("opengl32.dll")]
		private static extern void glPixelStorei(uint pname, int param);

		[DllImport("opengl32.dll")]
		private static extern void glReadBuffer(uint mode);

		[DllImport("opengl32.dll")]
		private static extern void glDisable(uint cap);

		[DllImport("opengl32.dll")]
		private static extern void glReadPixels(int x, int y, int width, int height, uint format, uint type, IntPtr pixels);

		// ReSharper restore InconsistentNaming

		#endregion
	}
}