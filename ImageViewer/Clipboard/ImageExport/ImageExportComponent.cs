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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Common.Auditing;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.StudyManagement;
using Path=System.IO.Path;

#pragma warning disable 0419,1574,1587,1591

namespace ClearCanvas.ImageViewer.Clipboard.ImageExport
{
	[ExtensionPoint]
	public sealed class ImageExportComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	#region ImageExporterInfo class

	/// <summary>
	/// Adapter used by the view to present data about an exporter.
	/// </summary>
	public class ImageExporterInfo
	{
		private readonly IImageExporter _sourceImageExporter;

		internal ImageExporterInfo(IImageExporter sourceImageExporter)
		{
			_sourceImageExporter = sourceImageExporter;
		}

		internal IImageExporter SourceImageExporter
		{
			get { return _sourceImageExporter; }
		}

		#region Public Properties

		public string Description
		{
			get { return _sourceImageExporter.Description; }
		}

		public string FileExtensionFilter
		{
			get
			{
				string filterPortion = StringUtilities.Combine(_sourceImageExporter.FileExtensions, ";",
													delegate(string extension)
													{
														return String.Format("*.{0}", extension);
													});

				return String.Format("{0}|{1}", _sourceImageExporter.Description, filterPortion);
			}
		}

		public string DefaultFileExtension
		{
			get { return _sourceImageExporter.FileExtensions[0]; }
		}

		public bool IsConfigurable
		{
			get { return _sourceImageExporter is IConfigurableImageExporter; }
		}

		#endregion
	}

	#endregion

	[AssociateView(typeof(ImageExportComponentViewExtensionPoint))]
	public partial class ImageExportComponent : ApplicationComponent
	{
		//can only have one of these running at a time
		private static MultipleImageExporter _multipleImageExporter;

		private List<ImageExporterInfo> _exporterInfoList;
		private volatile ImageExporterInfo _selectedExporterInfo;

		private volatile List<IClipboardItem> _itemsToExport;
		private volatile int _numberOfImagesToExport;
		private volatile string _exportFilePath;
		private volatile ExportOption _exportOption;
		private volatile float _scale = 1;

		private int _width = 512;
		private int _height = 512;
		private int _outputDpi = 96;
		private Color _backgroundColor = Color.Black;
		private SizeMode _sizeMode = SizeMode.Scale;

		private ImageExportComponent()
		{
		}

		#region Component

		private List<IClipboardItem> ItemsToExport
		{
			get { return _itemsToExport; }
			set { _itemsToExport = value; }
		}

		private IImageExporter SelectedImageExporter
		{
			get
			{
				if (_selectedExporterInfo == null)
					return null;

				return _selectedExporterInfo.SourceImageExporter;
			}
		}

		public override void Start()
		{
			InitializeExporterInfoList();
			InitializeOptions();

			base.Start();
		}
		
		#region Presentation Model

		public ICollection<ImageExporterInfo> ExporterInfoList
		{
			get { return _exporterInfoList; }
		}

		public ImageExporterInfo SelectedExporterInfo
		{
			get { return _selectedExporterInfo; }
			set
			{
				if (!_exporterInfoList.Contains(value))
					throw new ArgumentException("The specified image exporter does not exist.");

				_selectedExporterInfo = value;

				NotifyPropertyChanged("SelectedExporterInfo");
				NotifyPropertyChanged("ConfigureEnabled");
			}
		}

		public int NumberOfImagesToExport
		{
			get { return _numberOfImagesToExport; }
			private set { _numberOfImagesToExport = value; }
		}

		public string ExportFilePath
		{
			get { return _exportFilePath; }
			set { _exportFilePath = GetCorrectedExportFilePath(value); }
		}

		public ExportOption ExportOption
		{
			get { return _exportOption; }
			set
			{
				if (_exportOption != value)
				{
					_exportOption = value;
					this.NotifyPropertyChanged("ExportOption");
				}
			}
		}

