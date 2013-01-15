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

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Operations
{
	internal static class PresetVoiLutOperationFactories
	{
		private static List<IPresetVoiLutOperationFactory> _factories;

		private static List<IPresetVoiLutOperationFactory> InternalFactories
		{
			get
			{
				if (_factories == null)
				{
					_factories = new List<IPresetVoiLutOperationFactory>();

					_factories.Add(new LinearPresetVoiLutOperationFactory());
					//TODO: Uncomment this to re-enable multi-type preset lut functionality.
					//_factories.Add(new MinMaxAlgorithmPresetVoiLutOperationFactory());
					//_factories.Add(new AutoPresetVoiLutOperationFactory());

					PresetVoiLutOperationFactoryExtensionPoint xp = new PresetVoiLutOperationFactoryExtensionPoint();
					
					object[] factories = xp.CreateExtensions();
					foreach (object factory in factories)
					{
						if (factory is IPresetVoiLutOperationFactory)
						{
							IPresetVoiLutOperationFactory newFactory = (IPresetVoiLutOperationFactory)factory;
							if (!String.IsNullOrEmpty(newFactory.Name))
								_factories.Add(newFactory);
							else
								Platform.Log(LogLevel.Warn, SR.MessageFormatFactoryHasNoName, factory.GetType().FullName);
						}
						else
							Platform.Log(LogLevel.Warn, SR.MessageFormatFactoryDoesNotImplementRequiredInterface, factory.GetType().FullName, typeof(IPresetVoiLutOperationFactory).Name);
					}
				}

				return _factories;
			}
		}

		public static IEnumerable<IPresetVoiLutOperationFactory> Factories
		{
			get { return InternalFactories; }
		}

		public static IPresetVoiLutOperationFactory GetFactory(string factoryName)
		{
			return InternalFactories.Find(delegate(IPresetVoiLutOperationFactory factory) { return factory.Name == factoryName; });
		}
	}
}