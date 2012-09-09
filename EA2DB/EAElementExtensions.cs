namespace EA2DB
{
	using System.Collections.Generic;
	using System.Data;
	using System.Data.Common;
	using System.Data.OleDb;
	using System.Data.SqlClient;
	using System.Linq;

	/// <summary>
	/// EA DB extension methods.
	/// </summary>
	public static class EAElementExtensions
	{
		#region ### private members ###

		#region ### constants ###

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

		private const string SELECT_PACKAGES = @"Package_ID, Name, Parent_ID FROM t_package";

		private const string WHERE_OBJECT_ID_EQUALS = @"(t_object.Object_ID = {0})";
		private const string WHERE_OBJECT_NAME_LIKE = @"(t_object.Name LIKE '%{0}%')";
		private const string WHERE_OBJECT_NAME_EQUALS = @"(t_object.Name = '{0}')";

		private const string WHERE_PACKAGE_ID_EQUALS = @"(t_package.Package_ID = {0})";
		private const string WHERE_PACKAGE_NAME_LIKE = @"(t_package.Name LIKE '%{0}%')";
		private const string WHERE_PACKAGE_NAME_EQUALS = @"(t_package.Name = '{0}')";

		#endregion ### constants ###

		#region ### private methods ###

		private static void MakeFirstColumnPrimaryKey(DataTable table)
		{
			if(table != null)
			{
				DataColumn[] primaryKeys = new DataColumn[1];
				primaryKeys[0] = table.Columns[0];
				table.PrimaryKey = primaryKeys;
			}
		}

		private static int PackageIdByPathRecursive(DbConnection connection, IEnumerable<string> packagePath, int parentId)
		{
			int packageId = 0;
			var firstPackageSegment = packagePath.FirstOrDefault();

			EAQuery query = new EAQuery();
			query.Connection = connection;
			query.Select = SELECT_PACKAGES;
			DataSet dataSet = query.OnPackageName(firstPackageSegment)
			                  .And()
			                  .OnEqualsGeneric("t_package.Parent_ID", parentId)
			                  .QueryDataSet();
			if(packagePath.Count() == 1)
			{
				if(dataSet.Tables[0].Rows.Count > 0)
				{
					return dataSet.Tables[0].Rows[0].Field<int>(0);
				}
				else
				{
					return 0;
				}
			}

			var reminder = packagePath.Skip(1);
			foreach(var row in dataSet.Tables[0].AsEnumerable())
			{
				int subParentId = row.Field<int>(0);
				int tempPackageId = PackageIdByPathRecursive(connection, reminder, subParentId);
				if(tempPackageId > 0)
				{
					packageId = tempPackageId;
					break;
				}
			}
			return packageId;
		}

		private static DbDataAdapter GetDbDataAdapter(EAQuery query)
		{
			DbDataAdapter adapter = null;
			if(query.Connection.GetType() == typeof(OleDbConnection))
			{
				OleDbCommand command = new OleDbCommand(query.Query, query.Connection as OleDbConnection);
				adapter = new OleDbDataAdapter(command);
			}
			else
			{
				if(query.Connection.GetType() == typeof(SqlConnection))
				{
					SqlCommand command = new SqlCommand(query.Query, query.Connection as SqlConnection);
					adapter = new SqlDataAdapter(command);
				}
			}

			return adapter;
		}

		#endregion ### private methods ###

		#endregion ### private members ###


		#region ### Extension Methods ###

		#region OleDbConnection extensions

		public static EAQuery OperationsOfClass(this DbConnection connection)
		{
			EAQuery query = new EAQuery();
			query.Connection = connection;
			query.Select = SELECT_INNER_JOIN_OPERATIONS_OF_OBJECT;

			return query;
		}

		public static EAQuery Diagrams(this DbConnection connection)
		{
			EAQuery query = new EAQuery();
			query.Connection = connection;
			query.Select = SELECT_DIAGRAMS;

			return query;
		}

		public static EAQuery Packages(this DbConnection connection)
		{
			EAQuery query = new EAQuery();
			query.Connection = connection;
			query.Select = SELECT_PACKAGES;

			return query;
		}

		public static int PackageIdByPath(this DbConnection connection, string packagePath, string separator = ".")
		{
			var packagePathEnumerable = packagePath.Split(new string[] { separator }, System.StringSplitOptions.None);
			return PackageIdByPath(connection, packagePathEnumerable);
		}

		public static int PackageIdByPath(this DbConnection connection, IEnumerable<string> packagePath)
		{
			return PackageIdByPathRecursive(connection, packagePath, 0);
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

		public static EAQuery OnObjectName(this EAQuery query, string name)
		{
			query.Where.Add(string.Format(WHERE_OBJECT_NAME_EQUALS, name));
			return query;
		}

		public static EAQuery OnObjectNameLike(this EAQuery query, string name)
		{
			query.Where.Add(string.Format(WHERE_OBJECT_NAME_LIKE, name));
			return query;
		}

		public static EAQuery OnPackageID(this EAQuery query, int id)
		{
			query.Where.Add(string.Format(WHERE_PACKAGE_ID_EQUALS, id));
			return query;
		}

		public static EAQuery OnPackageName(this EAQuery query, string name)
		{
			query.Where.Add(string.Format(WHERE_PACKAGE_NAME_EQUALS, name));
			return query;
		}

		public static EAQuery OnPackageNameLike(this EAQuery query, string name)
		{
			query.Where.Add(string.Format(WHERE_PACKAGE_NAME_LIKE, name));
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
				// get DbDataAdapter by DbConnection type
				DbDataAdapter adapter = GetDbDataAdapter(query);

				// fill dataset
				DataSet dataSet = new DataSet();
				if(adapter != null)
				{
					adapter.Fill(dataSet);
					MakeFirstColumnPrimaryKey(dataSet.Tables[0]);
				}
				return dataSet;
			}
			return new DataSet();
		}

		#endregion EAQuery extensions

		#endregion ### Extension Methods ###
	}
}