		public float MinimumScale
		{
			get { return 0.1F; }
		}

		public float MaximumScale
		{
			get { return 25F; }
		}

		public float Scale
		{
			get { return _scale; }
			set
			{
				if (value == _scale)
					return;

				_scale = value;
				NotifyPropertyChanged("Scale");
			}
		}

		public int MinimumDimension
		{
			get { return 10; }
		}

		public int MaximumDimension
		{
			get { return 4096; }
		}

		public int Width
		{
			get { return _width; }
			set
			{
				if (_width != value)
				{
					_width = value;
					this.NotifyPropertyChanged("Width");
				}
			}
		}

		public int Height
		{
			get { return _height; }
			set
			{
				if (_height != value)
				{
					_height = value;
					this.NotifyPropertyChanged("Height");
				}
			}
		}

		public int OutputDpi
		{
			get { return _outputDpi; }
			set
			{
				if (_outputDpi != value)
				{
					_outputDpi = value;
					this.NotifyPropertyChanged("OutputDpi");
				}
			}
		}

		public Color BackgroundColor
		{
			get { return _backgroundColor; }
			set
			{
				if (_backgroundColor != value)
				{
					_backgroundColor = value;
					this.NotifyPropertyChanged("BackgroundColor");
				}
			}
		}

		public SizeMode SizeMode
		{
			get { return _sizeMode; }
			set
			{
				if (_sizeMode != value)
				{
					_sizeMode = value;
					this.NotifyPropertyChanged("SizeMode");
				}
			}
		}

		public bool ConfigureEnabled
		{
			get
			{
				if (_selectedExporterInfo == null)
					return false;

				return _selectedExporterInfo.IsConfigurable;
			}
		}

		public bool ConfigureVisible
		{
			get
			{
				return CollectionUtils.Contains(_exporterInfoList,
				                                delegate(ImageExporterInfo info)
				                                	{
				                                		return info.IsConfigurable;
				                                	});
			}	
		}

		public void Configure()
		{
			IConfigurableImageExporter exporter = SelectedImageExporter as IConfigurableImageExporter;
			if (exporter == null)
				return;

			try
			{
				IApplicationComponent component = exporter.GetConfigurationComponent();
				if (component == null)
					return;

				string title = String.Format("{0} ({1})", SR.ConfigureImageExport, exporter.Description);
				LaunchAsDialog(Host.DesktopWindow, component, title);
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				Host.DesktopWindow.ShowMessageBox(SR.MessageErrorLaunchingConfigurationComponent, MessageBoxActions.Ok);
			}
		}

		public void Accept()
		{
			if (HasValidationErrors)
			{
				ShowValidation(true);
			}
			else
			{
				SaveOptions();
				ExitCode = ApplicationComponentExitCode.Accepted;
				Host.Exit();
			}
		}

		public void Cancel()
		{
			ExitCode = ApplicationComponentExitCode.None;
			Host.Exit();
		}

		#endregion

		private ImageExporterInfo GetExporterInfo(string identifier)
		{
			return CollectionUtils.SelectFirst(_exporterInfoList,
		                                    delegate(ImageExporterInfo info)
		                                    	{
													return info.SourceImageExporter.Identifier == identifier;
		                                    	});
		}

		private void InitializeExporterInfoList()
		{
			_exporterInfoList = CollectionUtils.Map<object, ImageExporterInfo>(
					new ImageExporterExtensionPoint().CreateExtensions(),
					delegate(object exporterExtension)
					{
						return new ImageExporterInfo((IImageExporter)exporterExtension);
					});

			List<IImageExporter> standardExporters = StandardImageExporterFactory.CreateStandardExporters();
			foreach (IImageExporter standardExporter in standardExporters)
			{
				if (GetExporterInfo(standardExporter.Identifier) == null)
					_exporterInfoList.Add(new ImageExporterInfo(standardExporter));
			}

			SortExporterInfoList();
		}

