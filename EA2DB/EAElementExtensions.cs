namespace EA2DB
{
	using System.Collections.Generic;
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

		public static EAQuery Diagrams(this OleDbConnection connection)
		{
			EAQuery query = new EAQuery();
			query.Connection = connection;
			query.Select = SELECT_DIAGRAMS;

			return query;
		}

		#endregion OleDbConnection extensions

		#region DataSet extensions

		public static IEnumerable<int> IDs(this DataSet dataSet)
		{
			return dataSet.Tables[0].AsEnumerable().Select(x => x.Field<int>(0));
		}

		#endregion DataSet extensions

		#region EAQuery extensions

		public static EAQuery OnObjectID(this EAQuery query, int id)
		{
			query.Where.Add(string.Format(WHERE_OBJECT_ID_EQUALS, id));
			return query;
		}

		public static EAQuery OnEqualsGeneric(this EAQuery query, string columnName, string value)
		{
			query.Where.Add(string.Format(@"{0} = '{1}'", columnName, value));
			return query;
		}

		public static EAQuery OnEqualsGeneric(this EAQuery query, string columnName, int value)
		{
			query.Where.Add(string.Format(@"{0} = {1}", columnName, value));
			return query;
		}

		public static EAQuery OnName(this EAQuery query, string name)
		{
			query.Where.Add(string.Format(WHERE_OBJECT_NAME_EQUALS, name));
			return query;
		}

		public static EAQuery OnNameLike(this EAQuery query, string name)
		{
			query.Where.Add(string.Format(WHERE_OBJECT_NAME_LIKE, name));
			return query;
		}

		public static EAQuery OpenParenthesis(this EAQuery query)
		{
			query.Where.Add(OPEN_PARENTHESIS);
			return query;
		}

		public static EAQuery CloseParenthesis(this EAQuery query)
		{
			query.Where.Add(CLOSE_PARENTHESIS);
			return query;
		}

		public static EAQuery And(this EAQuery query)
		{
			query.Where.Add(AND);
			return query;
		}

		public static EAQuery Or(this EAQuery query)
		{
			query.Where.Add(OR);
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

		private const string AND = @"AND";
		private const string OR = @"OR";
		private const string CLOSE_PARENTHESIS = @")";
		private const string OPEN_PARENTHESIS = @"(";

		private const string SELECT_DIAGRAMS = @"Diagram_ID, Name FROM t_diagram";

		private const string SELECT_INNER_JOIN_OPERATIONS_OF_OBJECT = @"
t_operation.OperationID AS Operation_ID,
t_operation.Name AS Operation_Name,
t_object.Object_ID,
t_object.Name AS Object_Name
FROM (t_operation INNER JOIN t_object ON t_operation.Object_ID = t_object.Object_ID)";

		private const string WHERE_OBJECT_ID_EQUALS = @"(t_object.Object_ID = {0})";
		private const string WHERE_OBJECT_NAME_LIKE = @"(t_object.Name LIKE '%{0}%')";
		private const string WHERE_OBJECT_NAME_EQUALS = @"(t_object.Name = '{0}')";
	}
}