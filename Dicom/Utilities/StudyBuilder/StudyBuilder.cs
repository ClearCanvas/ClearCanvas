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
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.Dicom.Utilities.Anonymization;

namespace ClearCanvas.Dicom.Utilities.StudyBuilder
{
	public sealed class StudyBuilderException : DicomException
	{
		public StudyBuilderException(string message) : base(message) {}
		public StudyBuilderException(string message, Exception innerException) : base(message, innerException) {}
	}

	/// <summary>
	/// A generic DICOM study manipulator class. This class cannot be inherited.
	/// </summary>
	/// <remarks>
	/// <para>The <see cref="StudyBuilder"/> class abstracts the code required to construct new DICOM studies from scratch, or from existing instances.
	/// All resultant instances carry new UIDs that distinguish them from the original instances (if used), generated to conform with the structure defined
	/// in the builder and thereby erasing the original study structure.</para>
	/// <para>The hierarchial tree-based view defines standard tags on data nodes at four different levels in the study tree:
	/// <see cref="PatientNode">patient</see>, <see cref="StudyNode">study</see>, <see cref="SeriesNode">series</see>, and <see cref="SopInstanceNode">SOP</see>.
	/// By inserting instances, editing node properties and moving the individual nodes around the tree, an entirely new study structure can be
	/// created programmatically and henceforth exported to a filesystem or to a DICOM server.</para>
	/// </remarks>
	public sealed class StudyBuilder
	{
		private const string MSG_ASYNCEXIST = "This StudyBuilder is already running an asynchronous operation.";
		private const string MSG_ASYNCNOTEXIST = "This StudyBuilder is not running an asynchronous operation.";
		private readonly PatientNodeCollection _patients;
		private readonly RootNode _rootNode;
		private bool _anonymize = true;
		private float _progress = 0f;

		/// <summary>
		/// Constructs a new instance of the <see cref="StudyBuilder"/> using the default options.
		/// </summary>
		public StudyBuilder()
		{
			_patients = new PatientNodeCollection(this);
			_rootNode = new RootNode();
		}

		#region Behaviour Properties

		public bool Anonymize
		{
			get { return _anonymize; }
			set { _anonymize = value; }
		}

		#endregion

		#region Builder Properties

		/// <summary>
		/// Gets a collection of all the <see cref="PatientNode"/>s in the study builder tree.
		/// </summary>
		public PatientNodeCollection Patients
		{
			get { return _patients; }
		}

		/// <summary>
		/// Gets the root node for this builder tree.
		/// </summary>
		internal StudyBuilderNode Root
		{
			get { return _rootNode; }
		}

		#endregion

		#region Operation State

		public float Progress
		{
			get { return _progress; }
			private set { _progress = value; }
		}

		//public string Status
		//{
		//    get { return _status; }
		//    private set { _status = value; }
		//}

		#endregion

		#region Public Methods - Build Tree

		/// <summary>
		/// Builds the UID relationships of all nodes in the study builder according to the current tree structure.
		/// </summary>
		/// <remarks>
		/// <para>Building the study tree involves creating new instance UIDs, mapping any DICOM attributes with a VR of UI to use these new instance
		/// UIDs (if the UIDs are not instances in the same study tree, then they are not changed), and performing anonymization if the <see cref="Anonymize"/>
		/// property is set.</para>
		/// <para>All the export operations automatically call this method, so it is unnecessary to explicitly call <see cref="BuildTree"/> in those situations.
		/// This method is provided to allow access to the updated, underlying data sets of the individual SOP instances.</para>
		/// </remarks>
		public IEnumerable<DicomFile> BuildTree()
		{
			List<SopInstanceNode> sops;

			try
			{
				sops = DoBuildTree();
			}
			catch (Exception ex)
			{
				throw new StudyBuilderException("Unexpected StudyBuilder error", ex);
			}

			List<DicomFile> list = new List<DicomFile>(sops.Count);
			foreach (SopInstanceNode sop in sops)
			{
				list.Add(sop.DicomFile);
			}
			return list.AsReadOnly();
		}

		private delegate IEnumerable<DicomFile> BuildTreeDelegate();

		private BuildTreeDelegate _buildTreeDelegate;

		public IAsyncResult BeginBuildTree(AsyncCallback callback, object state)
		{
			if (_buildTreeDelegate != null)
				throw new InvalidOperationException(MSG_ASYNCEXIST);

			_buildTreeDelegate = new BuildTreeDelegate(BuildTree);
			return _buildTreeDelegate.BeginInvoke(callback, state);
		}