		private void SortExporterInfoList()
		{
			_exporterInfoList.Sort(delegate(ImageExporterInfo x, ImageExporterInfo y)
						{
							return String.Compare(x.Description, y.Description);
						});
		}

		private string GetCorrectedExportFilePath(string exportFilePath)
		{
			if (NumberOfImagesToExport == 1)
			{
				exportFilePath = FileUtilities.CorrectFileNameExtension(exportFilePath, SelectedImageExporter.FileExtensions);
				string directory = Path.GetDirectoryName(exportFilePath);

				if (!String.IsNullOrEmpty(directory) && Directory.Exists(directory))
				{
					string fileName = Path.GetFileName(exportFilePath);
					if (String.IsNullOrEmpty(fileName))
					{
						throw new FileNotFoundException("The specified file path is invalid: " + exportFilePath);
					}
				}
				else
				{
					throw new FileNotFoundException("The specified file path is invalid: " + exportFilePath ?? "");
				}
			}
			else
			{
				if (!String.IsNullOrEmpty(exportFilePath) && !Directory.Exists(exportFilePath))
					throw new FileNotFoundException("The specified directory does not exist: " + exportFilePath ?? "");
			}

			return exportFilePath;
		}

		private void InitializeOptions()
		{
			ImageExportSettings settings = ImageExportSettings.Default;
			_exportOption = (ExportOption)settings.SelectedImageExportOption;

			_selectedExporterInfo = GetExporterInfo(settings.SelectedImageExporterId);
			if (_selectedExporterInfo == null)
				_selectedExporterInfo = _exporterInfoList[0];

			_sizeMode = settings.SizeMode;
			_backgroundColor = settings.BackgroundColor;
		}

		private void SaveOptions()
		{
			ImageExportSettings settings = ImageExportSettings.Default;
			settings.SelectedImageExportOption = (int)ExportOption;
			settings.SelectedImageExporterId = SelectedImageExporter.Identifier;
			settings.SizeMode = SizeMode;
			settings.BackgroundColor = BackgroundColor;
			settings.Save();
			// NOTE: ShowTextOverlay is deliberately not persisted due to inherent patient privacy risks
		}

		#endregion

		#region Launch Dialog

		internal static void Launch(IDesktopWindow desktopWindow, List<IClipboardItem> clipboardItems)
		{
			Platform.CheckForNullReference(desktopWindow, "desktopWindow");
			Platform.CheckForNullReference(clipboardItems, "clipboardItems");

			if (_multipleImageExporter != null)
			{
				desktopWindow.ShowMessageBox(SR.MessageImageExportStillRunning, MessageBoxActions.Ok);
				return;
			}

			int numberOfImagesToExport = GetNumberOfImagesToExport(clipboardItems);
			Platform.CheckPositive(numberOfImagesToExport, "numberOfImagesToExport");

			string title = SR.TitleExportImages;
			if (numberOfImagesToExport == 1)
				title = SR.TitleExportSingleImage;

			//initialize the component.
			ImageExportComponent component = new ImageExportComponent();
			component.ItemsToExport = clipboardItems;
			component.NumberOfImagesToExport = numberOfImagesToExport;

			// give the width and height values from the first image to be exported
			if (clipboardItems.Count > 0)
			{
				object item = clipboardItems[0].Item;
				if (item is IPresentationImage)
				{
					var image = (IPresentationImage)item;
					component.Height = image.SceneSize.Height;
					component.Width = image.SceneSize.Width;
				}
				else if (item is IDisplaySet)
				{
					foreach (IPresentationImage image in ((IDisplaySet) item).PresentationImages)
					{
						component.Height = image.SceneSize.Height;
						component.Width = image.SceneSize.Width;
						break;
					}
				}
			}

			if (ApplicationComponentExitCode.Accepted != LaunchAsDialog(desktopWindow, component, title))
				return;

			component.Export();
		}

		#endregion

