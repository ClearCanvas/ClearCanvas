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
using System.Collections.ObjectModel;
using System.Drawing;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.Common;

namespace ClearCanvas.Dicom.Network.Scu
{
	public partial class PrintScu
	{
		/// <summary>
		/// Delegate for creating a filmBox.
		/// </summary>
		/// <returns></returns>
		public delegate FilmBox CreateFilmBoxDelegate(IList<IPrintItem> currentQueue);

		public delegate void GetPixelDataDelegate(ImageBox imageBox, ColorMode colorMode, out ushort rows, out ushort columns, out byte[] pixelData);

		public interface IPrintItem
		{
			void GetPixelData(ImageBox imageBox, ColorMode colorMode, out ushort rows, out ushort columns, out byte[] pixelData);
		}

		public class PrintItem : IPrintItem
		{
			public PrintItem()
			{
			}

			public PrintItem(GetPixelDataDelegate getPixelDataCallback)
			{
				GetPixelDataCallback = getPixelDataCallback;
			}

			public object PrintObject { get; set; }
			public GetPixelDataDelegate GetPixelDataCallback { get; set; }

			#region IPrintItem Members

			public void GetPixelData(ImageBox imageBox, ColorMode colorMode, out ushort rows, out ushort columns, out byte[] pixelData)
			{
				Platform.CheckMemberIsSet(GetPixelDataCallback, "GetPixelDataCallback");
				GetPixelDataCallback(imageBox, colorMode, out rows, out columns, out pixelData);
			}

			#endregion
		}

		public class FilmSession : BasicFilmSessionModuleIod
		{
			private readonly CreateFilmBoxDelegate _createFilmBoxCallback;

			private readonly List<FilmBox> _filmBoxes;
			private FilmBox _currentFilmBox;

			private readonly ReadOnlyCollection<IPrintItem> _printItems;
			private readonly Queue<IPrintItem> _printItemQueue;

			public FilmSession(List<IPrintItem> printItems, CreateFilmBoxDelegate createFilmBoxCallback)
			{
				_createFilmBoxCallback = createFilmBoxCallback;
				_filmBoxes = new List<FilmBox>();

				_printItems = printItems.AsReadOnly();
				_printItemQueue = new Queue<IPrintItem>(printItems);
			}

			internal DicomUid SopInstanceUid { get; set; }
			internal PrintScu PrintScu { get; set; }

			public ReadOnlyCollection<IPrintItem> PrintItems { get { return _printItems; } }

			public ReadOnlyCollection<FilmBox> FilmBoxes
			{
				get { return _filmBoxes.AsReadOnly(); }
			}

			internal void OnCreated(DicomUid filmSessionUid)
			{
				this.SopInstanceUid = filmSessionUid;

				// Move to the first element.
				_filmBoxes.Add(_currentFilmBox = _createFilmBoxCallback.Invoke(new List<IPrintItem>(_printItemQueue).AsReadOnly()));
				this.PrintScu.CreateFilmBox(this, _currentFilmBox);
			}

			internal void OnFilmBoxCreated(DicomUid filmBoxUid, List<DicomUid> imageBoxUids)
			{
				_currentFilmBox.SopInstanceUid = filmBoxUid;

				// The SCP returns a list of imageBoxUids.  Create an imageBox for each UID.
				var imageBoxes = new List<ImageBox>();
				for (var i = 0; i < imageBoxUids.Count; i++)
				{
					var imageBox = new ImageBox(_currentFilmBox, _printItemQueue.Dequeue())
						{
							ImageBoxPosition = (ushort) (i+1),  // position is 1-based
							SopInstanceUid = imageBoxUids[i]
						};
					imageBoxes.Add(imageBox);

					// No more print items.  Stop creating imageBoxes
					if (_printItemQueue.Count == 0)
						break;
				}

				// start setting the first imageBox
				_currentFilmBox.SetImageBoxes(imageBoxes);
				var imageBoxToSet = _currentFilmBox.GetNextImageBox();
				imageBoxToSet.BeforeSet(this.PrintScu.ColorMode);
				this.PrintScu.SetImageBox(imageBoxToSet);
				imageBoxToSet.AfterSet();
			}

			internal void OnImageBoxSet(DicomUid imageBoxUid)
			{
				var imageBoxToSet = _currentFilmBox.GetNextImageBox();
				if (imageBoxToSet == null)
				{
					// No more imageBox to set.  Print the filmBox.
					this.PrintScu.PrintFilmBox(_currentFilmBox);
				}
				else
				{
					imageBoxToSet.BeforeSet(this.PrintScu.ColorMode);
					this.PrintScu.SetImageBox(imageBoxToSet);
					imageBoxToSet.AfterSet();
				}
			}

			internal void OnFilmBoxPrinted(DicomUid filmBoxUid)
			{
				this.PrintScu.DeleteFilmBox(_currentFilmBox);
			}

