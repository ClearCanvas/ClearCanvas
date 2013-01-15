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
    public abstract class EnumBroker<TOutput> : Broker, IEnumBroker<TOutput>
        where TOutput : ServerEnum, new()
    {
        #region IEnumBroker<TOutput> Members

        List<TOutput> IEnumBroker<TOutput>.Execute() 
        {
            List<TOutput> list = new List<TOutput>();
            TOutput tempValue = new TOutput();

            SqlDataReader myReader = null;
            SqlCommand command = null;

            try
            {               
                command = new SqlCommand(String.Format("SELECT * FROM {0}",tempValue.Name), Context.Connection)
                              {
                                  CommandType = CommandType.Text,
                                  CommandTimeout = SqlServerSettings.Default.CommandTimeout
                              };

                myReader = command.ExecuteReader();
                if (myReader == null)
                {
                    Platform.Log(LogLevel.Error, "Unable to select contents of '{0}'", tempValue.Name);
                    command.Dispose();
                    return list;
                }
                else
                {
                    if (myReader.HasRows)
                    {
                        Dictionary<string, PropertyInfo> propMap = EntityMapDictionary.GetEntityMap(typeof(TOutput));

                        while (myReader.Read())
                        {
                            TOutput row = new TOutput();

                            PopulateEntity(myReader, row, propMap);

                            list.Add(row);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception when retrieving enumerated value: {0}", tempValue.Name);

                throw new PersistenceException(String.Format("Unexpected problem when retrieving enumerated value: {0}: {1}", tempValue.Name, e.Message), e);
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
            return list;
        }

        #endregion
    }
}