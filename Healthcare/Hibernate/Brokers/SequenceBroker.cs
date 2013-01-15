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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Enterprise.Hibernate.Ddl;
using NHibernate.Cfg;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	/// <summary>
	/// Base class for brokers that encapsulate sequence generators.
	/// </summary>
	public abstract class SequenceBroker : Broker
	{
		private readonly string _tableName;
		private readonly string _columnName;
		private readonly long _initialValue;

		protected SequenceBroker(string tableName, string columnName, long initialValue)
		{
			_tableName = tableName;
			_columnName = columnName;
			_initialValue = initialValue;
		}

		public string[] GenerateCreateScripts(Configuration config)
		{
			var defaultSchema = config.GetProperty(NHibernate.Cfg.Environment.DefaultSchema);
			var tableName = !string.IsNullOrEmpty(defaultSchema) ? defaultSchema + "." + _tableName : _tableName;

			return new[]
				{
					string.Format("create table {0} ( {1} {2} );", tableName, _columnName, DdlScriptGenerator.GetDialect(config).GetTypeName( NHibernate.SqlTypes.SqlTypeFactory.Int64 )),
					string.Format("insert into {0} values ( {1} )", tableName, _initialValue)
				};
		}

		public string[] GenerateUpgradeScripts(Configuration config, RelationalModelInfo baselineModel)
		{
			return new string[] { };	// nothing to do
		}

		public string[] GenerateDropScripts(Configuration config)
		{
			return new [] { DdlScriptGenerator.GetDialect(config).GetDropTableString(_tableName) };
		}


		/// <summary>
		/// Peeks at the next number in the sequence, but does not advance the sequence.
		/// </summary>
		/// <returns></returns>
		public string PeekNext()
		{
			// try to read the next accession number
			try
			{
				var select = this.CreateSqlCommand(string.Format("SELECT * from {0}", _tableName));
				return select.ExecuteScalar().ToString();
			}
			catch (Exception e)
			{
				throw new PersistenceException(SR.ErrorFailedReadNextSequenceNumber, e);
			}
		}

		/// <summary>
		/// Gets the next number in the sequence, advancing the sequence by 1.
		/// </summary>
		/// <returns></returns>
		public string GetNext()
		{
			int updatedRows;
			long sequenceValue;

			// the loop is necessary to ensure that we succeed in obtaining an accession number
			// It is possible that another process is trying to do this at the same time,
			// hence there is an inevitable race condition which may cause the operation to occassionally fail
			// can we avoid the need for a loop by using Serializable transaction isolation for this operation???
			do
			{
				// try to read the next accession number
				try
				{
					var select = this.CreateSqlCommand(string.Format("SELECT * from {0}", _tableName));
					sequenceValue = (long)select.ExecuteScalar();
				}
				catch (Exception e)
				{
					throw new PersistenceException(SR.ErrorFailedReadNextSequenceNumber, e);
				}

				if (sequenceValue == 0)
				{
					throw new HealthcareWorkflowException(SR.ErrorSequenceNotInitialized);
				}

				// update the sequence, by trying to update a row containing the previous number
				// this may fail if another process has updated in the meantime, in which case
				// the loop will just try again
				try
				{
					var updateSql = string.Format("UPDATE {0} SET {1} = @next WHERE {2} = @prev", _tableName, _columnName, _columnName);
					var update = this.CreateSqlCommand(updateSql);
					AddParameter(update, "next", sequenceValue + 1);
					AddParameter(update, "prev", sequenceValue);

					updatedRows = update.ExecuteNonQuery();
				}
				catch (Exception e)
				{
					throw new PersistenceException(SR.ErrorFailedUpdateNextSequenceNumber, e);
				}
			}
			while (updatedRows == 0);

			return sequenceValue.ToString();
		}

		private static void AddParameter(IDbCommand cmd, string name, object value)
		{
			var p = cmd.CreateParameter();
			p.ParameterName = name;
			p.Value = value;
			cmd.Parameters.Add(p);
		}
	}
}
