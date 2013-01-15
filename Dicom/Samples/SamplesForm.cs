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
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Codec;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.Dicom.Utilities.Xml;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using Timer = ClearCanvas.Common.Utilities.Timer;

namespace ClearCanvas.Dicom.Samples
{
    public partial class SamplesForm : Form
    {
        #region Private Constants
        private const string STR_Cancel = "Cancel";
        private const string STR_Verify = "Verify";
        #endregion

        private StorageScu _storageScu;
        private readonly VerificationScu _verificationScu = new VerificationScu();
		private DicomdirReader _reader = new DicomdirReader("DICOMDIR_READER");
        private readonly List<StorageInstance> _storageList = new List<StorageInstance>();
        private readonly Timer _timer;
        private readonly MemoryAppender _appender = new MemoryAppender();
        private FindScuBase _findScu;
        private MoveScuBase _moveScu;
        private EditSop _editSop;

        public SamplesForm()
        {
			InitializeComponent();
            _buttonStorageScuVerify.Text = STR_Verify;
         
            if (String.IsNullOrEmpty(Properties.Settings.Default.ScpStorageFolder))
            {
                Properties.Settings.Default.ScpStorageFolder = Path.Combine(Path.GetTempPath(), "DicomImages");
            }

			_destinationSyntaxCombo.Items.Clear();
			_destinationSyntaxCombo.Items.Add(TransferSyntax.ExplicitVrLittleEndian);
			foreach (TransferSyntax syntax in DicomCodecRegistry.GetCodecTransferSyntaxes())
				_destinationSyntaxCombo.Items.Add(syntax);

        	ComboBoxQueryScuQueryTypeSelectedIndexChanged(null, null);
            ComboBoxMoveScuQueryTypeSelectedIndexChanged(null, null);

            // Logging stuff
            Closing += SamplesFormClosing;
            BasicConfigurator.Configure(_appender);
            _timer = new Timer(delegate
            {
                try
                {
                    LoggingEvent[] events = _appender.GetEvents();
                    if (events != null && events.Length > 0)
                    {
                        // if there are events, we clear them from the logger,  
                        // since we're done with them  
                        _appender.Clear();
                        foreach (LoggingEvent ev in events)
                        {
                            // the line we want to log  
                            string line = String.Format("({0}) {1} {2} [{3}]: {4}\r\n",
                                                        ev.ThreadName,
                                                        ev.TimeStamp.ToShortDateString(),
                                                        ev.TimeStamp.ToLongTimeString(), ev.Level,
                                                        ev.RenderedMessage);

                            AppendText(line);

                            if (ev.ExceptionObject != null)
                            {
                                AppendText(string.Format("{0}: {1}\r\n", ev.ExceptionObject, ev.ExceptionObject.Message));
                                AppendText("Stack Trace:\r\n" + ev.ExceptionObject.StackTrace + "\r\n");
                            }
                        }
                    }
                }
                catch (Exception x)
                {
                    Platform.Log(LogLevel.Error,x,"Unexpected exception with logging event");
                }
            }, null, 500);
            _timer.Start();
        }

        private void SamplesFormClosing(object sender, CancelEventArgs e)
        {
            // Gotta stop our logging thread  
            _timer.Stop();

            if (StorageScp.Started)
            {
                StorageScp.StopListening(int.Parse(_textBoxStorageScpPort.Text));
            }
        }
        private void AppendText(string text)
        {
            Invoke(new Action<string>(logLine => OutputTextBox.AppendText(logLine)),
                                     new object[] { text });
        }

        #region Button Click Handlers
        private void ButtonStorageScuSelectFilesClick(object sender, EventArgs e)
        {
			openFileDialogStorageScu.Multiselect = true;

			openFileDialogStorageScu.Filter = "DICOM files|*.dcm|All files|*.*";
			if (DialogResult.OK == openFileDialogStorageScu.ShowDialog())
			{
				foreach (String file in openFileDialogStorageScu.FileNames)
				{
					if (file != null)
						try
						{
						    _storageList.Add(new StorageInstance(file));
						}
						catch (FileNotFoundException ex)
						{
							Platform.Log(LogLevel.Error, ex, "Unexpectedly cannot find file: {0}", file);
						}
				}
			}
        }

