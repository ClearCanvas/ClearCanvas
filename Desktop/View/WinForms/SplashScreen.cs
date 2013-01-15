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
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Utilities.Manifest;

namespace ClearCanvas.Desktop.View.WinForms
{
	/// <summary>
	/// This class represents the splash screen form.  All access to its properties is 
	/// mitigated through Invoke to avoid cross-threading form exceptions.
	/// </summary>
    public partial class SplashScreen : Form
    {
		// Delegate instances used to call user interface functions from other threads
		public delegate void UpdateStatusTextDelegate(string statusText);
		public delegate void UpdateLicenseTextDelegate(string licenseText);
		public delegate void UpdateOpacityDelegate(double opacity);
		public delegate void AddAssemblyIconDelegate(Assembly pluginAssembly);
		
		private const int DropShadowOffset = 15;
		private BitmapOverlayForm _dropShadow = null;

		private const int IconWidth = 48;
		private const int IconHeight = 48;
		private const int IconPaddingX = 12;
		private const int IconPaddingY = 6;
		private const int IconTextHeight = 36;

		private Rectangle _pluginIconsRectangle = Rectangle.Empty;
		private int _nextIconPositionX = 0;
		private int _nextIconPositionY = 0;

        public SplashScreen()
        {
            InitializeComponent();

			Initialize();
		}

		#region Public Methods

		/// <summary>
		/// Updates the splash screen's status text.
		/// </summary>
		/// <param name="status">The splash screen new status text.</param>
		public void UpdateStatusText(string statusText)
		{
			if (this.Handle != IntPtr.Zero)
				Invoke(new UpdateStatusTextDelegate(SetStatusText), new Object[] { statusText });
			else
				SetStatusText(statusText);
		}

		/// <summary>
		/// Updates the splash screen's license text.
		/// </summary>
		/// <param name="status">The splash screen new license text.</param>
		public void UpdateLicenseText(string licenseText)
		{
			if (this.Handle != IntPtr.Zero)
				Invoke(new UpdateLicenseTextDelegate(SetLicenseText), new Object[] { licenseText });
			else
				SetLicenseText(licenseText);
		}

		/// <summary>
		/// Updates the splash screen's opacity.
		/// </summary>
		/// <param name="opacity">The splash screen's new opacity.</param>
		public void UpdateOpacity(double opacity)
		{
			if (this.Handle != IntPtr.Zero)
				Invoke(new UpdateOpacityDelegate(SetOpacity), new Object[] { opacity });
			else
				SetOpacity(opacity);
		}

		public void AddAssemblyIcon(Assembly pluginAssembly)
		{
			if (this.Handle != IntPtr.Zero)
				Invoke(new AddAssemblyIconDelegate(AddAssemblyIconToImage), new Object[] { pluginAssembly });
			else
				AddAssemblyIconToImage(pluginAssembly);
		}

		public static Stream OpenSplashImageResourceStream()
		{
			var oemPath = System.IO.Path.Combine(Platform.InstallDirectory, @"oem\splash.png");
			if (File.Exists(oemPath))
				return File.OpenRead(oemPath);

			var assemblyName = SplashScreenSettings.Default.BackgroundImageAssemblyName;
			var resourceName = SplashScreenSettings.Default.BackgroundImageResourceName;
			if (!string.IsNullOrEmpty(assemblyName) && !string.IsNullOrEmpty(resourceName))
			{
				var assembly = Assembly.Load(assemblyName);
				if (assembly != null)
				{
					var streamName = assemblyName + @"." + resourceName;
					return assembly.GetManifestResourceStream(streamName);
				}
			}

			return null;
		}

		public static Image CreateRecoloredGraphic(Image image)
		{
			var oem = OemConfiguration.Load();
			return CreateRecoloredGraphic(image, oem.GlyphColor);
		}

