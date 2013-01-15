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

#if	UNIT_TESTS

using System;
using System.Collections.Generic;

using NUnit.Framework;
using NMock2;

using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common;
using System.Reflection;

namespace ClearCanvas.Ris.Application.Services.Tests
{
    [TestFixture]
    public class AdtServiceTest
    {
        //private Mockery _mocks;
        //private IPatientProfileBroker _mockPatientProfileBroker;
        //private IPatientBroker _mockPatientBroker;
        //private IPersistenceContext _mockPersistanceContext;
        //private IExtensionPoint _mockReconciliationStrategyXP;
        ////private IAdtService _adtService;

        //private IList<PatientProfile> _persistedProfiles;
        //private IList<Patient> _persistedPatients;

        public AdtServiceTest()
        {
        }

    //    [TestFixtureSetUp]
    //    public void TestFixtureSetup()
    //    {
    //        _mocks = new Mockery();
    //        _mockPatientProfileBroker = _mocks.NewMock<IPatientProfileBroker>();
    //        _mockPatientBroker = _mocks.NewMock<IPatientBroker>();
    //        _mockPersistanceContext = _mocks.NewMock<IPersistenceContext>();
    //        _mockReconciliationStrategyXP = _mocks.NewMock<IExtensionPoint>();

    //        //_adtService = new AdtService(_mockReconciliationStrategyXP);
    //        //ServiceLayerTestHelper.SetServiceLayerPersistenceContext((AdtService)_adtService, _mockPersistanceContext);
    //        _mocks.VerifyAllExpectationsHaveBeenMet();
    //    }

    //    [SetUp]
    //    public void PerTestSetup()
    //    {
    //        _persistedProfiles = new List<PatientProfile>();

    //        PatientProfile pat1a = new TestPatientProfile("805 1A", "SiteA", "6200 1", "Redmond", "Robert", new DateTime(1978, 10, 5));
    //        // First Name differs
    //        PatientProfile pat1b = new TestPatientProfile("805 1B", "SiteB", "6200 1", "Redmond", "Rob", new DateTime(1978, 10, 5));
    //        // Last name differs 
    //        PatientProfile pat1c = new TestPatientProfile("805 1C", "SiteC", "6200 1", "Redman", "Robert", new DateTime(1978, 10, 5));
    //        // Healthcard differs
    //        PatientProfile pat1d = new TestPatientProfile("805 1D", "SiteD", "6200 11", "Redmond", "Robert", new DateTime(1978, 10, 5));
    //        // DOB differs
    //        PatientProfile pat1e = new TestPatientProfile("805 1E", "SiteE", "6200 1", "Redmond", "Robert", new DateTime(1978, 5, 10));
    //        // only site differs
    //        PatientProfile pat1f = new TestPatientProfile("805 1F", "SiteF", "6200 1", "Redmond", "Robert", new DateTime(1978, 5, 10));
    //        // Duplicate site 
    //        PatientProfile pat1x = new TestPatientProfile("805 1X", "SiteA", "6200 1", "Redmond", "Robert", new DateTime(1978, 10, 5));

    //        PatientProfile pat2a = new TestPatientProfile("805 2", "SiteA", "6200 2", "Resnick", "Jonathan", new DateTime(1978, 10, 5));
    //        PatientProfile pat3a = new TestPatientProfile("805 3", "SiteA", "6200 3", "Chau", "Clinton", new DateTime(1978, 10, 5));
    //        PatientProfile pat4a = new TestPatientProfile("805 4", "SiteA", "6200 4", "Young", "Norman", new DateTime(1978, 10, 5));
    //        PatientProfile pat5a = new TestPatientProfile("805 5", "SiteA", "6200 5", "Hernaez", "Henry", new DateTime(1978, 10, 5));
    //        PatientProfile pat6a = new TestPatientProfile("805 6", "SiteA", "6200 6", "Bright", "Stewart", new DateTime(1978, 10, 5));

    //        _persistedProfiles.Add(pat1a);
    //        _persistedProfiles.Add(pat1b);
    //        _persistedProfiles.Add(pat1c);
    //        _persistedProfiles.Add(pat1d);
    //        _persistedProfiles.Add(pat1e);
    //        _persistedProfiles.Add(pat1f);
    //        _persistedProfiles.Add(pat1x);
    //        _persistedProfiles.Add(pat2a);
    //        _persistedProfiles.Add(pat3a);
    //        _persistedProfiles.Add(pat4a);
    //        _persistedProfiles.Add(pat5a);
    //        _persistedProfiles.Add(pat6a);