        private void InstanceSent(IAsyncResult ar)
        {
        }

        private void ButtonStorageScuConnectClick(object sender, EventArgs e)
        {
            int port;
            if (!int.TryParse(_textBoxStorageScuRemotePort.Text,out port))
            {
                Platform.Log(LogLevel.Error, "Unable to parse port number: {0}", _textBoxStorageScuRemotePort.Text);
                return;
            }

            _storageScu = new StorageScu(_textBoxStorageScuLocalAe.Text,
                                                _textBoxStorageScuRemoteAe.Text,
                                                _textBoxStorageScuRemoteHost.Text, port);

            _storageScu.AddStorageInstanceList(_storageList);
            _storageScu.BeginSend(InstanceSent, _storageScu);
        }

        private void ButtonStorageScpStartStopClick(object sender, EventArgs e)
        {
            if (StorageScp.Started)
            {
                _buttonStorageScpStartStop.Text = "Start";
                StorageScp.StopListening(int.Parse(_textBoxStorageScpPort.Text));
            }
            else
            {
                _buttonStorageScpStartStop.Text = "Stop";

                StorageScp.StorageLocation = _textBoxStorageScpStorageLocation.Text;
                StorageScp.Bitbucket = _checkBoxStorageScpBitbucket.Checked;
                StorageScp.List = _checkBoxStorageScpList.Checked;

                StorageScp.J2KLossless = _checkBoxStorageScpJ2KLossless.Checked;
                StorageScp.J2KLossy = _checkBoxStorageScpJ2KLossy.Checked;
                StorageScp.JpegLossless = _checkBoxStorageScpJpegLossless.Checked;
                StorageScp.JpegLossy = _checkBoxStorageScpJpegLossy.Checked;
                StorageScp.Rle = _checkBoxStorageScpRLE.Checked;

                StorageScp.StartListening(_textBoxStorageScpAeTitle.Text,
                    int.Parse(_textBoxStorageScpPort.Text));

            }

        }

        private void LoadDirectory(DirectoryInfo dir)
        {
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files)
            {
                _storageList.Add(new StorageInstance(file.FullName));
            }

            String[] subdirectories = Directory.GetDirectories(dir.FullName);
            foreach (String subPath in subdirectories)
            {
                var subDir = new DirectoryInfo(subPath);
                LoadDirectory(subDir);
            }
        }

