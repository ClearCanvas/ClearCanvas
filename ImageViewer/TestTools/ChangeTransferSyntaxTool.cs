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

#if DEBUG

using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.Explorer.Dicom;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.StudyManagement;
using Path=System.IO.Path;
using ClearCanvas.Dicom.Codec;

namespace ClearCanvas.ImageViewer.TestTools
{
	//Note: this class was basically copied from the anonymization tool and is *not* the correct
	//way to do this.  It's just a quick and dirty tool for testing purposes.
	//It should really be integrated into the viewer services.  Actually, so should anonymization!

	[ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
	public class ChangeTransferSyntaxTool : StudyBrowserTool
	{
		private string _tempPath;
		private TransferSyntax _syntax;

		public override IActionSet Actions
		{
			get
			{
				List<IAction> actions = new List<IAction>();
				IResourceResolver resolver = new ResourceResolver(typeof(ChangeTransferSyntaxTool).GetType().Assembly);

				actions.Add(CreateAction(TransferSyntax.ExplicitVrLittleEndian, resolver));
				actions.Add(CreateAction(TransferSyntax.ImplicitVrLittleEndian, resolver));

				foreach (IDicomCodecFactory factory in ClearCanvas.Dicom.Codec.DicomCodecRegistry.GetCodecFactories())
				{
					actions.Add(CreateAction(factory.CodecTransferSyntax, resolver));
				}

				return new ActionSet(actions);
			}
		}

		private IAction CreateAction(TransferSyntax syntax, IResourceResolver resolver)
		{
		    var action = new ClickAction(syntax.UidString,
		                                 new ActionPath("dicomstudybrowser-contextmenu/Change Transfer Syntax/" + syntax.ToString(), resolver),
		                                 ClickActionFlags.None, resolver) {Enabled = Enabled};

		    this.EnabledChanged += (sender, args) => action.Enabled = Enabled;

            action.SetClickHandler(() => ChangeToSyntax(syntax));
			action.Label = syntax.ToString();
			return action;
		}

		public void ChangeToSyntax(TransferSyntax syntax)
		{
			_syntax = syntax;

			BackgroundTask task = null;
			try
			{
				task = new BackgroundTask(ChangeToSyntax, false, Context.SelectedStudy);
				ProgressDialog.Show(task, this.Context.DesktopWindow, true);
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				string message = String.Format("An error occurred while compressing; folder must be deleted manually: {0}", _tempPath);
				this.Context.DesktopWindow.ShowMessageBox(message, MessageBoxActions.Ok);
			}
			finally
			{
				_tempPath = null;

				if (task != null)
					task.Dispose();
			}
		}

		private void ChangeToSyntax(IBackgroundTaskContext context)
		{
			var study = (StudyTableItem)context.UserState;

			try
			{
				_tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "ClearCanvas");
				_tempPath = System.IO.Path.Combine(_tempPath, "Compression");
				_tempPath = System.IO.Path.Combine(_tempPath, Path.GetRandomFileName());

				string message = String.Format("Changing transfer syntax to: {0}", _syntax);
				context.ReportProgress(new BackgroundTaskProgress(0, message));
			    var loader = study.Server.GetService<IStudyLoader>();
				int numberOfSops = loader.Start(new StudyLoaderArgs(study.StudyInstanceUid, null, null));
				if (numberOfSops <= 0)
					return;

				for (int i = 0; i < numberOfSops; ++i)
				{
                    Sop sop = loader.LoadNextSop();
					if (sop != null)
					{
						if (sop.DataSource is ILocalSopDataSource)
						{
							string filename = Path.Combine(_tempPath, string.Format("{0}.dcm", i));
							DicomFile file = ((ILocalSopDataSource)sop.DataSource).File;
							file.ChangeTransferSyntax(_syntax);
							file.Save(filename);
						}
					}

					int progressPercent = (int)Math.Floor((i + 1) / (float)numberOfSops * 100);
					context.ReportProgress(new BackgroundTaskProgress(progressPercent, message));
				}

				//trigger an import of the anonymized files.
			    var client = new DicomFileImportBridge();
                client.ImportFileList(new List<string> {_tempPath}, BadFileBehaviourEnum.Move, FileImportBehaviourEnum.Move);

                context.Complete();
			}
			catch(Exception e)
			{
				context.Error(e);
			}
		}

		private void UpdateEnabled()
		{
			if (this.Context.SelectedStudy == null)
			{
				this.Enabled = false;
				return;
			}

		    this.Enabled = this.Context.SelectedStudies.Count == 1 &&
		                   Context.SelectedServers.IsLocalServer &&
		                   WorkItemActivityMonitor.IsRunning;
		}

		protected override void OnSelectedStudyChanged(object sender, EventArgs e)
		{
			UpdateEnabled();
		}

		protected override void OnSelectedServerChanged(object sender, EventArgs e)
		{
			UpdateEnabled();
		}
	}
}

#endif