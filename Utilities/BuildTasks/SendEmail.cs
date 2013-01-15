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
using System.Text;
using System.Net.Mail;
using Microsoft.Build.Framework;
using System.Net;

namespace ClearCanvas.Utilities.BuildTasks
{
    public class SendEmail : Microsoft.Build.Utilities.Task
    {
        //Sends out an email
        //Was using Mail task from the MsBuild Community Tasks but that task does not allow us to specify an SMTP Port

        #region properties

        [Required]
        public string SmtpServer
        {
            get { return _smtpServer; }
            set { _smtpServer = value; }
        }

        public string SmtpUsername
        {
            get { return _smtpUsername; }
            set { _smtpUsername = value; }
        }

        public string SmtpPassword
        {
            get { return _smtpPassword; }
            set { _smtpPassword = value; }
        }

        public string SmtpPort
        {
            get { return _smtpPort; }
            set { _smtpPort = value; }
        }

        [Required]
        public string ToAddress
        {
            get { return _toAddress; }
            set { _toAddress = value; }
        }

        [Required]
        public string FromAddress
        {
            get { return _fromAddress; }
            set { _fromAddress = value; }
        }

        public string Subject
        {
            get { return _subject; }
            set { _subject = value; }
        }

        [Required]
        public string Body
        {
            get { return _body; }
            set { _body = value; }
        }

        public string Attachments
        {
            get { return _attachments; }
            set { _attachments = value; }
        }
        #endregion

        public override bool Execute()
        {
            MailMessage mail = new MailMessage();

            string[] toAddresses = _toAddress.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string address in toAddresses)
            {
                mail.To.Add(new MailAddress(address));
            }

            mail.From = new MailAddress(_fromAddress);
            mail.Subject = _subject;
            mail.Body = _body;

            if (_attachments != null && _attachments.Length > 0)
            {
                string[] attachments = _attachments.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (string attachment in attachments)
                {
                    mail.Attachments.Add(new Attachment(attachment));
                }
            }

            try
            {
                SmtpClient client = new SmtpClient();
                client.Host = _smtpServer;
                if (!string.IsNullOrEmpty(_smtpUsername))
                    client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                else
                    client.UseDefaultCredentials = true;

                client.Port = Int32.Parse(_smtpPort);

                client.Send(mail);
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
            finally
            {
                if (mail == null)
                    mail.Dispose();
            }

            return true;
        }
  
        private string _smtpServer;
        private string _smtpUsername;
        private string _smtpPassword;
        private string _smtpPort;
        private string _toAddress;
        private string _fromAddress;
        private string _subject;
        private string _body;
        private string _attachments;


    }
}