		public IEnumerable<DicomFile> EndBuildTree(IAsyncResult result)
		{
			if (_buildTreeDelegate == null)
				throw new InvalidOperationException(MSG_ASYNCNOTEXIST);

			return _buildTreeDelegate.EndInvoke(result);
		}

		#endregion

		#region Public Helpers - Publish

		/// <summary>
		/// Builds the study tree and publishes all the individual SOP instances to the specified directory.
		/// </summary>
		/// <remarks>
		/// <para>The published files use the SOP instance UID as the filename with a &quot;.dcm&quot; extension.</para>
		/// <para>The <see cref="BuildTree"/> method is called automatically, and hence does not need to be explicitly called before invoking this method.</para>
		/// </remarks>
		/// <param name="path">The path of the directory to which the SOP instances are saved.</param>
		/// <returns>A list of the filenames of the resulting SOP instances.</returns>
		public IList<string> Publish(string path)
		{
			List<SopInstanceNode> sops;

			try
			{
				sops = DoBuildTree();
			}
			catch (Exception ex)
			{
				throw new StudyBuilderException("Unexpected StudyBuilder error", ex);
			}

			List<string> files = new List<string>(sops.Count);

			try
			{
				foreach (SopInstanceNode sop in sops)
				{
					files.Add(sop.ExportToDirectory(path));
				}
			}
			catch (Exception ex)
			{
				throw new StudyBuilderException("I/O error", ex);
			}

			return files.AsReadOnly();
		}

		/// <summary>
		/// Builds the study tree and publishes all the created studies to the specified application entity.
		/// </summary>
		/// <remarks>
		/// <para>The <see cref="BuildTree"/> method is called automatically, and hence does not need to be explicitly called before invoking this method.</para>
		/// </remarks>
		/// <param name="localAE">The local AETITLE that is sending the studies.</param>
		/// <param name="remoteAE">The AETITLE of the device that is receiving the studies.</param>
		/// <param name="remoteHost">The hostname of the device that is receiving the studies.</param>
		/// <param name="remotePort">The port number on which the device receiving the studies is listening.</param>
		/// <returns>A list of the SOP instance UIDs that were created.</returns>
		public IList<string> Publish(string localAE, string remoteAE, string remoteHost, int remotePort)
		{
			List<SopInstanceNode> sops;

			try
			{
				sops = DoBuildTree();
			}
			catch (Exception ex)
			{
				throw new StudyBuilderException("Unexpected StudyBuilder error", ex);
			}

			List<string> uids = new List<string>(sops.Count);

			try
			{
				StorageScu scu = new StorageScu(localAE, remoteAE, remoteHost, remotePort);

				// queue each instance into scu
				foreach (SopInstanceNode sop in sops)
				{
					StorageInstance sInst = new StorageInstance(sop.DicomFile);
					scu.AddStorageInstance(sInst);
					uids.Add(sop.InstanceUid);
				}

				// begin asynch send operation
				scu.Send();
			}
			catch (Exception ex)
			{
				throw new StudyBuilderException("Storage SCU error", ex);
			}

			return uids.AsReadOnly();
		}

		#endregion

		#region Internal Tree Build

