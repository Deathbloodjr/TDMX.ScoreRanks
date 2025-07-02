using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreRanks.Patches
{
    internal class ScoreRankPlayerData
    {
        public int CurrentScore = 0;
        public int ScoreRankValue = -1;
        public ScoreRank CurrentRank = ScoreRank.None;

        public void Reset()
        {
            CurrentScore = 0;
            CurrentRank = ScoreRank.None;
        }

        public void Reset(int newScoreRankValue)
        {
            Reset();
            ScoreRankValue = newScoreRankValue;
        }
    }
}
