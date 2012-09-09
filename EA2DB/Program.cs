namespace EA2DB
{
	using System;
	using System.Data;
	using System.Data.OleDb;
	using System.Linq;
	using System.Collections.Generic;

	/// <summary>
	/// Main program.
	/// </summary>
	class Program
	{
		public static void Main(string[] args)
		{
			try
			{

				string connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=d:\temp\test.EAP";
				using(OleDbConnection connection = new OleDbConnection(connectionString))
				{
					connection.Open();
					DataSet dataSet;
					dataSet = connection.OperationsOfClass().OnObjectID(11748).Or().OnObjectName("intercpu").QueryDataSet();
					DumpDataSet(dataSet);
					ProjectDataSet(dataSet);

					dataSet = connection.OperationsOfClass().OnObjectName("intercpu").Or().OnObjectName("mc_fahrt").QueryDataSet();
					DumpDataSet(dataSet);
					ProjectDataSet(dataSet);

					var iDs = connection.Diagrams().OnEqualsGeneric("t_diagram.ShowDetails", 0).QueryDataSet().IDs();
					DumpIDs(iDs);

					dataSet = connection.Packages().OnPackageName("intercpu").QueryDataSet();
					DumpDataSet(dataSet);

					string packagePath = "MyModel.Class Model.Sub1.Sub2.Sub3";
					//string packagePath = "MC_860/Moduldesign MC/Frameworks";
					int packageId = connection.PackageIdByPath(packagePath);
					Console.WriteLine("{0}:{1}", packagePath, packageId);

				}
			}
			catch(System.Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			//Console.Write("Press any key to continue . . . ");
			//Console.ReadKey(true);
		}

		public static void ProjectDataSet(DataSet dataSet)
		{
			var names = dataSet.Tables[0].AsEnumerable().Select(x => new { X = x.Field<string>(1), Y = x.Field<string>(3) });
			names.ToList().ForEach(x =>
			{
				Console.WriteLine("Operation: {0}", x.X);
				Console.WriteLine("Class: {0}", x.Y);
			});
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

		private static void DumpDataSet(OleDbCommand command)
		{
			DataSet dataSet = new DataSet();
			OleDbDataAdapter adapter = new OleDbDataAdapter(command);
			adapter.Fill(dataSet);
			DumpDataSet(dataSet);
		}

		private static void DumpDataSet(DataSet dataSet)
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

		public static void DumpIDs(IEnumerable<int> iDs)
		{
			iDs.ToList().ForEach(x => { Console.WriteLine(x); });
		}
	}
}
