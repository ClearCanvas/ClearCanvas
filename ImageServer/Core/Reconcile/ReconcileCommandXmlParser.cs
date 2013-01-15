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
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Core.Reconcile.CreateStudy;
using ClearCanvas.ImageServer.Core.Reconcile.Discard;
using ClearCanvas.ImageServer.Core.Reconcile.MergeStudy;
using ClearCanvas.ImageServer.Core.Reconcile.ProcessAsIs;

namespace ClearCanvas.ImageServer.Core.Reconcile
{
	/// <summary>
	/// Parses reconciliation commands in Xml format.
	/// </summary>
	/// <remarks>
	/// Currently only "MergeStudy", "CreateStudy" or "Discard" commands are supported.
	/// 
	/// <example>
	/// The following examples shows the xml of the "Discard" and "CreateStudy" commands
	/// <code>
	/// <Discard></Discard>
	/// </code>
	/// 
	/// <code>
	/// <CreateStudy>
	///     <Set TagPath="00100010" Value="John^Smith"/>
	/// </CreateStudy>
	/// </code>
	/// </example>
	/// </remarks>
	public class ReconcileCommandXmlParser
	{
		#region Public Methods
		/// <summary>
		/// </summary>
		/// <param name="doc"></param>
		/// <returns></returns>
		/// <remarks>
		/// The reconciliation commands should be specified in <ImageCommands> node.
		/// </remarks>
		public IReconcileProcessor Parse(XmlDocument doc)
		{
			//TODO: Validate the xml
			Platform.CheckForNullReference(doc, "doc");

			if (doc.DocumentElement!=null)
			{
				StudyReconcileDescriptorParser parser = new StudyReconcileDescriptorParser();
				StudyReconcileDescriptor desc = parser.Parse(doc);
				switch(desc.Action)
				{
					case StudyReconcileAction.CreateNewStudy: return new ReconcileCreateStudyProcessor();
					case StudyReconcileAction.Discard: return new DiscardImageCommandProcessor();
					case StudyReconcileAction.Merge: return new MergeStudyCommandProcessor();
					case StudyReconcileAction.ProcessAsIs: return new ReconcileProcessAsIsProcessor();
					default:
						throw new NotSupportedException(String.Format("Reconcile Action: {0}", desc.Action));
				}
                
			}

			return null;
		}
		#endregion
	}
}