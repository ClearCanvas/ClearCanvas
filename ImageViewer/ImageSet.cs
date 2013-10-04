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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A container for <see cref="IDisplaySet"/> objects.
	/// </summary>
	public class ImageSet : IImageSet
	{
		#region Private fields

		private DisplaySetCollection _displaySets = new DisplaySetCollection();
		private LogicalWorkspace _parentLogicalWorkspace;
		private IImageViewer _imageViewer;
		private ImageSetDescriptor _descriptor;
		private event EventHandler _drawing;
		private List<IDisplaySet> _displaySetCopies;

		#endregion
		
		/// <summary>
		/// Initializes a new instance of <see cref="ImageSet"/>.
		/// </summary>
		public ImageSet()
			: this(new BasicImageSetDescriptor())
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="ImageSet"/>.
		/// </summary>
		public ImageSet(ImageSetDescriptor descriptor)
		{
			_displaySetCopies = new List<IDisplaySet>();

			_displaySets.ItemAdded += OnDisplaySetAdded;
			_displaySets.ItemChanging += OnDisplaySetChanging;
			_displaySets.ItemRemoved += OnDisplaySetRemoved;
			_displaySets.ItemChanged += OnDisplaySetChanged;

			Descriptor = descriptor;
            ExtensionData = new ExtensionData();
		}

		internal void AddCopy(IDisplaySet copy)
		{
			_displaySetCopies.Add(copy);
		}

		internal void RemoveCopy(IDisplaySet copy)
		{
			if (_displaySetCopies != null)
				_displaySetCopies.Remove(copy);
		}

		internal IEnumerable<IDisplaySet> GetCopies()
		{
			foreach (IDisplaySet copy in _displaySetCopies)
				yield return copy;
		}

		#region IImageSet Members

		/// <summary>
		/// Gets the associated <see cref="IImageViewer"/>.
		/// </summary>
		/// <value>The associated <see cref="IImageViewer"/> or <b>null</b> if the 
		/// <see cref="ImageSet"/> is not part of the 
		/// logical workspace yet.</value>
		public IImageViewer ImageViewer
		{
			get { return _imageViewer; }
			internal set
			{
				_imageViewer = value;

				if (_imageViewer != null)
				{
					foreach (DisplaySet displaySet in this.DisplaySets)
						displaySet.ImageViewer = value;

					foreach (DisplaySet copy in _displaySetCopies)
						copy.ImageViewer = value;
				}
			}
		}

		/// <summary>
		/// Gets the parent <see cref="LogicalWorkspace"/>
		/// </summary>
		/// <value>The parent <see cref="ILogicalWorkspace"/> or <b>null</b> if the 
		/// <see cref="ImageSet"/> has not been added to an 
		/// <see cref="ILogicalWorkspace"/> yet.</value>
		public ILogicalWorkspace ParentLogicalWorkspace
		{
			get { return _parentLogicalWorkspace; }
			internal set { _parentLogicalWorkspace = value as LogicalWorkspace; }
		}

		/// <summary>
		/// Gets the collection of <see cref="IDisplaySet"/> objects that belong
		/// to this <see cref="ImageSet"/>.
		/// </summary>
		public DisplaySetCollection DisplaySets
		{
			get { return _displaySets; }
		}

		/// <summary>
		/// Gets a collection of linked <see cref="IDisplaySet"/> objects.
		/// </summary>
		public IEnumerable<IDisplaySet> LinkedDisplaySets
		{
			get
			{
				foreach (IDisplaySet displaySet in DisplaySets)
				{
					if (displaySet.Linked)
						yield return displaySet;
				}

				foreach (DisplaySet copy in _displaySetCopies)
				{
					if (copy.Linked)
						yield return copy;
				}
			}
		}

		IImageSetDescriptor IImageSet.Descriptor
		{
			get { return _descriptor; }
		}

		/// <summary>
		/// Gets the <see cref="IImageSetDescriptor"/> describing this <see cref="IImageSet"/>.
		/// </summary>
		public ImageSetDescriptor Descriptor
		{
			get { return _descriptor; }
			set
			{
				Platform.CheckForNullReference(value, "Descriptor");
				_descriptor = value;
			}
		}

		/// <summary>
		/// Gets or sets the name of the image set.
		/// </summary>
		public string Name
		{
			get { return _descriptor.Name; }
			set { _descriptor.Name = value; }
		}

		/// <summary>
		/// Gets or sets the patient information associated with the image set.
		/// </summary>
		public string PatientInfo
		{
			get { return _descriptor.PatientInfo; }
			set { _descriptor.PatientInfo = value; }
		}


		/// <summary>
		/// Gets or sets unique identifier for this <see cref="IImageSet"/>.
		/// </summary>
		public string Uid
		{
			get { return _descriptor.Uid; }
			set { _descriptor.Uid = value; }
		}

	    public ExtensionData ExtensionData { get; private set; }

	    /// <summary>
		/// Fires just before the <see cref="ImageSet"/> is actually drawn/rendered.
		/// </summary>
		public event EventHandler Drawing
		{
			add { _drawing += value; }
			remove { _drawing -= value; }
		}

		/// <summary>
		/// Draws the <see cref="ImageSet"/>.
		/// </summary>
		public void Draw()
		{
			OnDrawing();
			foreach (DisplaySet displaySet in this.DisplaySets)
				displaySet.Draw();

			foreach (DisplaySet copy in _displaySetCopies)
				copy.Draw();
		}

		#endregion

		/// <summary>
		/// Returns the name of the image set.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return _descriptor.ToString();
		}

		#region Disposal

		#region IDisposable Members

		/// <summary>
		/// Releases all resources used by this <see cref="ImageSet"/>.
		/// </summary>
		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				// shouldn't throw anything from inside Dispose()
				Platform.Log(LogLevel.Error, e);
			}
		}

		#endregion

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				DisposeDisplaySets();

				if (ExtensionData != null)
				{
					ExtensionData.Dispose();
					ExtensionData = null;
				}
			}
		}

		private void DisposeDisplaySets()
		{
			if (this.DisplaySets == null)
				return;

			List<IDisplaySet> displaySetCopies = _displaySetCopies;
			_displaySetCopies = null;

			foreach (DisplaySet copy in displaySetCopies)
				copy.Dispose();

			foreach (DisplaySet displaySet in this.DisplaySets)
				displaySet.Dispose();

			_displaySets.ItemAdded -= OnDisplaySetAdded;
			_displaySets.ItemChanging -= OnDisplaySetChanging;
			_displaySets.ItemRemoved -= OnDisplaySetRemoved;
			_displaySets.ItemChanged -= OnDisplaySetChanged;

			_displaySets = null;
		}

		#endregion

		private void OnDisplaySetAdded(object sender, ListEventArgs<IDisplaySet> e)
		{
			OnDisplaySetAdded((DisplaySet)e.Item);
		}


		private void OnDisplaySetChanged(object sender, ListEventArgs<IDisplaySet> e)
		{
			OnDisplaySetAdded((DisplaySet)e.Item);
		}

		private void OnDisplaySetChanging(object sender, ListEventArgs<IDisplaySet> e)
		{
			OnDisplaySetRemoved((DisplaySet)e.Item);
		}

		private void OnDisplaySetRemoved(object sender, ListEventArgs<IDisplaySet> e)
		{
			OnDisplaySetRemoved((DisplaySet)e.Item);
		}

		/// <summary>
		/// Called when a new <see cref="DisplaySet"/> has been added.
		/// </summary>
		protected virtual void OnDisplaySetAdded(DisplaySet displaySet)
		{
			displaySet.ParentImageSet = this;
			displaySet.ImageViewer = this.ImageViewer;
		}

		/// <summary>
		/// Called when a <see cref="DisplaySet"/> has been removed.
		/// </summary>
		protected virtual void OnDisplaySetRemoved(DisplaySet displaySet)
		{
			displaySet.ParentImageSet = null;
			displaySet.ImageViewer = null;
		}

		private void OnDrawing()
		{
			EventsHelper.Fire(_drawing, this, EventArgs.Empty);
		}
	}
}
