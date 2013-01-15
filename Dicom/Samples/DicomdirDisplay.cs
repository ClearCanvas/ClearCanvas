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
using System.Windows.Forms;

namespace ClearCanvas.Dicom.Samples
{
	public partial class DicomdirDisplay : Form
	{
		public DicomdirDisplay()
		{
			InitializeComponent();
		}

		private void AddTagNodes(TreeNode parent, DicomSequenceItem record)
		{
			foreach (DicomAttribute attrib in record)
			{
				string name;
				if (attrib is DicomAttributeSQ || attrib is DicomAttributeOB || attrib is DicomAttributeOW || attrib is DicomAttributeOF)
					name = attrib.ToString();
				else
				{
					name = String.Format("{0}: {1}", attrib.Tag.ToString(), attrib.ToString());
				}
				TreeNode tagNode = new TreeNode(name);
				parent.Nodes.Add(tagNode);

				DicomAttributeSQ sqAttrib = attrib as DicomAttributeSQ;
				if (sqAttrib != null)
				{
					for (int i=0; i< sqAttrib.Count; i++)
					{
						TreeNode sqNode = new TreeNode("Sequence Item");
						tagNode.Nodes.Add(sqNode);
						AddTagNodes(sqNode, sqAttrib[i]);
					}
				}
			}
		}

		public void Add(DicomDirectory dir)
		{
			_treeViewDicomdir.BeginUpdate();
			_treeViewDicomdir.TopNode = new TreeNode();
			
			TreeNode topNode = new TreeNode("DICOMDIR: " + dir.FileSetId);

			_treeViewDicomdir.Nodes.Add( topNode);

			foreach (DirectoryRecordSequenceItem patientRecord in dir.RootDirectoryRecordCollection)
			{
				TreeNode patientNode = new TreeNode(patientRecord.ToString());
				topNode.Nodes.Add(patientNode);

				AddTagNodes(patientNode, patientRecord);

				foreach (DirectoryRecordSequenceItem studyRecord in patientRecord.LowerLevelDirectoryRecordCollection)
				{
					TreeNode studyNode = new TreeNode(studyRecord.ToString());
					patientNode.Nodes.Add(studyNode);

					AddTagNodes(studyNode, studyRecord);

					foreach (DirectoryRecordSequenceItem seriesRecord in studyRecord.LowerLevelDirectoryRecordCollection)
					{
						TreeNode seriesNode = new TreeNode(seriesRecord.ToString());
						studyNode.Nodes.Add(seriesNode);

						AddTagNodes(seriesNode, seriesRecord);

						foreach (DirectoryRecordSequenceItem instanceRecord in seriesRecord.LowerLevelDirectoryRecordCollection)
						{
							TreeNode instanceNode = new TreeNode(instanceRecord.ToString());
							seriesNode.Nodes.Add(instanceNode);

							AddTagNodes(instanceNode, instanceRecord);
						}
					}
				}
			}
			_treeViewDicomdir.EndUpdate();
		}
	}
}