    //        _persistedPatients = new List<Patient>();

    //        _persistedPatients.Add(pat1a.Patient);
    //        _persistedPatients.Add(pat1b.Patient);
    //        _persistedPatients.Add(pat1c.Patient);
    //        _persistedPatients.Add(pat1d.Patient);
    //        _persistedPatients.Add(pat1e.Patient);
    //        _persistedPatients.Add(pat1f.Patient);
    //        _persistedPatients.Add(pat1x.Patient);
    //        _persistedPatients.Add(pat2a.Patient);
    //        _persistedPatients.Add(pat3a.Patient);
    //        _persistedPatients.Add(pat4a.Patient);
    //        _persistedPatients.Add(pat5a.Patient);
    //        _persistedPatients.Add(pat6a.Patient);
    //    }

    //    [Test]
    //    public void CanListPatientProfiles()
    //    {
    //        PatientProfileSearchCriteria criteria = new PatientProfileSearchCriteria();
    //        criteria.Name.FamilyName.Like("Redm%");

    //        Expect.Once.On(_mockPersistanceContext).Method("GetBroker").Will(Return.Value(_mockPatientProfileBroker));
    //        Expect.Once.On(_mockPatientProfileBroker).Method("Find").With(criteria).Will(Return.Value(StubPatientProfileFinder.Find(criteria, _persistedProfiles)));

    //        IList<PatientProfile> profiles = _adtService.ListPatientProfiles(criteria);

    //        Assert.AreEqual(7, profiles.Count);
    //        Assert.AreEqual(_persistedProfiles[0], profiles[0]);
    //        Assert.AreEqual(_persistedProfiles[1], profiles[1]);
    //        Assert.AreEqual(_persistedProfiles[2], profiles[2]);
    //        Assert.AreEqual(_persistedProfiles[3], profiles[3]);
    //        Assert.AreEqual(_persistedProfiles[4], profiles[4]);
    //        Assert.AreEqual(_persistedProfiles[5], profiles[5]);
    //        Assert.AreEqual(_persistedProfiles[6], profiles[6]);
    //        _mocks.VerifyAllExpectationsHaveBeenMet();
    //    }

    //    /** This test may not be applicable anymore, since IAdtService.ListReconciledPatientProfiles was removed
    //    [Test]
    //    public void CanListExistingReconciledPatients()
    //    {
    //        PatientProfile profile1 = _persistedProfiles[0];
    //        PatientProfile profile2 = _persistedProfiles[1];
    //        Patient patient = profile1.Patient;

    //        patient.AddProfile(profile2);
    //        profile2.Patient = patient;

    //        using (_mocks.Ordered)
    //        {
    //            Expect.Once.On(_mockPersistanceContext).Method("GetBroker").Will(Return.Value(_mockPatientBroker));
    //            Expect.Once.On(_mockPatientBroker).Method("LoadRelated").Will(Return.Value(null));
    //        }

    //        IList<PatientProfile> reconciledPatientsFromProfile1 = _adtService.ListReconciledPatientProfiles(profile1);

    //        using (_mocks.Ordered)
    //        {
    //            Expect.Once.On(_mockPersistanceContext).Method("GetBroker").Will(Return.Value(_mockPatientBroker));
    //            Expect.Once.On(_mockPatientBroker).Method("LoadRelated").Will(Return.Value(null));
    //        }

    //        IList<PatientProfile> reconciledPatientsFromProfile2 = _adtService.ListReconciledPatientProfiles(profile2);

    //        Assert.AreEqual(1, reconciledPatientsFromProfile1.Count);
    //        Assert.AreEqual(1, reconciledPatientsFromProfile2.Count);
    //        Assert.AreEqual(profile2, reconciledPatientsFromProfile1[0]);
    //        Assert.AreEqual(profile1, reconciledPatientsFromProfile2[0]);

    //        _mocks.VerifyAllExpectationsHaveBeenMet();
    //    }
    //    */

    //    [Test]
    //    public void CanListReconciliationMatchesFromPatient()
    //    {
    //        PatientProfile profile = _persistedProfiles[0];  
    //        PatientProfile firstNameDiffers = _persistedProfiles[1];
    //        PatientProfile lastNameDiffers = _persistedProfiles[2];
    //        PatientProfile healthcardDiffers = _persistedProfiles[3];
    //        PatientProfile dobDiffers = _persistedProfiles[4];
    //        PatientProfile onlySiteDiffers = _persistedProfiles[5];

