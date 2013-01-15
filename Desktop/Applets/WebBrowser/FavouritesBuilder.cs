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
using ClearCanvas.Desktop.Actions;
using System.IO;

namespace ClearCanvas.Desktop.Applets.WebBrowser
{
	class FavouritesBuilder
	{
		public static ActionSet Build(WebBrowserComponent webBrowser)
		{
			List<IAction> actions = new List<IAction>();

			string favouritesFolder = Environment.GetFolderPath(Environment.SpecialFolder.Favorites);
			string[] favourites = Directory.GetFiles(favouritesFolder, "*.*", SearchOption.AllDirectories);

			// The action ID is arbitrary--it's just an identifier--as long as all the 
			// action IDs of all the actions in a given ActionModel are unqiue.
			// So, we'll just generate a new action ID for each action here
			// simply by incrementing an integer.
			int actionID = 0;

			foreach (string favouritePath in favourites)
			{
				IAction action = CreateAction(
					actionID.ToString(),
					favouritePath,
					webBrowser);
				
				actions.Add(action);
				actionID++;
			}

			return new ActionSet(actions);
		}

		private static IAction CreateAction(
			string actionID,
			string path,
			WebBrowserComponent webBrowser)
		{
			string url = ExtractUrlFromFavourite(path);
			string menuPath = CreateMenuPath(path);

			// Create the menu action
			ActionPath actionPath = new ActionPath(menuPath, null);
			MenuAction action = new MenuAction(actionID, actionPath, ClickActionFlags.None, null);
			action.Label = actionPath.LastSegment.LocalizedText;

			// Set what we're supposed to do when the menu item is clicked
			action.SetClickHandler(
				delegate
				{
					// Navigate to the URL
					webBrowser.Url = url;
					webBrowser.Go();
				});

			return action;
		}

		private static string ExtractUrlFromFavourite(string path)
		{
			// Read the .url file into a single string
			string text = File.ReadAllText(path);

			if (text.Length == 0)
				return String.Empty;

			// Break up the file into separate lines
			string[] lines = text.Split('\n');

			foreach (string line in lines)
			{
				// Find the first line that contains "URL=http";
				// that should be the link we want.
				if (line.Contains("URL=http"))
				{
					// The URL is everything to the right of the =
					string[] substrings = line.Split('=');

					if (substrings.Length >= 2)
						return substrings[1];
				}
			}

			return String.Empty;
		}

		private static string CreateMenuPath(string path)
		{
			string favouritesFolder = Environment.GetFolderPath(Environment.SpecialFolder.Favorites);
			string menuPath = path.Replace(favouritesFolder, "");
			menuPath = menuPath.Replace(".url", "");
			menuPath = menuPath.Replace("\\", "/");
			// We want the menu item to be in the global menu
			menuPath = "global-menus/Favourites" + menuPath;
			return menuPath;
		}
	}
}
