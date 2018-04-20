using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace PostgresService
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
	// NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
	public class Service1 : IService1
	{
		public List<string> GetData(int min, int max, string title, string tag)
		{
			var connString = "Host=localhost;Username=postgres;Password=9001;Database=movies";
			List<string> results = new List<string>();

			using (var conn = new NpgsqlConnection(connString))
			{
				conn.Open();

				// Retrieve all rows
				using (var cmd = new NpgsqlCommand("SELECT title FROM movies", conn))
				using (var reader = cmd.ExecuteReader())
					while (reader.Read())
						results.Add(reader.GetString(0));
			}
			return results;
		}

	}
}
