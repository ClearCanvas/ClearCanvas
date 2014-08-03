#if UNIT_TESTS

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Common.Configuration.Tests;
using ClearCanvas.ImageViewer.Common.ServerDirectory.Tests;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Common.Comparers.Tests
{
	[TestFixture]
	public class StudyDateTimeComparerTests
	{
		const string LocalAeTitle = "AETITLE";
		const string StreamingAeTitle = "AE1";
		const string NonStreaminAeTitle = "AE2";

		[TestFixtureSetUp]
		public void Initialize()
		{
			DicomServer.Tests.DicomServerTestServiceProvider.Reset();
			StudyManagement.Tests.StudyStoreTestServiceProvider.Reset();

			Platform.SetExtensionFactory(new UnitTestExtensionFactory
                                             {
                                                 {
                                                     typeof (ServiceProviderExtensionPoint),
                                                     typeof (TestSystemConfigurationServiceProvider)
                                                     },
                                                 {
                                                     typeof (ServiceProviderExtensionPoint),
                                                     typeof (ServerDirectoryTestServiceProvider)
                                                     },
                                                 {
                                                     typeof (ServiceProviderExtensionPoint),
                                                     typeof (DicomServer.Tests.DicomServerTestServiceProvider)
                                                     },
                                                 {
                                                     typeof (ServiceProviderExtensionPoint),
                                                     typeof (StudyManagement.Tests.StudyStoreTestServiceProvider)
                                                     }
                                             });

			//Force IsSupported to be re-evaluated.
			StudyStore.InitializeIsSupported();

		}

		[Test]
		public void TestAllLocalServer()
		{
			List<StudyRootStudyIdentifier> identifiers = new List<StudyRootStudyIdentifier>();

			const string localAeTitle = "AETITLE";

			identifiers.Add(CreateStudyIdentifier("3", "20080101", "112300", localAeTitle));
			identifiers.Add(CreateStudyIdentifier("4", "20080101", "", localAeTitle));
			identifiers.Add(CreateStudyIdentifier("2", "20080104", "184400", localAeTitle));
			identifiers.Add(CreateStudyIdentifier("1", "20080104", "184500", localAeTitle));
			identifiers.Add(CreateStudyIdentifier("5", "", "", localAeTitle));
			identifiers.Add(CreateStudyIdentifier("6", "", "", localAeTitle));

			identifiers.Sort(new StudyDateTimeComparer());

			int i = 1;
			foreach (StudyRootStudyIdentifier identifier in identifiers)
			{
				Assert.AreEqual(i.ToString(CultureInfo.InvariantCulture), identifier.StudyInstanceUid);
				++i;
			}
		}

		[Test]
		public void TestSameStudyMultipleServers()
		{
			var study1Local = CreateStudyIdentifier("1", "20130810", "112300", LocalAeTitle);
			var study2Local = CreateStudyIdentifier("2", "20130901", "122400", LocalAeTitle);

			var study1Streaming = new StudyRootStudyIdentifier(study1Local){RetrieveAeTitle = StreamingAeTitle};
			var study1NonStreaming = new StudyRootStudyIdentifier(study1Local){RetrieveAeTitle = NonStreaminAeTitle};
			var study1UnknownServer = new StudyRootStudyIdentifier(study1Local) { RetrieveAeTitle = null};

			var study2Streaming = new StudyRootStudyIdentifier(study2Local) { RetrieveAeTitle = StreamingAeTitle };
			var study2NonStreaming = new StudyRootStudyIdentifier(study2Local) { RetrieveAeTitle = NonStreaminAeTitle };
			var study2UnknownServer = new StudyRootStudyIdentifier(study2Local) { RetrieveAeTitle = null };

			var identifiers = new List<StudyRootStudyIdentifier>
			                  	{
			                  		study1UnknownServer, study2NonStreaming, study1NonStreaming,
									study1Streaming, study2Local,
									study1Local, study2UnknownServer, study2Streaming
			                  	};

			foreach (var identifier in identifiers)
				identifier.ResolveServer(false);

			identifiers.Sort(new StudyDateTimeComparer());

			Assert.AreEqual(identifiers[0], study2Local);
			Assert.AreEqual(identifiers[1], study2Streaming);
			Assert.AreEqual(identifiers[2], study2NonStreaming);
			Assert.AreEqual(identifiers[3], study2UnknownServer);
			Assert.AreEqual(identifiers[4], study1Local);
			Assert.AreEqual(identifiers[5], study1Streaming);
			Assert.AreEqual(identifiers[6], study1NonStreaming);
			Assert.AreEqual(identifiers[7], study1UnknownServer);
		}

		private static StudyRootStudyIdentifier CreateStudyIdentifier(string uid, string date, string time, string aeTitle)
		{
			return new StudyRootStudyIdentifier
			       	{
			       		StudyInstanceUid = uid,
			       		StudyDate = date,
			       		StudyTime = time,
			       		RetrieveAeTitle = aeTitle
			       	};
		}
	}
}

#endif