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
using System.IO;
using System.Windows.Forms;
using System.Xml;
using ClearCanvas.Dicom.Utilities.Xml;

namespace ClearCanvas.Dicom.XmlGenerator
{
    public partial class MainDialog : Form
    {
        StudyXml _theStream = new StudyXml();

        public MainDialog()
        {
            InitializeComponent();
        }


        private void ButtonLoadFile_Click(object sender, EventArgs e)
        {
            openFileDialog.DefaultExt = "dcm";
            openFileDialog.ShowDialog();

            DicomFile dicomFile = new DicomFile(openFileDialog.FileName);

            dicomFile.Load(DicomReadOptions.Default);

            _theStream.AddFile(dicomFile);
        }

        private void _buttonLoadDirectory_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.ShowDialog();

            String directory = folderBrowserDialog.SelectedPath;
			if (directory.Equals(String.Empty))
				return;

            DirectoryInfo dir = new DirectoryInfo(directory);

            LoadFiles(dir);
			

        }


        private void LoadFiles(DirectoryInfo dir)
        {
         
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files)
            {

                DicomFile dicomFile = new DicomFile(file.FullName);

                try
                {

                    DicomReadOptions options = new DicomReadOptions();


                    dicomFile.Load(options);
                    _theStream.AddFile(dicomFile);
                    /*
					if (true == dicomFile.Load())
					{
						_theStream.AddFile(dicomFile);
					}
                     * */
                }
                catch (DicomException) 
                {
                    // TODO:  Add some logging for failed files
                }

            }

            String[] subdirectories = Directory.GetDirectories(dir.FullName);
            foreach (String subPath in subdirectories)
            {
                DirectoryInfo subDir = new DirectoryInfo(subPath);
                LoadFiles(subDir);
                continue;
            }

        }

        private void _buttonGenerateXml_Click(object sender, EventArgs e)
        {
            saveFileDialog.DefaultExt = "xml";
            saveFileDialog.ShowDialog();

			StudyXmlOutputSettings settings = new StudyXmlOutputSettings();
        	settings.IncludeSourceFileName = false;
            XmlDocument doc = _theStream.GetMemento(settings);

            Stream fileStream = saveFileDialog.OpenFile();

            StudyXmlIo.Write(doc, fileStream);

            fileStream.Close();
        }

        private void _buttonLoadXml_Click(object sender, EventArgs e)
        {
            openFileDialog.DefaultExt = "xml";
            openFileDialog.ShowDialog();

            Stream fileStream = openFileDialog.OpenFile();

            XmlDocument theDoc = new XmlDocument();

            StudyXmlIo.Read(theDoc, fileStream);

            fileStream.Close();

            _theStream = new StudyXml();

            _theStream.SetMemento(theDoc);
        }

        private void _buttonGenerateGzipXml_Click(object sender, EventArgs e)
        {
            saveFileDialog.DefaultExt = "gzip";

            saveFileDialog.ShowDialog();

            XmlDocument doc = _theStream.GetMemento(StudyXmlOutputSettings.None);

            Stream fileStream = saveFileDialog.OpenFile();

            StudyXmlIo.WriteGzip(doc, fileStream);

            fileStream.Close();
        }

        private void _buttonLoadGzipXml_Click(object sender, EventArgs e)
        {
            openFileDialog.DefaultExt = "gzip";
            openFileDialog.ShowDialog();

            Stream fileStream = openFileDialog.OpenFile();

            XmlDocument theDoc = new XmlDocument();

            StudyXmlIo.ReadGzip(theDoc, fileStream);

            fileStream.Close();

            _theStream = new StudyXml();

            _theStream.SetMemento(theDoc);
        }
    }
}