			internal void OnFilmBoxDeleted()
			{
				_currentFilmBox.SopInstanceUid = null;

				if (_printItemQueue.Count == 0)
				{
					// No more items to create filmBox for.
					this.PrintScu.DeleteFilmSession(this);
				}
				else
				{
					// Create the next filmBox
					_filmBoxes.Add(_currentFilmBox = _createFilmBoxCallback.Invoke(new List<IPrintItem>(_printItemQueue).AsReadOnly()));
					this.PrintScu.CreateFilmBox(this, _currentFilmBox);
				}
			}

			internal void OnDeleted()
			{
				this.SopInstanceUid = null;
				this.PrintScu = null;
			}
		}

		public class FilmBox : BasicFilmBoxModuleIod
		{
			internal DicomUid SopInstanceUid { get; set; }

			private List<ImageBox> _imageBoxes;
			private List<ImageBox>.Enumerator _imageBoxEnumerator;

			private readonly int _standardResolutionDPI;
			private readonly int _highResolutionDPI;

			public FilmBox(int standardResolutionDPI, int highResolutionDPI)
			{
				_standardResolutionDPI = standardResolutionDPI;
				_highResolutionDPI = highResolutionDPI;
			}

			public ReadOnlyCollection<ImageBox> ImageBox
			{
				get { return _imageBoxes.AsReadOnly(); }
			}

			public int FilmDPI
			{
				get
				{
					return this.RequestedResolutionId == RequestedResolution.High
						? _highResolutionDPI
						: _standardResolutionDPI;
				}
			}

			public Size SizeInPixels
			{
				get
				{
					if (this.FilmSizeId == null)
						return new Size();

					var physicalWidthInInches = this.FilmSizeId.GetWidth(FilmSize.FilmSizeUnit.Inch);
					var physicalHeightInInches = this.FilmSizeId.GetHeight(FilmSize.FilmSizeUnit.Inch);

					var width = (int)Math.Ceiling(physicalWidthInInches * this.FilmDPI);
					var height = (int)Math.Ceiling(physicalHeightInInches * this.FilmDPI);

					return this.FilmOrientation == FilmOrientation.Landscape
						? new Size(height, width)
						: new Size(width, height); // default portrait, even if the value is None
				}
			}

			internal ImageBox GetNextImageBox()
			{
				return _imageBoxEnumerator.MoveNext() ? _imageBoxEnumerator.Current : null;
			}

			internal void SetImageBoxes(List<ImageBox> imageBoxes)
			{
				_imageBoxes = imageBoxes;
				_imageBoxEnumerator = _imageBoxes.GetEnumerator();
			}
		}

		public class ImageBox : ImageBoxPixelModuleIod
		{
			internal DicomUid SopInstanceUid;

			public FilmBox FilmBox { get; private set; }
			public IPrintItem PrintItem { get; private set; }

			public ImageBox(FilmBox filmBox, IPrintItem printItem)
			{
				this.FilmBox = filmBox;
				this.PrintItem = printItem;
			}

			/// <summary>
			/// Gets the physical width of this imageBox in millimeters.  The value is accurate only if the <see cref="FilmBox.FilmDPI"/> property is 
			/// configured to that of the actual printer.
			/// </summary>
			public float PhysicalWidth
			{
				get
				{
					var filmPixelSpacing = LengthInMillimeter.Inch / this.FilmBox.FilmDPI;
					return this.SizeInPixels.Width * filmPixelSpacing;
				}
			}

			/// <summary>
			/// Get the size in pixel.  The value depends on the image position, filmBox size, filmBox orentation and filmDPI.
			/// This property assumes that the spacing for each rows/columns of imageBoxes on a film are evenly divided.
			/// </summary>
			public Size SizeInPixels
			{
				get
				{
					var filmBoxSize = this.FilmBox.SizeInPixels;
					var imageDisplayFormat = this.FilmBox.ImageDisplayFormat;

					switch (imageDisplayFormat.Format)
					{
						case ImageDisplayFormat.FormatEnum.STANDARD:
							{
								var numberOfCols = imageDisplayFormat.Modifiers[0];
								var numberOfRows = imageDisplayFormat.Modifiers[1];
								// Size of rows and columns are uniform, and equals to total width or height divide by the count in either dimension
								return new Size(filmBoxSize.Width / numberOfCols, filmBoxSize.Height / numberOfRows);
							}

						case ImageDisplayFormat.FormatEnum.ROW:
							{
								// Major row order: left-to-right and top-to-bottom
								int rowIndex, colIndex;
								GetRowColumnIndex(imageDisplayFormat, this.ImageBoxPosition, out rowIndex, out colIndex);

								var numberOfRows = imageDisplayFormat.Modifiers.Count;
								var numberOfCols = imageDisplayFormat.Modifiers[rowIndex];  // # of columns for the row the imageBox is in
								return new Size(filmBoxSize.Width / numberOfCols, filmBoxSize.Height / numberOfRows);
							}

						case ImageDisplayFormat.FormatEnum.COL:
							{
								// Major column order: top-to-bottom and left-to-right
								int rowIndex, colIndex;
								GetRowColumnIndex(imageDisplayFormat, this.ImageBoxPosition, out rowIndex, out colIndex);

								var numberOfCols = imageDisplayFormat.Modifiers.Count;
								var numberOfRows = imageDisplayFormat.Modifiers[colIndex];  // # of rows for the column the imageBox is in
								return new Size(filmBoxSize.Width / numberOfCols, filmBoxSize.Height / numberOfRows);
							}

						case ImageDisplayFormat.FormatEnum.SLIDE:
						case ImageDisplayFormat.FormatEnum.SUPERSLIDE:
						case ImageDisplayFormat.FormatEnum.CUSTOM:
						default:
							break;
					}

					throw new NotSupportedException(string.Format("{0} image display format is not supported", imageDisplayFormat.Format));
				}
			}