    //        using (_mocks.Ordered)
    //        {
    //            Expect.Once.On(_mockReconciliationStrategyXP).Method("CreateExtension").Will(Return.Value(new DefaultPatientReconciliationStrategy()));
    //            Expect.Once.On(_mockPersistanceContext).Method("GetBroker").Will(Return.Value(_mockPatientProfileBroker));
    //            Expect.Once.On(_mockPatientProfileBroker).Method("Find").Will(Return.Value(profile));
    //            Expect.Once.On(_mockPersistanceContext).Method("GetBroker").Will(Return.Value(_mockPatientBroker));
    //            Expect.Once.On(_mockPatientBroker).Method("LoadRelated");
    //        }

    //        IList<PatientProfile> highMatches = new List<PatientProfile>();
    //        highMatches.Add(profile);
    //        highMatches.Add(onlySiteDiffers);  //f

    //        IList<PatientProfile> moderateMatchesViaName = new List<PatientProfile>();
    //        moderateMatchesViaName.Add(profile);
    //        moderateMatchesViaName.Add(healthcardDiffers);  //d
    //        moderateMatchesViaName.Add(onlySiteDiffers);  //f

    //        IList<PatientProfile> moderateMatchesViaHealthcard = new List<PatientProfile>();
    //        moderateMatchesViaHealthcard.Add(profile);
    //        moderateMatchesViaHealthcard.Add(firstNameDiffers);  //b
    //        moderateMatchesViaHealthcard.Add(lastNameDiffers);   //c
    //        moderateMatchesViaHealthcard.Add(dobDiffers);        //e
    //        moderateMatchesViaHealthcard.Add(onlySiteDiffers);   //f

    //        using (_mocks.Ordered)
    //        {
    //            Expect.Once.On(_mockPersistanceContext).Method("GetBroker").Will(Return.Value(_mockPatientProfileBroker));
    //            Expect.Once.On(_mockPatientProfileBroker).Method("Find").Will(Return.Value(highMatches));
    //            Expect.Once.On(_mockPatientProfileBroker).Method("Find").Will(Return.Value(moderateMatchesViaName));
    //            Expect.Once.On(_mockPatientProfileBroker).Method("Find").Will(Return.Value(moderateMatchesViaHealthcard));
    //        }

    //        IList<PatientProfileMatch> reconciliationMatches = new List<PatientProfileMatch>();// _adtService.FindPatientReconciliationMatches(profile);

    //        Assert.AreEqual(5, reconciliationMatches.Count);

    //        Assert.AreEqual(onlySiteDiffers, reconciliationMatches[0].PatientProfile);
    //        Assert.AreEqual(healthcardDiffers, reconciliationMatches[1].PatientProfile);
    //        Assert.AreEqual(firstNameDiffers, reconciliationMatches[2].PatientProfile);
    //        Assert.AreEqual(lastNameDiffers, reconciliationMatches[3].PatientProfile);
    //        Assert.AreEqual(dobDiffers, reconciliationMatches[4].PatientProfile);

    //        Assert.AreEqual(PatientProfileMatch.ScoreValue.High, reconciliationMatches[0].Score);
    //        Assert.AreEqual(PatientProfileMatch.ScoreValue.Moderate, reconciliationMatches[1].Score);
    //        Assert.AreEqual(PatientProfileMatch.ScoreValue.Moderate, reconciliationMatches[2].Score);
    //        Assert.AreEqual(PatientProfileMatch.ScoreValue.Moderate, reconciliationMatches[3].Score);
    //        Assert.AreEqual(PatientProfileMatch.ScoreValue.Moderate, reconciliationMatches[4].Score);

    //        _mocks.VerifyAllExpectationsHaveBeenMet();
    //    }

    //    //[Test]
    //    //public void CanReconcilePatientProfileWithPatientProfile()
    //    //{
    //    //    PatientProfile toBeKept = _persistedProfiles[0];
    //    //    PatientProfile toBeReconciled = _persistedProfiles[1];

    //    //    using (_mocks.Ordered)
    //    //    {
    //    //        Expect.Once.On(_mockPersistanceContext).Method("GetBroker").Will(Return.Value(_mockPatientBroker));
    //    //        Expect.Once.On(_mockPatientBroker).Method("Store");
    //    //    }

    //    //    _adtService.ReconcilePatients(toBeKept, toBeReconciled);

    //    //    PatientProfile[] profiles = new PatientProfile[toBeKept.Patient.Profiles.Count];
    //    //    toBeKept.Patient.Profiles.CopyTo(profiles, 0);

