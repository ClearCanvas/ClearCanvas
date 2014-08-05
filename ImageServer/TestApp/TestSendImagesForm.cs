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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageServer.TestApp
{

   
    public partial class TestSendImagesForm : Form
    {
        private Dictionary<string, string> _seriesMap;
        private Dictionary<string, List<string>> _seriesToFilesMap;
        private List<string> _lastNames = new List<string>();
        private List<string> _givenNames = new List<string>();
        private List<string> _seriesDesc = new List<string>();
        private List<DicomFile> _prevSentFiles = new List<DicomFile>();
        public TestSendImagesForm()
        {
            InitializeComponent();

        }

	    protected override void OnLoad(EventArgs e)
	    {
		    base.OnLoad(e);

			maxStudyDate.Text = DateTime.Today.ToString("yyyyMMdd");

			StreamReader re = File.OpenText("LastNames.txt");
			string input = null;
			while ((input = re.ReadLine()) != null)
			{
				_lastNames.Add(input.Trim());
			}
			re.Close();

			re = File.OpenText("GivenNames.txt");
			while ((input = re.ReadLine()) != null)
			{
				_givenNames.Add(input.Trim());
			}
			re.Close();

			re = File.OpenText("SeriesDescriptions.txt");
			while ((input = re.ReadLine()) != null)
			{
				_seriesDesc.Add(input.Trim());
			}
			re.Close();

			LoadDefaultSamples();
			InitNewPatient();

			var rand = new Random();
			InitNewStudy(new DateTime(1990, 1, 1).AddDays(rand.Next(0, 5000)));
	    }

	    private void SendRandom_Click(object sender, EventArgs e)
        {
            SendImages();
        }

        private void SendImages()
        {
            Random ran = new Random();
            textBox1.Clear();
            
                _seriesMap = new Dictionary<string, string>();
                _prevSentFiles = new List<DicomFile>();
                List<StorageScu> scuClients = new List<StorageScu>();
				StorageScu scu = new StorageScu(LocalAE.Text, ServerAE.Text, ServerHost.Text, int.Parse(ServerPort.Text));
				scu.ImageStoreCompleted += new EventHandler<ImageStoreEventArgs>(scu_ImageStoreCompleted);
				scuClients.Add(scu);

                int seriesCount = 0;
                do
                {
                    String seriesDescription = _seriesDesc[ran.Next(_seriesDesc.Count)];
                    string[] seriesUids = new string[_seriesToFilesMap.Count];
                    _seriesToFilesMap.Keys.CopyTo(seriesUids, 0);
                    String seriesToUse = seriesUids[ran.Next(_seriesToFilesMap.Count)];
                    List<string> files = _seriesToFilesMap[seriesToUse];

                
                    foreach (string path in files)
                    {
                        DicomFile file = new DicomFile(path);
                        file.Load();

                        RandomizeFile(file, seriesDescription);
                        _prevSentFiles.Add(file);


                        foreach (StorageScu client in scuClients)
                        {
                            client.AddStorageInstance(new StorageInstance(file));
                        }


                        if (ran.Next() % 50 == 0)
                            break; // don't use all images
                    }

                    seriesCount++;

                } while (ran.Next() % 5 != 0);

                Log(String.Format("Sending {0} images using {1} client(s)", _prevSentFiles.Count, scuClients.Count));
				scu.BeginSend(InstanceSent, scu);
				scu.Join();
                scu.Dispose();
                
        }

        void scu_ImageStoreCompleted(object sender, ImageStoreEventArgs e)
        {
            StorageScu scu = sender as StorageScu;
            Random rand = new Random();
            //Thread.Sleep(rand.Next(300, 1000));
            textBox1.BeginInvoke(new LogDelegate(Log),  e.StorageInstance.SopInstanceUid);
                                                     
        }

        private void InstanceSent(IAsyncResult ar)
        {
        }


        private void ResendImages()
        {
            if (_prevSentFiles != null && _prevSentFiles.Count>0)
            {
                using (StorageScu scu = new StorageScu(LocalAE.Text, ServerAE.Text, ServerHost.Text, int.Parse(ServerPort.Text)))
                {
                    foreach (DicomFile file in _prevSentFiles)
                    {
                        SetDicomFields(file);
                        scu.AddStorageInstance(new StorageInstance(file));
                    }
                    scu.ImageStoreCompleted += new EventHandler<ImageStoreEventArgs>(scu_ImageStoreCompleted);
                    scu.Send();
                    scu.Join();
                }
            }
            
        }

        private void SetDicomFields(DicomFile file)
        {

            if (!omitPatientId.Checked)
                file.DataSet[DicomTags.PatientId].SetStringValue(PatientsId.Text);
            else
                file.DataSet.RemoveAttribute(DicomTags.PatientId);

            if (!omitPatientName.Checked)
                file.DataSet[DicomTags.PatientsName].SetStringValue(PatientsName.Text);
            else
                file.DataSet.RemoveAttribute(DicomTags.PatientsName);

            if (!omitIssuerOfPatientId.Checked)
                file.DataSet[DicomTags.IssuerOfPatientId].SetStringValue(IssuerOfPatientsId.Text);
            else
                file.DataSet.RemoveAttribute(DicomTags.IssuerOfPatientId);

            if (!omitBirthdate.Checked)
                file.DataSet[DicomTags.PatientsBirthDate].SetStringValue(PatientsBirthdate.Text);
            else
                file.DataSet.RemoveAttribute(DicomTags.PatientsBirthDate);

            if (!omitGender.Checked)
                file.DataSet[DicomTags.PatientsSex].SetStringValue(PatientsSex.Text);
            else
                file.DataSet.RemoveAttribute(DicomTags.PatientsSex);

            if (!omitAccession.Checked)
                file.DataSet[DicomTags.AccessionNumber].SetStringValue(AccessionNumber.Text);
            else
                file.DataSet.RemoveAttribute(DicomTags.AccessionNumber);


            if (!omitStudyDate.Checked)
                file.DataSet[DicomTags.StudyDate].SetStringValue(StudyDate.Text);
            else
                file.DataSet.RemoveAttribute(DicomTags.StudyDate);

            file.DataSet[DicomTags.StudyInstanceUid].SetStringValue(StudyInstanceUid.Text);

        }

        private void RandomizeFile(DicomFile file, String seriesDescription)
        {
            string seriesUid = file.DataSet[DicomTags.SeriesInstanceUid].ToString();
            if (!_seriesMap.ContainsKey(seriesUid))
            {
                string newSeriesUid = DicomUid.GenerateUid().UID;
                file.DataSet[DicomTags.SeriesInstanceUid].SetStringValue(newSeriesUid);
                _seriesMap.Add(seriesUid, newSeriesUid);
            }
            else
            {
                string newSeriesUid = _seriesMap[seriesUid];
                file.DataSet[DicomTags.SeriesInstanceUid].SetStringValue(newSeriesUid);
            }
            DicomUid sopUid = DicomUid.GenerateUid();
            file.MediaStorageSopInstanceUid = sopUid.UID;
            file.DataSet[DicomTags.SopInstanceUid].SetStringValue(sopUid.UID);

            SetDicomFields(file);

        }

	    void LoadDefaultSamples()
	    {
			Log(String.Format("Loading Default Samples..."));
                
			_seriesToFilesMap = new Dictionary<string, List<string>>();

			string folder = "Samples";
			FileProcessor.Process(folder, "*.dcm",
				  delegate(string path)
				  {

					  DicomFile file = new DicomFile(path);
					  file.Load(DicomReadOptions.DoNotStorePixelDataInDataSet | DicomReadOptions.Default);

					  string seriesUid =
						  file.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
					  if (!_seriesToFilesMap.ContainsKey(seriesUid))
					  {
						  _seriesToFilesMap.Add(seriesUid, new List<String>());
					  }
            
					  _seriesToFilesMap[seriesUid].Add(path);

				  },
				  true);

		    Log(String.Format("Default Samples Loaded"));
            
			
	    }

        private void LoadSamples_Click(object sender, EventArgs e)
        {
            // Build the index
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                _seriesToFilesMap = new Dictionary<string, List<string>>();

                string folder = folderBrowserDialog1.SelectedPath;
                FileProcessor.Process(folder, "*.dcm",
                      delegate(string path)
                      {

                          DicomFile file = new DicomFile(path);
                          file.Load(DicomReadOptions.DoNotStorePixelDataInDataSet | DicomReadOptions.Default);

                          string seriesUid =
                              file.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
                          if (!_seriesToFilesMap.ContainsKey(seriesUid))
                          {
                              _seriesToFilesMap.Add(seriesUid, new List<String>());
                          }

                          _seriesToFilesMap[seriesUid].Add(path);

                      },
                      true);


                RandomPatient.Enabled = true;
                NewStudy.Enabled = true;
                GenerateImages.Enabled = true;
                AutoRun.Enabled = true;
                Resend.Enabled = true;
            }
        }

        private void RandomPatient_Click(object sender, EventArgs e)
        {
            InitNewPatient();
        }

        private void InitNewPatient()
        {
            Random ran = new Random();
            PatientsName.Text =
                String.Format("{0}^{1}", _lastNames[ran.Next(_lastNames.Count)],
                              _givenNames[ran.Next(_givenNames.Count)]);

            PatientsId.Text = String.Format("{0}{1}{2}-{3}", 
                    (char)((int)'A' +  ran.Next(26)),
                    (char)((int)'A' +  ran.Next(26)),
                    (char)((int)'A' +  ran.Next(26)),
                    ran.Next(1000, 9999));
            PatientsBirthdate.Text = ran.Next() % 10 == 0
                                         ? ""
                                         : DateParser.ToDicomString(new DateTime(1900, 1, 1).AddDays(ran.Next(0, 36000)));

        }

        private void InitNewStudy(DateTime studyDate)
        {
            Random ran = new Random();
            AccessionNumber.Text = String.Format("{0}{1}",  (char)((int)'A' +  ran.Next(26)), ran.Next(10000, 99999));
            StudyInstanceUid.Text = DicomUid.GenerateUid().UID;
	        StudyDate.Text = DateParser.ToDicomString(studyDate);

        }

        private void NewStudy_Click(object sender, EventArgs e)
        {
	        var ran = new Random();
			InitNewStudy(new DateTime(1990, 1, 1).AddDays(ran.Next(0, 5000)));
        }

        private bool _autoRunOn = false;
        private System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer();
        private Thread _sendThread;
        public delegate void LogDelegate(string message);

        private void Log(string message)
        {
            textBox1.Text +=Environment.NewLine + message; 
        }
        
        private void AutoRun_Click(object sender, EventArgs e)
        {
            _autoRunOn = !_autoRunOn;

            AutoRun.Text = _autoRunOn ? "Stop" : "Auto Run";
            RandomPatient.Enabled = !_autoRunOn;
            NewStudy.Enabled = !_autoRunOn;
            GenerateImages.Enabled = !_autoRunOn;
            //LoadSamples.Enabled = !_autoRunOn;
            Resend.Enabled = !_autoRunOn;
            
            if (_autoRunOn)
            {
                
                _sendThread = new Thread(delegate()
                                             {
                                                 do
                                                 {
                                                     Random rand = new Random();

                                                     textBox1.BeginInvoke(new LogDelegate(Log), "Sending...");
                                                     
                                                     try
                                                     {
	                                                     if (rand.Next(100)>20)
	                                                     {
		                                                     InitNewPatient();
	                                                     }

	                                                     var minDate = DateTime.ParseExact(minStudyDate.Text, "yyyyMMdd", null);
														 var maxDate = DateTime.ParseExact(maxStudyDate.Text, "yyyyMMdd", null);
														 var diff = (int)(maxDate - minDate).TotalDays;
														 InitNewStudy(minDate.AddDays(rand.Next(0, diff)));
                                                        
														 SendImages();
                                                     }
                                                     catch(Exception)
                                                     {
                                                         
                                                     }

                                                     textBox1.BeginInvoke(new LogDelegate(Log), "Paused");

                                                     Thread.Sleep(rand.Next(1000, 5000));
                                                 } while (_autoRunOn);
                                             });

                _sendThread.Start();

            }
        }

        private void Resend_Click(object sender, EventArgs e)
        {
            ResendImages();
        }

		private void checkableGroupBox1_CheckStateChanged(object sender, EventArgs e)
		{
			RandomPatient.Enabled = true;
			NewStudy.Enabled = true;
		}

		private void checkableGroupBox2_CheckStateChanged(object sender, EventArgs e)
		{

		}

    }

    
}