			internal void BeforeSet(ColorMode colorMode)
			{
				byte[] pixelData;
				ushort rows;
				ushort columns;

				if (colorMode == ColorMode.Color)
				{
					var image = new BasicColorImageSequenceIod
						{
							SamplesPerPixel = 3,
							PhotometricInterpretation = PhotometricInterpretation.Rgb,
							PixelRepresentation = 0,
							PixelAspectRatio = new PixelAspectRatio(1, 1),
							PlanarConfiguration = 0,
							BitsStored = 8,
							BitsAllocated = 8,
							HighBit = 7
						};

					this.PrintItem.GetPixelData(this, colorMode, out rows, out columns, out pixelData);
					image.PixelData = pixelData;
					image.Rows = rows;
					image.Columns = columns;

					this.BasicColorImageSequenceList.Add(image);
				}
				else
				{
					var image = new BasicGrayscaleImageSequenceIod
						{
							SamplesPerPixel = 1,
							PhotometricInterpretation = PhotometricInterpretation.Monochrome2,
							PixelRepresentation = 0,
							PixelAspectRatio = new PixelAspectRatio(1, 1),
							BitsStored = 8,
							BitsAllocated = 8,
							HighBit = 7
						};

					this.PrintItem.GetPixelData(this, colorMode, out rows, out columns, out pixelData);
					image.PixelData = pixelData;
					image.Rows = rows;
					image.Columns = columns;

					this.BasicGrayscaleImageSequenceList.Add(image);
				}
			}

			internal void AfterSet()
			{
				//Free up the pixel data immediately, rather than keeping it around in the data set.
				//Otherwise, we end up keeping *all* images around until the print session ends.
				this.BasicGrayscaleImageSequenceList.Clear();
			}

			#region Private helper methods

			/// <summary>
			/// Get the rowIndex and columnIndex of the specified imageBoxPosition for this ImageDisplayType.
			/// </summary>
			private static void GetRowColumnIndex(ImageDisplayFormat imageDisplayFormat, int imageBoxPosition, out int rowIndex, out int columnIndex)
			{
				rowIndex = 0;
				columnIndex = 0;

				switch (imageDisplayFormat.Format)
				{
					case ImageDisplayFormat.FormatEnum.STANDARD:
						{
							var numberOfColumns = imageDisplayFormat.Modifiers[0];
							var numberOfRows = imageDisplayFormat.Modifiers[1];
							var imageBoxIndex = imageBoxPosition - 1;
							rowIndex = imageBoxIndex / numberOfRows;
							columnIndex = imageBoxIndex % numberOfColumns;
							break;
						}

					case ImageDisplayFormat.FormatEnum.ROW:
						{
							// Major row order: left-to-right and top-to-bottom
							GetIndexForRowColumnFormat(imageDisplayFormat.Modifiers, imageBoxPosition, out rowIndex, out columnIndex);
							break;
						}

					case ImageDisplayFormat.FormatEnum.COL:
						{
							// Major column order: top-to-bottom and left-to-right
							GetIndexForRowColumnFormat(imageDisplayFormat.Modifiers, imageBoxPosition, out columnIndex, out rowIndex);
							break;
						}

					case ImageDisplayFormat.FormatEnum.SLIDE:
					case ImageDisplayFormat.FormatEnum.SUPERSLIDE:
					case ImageDisplayFormat.FormatEnum.CUSTOM:
					default:
						throw new NotSupportedException(string.Format("{0} image display format is not supported", imageDisplayFormat.Format));
				}
			}

			private static void GetIndexForRowColumnFormat(IList<int> imageBoxesPerLines, int imageBoxPosition, out int firstIndex, out int secondIndex)
			{
				firstIndex = 0;
				secondIndex = 0;

				var numberOfImageBoxesBeforeCurrentLine = 0;
				var numberOfLines = imageBoxesPerLines.Count;
				for (var lineIndex = 0; lineIndex < numberOfLines; lineIndex++)
				{
					var numberOfImageBoxesIncludingCurrentLine = numberOfImageBoxesBeforeCurrentLine + imageBoxesPerLines[lineIndex];
					if (imageBoxPosition <= numberOfImageBoxesIncludingCurrentLine)
					{
						// Image is in current line
						firstIndex = lineIndex;
						secondIndex = imageBoxPosition - numberOfImageBoxesBeforeCurrentLine - 1;

						break;
					}

					// Advance the total imageBox count
					numberOfImageBoxesBeforeCurrentLine = numberOfImageBoxesIncludingCurrentLine;
				}
			}

			#endregion
		}
	}
}