    //    //    Assert.AreEqual(toBeKept.Patient, toBeReconciled.Patient);
    //    //    Assert.AreEqual(toBeKept.Patient.Profiles.Count, 2);
    //    //    Assert.AreEqual(profiles[0], toBeKept);
    //    //    Assert.AreEqual(profiles[1], toBeReconciled);

    //    //    _mocks.VerifyAllExpectationsHaveBeenMet();
    //    //}

    //    //[Test]
    //    //public void CanReconcilePatientProfileWithPatient()
    //    //{
    //    //    Patient toBeKept = _persistedPatients[0];
    //    //    PatientProfile toBeReconciled = _persistedProfiles[1];

    //    //    using (_mocks.Ordered)
    //    //    {
    //    //        Expect.Once.On(_mockPersistanceContext).Method("GetBroker").Will(Return.Value(_mockPatientBroker));
    //    //        Expect.Once.On(_mockPatientBroker).Method("Store");
    //    //    }

    //    //    _adtService.ReconcilePatients(toBeKept, toBeReconciled);

    //    //    PatientProfile[] profiles = new PatientProfile[toBeKept.Profiles.Count];
    //    //    toBeKept.Profiles.CopyTo(profiles, 0);

    //    //    Assert.AreEqual(toBeKept, toBeReconciled.Patient);
    //    //    Assert.AreEqual(2, toBeKept.Profiles.Count);
    //    //    Assert.AreEqual(_persistedProfiles[0], profiles[0]);
    //    //    Assert.AreEqual(_persistedProfiles[1], profiles[1]);


    //    //    toBeReconciled = _persistedProfiles[2];

    //    //    using (_mocks.Ordered)
    //    //    {
    //    //        Expect.Once.On(_mockPersistanceContext).Method("GetBroker").Will(Return.Value(_mockPatientBroker));
    //    //        Expect.Once.On(_mockPatientBroker).Method("Store");
    //    //    }

    //    //    _adtService.ReconcilePatients(toBeKept, toBeReconciled);

    //    //    profiles = new PatientProfile[toBeKept.Profiles.Count];
    //    //    toBeKept.Profiles.CopyTo(profiles, 0);

    //    //    Assert.AreEqual(toBeKept, toBeReconciled.Patient);
    //    //    Assert.AreEqual(3, toBeKept.Profiles.Count);
    //    //    Assert.AreEqual(_persistedProfiles[0], profiles[0]);
    //    //    Assert.AreEqual(_persistedProfiles[1], profiles[1]);
    //    //    Assert.AreEqual(_persistedProfiles[2], profiles[2]);

    //    //    toBeReconciled = _persistedProfiles[3];

    //    //    using (_mocks.Ordered)
    //    //    {
    //    //        Expect.Once.On(_mockPersistanceContext).Method("GetBroker").Will(Return.Value(_mockPatientBroker));
    //    //        Expect.Once.On(_mockPatientBroker).Method("Store");
    //    //    }

    //    //    _adtService.ReconcilePatients(toBeKept, toBeReconciled);

    //    //    profiles = new PatientProfile[toBeKept.Profiles.Count];
    //    //    toBeKept.Profiles.CopyTo(profiles, 0);

    //    //    Assert.AreEqual(toBeKept, toBeReconciled.Patient);
    //    //    Assert.AreEqual(4, toBeKept.Profiles.Count);
    //    //    Assert.AreEqual(_persistedProfiles[0], profiles[0]);
    //    //    Assert.AreEqual(_persistedProfiles[1], profiles[1]);
    //    //    Assert.AreEqual(_persistedProfiles[2], profiles[2]);
    //    //    Assert.AreEqual(_persistedProfiles[3], profiles[3]);

    //    //    _mocks.VerifyAllExpectationsHaveBeenMet();
    //    //}

    //    //[Test]
    //    //[ExpectedException(typeof(PatientReconciliationException))]
    //    //public void CannotReconcileProfileWithItself()
    //    //{
    //    //    PatientProfile profile = new TestPatientProfile();

    //    //    _adtService.ReconcilePatients(profile, profile);
    //    //}


    //    //[Test]
    //    //[ExpectedException(typeof(PatientReconciliationException))]
    //    //public void CannotReconcilePatientsWithSameIdentifierSite()
    //    //{
    //    //    PatientProfile toBeKept = new TestPatientProfile();
    //    //    PatientProfile toBeReconciled = new TestPatientProfile();

