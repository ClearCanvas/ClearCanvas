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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Automation;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// An <see cref="ExtensionPoint"/> for views on to the 
	/// <see cref="ImageViewerComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class ImageViewerComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	/// <summary>
	/// An <see cref="ExtensionPoint"/> for image viewer tools.
	/// </summary>
	/// <remarks>
	/// This <see cref="ExtensionPoint"/> is the means by which tools
	/// are added to the <see cref="ImageViewerComponent"/>.
	/// </remarks>
	[ExtensionPoint]
	public sealed class ImageViewerToolExtensionPoint : ExtensionPoint<ITool> {}

	/// <summary>
	/// An <see cref="ApplicationComponent"/> capable of image display.
	/// </summary>
	/// <remarks>
	/// The <see cref="ImageViewerComponent"/> (IVC) is an <see cref="ApplicationComponent"/> 
	/// whose purpose is to display images and to allow users to interact with those images.
	/// It provides a number of core services, such as image loading, layout, selection,
	/// rendering, etc.  An API and a number of extension points allow plugin developers
	/// to extend the functionality of the IVC.
	/// </remarks>
	[AssociateView(typeof (ImageViewerComponentViewExtensionPoint))]
	public partial class ImageViewerComponent : ApplicationComponent, IImageViewer, IContextMenuProvider
	{
		public const string ContextMenuSite = "imageviewer-contextmenu";
		public const string KeyboardSite = "imageviewer-keyboard";

		#region Private fields

		private StudyTree _studyTree;
		private ILogicalWorkspace _logicalWorkspace;
		private IPhysicalWorkspace _physicalWorkspace;
		private EventBroker _eventBroker;

		private readonly ViewerShortcutManager _shortcutManager;

		private readonly IViewerSetupHelper _setupHelper;
		private ToolSet _toolSet;
		private IViewerActionFilter _contextMenuFilter;
		private IViewerActionFilter _toolbarFilter;
		private ILayoutManager _layoutManager;

		private readonly AsynchronousPriorStudyLoader _priorStudyLoader;
		private SynchronizationContext _uiThreadSynchronizationContext;

		private static readonly StudyFinderMap _studyFinders = new StudyFinderMap();
		private readonly StudyLoaderMap _studyLoaders = new StudyLoaderMap();

		private event EventHandler _closingEvent;
		private event CancelEventHandler _closingInternal;

		#endregion

		/// <summary>
		/// Instantiates a new instance of <see cref="ImageViewerComponent"/>.
		/// </summary>
		/// This constructor creates an <see cref="ImageViewerComponent"/>
		/// that automatically lays out images according to how many 
		/// <see cref="DisplaySet"/>s there are.  Use this constructor in 
		/// simple display scenarios where you don't require control over 
		/// how images are initially laid out.
		/// If you do require control over the initial layout, use
		/// <see cref="ImageViewerComponent(ILayoutManager)"/> instead.
		public ImageViewerComponent() : this(LayoutManagerCreationParameters.Simple) {}

		/// <summary>
		/// Instantiates a new instance of <see cref="ImageViewerComponent"/>
		/// taking a <see cref="LayoutManagerCreationParameters"/> to determine
		/// how the <see cref="LayoutManager"/> should be created.
		/// </summary>
		/// <remarks>
		///	If <paramref name="creationParameters"/> is <see cref="LayoutManagerCreationParameters.Extended"/>,
		/// then the <see cref="LayoutManagerExtensionPoint"/> is used to create the <see cref="LayoutManager"/>.  If
		/// no extension exists, a simple layout manager is used.
		/// </remarks>
		public ImageViewerComponent(LayoutManagerCreationParameters creationParameters)
			: this(ImageViewer.LayoutManager.Create(creationParameters)) {}

		/// <summary>
		/// Initializes a new instance of <see cref="ImageViewerComponent"/>
		/// with the specified <see cref="ILayoutManager"/>.
		/// </summary>
		public ImageViewerComponent(ILayoutManager layoutManager)
			: this(layoutManager, PriorStudyFinder.Create()) {}

		/// <summary>
		/// <see cref="ImageViewerComponent"/>
		/// taking a <see cref="LayoutManagerCreationParameters"/> to determine
		/// how the <see cref="LayoutManager"/> should be created, as well as an
		/// <see cref="IPriorStudyFinder"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		///	If <paramref name="creationParameters"/> is <see cref="LayoutManagerCreationParameters.Extended"/>,
		/// then the <see cref="LayoutManagerExtensionPoint"/> is used to create the <see cref="LayoutManager"/>.  If
		/// no extension exists, a simple layout manager is used.
		/// </para>
		/// <para>If <paramref name="priorStudyFinder"/> is null or <see cref="PriorStudyFinder.Null"/>, then the
		/// <see cref="ImageViewerComponent"/> will not automatically search for priors.
		/// </para>
		/// </remarks>
		public ImageViewerComponent(LayoutManagerCreationParameters creationParameters, IPriorStudyFinder priorStudyFinder)
			: this(ImageViewer.LayoutManager.Create(creationParameters), priorStudyFinder) {}

		/// <summary>
		/// Initializes a new instance of <see cref="ImageViewerComponent"/>
		/// with the specified <see cref="ILayoutManager"/> and <see cref="IPriorStudyFinder"/>.
		/// </summary>
		/// <remarks>
		/// <para>If <paramref name="priorStudyFinder"/> is null or <see cref="PriorStudyFinder.Null"/>, then the
		/// <see cref="ImageViewerComponent"/> will not automatically search for priors.
		/// </para>
		/// </remarks>
		public ImageViewerComponent(ILayoutManager layoutManager, IPriorStudyFinder priorStudyFinder)
			: this(new ViewerSetupHelper(layoutManager, priorStudyFinder)) {}

		public ImageViewerComponent(IViewerSetupHelper setupHelper)
		{
			Platform.CheckForNullReference(setupHelper, "setupHelper");
			_setupHelper = setupHelper;
			_setupHelper.SetImageViewer(this);

			_shortcutManager = new ViewerShortcutManager(this);
			_priorStudyLoader = new AsynchronousPriorStudyLoader(this, _setupHelper.GetPriorStudyFinder());

			ExtensionData = new ExtensionData();
		}

		/// <summary>
		/// Override of <see cref="ApplicationComponent.Start"/>
		/// </summary>
		/// <remarks>
		/// For internal Framework use only.
		/// </remarks>
		public override void Start()
		{
			base.Start();

			_uiThreadSynchronizationContext = SynchronizationContext.Current;

			_contextMenuFilter = _setupHelper.GetContextMenuFilter() ?? ViewerActionFilter.Null;
			_contextMenuFilter.SetImageViewer(this);

			_toolbarFilter = _setupHelper.GetToolbarFilter() ?? ViewerActionFilter.Null;
			_toolbarFilter.SetImageViewer(this);

			_toolSet = new ToolSet(CreateTools(), CreateToolContext());

			// since the keyboard action model is otherwise never used, we explicitly invoke it here to apply the persisted action model values to the actions
			ActionModelRoot.CreateModel(ActionsNamespace, KeyboardSite, _toolSet.Actions);

			foreach (ITool tool in _toolSet.Tools)
				_shortcutManager.RegisterImageViewerTool(tool);
		}

		/// <summary>
		/// Override of <see cref="ApplicationComponent.Stop"/>
		/// </summary>
		/// <remarks>
		/// For internal Framework use only.
		/// </remarks>
		public override void Stop()
		{
			EventsHelper.Fire(_closingEvent, this, EventArgs.Empty);

			base.Stop();
		}

		public override bool PrepareExit()
		{
			// allow viewer tools first opportunity to prevent closing the viewer, since most legit reasons to do so involve tools performing some operation on the loaded study.
			var e = new CancelEventArgs();
			EventsHelper.Fire(_closingInternal, this, e);
			return !e.Cancel && base.PrepareExit();
		}

		#region Public Properties

		/// <summary>
		/// Gets the host <see cref="IDesktopWindow"/>.
		/// </summary>
		public IDesktopWindow DesktopWindow
		{
			get { return Host.DesktopWindow; }
		}

		/// <summary>
		/// Gets actions associated with the <see cref="ImageViewerComponent"/>'s
		/// <see cref="ToolSet"/>.
		/// </summary>
		public override IActionSet ExportedActions
		{
			get
			{
				// we should technically only export the actions that target the global menus
				// or toolbars, but it doesn't matter - the desktop will sort it out
				return _toolSet.Actions;
			}
		}

		/// <summary>
		/// Gets the namespace that qualifies the global action models owned by this <see cref="IImageViewer"/>. This value may not be null.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This namespace only applies to the global action models (i.e. global menu and toolbar sites) when the
		/// <see cref="IImageViewer"/> is launched as a <see cref="IWorkspace"/>. In contrast, the namespace returned
		/// by <see cref="ActionsNamespace"/> applies to local action models such as the viewer context menu.
		/// </para>
		/// <para>
		/// The default implementation defers to <see cref="ActionsNamespace"/>.
		/// </para>
		/// </remarks>
		public override string GlobalActionsNamespace
		{
			get { return ActionsNamespace; }
		}

		/// <summary>
		/// Gets the namespace that qualifies the global action models owned by this <see cref="IImageViewer"/>. This value may not be null.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This namespace only applies to local action models such as the viewer context menu. In contrast,
		/// the namespace returned by <see cref="GlobalActionsNamespace"/> applies to the global action models
		/// (i.e. global menu and toolbar sites) when the <see cref="IImageViewer"/> is launched as a <see cref="IWorkspace"/>.
		/// </para>
		/// <para>
		/// The default implementation uses the default viewer namespace.
		/// </para>
		/// </remarks>
		public virtual string ActionsNamespace
		{
			get { return typeof (ImageViewerComponent).FullName; }
		}

		/// <summary>
		/// Gets the command history.
		/// </summary>
		/// <remarks>
		/// Each <see cref="ImageViewerComponent"/> (IVC) maintains its own 
		/// <see cref="CommandHistory"/>.
		/// </remarks>
		public CommandHistory CommandHistory
		{
			get { return Host.CommandHistory; }
		}

		/// <summary>
		/// Gets the <see cref="IPhysicalWorkspace"/>.
		/// </summary>
		public IPhysicalWorkspace PhysicalWorkspace
		{
			get { return _physicalWorkspace ?? (_physicalWorkspace = new PhysicalWorkspace(this)); }
		}

		/// <summary>
		/// Gets the <see cref="ILogicalWorkspace"/>.
		/// </summary>
		public ILogicalWorkspace LogicalWorkspace
		{
			get { return _logicalWorkspace ?? (_logicalWorkspace = new LogicalWorkspace(this)); }
		}

		/// <summary>
		/// Gets the <see cref="EventBroker"/>.
		/// </summary>
		public EventBroker EventBroker
		{
			get { return _eventBroker ?? (_eventBroker = new EventBroker()); }
		}

		/// <summary>
		/// Gets the currently selected <see cref="IImageBox"/>
		/// </summary>
		/// <value>The currently selected <see cref="IImageBox"/>, or <b>null</b> if
		/// no <see cref="IImageBox"/> is currently selected.</value>
		public IImageBox SelectedImageBox
		{
			get { return PhysicalWorkspace == null ? null : PhysicalWorkspace.SelectedImageBox; }
		}

		/// <summary>
		/// Gets the currently selected <see cref="Tile"/>
		/// </summary>
		/// <value>The currently selected <see cref="ITile"/>, or <b>null</b> if
		/// no <see cref="ITile"/> is currently selected.</value>
		public ITile SelectedTile
		{
			get { return SelectedImageBox == null ? null : SelectedImageBox.SelectedTile; }
		}

		/// <summary>
		/// Gets the currently selected <see cref="PresentationImage"/>
		/// </summary>
		/// <value>The currently selected <see cref="IPresentationImage"/>, or <b>null</b> if
		/// no <see cref="IPresentationImage"/> is currently selected.</value>
		public IPresentationImage SelectedPresentationImage
		{
			get { return SelectedTile == null ? null : SelectedTile.PresentationImage; }
		}

		/// <summary>
		/// Gets the <see cref="IViewerShortcutManager"/>.
		/// </summary>
		/// <remarks>
		/// This property is intended for internal framework use only.
		/// </remarks>
		public IViewerShortcutManager ShortcutManager
		{
			get { return _shortcutManager; }
		}

		/// <summary>
		/// Gets the <see cref="StudyTree"/>.
		/// </summary>
		/// <remarks>
		/// Although each <see cref="ImageViewerComponent"/> (IVC) maintains its own
		/// <see cref="StudyTree"/>, actual <see cref="ImageSop"/> objects are shared
		/// between IVCs for efficient memory usage.
		/// </remarks>
		public StudyTree StudyTree
		{
			get { return _studyTree ?? (_studyTree = new StudyTree()); }
		}

		/// <summary>
		/// Gets the <see cref="ILayoutManager"/> associated with this <see cref="ImageViewerComponent"/>
		/// </summary>
		public ILayoutManager LayoutManager
		{
			get
			{
				if (_layoutManager == null)
				{
					_layoutManager = _setupHelper.GetLayoutManager() ?? new LayoutManager();
					_layoutManager.SetImageViewer(this);
				}

				return _layoutManager;
			}
		}

		/// <summary>
		/// Gets the <see cref="IPriorStudyLoader"/> that uses a given <see cref="IPriorStudyFinder"/> to search
		/// for prior studies, and adds the studies to the <see cref="IImageViewer.StudyTree"/>.
		/// </summary>
		public IPriorStudyLoader PriorStudyLoader
		{
			get { return _priorStudyLoader; }
		}

		/// <summary>
		/// Gets a string containing the patients currently loaded in this
		/// <see cref="ImageViewerComponent"/>.
		/// </summary>
		public string PatientsLoadedLabel
		{
			get { return CreateTitle(CollectionUtils.Map(StudyTree.Patients, (Patient patient) => (IPatientData) patient)); }
		}

		public ExtensionData ExtensionData { get; private set; }

		#endregion

		#region Events

		/// <summary>
		/// Occurs when the <see cref="ImageViewerComponent"/> is about to close.
		/// </summary>
		public event EventHandler Closing
		{
			add { _closingEvent += value; }
			remove { _closingEvent -= value; }
		}

		#endregion

		#region Service Automation

		/// <summary>
		/// Allows a one-time call of one of the viewer's automation services, which is usually
		/// an <see cref="ImageViewerTool"/>, but can also be
		/// an extension of <see cref="AutomationExtensionPoint{T}"/>.
		/// </summary>
		/// <remarks>
		/// If the service is itself a tool, it can access the viewer via it's own <see cref="IImageViewerToolContext">tool context</see>.
		/// Otherwise, this method sets the value of <see cref="AutomationContext.Current"/> for the duration of the call.
		/// </remarks>
		/// <typeparam name="TService">The type of automation service to be used; this must be an interface and not a tool class.</typeparam>
		/// <param name="withService">The delegate this method will call, passing in the service object.</param>
		public void Automate<TService>(Action<TService> withService) where TService : class
		{
			if (!typeof (TService).IsInterface)
				throw new ArgumentException("Automation Service must be an interface, not a class.");

			AutomationContext.Current = new AutomationContext(this);
			try
			{
				withService(GetAutomationService<TService>());
			}
			finally
			{
				AutomationContext.Current = null;
			}
		}

		/// <summary>
		/// Allows a one-time call of one of the viewer's automation services, which is usually
		/// an <see cref="ImageViewerTool"/>, but can also be
		/// an extension of <see cref="AutomationExtensionPoint{T}"/>.
		/// </summary>
		/// <remarks>
		/// If the service is itself a tool, it can access the viewer via it's own <see cref="IImageViewerToolContext">tool context</see>.
		/// Otherwise, this method sets the value of <see cref="AutomationContext.Current"/> for the duration of the call.
		/// </remarks>
		/// <typeparam name="TService">The type of automation service to be used; this must be an interface and not a tool class.</typeparam>
		/// <typeparam name="TResult">The type of result returned from the automation service call.</typeparam>
		/// <param name="withService">The delegate this method will call, passing in the service object.</param>
		public TResult Automate<TService, TResult>(Func<TService, TResult> withService) where TService : class
		{
			if (!typeof (TService).IsInterface)
				throw new ArgumentException("Automation Service must be an interface, not a class.");

			AutomationContext.Current = new AutomationContext(this);
			try
			{
				return withService(GetAutomationService<TService>());
			}
			finally
			{
				AutomationContext.Current = null;
			}
		}

		internal TService GetAutomationService<TService>() where TService : class
		{
			try
			{
				//Try looking for an extension first.
				return (TService) new AutomationExtensionPoint<TService>().CreateExtension();
			}
			catch (NotSupportedException) {}

			var tool = _toolSet.Tools.OfType<TService>().FirstOrDefault();
			if (tool == null)
				throw new NotSupportedException(String.Format("Component service '{0}' does not exist.", typeof (TService).Name));
			return tool;
		}

		#endregion

		#region Protected properties

		/// <summary>
		/// Gets the <see cref="ToolSet"/>.
		/// </summary>
		protected ToolSet ToolSet
		{
			get { return _toolSet; }
		}

		/// <summary>
		/// Creates a set of tools for this image viewer to load into its tool set.  Subclasses can override
		/// this to provide their own tools or cull the set of tools this creates.
		/// </summary>
		/// <returns></returns>
		protected virtual IEnumerable CreateTools()
		{
			return _setupHelper.GetTools();
		}

		/// <summary>
		/// Creates an <see cref="IImageViewerToolContext"/> to provide to all the tools owned by this image viewer.
		/// </summary>
		/// <remarks>
		/// Subclasses can override this to provide their own custom implementation of an <see cref="IImageViewerToolContext"/>.
		/// </remarks>
		protected virtual ImageViewerToolContext CreateToolContext()
		{
			return new ImageViewerToolContext(this);
		}

		#endregion

		#region Private/internal properties

		private static StudyFinderMap StudyFinders
		{
			get { return _studyFinders; }
		}

		private StudyLoaderMap StudyLoaders
		{
			get { return _studyLoaders; }
		}

		private ActionModelNode ContextMenuModel
		{
			get
			{
				IActionSet actions = _toolSet.Actions.Select(_contextMenuFilter.Evaluate);
				return ActionModelRoot.CreateModel(ActionsNamespace, ContextMenuSite, actions);
			}
		}

		private ActionModelNode KeyboardModel
		{
			get { return ActionModelRoot.CreateModel(ActionsNamespace, KeyboardSite, _toolSet.Actions); }
		}

		public ActionModelNode ToolbarModel
		{
			get
			{
				IActionSet actions = _toolSet.Actions.Select(_toolbarFilter.Evaluate);
				return ActionModelRoot.CreateModel(GlobalActionsNamespace, "global-toolbars", actions);
			}
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Lays out the images in the <see cref="ImageViewerComponent"/> using
		/// the current layout manager.
		/// </summary>
		/// <remarks>
		/// Immediately after <see cref="ILayoutManager.Layout"/> is called, the
		/// <see cref="PriorStudyLoader"/> then starts searching for priors
		/// and adding them to the <see cref="StudyTree"/>.
		/// </remarks>
		public void Layout()
		{
			LayoutManager.Layout();
			_priorStudyLoader.Start();
		}

		/// <summary>
		/// Queries for studies matching a specified set of query parameters.
		/// </summary>
		/// <param name="queryParameters">The search criteria.</param>
		/// <param name="targetServer">The server to query. Can be null
		/// if the <paramref name="studyFinderName"/> does not support the querying
		/// of different servers.</param>
		/// <param name="studyFinderName">The name of the <see cref="IStudyFinder"/> to use, which is specified
		/// by <see cref="IStudyFinder.Name"/>.</param>
		/// <exception cref="StudyFinderNotFoundException">Thrown when a matching <see cref="IStudyFinder"/> does not exist.</exception>
		public static StudyItemList FindStudy(
			QueryParameters queryParameters,
			IApplicationEntity targetServer,
			string studyFinderName)
		{
			Platform.CheckForNullReference(queryParameters, "queryParameters");
			Platform.CheckForEmptyString(studyFinderName, "studyFinderName");

			return StudyFinders[studyFinderName].Query(queryParameters, targetServer);
		}

		#region Study Loading

		/// <summary>
		/// Loads a study using the specified parameters.
		/// </summary>
		/// <remarks>After this method is executed, the image viewer's <see cref="StudyTree"/>
		/// will be populated with the appropriate <see cref="Study"/>, <see cref="Series"/> 
		/// and <see cref="Sop"/> objects.
		/// 
		/// By default, the Framework provides an implementation of 
		/// <see cref="IStudyLoader"/> called <b>LocalStoreStudyLoader</b> which loads
		/// studies from the local database.  If you have implemented your own 
		/// <see cref="IStudyLoader"/> and want to load a study using that implementation,
		/// just pass in the name provided by <see cref="IStudyLoader.Name"/> as the source.
		/// </remarks>
		/// <param name="loadStudyArgs">A <see cref="LoadStudyArgs"/> object containing information about the study to be loaded.</param>
		/// <exception cref="InUseLoadStudyException">The specified study is in use and cannot be opened at this time.</exception>
		/// <exception cref="NearlineLoadStudyException">The specified study is nearline and cannot be opened at this time.</exception>
		/// <exception cref="OfflineLoadStudyException">The specified study is offline and cannot be opened at this time.</exception>
		/// <exception cref="NotFoundLoadStudyException">The specified study could not be found.</exception>
		/// <exception cref="LoadStudyException">One or more images could not be opened, or an unspecified error has occurred.</exception>
		/// <exception cref="StudyLoaderNotFoundException">The specified <see cref="IStudyLoader"/> could not be found.</exception>
		public void LoadStudy(LoadStudyArgs loadStudyArgs)
		{
			SynchronizationContext context = _uiThreadSynchronizationContext ?? SynchronizationContext.Current;
			using (SingleStudyLoader loader = new SingleStudyLoader(context, this, loadStudyArgs))
			{
				loader.LoadStudy();
				if (loader.Error != null)
					throw loader.Error;
			}
		}

		/// <summary>
		/// Loads multiple studies, consolidating any errors that have occurred into a single
		/// <see cref="LoadMultipleStudiesException"/>.
		/// </summary>
		/// <remarks>
		/// This method of loading studies allows studies to be partially loaded.  When an error
		/// has occurred, the exception is simply added to a list and studies continue to be loaded.
		/// Once complete, if any errors have occurred, a <see cref="LoadMultipleStudiesException"/> is thrown.
		/// If only a single study was requested, the behaviour is identical to <see cref="LoadStudy(LoadStudyArgs)"/>
		/// and the exception will be one of those described for that method, not a <see cref="LoadMultipleStudiesException"/>.
		/// </remarks>
		/// <exception cref="InUseLoadStudyException">(When only a single study is requested) The specified study is in use and cannot be opened at this time.</exception>
		/// <exception cref="NearlineLoadStudyException">(When only a single study is requested) The specified study is nearline and cannot be opened at this time.</exception>
		/// <exception cref="OfflineLoadStudyException">(When only a single study is requested) The specified study is offline and cannot be opened at this time.</exception>
		/// <exception cref="NotFoundLoadStudyException">(When only a single study is requested) The specified study could not be found.</exception>
		/// <exception cref="LoadStudyException">(When only a single study is requested) One or more images could not be opened, or an unspecified error has occurred.</exception>
		/// <exception cref="LoadMultipleStudiesException">Thrown when one or more of the specified studies has completely or partially failed to load.</exception>
		/// <exception cref="StudyLoaderNotFoundException">The specified <see cref="IStudyLoader"/> could not be found.</exception>
		public void LoadStudies(IList<LoadStudyArgs> loadStudyArgs)
		{
			if (loadStudyArgs.Count == 0)
				throw new ArgumentException("At least one study must be specified.");

			if (loadStudyArgs.Count == 1)
			{
				LoadStudy(loadStudyArgs[0]);
			}
			else
			{
				List<Exception> loadStudyExceptions = new List<Exception>();
				foreach (LoadStudyArgs args in loadStudyArgs)
				{
					try
					{
						LoadStudy(args);
					}
					catch (LoadStudyException e)
					{
						string message = String.Format("An error occurred while loading study '{0}'", args.StudyInstanceUid);
						Platform.Log(LogLevel.Error, e, message);
						loadStudyExceptions.Add(e);
					}
					catch (StudyLoaderNotFoundException e)
					{
						string message =
							String.Format("An error occurred while loading study '{0}' from server '{1}'",
							              args.StudyInstanceUid, args.Server);
						Platform.Log(LogLevel.Error, e, message);
						loadStudyExceptions.Add(e);
					}
				}

				if (loadStudyExceptions.Count > 0)
					throw new LoadMultipleStudiesException(loadStudyExceptions, loadStudyArgs.Count);
			}
		}

		#endregion

		#region File Loading

		/// <summary>
		/// Loads images from the specified file paths.
		/// </summary>
		/// <param name="files">An array of file paths.</param>
		/// <exception cref="LoadSopsException">One or more images could not be opened.</exception>
		/// <exception cref="ArgumentNullException">A parameter is <b>null</b>.</exception>
		public void LoadImages(string[] files)
		{
			// Dummy variable; this overload can't be cancelled
			bool cancelled;
			LoadImages(files, null, out cancelled);
		}

		/// <summary>
		/// Loads images from the specified file paths and displays a progress bar.
		/// </summary>
		/// <param name="files">A list of file paths.</param>
		/// <param name="desktop">The desktop window.  This is necessary for
		/// a progress bar to be shown.  If <b>null</b>, no progress bar
		/// will be shown.</param>
		/// <param name="cancelled">A value that indicates whether the operation
		/// was cancelled.</param>
		/// <exception cref="LoadSopsException">One or more images could not be opened.</exception>
		/// <exception cref="ArgumentNullException">A parameter is <b>null</b>.</exception>
		public void LoadImages(string[] files, IDesktopWindow desktop, out bool cancelled)
		{
			Platform.CheckForNullReference(files, "files");

			using (LocalSopLoader loader = new LocalSopLoader(this))
			{
				loader.Load(files, desktop, out cancelled);
			}
		}

		#endregion

		#region Utility Methods

		/// <summary>
		/// Gets whether or not the specified, named, <see cref="IStudyFinder"/> is supported.
		/// </summary>
		public static bool IsStudyFinderSupported(string studyFinderName)
		{
			return StudyFinderMap.IsStudyFinderSupported(studyFinderName);
		}

		/// <summary>
		/// Gets whether or not the specified, named, <see cref="IStudyLoader"/> is supported.
		/// </summary>
		public static bool IsStudyLoaderSupported(string studyLoaderName)
		{
			return StudyLoaderMap.IsStudyLoaderSupported(studyLoaderName);
		}

		/// <summary>
		/// Creates the default title for an <see cref="ImageViewerComponent"/>, based on the given patient information.
		/// </summary>
		public static string CreateTitle(IEnumerable<IPatientData> patientData)
		{
			return StringUtilities.Combine(patientData, String.Format(" {0} ", SR.SeparatorPatientsLoaded),
			                               delegate(IPatientData data)
			                               	{
			                               		PersonName name = new PersonName(data.PatientsName);
			                               		string formattedName = name.FormattedName;
			                               		return StringUtilities.Combine(new[] {formattedName, data.PatientId}, String.Format(" {0} ", SR.SeparatorPatientDescription));
			                               	});
		}

		/// <summary>
		/// Tries to get a reference to an <see cref="IImageViewer"/> hosted by a workspace.
		/// </summary>
		/// <returns>The active <see cref="IImageViewer"/> or <b>null</b> if 
		/// the workspace does not host an <see cref="IImageViewer"/>.</returns>
		public static IImageViewer GetAsImageViewer(Workspace workspace)
		{
			Platform.CheckForNullReference(workspace, "workspace");

			// return the hosted IImageViewer, or null if the hosted component is not an IImageViewer
			return workspace.Component as IImageViewer;
		}

		/// <summary>
		/// Launches the specified <see cref="ImageViewerComponent"/> using the parameters specified in <paramref name="launchArgs"/>.
		/// </summary>
		public static void Launch(ImageViewerComponent imageViewer, LaunchImageViewerArgs launchArgs)
		{
			IDesktopWindow window = GetLaunchWindow(launchArgs.WindowBehaviour);

			Launch(imageViewer, window, launchArgs.Title);
		}

		/// <summary>
		/// Launches the specified <see cref="ImageViewerComponent"/> using the <paramref name="window"/> parameter.
		/// </summary>
		public static void Launch(ImageViewerComponent imageViewer, IDesktopWindow window, string title)
		{
			if (title == null)
				title = imageViewer.PatientsLoadedLabel;

			IWorkspace workspace = LaunchAsWorkspace(window, imageViewer, title);
			imageViewer.ParentDesktopObject = workspace;
			try
			{
				imageViewer.Layout();
			}
			catch (Exception)
			{
				workspace.Close();
				throw;
			}
		}

		/// <summary>
		/// Launches an <see cref="ImageViewerComponent"/> in the active desktop window.
		/// </summary>
		/// <param name="imageViewer"></param>
		/// <remarks>
		/// Subsequent <see cref="ImageViewerComponent"/>s will also be launched in workspaces in
		/// the same window.
		/// </remarks>
		[Obsolete("Use Launch instead.")]
		public static void LaunchInActiveWindow(ImageViewerComponent imageViewer)
		{
			Launch(imageViewer, new LaunchImageViewerArgs(WindowBehaviour.Single));
		}

		/// <summary>
		/// Launches an <see cref="ImageViewerComponent"/> in a separate desktop window
		/// devoted strictly to image display.
		/// </summary>
		/// <param name="imageViewer"></param>
		/// <remarks>
		/// Subsequent <see cref="ImageViewerComponent"/>s will also be launched in workspaces in
		/// the same window.
		/// </remarks>
		[Obsolete("Use Launch instead.")]
		public static void LaunchInSeparateWindow(ImageViewerComponent imageViewer)
		{
			Launch(imageViewer, new LaunchImageViewerArgs(WindowBehaviour.Separate));
		}

		#endregion

		#endregion

		private static IDesktopWindow GetLaunchWindow(WindowBehaviour windowBehaviour)
		{
			if (windowBehaviour == WindowBehaviour.Auto)
				return Application.ActiveDesktopWindow;
			else if (windowBehaviour == WindowBehaviour.Single)
				return Application.ActiveDesktopWindow;

			IDesktopWindow window;
			const string imageViewerWindow = "ImageViewer";

			// If an image viewer desktop window already exists, use it
			if (Application.DesktopWindows.Contains(imageViewerWindow))
			{
				window = Application.DesktopWindows[imageViewerWindow];
			}
				// If not, create one
			else
			{
				DesktopWindowCreationArgs args = new DesktopWindowCreationArgs("", imageViewerWindow);
				window = Application.DesktopWindows.AddNew(args);
			}

			return window;
		}

		#region Disposal

		#region IDisposable Members

		/// <summary>
		/// Releases all resources used by this <see cref="ImageViewerComponent"/>.
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
				ParentDesktopObject = null;

				if (_toolSet != null)
				{
					_toolSet.Dispose();
					_toolSet = null;
				}

				StopLoadingPriors();
				StopPrefetching();

				if (_physicalWorkspace != null)
				{
					_physicalWorkspace.Dispose();
					_physicalWorkspace = null;
				}

				if (_logicalWorkspace != null)
				{
					_logicalWorkspace.Dispose();
					_logicalWorkspace = null;
				}

				if (_studyTree != null)
				{
					_studyTree.Dispose();
					_studyTree = null;
				}

				if (_layoutManager != null)
				{
					_layoutManager.Dispose();
					_layoutManager = null;
				}

				if (ExtensionData != null)
				{
					ExtensionData.Dispose();
					ExtensionData = null;
				}
			}
		}

		#endregion

		#region Private methods

		private void StopPrefetching()
		{
			foreach (IStudyLoader loader in _studyLoaders)
			{
				if (loader.PrefetchingStrategy != null)
					loader.PrefetchingStrategy.Stop();
			}
		}

		private void StopLoadingPriors()
		{
			if (_priorStudyLoader != null)
				_priorStudyLoader.Stop();
		}

		#endregion

		#region Desktop Host Closed Event Handling

		private IDesktopObject _parentDesktopObject;

		/// <summary>
		/// Sets the parent desktop object for the sole purpose of hooking up an event handler to perform disposal on ourself.
		/// </summary>
		private IDesktopObject ParentDesktopObject
		{
			set
			{
				if (ReferenceEquals(_parentDesktopObject, value)) return;
				if (_parentDesktopObject != null) _parentDesktopObject.Closed -= OnParentDesktopObjectClosed;
				_parentDesktopObject = value;
				if (_parentDesktopObject != null) _parentDesktopObject.Closed += OnParentDesktopObjectClosed;
			}
		}

		private void OnParentDesktopObjectClosed(object sender, ClosedEventArgs e)
		{
			ParentDesktopObject = null;
			Dispose();
		}

		#endregion

		#region IContextMenuProvider Members

		/// <summary>
		/// Gets the context menu model for the <see cref="ImageViewerComponent"/>.
		/// </summary>
		/// <param name="mouseInformation"></param>
		/// <returns>An <see cref="ActionModelNode"/></returns>
		/// <remarks>
		/// This method is used by the tile's view class to generate the 
		/// <see cref="ImageViewerComponent"/> context menu when a user right-clicks
		/// on a tile.  It is unlikely that you will ever need to use this method.
		/// </remarks>
		public virtual ActionModelNode GetContextMenuModel(IMouseInformation mouseInformation)
		{
			return ContextMenuModel;
		}

		#endregion
	}
}