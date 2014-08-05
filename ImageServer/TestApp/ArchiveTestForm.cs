using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Xml;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Core.Command;
using ClearCanvas.ImageServer.Core.Command.Archiving;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.TestApp.Annotations;
using MessageBox = System.Windows.Forms.MessageBox;
using Size = System.Drawing.Size;

namespace ClearCanvas.ImageServer.TestApp
{
	
	public partial class ArchiveTestForm : Form
	{
		private BindingList<StudyItem> _studyList;

		public ArchiveTestForm()
		{
			InitializeComponent();

			_studyListGridView.AutoGenerateColumns = false;
			_studyListGridView.ReadOnly = true;
			_studyListGridView.AllowUserToAddRows = false;

			_studyListGridView.Columns.Add(new DataGridViewColumn() { Name = "Accession Number", HeaderText = "Accession #", DataPropertyName = "AccessionNumber", CellTemplate = new DataGridViewTextBoxCell() });
			_studyListGridView.Columns.Add(new DataGridViewColumn() { Name = "Patient ID", HeaderText = "Patient ID", DataPropertyName = "PatientId", CellTemplate = new DataGridViewTextBoxCell() });
			_studyListGridView.Columns.Add(new DataGridViewColumn() { Name = "Patient Name", HeaderText = "Patient Name", DataPropertyName = "PatientsName", CellTemplate = new DataGridViewTextBoxCell() });
			_studyListGridView.Columns.Add(new DataGridViewColumn() { Name = "StudyDescription", HeaderText = "Study Description", DataPropertyName = "StudyDescription", CellTemplate = new DataGridViewTextBoxCell() });
			_studyListGridView.Columns.Add(new DataGridViewColumn() { Name = "Modalities", HeaderText = "Modalities", DataPropertyName = "Modalities", CellTemplate = new DataGridViewTextBoxCell() });
			_studyListGridView.Columns.Add(new DataGridViewColumn() { Name = "Status", HeaderText = "Status", DataPropertyName = "Status", CellTemplate = new DataGridViewTextBoxCell() });

			_studyListGridView.Columns.Add(new DataGridViewColumn() { 
				Name = "Progress", 
				HeaderText = "Progress",
				DataPropertyName = "OperationProgress",
				DefaultCellStyle = null,
				CellTemplate = new DataGridViewProgressCell()
			});

			_studyList = new BindingList<StudyItem>();
			_studyListGridView.DataSource = _studyList;
		}

	
		private void _queryButton_Click(object sender, EventArgs e)
		{
			Reload();
		}

		private void _purgeStudyBtn_Click(object sender, EventArgs e)
		{
			for(var i=0; i<_studyListGridView.SelectedRows.Count;i++)
			{
				var row = _studyListGridView.SelectedRows[i];
				var study = row.DataBoundItem as StudyItem;

				Purge(study);
			}
		}

