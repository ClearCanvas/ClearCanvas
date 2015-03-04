using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scu;

namespace ClearCanvas.ImageServer.TestApp.PerfCFind
{
	public partial class CFindPerformanceTestForm : Form
	{
		private IList<DicomAttributeCollection> _results;
		private int _startTime;

		public CFindPerformanceTestForm()
		{
			InitializeComponent();
		}

		private void _queryButton_Click(object sender, EventArgs e)
		{
			var scu = new StudyRootFindScu();
			var query = new DicomAttributeCollection();
			scu.AssociationRejected += scu_AssociationRejected;
			scu.AssociationAccepted += scu_AssociationAccepted;
			scu.AssociationReleased += scu_AssociationReleased;
			scu.AssociationAborted += scu_AssociationAborted;
			scu.NetworkError += scu_NetworkError;

            query[DicomTags.QueryRetrieveLevel].SetStringValue("STUDY");
            query[DicomTags.PatientId].SetStringValue("");
			query[DicomTags.PatientsName].SetStringValue("");
			query[DicomTags.AccessionNumber].SetStringValue("");
			query[DicomTags.StudyDescription].SetStringValue("");
			query[DicomTags.StudyDate].SetStringValue("");
			query[DicomTags.StudyInstanceUid].SetStringValue("");

			if (_studyDateTextBox.Text != "")
			{
				query[DicomTags.StudyDate].SetStringValue(_studyDateTextBox.Text +"-" + DateTime.Today.ToString("yyyyMMdd"));
			}
			
			_results = null;
			_results = scu.Find(_aeTitleTextbox.Text, _calledAETitleTextbox.Text, _hostnameTextbox.Text, int.Parse(_portTextbox.Text), query);

			scu.Dispose();

		}

		private void scu_NetworkError(object sender, AssociationParameters e)
		{
			MessageBox.Show("A network error has occurred");
		}

		void scu_AssociationAborted(object sender, Dicom.Network.AssociationParameters e)
		{
			MessageBox.Show("Association was aborted");
		}

		void scu_AssociationRejected(object sender, Dicom.Network.AssociationParameters e)
		{
			MessageBox.Show("Association was rejected");
		}

		void scu_AssociationAccepted(object sender, Dicom.Network.AssociationParameters e)
		{
			_startTime = Environment.TickCount;
		}

		void scu_AssociationReleased(object sender, EventArgs e)
		{
			var elapsed = TimeSpan.FromMilliseconds(Environment.TickCount - _startTime);
			MessageBox.Show(string.Format("{0} results in {1} ms", _results.Count(), elapsed.TotalMilliseconds));
			_queryResultGridView.DataSource = _results.Where(x=>x!=null).Select(x=>new StudyRootQueryResultEntry(x)).ToList();
		}
	}

	class StudyRootQueryResultEntry
	{
		private DicomAttributeCollection _attributeCollection;

		public StudyRootQueryResultEntry(DicomAttributeCollection attributeCollection)
		{
			this._attributeCollection = attributeCollection;
		}

		public string PatientID
		{
			get { return this._attributeCollection[DicomTags.PatientId].ToString(); }
		}
		public string PatientsName
		{
			get { return this._attributeCollection[DicomTags.PatientsName].ToString(); }
		}
		public string AccessionNumber
		{
			get { return this._attributeCollection[DicomTags.AccessionNumber].ToString(); }
		}
		public string StudyDescription
		{
			get { return this._attributeCollection[DicomTags.StudyDescription].ToString(); }
		}
		public string StudyDate
		{
			get { return this._attributeCollection[DicomTags.StudyDate].ToString(); }
		}
	}
}
