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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using System.Collections;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Documents;
using System.Collections.Generic;
using System.IO;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="AttachDocumentComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class AttachDocumentComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// AttachDocumentComponent class
	/// </summary>
	[AssociateView(typeof(AttachDocumentComponentViewExtensionPoint))]
	public class AttachDocumentComponent : ApplicationComponent
	{
		private readonly AttachmentSite _site;

		public AttachDocumentComponent(AttachmentSite site)
		{
			_site = site;
		}

		public AttachedDocumentSummary Document { get; private set; }

		public override void Start()
		{
			base.Start();

			Platform.GetService<IDocumentsService>(service =>
				{
					var response = service.GetAttachedDocumentFormData(new GetAttachedDocumentFormDataRequest());
					CategoryChoices = _site == AttachmentSite.Patient
									? response.PatientAttachmentCategoryChoices
									: response.OrderAttachmentCategoryChoices;
				});
		}



		#region Presentation Model

		[ValidateNotNull]
		[ValidateRegex(@"\.pdf$|\.PDF$", Message = "MessageAttachmentMustBePdf")]
		public string FilePath { get; set; }

		public IList CategoryChoices { get; private set; }

		[ValidateNotNull]
		public EnumValueInfo Category { get; set; }


		public void BrowseFiles()
		{
			try
			{
				var args = new FileDialogCreationArgs();
				args.Filters.Add(new FileExtensionFilter("*.pdf", SR.FileExtensionPdfFilterDescription));

				var result = this.Host.DesktopWindow.ShowOpenFileDialogBox(args);
				if (result.Action == DialogBoxAction.Ok)
				{
					this.FilePath = result.FileName;
					NotifyPropertyChanged("FilePath");
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		public void Accept()
		{
			if(this.HasValidationErrors)
			{
				this.ShowValidation(true);
				return;
			}

			try
			{
				this.Document = UploadFile();
				this.Exit(ApplicationComponentExitCode.Accepted);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		public void Cancel()
		{
			this.Exit(ApplicationComponentExitCode.None);
		}

		#endregion

		private AttachedDocumentSummary UploadFile()
		{
			AttachedDocumentSummary result = null;
			var data = File.ReadAllBytes(this.FilePath);
			Platform.GetService<IDocumentsService>(service =>
			{
				var response = service.Upload(new UploadRequest("pdf", "pdf", data));
				result = response.Document;
			});
			return result;
		}
	}
}
