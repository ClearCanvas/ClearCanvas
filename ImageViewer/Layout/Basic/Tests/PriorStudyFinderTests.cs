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

#if UNIT_TESTS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Layout.Basic.Tests
{
    [TestFixture]
    public class PriorStudyFinderTests
    {
        private class Context : IExceptionHandlingContext
        {
            public bool ShowedMessageBox = false;

            #region Implementation of IExceptionHandlingContext

            public IDesktopWindow DesktopWindow
            {
                get { return null; }
            }

            public string ContextualMessage
            {
                get { return ""; }
            }

            public void Log(LogLevel level, Exception e)
            {
            }

            public void Abort()
            {
            }

            public void ShowMessageBox(string detailMessage)
            {
                ShowedMessageBox = true;
            }

            public void ShowMessageBox(string detailMessage, bool prependContextualMessage)
            {
                ShowedMessageBox = true;
            }

            #endregion
        }

        [Test]
        public void TestShowFindPriorsError_SingleFailure()
        {
            Test(new[] { TestParameters.Fail(true) });
        }

        [Test]
        public void TestShowFindPriorsError_Fail_Success_Fail()
        {
            Test(new[]
                     {
                         TestParameters.Fail(true),
                         TestParameters.Success(),
                         TestParameters.FailIncomplete(true)
                     });

        }

        [Test]
        public void TestShowFindPriorsError_Fail_Fail_Success_Fail()
        {
            Test(new[]
                     {
                         TestParameters.Fail(true),
                         TestParameters.FailIncomplete(false),
                         TestParameters.Success(),
                         TestParameters.FailIncomplete(true)
                     });

        }

        [Test]
        public void TestShowFindPriorsError_Success_Fail_Success_Fail()
        {
            Test(new[]
                     {
                         TestParameters.Success(),
                         TestParameters.Fail(true),
                         TestParameters.Success(),
                         TestParameters.FailIncomplete(true)
                     });

        }

        [Test]
        public void TestShowFindPriorsError_Success_Success_Fail_Fail_Success_Success_Fail()
        {
            Test(new[]
                     {
                         TestParameters.Success(),
                         TestParameters.Success(),
                         TestParameters.FailIncomplete(true),
                         TestParameters.Fail(false),
                         TestParameters.Success(),
                         TestParameters.Success(),
                         TestParameters.Fail(true)
                     });

        }


        private struct TestParameters
        {
            public TestParameters(bool findFailed, bool resultsComplete, bool messageBoxExpected)
            {
                FindFailed = findFailed;
                ResultsComplete = resultsComplete;
                MessageBoxExpected = messageBoxExpected;
                StudyCount = 1;
                OtherExceptions = new List<Exception>();
            }

            public bool FindFailed;
            public bool ResultsComplete;
            public bool MessageBoxExpected;
            public int StudyCount;
            public List<Exception> OtherExceptions;

            public static TestParameters Success()
            {
                return new TestParameters(false, true, false);
            }
            public static TestParameters Fail(bool expectMessageBox)
            {
                return new TestParameters(true, false, expectMessageBox);
            }

            public static TestParameters FailIncomplete(bool expectMessageBox)
            {
                return new TestParameters(false, false, expectMessageBox);
            }
        }

        private void Test(TestParameters[] tests)
        {
            PriorStudyLoaderExceptionPolicy.Reset();

            var policy = new PriorStudyLoaderExceptionPolicy();
            var context = new Context();
            foreach (var test in tests)
            {
                //reset.
                context.ShowedMessageBox = false;

                if (test.FindFailed || !test.ResultsComplete)
                {
                    var exception = test.FindFailed
                        ? new LoadPriorStudiesException()
                        : new LoadPriorStudiesException(test.OtherExceptions, test.StudyCount, test.ResultsComplete);

                    PriorStudyLoaderExceptionPolicy.NotifyFailedQuery();
                    policy.Handle(exception, context);
                    Assert.AreEqual(test.MessageBoxExpected, context.ShowedMessageBox);
                }
                else
                {
                    PriorStudyLoaderExceptionPolicy.NotifySuccessfulQuery();

                    if (test.OtherExceptions.Count > 0)
                    {
                        var exception = new LoadPriorStudiesException(test.OtherExceptions, test.StudyCount, test.ResultsComplete);
                        policy.Handle(exception, context);
                    }

                    Assert.AreEqual(test.MessageBoxExpected, context.ShowedMessageBox);
                }
            }
        }
    }
}

#endif