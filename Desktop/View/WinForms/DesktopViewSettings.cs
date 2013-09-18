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

using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ClearCanvas.Common.Configuration;
using System.Xml;
using System.ComponentModel;
using System.Text;
using ClearCanvas.Common;
using System;

namespace ClearCanvas.Desktop.View.WinForms
{
	[SettingsGroupDescription("Stores settings such as window size/position and toolstrip alignment.")]
	[SettingsProvider(typeof(StandardSettingsProvider))]
	internal sealed partial class DesktopViewSettings : IMigrateSettings
	{
		#region IMigrateSettings Members

		public void MigrateSettingsProperty(SettingsPropertyMigrationValues migrationValues)
		{
			if (migrationValues.PropertyName != "EnableNonPermissibleActions")
				migrationValues.CurrentValue = migrationValues.PreviousValue;
		}

		#endregion

		#region Desktop class

		private class Desktop
		{
			#region Fields

			public readonly Rectangle VirtualScreen;

			private readonly List<Screen> _screens = new List<Screen>();
			private readonly List<DesktopWindow> _desktopWindows = new List<DesktopWindow>();

			#endregion

			private Desktop(Rectangle virtualScreen)
			{
				VirtualScreen = virtualScreen;
			}

			#region Public Properties and Methods

			public bool IsCurrent
			{
				get
				{
					if (VirtualScreen != SystemInformation.VirtualScreen)
						return false;

					List<Screen> currentScreens = Screen.GetAllScreens();
					if (currentScreens.Count == _screens.Count)
					{
						for (int i = 0; i < currentScreens.Count; ++i)
						{
							if (!currentScreens[i].Equals(_screens[i]))
								return false;
						}
					}

					return true;
				}
			}

			public static Desktop CreateCurrent()
			{
				Desktop desktop = new Desktop(SystemInformation.VirtualScreen);
				desktop._screens.AddRange(Screen.GetAllScreens());
				return desktop;
			}

			public static Desktop FromXmlElement(XmlElement element)
			{
				Platform.CheckTrue(element.Name == "desktop", "The settings xml is invalid.");
				
				TypeConverter converter = TypeDescriptor.GetConverter(typeof(Rectangle));
				Rectangle virtualScreen = (Rectangle)converter.ConvertFromInvariantString(element.GetAttribute("virtual-screen"));

				Desktop desktop = new Desktop(virtualScreen);
				
				foreach (XmlElement screen in element["screens"].ChildNodes)
					desktop._screens.Add(Screen.FromXmlElement(screen));

				foreach (XmlElement window in element["desktop-windows"].ChildNodes)
					desktop._desktopWindows.Add(DesktopWindow.FromXmlElement(window));

				return desktop;
			}

			public void WriteToXml(XmlTextWriter writer)
			{
				TypeConverter converter = TypeDescriptor.GetConverter(typeof(Rectangle));
				string virtualScreen = converter.ConvertToInvariantString(VirtualScreen);

				writer.WriteStartElement("desktop");
				writer.WriteAttributeString("virtual-screen", virtualScreen);
				
				writer.WriteStartElement("screens");
				foreach (Screen screen in _screens)
					screen.WriteToXml(writer);
				writer.WriteEndElement();
				
				writer.WriteStartElement("desktop-windows");
				foreach (DesktopWindow window in _desktopWindows)
					window.WriteToXml(writer);
				writer.WriteEndElement();
				
				writer.WriteEndElement();
			}

			public DesktopWindow GetDesktopWindow(string name)
			{
				return _desktopWindows.Find(delegate(DesktopWindow window) { return window.Name == name; });
			}

			public void AddDesktopWindow(DesktopWindow desktopWindow)
			{
				DesktopWindow existing = _desktopWindows.Find(delegate(DesktopWindow test) { return test.Name == desktopWindow.Name; });
				if (existing != null)
					throw new InvalidOperationException("A desktop window with the specified name already exists.");

				_desktopWindows.Add(desktopWindow);
			}

			#endregion
		}

		#endregion

		#region Screen class

		private class Screen
		{
			#region Fields

			public readonly Rectangle Bounds;

			#endregion

			private Screen(Rectangle bounds)
			{
				this.Bounds = bounds;
			}

			#region Public Methods

			public static List<Screen> GetAllScreens()
			{
				List<Screen> screens = new List<Screen>();

				foreach (System.Windows.Forms.Screen screen in System.Windows.Forms.Screen.AllScreens)
					screens.Add(new Screen(screen.Bounds));

				return screens;
			}

