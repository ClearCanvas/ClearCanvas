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
using System.Windows.Forms;

/// <summary>
/// Exposes various Win32 types and functions needed by BitmapOverlayForm.
/// </summary>
internal class Win32
{
	public const Int32 WS_EX_LAYERED = 0x00080000;
	public const Int32 ULW_ALPHA = 0x00000002;

	public enum BOOL { FALSE = 0, TRUE };

	[StructLayout(LayoutKind.Sequential)]
	public struct POINT
	{
		public Int32 x;
		public Int32 y;

		public POINT(Int32 x, Int32 y) { this.x = x; this.y = y; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct SIZE
	{
		public Int32 cx;
		public Int32 cy;

		public SIZE(Int32 cx, Int32 cy) { this.cx = cx; this.cy = cy; }
	}

	[DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
	public static extern IntPtr GetDC(IntPtr hWnd);

	[DllImport("user32.dll", ExactSpelling = true)]
	public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

	[DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
	public static extern BOOL UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref POINT pptDst, ref SIZE psize, IntPtr hdcSrc, ref POINT pprSrc, Int32 crKey, ref GDI32.BLENDFUNCTION pblend, Int32 dwFlags);
}

/// <summary>
/// Exposes various GDI32 types and functions needed by BitmapOverlayForm.
/// </summary>
internal class GDI32
{
	public const byte AC_SRC_OVER = 0x00;
	public const byte AC_SRC_ALPHA = 0x01;

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct BLENDFUNCTION
	{
		public byte BlendOp;
		public byte BlendFlags;
		public byte SourceConstantAlpha;
		public byte AlphaFormat;
	}

	[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
	public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

	[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
	public static extern Win32.BOOL DeleteDC(IntPtr hdc);

	[DllImport("gdi32.dll", ExactSpelling = true)]
	public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

	[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
	public static extern Win32.BOOL DeleteObject(IntPtr hObject);
}

/// <summary>
/// This Form extension class does away with a form's normal rendering mechanism and instead renders 
/// an alpha-blended bitmap in its place.
/// </summary>
internal class BitmapOverlayForm : Form
{
	private Bitmap _bitmap = null;
	private double _bitmapOpacity = 1.0;
	
	public BitmapOverlayForm()
	{
		// The form itself shouldn't have a border
		FormBorderStyle = FormBorderStyle.None;
	}

	/// <summary>
	/// The bitmap to overlay onto the form.
	/// </summary>
	public Bitmap Bitmap
	{
		get { return _bitmap; }
		set
		{
			_bitmap = value;
			RefreshBitmap();
		}
	}
 
	/// <summary>
	/// The opacity of the bitmap overlay.
	/// </summary>
	public double BitmapOpacity
	{
		get { return _bitmapOpacity; }
		set
		{
			_bitmapOpacity = value;
			RefreshBitmap();
		}
	}

	/// <summary>
	/// Redraws the bitmap overlay.
	/// </summary>
	/// <returns>True if successful, false otherwise.</returns>
	protected bool RefreshBitmap()
	{
		// The bitmap must be 32-bit RGBA
		if (_bitmap == null || _bitmap.PixelFormat != PixelFormat.Format32bppArgb)
			return false;

		// Create a memory DC that's compatible with the screen
		IntPtr screenDC = Win32.GetDC(IntPtr.Zero);
		if (screenDC == IntPtr.Zero)
			return false;

		IntPtr memDC = GDI32.CreateCompatibleDC(screenDC);
		if (memDC == IntPtr.Zero)
		{
			Win32.ReleaseDC(IntPtr.Zero, screenDC);
			return false;
		}

		// Prepare to draw the bitmap
		IntPtr hBitmap = IntPtr.Zero;
		IntPtr oldBitmap = IntPtr.Zero;

		bool success = false;

		try
		{
			// Select the bitmap into the memory DC
			hBitmap = _bitmap.GetHbitmap(Color.FromArgb(0));  // grab a GDI handle from this GDI+ bitmap
			oldBitmap = GDI32.SelectObject(memDC, hBitmap);

			// Call UpdateLayeredWindow to effectively blit the contents of the memory DC into the form while performing alpha blending
			Win32.POINT windowTopLeft = new Win32.POINT(Left, Top);

			Win32.SIZE bitmapSize = new Win32.SIZE(_bitmap.Width, _bitmap.Height);
			Win32.POINT bitmapTopLeft = new Win32.POINT(0, 0);

			byte blendAlpha = 0;
			if (_bitmapOpacity < 0)
				blendAlpha = 0;
			else if (_bitmapOpacity > 1)
				blendAlpha = 255;
			else
				blendAlpha = (byte)(_bitmapOpacity * 255);

			GDI32.BLENDFUNCTION blendFunc = new GDI32.BLENDFUNCTION();
			blendFunc.BlendOp = GDI32.AC_SRC_OVER;
			blendFunc.BlendFlags = 0;
			blendFunc.SourceConstantAlpha = blendAlpha;
			blendFunc.AlphaFormat = GDI32.AC_SRC_ALPHA;

			Win32.UpdateLayeredWindow(Handle, screenDC, ref windowTopLeft, ref bitmapSize, memDC, ref bitmapTopLeft, 0, ref blendFunc, Win32.ULW_ALPHA);

			success = true;
		}
		finally
		{
			// Clean up the resources
			if (hBitmap != IntPtr.Zero)
			{
				GDI32.SelectObject(memDC, oldBitmap);
				GDI32.DeleteObject(hBitmap);
			}

			GDI32.DeleteDC(memDC);
			Win32.ReleaseDC(IntPtr.Zero, screenDC);
		}

		return success;
	}

	protected override CreateParams CreateParams
	{
		get
		{
			// The form must have the WS_EX_LAYERED extended style
			CreateParams createParams = base.CreateParams;
			createParams.ExStyle |= Win32.WS_EX_LAYERED;

			return createParams;
		}
	}
}
