using HarmonyLib;
using SongSelect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ScoreRanks.Patches
{
    internal class SongSelectScoreRankpatch
    {
        static Dictionary<Kanban, ButtonCrownObject> SongButtonObjects = new Dictionary<Kanban, ButtonCrownObject>();

        [HarmonyPatch(typeof(SongSelectManager))]
        [HarmonyPatch(nameof(SongSelectManager.Start))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void Start_Postfix(SongSelectManager __instance)
        {
            SpriteInitialization.InitializeDifficultySprites(__instance);
        }

        private static void ClearSongButtonDictionary()
        {
            foreach (var item in SongButtonObjects)
            {
                if (item.Key == null ||
                    item.Value == null)
                {
                    SongButtonObjects.Remove(item.Key);
                }
            }
        }

        [HarmonyPatch(typeof(SongSelectKanban))]
        [HarmonyPatch(nameof(SongSelectKanban.UpdateDisplay))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void SongSelectKanban_UpdateDisplay_Postfix(SongSelectKanban __instance, in SongSelectManager.Song song)
        {
            if (!SongButtonObjects.ContainsKey(KanbanUtility.KanbanToEnum(__instance)))
            {
                SongButtonObjects.Add(KanbanUtility.KanbanToEnum(__instance), new ButtonCrownObject(__instance));
            }

            var obj = SongButtonObjects[KanbanUtility.KanbanToEnum(__instance)];
            Plugin.Instance.StartCoroutine(obj.ChangeScoreRanks());
        }

        static SongSelectManager.KanbanMoveType prevMoveType = SongSelectManager.KanbanMoveType.Initialize;

        [HarmonyPatch(typeof(SongSelectManager))]
        [HarmonyPatch(nameof(SongSelectManager.PlayKanbanMoveAnim))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void SongSelectManager_PlayKanbanMoveAnim_Postfix(SongSelectManager __instance, SongSelectManager.KanbanMoveType moveType, SongSelectManager.KanbanMoveSpeed moveSpeed = SongSelectManager.KanbanMoveSpeed.Normal)
        {
            // The middle 3 Kanban are layed out like:
            // 2 (Top)    Index 1
            // 1 (Center) Index 0
            // 3 (Bottom) Index 2
            // They then alternate between top and bottom, where evens go on top, odds on bottom
            // But we only really care about the middle 3
            // Actually, we only care about the center one

            if (prevMoveType == SongSelectManager.KanbanMoveType.MoveEnded &&
                (moveType == SongSelectManager.KanbanMoveType.MoveUp || moveType == SongSelectManager.KanbanMoveType.MoveDown))
            {
                if (moveType == SongSelectManager.KanbanMoveType.MoveUp)
                {
                    SongButtonObjects[KanbanUtility.KanbanToEnum(__instance.kanbans[0])].ShrinkCrowns();
                }
                else
                {
                    SongButtonObjects[KanbanUtility.KanbanToEnum(__instance.kanbans[0])].ShrinkCrownsImmediate();
                    SongButtonObjects[KanbanUtility.KanbanToEnum(__instance.kanbans[1])].ExpandCrownsImmediate();
                    SongButtonObjects[KanbanUtility.KanbanToEnum(__instance.kanbans[1])].ShrinkCrowns();
                }
            }
            else if ((prevMoveType == SongSelectManager.KanbanMoveType.MoveUp || prevMoveType == SongSelectManager.KanbanMoveType.MoveDown) &&
                    moveType == SongSelectManager.KanbanMoveType.MoveEnded)
            {
                SongButtonObjects[KanbanUtility.KanbanToEnum(__instance.kanbans[0])].ExpandCrowns();
            }

            prevMoveType = moveType;
            if (moveType == SongSelectManager.KanbanMoveType.Initialize)
            {
                prevMoveType = SongSelectManager.KanbanMoveType.MoveEnded;
            }
        }
    }
}
