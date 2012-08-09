namespace EA2DB
{
	using System.Data;
	using System.Data.OleDb;

	/// <summary>
	/// EA DB extension methods.
	/// </summary>
	public static class EAElementExtensions
	{
		public static void MakeFirstColumnPrimaryKey(DataTable table)
		{
			if(table != null)
			{
				DataColumn[] primaryKeys = new DataColumn[1];
				primaryKeys[0] = table.Columns[0];
				table.PrimaryKey = primaryKeys;
			}
		}
		#region ### Extension Methods ###


		#region OleDbConnection extensions

		public static EAQuery OperationsOfClass(this OleDbConnection connection)
		{
			EAQuery query = new EAQuery();
			query.Connection = connection;
			query.Select = SELECT_INNER_JOIN_OPERATIONS_OF_OBJECT;

			return query;
		}

		#endregion OleDbConnection extensions

		#region EAQuery extensions

		public static EAQuery OnObjectID(this EAQuery query, int id)
		{
			query.Where = string.Format(WHERE_OBJECT_ID_EQUALS, id);
			return query;
		}

		public static EAQuery OnName(this EAQuery query, string name)
		{
			query.Where = string.Format(WHERE_OBJECT_NAME_EQUALS, name);
			return query;
		}

		public static EAQuery OnNameLike(this EAQuery query, string name)
		{
			query.Where = string.Format(WHERE_OBJECT_NAME_LIKE, name);
			return query;
		}

		public static DataSet QueryDataSet(this EAQuery query)
		{
			if(query.IsValid)
			{
				DataSet dataSet = new DataSet();
				OleDbCommand command = new OleDbCommand(query.Query, query.Connection);
				OleDbDataAdapter a = new OleDbDataAdapter(command);
				a.Fill(dataSet);
				MakeFirstColumnPrimaryKey(dataSet.Tables[0]);
				return dataSet;
			}
			return new DataSet();
		}

		#endregion EAQuery extensions

		#endregion ### Extension Methods ###


		private const string SELECT_INNER_JOIN_OPERATIONS_OF_OBJECT = @"
SELECT t_operation.OperationID AS Operation_ID,
t_operation.Name AS Operation_Name,
t_object.Object_ID,
t_object.Name AS Object_Name
FROM (t_operation INNER JOIN t_object ON t_operation.Object_ID = t_object.Object_ID)";
		private const string WHERE_OBJECT_ID_EQUALS = @"WHERE (t_object.Object_ID = {0})";
		private const string WHERE_OBJECT_NAME_LIKE = @"WHERE (t_object.Name LIKE '%{0}%')";
		private const string WHERE_OBJECT_NAME_EQUALS = @"WHERE (t_object.Name = '{0}')";
	}
}
