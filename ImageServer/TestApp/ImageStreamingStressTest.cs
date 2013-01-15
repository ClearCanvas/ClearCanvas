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
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ClearCanvas.Dicom.ServiceModel.Streaming;

namespace ClearCanvas.ImageServer.TestApp
{
    public partial class ImageStreamingStressTest : Form
    {
        public ImageStreamingStressTest()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for(int i=0;i<100;i++)
            {
                Thread t = new Thread(delegate(object parm)
                                      {
                                          StreamingClient client =
                                              new StreamingClient(new Uri("http://localhost:1000/wado"));
                                          do
                                          {
                                              Stream image = client.RetrieveImage("ImageServer",
                                                                                  "1.2.840.113619.2.5.1762583153.215519.978957063.78",
                                                                                  "1.2.840.113619.2.5.1762583153.215519.978957063.79",
                                                                                  "1.2.840.113619.2.5.1762583153.215519.978957063.89"
                                                  );
                                          } while (true);
                                      });

                t.Start();
            
            }
            
                            
        }
    }
}