		private static int GetNumberOfImagesToExport(IEnumerable<IClipboardItem> itemsToExport)
		{
			int number = 0;
			foreach (ClipboardItem clipboardItem in itemsToExport)
			{
				if (clipboardItem.Item is IPresentationImage)
				{
					++number;
				}
				else if (clipboardItem.Item is IDisplaySet)
				{
					number += ((IDisplaySet)clipboardItem.Item).PresentationImages.Count;
				}
			}

			return number;
		}

		private static AuditedInstances GetInstancesForAudit(IEnumerable<IClipboardItem> itemsToExport, string exportFilePath)
		{
			AuditedInstances exportedInstances = new AuditedInstances();
			foreach (ClipboardItem clipboardItem in itemsToExport)
			{
				if (clipboardItem.Item is IPresentationImage && clipboardItem.Item is IImageSopProvider)
				{
					IImageSopProvider sopProv = (IImageSopProvider) clipboardItem.Item;
					exportedInstances.AddInstance(sopProv.ImageSop.PatientId, sopProv.ImageSop.PatientsName, sopProv.ImageSop.StudyInstanceUid, exportFilePath);
				}
				else if (clipboardItem.Item is IDisplaySet)
				{
					foreach (IPresentationImage image in ((IDisplaySet) clipboardItem.Item).PresentationImages)
					{
						IImageSopProvider sopProv = image as IImageSopProvider;
						if (sopProv != null)
							exportedInstances.AddInstance(sopProv.ImageSop.PatientId, sopProv.ImageSop.PatientsName, sopProv.ImageSop.StudyInstanceUid, exportFilePath);
					}
				}
			}
			return exportedInstances;
		}

		#region Export

		private void Export()
		{
			if (SelectedImageExporter == null)
				throw new InvalidOperationException("No exporter was chosen; unable to export any images.");

			if (NumberOfImagesToExport == 1)
			{
				EventResult result = EventResult.Success;
				AuditedInstances exportedInstances = GetInstancesForAudit(ItemsToExport, this.ExportFilePath);

				try
				{
					if (!Directory.Exists(Path.GetDirectoryName(ExportFilePath ?? "")))
						throw new FileNotFoundException("The specified export file path does not exist: " + ExportFilePath ?? "");

					ClipboardItem clipboardItem = (ClipboardItem) _itemsToExport[0];

					ExportImageParams exportParams = GetExportParams(clipboardItem);
					SelectedImageExporter.Export((IPresentationImage) clipboardItem.Item, ExportFilePath, exportParams);
				}
				catch (Exception ex)
				{
					result = EventResult.SeriousFailure;
					Platform.Log(LogLevel.Error, ex);
				}
				finally
				{
					AuditHelper.LogExportStudies(exportedInstances, EventSource.CurrentUser, result);
				}
			}
			else
			{
				if (!Directory.Exists(ExportFilePath ?? ""))
					throw new FileNotFoundException("The specified export directory does not exist." + ExportFilePath ?? "");

				_multipleImageExporter = new MultipleImageExporter(this);
				_multipleImageExporter.Run();
			}
		}

		private void OnMultipleImageExportComplete(Exception error)
		{
			if (error != null)
				this.Host.DesktopWindow.ShowMessageBox(SR.MessageExportFailed, MessageBoxActions.Ok); 

			_multipleImageExporter = null;
		}

		private ExportImageParams GetExportParams(ClipboardItem clipboardItem)
		{
			ExportImageParams exportParams = new ExportImageParams();
			exportParams.ExportOption = ExportOption;
			exportParams.DisplayRectangle = clipboardItem.DisplayRectangle;
			exportParams.Scale = Scale;
			exportParams.SizeMode = SizeMode;
			exportParams.OutputSize = new Size(Width, Height);
			exportParams.BackgroundColor = BackgroundColor;
			exportParams.Dpi = OutputDpi;
			return exportParams;
		}

		#endregion
	}
}
