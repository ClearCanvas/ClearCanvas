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

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Common.ServerDirectory;

namespace ClearCanvas.ImageViewer.StudyManagement.Tests
{
	/// <summary>
	/// Represents a provider of <see cref="ISopDataSource"/>s in a unit testing context.
	/// </summary>
	public interface IUnitTestStudyProvider
	{
		/// <summary>
		/// Called to get <see cref="ISopDataSource"/>s matching the specified query arguments.
		/// </summary>
		/// <param name="server">The server from which the data sources are "retrieved."</param>
		/// <param name="studyInstanceUid">The study instance UID of the data sources to be "retrieved."</param>
		/// <returns>An enumerable of matching <see cref="ISopDataSource"/> results. May be null.</returns>
        IEnumerable<ISopDataSource> GetSopDataSources(IApplicationEntity server, string studyInstanceUid);
	}

    public class TestServiceNodeServiceProvider : ServiceNodeServiceProvider
    {
        public override bool IsSupported(Type type)
        {
            return (type == typeof (IStudyLoader));
        }

        public override object GetService(Type type)
        {
            if (type == typeof(IStudyLoader))
                return new UnitTestStudyLoader();
            return null;
        }
    }

	/// <summary>
	/// An implementation of <see cref="IStudyLoader"/> suitable for unit testing contexts.
	/// </summary>
	/// <remarks>
	/// <para>
	/// In order to use this mock extension, the class must be explicitly installed as an extension
	/// of the <see cref="StudyLoaderExtensionPoint"/> extension point before the unit test is
	/// executed. This can be accomplised by installing a custom <see cref="IExtensionFactory"/>, such
	/// as <see cref="UnitTestExtensionFactory"/>.
	/// </para>
	/// <para>
	/// Individual <see cref="ISopDataSource"/>s (or, alternatively, providers thereof) may then be
	/// registered with the <see cref="UnitTestStudyLoader"/>, and will be provided in response to any
	/// future requests by the ImageViewer framework for a study matching the study instance UID and
	/// the "server" object. This behaviour will continue until the providers/data sources are explicitly
	/// unregistered.
	/// </para>
	/// </remarks>
	/// <example lang="CS">
	/// <code><![CDATA[
	/// UnitTestExtensionFactory extensionFactory = new UnitTestExtensionFactory();
	/// extensionFactory.Define(typeof(StudyLoaderExtensionPoint), typeof(UnitTestStudyLoader));
	/// Platform.SetExtensionFactory(extensionFactory);
	/// 
	/// IEnumerable<ISopDataSource> myStudies = ...;
	/// using (UnitTestStudyLoader.UnitTestStudyProviderContext studyProviderContext = UnitTestStudyLoader.RegisterStudies(myStudies))
	/// {
	///     // perform tasks that use study loaders
	///     using (ImageViewerComponent viewer = new ImageViewerComponent())
	///     {
	///         viewer.LoadStudy(new LoadStudyArgs("1.2.840.10008.1.20.2.1", studyProviderContext.Server, UnitTestStudyLoader.StudyLoaderId));
	///         ...
	///     }
	/// }
	/// ]]></code>
	/// </example>
	public sealed class UnitTestStudyLoader : StudyLoader
	{
	    /// <summary>
		/// Gets the ID string of the <see cref="UnitTestStudyLoader"/>.
		/// </summary>
		public static readonly string StudyLoaderId = typeof (UnitTestStudyLoader).Name;

		private static readonly object _syncLock = new object();
		private static readonly IList<IUnitTestStudyProvider> _providers = new List<IUnitTestStudyProvider>();

		/// <summary>
		/// Registers a number of <see cref="ISopDataSource"/>s with the <see cref="UnitTestStudyLoader"/>.
		/// </summary>
		/// <remarks>
		/// <para>Dispose the returned context to unregister the data sources.</para>
		/// <para>This method is thread safe.</para>
		/// </remarks>
		/// <param name="sopDataSources">The <see cref="ISopDataSource"/>s to be registered.</param>
		/// <returns>A context object that should be disposed in order to unregister the data sources.</returns>
		public static UnitTestStudyProviderContext RegisterStudies(IEnumerable<ISopDataSource> sopDataSources)
		{
			return RegisterStudies(ServerDirectory.GetLocalServer(), sopDataSources);
		}

