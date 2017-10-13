using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EuroleagueParser
{
    public class Ranking
    {
        public Ranking(List<string> info)
        {
            PlayerId = info[1].Split(',')[1];
            RankingPosition = Int32.Parse(info[0]);
            PlayerName = info[1].Split(',')[1];
            Team = info[2];
            NumberOfGames = Int32.Parse(info[3]);
            TotalRating = Decimal.Parse(info[4]);
            AverageRating = Decimal.Parse(info[5]);
            AverageRatingPer40Mins = Decimal.Parse(info[6]);
        }
        public string PlayerId { get; set; }
        public int RankingPosition { get; set; }
        public string PlayerName { get; set; }
        public string Team { get; set; }
        public int NumberOfGames { get; set; }
        public decimal TotalRating { get; set; }
        public decimal AverageRating { get; set; }
        public decimal AverageRatingPer40Mins { get; set; }
    }
}
