using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ScoreRanks.Patches
{
    internal class ScoreRankPatch
    {
        static ScoreRankPlayerData Player1 = new ScoreRankPlayerData();
        static ScoreRankPlayerData Player2 = new ScoreRankPlayerData();

        [HarmonyPatch(typeof(EnsoGameManager))]
        [HarmonyPatch(nameof(EnsoGameManager.Start))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void EnsoGameManager_Start_Postfix(EnsoGameManager __instance)
        {
            var musicInfo = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.MusicData.GetInfoById(__instance.settings.musicuid);
            Player1.ScoreRankValue = musicInfo.Scores[(int)__instance.settings.ensoPlayerSettings[0].courseType];
            Player2.ScoreRankValue = musicInfo.Scores[(int)__instance.settings.ensoPlayerSettings[1].courseType];
            ResetScore();
        }


        [HarmonyPatch(typeof(ScorePlayer))]
        [HarmonyPatch(nameof(ScorePlayer.SetAddScorePool))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void ScorePlayer_SetAddScorePool_Postfix(ScorePlayer __instance, int score)
        {
            ScoreRankPlayerData curPlayer = __instance.playerNo == 0 ? Player1 : Player2;

            curPlayer.CurrentScore += score;
            var newRank = ScoreRankUtility.GetScoreRank(curPlayer);
            if (newRank != curPlayer.CurrentRank)
            {
                curPlayer.CurrentRank = newRank;
                ModLogger.Log("P" + (__instance.playerNo + 1) + "ScoreRank: " + curPlayer.CurrentRank.ToString());
                CreateEnsoScoreRankIcon(curPlayer.CurrentRank, __instance.playerNo);
            }

            return;
        }

        [HarmonyPatch(typeof(EnsoPauseMenu))]
        [HarmonyPatch(nameof(EnsoPauseMenu.OnRestartClicked))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void EnsoPauseMenu_OnRestartClicked_Prefix(EnsoPauseMenu __instance)
        {
            ResetScore();
        }

        [HarmonyPatch(typeof(EnsoPauseMenu))]
        [HarmonyPatch(nameof(EnsoPauseMenu.OnReturnClicked))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void EnsoPauseMenu_OnReturnClicked_Postfix(EnsoPauseMenu __instance)
        {
            ResetScore();
        }

        [HarmonyPatch(typeof(EnsoPauseMenu))]
        [HarmonyPatch(nameof(EnsoPauseMenu.OnButtonModeClicked))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void EnsoPauseMenu_OnButtonModeClicked_Postfix(EnsoPauseMenu __instance)
        {
            ResetScore();
        }

        [HarmonyPatch(typeof(ResultPlayer))]
        [HarmonyPatch(nameof(ResultPlayer.DisplayScore))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void ResultPlayer_DisplayScore_Postfix(ResultPlayer __instance)
        {
            var parent = GameObject.Find("BaseMain");

            // This has ScoreRanks appearing at the same position for Player 1 and Player 2
            // That's bad for GhostBattle
            Vector2 DesiredPosition = new Vector2(-231, -35);
            Vector2 RealPosition = new Vector2(DesiredPosition.x + 868, DesiredPosition.y + 224);

            ScoreRankPlayerData curPlayer = __instance.playerNo == 0 ? Player1 : Player2;

            var imageObj = AssetUtility.CreateImageChild(parent, "ScoreRankResult", RealPosition, ScoreRankUtility.GetSpriteFilePath(curPlayer.CurrentRank, ScoreRankSpriteVersion.Small));
            var image = imageObj.GetComponent<Image>();
            var imageColor = image.color;
            imageColor.a = 0;
            image.color = imageColor;
            Plugin.Instance.StartCoroutine(AssetUtility.ChangeTransparencyOverSeconds(imageObj, 1, true));

            //DesiredPosition = new Vector2(-442, 190);
            //RealPosition = new Vector2(DesiredPosition.x + 868, DesiredPosition.y + 224);

            //var imageObj2 = AssetUtility.CreateImageChild(parent, "ScoreRankResult", RealPosition, Path.Combine(Plugin.Instance.ConfigScoreRankAssetFolderPath.Value, "Big", currentP1Rank.ToString() + ".png"));
            //var image2 = imageObj2.GetComponent<Image>();
            //var imageColor2 = image2.color;
            //imageColor2.a = 0;
            //image2.color = imageColor2;

            //Plugin.Instance.StartCoroutine(AssetUtility.ChangeTransparencyOverSeconds(imageObj2, 1, true));
        }


        public static void ResetScore()
        {
            Player1.Reset();
            Player2.Reset();
        }

        public static void CreateEnsoScoreRankIcon(ScoreRank scoreRank, int playerNo)
        {
            // I'd like to change this to use CustomGameMode with some new stored data in it
            // Make it more generic
            // Actually I still don't know how I want to do this
            // I know I have some mods that I'd want to have this mod disabled in
            // And other mods that I'd want this mod to be enabled in
            // So should I have something in those mods that tells ScoreRanks to be disabled?
            // That'd make more sense than having it in ScoreRanks checking if it's in a different mod and should be disabled
            // I'm not sure how to get that to work yet though
            if (GameObject.Find("DaniDojo") == null)
            {
                Plugin.Instance.StartCoroutine(CreateEnsoScoreRankAnimation(scoreRank, playerNo));
            }
        }

        public static IEnumerator CreateEnsoScoreRankAnimation(ScoreRank scoreRank, int playerNo)
        {
            var canvasFgObject = GameObject.Find("CanvasFg");

            Vector2 StartPosition = Vector2.zero;
            Vector2 MainPosition = Vector2.zero;
            Vector2 EndPosition = Vector2.zero;
            if (playerNo == 0)
            {
                float x = -905;
                float y = 305;
                MainPosition = GetScoreRankPosition(x, y);
                StartPosition = GetScoreRankPosition(x, y - 50);
                EndPosition = GetScoreRankPosition(x, y + 50);
            }
            else if (playerNo == 1)
            {
                float x = -905;
                float y = -500;
                MainPosition = GetScoreRankPosition(x, y);
                StartPosition = GetScoreRankPosition(x, y + 50);
                EndPosition = GetScoreRankPosition(x, y - 50);
            }

            // This position is changed at runtime, but the desired location is -920, 300
            // Adding 1920/2 or 1080/2 will put it at that location
            var scoreRankObject = AssetUtility.CreateImageChild(canvasFgObject, "ScoreRank", StartPosition, Path.Combine(Plugin.Instance.ConfigScoreRankAssetFolderPath.Value, "Big", scoreRank.ToString() + ".png"));
            var image = scoreRankObject.GetOrAddComponent<Image>();
            var imageColor = image.color;
            imageColor.a = 0;
            image.color = imageColor;

            Plugin.Instance.StartCoroutine(AssetUtility.MoveAnchoredPositionOverSeconds(scoreRankObject, MainPosition, 0.25f));
            Plugin.Instance.StartCoroutine(AssetUtility.ChangeTransparencyOverSeconds(scoreRankObject, 0.25f, true));
            yield return new WaitForSeconds(0.25f);

            // Grow and shrink over 200 ms?

            yield return new WaitForSeconds(0.2f);

            // Wait 2 seconds before moving up and disappearing
            yield return new WaitForSeconds(2);

            Plugin.Instance.StartCoroutine(AssetUtility.MoveAnchoredPositionOverSeconds(scoreRankObject, EndPosition, 0.25f));
            Plugin.Instance.StartCoroutine(AssetUtility.ChangeTransparencyOverSeconds(scoreRankObject, 0.25f, false));
            yield return new WaitForSeconds(0.5f);

            //GameObject.Destroy(scoreRankObject);

        }

        private static Vector2 GetScoreRankPosition(float x, float y)
        {
            return new Vector2(x + (1920 / 2), y + (1080 / 2));
        }
        private static Vector2 GetScoreRankPosition(Vector2 input)
        {
            return GetScoreRankPosition(input.x, input.y);
        }
    }
}
