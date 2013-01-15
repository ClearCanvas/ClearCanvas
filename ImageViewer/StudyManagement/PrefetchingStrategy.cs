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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer;

namespace ClearCanvas.ImageViewer.StudyManagement
{
    // TODO (CR Mar 2012): Move this and associated classes to their own namespace.
	/// <summary>
	/// Abstract base class for <see cref="IPrefetchingStrategy"/>.
	/// </summary>
	public abstract class PrefetchingStrategy : IPrefetchingStrategy
	{
		private IImageViewer _imageViewer;
		private readonly string _name;
		private readonly string _description;

		/// <summary>
		/// Constructs a new <see cref="PrefetchingStrategy"/> with the given <paramref name="name"/>
		/// and <paramref name="description"/>.
		/// </summary>
		protected PrefetchingStrategy(string name, string description)
		{
			_name = name;
			_description = description;
		}

		/// <summary>
		/// Gets the <see cref="IImageViewer"/> for which data is to be prefetched.
		/// </summary>
		protected IImageViewer ImageViewer
		{
			get { return _imageViewer; }
		}

		/// <summary>
		/// Starts prefetching.
		/// </summary>
		protected abstract void Start();

		/// <summary>
		/// Stops prefetching.
		/// </summary>
		protected abstract void Stop();

		#region IPrefetchingStrategy Members

		/// <summary>
		/// Gets the friendly name of the prefetching strategy.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}

		/// <summary>
		/// Gets the friendly description of the prefetching strategy.
		/// </summary>
		public string Description
		{
			get { return _description; }
		}

		void IPrefetchingStrategy.Start(IImageViewer imageViewer)
		{
			Platform.CheckForNullReference(imageViewer, "imageViewer");

			//Only start if we haven't already been started.
			if (_imageViewer == null)
			{
				_imageViewer = imageViewer;
				Start();
			}
		}

		void IPrefetchingStrategy.Stop()
		{
			if(_imageViewer != null)
			{
				Stop();
				_imageViewer = null;
			}
		}

		#endregion
	}
}
