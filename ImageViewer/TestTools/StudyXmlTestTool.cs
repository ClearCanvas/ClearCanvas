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
using System.IO;
using System.Xml;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Explorer.Dicom;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageViewer.StudyManagement;
using System.Text;

namespace ClearCanvas.ImageViewer.TestTools
{
	[MenuAction("create", "dicomstudybrowser-contextmenu/Create Study Xml", "Create")]

	[ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
	public class StudyXmlTestTool : StudyBrowserTool
	{
		private string _lastFolder = null;

		public StudyXmlTestTool()
		{
		}

		protected override void OnSelectedStudyChanged(object sender, EventArgs e)
		{
			base.Enabled = true;
		}

		protected override void OnSelectedServerChanged(object sender, EventArgs e)
		{
			base.Enabled = true;
		}

		public void Create()
		{
			
			SelectFolderDialogCreationArgs args = new SelectFolderDialogCreationArgs();
			args.Path = _lastFolder ?? Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

			FileDialogResult result = base.Context.DesktopWindow.ShowSelectFolderDialogBox(args);
			if (result.Action == DialogBoxAction.Ok)
			{
				_lastFolder = result.FileName;

				StudyLoaderExtensionPoint xp = new StudyLoaderExtensionPoint();
				IStudyLoader loader = (IStudyLoader)CollectionUtils.SelectFirst(xp.CreateExtensions(),
					delegate(object extension) { return ((IStudyLoader) extension).Name == "DICOM_LOCAL";});

				var selected = base.Context.SelectedStudy;

				loader.Start(new StudyLoaderArgs(selected.StudyInstanceUid, selected.Server, null));
				StudyXml xml = new StudyXml();
				Sop sop;
				
				while (null != (sop = loader.LoadNextSop()))
				{
					xml.AddFile(((ILocalSopDataSource) sop.DataSource).File);
				}

				StudyXmlOutputSettings settings = new StudyXmlOutputSettings();
				settings.IncludePrivateValues = StudyXmlTagInclusion.IgnoreTag;
				settings.IncludeUnknownTags = StudyXmlTagInclusion.IgnoreTag;
				settings.MaxTagLength = 100 * 1024;
				settings.IncludeSourceFileName = true;

				var memento = xml.GetMemento(settings);
				string fileName = System.IO.Path.Combine(result.FileName, "studyxml.xml");

				TextWriter writer = new StreamWriter(fileName, false, Encoding.UTF8);
				memento.Save(writer);
			}
		}
	}
}
