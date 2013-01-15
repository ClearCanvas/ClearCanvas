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
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	//NOTE: keep this internal for now, as I'm not too sure of their usefulness right now.
	internal interface IImageSetFactory
	{
		void SetStudyTree(StudyTree studyTree);

		IImageSet CreateImageSet(Study study);
	}

	internal class ImageSetFactory : IImageSetFactory
	{
		private StudyTree _studyTree;
		private readonly IDisplaySetFactory _displaySetFactory;

	    public ImageSetFactory()
			: this(new BasicDisplaySetFactory())
		{
		}

        public ImageSetFactory(IDisplaySetFactory displaySetFactory)
		{
			_displaySetFactory = displaySetFactory;
		}

		#region IImageSetFactory Members

		void IImageSetFactory.SetStudyTree(StudyTree studyTree)
		{
			_studyTree = studyTree;
			_displaySetFactory.SetStudyTree(studyTree);
		}

		IImageSet IImageSetFactory.CreateImageSet(Study study)
		{
			Platform.CheckForNullReference(study, "study");
			Platform.CheckMemberIsSet(_studyTree, "_studyTree");

			return CreateImageSet(study);
		}

		#endregion

		protected virtual IImageSet CreateImageSet(Study study)
		{
			ImageSet imageSet = null;
			List<IDisplaySet> displaySets = CreateDisplaySets(study);

			if (displaySets.Count > 0)
			{
				imageSet = new ImageSet(CreateImageSetDescriptor(study));
				
				foreach (IDisplaySet displaySet in displaySets)
					imageSet.DisplaySets.Add(displaySet);
			}

			return imageSet;
		}

		protected virtual List<IDisplaySet> CreateDisplaySets(Study study)
		{
		    return _displaySetFactory.CreateDisplaySets(study);
		}

        protected virtual DicomImageSetDescriptor CreateImageSetDescriptor(Study study)
		{
			return new DicomImageSetDescriptor(study.GetIdentifier());
		}
	}
}