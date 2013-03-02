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
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Controls
{
    public partial class ServerPartitionSelector : System.Web.UI.UserControl
    {
        #region Private Members
        private bool _partitionListLoaded;
        private IList<ServerPartition> _partitionList;
        private ServerPartition _selectedPartition;
        private UpdatePanel _updatePanel;
        private readonly Dictionary<ServerEntityKey, LinkButton> _buttons = new Dictionary<ServerEntityKey, LinkButton>(); 
        #endregion

        #region Events/Delegates

        /// <summary>
        /// Defines the event handler for <seealso cref="PartitionChanged"/> event.
        /// </summary>
        /// <param name="thePartition">The selected </param>
        public delegate void PartitionChangedEvent(ServerPartition thePartition);

        /// <summary>
        /// Occurs when users click on a partition link
        /// </summary>
        public event PartitionChangedEvent PartitionChanged;

        #endregion


        public bool IsEmpty
        {
            get
            {
                return ServerPartitionList == null || ServerPartitionList.Count == 0;
            }
        }

        public ServerPartition SelectedPartition
        {
            get
            {
                if (_selectedPartition == null)
                {
                    if (SelectedPartitionKey != null)
                    {
                        _selectedPartition = ServerPartitionList.SingleOrDefault(p => p.Key.Equals(SelectedPartitionKey));
                    }
                    else
                    {
                        string aeTitle = Request["AETitle"];
                        var partitionKey = Request["PartitionKey"];

                        if (aeTitle != null)
                        {
                            _selectedPartition = ServerPartitionList.SingleOrDefault(p => p.AeTitle == aeTitle);
                        }

                        if (_selectedPartition == null && partitionKey != null)
                        {
                            var partitionEntityKey = new Guid(partitionKey);
                            _selectedPartition = ServerPartitionList.SingleOrDefault(p => p.Key.Key.Equals(partitionEntityKey));
                        }
                    }

                    // no partition selected, pick the first in the list
                    if (_selectedPartition == null)
                    {
                        _selectedPartition = (ServerPartitionList != null && ServerPartitionList.Count > 0) ? ServerPartitionList.First() : null;                       
                    }

                    SelectedPartitionKey = _selectedPartition!=null? _selectedPartition.Key:null;
                }

                return _selectedPartition;
            }
            set
            {
                _selectedPartition = value;
                SelectedPartitionKey = _selectedPartition != null ? _selectedPartition.Key : null;
            }
        }

        public IList<ServerPartition> ServerPartitionList
        {
            get
            {
                if (!_partitionListLoaded)
                {
                    _partitionList = LoadPartitions();
                    _partitionListLoaded = true;
                }

                return _partitionList;
            }
            set { _partitionList = value; }
        }

        private ServerEntityKey SelectedPartitionKey
        {
            set
            {
                ViewState["SelectedPartitionKey"] = value;
            }
            get
            {
                return ViewState["SelectedPartitionKey"] as ServerEntityKey;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (ServerPartitionList == null || ServerPartitionList.Count == 0)
            {
                ShowNoPartitionMessage();
            }
            else
            {
                AddAccessiblePartitions();
            }
            
        }

        private void ShowNoPartitionMessage()
        {
            NoPartitionPanel.Visible = true;
            PartitionPanel.Visible = false;
        }


        #region Public Methods
        internal void SetUpdatePanel(UpdatePanel updatePanel)
        {
            _updatePanel = updatePanel;
        }
        #endregion

        #region Private Methods


        private void AddAccessiblePartitions()
        {
            foreach (ServerPartition partition in ServerPartitionList)
            {
                var button = new LinkButton
                {
                    Text = partition.AeTitle,
                    CommandArgument = partition.AeTitle,
                    ID = partition.AeTitle + "_ID",
                    CssClass = "PartitionLink"
                };
                button.Command += LinkClick;

                if (partition.Key.Equals(SelectedPartition.Key))
                {
                    button.Enabled = false;
                    button.CssClass = "PartitionLinkDisabled";
                }

                _buttons.Add(partition.Key, button);

                PartitionLinkPanel.Controls.Add(button);
                PartitionLinkPanel.Controls.Add(new LiteralControl(" "));
                if (_updatePanel != null)
                {
                    var trigger = new AsyncPostBackTrigger
                    {
                        ControlID = button.ID,
                        EventName = "Click"
                    };
                    _updatePanel.Triggers.Add(trigger);
                }
            }
        }

        protected void LinkClick(object sender, CommandEventArgs e)
        {
            foreach (ServerPartition partition in ServerPartitionList)
            {
                var button = _buttons[partition.Key];

                if (partition.AeTitle.Equals(e.CommandArgument.ToString()))
                {
                    SelectedPartition = partition;

                    if (PartitionChanged != null)
                        PartitionChanged(SelectedPartition);

                    button.Enabled = false;
                    button.CssClass = "PartitionLinkDisabled";
                }
                else
                {
                    button.Enabled = true;
                    button.CssClass = "PartitionLink";
                }
            }

            //if (_updatePanel != null)
            //    _updatePanel.Update();
        }

        private IList<ServerPartition> LoadPartitions()
        {
            try
            {
                var xp = new ServerPartitionTabsExtensionPoint();
                var extension = xp.CreateExtension() as IServerPartitionTabsExtension;

                if (extension != null)
                {
                    return new List<ServerPartition>(extension.LoadServerPartitions());
                }
            }
            catch (Exception)
            {

            }

            var defaultImpl = new DefaultServerPartitionTabsExtension();
            return new List<ServerPartition>(defaultImpl.LoadServerPartitions());
        }

        #endregion
    }
}