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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Operations
{
	public class PresetVoiLutConfigurationAttribute : Attribute
	{
		public PresetVoiLutConfigurationAttribute()
		{
		}
	}

	[Serializable]
	public sealed class PresetVoiLutOperationValidationException : Exception
	{
		internal PresetVoiLutOperationValidationException(string message)
			: base(message)
		{
		}
	}

	public abstract class PresetVoiLutOperationComponent : ApplicationComponent, IPresetVoiLutOperation, IPresetVoiLutOperationComponent
	{
		private IPresetVoiLutOperationFactory _sourceFactory;
		private EditContext _editContext;
		private bool _valid;

		protected PresetVoiLutOperationComponent()
		{
			_valid = false;
		}

		#region Sealed Off Application Component functionality

		public sealed override IActionSet ExportedActions
		{
			get
			{
				return base.ExportedActions;
			}
		}

		#endregion

		#region IPresetVoiLutOperation Members

		public abstract string Name { get; }
		public abstract string Description { get; }

		public IPresetVoiLutOperationFactory SourceFactory
		{
			get { return _sourceFactory; }
			internal set
			{
				Platform.CheckForNullReference(value, "SourceFactory");
				_sourceFactory = value;
			}
		}
		#region IImageOperation Members

		public IMemorable GetOriginator(IPresentationImage image)
		{
			if (image is IVoiLutProvider)
				return ((IVoiLutProvider) image).VoiLutManager;

			return null;
		}

		public virtual bool AppliesTo(IPresentationImage presentationImage)
		{
            return LutHelper.IsVoiLutEnabled(presentationImage) && GetOriginator(presentationImage) != null;
		}

		public abstract void Apply(IPresentationImage image);

		#endregion

		public PresetVoiLutConfiguration GetConfiguration()
		{
			Validate();

			PresetVoiLutConfiguration configuration = PresetVoiLutConfiguration.FromFactory(_sourceFactory);
			foreach (KeyValuePair<string, string> pair in SimpleSerializer.Deserialize<PresetVoiLutConfigurationAttribute>(this))
				configuration[pair.Key] = pair.Value;

			return configuration;
		}

		#endregion

		#region IEditPresetVoiLutApplicationComponent Members

		public IPresetVoiLutOperation GetOperation()
		{
			Validate();
			return this;
		}

		public EditContext EditContext
		{
			get { return _editContext; }
			set { _editContext = value; }
		}

		public bool Valid
		{
			get { return _valid; }
			protected set
			{
				if (_valid == value)
					return;

				_valid = value;
				NotifyPropertyChanged("Valid");
			}
		}

		#endregion

		public abstract void Validate();

		protected virtual void UpdateValid()
		{
		}

		protected void OnPropertyChanged(string propertyName)
		{
			UpdateValid();
			base.Modified = true;
			NotifyPropertyChanged(propertyName);
		}

		#region Helper Methods

		protected static PresetVoiLutOperationValidationException CreateValidationException(string message)
		{
			return new PresetVoiLutOperationValidationException(message);
		}

		#endregion
	}
}