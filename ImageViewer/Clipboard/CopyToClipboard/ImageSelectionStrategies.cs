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
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.StudyManagement;

#pragma warning disable 0419,1574,1587,1591

namespace ClearCanvas.ImageViewer.Clipboard.CopyToClipboard
{
	public partial class CopySubsetToClipboardComponent
	{
		internal struct Range
		{
			public Range(int start, int end)
			{
				Start = start;
				End = end;
			}

			public readonly int Start;
			public readonly int End;
		}

		internal class RangeImageSelectionStrategy : IImageSelectionStrategy
		{
			private readonly int _startValue;
			private readonly int _endValue;
			private readonly int _selectionInterval;
			private readonly bool _useInstanceNumbers;

			public RangeImageSelectionStrategy(int startValue, int endValue, int selectionInterval, bool useInstanceNumbers)
			{
				if (!useInstanceNumbers)
				{
					Platform.CheckPositive(startValue, "startValue");
					Platform.CheckPositive(endValue, "endValue");
				}
				else
				{
					Platform.CheckNonNegative(startValue, "startValue");
					Platform.CheckNonNegative(endValue, "endValue");
				}

				Platform.CheckPositive(selectionInterval, "selectionInterval");

				if (endValue < startValue)
					throw new ArgumentException("End value must be greater than or equal to start value.");

				_startValue = startValue;
				_endValue = endValue;
				_selectionInterval = selectionInterval;
				_useInstanceNumbers = useInstanceNumbers;
			}

			#region IImageSubsetSelectionStrategy Members

			public string Description
			{
				get { return SR.DescriptionSubsetRange; }
			}

			public IEnumerable<IPresentationImage> GetImages(IDisplaySet displaySet)
			{
				if (_useInstanceNumbers)
				{
					foreach (IPresentationImage image in displaySet.PresentationImages)
					{
						if (image is IImageSopProvider)
						{
							IImageSopProvider provider = (IImageSopProvider)image;
							
							int maxValue = _endValue - _startValue;
							int testValue = provider.ImageSop.InstanceNumber - _startValue;

							if (testValue >= 0 && testValue <= maxValue && (0 == testValue % _selectionInterval))
								yield return image;
						}
					}
				}
				else
				{
					//selection indices are 1-based.
					for (int i = _startValue - 1; i < _endValue && i < displaySet.PresentationImages.Count; i += _selectionInterval)
						yield return displaySet.PresentationImages[i];
				}
			}

			#endregion
		}

		internal class CustomImageSelectionStrategy : IImageSelectionStrategy
		{
			private readonly List<Range> _ranges;
			private readonly bool _useInstanceNumbers;

			public CustomImageSelectionStrategy(string custom, int rangeMin, int rangeMax, bool useInstanceNumbers)
			{
				Platform.CheckForEmptyString(custom, "custom");

				if (!Parse(custom, rangeMin, rangeMax, out _ranges))
					throw new ArgumentException(String.Format("Invalid custom range string ({0}).", custom));

				_useInstanceNumbers = useInstanceNumbers;
			}

			public static bool Parse(string customRanges, int rangeMin, int rangeMax, out List<Range> parsedRanges)
			{
				customRanges = (customRanges ?? "").Trim();
				parsedRanges = new List<Range>();

				if (rangeMin > rangeMax)
					return false;

				if (String.IsNullOrEmpty(customRanges))
					return false;

				string[] ranges = customRanges.Split(new char[] { ',' }, StringSplitOptions.None);
				foreach (string range in ranges)
				{
					if (String.IsNullOrEmpty(range))
					{
						parsedRanges.Clear();
						return false;
					}

					int start, end;
					if (!ParseRange(range, rangeMin, rangeMax, out start, out end))
					{
						parsedRanges.Clear();
						return false;
					}

					parsedRanges.Add(new Range(start, end));
				}

				return true;
			}

			private static bool ParseRange(string range, int rangeMin, int rangeMax, out int start, out int end)
			{
				start = end = -1;

				string[] splitRange = range.Trim().Split(new char[] { '-' }, StringSplitOptions.None);
				if (splitRange.Length == 0 || splitRange.Length > 2)
					return false;

				if (splitRange.Length == 1)
				{
					if (!int.TryParse(splitRange[0], out start))
						return false;

					end = start;
				}
				else if (splitRange.Length == 2)
				{
					string splitStart = splitRange[0].Trim();
					string splitEnd = splitRange[1].Trim();

					bool startValid = String.IsNullOrEmpty(splitStart) || int.TryParse(splitStart, out start);
					bool endValid = String.IsNullOrEmpty(splitEnd) || int.TryParse(splitEnd, out end);

					if ((!startValid && !endValid) || (String.IsNullOrEmpty(splitStart) && String.IsNullOrEmpty(splitEnd)))
						return false;

					if (String.IsNullOrEmpty(splitStart))
						start = rangeMin;
					else if (String.IsNullOrEmpty(splitEnd))
						end = rangeMax;
				}

				return start > 0 && end >= start && end <= rangeMax;
			}

			#region IImageSubsetSelectionStrategy Members

			public string Description
			{
				get { return SR.DescriptionSubsetCustom; }
			}

			public IEnumerable<IPresentationImage> GetImages(IDisplaySet displaySet)
			{
				List<IPresentationImage> images = new List<IPresentationImage>();

				if (_useInstanceNumbers)
				{
					foreach (IPresentationImage image in displaySet.PresentationImages)
					{
						if (!images.Contains(image))
						{
							if (image is IImageSopProvider)
							{
								int instanceNumber = ((IImageSopProvider)image).ImageSop.InstanceNumber;
								foreach (Range range in _ranges)
								{
									if (instanceNumber >= range.Start && instanceNumber <= range.End)
										images.Add(image);
								}
							}
						}
					}
				}
				else
				{
					foreach (Range range in _ranges)
					{
						for (int j = range.Start - 1; j <= range.End - 1; ++j)
						{
							if (j >= displaySet.PresentationImages.Count)
								break;

							if (!images.Contains(displaySet.PresentationImages[j]))
								images.Add(displaySet.PresentationImages[j]);
						}
					}
				}

				return images;
			}

			#endregion
		}
	}
}