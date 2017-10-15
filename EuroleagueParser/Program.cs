using System.Collections.Generic;
using System.Net;
using System.Linq;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace EuroleagueParser
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                DeleteTables();
                AcquireData();
                
            }
            catch (SystemException excep)
            {

            }

        }

        private static void AcquireData()
        {
            WebClient webClient = new WebClient();
            string webUrl = ConfigurationManager.AppSettings["WebSiteUrl"];
            bool isFinished = false;
            int newPage = 1;
            while (!isFinished)
            {
                string page = webClient.DownloadString(webUrl);
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(page);
                List<List<string>> table = doc.DocumentNode.SelectSingleNode("//table [not(descendant::table)]")
                            .Descendants("tr")
                            .Where(tr => tr.Elements("td").Count() > 1)
                            .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())
                            .ToList();
                
                MapHtmlWithRanking(table);
                if (table.Count < 50)
                    isFinished = true;
                newPage++;
                string pageNumber = newPage.ToString().PadLeft(3, '0');
                webUrl = ConfigurationManager.AppSettings["WebSiteUrl"] + "&page=" + pageNumber;
            }

        }

        private static void DeleteTables()
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["AppConnString"].ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "usp_DeleteRanking";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();

                }
            }
        }

        private static void MapHtmlWithRanking(List<List<string>> table)
        {
            foreach(List<string> info in table)
            {
                Ranking r = new Ranking(info);
                SaveRanking(r);
            }
        }

        private static void SaveRanking(Ranking r)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["AppConnString"].ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "usp_InsertRanking";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PlayerId", r.PlayerId);
                    cmd.Parameters.AddWithValue("@Ranking", r.RankingPosition);
                    cmd.Parameters.AddWithValue("@PlayerName", r.PlayerName);
                    cmd.Parameters.AddWithValue("@Team", r.Team);
                    cmd.Parameters.AddWithValue("@NumberOfGames", r.NumberOfGames);
                    cmd.Parameters.AddWithValue("@TotalRating", r.TotalRating);
                    cmd.Parameters.AddWithValue("@AverageRating", r.AverageRating);
                    cmd.Parameters.AddWithValue("@AverageRatingPer40Mins", r.AverageRatingPer40Mins);
                    cmd.ExecuteNonQuery();

                }
            }
        }
    }
}
