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
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise.SqlServer
{
    /// <summary>
    /// Provides base implementation of <see cref="IProcedureQueryBroker{TInput,TOutput}"/>
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    public abstract class ProcedureQueryBroker<TInput,TOutput> : Broker,IProcedureQueryBroker<TInput,TOutput>
        where TInput : ProcedureParameters
        where TOutput : ServerEntity, new()
    {
        private readonly String _procedureName;

        protected ProcedureQueryBroker(String procedureName)
        {
            _procedureName = procedureName;
        }

        #region IProcedureQueryBroker<TInput,TOutput> Members

        public IList<TOutput> Find(TInput criteria)
        {
            IList<TOutput> list = new List<TOutput>();

            InternalFind(criteria, -1, list.Add);

            return list;
        }

        public void Find(TInput criteria, ProcedureQueryCallback<TOutput> callback)
        {
            InternalFind(criteria, -1, callback);
        }

        public TOutput FindOne(TInput criteria)
        {
            TOutput result = null;

            InternalFind(criteria, 1, delegate(TOutput row)
                                          {
                                              result = row;
                                          });

            return result;
        }

        private void InternalFind(TInput criteria, int maxResults, ProcedureQueryCallback<TOutput> callback)
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
                
                // Set parameters
                SetParameters(command, criteria);

                if (Platform.IsLogLevelEnabled(LogLevel.Debug))
                    Platform.Log(LogLevel.Debug, "Executing stored procedure: {0}", _procedureName);

                myReader = command.ExecuteReader();
                if (myReader == null)
                {
                    Platform.Log(LogLevel.Error, "Unable to execute stored procedure '{0}'", _procedureName);
                    command.Dispose();
                    return;
                }
                else
                {

                    if (myReader.HasRows)
                    {
                        int resultCount = 0;
                        Dictionary<string, PropertyInfo> propMap = EntityMapDictionary.GetEntityMap(typeof(TOutput));
                        while (myReader.Read())
                        {
                            TOutput row = new TOutput();

                            PopulateEntity(myReader, row, propMap);

                            callback(row);

                            resultCount++;
                            if (maxResults > 0 && resultCount >= maxResults)
                                break;
                        }
                        myReader.Close();
                        myReader = null;
                    }
                    // Note:  The retrieving of output parameters must occur after
                    // the reader has been closed.
                    GetOutputParameters(command, criteria);
                }
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception when calling stored procedure: {0}", _procedureName);

                throw new PersistenceException(String.Format("Unexpected problem with stored procedure: {0}: {1}", _procedureName, e.Message), e);
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

        }

        #endregion
    }
}