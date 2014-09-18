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

using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.Workflow
{
	/// <summary>
	/// Extension point for views onto <see cref="MppsDocumentationComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class MppsDocumentationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// MppsDocumentationComponent class
	/// </summary>
	[AssociateView(typeof(MppsDocumentationComponentViewExtensionPoint))]
	public class MppsDocumentationComponent : ApplicationComponent, IPerformedStepEditorPage
	{
		private readonly IPerformedStepEditorContext _context;
		private ICannedTextLookupHandler _cannedTextLookupHandler;

		public MppsDocumentationComponent(IPerformedStepEditorContext context)
		{
			_context = context;
		}

		public override void Start()
		{
			_cannedTextLookupHandler = new CannedTextLookupHandler(this.Host.DesktopWindow);

			// when the selected step changes, refresh the browser
			_context.SelectedPerformedStepChanged += delegate
			{
				NotifyPropertyChanged("Comments");
				NotifyPropertyChanged("CommentsEnabled");
			};

			base.Start();
		}

		#region Presentation Model

		public string CommentsLabel
		{
			get
			{
				return _context.SelectedPerformedStep == null ? ""
				: string.Format(SR.FormatCommentsFor,
					string.Join("/", _context.SelectedPerformedStep.ModalityProcedureSteps.Select(mps => mps.Description).ToArray()));
			}
		}

		public bool CommentsEnabled
		{
			get { return _context.SelectedPerformedStep != null; }
		}

		public string Comments
		{
			get
			{
				string comments;
				return _context.SelectedPerformedStepExtendedProperties != null
					   && _context.SelectedPerformedStepExtendedProperties.TryGetValue("Comments", out comments)
						? comments
						: null;
			}
			set
			{
				if (_context.SelectedPerformedStepExtendedProperties != null)
				{
					_context.SelectedPerformedStepExtendedProperties["Comments"] = value;
				}
			}
		}

		public ICannedTextLookupHandler CannedTextLookupHandler
		{
			get { return _cannedTextLookupHandler; }
		}

		#endregion

		#region IPerformedStepEditorPage Members

		Path IExtensionPage.Path
		{
			get { return new Path("TitleMppsDocumentation", new ResourceResolver(this.GetType().Assembly)); }
		}

		IApplicationComponent IExtensionPage.GetComponent()
		{
			return this;
		}

		void IPerformedStepEditorPage.Save()
		{
		}

		#endregion
	}
}
