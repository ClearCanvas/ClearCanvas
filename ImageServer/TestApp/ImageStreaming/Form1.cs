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
using System.Net;
using System.Text;
using System.Web;
using System.Windows.Forms;
using ClearCanvas.Common.Statistics;

namespace ImageStreaming
{
    /// <summary>
    /// Types of data to be retrieved from the streaming server.
    /// </summary>
    public enum ContentTypes
    {
        /// <summary>
        /// Auto-detected by the server according to the protocol and the data
        /// </summary>
        NotSpecified,

        /// <summary>
        /// Indicates the client is expecting data to be in Dicom format.
        /// </summary>
        Dicom,

        /// <summary>
        /// Indicates the client is expecting data to be raw pixel data.
        /// </summary>
        RawPixel
    }

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Array types = Enum.GetValues(typeof (ContentTypes));
            foreach(ContentTypes type in types)
            {
                ContentTypes.Items.Add(type);
            }

            ContentTypes.SelectedIndex = 0;
        }

        private void Retrieve_Click(object sender, EventArgs e)
        {
            
            try
            {
                Uri baseUri = new Uri(String.Format("{0}/{1}", BaseUri.Text, ServerAE.Text));

                StringBuilder url = new StringBuilder();
                url.AppendFormat("{0}?requesttype=WADO&studyUID={1}&seriesUID={2}&objectUID={3}", baseUri, StudyUid.Text, SeriesUid.Text, ObjectUid.Text);

                if (UseFrame.Checked)
                {
                    url.AppendFormat("&frameNumber={0}", int.Parse(Frame.Text));
                }

                ContentTypes type = (ContentTypes)ContentTypes.SelectedItem;

                switch(type)
                {
                    case ImageStreaming.ContentTypes.Dicom:
                        url.AppendFormat("&ContentType={0}", "application/dicom");
                        break;
                    case ImageStreaming.ContentTypes.RawPixel:
                        url.AppendFormat("&ContentType={0}", "application/clearcanvas");
                        break;
                    case ImageStreaming.ContentTypes.NotSpecified:
                        break;

                }

                RateStatistics speed = new RateStatistics("Speed", RateType.BYTES);
                speed.Start();

                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url.ToString());
                request.Accept = "application/dicom,application/clearcanvas";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(String.Format("Server responded with an error: {0}", HttpUtility.HtmlDecode(response.StatusDescription)));
                }


                byte[] buffer = new byte[response.ContentLength];
                Stream stream = response.GetResponseStream();

                int offset = 0;
                do
                {
                    int readSize = stream.Read(buffer, offset, buffer.Length - offset);
                    if (readSize <= 0)
                        break;
                    offset += readSize;

                } while (true);


                stream.Close();

                speed.SetData(buffer.Length);
                speed.End();

                String msg =
                    String.Format("Mime:\t{0}\nSize:\t{1}\nSpeed:\t{2}", response.ContentType,ByteCountFormatter.Format((ulong)buffer.Length),speed.FormattedValue);
                MessageBox.Show(msg);
                
            }
            catch(WebException ex)
            {
                HttpWebResponse rsp = (ex.Response as HttpWebResponse);
                if (rsp != null)
                {
                    string msg = String.Format("Error: {0}\n{1}",
                                               rsp.StatusCode,
                                               HttpUtility.HtmlDecode(rsp.StatusDescription)
                        );
                    MessageBox.Show(msg);
                }
                
            }
        

           
        }

        private void Browse_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog()==DialogResult.OK)
            {
                FileInfo file = new FileInfo(openFileDialog1.FileName);
                
                ObjectUid.Text = file.Name.Replace(".dcm", "");

                SeriesUid.Text = file.Directory.Name;
                StudyUid.Text = file.Directory.Parent.Name;

               
            }
        }

        
        private void RetrieveFrame_CheckedChanged(object sender, EventArgs e)
        {
            Frame.Enabled = UseFrame.Checked;
        }

       
    }
}