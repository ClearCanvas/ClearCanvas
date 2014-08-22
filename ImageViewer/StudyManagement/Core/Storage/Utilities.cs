using System;
using System.Data.Common;
using System.Data.Linq;
using System.Data.SqlTypes;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage
{
	internal static class Utilities
	{
		public static DbCommand CreateCommand(this DbConnection connection, string commandText, DbTransaction transaction)
		{
			var command = connection.CreateCommand();
			command.CommandText = commandText;
			command.Transaction = transaction;
			return command;
		}

		private static Binary ToLinqBinary(object binary)
		{
			var bytes = binary as byte[];
			if (bytes != null) return bytes;
			if (binary is SqlBinary) return ((SqlBinary) binary).Value;
			throw new InvalidCastException("Unable to convert binary column to System.Data.Linq.Binary");
		}

		public static void ReadInsertedRowIdentity(this DbCommand command, ILinq2SqlEntity entity, string tableName, string rowIdColumn = "oid", string rowVersionColumn = "version")
		{
			var commandText = string.Format("SELECT [{1}] FROM [{2}] WHERE [{0}] = @id", rowIdColumn, rowVersionColumn, tableName);
			using (var cmd = CreateCommand(command.Connection, "SELECT @@IDENTITY", command.Transaction))
			{
				// executing a scalar command twice is significantly faster then executing a reader query for a two-column-single-row result
				entity.RowId = Convert.ToInt64(cmd.ExecuteScalar());
				cmd.CommandText = commandText;
				cmd.SetParameter("id", entity.RowId);
				entity.RowVersion = ToLinqBinary(cmd.ExecuteScalar());
			}
		}

		public static void ReadUpdatedRowVersion(this DbCommand command, ILinq2SqlEntity entity, string tableName, string rowIdColumn = "oid", string rowVersionColumn = "version")
		{
			var commandText = string.Format("SELECT [{1}] FROM [{2}] WHERE [{0}] = @id", rowIdColumn, rowVersionColumn, tableName);
			using (var cmd = CreateCommand(command.Connection, commandText, command.Transaction))
			{
				cmd.SetParameter("id", entity.RowId);
				entity.RowVersion = ToLinqBinary(cmd.ExecuteScalar());
			}
		}

		public static void SetParameter<T>(this DbCommand command, string name, T? value)
			where T : struct
		{
			SetParameter(command, name, value.HasValue ? (object) value.Value : DBNull.Value);
		}

		public static void SetParameter(this DbCommand command, string name, Binary value)
		{
			SetParameter(command, name, (object) value.ToArray());
		}

		public static void SetParameter(this DbCommand command, string name, object value)
		{
			var parameter = command.CreateParameter();
			parameter.ParameterName = name;
			parameter.Value = value ?? DBNull.Value;
			command.Parameters.Add(parameter);
		}
	}

	internal interface ILinq2SqlEntity
	{
		long RowId { get; set; }
		Binary RowVersion { get; set; }
	}
}