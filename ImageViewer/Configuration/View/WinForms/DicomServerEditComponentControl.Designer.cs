#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

namespace ClearCanvas.ImageViewer.Configuration.View.WinForms
{
    partial class DicomServerEditComponentControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DicomServerEditComponentControl));
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this._serverName = new System.Windows.Forms.TextBox();
			this._ae = new System.Windows.Forms.TextBox();
			this._host = new System.Windows.Forms.TextBox();
			this._port = new System.Windows.Forms.TextBox();
			this._location = new System.Windows.Forms.TextBox();
			this._btnAccept = new System.Windows.Forms.Button();
			this._btnCancel = new System.Windows.Forms.Button();
			this._isStreaming = new System.Windows.Forms.CheckBox();
			this._dicom = new System.Windows.Forms.GroupBox();
			this._general = new System.Windows.Forms.GroupBox();
			this._streaming = new System.Windows.Forms.GroupBox();
			this.label6 = new System.Windows.Forms.Label();
			this._headerServicePort = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this._wadoServicePort = new System.Windows.Forms.TextBox();
			this._serverRoles = new System.Windows.Forms.GroupBox();
			this._isPriorsServer = new System.Windows.Forms.CheckBox();
			this._dicom.SuspendLayout();
			this._general.SuspendLayout();
			this._streaming.SuspendLayout();
			this._serverRoles.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			// 
			// label5
			// 
			resources.ApplyResources(this.label5, "label5");
			this.label5.Name = "label5";
			// 
			// _serverName
			// 
			resources.ApplyResources(this._serverName, "_serverName");
			this._serverName.Name = "_serverName";
			// 
			// _ae
			// 
			resources.ApplyResources(this._ae, "_ae");
			this._ae.Name = "_ae";
			// 
			// _host
			// 
			resources.ApplyResources(this._host, "_host");
			this._host.Name = "_host";
			// 
			// _port
			// 
			resources.ApplyResources(this._port, "_port");
			this._port.Name = "_port";
			// 
			// _location
			// 
			resources.ApplyResources(this._location, "_location");
			this._location.Name = "_location";
			// 
			// _btnAccept
			// 
			resources.ApplyResources(this._btnAccept, "_btnAccept");
			this._btnAccept.Name = "_btnAccept";
			this._btnAccept.UseVisualStyleBackColor = true;
			// 
			// _btnCancel
			// 
			resources.ApplyResources(this._btnCancel, "_btnCancel");
			this._btnCancel.Name = "_btnCancel";
			this._btnCancel.UseVisualStyleBackColor = true;
			// 
			// _isStreaming
			// 
			resources.ApplyResources(this._isStreaming, "_isStreaming");
			this._isStreaming.Name = "_isStreaming";
			this._isStreaming.UseVisualStyleBackColor = true;
			// 
			// _dicom
			// 
			this._dicom.Controls.Add(this.label2);
			this._dicom.Controls.Add(this._ae);
			this._dicom.Controls.Add(this.label4);
			this._dicom.Controls.Add(this._port);
			resources.ApplyResources(this._dicom, "_dicom");
			this._dicom.Name = "_dicom";
			this._dicom.TabStop = false;
			// 
			// _general
			// 
			this._general.Controls.Add(this.label1);
			this._general.Controls.Add(this._serverName);
			this._general.Controls.Add(this.label3);
			this._general.Controls.Add(this._host);
			this._general.Controls.Add(this.label5);
			this._general.Controls.Add(this._location);
			resources.ApplyResources(this._general, "_general");
			this._general.Name = "_general";
			this._general.TabStop = false;
			// 
			// _streaming
			// 
			this._streaming.Controls.Add(this._isStreaming);
			this._streaming.Controls.Add(this.label6);
			this._streaming.Controls.Add(this._headerServicePort);
			this._streaming.Controls.Add(this.label7);
			this._streaming.Controls.Add(this._wadoServicePort);
			resources.ApplyResources(this._streaming, "_streaming");
			this._streaming.Name = "_streaming";
			this._streaming.TabStop = false;
			// 
			// label6
			// 
			resources.ApplyResources(this.label6, "label6");
			this.label6.Name = "label6";
			// 
			// _headerServicePort
			// 
			resources.ApplyResources(this._headerServicePort, "_headerServicePort");
			this._headerServicePort.Name = "_headerServicePort";
			// 
			// label7
			// 
			resources.ApplyResources(this.label7, "label7");
			this.label7.Name = "label7";
			// 
			// _wadoServicePort
			// 
			resources.ApplyResources(this._wadoServicePort, "_wadoServicePort");
			this._wadoServicePort.Name = "_wadoServicePort";
			// 
			// _serverRoles
			// 
			this._serverRoles.Controls.Add(this._isPriorsServer);
			resources.ApplyResources(this._serverRoles, "_serverRoles");
			this._serverRoles.Name = "_serverRoles";
			this._serverRoles.TabStop = false;
			// 
			// _isPriorsServer
			// 
			resources.ApplyResources(this._isPriorsServer, "_isPriorsServer");
			this._isPriorsServer.Name = "_isPriorsServer";
			this._isPriorsServer.UseVisualStyleBackColor = true;
			// 
			// DicomServerEditComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._serverRoles);
			this.Controls.Add(this._btnCancel);
			this.Controls.Add(this._btnAccept);
			this.Controls.Add(this._general);
			this.Controls.Add(this._dicom);
			this.Controls.Add(this._streaming);
			this.Name = "DicomServerEditComponentControl";
			this._dicom.ResumeLayout(false);
			this._dicom.PerformLayout();
			this._general.ResumeLayout(false);
			this._general.PerformLayout();
			this._streaming.ResumeLayout(false);
			this._streaming.PerformLayout();
			this._serverRoles.ResumeLayout(false);
			this._serverRoles.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox _serverName;
        private System.Windows.Forms.TextBox _ae;
        private System.Windows.Forms.TextBox _host;
        private System.Windows.Forms.TextBox _port;
        private System.Windows.Forms.TextBox _location;
        private System.Windows.Forms.Button _btnAccept;
		private System.Windows.Forms.Button _btnCancel;
		private System.Windows.Forms.CheckBox _isStreaming;
		private System.Windows.Forms.GroupBox _dicom;
		private System.Windows.Forms.GroupBox _general;
		private System.Windows.Forms.GroupBox _streaming;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox _wadoServicePort;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox _headerServicePort;
		private System.Windows.Forms.GroupBox _serverRoles;
		private System.Windows.Forms.CheckBox _isPriorsServer;
    }
}
