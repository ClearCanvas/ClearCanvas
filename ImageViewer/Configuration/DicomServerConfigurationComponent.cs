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
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.ImageViewer.Common.DicomServer;

namespace ClearCanvas.ImageViewer.Configuration
{
	/// <summary>
    /// Extension point for views onto <see cref="DicomServerConfigurationComponent"/>
    /// </summary>
    [ExtensionPoint]
	public sealed class DicomServerConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    //TODO (Marmot):Move to ImageViewer?

    /// <summary>
    /// DicomServerConfigurationComponent class
    /// </summary>
    [AssociateView(typeof(DicomServerConfigurationComponentViewExtensionPoint))]
    public class DicomServerConfigurationComponent : ConfigurationApplicationComponent
    {
        private string _hostName;
        private string _aeTitle;
        private int _port;

    	public override void Start()
        {
			base.Start();

    	    _hostName = DicomServer.HostName;
            _aeTitle = DicomServer.AETitle;
            _port = DicomServer.Port;
		}

    	public override void Save()
    	{
    		try
    		{
    		    DicomServer.UpdateConfiguration(new DicomServerConfiguration
    		                                        {
    		                                            HostName = _hostName,
    		                                            AETitle = AETitle,
    		                                            Port = Port
    		                                        });
    		}
    		catch (Exception e)
    		{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
    		}
    	}

    	public override bool HasValidationErrors
		{
			get
			{
				AETitle = AETitle.Trim();
				return base.HasValidationErrors;
			}
		}

        #region Presentation Model

		[ValidateLength(1, 16, Message = "ValidationAETitleLengthIncorrect")]
		[ValidateRegex(@"[\r\n\e\f\\]+", SuccessOnMatch = false, Message = "ValidationAETitleInvalidCharacters")]
		public string AETitle
        {
            get { return _aeTitle; }
            set 
            {
				if (_aeTitle == value)
					return;

				_aeTitle = value ?? "";
                base.Modified = true;
				NotifyPropertyChanged("AETitle");
            }
        }

		[ValidateGreaterThan(0, Inclusive = false, Message = "ValidationPortOutOfRange")]
		[ValidateLessThan(65536, Inclusive = false, Message = "ValidationPortOutOfRange")]
		public int Port
        {
            get { return _port; }
            set 
            {
				if (_port == value)
					return;

                _port = value;
				base.Modified = true;
				NotifyPropertyChanged("Port");
			}
        }
		
		#endregion
    }
}
