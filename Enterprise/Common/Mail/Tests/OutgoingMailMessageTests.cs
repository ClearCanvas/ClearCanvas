#region License

// Copyright (c) 2014, ClearCanvas Inc.
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
using NUnit.Framework;

namespace ClearCanvas.Enterprise.Common.Mail.Tests
{
	[TestFixture]
	internal class OutgoingMailMessageTests
	{
		[Test]
		public void TestResolveParameters()
		{
			var result = OutgoingMailMessage.ResolveParameters("$L$orem $ipsum$ dolor $$s$i$t$$ amet$blank$, $ $consectetuer $0.99 ad$i$p$i$scing $$elit$. $$$Aenean$ $commodo$ $l$igula eget $dol$or.",
				new Dictionary<string, string>
				{
					{"i", "i"},
					{"L", "L"},
					{"ipsum", "ipswich"},
					{"Aenean", "dol$"},
					{"corpus", "sodalitas"},
					{"nothing", "nil"},
					{"blank", ""},
					{"dol", "do"}
				});

			Assert.AreEqual("Lorem ipswich dolor $sit$ amet, $ $consectetuer $0.99 adipiscing $elit$. $dol$ $commodo$ $l$igula eget door.", result);
		}

		[Test]
		public void TestResolveParametersLazy()
		{
			var countI = 0;
			var countL = 0;
			var countIpsum = 0;
			var countAenean = 0;
			var countCorpus = 0;
			var countNothing = 0;
			var countBlank = 0;
			var countDol = 0;

			var state = new Object();
			var result = OutgoingMailMessage.ResolveParameters("$L$orem $ipsum$ dolor $$s$i$t$$ amet$blank$, $ $consectetuer $0.99 ad$i$p$i$scing $$elit$. $$$Aenean$ $commodo$ $l$igula eget $dol$or.", state,
				new Dictionary<string, Func<Object, string>>
				{
					{
						"i", s =>
						{
							countI++;
							return "i";
						}
					},
					{
						"L", s =>
						{
							countL++;
							return "L";
						}
					},
					{
						"ipsum", s =>
						{
							countIpsum++;
							return "ipswich";
						}
					},
					{
						"Aenean", s =>
						{
							countAenean++;
							return "dol$";
						}
					},
					{
						"corpus", s =>
						{
							countCorpus++;
							return "sodalitas";
						}
					},
					{
						"nothing", s =>
						{
							countNothing++;
							return "nil";
						}
					},
					{
						"blank", s =>
						{
							countBlank++;
							return "";
						}
					},
					{
						"dol", s =>
						{
							countDol++;
							return "do";
						}
					}
				});

			Assert.AreEqual("Lorem ipswich dolor $sit$ amet, $ $consectetuer $0.99 adipiscing $elit$. $dol$ $commodo$ $l$igula eget door.", result);
			Assert.AreEqual(1, countI, "function 'i' should have been evaluated exactly once");
			Assert.AreEqual(1, countL, "function 'L' should have been evaluated exactly once");
			Assert.AreEqual(1, countIpsum, "function 'ipsum' should have been evaluated exactly once");
			Assert.AreEqual(1, countAenean, "function 'Aenean' should have been evaluated exactly once");
			Assert.AreEqual(0, countCorpus, "function 'corpus' should not have been evaluated at all");
			Assert.AreEqual(0, countNothing, "function 'nothing' should not have been evaluated at all");
			Assert.AreEqual(1, countBlank, "function 'blank' should have been evaluated exactly once");
			Assert.AreEqual(1, countDol, "function 'dol' should have been evaluated exactly once");
		}
	}
}

#endif