    //    //    _adtService.ReconcilePatients(toBeKept, toBeReconciled);
    //    //}



    //}

    //public class TestPatientProfile : PatientProfile
    //{
    //    public TestPatientProfile(string Mrn, string site, string HC, string LastName, string GivenName, DateTime dob) : base()
    //    {

    //        this.Mrn = new CompositeIdentifier(Mrn, site);
    //        this.Healthcard = new HealthcardNumber(HC, "Ontario");

    //        this.Name.FamilyName = LastName;
    //        this.Name.GivenName = GivenName;
    //        this.DateOfBirth = dob;

    //        this.Patient = new Patient();
    //        this.Patient.Profiles.Add(this);
    //    }

    //    public TestPatientProfile()
    //        : this("1234", "SiteA", "6200 123456", "Test", "Patient", new DateTime(2006, 1, 1))
    //    {
    //    }

    //    public override string ToString()
    //    {
    //        return Mrn.Id + " " + Healthcard.Id + " " + Name.FamilyName + " " + Name.GivenName;
    //    }
    //}


    //public class StubPatientProfileFinder
    //{
    //    public static IList<PatientProfile> Find(PatientProfileSearchCriteria criteria, IList<PatientProfile> sourceProfiles)
    //    {
    //        IList<PatientProfile> filteredProfiles = new List<PatientProfile>();

    //        foreach (PatientProfile profile in sourceProfiles)
    //        {
    //            if (MeetsAllCriteria(criteria, profile))
    //            {
    //                filteredProfiles.Add(profile);
    //            }
    //        }

    //        return filteredProfiles;
    //    }

    //    private static bool MeetsAllCriteria(SearchCriteria criteria, Object profile)
    //    {
    //        bool match = true;
    //        foreach( SearchCriteria subCriteria in criteria.SubCriteria.Values )
    //        {
    //            if(!MeetsCurrentCriteria(subCriteria, profile))
    //            {
    //                match = false;
    //            }
    //        }
    //        return match;
    //    }

    //    private static bool MeetsCurrentCriteria(SearchCriteria subCriteria, Object profile)
    //    {
    //        PropertyInfo[] props = profile.GetType().GetProperties();
    //        PropertyInfo propToBeTested = null;
    //        foreach (PropertyInfo prop in props)
    //        {
    //            if (prop.Name == subCriteria.GetKey())
    //            {
    //                propToBeTested = prop;
    //                break;
    //            }
    //        }

    //        if (propToBeTested != null)
    //        {
    //            if (subCriteria is SearchConditionBase)
    //            {
    //                SearchConditionBase sc = (SearchConditionBase)subCriteria;
    //                return EntityPropertyMeetsSearchConditionTest(propToBeTested.GetValue(profile, null), sc.Test, sc.Values);
    //            }
    //            else
    //            {
    //                return MeetsAllCriteria(subCriteria, propToBeTested.GetValue(profile, null));
    //            }
    //        }
    //        else
    //        {
    //            return false;
    //        }
    //    }

    //    private static bool EntityPropertyMeetsSearchConditionTest(object propertyToBeTested, SearchConditionTest searchConditionTest, object[] searchConditionValues)
    //    {
    //        bool result = false;
    //        switch (searchConditionTest)
    //        {
    //            case SearchConditionTest.Like:
    //                //incomplete!  only handles like 'blah%' types of queries right now.
    //                string pattern = searchConditionValues[0].ToString().Substring(0, searchConditionValues[0].ToString().Length - 1); 
    //                result = propertyToBeTested.ToString().Contains(pattern) ? true : false;
    //                break;
    //            case SearchConditionTest.Equal:
    //                result = propertyToBeTested.Equals(searchConditionValues[0]) ? true : false;
    //                break;                 
    //            case SearchConditionTest.Between:
    //                break;
    //            case SearchConditionTest.In:
    //                break;
    //            case SearchConditionTest.LessThan:
    //                break;
    //            case SearchConditionTest.LessThanOrEqual:
    //                break;
    //            case SearchConditionTest.MoreThan:
    //                break;
    //            case SearchConditionTest.MoreThanOrEqual:
    //                break;
    //            case SearchConditionTest.None:
    //                break;
    //            case SearchConditionTest.NotEqual:
    //                break;
    //            case SearchConditionTest.NotLike:
    //                break;
    //            case SearchConditionTest.NotNull:
    //                break;
    //            case SearchConditionTest.Null:
    //                break;
    //        }
    //        return result;
    //    }

    }
}

#endif