		private void Reload()
		{
			using (var ctx = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
			{
				var broker = ctx.GetBroker<IStudyEntityBroker>();
				var criteria = new StudySelectCriteria();
				criteria.AccessionNumber.Like("%" + _accessionTextBox.Text + "%");
				var studies = broker.Find(criteria);

				_studyList.Clear();
				foreach(var item in studies)
					_studyList.Add(new StudyItem(item));
				
			}
		}

		private void Purge(StudyItem study)
		{
			Task.Factory.StartNew(() =>
			{
				var archive = SelectPartitionArchive(study);

				if (archive == null)
				{
					MessageBox.Show("Please add an archive in the partition where this study is located");
					return;
				}

				if (study.IsArchivingScheduled())
				{
					if (
						MessageBox.Show(
							"This study is scheduled in the Archive Queue. Do you want to remove it from the queue before purging?",
							"Study is scheduled for archiving",
							MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
					{
						using (var processor = new ServerCommandProcessor("archive"))
						{
							processor.AddCommand(new DeleteAllArchiveQueueItemCommand(study.StudyStorage, archive));
							processor.Execute();
						}

					}
				}


				if (study.StudyStorageLocation.ArchiveLocations == null || !study.StudyStorageLocation.ArchiveLocations.Any())
				{
					var i = 0;
					using (var processor = new ServerCommandProcessor("archive"))
					{
						var archiveCommand = new ArchiveStudyCommand(study.StudyStorageLocation, GetArchivePath(archive), @"C:\temp", archive);
						archiveCommand.ProgressUpdated += (s, e) =>
						{

							study.OperationProgress = new OperationProgress()
							{
								Status = e.Percentage == 100 ? "Archived" : e.Status,
								Percentage = (int)e.Percentage
							};
						};
						
						processor.AddCommand(archiveCommand);

						if (!processor.Execute())
							MessageBox.Show(string.Format("Unable to archive study: {0}", processor.FailureException.Message));
					}
				}


				using (var processor = new ServerCommandProcessor("archive"))
				{
					processor.AddCommand(new PurgeStudyCommand(study.StudyStorage));
					if (!processor.Execute())
						MessageBox.Show(string.Format("Unable to purge study: {0}", processor.FailureException.Message));
					else
					{
						MessageBox.Show("Study has been succesfully purged");
						study.Status = "Nearline";
					}
				}
				
			});

			
		}

		private PartitionArchive SelectPartitionArchive(StudyItem study)
		{
			using (var ctx = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
			{
				var partitionArchiveBroker = ctx.GetBroker<IPartitionArchiveEntityBroker>();
				var partitionArchiveCriteria = new PartitionArchiveSelectCriteria();
				partitionArchiveCriteria.ServerPartitionKey.EqualTo(study.StudyStorage.ServerPartitionKey);
				return partitionArchiveBroker.Find(partitionArchiveCriteria).FirstOrDefault();

			}
		}

		
		public string GetArchivePath(PartitionArchive archive)
		{
			var element = archive.ConfigurationXml.DocumentElement;
			if (element != null)
				foreach (XmlElement node in element.ChildNodes)
					if (node.Name.Equals("RootDir"))
						return node.InnerText;

			return null;
		}

		private void _studyListGridView_SelectionChanged(object sender, EventArgs e)
		{
			
			for (var i = 0; i < _studyListGridView.SelectedRows.Count; i++)
			{
				var row = _studyListGridView.SelectedRows[i];
				var study = row.DataBoundItem as StudyItem;

				if (study!=null && (study.StudyStorageLocation==null || study.Status=="Nearline"))
				{
					_purgeStudyBtn.Enabled = false;
					return;
				}

			}

			_purgeStudyBtn.Enabled = _studyListGridView.SelectedRows.Count>0;
		}
	}

	internal class DeleteAllArchiveQueueItemCommand : ServerDatabaseCommand
	{
		private readonly PartitionArchive _archive;
		private readonly StudyStorage _studyStorage;

		public DeleteAllArchiveQueueItemCommand(StudyStorage studyStorage, PartitionArchive archive)
			:base("Delete Archive Queue items")
		{
			_archive = archive;
			_studyStorage = studyStorage;
		}

		protected override void OnExecute(CommandProcessor theProcessor, IUpdateContext updateContext)
		{
			var broker = updateContext.GetBroker<IArchiveQueueEntityBroker>();
			var criteria = new ArchiveQueueSelectCriteria();
			criteria.PartitionArchiveKey.EqualTo(_archive.GetKey());
			criteria.StudyStorageKey.EqualTo(_studyStorage.GetKey());
			broker.Delete(criteria);

		}
	}


	public class OperationProgress
	{
		public int Percentage { get; set; }
		public string Status { get; set; }
	}


	public class StudyItem:INotifyPropertyChanged
	{
		private readonly Study _study;

		public StudyStorage StudyStorage;
		public StudyStorageLocation StudyStorageLocation;
		private OperationProgress _operationProgress;
		private string _status;

		public int ProgressPercentage
		{
			get { return _operationProgress==null? 0: _operationProgress.Percentage; }
		}

		public OperationProgress OperationProgress
		{
			get { return _operationProgress; }
			set
			{
				_operationProgress = value;
				//OnPropertyChanged("ProgressPercentage");
				OnPropertyChanged("OperationProgress");
			}
		}

		public StudyItem()
		{
			
		}

		public StudyItem(Study study)
		{
			_study = study;
			StudyStorage = StudyStorage.Load(_study.StudyStorageKey);
			StudyStorageLocation = StudyStorageLocation.FindStorageLocations(StudyStorage).FirstOrDefault();
			_status = StudyStorage.StudyStatusEnum.Description;
		}

		public string AccessionNumber
		{
			get { return _study.AccessionNumber; }
		}

		public string PatientsName { get { return _study.PatientsName; } }
		public string PatientId { get { return _study.PatientId; } }
		public string StudyDate { get { return _study.StudyDate; } }
		public string StudyDescription { get { return _study.StudyDescription; } }

		public string Modalities
		{
			get { return string.Join(",", _study.Series.Values.Select(x => x.Modality).Distinct()); }
		}

		public string Status
		{
			get { return _status; }
			set
			{
				_status = value; 
				OnPropertyChanged("Status");
			}
		}

		public bool IsArchivingScheduled()
		{
			using (var ctx = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
			{
				var broker = ctx.GetBroker<IArchiveQueueEntityBroker>();
				var criteria = new ArchiveQueueSelectCriteria();
				criteria.StudyStorageKey.EqualTo(StudyStorage.GetKey());
				return broker.Find(criteria).Any();
			}
		}


		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) 
				handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	class DataGridViewProgressCell : DataGridViewImageCell
	{
		// Used to make custom cell consistent with a DataGridViewImageCell
		private readonly Bitmap _bitmap;
		private Size _dimensions;

		public DataGridViewProgressCell()
		{
			this.ValueType = typeof(Bitmap);
			_dimensions = new Size(80, 14);
			_bitmap = new Bitmap(_dimensions.Width, _dimensions.Height);

		}

		protected override object GetFormattedValue(object value,
						   int rowIndex, ref DataGridViewCellStyle cellStyle,
						   TypeConverter valueTypeConverter,
						   TypeConverter formattedValueTypeConverter,
						   DataGridViewDataErrorContexts context)
		{
			if (value == null)
				return _bitmap;


			var percent = ((OperationProgress)value).Percentage; // 1-100
			using (var g = System.Drawing.Graphics.FromImage(_bitmap))
			{
				g.FillRectangle(Brushes.White, 0, 0, _dimensions.Width - 1, _dimensions.Height - 1);
				g.DrawRectangle(Pens.DarkGray, 0, 0, _dimensions.Width - 1, _dimensions.Height - 1);
				g.FillRectangle(GetBrush(percent), 1, 1, Math.Min((int)(_dimensions.Width * percent / 100.0), _dimensions.Width - 2), _dimensions.Height - 2);
			}
			return _bitmap;
		}

		private Brush GetBrush(float percent)
		{
			//if (percent<100)
			//	return Brushes.Yellow;

			return Brushes.Green;
		}


	}
}
