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
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{


    /// <summary>
    /// Extension point for views onto <see cref="DefaultCodeEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class DefaultCodeEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// A default code editor class component class.  This is a no frills editor and is typically
    /// used only when a better code editor does not exist in the installed plugin base.
    /// </summary>
    [AssociateView(typeof(DefaultCodeEditorComponentViewExtensionPoint))]
    public class DefaultCodeEditorComponent : ApplicationComponent, ICodeEditor
    {
        /// <summary>
        /// 
        /// </summary>
        public class InsertTextEventArgs : EventArgs
        {
            private string _text;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="text"></param>
            public InsertTextEventArgs(string text)
            {
                _text = text;
            }

            /// <summary>
            /// Text to insert.
            /// </summary>
            public string Text
            {
                get { return _text; }
            }
        }

        private string _text;
        private event EventHandler<InsertTextEventArgs> _insertTextRequested;


        /// <summary>
        /// Constructor
        /// </summary>
        internal DefaultCodeEditorComponent()
        {
        }

        #region ICodeEditor Members

        IApplicationComponent ICodeEditor.GetComponent()
        {
            return this;
        }

        string ICodeEditor.Text
        {
            get { return this.Text; }
            set { this.Text = value; }
        }

        void ICodeEditor.InsertText(string text)
        {
            EventsHelper.Fire(_insertTextRequested, this, new InsertTextEventArgs(text));
        }

        string ICodeEditor.Language
        {
            get { return null; }
            set { /* not supported */ }
        }

        bool ICodeEditor.Modified
        {
            get { return this.Modified; }
            set { this.Modified = value; }
        }

        event EventHandler ICodeEditor.ModifiedChanged
        {
            add { this.ModifiedChanged += value; }
            remove { this.ModifiedChanged -= value; }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Starts the component.
        /// </summary>
        public override void Start()
        {
            base.Start();
        }

        /// <summary>
        /// Stops the component.
        /// </summary>
        public override void Stop()
        {
            base.Stop();
        }

        #endregion

        #region Presentation Model

        /// <summary>
        /// Notifies the view that it should insert the specified text at the current location.
        /// </summary>
        public event EventHandler<InsertTextEventArgs> InsertTextRequested
        {
            add { _insertTextRequested += value; }
            remove { _insertTextRequested -= value; }
        }

        /// <summary>
        /// Gets or sets the text that is displayed in the editor.
        /// </summary>
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                this.Modified = true;
                NotifyPropertyChanged("Text");
            }
        }

        #endregion

    }
}
