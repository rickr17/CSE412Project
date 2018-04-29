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
		public List<string> GetCount(int min, int max, string title, string tag)
		{
			var connString = "Host=localhost;Username=postgres;Password=9001;Database=movies";
			List<string> results = new List<string>();

			if (title == "startup" && tag == "startup") //load graphs on startup of application 
			{
				using (var conn = new NpgsqlConnection(connString))
				{
					conn.Open();

					// Retrieve all rows
					using (var cmd = new NpgsqlCommand("SELECT g.name AS name, COUNT(m.movieid) AS moviecount FROM movies m, genres g, hasagenre h WHERE m.movieid = h.movieid AND g.genreid = h.genreid GROUP BY g.genreid", conn))
					using (var reader = cmd.ExecuteReader())
						while (reader.Read())
						{
							results.Add(reader.GetString(0));
							results.Add(reader.GetString(1));

						}

				}
			}
			else
			{
				using (var conn = new NpgsqlConnection(connString))
				{
					conn.Open();

					// Retrieve all rows
					using (var cmd = new NpgsqlCommand("DROP TABLE filtertablebytitle", conn))
					using (var reader = cmd.ExecuteReader())
						while (reader.Read())
						{
							results.Add(reader.GetString(0));
						}

				}

				if (title == "" && tag == "") //filter by rating only
				{
					using (var conn = new NpgsqlConnection(connString))
					{
						conn.Open();

						// Retrieve all rows
						using (var cmd = new NpgsqlCommand("SELECT g.name AS name, COUNT(m.movieid) AS moviecount FROM movies m, genres g, hasagenre h, ratings r WHERE(r.rating >= " + min + " AND r.rating <= " + max + ") AND m.movieid = r.movieid AND m.movieid = h.movieid AND g.genreid = h.genreid GROUP BY g.genreid", conn))
						using (var reader = cmd.ExecuteReader())
							while (reader.Read())
							{
								results.Add(reader.GetString(0));
								results.Add(reader.GetString(1));

							}

					}
				}
				else if (title != "" && tag == "") //filter by title and rating
				{

				}
				else if (title == "" && tag == "") //filter by tag and rating
				{

				}
				else if (title != "" && tag != "") //filter by all 3
				{

				}
			}
			return results;
		}

		public List<string> GetRating(int min, int max, string title, string tag)
		{
			var connString = "Host=localhost;Username=postgres;Password=9001;Database=movies";
			List<string> results = new List<string>();

			if (title == "startup" && tag == "startup")
			{
				using (var conn = new NpgsqlConnection(connString))
				{
					conn.Open();

					// Retrieve all rows
					using (var cmd = new NpgsqlCommand("SELECT g.name AS name, AVG(r.rating) AS rating FROM movies m, genres g, hasagenre h, ratings r, users u WHERE m.movieid = h.movieid AND g.genreid = h.genreid AND r.movieid = m.movieid AND u.userid = r.userid GROUP BY g.genreid", conn))
					using (var reader = cmd.ExecuteReader())
						while (reader.Read())
						{
							results.Add(reader.GetString(0));
							results.Add(reader.GetString(1));

						}

				}
			}
			else
			{
				if (title == "" && tag == "") //filter by rating only
				{
					
				}
				else if (title != "" && tag == "") //filter by title && rating
				{

				}
				else if (title == "" && tag == "") //filter by tag && rating
				{

				}
				else if (title != "" && tag != "") //filter by all 3
				{

				}
			}
			return results;
		}
	}
}