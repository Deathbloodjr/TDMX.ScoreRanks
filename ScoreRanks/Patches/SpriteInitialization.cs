using SongSelect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ScoreRanks.Patches
{
    internal class SpriteInitialization
    {
        public static Dictionary<EnsoData.EnsoLevelType, Sprite> DifficultySprites = new Dictionary<EnsoData.EnsoLevelType, Sprite>();

        public static bool IsInitialized()
        {
            ClearNullsFromDiffDictionary();

            // This is a pretty bad check I think
            if (DifficultySprites.Count > 0)
            {
                return true;
            }
            return false;
        }
       
        public static void InitializeDifficultySprites(SongSelectManager songSelectManager)
        {
            ModLogger.Log("InitializeDifficultySprites");
            InitializeDifficultySprites(songSelectManager.songFilterSetting);
        }

        public static void InitializeDifficultySprites(SongFilterSetting songFilterSetting)
        {
            ClearNullsFromDiffDictionary();

            if (songFilterSetting is null)
            {
                return;
            }

            // Check if everything's already initialized
            if (DifficultySprites.ContainsKey(EnsoData.EnsoLevelType.Easy) &&
                DifficultySprites.ContainsKey(EnsoData.EnsoLevelType.Normal) &&
                DifficultySprites.ContainsKey(EnsoData.EnsoLevelType.Hard) &&
                DifficultySprites.ContainsKey(EnsoData.EnsoLevelType.Mania) &&
                DifficultySprites.ContainsKey(EnsoData.EnsoLevelType.Ura))
            {
                return;
            }

            for (EnsoData.EnsoLevelType i = 0; i < EnsoData.EnsoLevelType.Num; i++)
            {
                DifficultySprites.Add(i, songFilterSetting.difficultyIconSprites[(int)i]);
            }
        }

        private static void ClearNullsFromDiffDictionary()
        {
            foreach (var item in DifficultySprites)
            {
                if (item.Value == null)
                {
                    DifficultySprites.Remove(item.Key);
                }
            }
        }
    }
}
