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
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom.Utilities.StudyBuilder;

namespace ClearCanvas.ImageViewer.Utilities.StudyComposer
{
	[ExtensionPoint]
	public class StudyComposerItemEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (StudyComposerItemEditorComponentViewExtensionPoint))]
	public class StudyComposerItemEditorComponent : ApplicationComponent
	{
		private readonly IStudyComposerItem _item;

		public StudyComposerItemEditorComponent(IStudyComposerItem item)
		{
			_item = item;
		}

		public string Name
		{
			get { return _item.Name; }
			set { _item.Name = value; }
		}

		public string Description
		{
			get { return _item.Description; }
		}

		public Image Icon
		{
			get { return _item.Icon; }
		}

		public StudyBuilderNode Node
		{
			get { return _item.Node; }
		}

		public void Ok()
		{
			Apply();
			base.Exit(ApplicationComponentExitCode.Accepted);
		}

		public void Cancel()
		{
			base.Exit(ApplicationComponentExitCode.None);
		}

		public void Apply()
		{
		}
	}
}