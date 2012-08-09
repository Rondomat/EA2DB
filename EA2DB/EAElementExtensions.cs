// -----------------------------------------------------------------------
// <copyright file="EAElementExtensions.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace EA2DB
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Data;
	using System.Data.OleDb;

	/// <summary>
	/// TODO: Update summary.
	/// </summary>
	public static class EAElementExtensions
	{
		#region ### Extension Methods ###


		#endregion ### Extension Methods ###

		public static DataSet OperationsOfClassLike(this IEnumerable<string> names, bool bLike, OleDbConnection connection)
		{
			DataSet dataSet = new DataSet();
			
			foreach(var name in names)
			{
				var childDataSet = OperationsOfClassLike(name, bLike, connection);
				dataSet.Merge(childDataSet.Tables[0]);
			}
			return dataSet;
		}

		public static DataSet OperationsOfClassLike(this string name, bool bLike, OleDbConnection connection)
		{
			DataSet dataSet = new DataSet();
			string like = string.Format(OPERATIONS_OF_CLASS_LIKE, name);
			string query = string.Format("{0}{1}", OPERATIONS_OF_CLASS_SELECT, like);
			OleDbCommand command = new OleDbCommand(query, connection);
			OleDbDataAdapter a = new OleDbDataAdapter(command);
			a.Fill(dataSet);
			DataColumn[] primaryKeys = new DataColumn[1];
			primaryKeys[0] = dataSet.Tables[0].Columns[0];
			dataSet.Tables[0].PrimaryKey = primaryKeys;
			return dataSet;
		}



		private const string OPERATIONS_OF_CLASS_SELECT = @"
SELECT t_operation.OperationID AS Operation_ID,
t_operation.Name AS Operation_Name,
t_object.Object_ID,
t_object.Name AS Object_Name
FROM (t_operation INNER JOIN t_object ON t_operation.Object_ID = t_object.Object_ID)";
		private const string OPERATIONS_OF_CLASS_WHERE = @"WHERE (t_object.Object_ID = {0})";
		private const string OPERATIONS_OF_CLASS_LIKE = @"WHERE (t_object.Name LIKE '%{0}%')";
		private const string OPERATIONS_OF_CLASS_EXACT = @"WHERE (t_object.Name = {0})";
	}
}
