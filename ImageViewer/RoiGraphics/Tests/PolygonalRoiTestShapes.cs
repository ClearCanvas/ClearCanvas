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

#pragma warning disable 1591,0419,1574,1587

using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace ClearCanvas.ImageViewer.RoiGraphics.Tests
{
	partial class PolygonalRoiTests
	{
		public class PolygonShape : IEnumerable<PointF>
		{
			#region Shape "Sierra" (for Complex07, 08 and 09)

			/// <summary>
			/// &quot;S&quot; shape for
			/// <see cref="ImageKey.Complex07"/>, <see cref="ImageKey.Complex08"/> and <see cref="ImageKey.Complex09"/>.
			/// </summary>
			public static readonly PolygonShape Sierra = new PolygonShape("SIERRA",
			                                                              new PointF(200, 50),
			                                                              new PointF(50, 50),
			                                                              new PointF(50, 150),
			                                                              new PointF(175, 150),
			                                                              new PointF(175, 200),
			                                                              new PointF(50, 200),
			                                                              new PointF(50, 225),
			                                                              new PointF(200, 225),
			                                                              new PointF(200, 125),
			                                                              new PointF(75, 125),
			                                                              new PointF(75, 75),
			                                                              new PointF(200, 75));

			/// <summary>
			/// &quot;S&quot; shape as a complex polygon definition for
			/// <see cref="ImageKey.Complex07"/>, <see cref="ImageKey.Complex08"/> and <see cref="ImageKey.Complex09"/>.
			/// </summary>
			public static readonly PolygonShape SierraPrime = new PolygonShape("SIERRA_PRIME",
			                                                                   new PointF(200, 50),
			                                                                   new PointF(50, 50),
			                                                                   new PointF(50, 150),
			                                                                   new PointF(170, 150),
			                                                                   new PointF(112.5f, 130),
			                                                                   new PointF(55, 130),
			                                                                   new PointF(55, 150),
			                                                                   new PointF(175, 150),
			                                                                   new PointF(175, 200),
			                                                                   new PointF(50, 200),
			                                                                   new PointF(50, 225),
			                                                                   new PointF(200, 225),
			                                                                   new PointF(200, 125),
			                                                                   new PointF(75, 125),
			                                                                   new PointF(75, 75),
			                                                                   new PointF(200, 75));

			#endregion

			#region Shape "Golf" (for Complex01, 02 and 03)

			/// <summary>
			/// &quot;G&quot; shape for
			/// <see cref="ImageKey.Complex01"/>, <see cref="ImageKey.Complex02"/> and <see cref="ImageKey.Complex03"/>.
			/// </summary>
			public static readonly PolygonShape Golf = new PolygonShape("GOLF",
			                                                            new PointF(200, 100),
			                                                            new PointF(200, 50),
			                                                            new PointF(50, 50),
			                                                            new PointF(50, 200),
			                                                            new PointF(200, 200),
			                                                            new PointF(200, 125),
			                                                            new PointF(125, 125),
			                                                            new PointF(125, 150),
			                                                            new PointF(175, 150),
			                                                            new PointF(175, 175),
			                                                            new PointF(75, 175),
			                                                            new PointF(75, 75),
			                                                            new PointF(175, 75),
			                                                            new PointF(175, 100));

			/// <summary>
			/// &quot;G&quot; shape as a complex polygon definition for
			/// <see cref="ImageKey.Complex01"/>, <see cref="ImageKey.Complex02"/> and <see cref="ImageKey.Complex03"/>.
			/// </summary>
			public static readonly PolygonShape GolfPrime = new PolygonShape("GOLF_PRIME",
			                                                                 new PointF(200, 100),
			                                                                 new PointF(200, 50),
			                                                                 new PointF(50, 50),
			                                                                 new PointF(50, 175),
			                                                                 new PointF(70, 175),
			                                                                 new PointF(70, 75),
			                                                                 new PointF(50, 75),
			                                                                 new PointF(50, 200),
			                                                                 new PointF(200, 200),
			                                                                 new PointF(200, 125),
			                                                                 new PointF(125, 125),
			                                                                 new PointF(125, 150),
			                                                                 new PointF(175, 150),
			                                                                 new PointF(175, 175),
			                                                                 new PointF(75, 175),
			                                                                 new PointF(75, 75),
			                                                                 new PointF(175, 75),
			                                                                 new PointF(175, 100));

			#endregion

			#region Shape "Charlie" (for Complex04, 05 and 06)

			/// <summary>
			/// &quot;C&quot; shape for
			/// <see cref="ImageKey.Complex04"/>, <see cref="ImageKey.Complex05"/> and <see cref="ImageKey.Complex06"/>.
			/// </summary>
			public static readonly PolygonShape Charlie = new PolygonShape("CHARLIE", // the Unicorn
			                                                               new PointF(225, 100),
			                                                               new PointF(225, 50),
			                                                               new PointF(50, 50),
			                                                               new PointF(50, 225),
			                                                               new PointF(225, 225),
			                                                               new PointF(225, 150),
			                                                               new PointF(200, 150),
			                                                               new PointF(200, 200),
			                                                               new PointF(100, 200),
			                                                               new PointF(100, 75),
			                                                               new PointF(200, 75),
			                                                               new PointF(200, 100));

			/// <summary>
			/// &quot;C&quot; shape as a complex polygon definition for
			/// <see cref="ImageKey.Complex04"/>, <see cref="ImageKey.Complex05"/> and <see cref="ImageKey.Complex06"/>.
			/// </summary>
			public static readonly PolygonShape CharliePrime = new PolygonShape("CHARLIE_PRIME",
			                                                                    new PointF(225, 100),
			                                                                    new PointF(225, 50),
			                                                                    new PointF(50, 50),
			                                                                    new PointF(50, 137.5f),
			                                                                    new PointF(100, 137.5f),
			                                                                    new PointF(100, 200),
			                                                                    new PointF(200, 200),
			                                                                    new PointF(200, 150),
			                                                                    new PointF(225, 150),
			                                                                    new PointF(225, 225),
			                                                                    new PointF(50, 225),
			                                                                    new PointF(50, 137.5f),
			                                                                    new PointF(100, 137.5f),
			                                                                    new PointF(100, 75),
			                                                                    new PointF(200, 75),
			                                                                    new PointF(200, 100));

			#endregion

			#region Shape "Arrowhead" (for Complex10, 11 and 12)

			/// <summary>
			/// Arrowhead shape for
			/// <see cref="ImageKey.Complex10"/>, <see cref="ImageKey.Complex11"/> and <see cref="ImageKey.Complex12"/>.
			/// </summary>
			public static readonly PolygonShape Arrowhead = new PolygonShape("ARROWHEAD", // the "Star Trek" shape
			                                                                 new PointF(125, 50),
			                                                                 new PointF(50, 225),
			                                                                 new PointF(125, 175),
			                                                                 new PointF(200, 225));

			/// <summary>
			/// Arrowhead shape as a complex polygon definition for
			/// <see cref="ImageKey.Complex10"/>, <see cref="ImageKey.Complex11"/> and <see cref="ImageKey.Complex12"/>.
			/// </summary>
			public static readonly PolygonShape ArrowheadPrime = new PolygonShape("ARROWHEAD_PRIME", // the "Star Trek" shape
			                                                                      new PointF(125, 50),
			                                                                      new PointF(50, 225),
			                                                                      new PointF(125, 175),
			                                                                      new PointF(125, 50),
			                                                                      new PointF(200, 225),
			                                                                      new PointF(125, 175),
			                                                                      new PointF(125, 50));

			#endregion

			#region Shape "Triangle"

			/// <summary>
			/// Equilateral triangle (Side 200, Altitude 173) for
			/// <see cref="ImageKey.Aspect01"/>, <see cref="ImageKey.Aspect02"/>, <see cref="ImageKey.Aspect03"/> and <see cref="ImageKey.Aspect04"/>.
			/// </summary>
			public static readonly PolygonShape Triangle = new PolygonShape("TRIANGLE", // an equilateral triangle (side=200 altitude=173)
			                                                                new PointF(125, 52),
			                                                                new PointF(25, 225),
																			new PointF(225, 225));

			/// <summary>
			/// Isoceles triangle (Base 267, Altitude 173) for
			/// <see cref="ImageKey.Aspect05"/>, <see cref="ImageKey.Aspect06"/>, <see cref="ImageKey.Aspect07"/> and <see cref="ImageKey.Aspect08"/>.
			/// </summary>
			public static readonly PolygonShape TriangleWide = new PolygonShape("TRIANGLE_WIDE", // an isoceles triangle (base=267 altitude=173)
			                                                                    new PointF(166.5f, 52),
			                                                                    new PointF(33, 225),
			                                                                    new PointF(300, 225));

			/// <summary>
			/// Isoceles triangle (Base 200, Altitude 231) for
			/// <see cref="ImageKey.Aspect09"/>, <see cref="ImageKey.Aspect10"/>, <see cref="ImageKey.Aspect11"/> and <see cref="ImageKey.Aspect12"/>.
			/// </summary>
			public static readonly PolygonShape TriangleNarrow = new PolygonShape("TRIANGLE_NARROW", // an isoceles triangle (base=200 altitude=231)
			                                                                      new PointF(125, 69),
			                                                                      new PointF(25, 300),
			                                                                      new PointF(225, 300));

			#endregion

			#region Shape Real01

			/// <summary>
			/// Anatomy outline shape for <see cref="ImageKey.Real01"/>.
			/// </summary>
			public static readonly PolygonShape ShapeOnReal01 = new PolygonShape("REAL01", true,
			                                                                     new PointF(110.864601f, 91.345749f),
			                                                                     new PointF(110.735779f, 93.020645f),
			                                                                     new PointF(112.152992f, 93.535995f),
			                                                                     new PointF(113.956711f, 95.726234f),
			                                                                     new PointF(115.245087f, 95.726234f),
			                                                                     new PointF(116.6623f, 96.241585f),
			                                                                     new PointF(115.373924f, 98.947166f),
			                                                                     new PointF(116.6623f, 99.591354f),
			                                                                     new PointF(117.821838f, 100.493217f),
			                                                                     new PointF(119.239044f, 101.266243f),
			                                                                     new PointF(121.04277f, 100.87973f),
			                                                                     new PointF(122.975334f, 101.910431f),
			                                                                     new PointF(123.619522f, 103.58532f),
			                                                                     new PointF(126.711617f, 101.652756f),
			                                                                     new PointF(126.840454f, 99.591354f),
			                                                                     new PointF(126.840454f, 98.174149f),
			                                                                     new PointF(128.644165f, 98.302971f),
			                                                                     new PointF(128.51532f, 100.750877f),
			                                                                     new PointF(128.386505f, 103.069969f),
			                                                                     new PointF(129.674881f, 103.842995f),
			                                                                     new PointF(130.963242f, 103.714157f),
			                                                                     new PointF(131.478592f, 102.168106f),
			                                                                     new PointF(131.865112f, 101.008568f),
			                                                                     new PointF(133.411163f, 101.008568f),
			                                                                     new PointF(135.472565f, 101.523918f),
			                                                                     new PointF(136.760925f, 101.910431f),
			                                                                     new PointF(139.337677f, 101.910431f),
			                                                                     new PointF(139.337677f, 100.750893f),
			                                                                     new PointF(140.110703f, 99.076004f),
			                                                                     new PointF(140.626053f, 98.560654f),
			                                                                     new PointF(142.172104f, 97.01461f),
			                                                                     new PointF(143.846985f, 95.98391f),
			                                                                     new PointF(143.718155f, 94.309021f),
			                                                                     new PointF(145.779556f, 94.180183f),
			                                                                     new PointF(146.423737f, 93.793671f),
			                                                                     new PointF(146.294907f, 92.891808f),
			                                                                     new PointF(146.552582f, 91.732277f),
			                                                                     new PointF(146.552582f, 90.572739f),
			                                                                     new PointF(145.393036f, 90.443901f),
			                                                                     new PointF(143.97583f, 90.443901f),
			                                                                     new PointF(142.172104f, 91.861115f),
			                                                                     new PointF(140.626053f, 92.76297f),
			                                                                     new PointF(138.822327f, 93.149483f),
			                                                                     new PointF(136.50325f, 93.793671f),
			                                                                     new PointF(135.601395f, 95.082047f),
			                                                                     new PointF(133.668839f, 95.597397f),
			                                                                     new PointF(131.736237f, 96.112732f),
			                                                                     new PointF(130.447891f, 96.756935f),
			                                                                     new PointF(127.484619f, 97.658783f),
			                                                                     new PointF(126.582779f, 96.756935f),
			                                                                     new PointF(125.680916f, 96.756935f),
			                                                                     new PointF(124.006035f, 96.49926f),
			                                                                     new PointF(122.975334f, 95.98391f),
			                                                                     new PointF(122.202309f, 95.855072f),
			                                                                     new PointF(121.04277f, 95.597397f),
			                                                                     new PointF(119.883232f, 94.566696f),
			                                                                     new PointF(118.723694f, 93.535995f),
			                                                                     new PointF(117.048813f, 93.27832f),
			                                                                     new PointF(115.502762f, 92.505302f),
			                                                                     new PointF(114.600899f, 91.603439f),
			                                                                     new PointF(113.570198f, 90.572739f),
			                                                                     new PointF(112.668335f, 90.057388f),
			                                                                     new PointF(110.864601f, 91.345749f));

			#endregion

			#region Shape Real02

			/// <summary>
			/// Anatomy outline shape for <see cref="ImageKey.Real02"/>.
			/// </summary>
			public static readonly PolygonShape ShapeOnReal02 = new PolygonShape("REAL02", true,
			                                                                     new PointF(149.773514f, 158.856567f),
			                                                                     new PointF(146.810257f, 146.74585f),
			                                                                     new PointF(142.687454f, 142.365372f),
			                                                                     new PointF(141.656754f, 139.917465f),
			                                                                     new PointF(141.527924f, 136.052338f),
			                                                                     new PointF(141.527924f, 133.733276f),
			                                                                     new PointF(139.981873f, 131.54303f),
			                                                                     new PointF(141.656754f, 129.095123f),
			                                                                     new PointF(143.202805f, 127.03373f),
			                                                                     new PointF(144.877686f, 125.487679f),
			                                                                     new PointF(146.037231f, 125.101166f),
			                                                                     new PointF(146.037231f, 117.242081f),
			                                                                     new PointF(146.037231f, 111.573235f),
			                                                                     new PointF(145.908386f, 108.609978f),
			                                                                     new PointF(143.718155f, 109.769516f),
			                                                                     new PointF(142.429779f, 115.438362f),
			                                                                     new PointF(139.595352f, 119.303482f),
			                                                                     new PointF(137.147446f, 120.978371f),
			                                                                     new PointF(134.828369f, 122.910927f),
			                                                                     new PointF(133.153488f, 123.81279f),
			                                                                     new PointF(132.122787f, 123.683952f),
			                                                                     new PointF(131.349762f, 123.039764f),
			                                                                     new PointF(130.963257f, 125.874191f),
			                                                                     new PointF(131.607437f, 128.708603f),
			                                                                     new PointF(130.705582f, 130.125824f),
			                                                                     new PointF(131.607437f, 134.506287f),
			                                                                     new PointF(131.736282f, 138.629089f),
			                                                                     new PointF(134.828369f, 141.721191f),
			                                                                     new PointF(136.760941f, 144.040268f),
			                                                                     new PointF(137.791641f, 146.488174f),
			                                                                     new PointF(139.466522f, 149.193756f),
			                                                                     new PointF(141.656754f, 152.930038f),
			                                                                     new PointF(143.202805f, 153.187714f),
			                                                                     new PointF(145.908386f, 156.022141f),
			                                                                     new PointF(149.773483f, 158.856537f));

			#endregion

			#region Shape Real03

			/// <summary>
			/// Anatomy outline shape for <see cref="ImageKey.Real03"/>.
			/// </summary>
			public static readonly PolygonShape ShapeOnReal03 = new PolygonShape("REAL03", true,
			                                                                     new PointF(230, 133),
			                                                                     new PointF(228, 135),
			                                                                     new PointF(226, 135),
			                                                                     new PointF(225, 134),
			                                                                     new PointF(224, 135),
			                                                                     new PointF(224, 137),
			                                                                     new PointF(223, 138),
			                                                                     new PointF(223, 140),
			                                                                     new PointF(222, 141),
			                                                                     new PointF(223, 143),
			                                                                     new PointF(225, 145),
			                                                                     new PointF(227, 148),
			                                                                     new PointF(228, 149),
			                                                                     new PointF(230, 150),
			                                                                     new PointF(232, 151),
			                                                                     new PointF(237, 151),
			                                                                     new PointF(239, 149),
			                                                                     new PointF(241, 149),
			                                                                     new PointF(245, 147),
			                                                                     new PointF(249, 147),
			                                                                     new PointF(251, 145),
			                                                                     new PointF(253, 145),
			                                                                     new PointF(261, 145),
			                                                                     new PointF(263, 144),
			                                                                     new PointF(264, 143),
			                                                                     new PointF(271, 143),
			                                                                     new PointF(278, 147),
			                                                                     new PointF(283, 146),
			                                                                     new PointF(297, 146),
			                                                                     new PointF(297, 147),
			                                                                     new PointF(300, 147),
			                                                                     new PointF(302, 146),
			                                                                     new PointF(305, 143),
			                                                                     new PointF(305, 139),
			                                                                     new PointF(303, 137),
			                                                                     new PointF(302, 135),
			                                                                     new PointF(299, 133),
			                                                                     new PointF(294, 131),
			                                                                     new PointF(289, 130),
			                                                                     new PointF(285, 129),
			                                                                     new PointF(281, 131),
			                                                                     new PointF(280, 131),
			                                                                     new PointF(273, 131),
			                                                                     new PointF(270, 129),
			                                                                     new PointF(266, 128),
			                                                                     new PointF(263, 126),
			                                                                     new PointF(259, 126),
			                                                                     new PointF(258, 126),
			                                                                     new PointF(256, 126),
			                                                                     new PointF(250, 126),
			                                                                     new PointF(247, 127),
			                                                                     new PointF(242, 128),
			                                                                     new PointF(238, 129),
			                                                                     new PointF(234, 131));

			#endregion

			#region Shape Real04

			/// <summary>
			/// Anatomy outline shape for <see cref="ImageKey.Real04"/>.
			/// </summary>
			public static readonly PolygonShape ShapeOnReal04 = new PolygonShape("REAL04", true,
			                                                                     new PointF(162.928177f, 442.904297f),
			                                                                     new PointF(173.52829f, 429.835663f),
			                                                                     new PointF(177.884506f, 430.706909f),
			                                                                     new PointF(178.900955f, 431.723358f),
			                                                                     new PointF(181.369476f, 432.739807f),
			                                                                     new PointF(192.695633f, 431.723358f),
			                                                                     new PointF(201.262848f, 428.674011f),
			                                                                     new PointF(213.16983f, 425.479431f),
			                                                                     new PointF(221.156219f, 422.865723f),
			                                                                     new PointF(225.222015f, 417.202637f),
			                                                                     new PointF(226.964493f, 410.377899f),
			                                                                     new PointF(228.561768f, 397.744904f),
			                                                                     new PointF(229.723434f, 392.372223f),
			                                                                     new PointF(232.917984f, 389.177673f),
			                                                                     new PointF(236.98378f, 386.273529f),
			                                                                     new PointF(241.339996f, 384.966675f),
			                                                                     new PointF(245.115372f, 386.128326f),
			                                                                     new PointF(246.277039f, 387.580414f),
			                                                                     new PointF(248.019516f, 385.692719f),
			                                                                     new PointF(245.986618f, 380.465271f),
			                                                                     new PointF(243.808517f, 379.448822f),
			                                                                     new PointF(242.646866f, 373.930939f),
			                                                                     new PointF(241.920822f, 367.54184f),
			                                                                     new PointF(241.77562f, 363.621246f),
			                                                                     new PointF(241.77562f, 361.007507f),
			                                                                     new PointF(268.929321f, 361.15271f),
			                                                                     new PointF(269.074524f, 367.832245f),
			                                                                     new PointF(266.751221f, 393.098267f),
			                                                                     new PointF(265.734772f, 403.407959f),
			                                                                     new PointF(265.879974f, 406.602539f),
			                                                                     new PointF(265.589569f, 414.588928f),
			                                                                     new PointF(263.992279f, 423.156128f),
			                                                                     new PointF(260.216919f, 430.852112f),
			                                                                     new PointF(251.504486f, 436.6604f),
			                                                                     new PointF(246.712662f, 438.402863f),
			                                                                     new PointF(239.307098f, 437.241211f),
			                                                                     new PointF(235.096085f, 436.805603f),
			                                                                     new PointF(232.337158f, 436.369965f),
			                                                                     new PointF(226.81929f, 440.871399f),
			                                                                     new PointF(221.011002f, 443.920746f),
			                                                                     new PointF(214.767105f, 445.953644f),
			                                                                     new PointF(205.328644f, 448.712585f),
			                                                                     new PointF(198.503906f, 450.890686f),
			                                                                     new PointF(190.662735f, 454.811279f),
			                                                                     new PointF(184.709244f, 460.764771f),
			                                                                     new PointF(183.257172f, 464.975769f),
			                                                                     new PointF(162.928162f, 442.904236f));

			#endregion

			#region Shape Real05

			/// <summary>
			/// Anatomy outline shape for <see cref="ImageKey.Real05"/>.
			/// </summary>
			public static readonly PolygonShape ShapeOnReal05 = new PolygonShape("REAL05", true,
			                                                                     new PointF(229.073303f, 50.312508f),
			                                                                     new PointF(225.507919f, 56.795002f),
			                                                                     new PointF(224.85968f, 60.900581f),
			                                                                     new PointF(228.641129f, 67.383072f),
			                                                                     new PointF(230.585876f, 69.976067f),
			                                                                     new PointF(229.82959f, 77.322891f),
			                                                                     new PointF(228.749176f, 84.993843f),
			                                                                     new PointF(227.992889f, 94.501495f),
			                                                                     new PointF(228.425049f, 104.657402f),
			                                                                     new PointF(239.013123f, 110.815773f),
			                                                                     new PointF(245.387573f, 115.677635f),
			                                                                     new PointF(251.978104f, 118.594757f),
			                                                                     new PointF(256.947998f, 120.431465f),
			                                                                     new PointF(264.72699f, 123.456627f),
			                                                                     new PointF(269.588867f, 123.888794f),
			                                                                     new PointF(272.614044f, 121.619926f),
			                                                                     new PointF(276.07135f, 116.866096f),
			                                                                     new PointF(279.528687f, 111.680099f),
			                                                                     new PointF(281.149323f, 106.71019f),
			                                                                     new PointF(281.149323f, 101.63224f),
			                                                                     new PointF(280.284973f, 95.25779f),
			                                                                     new PointF(273.910522f, 87.586838f),
			                                                                     new PointF(268.940613f, 82.076721f),
			                                                                     new PointF(274.126617f, 81.428467f),
			                                                                     new PointF(276.395477f, 77.971138f),
			                                                                     new PointF(278.7724f, 72.136902f),
			                                                                     new PointF(274.774872f, 65.654404f),
			                                                                     new PointF(268.616486f, 62.197079f),
			                                                                     new PointF(265.159149f, 59.60408f),
			                                                                     new PointF(259.75708f, 53.445713f),
			                                                                     new PointF(255.43544f, 49.988384f),
			                                                                     new PointF(246.035812f, 48.15168f),
			                                                                     new PointF(235.879913f, 46.855179f),
			                                                                     new PointF(231.126083f, 48.90797f),
			                                                                     new PointF(229.073288f, 50.312504f));

			#endregion

			#region Shape Real06

			/// <summary>
			/// Anatomy outline shape for <see cref="ImageKey.Real06"/>.
			/// </summary>
			public static readonly PolygonShape ShapeOnReal06 = new PolygonShape("REAL06", true,
			                                                                     new PointF(138.136063f, 52.520763f),
			                                                                     new PointF(137.575851f, 59.865772f),
			                                                                     new PointF(137.887085f, 61.857639f),
			                                                                     new PointF(140.19017f, 66.961792f),
			                                                                     new PointF(142.555511f, 70.6343f),
			                                                                     new PointF(143.862686f, 74.182312f),
			                                                                     new PointF(146.974976f, 77.668076f),
			                                                                     new PointF(150.460739f, 78.601768f),
			                                                                     new PointF(153.448532f, 79.348717f),
			                                                                     new PointF(159.299652f, 78.850746f),
			                                                                     new PointF(162.411942f, 78.53952f),
			                                                                     new PointF(165.46199f, 77.668076f),
			                                                                     new PointF(169.321228f, 76.236427f),
			                                                                     new PointF(172.309021f, 75.987442f),
			                                                                     new PointF(177.911148f, 75.987442f),
			                                                                     new PointF(184.509201f, 76.609901f),
			                                                                     new PointF(191.16951f, 78.228294f),
			                                                                     new PointF(199.510452f, 79.597702f),
			                                                                     new PointF(205.486053f, 78.912994f),
			                                                                     new PointF(210.839188f, 77.979309f),
			                                                                     new PointF(213.017792f, 77.045616f),
			                                                                     new PointF(213.951492f, 73.74659f),
			                                                                     new PointF(214.698441f, 69.078156f),
			                                                                     new PointF(215.632126f, 63.662769f),
			                                                                     new PointF(216.254578f, 60.92395f),
			                                                                     new PointF(215.009659f, 57.126957f),
			                                                                     new PointF(212.270844f, 54.574875f),
			                                                                     new PointF(206.793213f, 54.325893f),
			                                                                     new PointF(198.452271f, 54.82386f),
			                                                                     new PointF(192.787903f, 54.886105f),
			                                                                     new PointF(183.513275f, 54.82386f),
			                                                                     new PointF(178.907089f, 55.695301f),
			                                                                     new PointF(174.612122f, 55.633057f),
			                                                                     new PointF(168.63652f, 54.574875f),
			                                                                     new PointF(160.669052f, 53.32996f),
			                                                                     new PointF(154.63121f, 51.089111f),
			                                                                     new PointF(147.84642f, 50.715633f),
			                                                                     new PointF(143.177979f, 49.968685f),
			                                                                     new PointF(140.563644f, 50.093178f),
			                                                                     new PointF(138.696274f, 51.462585f),
			                                                                     new PointF(138.136032f, 52.520756f));

			#endregion

			#region Implementation

			private readonly PointF[] _data;
			private readonly string _name;

			private PolygonShape(string name, params PointF[] vertices)
			{
				_name = name;
				_data = vertices;
			}

			private PolygonShape(string name, bool roundOffVertices, params PointF[] vertices)
				: this(name, vertices)
			{
				if (roundOffVertices)
				{
					for (int n = 0; n < _data.Length; n++)
						_data[n] = Point.Round(_data[n]);
				}
			}

			public PointF this[int index]
			{
				get { return _data[index]; }
			}

			/// <summary>
			/// Gets a polygon shape definition consisting of the points of the current shape but in the reverse ordering.
			/// </summary>
			public PolygonShape CreateAntiShape()
			{
				PointF[] antiData = new PointF[_data.Length];
				for (int n = 0; n < _data.Length; n++)
					antiData[_data.Length - n - 1] = _data[n];
				return new PolygonShape(string.Format("ANTI_{0}", _name), antiData);
			}

			public IEnumerator<PointF> GetEnumerator()
			{
				foreach (PointF data in _data)
					yield return data;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			public override string ToString()
			{
				return _name;
			}

			#endregion
		}
	}
}

#endif