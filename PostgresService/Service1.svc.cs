﻿using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Transactions;

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
                using (TransactionScope ts = new TransactionScope())
                {
                    using (var conn = new NpgsqlConnection(connString))
                    {
                        conn.Open();

                        // Retrieve all rows
                        string titlesql = "CREATE TEMPORARY TABLE filtertablebytitle AS SELECT * FROM movies m WHERE title ILIKE '%" + title + "%'; ";
                        string tagsql = "CREATE TEMPORARY TABLE filtertablebytag AS SELECT * FROM taginfo WHERE content ILIKE '%" + tag + "%'; ";
                        string combine = "CREATE TEMPORARY TABLE combine AS SELECT tags.tagid, tags.movieid FROM tags INNER JOIN filtertablebytag ON tags.tagid = filtertablebytag.tagid; ";
                        string complete = "CREATE TEMPORARY TABLE complete AS SELECT filtertablebytitle.movieid, filtertablebytitle.title FROM combine INNER JOIN filtertablebytitle ON filtertablebytitle.movieid = combine.movieid; ";
                        string rating = "CREATE TEMPORARY TABLE rating AS SELECT avg(r.rating) as rating, h.genreid FROM hasagenre h, ratings r, complete WHERE h.movieid = r.movieid AND complete.movieid = r.movieid AND r.rating >=" + min + " AND r.rating <=" + max + " GROUP BY genreid; ";
                        string averageratings = "CREATE TEMPORARY TABLE averageratingspergenre AS SELECT name, rating FROM genres, rating WHERE genres.genreid = rating.genreid;";
                        string genereCount = "CREATE TEMPORARY TABLE filtercount AS SELECT filtertablebytitle.movieid, filtertablebytitle.title, hasagenre.genreid FROM hasagenre INNER JOIN filtertablebytitle ON hasagenre.movieid = filtertablebytitle.movieid;";
                        using (var cmd = new NpgsqlCommand())
                        {
                            cmd.Connection = conn;
                            cmd.CommandText = titlesql + tagsql + combine + complete + rating + averageratings + genereCount;
                            cmd.ExecuteNonQuery();
                        }

                        //using (var cmd = new NpgsqlCommand(averageratings, conn))
                        using (var cmd = new NpgsqlCommand("SELECT g.name, moviecount FROM(SELECT h.genreid, count(h.movieid) as moviecount FROM filtercount h GROUP BY h.genreid) q1h, genres g WHERE q1h.genreid = g.genreid;", conn))
                        using (var reader = cmd.ExecuteReader())         //averageratingspergenre
                            while (reader.Read())
                            {
                                results.Add(reader.GetString(0));
                                results.Add(reader.GetString(1));

                            }
                        conn.Close();
                    }
                    ts.Complete();
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
                using (TransactionScope ts = new TransactionScope())
                {
                    using (var conn = new NpgsqlConnection(connString))
                    {
                        conn.Open();

                        // Retrieve all rows
                        string titlesql = "CREATE TEMPORARY TABLE filtertablebytitle AS SELECT * FROM movies m WHERE title ILIKE '%" + title + "%'; ";
                        string tagsql = "CREATE TEMPORARY TABLE filtertablebytag AS SELECT * FROM taginfo WHERE content ILIKE '%" + tag + "%'; ";
                        string combine = "CREATE TEMPORARY TABLE combine AS SELECT tags.tagid, tags.movieid FROM tags INNER JOIN filtertablebytag ON tags.tagid = filtertablebytag.tagid; ";
                        string complete = "CREATE TEMPORARY TABLE complete AS SELECT filtertablebytitle.movieid, filtertablebytitle.title FROM combine INNER JOIN filtertablebytitle ON filtertablebytitle.movieid = combine.movieid; ";
                        string rating = "CREATE TEMPORARY TABLE rating AS SELECT avg(r.rating) as rating, h.genreid FROM hasagenre h, ratings r, complete WHERE h.movieid = r.movieid AND complete.movieid = r.movieid AND r.rating >=" + min + " AND r.rating <=" + max + " GROUP BY genreid; ";
                        string averageratings = "CREATE TEMPORARY TABLE averageratingspergenre AS SELECT name, rating FROM genres, rating WHERE genres.genreid = rating.genreid;";
                        using (var cmd = new NpgsqlCommand())
                        {
                            cmd.Connection = conn;
                            cmd.CommandText = titlesql + tagsql + combine + complete + rating + averageratings;
                            cmd.ExecuteNonQuery();
                        }
                       
                        //using (var cmd = new NpgsqlCommand(averageratings, conn))
                        using (var cmd = new NpgsqlCommand("SELECT * FROM averageratingspergenre", conn))
                        using (var reader = cmd.ExecuteReader())         //averageratingspergenre
                            while (reader.Read())
                            {
                                results.Add(reader.GetString(0));
                                results.Add(reader.GetString(1));

                            }
                        conn.Close();
                    }
                    ts.Complete();
                }
            }
            return results;
        }
    }
}