        private void buttonStorageScuSelectDirectory_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK != folderBrowserDialogStorageScu.ShowDialog()) return;

            if (folderBrowserDialogStorageScu.SelectedPath == null)
                return;
			
            LoadDirectory(new DirectoryInfo(folderBrowserDialogStorageScu.SelectedPath));
        }

        private void _buttonStorageScuSelectStorageLocation_Click(object sender, EventArgs e)
        {
            folderBrowserDialogStorageScp.ShowDialog();

            _textBoxStorageScpStorageLocation.Text = folderBrowserDialogStorageScp.SelectedPath;
        }

        private void _buttonStorageScuVerify_Click(object sender, EventArgs e)
        {
            if (_buttonStorageScuVerify.Text == STR_Verify)
                StartVerify();
            else
                CancelVerify();
        }

        private void _buttonOutputClearLog_Click(object sender, EventArgs e)
        {
            OutputTextBox.Text = string.Empty;
        }

        private void _buttonStorageScuClearFiles_Click(object sender, EventArgs e)
        {
            _storageList.Clear();
        }
        #endregion

        private void StartVerify()
        {
            int port;
            if (!int.TryParse(_textBoxStorageScuRemotePort.Text, out port))
            {
                Platform.Log(LogLevel.Error, "Unable to parse port number: {0}", _textBoxStorageScuRemotePort.Text);
                return;
            }
            _verificationScu.BeginVerify(_textBoxStorageScuLocalAe.Text, _textBoxStorageScuRemoteAe.Text, _textBoxStorageScuRemoteHost.Text, port, new AsyncCallback(VerifyComplete), null);
            SetVerifyButton(true);
        }

        private void VerifyComplete(IAsyncResult ar)
        {
            VerificationResult verificationResult = _verificationScu.EndVerify(ar);
            Platform.Log(LogLevel.Info, "Verify result: " + verificationResult);
            SetVerifyButton(false);
        }

        private void SetVerifyButton(bool running)
        {
            if (!InvokeRequired)
            {
                _buttonStorageScuVerify.Text = running ? STR_Cancel : STR_Verify;
            }
            else
            {
                BeginInvoke(new Action<bool>(SetVerifyButton), new object[] { running });
            }
        }

        private void CancelVerify()
        {
            _verificationScu.Cancel();
        }

    	private Compression _compression;

		private void OpenFileButtonClick(object sender, EventArgs e)
		{
			openFileDialogStorageScu.Multiselect = false;
			openFileDialogStorageScu.Filter = "DICOM files|*.dcm|All files|*.*";
			if (DialogResult.OK == openFileDialogStorageScu.ShowDialog())
			{
				_sourcePathTextBox.Text = openFileDialogStorageScu.FileName;
				_destinationPathTextBox.Text = string.Empty;

				_compression = new Compression(openFileDialogStorageScu.FileName);
				_compression.Load();

				_sourceTransferSyntaxCombo.Items.Clear();
				_sourceTransferSyntaxCombo.Items.Add(_compression.DicomFile.TransferSyntax);
				_sourceTransferSyntaxCombo.SelectedItem = _compression.DicomFile.TransferSyntax;
			}
		}

		private void SaveFileButtonClick(object sender, EventArgs e)
		{
			if (_compression != null)
			{
				var destinationSyntax = _destinationSyntaxCombo.SelectedItem as TransferSyntax;

				string dump = _compression.DicomFile.Dump();
				Platform.Log(LogLevel.Info, dump);

				_compression.ChangeSyntax(destinationSyntax);

				dump = _compression.DicomFile.Dump();
				Platform.Log(LogLevel.Info, dump);

				saveFileDialog.Filter = "DICOM|*.dcm";
				if (DialogResult.OK == saveFileDialog.ShowDialog())
				{
					_destinationPathTextBox.Text = saveFileDialog.FileName;
					_compression.Save(saveFileDialog.FileName);
				}
			}
		}

		private void SavePixelsButtonClick(object sender, EventArgs e)
		{
			if (_compression != null)
			{
				if (!_compression.DicomFile.TransferSyntax.Encapsulated)
					saveFileDialog.Filter = "RAW|*.raw";
				else if (_compression.DicomFile.TransferSyntax.Equals(TransferSyntax.Jpeg2000ImageCompression)
					|| _compression.DicomFile.TransferSyntax.Equals(TransferSyntax.Jpeg2000ImageCompressionLosslessOnly))
					saveFileDialog.Filter = "JPEG 2000|*.j2k";
				else if (_compression.DicomFile.TransferSyntax.Equals(TransferSyntax.RleLossless))
					saveFileDialog.Filter = "RLE|*.rle";
				else
					saveFileDialog.Filter = "JPEG|*.jpg";

				if (DialogResult.OK == saveFileDialog.ShowDialog())
					_compression.SavePixels(saveFileDialog.FileName);
			}
		}

		private void ComboBoxQueryScuQueryTypeSelectedIndexChanged(object sender, EventArgs e)
		{
			var doc = new XmlDocument();

			if (comboBoxQueryScuQueryType.SelectedIndex == 0)
			{
                Stream stream = GetType().Assembly.GetManifestResourceStream("ClearCanvas.Dicom.Samples.SampleXml.StudyRootStudy.xml");
				if (stream != null)
				{
					doc.Load(stream);
					stream.Close();
				}
				comboBoxQueryScuQueryLevel.Items.Clear();
				comboBoxQueryScuQueryLevel.Items.Add("STUDY");
				comboBoxQueryScuQueryLevel.Items.Add("SERIES");
				comboBoxQueryScuQueryLevel.Items.Add("IMAGE");
			}
			else
			{
                var stream = GetType().Assembly.GetManifestResourceStream("ClearCanvas.Dicom.Samples.SampleXml.PatientRootPatient.xml");
				if (stream != null)
				{
					doc.Load(stream);
					stream.Close();
				}
				comboBoxQueryScuQueryLevel.Items.Clear();
				comboBoxQueryScuQueryLevel.Items.Add("PATIENT");
				comboBoxQueryScuQueryLevel.Items.Add("STUDY");
				comboBoxQueryScuQueryLevel.Items.Add("SERIES");
				comboBoxQueryScuQueryLevel.Items.Add("IMAGE");
			}

			var sw = new StringWriter();

		    var xmlSettings = new XmlWriterSettings
		                          {
		                              Encoding = Encoding.UTF8,
		                              ConformanceLevel = ConformanceLevel.Fragment,
		                              Indent = true,
		                              NewLineOnAttributes = false,
		                              CheckCharacters = true,
		                              IndentChars = "  "
		                          };


		    XmlWriter tw = XmlWriter.Create(sw, xmlSettings);
			if (tw != null)
			{
				doc.WriteTo(tw);
				tw.Close();
			}
			textBoxQueryMessage.Text = sw.ToString();
		}

		private void buttonQueryScuSearch_Click(object sender, EventArgs e)
		{
            if (_findScu != null)
            {
                if (_findScu.Status == ScuOperationStatus.Running)
                {
                    return;
                }
                _findScu.Dispose();
                _findScu = null;
            }
		    var theDoc = new XmlDocument();

			try
			{
				theDoc.LoadXml(textBoxQueryMessage.Text);
				var instanceXml = new InstanceXml(theDoc.DocumentElement, null);
				DicomAttributeCollection queryMessage = instanceXml.Collection;

				if (queryMessage == null)
				{
					Platform.Log(LogLevel.Error, "Unexpected error parsing query message");
				}

				int maxResults;
				if (!int.TryParse(textBoxQueryScuMaxResults.Text, out maxResults))
					maxResults = -1;

				IList<DicomAttributeCollection> resultsList;
				if (comboBoxQueryScuQueryType.SelectedIndex == 0)
				{
				    _findScu = new StudyRootFindScu
				                      {
				                          MaxResults = maxResults
				                      };
                    _findScu.BeginFind(textBoxQueryScuLocalAe.Text,
								 textBoxQueryScuRemoteAe.Text,
								 textBoxQueryScuRemoteHost.Text,
								 int.Parse(textBoxQueryScuRemotePort.Text), queryMessage, delegate
								                                                             {
                                                                                                 foreach (DicomAttributeCollection msg in _findScu.Results)
                                                                                                 {
                                                                                                     Platform.Log(LogLevel.Info, msg.DumpString);
                                                                                                 }
								                                                             },this);
				}
				else
				{
                    _findScu = new PatientRootFindScu
				                      {
				                          MaxResults = maxResults
				                      };
                    _findScu.BeginFind(textBoxQueryScuLocalAe.Text,
								 textBoxQueryScuRemoteAe.Text,
								 textBoxQueryScuRemoteHost.Text,
								 int.Parse(textBoxQueryScuRemotePort.Text), queryMessage, delegate
								                                                             {
                                                                                                 foreach (DicomAttributeCollection msg in _findScu.Results)
                                                                                                 {
                                                                                                     Platform.Log(LogLevel.Info, msg.DumpString);
                                                                                                 }
								                                                             },this);
              
				}

			
			}
			catch (Exception x)
			{
				Platform.Log(LogLevel.Error, x, "Unable to perform query");
			}		
		}

		private void ComboBoxQueryScuQueryLevelSelectedIndexChanged(object sender, EventArgs e)
		{
			var doc = new XmlDocument();

			string xmlFile;
			if (comboBoxQueryScuQueryType.SelectedIndex == 0)
			{
				if (comboBoxQueryScuQueryLevel.SelectedItem.Equals("STUDY"))
                    xmlFile = "ClearCanvas.Dicom.Samples.SampleXml.StudyRootStudy.xml";
				else if (comboBoxQueryScuQueryLevel.SelectedItem.Equals("SERIES"))
                    xmlFile = "ClearCanvas.Dicom.Samples.SampleXml.StudyRootSeries.xml";
				else
                    xmlFile = "ClearCanvas.Dicom.Samples.SampleXml.StudyRootImage.xml";
			}
			else
			{
				if (comboBoxQueryScuQueryLevel.SelectedItem.Equals("PATIENT"))
                    xmlFile = "ClearCanvas.Dicom.Samples.SampleXml.PatientRootPatient.xml";
				else if (comboBoxQueryScuQueryLevel.SelectedItem.Equals("STUDY"))
                    xmlFile = "ClearCanvas.Dicom.Samples.SampleXml.PatientRootStudy.xml";
				else if (comboBoxQueryScuQueryLevel.SelectedItem.Equals("SERIES"))
                    xmlFile = "ClearCanvas.Dicom.Samples.SampleXml.PatientRootSeries.xml";
				else
                    xmlFile = "ClearCanvas.Dicom.Samples.SampleXml.PatientRootImage.xml";
			}

			Stream stream = GetType().Assembly.GetManifestResourceStream(xmlFile);
			if (stream != null)
			{
				doc.Load(stream);
				stream.Close();
			}

			var sw = new StringWriter();

		    var xmlSettings = new XmlWriterSettings
		                          {
		                              Encoding = Encoding.UTF8,
		                              ConformanceLevel = ConformanceLevel.Fragment,
		                              Indent = true,
		                              NewLineOnAttributes = false,
		                              CheckCharacters = true,
		                              IndentChars = "  "
		                          };


		    XmlWriter tw = XmlWriter.Create(sw, xmlSettings);

			if (tw != null)
			{
				doc.WriteTo(tw); 
				tw.Close();
			}

			textBoxQueryMessage.Text = sw.ToString();
		}

		private void ButtonOpenDicomdirClick(object sender, EventArgs e)
		{
			openFileDialogStorageScu.Filter = "DICOMDIR|*.*|DICOM files|*.dcm";
			openFileDialogStorageScu.FileName = String.Empty;
			openFileDialogStorageScu.Multiselect = false;
			if (DialogResult.OK == openFileDialogStorageScu.ShowDialog())
			{
				_textBoxDicomdir.Text = openFileDialogStorageScu.FileName;

				_reader = new DicomdirReader("DICOMDIR_READER");
				_reader.Load(openFileDialogStorageScu.FileName);

				var display = new DicomdirDisplay();
				display.Add(_reader.Dicomdir);

				display.Show(this);
			}
		}

		private void ButtonSendDicomdirClick(object sender, EventArgs e)
		{
			string rootDirectory = Path.GetDirectoryName(_textBoxDicomdir.Text);

			_reader.Send(rootDirectory, _textBoxDicomdirRemoteAe.Text, _textBoxDicomdirRemoteHost.Text, int.Parse(_textBoxDicomdirRemotePort.Text));
		}

        private void buttonMoveScuMove_Click(object sender, EventArgs e)
        {
            if (_moveScu != null)
            {
                if (_moveScu.Status == ScuOperationStatus.Running)
                {
                    _moveScu.Cancel();
                    return;
                }
                buttonMoveScuMove.Text = "Move";
                _moveScu.Dispose();
                _moveScu = null;
            }

            var theDoc = new XmlDocument();

            buttonMoveScuMove.Text = "Cancel";

            try
            {
                theDoc.LoadXml(textBoxMoveMessage.Text);
                var instanceXml = new InstanceXml(theDoc.DocumentElement, null);
                DicomAttributeCollection queryMessage = instanceXml.Collection;

                if (queryMessage == null)
                {
                    Platform.Log(LogLevel.Error, "Unexpected error parsing move message");
                    return;
                }

                

                if (comboBoxMoveScuQueryType.SelectedIndex == 0)
                {
                    _moveScu = new StudyRootMoveScu(textBoxMoveScuLocalAe.Text,textBoxMoveScuRemoteAe.Text,textBoxMoveScuRemoteHost.Text,
                        int.Parse(textBoxMoveScuRemotePort.Text),textBoxMoveScuMoveDestination.Text);
                }
                else
                {
                    _moveScu = new PatientRootMoveScu(textBoxMoveScuLocalAe.Text, textBoxMoveScuRemoteAe.Text,
                                                         textBoxMoveScuRemoteHost.Text,
                                                         int.Parse(textBoxMoveScuRemotePort.Text),
                                                         textBoxMoveScuMoveDestination.Text);
                }
                if (queryMessage.Contains(DicomTags.PatientId))
                {
                    var array = queryMessage[DicomTags.PatientId].Values as string[];
                    if (array != null)
                        foreach(string s in array)
                            _moveScu.AddPatientId(s);
                }
                if (queryMessage.Contains(DicomTags.StudyInstanceUid))
                {
                    var array = queryMessage[DicomTags.StudyInstanceUid].Values as string[];
                    if (array != null)
                        foreach (string s in array)
                            _moveScu.AddStudyInstanceUid(s);
                }
                if (queryMessage.Contains(DicomTags.SeriesInstanceUid))
                {
                    var array = queryMessage[DicomTags.SeriesInstanceUid].Values as string[];
                    if (array != null)
                        foreach (string s in array)
                            _moveScu.AddSeriesInstanceUid(s);
                }
                if (queryMessage.Contains(DicomTags.SopInstanceUid))
                {
                    var array = queryMessage[DicomTags.SopInstanceUid].Values as string[];
                    if (array != null)
                        foreach (string s in array)
                            _moveScu.AddSopInstanceUid(s);
                }

                _moveScu.ImageMoveCompleted += delegate(object o, EventArgs args)
                                                   {
                                                       var eventScu = o as MoveScuBase;
                                                       if (eventScu != null)
                                                       {
                                                           Platform.Log(LogLevel.Info,
                                                                        "Total SubOps: {0}, Remaining SubOps {1}, Success SubOps: {2}, Failure SubOps: {3}, Warning SubOps: {4}, Failure Description: {5}",
                                                                        eventScu.TotalSubOperations,
                                                                        eventScu.RemainingSubOperations,
                                                                        eventScu.SuccessSubOperations,
                                                                        eventScu.FailureSubOperations,
                                                                        eventScu.WarningSubOperations,
                                                                        eventScu.FailureDescription);
                                                       }
                                                   };
                _moveScu.BeginMove(delegate {
                                           Invoke(new Action<string>(delegate { buttonMoveScuMove.Text = "Move"; }), new object[]{"Move"});                                           
                                       }, this );

            }
            catch (Exception x)
            {
                Platform.Log(LogLevel.Error, x, "Unable to perform move");
                buttonMoveScuMove.Text = "Move";
            }		
        }

        private void ComboBoxMoveScuQueryTypeSelectedIndexChanged(object sender, EventArgs e)
        {
            var doc = new XmlDocument();

            if (comboBoxMoveScuQueryType.SelectedIndex == 0)
            {
                Stream stream = GetType().Assembly.GetManifestResourceStream("ClearCanvas.Dicom.Samples.SampleXml.StudyRootMoveStudy.xml");
                if (stream != null)
                {
                    doc.Load(stream);
                    stream.Close();
                }
                comboBoxMoveScuQueryLevel.Items.Clear();
                comboBoxMoveScuQueryLevel.Items.Add("STUDY");
                comboBoxMoveScuQueryLevel.Items.Add("SERIES");
                comboBoxMoveScuQueryLevel.Items.Add("IMAGE");
            }
            else
            {
                var stream = GetType().Assembly.GetManifestResourceStream("ClearCanvas.Dicom.Samples.SampleXml.PatientRootMovePatient.xml");
                if (stream != null)
                {
                    doc.Load(stream);
                    stream.Close();
                }
                comboBoxMoveScuQueryLevel.Items.Clear();
                comboBoxMoveScuQueryLevel.Items.Add("PATIENT");
                comboBoxMoveScuQueryLevel.Items.Add("STUDY");
                comboBoxMoveScuQueryLevel.Items.Add("SERIES");
                comboBoxMoveScuQueryLevel.Items.Add("IMAGE");
            }

            var sw = new StringWriter();

            var xmlSettings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                ConformanceLevel = ConformanceLevel.Fragment,
                Indent = true,
                NewLineOnAttributes = false,
                CheckCharacters = true,
                IndentChars = "  "
            };


            XmlWriter tw = XmlWriter.Create(sw, xmlSettings);
            if (tw != null)
            {
                doc.WriteTo(tw);
                tw.Close();
            }
            textBoxMoveMessage.Text = sw.ToString();
        }

        private void ComboBoxMoveScuQueryLevelSelectedIndexChanged(object sender, EventArgs e)
        {
            var doc = new XmlDocument();

            string xmlFile;
            if (comboBoxMoveScuQueryType.SelectedIndex == 0)
            {
                if (comboBoxMoveScuQueryLevel.SelectedItem.Equals("STUDY"))
                    xmlFile = "ClearCanvas.Dicom.Samples.SampleXml.StudyRootMoveStudy.xml";
                else if (comboBoxMoveScuQueryLevel.SelectedItem.Equals("SERIES"))
                    xmlFile = "ClearCanvas.Dicom.Samples.SampleXml.StudyRootMoveSeries.xml";
                else
                    xmlFile = "ClearCanvas.Dicom.Samples.SampleXml.StudyRootMoveImage.xml";
            }
            else
            {
                if (comboBoxMoveScuQueryLevel.SelectedItem.Equals("PATIENT"))
                    xmlFile = "ClearCanvas.Dicom.Samples.SampleXml.PatientRootMovePatient.xml";
                else if (comboBoxMoveScuQueryLevel.SelectedItem.Equals("STUDY"))
                    xmlFile = "ClearCanvas.Dicom.Samples.SampleXml.PatientRootMoveStudy.xml";
                else if (comboBoxMoveScuQueryLevel.SelectedItem.Equals("SERIES"))
                    xmlFile = "ClearCanvas.Dicom.Samples.SampleXml.PatientRootMoveSeries.xml";
                else
                    xmlFile = "ClearCanvas.Dicom.Samples.SampleXml.PatientRootMoveImage.xml";
            }

            Stream stream = GetType().Assembly.GetManifestResourceStream(xmlFile);
            if (stream != null)
            {
                doc.Load(stream);
                stream.Close();
            }

            var sw = new StringWriter();

            var xmlSettings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                ConformanceLevel = ConformanceLevel.Fragment,
                Indent = true,
                NewLineOnAttributes = false,
                CheckCharacters = true,
                IndentChars = "  "
            };


            XmlWriter tw = XmlWriter.Create(sw, xmlSettings);

            if (tw != null)
            {
                doc.WriteTo(tw);
                tw.Close();
            }

            textBoxMoveMessage.Text = sw.ToString();
		
        }

        private void _editSopSaveFileButton_Click(object sender, EventArgs e)
        {
            if (_editSop != null)
            {
                _editSop.UpdateTags(_editSopTextBox.Text);

                saveFileDialog.Filter = "DICOM|*.dcm";
                if (DialogResult.OK == saveFileDialog.ShowDialog())
                {
                    _destinationPathTextBox.Text = saveFileDialog.FileName;
                    _editSop.Save(saveFileDialog.FileName);
                }
            }
        }

        private void _editSopOpenFileButton_Click(object sender, EventArgs e)
        {
            openFileDialogStorageScu.Multiselect = false;
            openFileDialogStorageScu.Filter = "DICOM files|*.dcm|All files|*.*";
            if (DialogResult.OK == openFileDialogStorageScu.ShowDialog())
            {
                _editSopSourcePathTextBox.Text = openFileDialogStorageScu.FileName;
                _editSopDestinationPathTextBox.Text = string.Empty;

                _editSop = new EditSop(openFileDialogStorageScu.FileName);
                _editSop.Load();

                _editSopTextBox.Text = _editSop.GetXmlRepresentation();

                string dump = _editSop.DicomFile.Dump();
                Platform.Log(LogLevel.Info, dump);
            }
        }
    }
}