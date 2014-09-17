using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageServer.TestApp.PerfMon
{
	public partial class WorkQueuePerfMon : Form
	{
		public delegate void DrawSampleDelegate(WorkQueueProcessingSample sample);
		public DrawSampleDelegate _drawDelegate;
		private WorkQueueProcessingSpeedMonitor _monitor;

		public WorkQueuePerfMon()
		{
			InitializeComponent();
			_drawDelegate = DrawSample;

			_monitor = new WorkQueueProcessingSpeedMonitor();
			_monitor.SampleTaken += OnSampleTaken;
			_monitor.Start();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			_monitor.Stop();
			base.OnClosing(e);
		}

		void OnSampleTaken(object sender, SampleTakenEventArgs e)
		{
			Trace.WriteLine(string.Format("{0}: {1}", e.WorkQueueProcessingSample.Processor, e.WorkQueueProcessingSample.TotalSOProcessed));
			this.Invoke(_drawDelegate, e.WorkQueueProcessingSample);
		}

		void DrawSample(WorkQueueProcessingSample sample)
		{
			this._processingSpeedChart.Series[0].Points.AddXY(sample.Timestamp.ToOADate(), sample.Speed);
		}

		public class SampleTakenEventArgs : EventArgs
		{
			public WorkQueueProcessingSample WorkQueueProcessingSample;
		}
		
		public class WorkQueueProcessingSample
		{
			public DateTime Timestamp = DateTime.Now;
			public string Processor;
			public int TotalSOProcessed;
			public double Speed;
		}


		public class WorkQueueProcessingSpeedMonitor
		{
			private const int WorkQueueTypeStudyProcess = 100;
			
			public event EventHandler<SampleTakenEventArgs> SampleTaken;
			readonly CancellationTokenSource _cancelSource = new CancellationTokenSource();
			private ImageServerDBDataContext _db ;
			private readonly Dictionary<string, List<WorkQueueProcessingSample>> _samples = new Dictionary<string, List<WorkQueueProcessingSample>>();

			public void Start()
			{
				Task.Factory.StartNew(() =>
				{
					_cancelSource.Token.ThrowIfCancellationRequested();
					
					while (true)
					{
						if (_cancelSource.Token.IsCancellationRequested)
						{
							// Clean up here, then...
							_cancelSource.Token.ThrowIfCancellationRequested();
						} 
						
						SampleNow();
						Thread.Sleep(5000);
					}

				}, _cancelSource.Token);
			}

			public void Stop()
			{
				_cancelSource.Cancel();
			}

			private void SampleNow()
			{
				using (var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions
					{
						IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
					}))
				{
					_db = new ImageServerDBDataContext();
					foreach (var processorId in GetActiveProcessorIds())
					{
						var studyStorages = GetStudiesProcessed(processorId);
						var sopCount = Enumerable.Sum(studyStorages, guid => GetStudySopCount(guid));
						
						var sample = new WorkQueueProcessingSample()
						{
							Processor=processorId,
							TotalSOProcessed = sopCount
						};

						var lastSample = GetPreviousSample(processorId);
						sample.Speed = lastSample == null
							? 0
							: (sample.TotalSOProcessed - lastSample.TotalSOProcessed)/(sample.Timestamp - lastSample.Timestamp).TotalSeconds;

						AddSample(sample);
					}

					scope.Dispose();
				}

			}

			private WorkQueueProcessingSample GetPreviousSample(string processor)
			{
				lock (_samples)
				{
					List<WorkQueueProcessingSample> list;
					if (!_samples.TryGetValue(processor, out list))
					{
						return null;
					}

					return list.Last();
				}
			}

			private void AddSample(WorkQueueProcessingSample sample)
			{
				lock (_samples)
				{
					List<WorkQueueProcessingSample> list;
					if (!_samples.TryGetValue(sample.Processor, out list))
					{
						list = new List<WorkQueueProcessingSample>();
						_samples.Add(sample.Processor, list);
					}

					list.Add(sample);
				}

				EventsHelper.Fire(SampleTaken, this, new SampleTakenEventArgs() { WorkQueueProcessingSample = sample });
			}

			private List<string> GetActiveProcessorIds()
			{
				var list=_db.WorkQueues.Select(x => x.ProcessorID).Distinct().Where(x => !string.IsNullOrEmpty(x));
				return list.ToList();
			}

			private List<Guid> GetStudiesProcessed(string processorId)
			{
				return
					_db.WorkQueues.Where(x => x.ProcessorID == processorId && x.WorkQueueTypeEnum==WorkQueueTypeStudyProcess).Select(x => x.StudyStorageGUID).ToList();
			}
			

			private int GetStudySopCount(Guid studyStorageGuid)
			{
				var study = _db.Studies.SingleOrDefault(x => x.StudyStorageGUID == studyStorageGuid);
				return study==null? 0:study.NumberOfStudyRelatedInstances;
			}
		}

	}
}
