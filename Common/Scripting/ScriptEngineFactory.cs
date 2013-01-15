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
using ClearCanvas.Common.Utilities;
using System.Collections.Generic;

namespace ClearCanvas.Common.Scripting
{
	/// <summary>
	/// Extension point for <see cref="IScriptEngine"/>s.
	/// </summary>
	[ExtensionPoint]
	public sealed class ScriptEngineExtensionPoint : ExtensionPoint<IScriptEngine>
	{
	}

	/// <summary>
	/// Factory for creating instances of <see cref="IScriptEngine"/>s that support a given language.
	/// </summary>
	public static class ScriptEngineFactory
	{
		private static readonly Dictionary<string, IScriptEngine> _singletonEngineInstances = new Dictionary<string, IScriptEngine>();
		private static readonly object _syncLock = new object();

		/// <summary>
		/// Attempts to instantiate a script engine for the specified language. 
		/// </summary>
		/// <remarks>
		/// <para>
		/// Internally, this class looks for an extension of <see cref="ScriptEngineExtensionPoint"/> 
		/// that is capable of running scripts in the specified language.
		/// In order to be considered a match, extensions must be decorated with a 
		/// <see cref="LanguageSupportAttribute"/> matching the <paramref name="language"/> parameter.
		/// </para>
		/// <para>
		/// If the engine is marked as a singleton, and a cached instance already exists, that instance will
		/// be returned.
		/// </para>
		/// <para>
		/// This method can safely be called by multiple threads.
		/// </para>
		/// </remarks>
		/// <param name="language">The case-insensitive script language, so jscript is equivalent to JScript.</param>
		public static IScriptEngine GetEngine(string language)
		{
			lock (_syncLock)
			{
				// check for a cached singleton engine instance for this language
				IScriptEngine engine;
				if (_singletonEngineInstances.TryGetValue(language, out engine))
					return engine;

				// create a new engine instance
				engine = CreateEngine(language);

				var optionsAttr = AttributeUtils.GetAttribute<ScriptEngineOptionsAttribute>(engine.GetType());
				if (optionsAttr != null)
				{
					// if the engine requires synchronization, wrap it
					if (optionsAttr.SynchronizeAccess)
					{
						engine = new SynchronizedScriptEngineWrapper(engine);
					}

					// if the engine is a singleton, cache this instance
					if (optionsAttr.Singleton)
					{
						_singletonEngineInstances.Add(language, engine);
					}
				}
				return engine;
			}
		}

		private static IScriptEngine CreateEngine(string language)
		{
			try
			{
				var xp = new ScriptEngineExtensionPoint();
				return (IScriptEngine)xp.CreateExtension(new AttributeExtensionFilter(new LanguageSupportAttribute(language)));
			}
			catch (NotSupportedException e)
			{
				throw new NotSupportedException(string.Format(SR.ExceptionScriptEngineLanguage, language), e);
			}
		}
	}
}
