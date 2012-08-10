namespace EA2DB
{
	using System.Collections.Generic;
	using System.Data.OleDb;

	/// <summary>
	/// Query Object passed through the EAElementExtensions extension methods
	/// </summary>
	public class EAQuery
	{
		public EAQuery()
		{
			Where = new List<string>();
		}

		public OleDbConnection Connection
		{
			get;
			set;
		}

		public bool IsValid
		{
			get
			{
				return (Connection != null) && !string.IsNullOrEmpty(Query);
			}
		}

		public string Query
		{
			get
			{
				if(Where.Count == 0)
				{
					return Select;
				}
				else
				{
					return string.Format("SELECT {0} WHERE {1}", Select, string.Join(" ", Where));
				}
			}
		}

		public string Select
		{
			get;
			set;
		}

		public List<string> Where
		{
			get;
			set;
		}

		public override string ToString()
		{
			return Query;
		}
	}
}