		/// <summary>
		/// Registers a number of <see cref="ISopDataSource"/>s with the <see cref="UnitTestStudyLoader"/>.
		/// </summary>
		/// <remarks>
		/// <para>Dispose the returned context to unregister the data sources.</para>
		/// <para>This method is thread safe.</para>
		/// </remarks>
		/// <param name="server">Specifies the "server" on which these data sources "reside."</param>
		/// <param name="sopDataSources">The <see cref="ISopDataSource"/>s to be registered.</param>
		/// <returns>A context object that should be disposed in order to unregister the data sources.</returns>
		public static UnitTestStudyProviderContext RegisterStudies(IDicomServiceNode server, IEnumerable<ISopDataSource> sopDataSources)
		{
			var unitTestStudyProvider = new UnitTestStudyProviderContext(server, sopDataSources);
			RegisterStudyProvider(unitTestStudyProvider);
			return unitTestStudyProvider;
		}

		/// <summary>
		/// Registers a <see cref="IUnitTestStudyProvider"/> with the <see cref="UnitTestStudyLoader"/>.
		/// </summary>
		/// <remarks>
		/// <para>This method is thread safe.</para>
		/// </remarks>
		/// <param name="provider">The <see cref="IUnitTestStudyProvider"/> to be registered.</param>
		public static void RegisterStudyProvider(IUnitTestStudyProvider provider)
		{
			Platform.CheckForNullReference(provider, "provider");
			lock (_syncLock)
			{
				if (!_providers.Contains(provider))
					_providers.Add(provider);
			}
		}

		/// <summary>
		/// Unregisters the specified <see cref="IUnitTestStudyProvider"/> from the <see cref="UnitTestStudyLoader"/>.
		/// </summary>
		/// <remarks>
		/// <para>This method is thread safe.</para>
		/// </remarks>
		/// <param name="provider">The <see cref="IUnitTestStudyProvider"/> to be unregistered.</param>
		public static void UnregisterStudyProvider(IUnitTestStudyProvider provider)
		{
			Platform.CheckForNullReference(provider, "provider");
			lock (_syncLock)
			{
				_providers.Remove(provider);
			}
		}

		private static IEnumerable<ISopDataSource> GetSopDataSources(IApplicationEntity server, string studyInstanceUid)
		{
			var combinedResults = new List<ISopDataSource>();
			lock (_syncLock)
			{
				foreach (var provider in _providers)
				{
					var results = provider.GetSopDataSources(server, studyInstanceUid);
					if (results != null)
						combinedResults.AddRange(results);
				}
			}
			return combinedResults;
		}

		private IEnumerator<ISopDataSource> _enumerator;

		/// <summary>
		/// Initializes a new <see cref="UnitTestStudyLoader"/> instance.
		/// </summary>
		/// <remarks>
		/// All instances of <see cref="UnitTestStudyLoader"/> have access to the same pool of registered
		/// <see cref="IUnitTestStudyProvider"/>s and <see cref="ISopDataSource"/>s.
		/// The ImageViewer framework uses individual instances to perform the actual loading.
		/// </remarks>
		public UnitTestStudyLoader() : base(StudyLoaderId) {}

		/// <summary>
		/// Resets any current loading progress.
		/// </summary>
		protected override int OnStart(StudyLoaderArgs studyLoaderArgs)
		{
			var sopDataSources = new List<ISopDataSource>(GetSopDataSources(studyLoaderArgs.Server, studyLoaderArgs.StudyInstanceUid));
			_enumerator = sopDataSources.GetEnumerator();
			return sopDataSources.Count;
		}

		/// <summary>
		/// Gets the next <see cref="SopDataSource"/>.
		/// </summary>
		/// <remarks>
		/// Due to the nature of this implementation, it is quite likely like all pixel data has already been loaded.
		/// </remarks>
		protected override SopDataSource LoadNextSopDataSource()
		{
			if (_enumerator == null)
				return null;

			if (!_enumerator.MoveNext())
			{
				_enumerator = null;
				return null;
			}

			var current = _enumerator.Current;
			return current is SopDataSource ? (SopDataSource) current : new SopDataSourceAdapter(current);
		}