			public static Screen FromXmlElement(XmlElement element)
			{
				Platform.CheckTrue(element.Name == "screen", "The settings xml is invalid.");

				string bounds = element.GetAttribute("bounds");
				TypeConverter converter = TypeDescriptor.GetConverter(typeof(Rectangle));
				return new Screen((Rectangle) converter.ConvertFromInvariantString(bounds));
			}

			public void WriteToXml(XmlTextWriter writer)
			{
				TypeConverter converter = TypeDescriptor.GetConverter(typeof(Rectangle));
				string bounds = converter.ConvertToInvariantString(this.Bounds);

				writer.WriteStartElement("screen");
				writer.WriteAttributeString("bounds", bounds);
				writer.WriteEndElement();
			}

			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				if (obj == this)
					return true;

				if (obj is Screen)
					return Bounds == ((Screen) obj).Bounds;

				return false;
			}
			
			#endregion
		}

		#endregion

		#region DesktopWindow class

		private class DesktopWindow
		{
			public DesktopWindow(string name)
			{
				this.Name = name;
			}

			#region Fields

			public readonly string Name;
			public Rectangle Bounds;
			public FormWindowState State;

			private readonly List<Shelf> _shelves = new List<Shelf>();

			#endregion

			#region Public Methods

			public static DesktopWindow FromXmlElement(XmlElement element)
			{
				Platform.CheckTrue(element.Name == "desktop-window", "The settings xml is invalid.");

				string name = element.GetAttribute("name");
				TypeConverter converter = TypeDescriptor.GetConverter(typeof(Rectangle));
				Rectangle restoreBounds = (Rectangle)converter.ConvertFromInvariantString(element.GetAttribute("bounds"));

				converter = TypeDescriptor.GetConverter(typeof(FormWindowState));
				FormWindowState restoreState = (FormWindowState)converter.ConvertFromInvariantString(element.GetAttribute("state"));

				DesktopWindow window = new DesktopWindow(name);
				window.Bounds = restoreBounds;
				window.State = restoreState;

				foreach (XmlElement shelf in element["shelves"].ChildNodes)
					window._shelves.Add(Shelf.FromXmlElement(shelf));

				return window;
			}

			public void WriteToXml(XmlWriter writer)
			{
				TypeConverter converter = TypeDescriptor.GetConverter(typeof(Rectangle));
				string bounds = converter.ConvertToInvariantString(this.Bounds);
				converter = TypeDescriptor.GetConverter(typeof(FormWindowState));
				string state = converter.ConvertToInvariantString(this.State);

				writer.WriteStartElement("desktop-window");
				writer.WriteAttributeString("name", this.Name);
				writer.WriteAttributeString("bounds", bounds);
				writer.WriteAttributeString("state", state);
				
				writer.WriteStartElement("shelves");
				foreach (Shelf shelf in _shelves)
					shelf.WriteToXml(writer);
				writer.WriteEndElement();
				
				writer.WriteEndElement();
			}

			public Shelf GetShelf(string name)
			{
				return _shelves.Find(delegate(Shelf shelf) { return shelf.Name == name; });
			}

			public void AddShelf(Shelf shelf)
			{
				Shelf existing = _shelves.Find(delegate(Shelf test) { return test.Name == shelf.Name; });
				if (existing != null)
					throw new InvalidOperationException("A shelf with the specified name already exists.");

				_shelves.Add(shelf);
			}

			#endregion
		}

		#endregion
		
		#region Shelf class

		private class Shelf
		{
			#region Fields

			public readonly string Name;
			public XmlDocument RestoreDocument;
			
			#endregion

			public Shelf(string name)
			{
				this.Name = name;
			}

			#region Public Methods

			public static Shelf FromXmlElement(XmlElement element)
			{
				Platform.CheckTrue(element.Name == "shelf", "The settings xml is invalid.");

				string name = element.GetAttribute("name");
				XmlElement restoreElement = (XmlElement) element["restore-document"].FirstChild;
				XmlDocument restoreDocument = new XmlDocument();
				restoreDocument.AppendChild(restoreDocument.ImportNode(restoreElement, true));

				Shelf shelf = new Shelf(name);
				shelf.RestoreDocument = restoreDocument;
				return shelf;
			}

			public void WriteToXml(XmlWriter writer)
			{
				writer.WriteStartElement("shelf");
				writer.WriteAttributeString("name", this.Name);
				writer.WriteStartElement("restore-document");
				this.RestoreDocument.WriteContentTo(writer);
				writer.WriteEndElement();
				writer.WriteEndElement();
			}