		private List<SopInstanceNode> DoBuildTree()
		{
			bool doAnonymize = _anonymize;
			Dictionary<string, string> uidMap = new Dictionary<string, string>();
			List<SopInstanceNode> sops = new List<SopInstanceNode>();

			// TODO: perform some performance tests to adjust these weights
			float aweight = doAnonymize ? 0.45f : 0; // portion of the work to anonymize the instances
			float pweight = (1 - aweight)*0.75f; // portion of the work to reassign uids
			float mweight = 1 - pweight - aweight; // portion of the work to remap related uids
			int count = _patients.Count;
			int now = 0;

			this.Progress = 0;

			// traverse the tree assign new instance uids
			foreach (PatientNode patient in _patients)
			{
				if (patient.Parent != _rootNode)
					throw new NullReferenceException("Unsynchronized parent-child relationship");

				now++;

				foreach (StudyNode study in patient.Studies)
				{
					if (study.Parent != patient)
						throw new NullReferenceException("Unsynchronized parent-child relationship");

					string studyUid = NewUid();
					uidMap.Add(study.InstanceUid, studyUid);
					study.InstanceUid = studyUid;

					foreach (SeriesNode series in study.Series)
					{
						if (series.Parent != study)
							throw new NullReferenceException("Unsynchronized parent-child relationship");

						string seriesUid = NewUid();
						uidMap.Add(series.InstanceUid, seriesUid);
						series.InstanceUid = seriesUid;

						foreach (SopInstanceNode sop in series.Images)
						{
							if (sop.Parent != series)
								throw new NullReferenceException("Unsynchronized parent-child relationship");

							string sopUid = NewUid();
							uidMap.Add(sop.InstanceUid, sopUid);
							sop.InstanceUid = sopUid;

							patient.Update(sop.DicomData);
							study.Update(sop.DicomData, true);
							series.Update(sop.DicomData, true);
							sop.Update(sop.DicomData, true);

							sops.Add(sop);
						}
					}
				}

				this.Progress = mweight*now/count;
			}

			// map any uids that point to an instance that was just reassigned
			count = sops.Count;
			now = 0;
			foreach (SopInstanceNode sop in sops)
			{
				MapKnownUids(sop.DicomData, uidMap);

				now++;
				this.Progress = mweight*now/count;
			}

			// run the anonymizer if required
			if (doAnonymize)
			{
				DicomAnonymizer anonymizer = new DicomAnonymizer();
				anonymizer.ValidationOptions = ValidationOptions.RelaxAllChecks;

				count = sops.Count;
				now = 0;

				foreach (SopInstanceNode sop in sops)
				{
					anonymizer.Anonymize(sop.DicomFile);

					SeriesNode series = sop.Parent;
					StudyNode study = series.Parent;
					PatientNode patient = study.Parent;

					// overwrite the anonymized data with any edited properties
					// anonymizer writes in new anonymized uids based on the new structure, so don't overwrite them!
					// instead, get the new uids and put them back into the node
					patient.Update(sop.DicomData);

					study.Update(sop.DicomData, false);
					study.InstanceUid = sop.DicomData[DicomTags.StudyInstanceUid].GetString(0, "");

					series.Update(sop.DicomData, false);
					series.InstanceUid = sop.DicomData[DicomTags.SeriesInstanceUid].GetString(0, "");

					sop.Update(sop.DicomData, false);
					sop.InstanceUid = sop.DicomData[DicomTags.SopInstanceUid].GetString(0, "");

					now++;
					this.Progress = mweight*now/count;
				}
			}

			return sops;
		}

		/// <summary>
		/// Recursively find UI attributes and try to map any known uids it finds (unknown uids are left alone)
		/// </summary>
		/// <param name="dataset"></param>
		/// <param name="uidMap"></param>
		private static void MapKnownUids(DicomAttributeCollection dataset, IDictionary<string, string> uidMap)
		{
			foreach (uint tag in GetTagsToRemap())
			{
				DicomAttribute attribute = dataset[tag];
				int count = (int) (0x7fffffff & attribute.Count);
				for (int n = 0; n < count; n++)
				{
					string originalUid = attribute.GetString(n, "");
					if (uidMap.ContainsKey(originalUid))
					{
						attribute.SetString(n, uidMap[originalUid]);
					}
				}
			}

			//foreach (DicomAttribute attribute in dataset)
			//{
			//    if (attribute is DicomAttributeUI)
			//    {
			//        int count = (int) (0x7fffffff & attribute.Count);
			//        for (int n = 0; n < count; n++)
			//        {
			//            string originalUid = attribute.GetString(n, "");
			//            if (uidMap.ContainsKey(originalUid))
			//            {
			//                attribute.SetString(n, uidMap[originalUid]);
			//            }
			//        }
			//    }
			//    else if (attribute is DicomAttributeSQ)
			//    {
			//        DicomAttributeSQ sq = (DicomAttributeSQ) attribute;
			//        int count = (int) (0x7fffffff & attribute.Count);
			//        for (int n = 0; n < count; n++)
			//        {
			//            MapKnownUids(sq[n], uidMap);
			//        }
			//    }
			//}
		}

		private static IEnumerable<uint> GetTagsToRemap()
		{
			yield return DicomTags.ReferencedSopInstanceUid;
			yield return DicomTags.FrameOfReferenceUid;
			yield return DicomTags.SynchronizationFrameOfReferenceUid;
			yield return DicomTags.Uid;
			yield return DicomTags.ReferencedFrameOfReferenceUid;
			yield return DicomTags.RelatedFrameOfReferenceUid;
		}

		#endregion

		#region Internal Uid Generation

		/// <summary>
		/// Gets a new uid using the <see cref="StudyBuilder"/>-preferred method for generating uids.
		/// </summary>
		/// <returns></returns>
		internal static string NewUid()
		{
			return DicomUid.GenerateUid().UID;
		}

		#endregion

		#region Internal Root Node Class

		private class RootNode : StudyBuilderNode {}

		#endregion
	}
}