		public static Image CreateRecoloredGraphic(Image image, Color color)
		{
			var bitmap = new Bitmap(image);
			if (!color.IsEmpty && color.ToArgb() != 0)
			{
				var bitmapData = bitmap.LockBits(new Rectangle(Point.Empty, image.Size), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
				try
				{
					var rescaleRed = color.R/255f;
					var rescaleGreen = color.G/255f;
					var rescaleBlue = color.B/255f;
					var length = bitmapData.Height*bitmapData.Width;
					var buffer = new int[length];
					Marshal.Copy(bitmapData.Scan0, buffer, 0, length);
					for (var n = 0; n < length; ++n)
					{
						var value = buffer[n];
						buffer[n] = ((int) (value & 0xFF000000)) | (((byte) (((value >> 16) & 0x0FF)*rescaleRed)) << 16) | (((byte) (((value >> 8) & 0x0FF)*rescaleGreen)) << 8) | ((byte) ((value & 0x0FF)*rescaleBlue));
					}
					Marshal.Copy(buffer, 0, bitmapData.Scan0, length);
				}
				finally
				{
					bitmap.UnlockBits(bitmapData);
				}
			}
			return bitmap;
		}

		#endregion

		#region Private Methods

		private void Initialize()
		{
			// No status at first
			SetStatusText(string.Empty);

			// Initialize the version text to the executing assembly's
			_version.Text = String.Format(SplashScreenSettings.Default.VersionTextFormat, ProductInformation.GetVersion(true, true, true));
			_copyright.Text = ProductInformation.Copyright;
			_license.Text = ProductInformation.License;

			// Make the window completely transparent
			Opacity = 0;

            // Set the manifest warning.
            _manifest.Visible = !ManifestVerification.Valid;
             
			// Apply any splash screen settings, if requested
			if (SplashScreenSettings.Default.UseSplashScreenSettings)
			{
				this.SuspendLayout();

				try
				{
					var stream = SplashScreen.OpenSplashImageResourceStream();
					if (stream != null)
					{
						// GDI+ resource management quirk: don't dispose the source stream (or create an independent copy of the bitmap)
						BackgroundImage = new Bitmap(stream);
						ClientSize = BackgroundImage.Size;
					}
				}
				catch (Exception ex)
				{
					Platform.Log(LogLevel.Warn, ex, "Failed to resolve splash screen resources.");
				}

				this._status.Visible = SplashScreenSettings.Default.StatusVisible;
				this._status.Location = SplashScreenSettings.Default.StatusLocation;
				this._status.Size = SplashScreenSettings.Default.StatusSize;
				this._status.AutoSize = SplashScreenSettings.Default.StatusAutoSize;
				this._status.ForeColor = SplashScreenSettings.Default.StatusForeColor;
				this._status.Font = SplashScreenSettings.Default.StatusFontBold ? new Font(this._status.Font, FontStyle.Bold) : this._status.Font;
				this._status.TextAlign = SplashScreenSettings.Default.StatusTextAlign;

              	this._copyright.Visible = SplashScreenSettings.Default.CopyrightVisible;
				this._copyright.Location = SplashScreenSettings.Default.CopyrightLocation;
				this._copyright.Size = SplashScreenSettings.Default.CopyrightSize;
				this._copyright.AutoSize = SplashScreenSettings.Default.CopyrightAutoSize;
				this._copyright.ForeColor = SplashScreenSettings.Default.CopyrightForeColor;
				this._copyright.Font = SplashScreenSettings.Default.CopyrightFontBold ? new Font(this._copyright.Font, FontStyle.Bold) : this._copyright.Font;
				this._copyright.TextAlign = SplashScreenSettings.Default.CopyrightTextAlign;

				this._version.Visible = SplashScreenSettings.Default.VersionVisible;
				this._version.Location = SplashScreenSettings.Default.VersionLocation;
				this._version.Size = SplashScreenSettings.Default.VersionSize;
				this._version.AutoSize = SplashScreenSettings.Default.VersionAutoSize;
				this._version.ForeColor = SplashScreenSettings.Default.VersionForeColor;
				this._version.Font = SplashScreenSettings.Default.VersionFontBold ? new Font(this._version.Font, FontStyle.Bold) : this._version.Font;
				this._version.TextAlign = SplashScreenSettings.Default.VersionTextAlign;

				this._license.Visible = SplashScreenSettings.Default.LicenseVisible;
				this._license.Location = SplashScreenSettings.Default.LicenseLocation;
				this._license.Size = SplashScreenSettings.Default.LicenseSize;
				this._license.AutoSize = SplashScreenSettings.Default.LicenseAutoSize;
				this._license.ForeColor = SplashScreenSettings.Default.LicenseForeColor;
				this._license.Font = SplashScreenSettings.Default.LicenseFontBold ? new Font(this._license.Font, FontStyle.Bold) : this._license.Font;
				this._license.TextAlign = SplashScreenSettings.Default.LicenseTextAlign;

                this._manifest.Location = SplashScreenSettings.Default.ManifestLocation;
                this._manifest.Size = SplashScreenSettings.Default.ManifestSize;
                this._manifest.AutoSize = SplashScreenSettings.Default.ManifestAutoSize;
                this._manifest.ForeColor = SplashScreenSettings.Default.ManifestForeColor;
				this._manifest.Font = SplashScreenSettings.Default.ManifestFontBold ? new Font(this._manifest.Font, FontStyle.Bold) : this._manifest.Font;
                this._manifest.TextAlign = SplashScreenSettings.Default.ManifestTextAlign;

				this._pluginIconsRectangle = SplashScreenSettings.Default.PluginIconsRectangle;
				this._nextIconPositionX = _pluginIconsRectangle.Left + IconPaddingX / 2;
				this._nextIconPositionY = _pluginIconsRectangle.Top;

				this.ResumeLayout();
			}
		}

		private void SetStatusText(string statusText)
		{
			_status.Text = statusText;
		}

		private void SetLicenseText(string licenseText)
		{
			_license.Text = licenseText;
		}

		private void SetOpacity(double opacity)
		{
			Opacity = opacity;

			// Pass the opacity to the drop shadow form if it exists
			if (_dropShadow != null && !_dropShadow.IsDisposed)
				_dropShadow.BitmapOpacity = opacity;
		}

		private void AddAssemblyIconToImage(Assembly pluginAssembly)
		{
			object[] pluginAttributes = pluginAssembly.GetCustomAttributes(typeof(PluginAttribute), false);

			foreach (PluginAttribute pluginAttribute in pluginAttributes)
			{
				if (!string.IsNullOrEmpty(pluginAttribute.Icon))
				{
					try
					{
						IResourceResolver resolver = new ResourceResolver(pluginAssembly);
						Bitmap icon = new Bitmap(resolver.OpenResource(pluginAttribute.Icon));

						// Burn the icon into the background image
						Graphics g = Graphics.FromImage(this.BackgroundImage);

						int positionX = _nextIconPositionX;
						int positionY = _nextIconPositionY;

						g.DrawImage(icon, positionX, positionY, IconWidth, IconHeight);

						// Burn the icon's name and version into the background image
						string pluginName = pluginAttribute.Name;
						string pluginVersion = string.Empty;
						try
						{
							FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(pluginAssembly.Location);
							pluginVersion = versionInfo.ProductVersion;
						}
						catch
						{
						}

						Font font = new Font("Tahoma", 10F, GraphicsUnit.Pixel);
						Brush brush = new SolidBrush(Color.FromArgb(50, 50, 50));

						StringFormat format = new StringFormat();
						format.Alignment = StringAlignment.Center;

						Rectangle layoutRect = new Rectangle(positionX - IconPaddingX / 2, positionY + IconHeight, IconWidth + IconPaddingX, IconTextHeight);

						g.DrawString(pluginName + "\n" + pluginVersion, font, brush, layoutRect, format);

						font.Dispose();
						brush.Dispose();

						g.Dispose();

						// Advance to the next icon position within the plugin rectangle
						_nextIconPositionX += (IconWidth + IconPaddingX);
						if (_nextIconPositionX + IconWidth + IconPaddingX / 2 > _pluginIconsRectangle.Right)
						{
							_nextIconPositionX = _pluginIconsRectangle.Left + IconPaddingX / 2;
							_nextIconPositionY += IconPaddingY + IconHeight + IconTextHeight;
						}

						this.Invalidate();
					}
					catch
					{
					}
				}
			}
		}

		private void DrawDropShadow(Graphics graphics, Rectangle rect, Color shadowColor, int shadowDepth, byte maxAlpha)
		{
			// Determine the shadow colors
			Color darkShadow = Color.FromArgb(maxAlpha, shadowColor);
			Color lightShadow = Color.FromArgb(0, shadowColor);

			// Create a brush that will create a softshadow circle
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddEllipse(0, 0, 2 * shadowDepth, 2 * shadowDepth);

			PathGradientBrush brush = new PathGradientBrush(graphicsPath);
			brush.CenterColor = darkShadow;
			brush.SurroundColors = new Color[] { lightShadow };

			// Generate a softshadow pattern that can be used to paint the shadow
			Bitmap pattern = new Bitmap(2 * shadowDepth, 2 * shadowDepth);

			Graphics patternGraphics = Graphics.FromImage(pattern);
			patternGraphics.FillEllipse(brush, 0, 0, 2 * shadowDepth, 2 * shadowDepth);

			patternGraphics.Dispose();
			brush.Dispose();

			// Top right corner
			graphics.DrawImage(pattern, new Rectangle(rect.Right - shadowDepth, rect.Top + shadowDepth, shadowDepth, shadowDepth), shadowDepth, 0, shadowDepth, shadowDepth, GraphicsUnit.Pixel);

			// Right side
			graphics.DrawImage(pattern, new Rectangle(rect.Right - shadowDepth, rect.Top + 2 * shadowDepth, shadowDepth, rect.Height - 3 * shadowDepth), shadowDepth, shadowDepth, shadowDepth, 1, GraphicsUnit.Pixel);

			// Bottom right corner
			graphics.DrawImage(pattern, new Rectangle(rect.Right - shadowDepth, rect.Bottom - shadowDepth, shadowDepth, shadowDepth), shadowDepth, shadowDepth, shadowDepth, shadowDepth, GraphicsUnit.Pixel);

			// Bottom side
			graphics.DrawImage(pattern, new Rectangle(rect.Left + 2 * shadowDepth, rect.Bottom - shadowDepth, rect.Width - 3 * shadowDepth, shadowDepth), shadowDepth, shadowDepth, 1, shadowDepth, GraphicsUnit.Pixel);

			// Bottom left corner
			graphics.DrawImage(pattern, new Rectangle(rect.Left + shadowDepth, rect.Bottom - shadowDepth, shadowDepth, shadowDepth), 0, shadowDepth, shadowDepth, shadowDepth, GraphicsUnit.Pixel);

			pattern.Dispose();
		}

		#endregion

		#region Private Event Handlers

		private void SplashScreen_Shown(object sender, EventArgs e)
		{
			// Create the drop shadow form when the splash screen is shown
			if (_dropShadow == null)
			{
				_dropShadow = new BitmapOverlayForm();

				_dropShadow.Owner = this;
				_dropShadow.TopMost = false;
				_dropShadow.ShowInTaskbar = false;

				// Show the drop shadow form
				_dropShadow.Show();
			}

			// Position the drop shadow form (has to be done after it's shown)
			_dropShadow.Top = this.Top - DropShadowOffset;
			_dropShadow.Left = this.Left - DropShadowOffset;

			// Create the drop shadow bitmap given the size of the background image
			Bitmap dropShadowBmp = new Bitmap(this.BackgroundImage.Width + DropShadowOffset * 2, this.BackgroundImage.Height + DropShadowOffset * 2, PixelFormat.Format32bppArgb);

			Graphics graphics = Graphics.FromImage(dropShadowBmp);
			DrawDropShadow(graphics, new Rectangle(0, 0, dropShadowBmp.Width, dropShadowBmp.Height), Color.FromArgb(255, 0, 0, 0), DropShadowOffset, 96);
			graphics.Dispose();

			_dropShadow.Bitmap = dropShadowBmp;
			_dropShadow.BitmapOpacity = Opacity;

			// Make sure the splash screen and drop shadow are the frontmost windows when they appear
			BringToFront();
			_dropShadow.BringToFront();
		}

		#endregion
	}
}