			#endregion
		}

		#endregion

		#region Private fields

		private readonly List<Desktop> _desktops = new List<Desktop>();
		private Desktop _currentDesktop = null;
		
		#endregion

		private DesktopViewSettings()
		{
		}

		#region Private Methods

		private void OnQuitting(object sender, QuittingEventArgs e)
		{
			try
			{
				SaveAllDesktops();
			}
			catch(Exception ex)
			{
				Platform.Log(LogLevel.Error, ex);
			}
		}

		private void LoadAllDesktops()
		{
			_currentDesktop = null;
			_desktops.Clear();

			XmlElement desktops = DesktopViewSettingsXml["desktops"];
			if (desktops != null)
			{
				foreach (XmlElement desktop in desktops.ChildNodes)
					_desktops.Add(Desktop.FromXmlElement(desktop));

				foreach (Desktop desktop in _desktops)
				{
					if (desktop.IsCurrent)
					{
						_currentDesktop = desktop;
						break;
					}
				}
			}
		}

		private void SaveAllDesktops()
		{
			using (MemoryStream stream = new MemoryStream(1024))
			{
				using (XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8))
				{
					writer.Formatting = Formatting.Indented;

					writer.WriteStartDocument();
					writer.WriteStartElement("desktops");

					foreach (Desktop desktop in _desktops)
						desktop.WriteToXml(writer);

					writer.WriteEndElement();
					writer.WriteEndDocument();

					writer.Flush();
					stream.Position = 0;

					XmlDocument doc = new XmlDocument();
					doc.Load(stream);
					
					writer.Close();
					stream.Close();
					
					DesktopViewSettingsXml = doc;
					Save();
				}
			}
		}

		private void Initialize()
		{
			if (_currentDesktop != null)
				return;

			Application.Quitting += new EventHandler<QuittingEventArgs>(OnQuitting);

			try
			{
				LoadAllDesktops();
			}
			catch
			{
				_desktops.Clear();
				_currentDesktop = null;
				throw;
			}
			finally
			{
				if (_currentDesktop == null)
				{
					_currentDesktop = Desktop.CreateCurrent();
					_desktops.Add(_currentDesktop);
				}
			}
		}

		#endregion

		#region Public Methods

		public void SaveDesktopWindowState(string desktopWindowName, Rectangle restoreBounds, FormWindowState restoreState)
		{
			Initialize();

			DesktopWindow window = _currentDesktop.GetDesktopWindow(desktopWindowName);
			if (window == null)
			{
				window = new DesktopWindow(desktopWindowName);
				window.Bounds = restoreBounds;
				window.State = restoreState;
				_currentDesktop.AddDesktopWindow(window);
			}

			window.Bounds = restoreBounds;
			window.State = restoreState;
		}

		public bool GetDesktopWindowState(string windowName, out Rectangle restoreBounds, out FormWindowState restoreState)
		{
			Initialize();

			restoreBounds = Rectangle.Empty;
			restoreState = FormWindowState.Maximized;

			DesktopWindow window = _currentDesktop.GetDesktopWindow(windowName);
			if (window == null || window.Bounds.IsEmpty)
				return false;

			restoreBounds = window.Bounds;
			restoreState = window.State;

			return true;
		}

		public void SaveShelfState(string desktopWindowName, string shelfName, XmlDocument restoreDocument)
		{
			if (String.IsNullOrEmpty(shelfName))
				return;

			desktopWindowName = desktopWindowName ?? "";

			Initialize();

			DesktopWindow window = _currentDesktop.GetDesktopWindow(desktopWindowName);
			if (window == null)
			{
				window = new DesktopWindow(desktopWindowName);
				_currentDesktop.AddDesktopWindow(window);
			}

			Shelf shelf = window.GetShelf(shelfName);
			if (shelf == null)
			{
				shelf = new Shelf(shelfName);
				window.AddShelf(shelf);
			}

			shelf.RestoreDocument = restoreDocument;
		}

		public bool GetShelfState(string desktopWindowName, string shelfName, out XmlDocument restoreDocument)
		{
			restoreDocument = null;

			if (String.IsNullOrEmpty(shelfName))
				return false;

			Initialize();

			DesktopWindow window = _currentDesktop.GetDesktopWindow(desktopWindowName);
			if (window == null)
				return false;

			Shelf shelf = window.GetShelf(shelfName);
			if (shelf == null)
				return false;

			restoreDocument = shelf.RestoreDocument;
			return true;
		}

		#endregion
	}
}
