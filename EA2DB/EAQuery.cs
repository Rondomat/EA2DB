namespace EA2DB
{
	using System.Data.OleDb;

	/// <summary>
	/// Query Object passed through the EAElementExtensions extension methods
	/// </summary>
	public class EAQuery
	{
		public OleDbConnection Connection { get; set; }
		public string Select { get; set; }
		public string Where { get; set; }
		public string Query
		{
			get
			{
				if(string.IsNullOrEmpty(Where))
				{
					return Select;
				}
				else
				{
					return string.Format("{0} {1}", Select, Where);
				}
			}
		}

		public bool IsValid
		{
			get
			{
				return (Connection != null) && !string.IsNullOrEmpty(Query);
			}
		}

		public override string ToString()
		{
			return Query;
		}
	}
}
