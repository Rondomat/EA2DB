using System.Data;
/*
 * Created by SharpDevelop.
 * User: Ron
 * Date: 07.08.2012
 * Time: 20:49
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

namespace EA2DB
{
	using System;
	using System.Data;
	using System.Data.OleDb;
	using System.Linq;
	using System.Collections.Generic;
	class Program
	{
		public static void Main(string[] args)
		{
			try
			{

				string connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=c:\temp\test.EAP";
				using(OleDbConnection connection = new OleDbConnection(connectionString))
				{
					connection.Open();

					List<string> opNames = new List<string>();

					opNames.Add("Version");
					opNames.Add("main");
					opNames.Add("main");
					opNames.Add("ma");

					DataSet dataSet = opNames.OperationsOfClassLike(true, connection);
					//DataSet dataSet = "un".OperationsOfClassLike(true, connection);
					DumpDS(dataSet);

					var names = dataSet.Tables[0].AsEnumerable().Select(x => new { X = x.Field<string>(1), Y = x.Field<string>(3) });
					names.ToList().ForEach(x =>
					{ 
						Console.WriteLine("Class: {0}", x.X);
						Console.WriteLine("Operation: {0}", x.Y);
					});



				}
			}
			catch(System.Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			//Console.Write("Press any key to continue . . . ");
			//Console.ReadKey(true);
		}

		private static void Dump(OleDbCommand command)
		{
			OleDbDataReader reader = command.ExecuteReader();
			while(reader.Read())
			{
				for(int fieldCount = 0; fieldCount < reader.FieldCount; fieldCount++)
				{
					Console.WriteLine(string.Format("[{0}] {1} ({2}):{3}",
													fieldCount,
													reader.GetName(fieldCount),
													reader.GetDataTypeName(fieldCount),
													reader[fieldCount].ToString()));
				}
			}
			reader.Close();
		}
		private static void DumpDS(OleDbCommand command)
		{
			DataSet dataSet = new DataSet();
			OleDbDataAdapter adapter = new OleDbDataAdapter(command);
			adapter.Fill(dataSet);
			DumpDS(dataSet);
		}

		private static void DumpDS(DataSet dataSet)
		{
			foreach(DataTable table in dataSet.Tables)
			{
				Console.WriteLine("++++{0}+++++", table.TableName);
				Console.Write("Column-> |");
				int count = 0;
				foreach(DataColumn column in table.Columns)
				{
					Console.Write("{0}:{1} [{2}]|", count++, column.ColumnName, column.DataType);
				}
				Console.WriteLine("-----------");
				count = 0;
				foreach(DataRow row in table.Rows)
				{
					Console.Write("{0}-> |", count++);
					int innerCount = 0;
					foreach(var item in row.ItemArray)
					{
						Console.Write("{0}:{1}|", innerCount++, item);
					}
				}
				Console.WriteLine("+++++++++");
			}
		}
	}
}