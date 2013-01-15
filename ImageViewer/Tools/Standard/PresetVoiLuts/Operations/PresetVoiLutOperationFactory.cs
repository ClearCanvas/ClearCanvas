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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Operations
{
    public sealed class PresetVoiLutOperationFactoryExtensionPoint : ExtensionPoint<IPresetVoiLutOperationFactory>
    {
    }

    public interface IPresetVoiLutOperationFactory
    {
        string Name { get; }
        string Description { get; }

        bool CanCreateMultiple { get; }

        IPresetVoiLutOperation Create(PresetVoiLutConfiguration configuration);
        IPresetVoiLutOperationComponent GetEditComponent(PresetVoiLutConfiguration configuration);
    }

    public abstract class PresetVoiLutOperationFactory<PresetVoiLutOperationComponentType> : IPresetVoiLutOperationFactory
		where PresetVoiLutOperationComponentType : PresetVoiLutOperationComponent, new()
	{
		#region IPresetVoiLutOperationFactory Members

		public abstract string Name { get; }
		public abstract string Description { get; }

		public bool CanCreateMultiple
		{
			get { return typeof (PresetVoiLutOperationComponentType).IsDefined(typeof(AllowMultiplePresetVoiLutOperationsAttribute), false); }
		}

		public IPresetVoiLutOperation Create(PresetVoiLutConfiguration configuration)
		{
			Platform.CheckForNullReference(configuration, "configuration");

			ValidateFactoryName(configuration);

			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			configuration.CopyTo(dictionary);

			PresetVoiLutOperationComponentType component = new PresetVoiLutOperationComponentType();
			component.SourceFactory = this;
			SimpleSerializer.Serialize<PresetVoiLutConfigurationAttribute>(component, dictionary);
			component.Validate();
			return component;
		}

		public IPresetVoiLutOperationComponent GetEditComponent(PresetVoiLutConfiguration configuration)
		{
			PresetVoiLutOperationComponentType component = new PresetVoiLutOperationComponentType();
			component.SourceFactory = this;

			if (configuration != null)
			{
				ValidateFactoryName(configuration);
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				configuration.CopyTo(dictionary);
				SimpleSerializer.Serialize<PresetVoiLutConfigurationAttribute>(component, dictionary);
			}

			return component;
		}

		#endregion

		private void ValidateFactoryName(PresetVoiLutConfiguration configuration)
		{
			if (configuration.FactoryName != this.Name)
			{
				throw new ArgumentException(
					String.Format(SR.ExceptionFormatFactoryNamesDoNotMatch, configuration.FactoryName, this.Name));
			}
		}
	}
}