		#region UnitTestStudyProviderContext Class

		/// <summary>
		/// Represents the context of a number of <see cref="ISopDataSource"/>s that have been registered with the <see cref="UnitTestStudyLoader"/>.
		/// </summary>
		public sealed class UnitTestStudyProviderContext : IUnitTestStudyProvider, IDisposable
		{
			internal UnitTestStudyProviderContext(IDicomServiceNode server, IEnumerable<ISopDataSource> sopDataSources)
			{
				Platform.CheckForNullReference(server, "server");
				Platform.CheckForNullReference(sopDataSources, "sopDataSources");

				Server = server;
				SopDataSources = sopDataSources;
			}

			/// <summary>
			/// Unregisters the represented data sources from the <see cref="UnitTestStudyLoader"/>. Does <b>NOT</b> dispose the <see cref="ISopDataSource"/>s themselves.
			/// </summary>
			public void Dispose()
			{
				UnregisterStudyProvider(this);
			}

			/// <summary>
			/// Gets the "server" on which the represented data sources "reside."
			/// </summary>
			public IDicomServiceNode Server { get; private set; }

			/// <summary>
			/// Gets the represented data sources.
			/// </summary>
			public IEnumerable<ISopDataSource> SopDataSources { get; private set; }

			/// <summary>
			/// Gets the represented data sources for the specified study.
			/// </summary>
			/// <param name="studyInstanceUid">The study instance UID of the desired data sources.</param>
			public IEnumerable<ISopDataSource> this[string studyInstanceUid]
			{
				get { return CollectionUtils.Select(SopDataSources, s => s != null && s.StudyInstanceUid == studyInstanceUid); }
			}

            IEnumerable<ISopDataSource> IUnitTestStudyProvider.GetSopDataSources(IApplicationEntity server, string studyInstanceUid)
			{
				if (!Equals(Server, server))
					return null;
				return this[studyInstanceUid];
			}
		}

		#endregion

		#region SopDataSourceAdapter Class

		private class SopDataSourceAdapter : SopDataSource
		{
			private ISopDataSource _sopDataSource;

			public SopDataSourceAdapter(ISopDataSource sopDataSource)
			{
				_sopDataSource = sopDataSource;
			}

			protected override void Dispose(bool disposing)
			{
				if (_sopDataSource != null)
				{
					_sopDataSource.Dispose();
					_sopDataSource = null;
				}
				base.Dispose(disposing);
			}

			public override DicomAttribute this[DicomTag tag]
			{
				get { return _sopDataSource[tag]; }
			}

			public override DicomAttribute this[uint tag]
			{
				get { return _sopDataSource[tag]; }
			}

			public override bool IsImage
			{
				get { return _sopDataSource.IsImage; }
			}

			public override bool IsStored
			{
				get { return _sopDataSource.IsStored; }
			}

			public override int InstanceNumber
			{
				get { return _sopDataSource.InstanceNumber; }
			}

			public override string SeriesInstanceUid
			{
				get { return _sopDataSource.SeriesInstanceUid; }
			}

			public override string SopClassUid
			{
				get { return _sopDataSource.SopClassUid; }
			}

			public override string SopInstanceUid
			{
				get { return _sopDataSource.SopInstanceUid; }
			}

			public override string StudyInstanceUid
			{
				get { return _sopDataSource.StudyInstanceUid; }
			}

			public override string TransferSyntaxUid
			{
				get { return _sopDataSource.TransferSyntaxUid; }
			}

			protected override ISopFrameData GetFrameData(int frameNumber)
			{
				return _sopDataSource.GetFrameData(frameNumber);
			}

			public override bool TryGetAttribute(DicomTag tag, out DicomAttribute attribute)
			{
				return _sopDataSource.TryGetAttribute(tag, out attribute);
			}

			public override bool TryGetAttribute(uint tag, out DicomAttribute attribute)
			{
				return _sopDataSource.TryGetAttribute(tag, out attribute);
			}
		}

		#endregion
	}
}

#endif