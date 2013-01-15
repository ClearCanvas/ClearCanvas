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
using System.ServiceProcess;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Server.ShredHost;
using System.Threading;

namespace ClearCanvas.Server.ShredHostClientUI
{
    [ExtensionPoint()]
    public class ShredHostClientToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    /// <summary>
    /// Extension point for views onto <see cref="ShredHostClientComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ShredHostClientComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    public interface IShredHostClientToolContext : IToolContext
    {
        Shred SelectedShred { get; }
        void StartShred(Shred shred);
        void StopShred(Shred shred);
    }
	
    /// <summary>
    /// ShredHostClientComponent class
    /// </summary>
    [AssociateView(typeof(ShredHostClientComponentViewExtensionPoint))]
    public class ShredHostClientComponent : ApplicationComponent
    {
        public class ShredHostClientToolContext : ToolContext, IShredHostClientToolContext
        {

            public ShredHostClientToolContext(ShredHostClientComponent component)
            {
                Platform.CheckForNullReference(component, "component");
                _component = component;
            }

            #region IShredHostClientToolContext Members

            public Shred SelectedShred
            {
                get 
                {
                    Shred selectedShred = _component.TableSelection.Item as Shred;
                    return selectedShred;
                }
            }

            public void StartShred(Shred shred)
            {
                _component.StartShred(shred);
            }

            public void StopShred(Shred shred)
            {
                _component.StopShred(shred);
            }

            #endregion

            #region Private fields
            ShredHostClientComponent _component;
            #endregion
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ShredHostClientComponent()
        {

        }

        public override void Start()
        {
            // TODO prepare the component for its live phase
            base.Start();

            // initialize the list object that will display the shreds
            _shredCollection = new Table<Shred>();
            _shredCollection.Columns.Add(new TableColumn<Shred, string>("Shreds",
                delegate(Shred shred) { return shred.Name; }
                ));
            _shredCollection.Columns.Add(new TableColumn<Shred, string>("Status",
                delegate(Shred shred) { return (shred.IsRunning) ? "Running" : "Stopped"; }
                ));

            // refresh the state of the Shred Host and shreds
            Refresh();

            _toolSet = new ToolSet(new ShredHostClientToolExtensionPoint(), new ShredHostClientToolContext(this));
            _toolbarModel = ActionModelRoot.CreateModel(this.GetType().FullName, "shredhostclient-toolbar", _toolSet.Actions);
            _contextMenuModel = ActionModelRoot.CreateModel(this.GetType().FullName, "shredhostclient-contextmenu", _toolSet.Actions);

            _refreshTask = new BackgroundTask(delegate(IBackgroundTaskContext context)
            {
                while (true)
                {
                    System.Threading.Thread.Sleep(2000);
                    Refresh();
                }
            },true
            );

            _refreshTask.Run();

        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            _toolSet.Dispose();
            _toolSet = null;

            // signal the refresh thread to stop
            _refreshTask.RequestCancel();

            base.Stop();
        }

        public void Toggle()
        {
            ServiceController controller = GetShredHostServiceController();
            switch (controller.Status)
            {
                case ServiceControllerStatus.Running:
                    controller.Stop();
                    _shredCollection.Items.Clear();
                    return;
                case ServiceControllerStatus.Stopped:
                    controller.Start();
                    return;
                default:
                    return;
            }
        }

        private void Refresh()
        {
            // poll to see if ShredHost is running
            ServiceController controller = GetShredHostServiceController();
            bool newStatus = (controller.Status == ServiceControllerStatus.Running);
            if (this.IsShredHostRunning != newStatus)
                this.IsShredHostRunning = newStatus;

            // if the shred host is not running, the WCF service will not be reachable.
            if (!this.IsShredHostRunning)
                return;

            WcfDataShred[] shreds;
            using (ShredHostClient client = new ShredHostClient())
            {
                shreds = client.GetShreds();
            }

            foreach (WcfDataShred shred in shreds)
            {
                int matchIndex = _shredCollection.Items.FindIndex(delegate(Shred otherShred)
                {
                    return otherShred.Id == shred._id;
                });

                // this is a new shred being reported from Shred Host, add it to our list,
                // usually the case if we are refreshing for the first time
                if (-1 == matchIndex)
                {
                    _shredCollection.Items.Add(new Shred(shred._id, shred._name, shred._description, shred._isRunning));
                }
                else
                {
                    if (_shredCollection.Items[matchIndex].IsRunning != shred._isRunning)
                        _shredCollection.Items[matchIndex].IsRunning = shred._isRunning;
                }
            }
        }

        public void StartShred(Shred shred)
        {
            bool isRunning;
            using (ShredHostClient client = new ShredHostClient())
            {
                isRunning = client.StartShred(shred.GetWcfDataShred());
            }

            int indexCurrentShred = this.ShredCollection.Items.FindIndex(delegate(Shred otherShred)
            {
                return otherShred.Id == shred.Id;
            });

            this.ShredCollection.Items[indexCurrentShred].IsRunning = isRunning;
            this.ShredCollection.Items.NotifyItemUpdated(indexCurrentShred);
        }

        public void StopShred(Shred shred)
        {
            bool isRunning;
            using (ShredHostClient client = new ShredHostClient())
            {
                isRunning = client.StopShred(shred.GetWcfDataShred());
            }

            int indexCurrentShred = this.ShredCollection.Items.FindIndex(delegate(Shred otherShred)
            {
                return otherShred.Id == shred.Id;
            });

            this.ShredCollection.Items[indexCurrentShred].IsRunning = isRunning;
            this.ShredCollection.Items.NotifyItemUpdated(indexCurrentShred);
        }

        public ServiceController GetShredHostServiceController()
        {
            return new ServiceController("ClearCanvas Shred Host Service");
        }

        #region Properties
        private bool _isShredHostRunning;
        private ISelection _tableSelection;
        private Table<Shred> _shredCollection;

        public ISelection TableSelection
        {
            get { return _tableSelection; }
            set
            {
                if (_tableSelection != value)
                {
                    _tableSelection = value;
                    NotifyPropertyChanged("TableSelection");
                }
            }
        }

        public bool IsShredHostRunning
        {
            get { return _isShredHostRunning; }
            set
            {
                _isShredHostRunning = value;
                NotifyPropertyChanged("IsShredHostRunning");
            }
        }

        public Table<Shred> ShredCollection
        {
            get { return _shredCollection; }
        }

        public ActionModelRoot ToolbarModel
        {
            get { return _toolbarModel; }
        }

        public ActionModelRoot ContextMenuModel
        {
            get { return _contextMenuModel; }
        }
        #endregion

        #region Private fields
        ToolSet _toolSet;
        ActionModelRoot _toolbarModel;
        ActionModelRoot _contextMenuModel;
        BackgroundTask _refreshTask;
        #endregion
    }
}
