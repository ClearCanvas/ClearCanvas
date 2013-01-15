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
using System.Security.Cryptography.X509Certificates;

namespace ClearCanvas.Enterprise.Common.ServiceConfiguration.Server
{
	/// <summary>
	/// Specifies how to find the certificate to host the service
	/// </summary>
	public class CertificateSearchDirective
	{
		/// <summary>
		/// Creates a basic certificate search directive based on the host name, useful for self-signed certificates.
		/// </summary>
		/// <param name="hostUri"></param>
		/// <returns></returns>
		public static CertificateSearchDirective CreateBasic(Uri hostUri)
		{
			return new CertificateSearchDirective
					{
						FindType = X509FindType.FindBySubjectDistinguishedName,
						FindValue = string.Format("CN={0}", hostUri.Host)
					};
		}

		/// <summary>
		/// Creates a custom certificate search directive, based on the specified find type and value.
		/// </summary>
		/// <param name="findType"></param>
		/// <param name="findValue"></param>
		/// <returns></returns>
		public static CertificateSearchDirective CreateCustom(X509FindType findType, string findValue)
		{
			return new CertificateSearchDirective
			{
				FindType = findType,
				FindValue = findValue
			};
		}


		private X509FindType _findType;

		private CertificateSearchDirective()
		{
			// set defaults consistent with behaviour prior to #8219
			this.StoreLocation = StoreLocation.LocalMachine;
			this.StoreName = StoreName.My;
			this.FindType = X509FindType.FindBySubjectName;
		}

		/// <summary>
		/// Gets or sets the store location.
		/// </summary>
		public StoreLocation StoreLocation { get; set; }

		/// <summary>
		/// Gets or sets the store name.
		/// </summary>
		public StoreName StoreName { get; set; }

		/// <summary>
		/// Gets the find type.
		/// </summary>
		public X509FindType FindType
		{
			get { return _findType; }
			set
			{
				if (value == X509FindType.FindByTimeExpired ||
					value == X509FindType.FindByTimeNotYetValid ||
					value == X509FindType.FindByTimeValid)
				{
					// for these, FindValue must be a DateTime value
					throw new NotSupportedException(string.Format("{0} is not supported", value));
				}
				_findType = value;
			}
		}

		/// <summary>
		/// Gets or sets the find value.
		/// </summary>
		public string FindValue { get; set; }
	}
}
