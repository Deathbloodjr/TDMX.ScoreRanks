using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ScoreRanks.Patches
{
    enum ScoreRankSpriteVersion
    {
        Big,
        Small,
    }
    enum ScoreRank
    {
        None,
        WhiteIki,
        BronzeIki,
        SilverIki,
        GoldMiyabi,
        PinkMiyabi,
        PurpleMiyabi,
        Kiwami,
        Num,
    }

    internal class ScoreRankUtility
    {
        public static void InitializeSprites()
        {
            for (ScoreRank i = ScoreRank.WhiteIki; i < ScoreRank.Num; i++)
            {
                AssetUtility.LoadSprite(GetSpriteFilePath(i, ScoreRankSpriteVersion.Big));
                AssetUtility.LoadSprite(GetSpriteFilePath(i, ScoreRankSpriteVersion.Small));
            }
        }

        public static string GetSpriteFilePath(ScoreRank rank, ScoreRankSpriteVersion version)
        {

            return Path.Combine(Plugin.Instance.ConfigScoreRankAssetFolderPath.Value, version.ToString(), rank.ToString() + ".png");
        }

        public static Sprite GetSprite(ScoreRank rank, ScoreRankSpriteVersion version)
        {
            return AssetUtility.LoadSprite(GetSpriteFilePath(rank, version));
        }

        public static ScoreRank GetScoreRank(ScoreRankPlayerData player)
        {
            return GetScoreRank(player.CurrentScore, player.ScoreRankValue);
        }
       
        public static ScoreRank GetScoreRank(int score, int maxScore)
        {
            var ratio = (float)score / (float)maxScore;
            if (ratio >= 1f)
            {
                return ScoreRank.Kiwami;
            }
            else if (ratio >= 0.95f)
            {
                return ScoreRank.PurpleMiyabi;
            }
            else if (ratio >= 0.9f)
            {
                return ScoreRank.PinkMiyabi;
            }
            else if (ratio >= 0.8f)
            {
                return ScoreRank.GoldMiyabi;
            }
            else if (ratio >= 0.7f)
            {
                return ScoreRank.SilverIki;
            }
            else if (ratio >= 0.6f)
            {
                return ScoreRank.BronzeIki;
            }
            else if (ratio >= 0.5f)
            {
                return ScoreRank.WhiteIki;
            }
            else
            {
                return ScoreRank.None;
            }
        }
    }
}
