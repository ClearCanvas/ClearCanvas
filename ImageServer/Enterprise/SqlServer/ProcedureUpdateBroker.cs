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
using System.Data;
using System.Data.SqlClient;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise.SqlServer
{
    /// <summary>
    /// Provides base implementation of <see cref="IProcedureUpdateBroker{TInput}"/>
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    public abstract class ProcedureUpdateBroker<TInput> : Broker, IProcedureUpdateBroker<TInput>
        where TInput : ProcedureParameters
    {
        private readonly String _procedureName;

        protected ProcedureUpdateBroker(String procedureName)
        {
            _procedureName = procedureName;
        }

        #region IProcedureUpdateBroker<TInput> Members

        public bool Execute(TInput criteria)
        {
            SqlDataReader myReader = null;
            SqlCommand command = null;

            try
            {
                command = new SqlCommand(_procedureName, Context.Connection)
                              {
                                  CommandType = CommandType.StoredProcedure,
                                  CommandTimeout = SqlServerSettings.Default.CommandTimeout
                              };

                UpdateContext update = Context as UpdateContext;
                if (update != null)
                    command.Transaction = update.Transaction;

                if (Platform.IsLogLevelEnabled(LogLevel.Debug))
                    Platform.Log(LogLevel.Debug, "Executing stored procedure: {0}", _procedureName);

                // Set parameters
                SetParameters(command, criteria);

                int rows = command.ExecuteNonQuery();

                GetOutputParameters(command, criteria);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception when calling stored procedure: {0}", _procedureName);

                throw new PersistenceException(String.Format("Unexpected error with stored procedure: {0}", _procedureName), e);
            }
            finally
            {
                // Cleanup the reader/command, or else we won't be able to do anything with the
                // connection the next time here.
                if (myReader != null)
                {
                    myReader.Close();
                    myReader.Dispose();
                }
                if (command != null)
                    command.Dispose();
            }

            return true;
        }

        #endregion
    }
}